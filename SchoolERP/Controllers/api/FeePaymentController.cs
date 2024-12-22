using Newtonsoft.Json;
using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/feePayment")]
    public class FeePaymentController : ApiController
    {
        [HttpPost]
        [Route("saveFeePayment")]
        public ExpandoObject SaveFeePayment(SaveFeePaymentModel saveFeePaymentModel)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                var FeeCollectionModel = saveFeePaymentModel.FeeCollection;
                if (dataContext.Cashbooks.Any())
                {
                    DateTime lastCashbookDate = dataContext.Cashbooks.OrderByDescending(x => x.CashbookDate).First().CashbookDate;
                    if (lastCashbookDate.Date >= FeeCollectionModel.PaymentDate.Date)
                    {
                        response.Message = "Cashbook is completed,so you can not take fee";
                        return response;
                    }
                }
                var admission = (from a1 in dataContext.Admissions
                                 where a1.AdmissionId == saveFeePaymentModel.AdmissionId
                                 select a1).First();

                PaymentMode paymentMode = (PaymentMode)FeeCollectionModel.PaymentMode;
                var MonthFeeHeadList = saveFeePaymentModel.MonthFeeHeadList.Where(x => x.IsChecked && x.IsEnabled).ToList();
                FeeCollection feeCollection = new FeeCollection
                {
                    ReceiptNo = FeeData.GenerateReceiptNo(dataContext),
                    PaymentForMonths = string.Join(", ", MonthFeeHeadList.Select(x => x.MonthName).ToList()),
                    AdmissionId = admission.AdmissionId,
                    AcademicSessionId = admission.AcademicSessionId.Value,
                    //PaymentDate = FeeCollectionModel.PaymentDate,
                    PaymentDate = DateTime.Now,
                    PaymentMode = FeeCollectionModel.PaymentMode,
                    SchoolBankId = null,
                    PaymentFor = (byte)PaymentFor.Fees,
                    PreviousDues = FeeCollectionModel.PreviousDues,
                    LineTotal = FeeCollectionModel.LineTotal,
                    Concession = FeeCollectionModel.Concession,
                    Subtotal = FeeCollectionModel.Subtotal,
                    PaidAmount = FeeCollectionModel.PaidAmount,
                    DuesAmount = FeeCollectionModel.DuesAmount,
                    Remarks = FeeCollectionModel.Remarks,
                    RemarksForConcession = FeeCollectionModel.RemarksForConcession,
                    RemarksForStudent = FeeCollectionModel.RemarksForStudent,
                    CreatedBy = SessionUtilies.GetLoginId(),
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now,
                    LastUpdatedBy = SessionUtilies.GetLoginId(),
                    FeeCollectionStatus = (byte)FeeCollectionStatus.Paid
                };

                if (paymentMode == PaymentMode.Cheque)
                {
                    feeCollection.ChequeNo = FeeCollectionModel.ChequeNo;
                    feeCollection.ChequeDate = FeeCollectionModel.ChequeDate;
                    feeCollection.ChequeBank = FeeCollectionModel.ChequeBank;
                    feeCollection.ChequeBankBranch = FeeCollectionModel.ChequeBankBranch;
                }
                //else if (paymentMode == PaymentMode.DemandDraft)
                //{
                //    feeCollection.DemandDraftNo = FeeCollectionModel.DemandDraftNo;
                //    feeCollection.DemandDraftDate = FeeCollectionModel.DemandDraftDate;
                //    feeCollection.DemandDraftBank = FeeCollectionModel.DemandDraftBank;
                //}
                else if (paymentMode == PaymentMode.UPI || paymentMode == PaymentMode.Online || paymentMode == PaymentMode.QRCode)
                {
                    feeCollection.TransactionNo = FeeCollectionModel.TransactionNo;
                }

                dataContext.FeeCollections.InsertOnSubmit(feeCollection);
                dataContext.SubmitChanges();

                //----------------------------Class & Admission Fee--------------------------------------------------------
                var SelectedFeeHeadGroupList = saveFeePaymentModel.FeeHeadGroupList.Where(x => x.IsSelected && x.FeeAmount > 0).ToList();
                SelectedFeeHeadGroupList.ForEach(feeHeadGroup =>
                {
                    var feeHead = (from fh in dataContext.FeeHeads
                                   where fh.FeeHeadId == feeHeadGroup.FeeHeadId
                                   select fh).First();

                    FeeCollectionDetail feeCollectionDetail = new FeeCollectionDetail
                    {
                        FeeCollectionId = feeCollection.FeeCollectionId,
                        FeeHeadId = feeHead.FeeHeadId,
                        IsIncluded = true,
                        FeeHeadTitle = feeHead.FeeHeadTitle,
                        FeeType = (byte)feeHeadGroup.FeeType,
                        Amount = feeHeadGroup.FeeAmount,
                        Concession = feeHeadGroup.Concession,
                        Total = feeHeadGroup.LineTotal,
                        Comment = null,
                        LastUpdatedOn = DateTime.Now,
                        LastUpdatedBy = SessionUtilies.GetLoginId()
                    };

                    if (feeHeadGroup.IsFine)
                        feeCollectionDetail.IsIncluded = false;

                    if (feeHeadGroup.StudentChargeId > 0)
                        feeCollectionDetail.StudentChargeId = feeHeadGroup.StudentChargeId;

                    dataContext.FeeCollectionDetails.InsertOnSubmit(feeCollectionDetail);
                    dataContext.SubmitChanges();

                });

                if (!SelectedFeeHeadGroupList.Any() && !feeCollection.PreviousDues.HasValue)
                    throw new Exception("There is not particulars selected in Bill.");


                //----------------------Previous Dues---------------------------------------------
                if (feeCollection.PreviousDues.HasValue && feeCollection.PreviousDues.Value > 0)
                {

                    FeeCollectionDetail feeCollectionDetail = new FeeCollectionDetail
                    {
                        FeeCollectionId = feeCollection.FeeCollectionId,
                        FeeHeadId = 13,
                        IsIncluded = true,
                        FeeHeadTitle = "Previous Dues",
                        FeeType = (byte)FeeType.PreviousDues,
                        Amount = feeCollection.PreviousDues.Value,
                        Concession = 0,
                        Total = feeCollection.PreviousDues.Value,
                        Comment = null,
                        LastUpdatedOn = DateTime.Now,
                        LastUpdatedBy = SessionUtilies.GetLoginId()
                    };
                    dataContext.FeeCollectionDetails.InsertOnSubmit(feeCollectionDetail);
                    dataContext.SubmitChanges();
                }

                MonthFeeHeadList.ForEach(mn =>
                {
                    FeeCollectionMonth fcm = new FeeCollectionMonth
                    {
                        MonthNo = mn.MonthNo,
                        FeeCollectionId = feeCollection.FeeCollectionId,
                        AdmissionId = feeCollection.AdmissionId.Value
                    };
                    dataContext.FeeCollectionMonths.InsertOnSubmit(fcm);
                    dataContext.SubmitChanges();

                    //Student Concession
                    mn.FeeHeadDetails.Where(x => x.StudentConcessionId > 0).ToList().ForEach(feeHead =>
                      {
                          var studentConcession = dataContext.StudentConcessions.Where(x => x.StudentConcessionId == feeHead.StudentConcessionId).First();
                          studentConcession.StudentConcessionStatus = (byte)StudentConcessionStatus.Inactive;
                          dataContext.SubmitChanges();
                      });
                });
                string paymentMonths = "";
                saveFeePaymentModel.YearFeeList.ForEach(y1 =>
                {
                    y1.MonthFeeList.Where(x => x.IsChecked && x.IsEnabled).ToList().ForEach(mn =>
                      {
                          FeeCollectionMonthData fcm = new FeeCollectionMonthData
                          {
                              MonthNo = mn.MonthNo,
                              Year = mn.Year,
                              FeeCollectionId = feeCollection.FeeCollectionId,
                              AdmissionId = feeCollection.AdmissionId.Value
                          };
                          dataContext.FeeCollectionMonthDatas.InsertOnSubmit(fcm);
                          dataContext.SubmitChanges();

                          paymentMonths += (paymentMonths != "" ? ", " : "") + mn.MonthName + "-" + mn.Year.ToString().Substring(2);
                      });
                });

                feeCollection.PaymentMonths = paymentMonths;
                feeCollection.PaymentForMonths += (feeCollection.PaymentForMonths != null ? ", " : "") + paymentMonths; 
                dataContext.SubmitChanges();

                transaction.Commit();
                IsCompleted = true;
                response.FeeCollectionId = feeCollection.FeeCollectionId;
                response.ReceiptNo = feeCollection.ReceiptNo;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpPost]
        [Route("updateFeePayment")]
        public ExpandoObject UpdateFeePayment(FeeCollectionModel FeeCollectionModel)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                PaymentMode paymentMode = (PaymentMode)FeeCollectionModel.PaymentMode;
                FeeCollection feeCollection = dataContext.FeeCollections.Where(x => x.FeeCollectionId == FeeCollectionModel.FeeCollectionId).First();
                feeCollection.PaymentMode = FeeCollectionModel.PaymentMode;
                feeCollection.LineTotal = FeeCollectionModel.LineTotal;
                feeCollection.Concession = FeeCollectionModel.Concession;
                feeCollection.Subtotal = FeeCollectionModel.PaidAmount;
                feeCollection.PaidAmount = FeeCollectionModel.PaidAmount;
                feeCollection.DuesAmount = feeCollection.Subtotal - feeCollection.PaidAmount;
                feeCollection.Remarks = FeeCollectionModel.Remarks;
                feeCollection.RemarksForStudent = FeeCollectionModel.RemarksForStudent;
                feeCollection.LastUpdatedOn = DateTime.Now;
                feeCollection.LastUpdatedBy = SessionUtilies.GetLoginId();
                if (paymentMode == PaymentMode.Cheque)
                {
                    feeCollection.ChequeNo = FeeCollectionModel.ChequeNo;
                    feeCollection.ChequeDate = FeeCollectionModel.ChequeDate;
                    feeCollection.ChequeBank = FeeCollectionModel.ChequeBank;
                    feeCollection.ChequeBankBranch = FeeCollectionModel.ChequeBankBranch;
                }
                else if (paymentMode == PaymentMode.UPI || paymentMode == PaymentMode.Online || paymentMode == PaymentMode.QRCode)
                {
                    feeCollection.TransactionNo = FeeCollectionModel.TransactionNo;
                }

                dataContext.SubmitChanges();

                //----------------------------Class & Admission Fee--------------------------------------------------------
                FeeCollectionModel.FeeCollectionDetailList.ForEach(feeCollectionDetailModel =>
                {
                    FeeCollectionDetail feeCollectionDetail = dataContext.FeeCollectionDetails.Where(x => x.FeeCollectionDetailsId == feeCollectionDetailModel.FeeCollectionDetailsId).First();
                    feeCollectionDetail.Amount = feeCollectionDetailModel.Amount;
                    feeCollectionDetail.Concession = feeCollectionDetailModel.Concession;
                    feeCollectionDetail.Total = feeCollectionDetailModel.Total;
                    feeCollectionDetail.LastUpdatedOn = DateTime.Now;
                    feeCollectionDetail.LastUpdatedBy = SessionUtilies.GetLoginId();
                    dataContext.SubmitChanges();
                });

                transaction.Commit();
                IsCompleted = true;
                response.FeeCollectionId = feeCollection.FeeCollectionId;
                response.ReceiptNo = feeCollection.ReceiptNo;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpGet]
        [Route("feePaymentDetail")]
        public ExpandoObject FeePaymentDetail(int AdmissionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var FeePaymentDetail = FeeData.GetFeePaymentDetail(dataContext, AdmissionId);
                response.StudentChargeFeeHeadList = FeePaymentDetail.StudentChargeFeeHeadList;
                response.PreviousDueRemarks = FeePaymentDetail.PreviousDueRemarks;
                response.PaidFeeHeadList = FeePaymentDetail.PaidFeeHeadList;
                response.PreviousDueAmount = FeePaymentDetail.PreviousDuesAmount;
                response.AdmissionDetail = FeePaymentDetail.AdmissionDetail;
                response.MonthFeeList = FeePaymentDetail.MonthFeeList;
                response.YearFeeList = FeePaymentDetail.YearFeeList;
                response.TuitionFeeHeadId = ConfigurationManager.AppSettings["TuitionFeeHeadId"];
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("FeePaymentListByAdmissionId")]
        public ExpandoObject FeePaymentListByAdmissionId(int AdmissionId, int NoOfRecords)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var feeCollectionList = (from fc1 in dataContext.FeeCollections
                                         where fc1.AdmissionId == AdmissionId
                                         && fc1.FeeCollectionStatus != (byte)FeeCollectionStatus.Cancelled
                                         orderby fc1.FeeCollectionId descending
                                         select new FeeCollectionModel
                                         {
                                             FeeCollectionId = fc1.FeeCollectionId,
                                             ReceiptNo = fc1.ReceiptNo,
                                             PaymentDate = fc1.PaymentDate,
                                             OnlinePaymentDate = fc1.OnlinePaymentDate,
                                             LineTotal = fc1.LineTotal,
                                             PreviousDues = fc1.PreviousDues,
                                             PaymentForMonths = fc1.PaymentForMonths,
                                             Concession = fc1.Concession,
                                             Subtotal = fc1.Subtotal,
                                             PaidAmount = fc1.PaidAmount,
                                             DuesAmount = fc1.DuesAmount,
                                             PaymentMode = fc1.PaymentMode,
                                             MisscellaneousId = fc1.MisscellaneousId,
                                             Remarks = fc1.Remarks,
                                             RemarksForStudent = fc1.RemarksForStudent,
                                             IsToday = fc1.PaymentDate.Date == DateTime.Now.Date
                                         });
                if (NoOfRecords > 0)
                    response.FeeCollectionList = (feeCollectionList.Count() > NoOfRecords ? feeCollectionList.Take(NoOfRecords) : feeCollectionList).ToList();
                else
                    response.FeeCollectionList = feeCollectionList.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("FeePaymentListByStudentId")]
        public ExpandoObject FeePaymentListByStudentId(int StudentId, int NoOfRecords)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var feeCollectionList = (from fc1 in dataContext.FeeCollections
                                         join a1 in dataContext.Admissions
                                         on fc1.AdmissionId equals a1.AdmissionId
                                         join ss1 in dataContext.AcademicSessions
                                         on a1.AcademicSessionId equals ss1.AcademicSessionId
                                         where a1.StudentId == StudentId
                                         && fc1.FeeCollectionStatus != (byte)FeeCollectionStatus.Cancelled
                                         orderby fc1.FeeCollectionId descending
                                         select new
                                         {
                                             ss1.SessionName,
                                             FeeCollectionId = fc1.FeeCollectionId,
                                             ReceiptNo = fc1.ReceiptNo,
                                             PaymentDate = fc1.PaymentDate,
                                             OnlinePaymentDate = fc1.OnlinePaymentDate,
                                             LineTotal = fc1.LineTotal,
                                             PreviousDues = fc1.PreviousDues,
                                             PaymentForMonths = fc1.PaymentForMonths,
                                             Concession = fc1.Concession,
                                             Subtotal = fc1.Subtotal,
                                             PaidAmount = fc1.PaidAmount,
                                             DuesAmount = fc1.DuesAmount,
                                             PaymentMode = fc1.PaymentMode,
                                             MisscellaneousId = fc1.MisscellaneousId,
                                             Remarks = fc1.Remarks,
                                             RemarksForStudent = fc1.RemarksForStudent,
                                             IsToday = fc1.PaymentDate.Date == DateTime.Now.Date
                                         });
                if (NoOfRecords > 0)
                    response.FeeCollectionList = (feeCollectionList.Count() > NoOfRecords ? feeCollectionList.Take(NoOfRecords) : feeCollectionList).ToList();
                else
                    response.FeeCollectionList = feeCollectionList.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("FeePaymentCancle")]
        public ExpandoObject FeePaymentCancle(FeePaymentCancleModel feePayment)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;
                var feeCollection = dataContext.FeeCollections.Where(x => x.FeeCollectionId == feePayment.FeeCollectionId).First();
                feeCollection.FeeCollectionStatus = (byte)FeeCollectionStatus.Cancelled;
                feeCollection.LastUpdatedOn = DateTime.Now;
                feeCollection.LastUpdatedBy = SessionUtilies.GetLoginId();
                dataContext.SubmitChanges();

                StudentCharge studentCharge = studentCharge = new StudentCharge
                {
                    IsRefundable = false,
                    FeeType = (byte)FeeType.ExtraCharge,
                    StudentChargeStatus = (byte)StudentChargeStatus.Active,
                    AdmissionId = feeCollection.AdmissionId.Value,
                    FineAmount = feePayment.FineAmount,
                    FeeHeadId = feePayment.FeeHeadId,
                    Comments = feePayment.Comments,
                    FineDate = DateTime.Now
                };
                dataContext.StudentCharges.InsertOnSubmit(studentCharge);
                dataContext.SubmitChanges();

                transaction.Commit();
                IsCompleted = true;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpGet]
        [Route("FeeCollectionDetailByReceiptNo")]
        public ExpandoObject FeeCollectionDetailByReceiptNo(string ReceiptNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (!dataContext.FeeCollections.Where(x => x.ReceiptNo == ReceiptNo && x.FeeCollectionStatus != (byte)FeeCollectionStatus.Cancelled).Any())
                {
                    response.Message = "Invalid Receipt No";
                    return response;
                }

                var feeCollectionDetail = LoadData.GetFeeCollection(dataContext, ReceiptNo);

                if (dataContext.Cashbooks.Any() && feeCollectionDetail.PaymentMode != (byte)PaymentMode.Cheque)
                {
                    var lastCashbook = dataContext.Cashbooks.OrderByDescending(x => x.CashbookDate).First();
                    if (lastCashbook.CashbookDate.Date >= feeCollectionDetail.PaymentDate.Date)
                    {
                        response.Message = "Cashbook is aleady generated!!";
                        return response;
                    }
                }
                var studentDetail = (from s1 in dataContext.Students
                                     join a1 in dataContext.Admissions on s1.StudentId equals a1.StudentId
                                     where a1.AdmissionId == feeCollectionDetail.AdmissionId
                                     select new
                                     {
                                         s1.FullName,
                                         s1.AdmissionNo,
                                         s1.FatherName,
                                         a1.Section.SectionName,
                                         a1.Section.Class.ClassName,
                                         s1.MobileNo,
                                         a1.RollNo
                                     }).First();
                response.StudentDetail = studentDetail;
                response.FeeCollection = feeCollectionDetail;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("FeeCollectionForDelete")]
        public ExpandoObject FeeCollectionForDelete(string ReceiptNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (!dataContext.FeeCollections.Where(x => x.ReceiptNo == ReceiptNo).Any())
                {
                    response.Message = "Invalid Receipt No";
                    return response;
                }

                var FeeCollection = LoadData.GetFeeCollection(dataContext, ReceiptNo);

                var cashbooks = dataContext.Cashbooks.OrderByDescending(x => x.CashbookDate);
                if (cashbooks.Any())
                {
                    if (FeeCollection.PaymentDate.Date <= cashbooks.First().CashbookDate.Date)
                    {
                        response.Message = "Cashbook is already submitted, so you are not able to edit or delete this receipt.";
                        return response;
                    }
                }
                response.FeeCollection = FeeCollection;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("DeleteFeeCollection")]
        public ExpandoObject DeleteFeeCollection(int FeeCollectionId)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                DateTime PaymentDate = dataContext.FeeCollections.Where(x => x.FeeCollectionId == FeeCollectionId).First().PaymentDate;

                var cashbooks = dataContext.Cashbooks.OrderByDescending(x => x.CashbookDate);
                if (cashbooks.Any())
                {
                    if (PaymentDate.Date <= cashbooks.First().CashbookDate.Date)
                    {
                        response.Message = "Cashbook is already submitted, so you are not able to edit or delete this receipt.";
                        return response;
                    }
                }

                //if (dataContext.FeeCollections.OrderByDescending(x => x.FeeCollectionId).First().FeeCollectionId != FeeCollectionId)
                //{
                //    response.Message = "This is not last fee payment !!";
                //    return response;
                //}

                var feeCollectionMonth = dataContext.FeeCollectionMonths.Where(x => x.FeeCollectionId == FeeCollectionId);
                dataContext.FeeCollectionMonths.DeleteAllOnSubmit(feeCollectionMonth);
                dataContext.SubmitChanges();

                var feeCollectionDetails = dataContext.FeeCollectionDetails.Where(x => x.FeeCollectionId == FeeCollectionId);
                dataContext.FeeCollectionDetails.DeleteAllOnSubmit(feeCollectionDetails);
                dataContext.SubmitChanges();

                var feeCollection = dataContext.FeeCollections.Where(x => x.FeeCollectionId == FeeCollectionId).First();
                dataContext.FeeCollections.DeleteOnSubmit(feeCollection);
                dataContext.SubmitChanges();

                transaction.Commit();
                IsCompleted = true;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpGet]
        [Route("PendingFeePaymentTempList")]
        public ExpandoObject PendingFeePaymentTempList(DateTime FromDate, DateTime ToDate, int AdmissionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //Pending payments By AdmissionId
                var feeCollectionTempList = (from fc1 in dataContext.FeeCollectionTemps
                                             join a1 in dataContext.Admissions
                                             on fc1.AdmissionId equals a1.AdmissionId
                                             join s1 in dataContext.Students
                                             on a1.StudentId equals s1.StudentId
                                             join s2 in dataContext.Sections
                                             on a1.SectionId equals s2.SectionId into subSection
                                             from sSection in subSection.DefaultIfEmpty()
                                             join c1 in dataContext.Classes
                                             on sSection.ClassId equals c1.ClassId into subClass
                                             from sClass in subClass.DefaultIfEmpty()
                                             where fc1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending
                                             && (AdmissionId > 0 ? a1.AdmissionId == AdmissionId : (fc1.PaymentDate.Date >= FromDate.Date && fc1.PaymentDate.Date <= ToDate.Date))
                                             && (AdmissionId == 0 ? fc1.PaymentDate >= ConstantData.TrasactionIdStartDate : true)
                                             orderby fc1.FeeCollectionTempId descending
                                             select new FeeCollectionModel
                                             {
                                                 FeeCollectionTempId = fc1.FeeCollectionTempId,
                                                 PaymentDate = fc1.PaymentDate,
                                                 LineTotal = fc1.LineTotal,
                                                 PreviousDues = fc1.PreviousDues,
                                                 Concession = fc1.Concession,
                                                 Subtotal = fc1.Subtotal,
                                                 PaidAmount = fc1.PaidAmount,
                                                 DuesAmount = fc1.DuesAmount,
                                                 PaymentMode = fc1.PaymentMode,
                                                 MisscellaneousId = fc1.MisscellaneousId,
                                                 Remarks = fc1.Remarks,
                                                 TransactionNo = fc1.TransactionNo,
                                                 TransactionId = fc1.TransactionId,
                                                 StudentDetail = s1.AdmissionNo + " (" + s1.FullName + (s1.FatherName != null ? (", " + s1.FatherName) : "") + (sClass.ClassName != null ? (", " + sClass.ClassName) : "") + ")",
                                             }).ToList();
                if (AdmissionId == 0)
                {
                    //Pending payments By RegistrationOnline
                    var RegistrationOnlinePayments = (from fc1 in dataContext.FeeCollectionTemps
                                                      join s1 in dataContext.RegistrationOnlines
                                                      on fc1.RegistrationOnlineId equals s1.RegistrationOnlineId
                                                      join c1 in dataContext.Classes
                                                      on s1.ClassNo equals c1.ClassNo
                                                      where fc1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending
                                                      && fc1.PaymentDate.Date >= FromDate.Date
                                                      && fc1.PaymentDate.Date <= ToDate.Date
                                                      orderby fc1.FeeCollectionTempId descending
                                                      select new FeeCollectionModel
                                                      {
                                                          FeeCollectionTempId = fc1.FeeCollectionTempId,
                                                          PaymentDate = fc1.PaymentDate,
                                                          LineTotal = fc1.LineTotal,
                                                          PreviousDues = fc1.PreviousDues,
                                                          Concession = fc1.Concession,
                                                          Subtotal = fc1.Subtotal,
                                                          PaidAmount = fc1.PaidAmount,
                                                          DuesAmount = fc1.DuesAmount,
                                                          PaymentMode = fc1.PaymentMode,
                                                          MisscellaneousId = fc1.MisscellaneousId,
                                                          Remarks = fc1.Remarks,
                                                          TransactionNo = fc1.TransactionNo,
                                                          TransactionId = fc1.TransactionId,
                                                          StudentDetail = s1.RegistrationNo + " (" + s1.StudentName + (s1.FathersName != null ? (", " + s1.FathersName) : "") + (c1.ClassName2 != null ? (", " + c1.ClassName2) : "") + ")",
                                                      }).Distinct().ToList();
                    feeCollectionTempList.AddRange(RegistrationOnlinePayments);
                }
                response.FeeCollectionTempList = feeCollectionTempList;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("successTempPaymentList")]
        public ExpandoObject SuccessTempPaymentList(DateTime FromDate, DateTime ToDate, int AdmissionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //Pending payments By AdmissionId
                var feeCollectionTempList = (from fc1 in dataContext.FeeCollectionTemps
                                             join a1 in dataContext.Admissions
                                             on fc1.AdmissionId equals a1.AdmissionId
                                             join s1 in dataContext.Students
                                             on a1.StudentId equals s1.StudentId
                                             join s2 in dataContext.Sections
                                             on a1.SectionId equals s2.SectionId into subSection
                                             from sSection in subSection.DefaultIfEmpty()
                                             join c1 in dataContext.Classes
                                             on sSection.ClassId equals c1.ClassId into subClass
                                             from sClass in subClass.DefaultIfEmpty()
                                             where (fc1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending || fc1.FeeCollectionStatus == (byte)FeeCollectionStatus.Failed)
                                             && (AdmissionId > 0 ? a1.AdmissionId == AdmissionId : (fc1.PaymentDate.Date >= FromDate.Date && fc1.PaymentDate.Date <= ToDate.Date))
                                             && (AdmissionId == 0 ? fc1.PaymentDate >= ConstantData.TrasactionIdStartDate : true)
                                             orderby fc1.FeeCollectionTempId descending
                                             select new FeeCollectionModel
                                             {
                                                 FeeCollectionTempId = fc1.FeeCollectionTempId,
                                                 PaymentDate = fc1.PaymentDate,
                                                 LineTotal = fc1.LineTotal,
                                                 PreviousDues = fc1.PreviousDues,
                                                 Concession = fc1.Concession,
                                                 Subtotal = fc1.Subtotal,
                                                 PaidAmount = fc1.PaidAmount,
                                                 DuesAmount = fc1.DuesAmount,
                                                 PaymentMode = fc1.PaymentMode,
                                                 MisscellaneousId = fc1.MisscellaneousId,
                                                 Remarks = fc1.Remarks,
                                                 TransactionNo = fc1.TransactionNo,
                                                 TransactionId = fc1.TransactionId,
                                                 StudentDetail = s1.AdmissionNo + " (" + s1.FullName + (s1.FatherName != null ? (", " + s1.FatherName) : "") + (sClass.ClassName != null ? (", " + sClass.ClassName) : "") + ")",
                                                 FeeCollectionStatus = fc1.FeeCollectionStatus
                                             }).ToList();
                if (AdmissionId == 0)
                {
                    //Pending payments By RegistrationOnline
                    var RegistrationOnlinePayments = (from fc1 in dataContext.FeeCollectionTemps
                                                      join s1 in dataContext.RegistrationOnlines
                                                      on fc1.RegistrationOnlineId equals s1.RegistrationOnlineId
                                                      join c1 in dataContext.Classes
                                                      on s1.ClassNo equals c1.ClassNo
                                                      where (fc1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending || fc1.FeeCollectionStatus == (byte)FeeCollectionStatus.Failed)
                                                               && fc1.PaymentDate.Date >= FromDate.Date
                                                               && fc1.PaymentDate.Date <= ToDate.Date
                                                      orderby fc1.FeeCollectionTempId descending
                                                      select new FeeCollectionModel
                                                      {
                                                          FeeCollectionTempId = fc1.FeeCollectionTempId,
                                                          PaymentDate = fc1.PaymentDate,
                                                          LineTotal = fc1.LineTotal,
                                                          PreviousDues = fc1.PreviousDues,
                                                          Concession = fc1.Concession,
                                                          Subtotal = fc1.Subtotal,
                                                          PaidAmount = fc1.PaidAmount,
                                                          DuesAmount = fc1.DuesAmount,
                                                          PaymentMode = fc1.PaymentMode,
                                                          MisscellaneousId = fc1.MisscellaneousId,
                                                          Remarks = fc1.Remarks,
                                                          TransactionNo = fc1.TransactionNo,
                                                          TransactionId = fc1.TransactionId,
                                                          StudentDetail = s1.RegistrationNo + " (" + s1.StudentName + (s1.FathersName != null ? (", " + s1.FathersName) : "") + (c1.ClassName2 != null ? (", " + c1.ClassName2) : "") + ")",
                                                      }).Distinct().ToList();
                    feeCollectionTempList.AddRange(RegistrationOnlinePayments);
                }
                feeCollectionTempList = feeCollectionTempList.OrderBy(x => x.FeeCollectionTempId).ToList();
                int count = Convert.ToInt32(Math.Ceiling(feeCollectionTempList.Count() * 0.02));
                int LastFeeCollectionTempId = 0;
                for (var i = 0; i < count; i++)
                {
                    var feeCol = feeCollectionTempList.Where(x => x.FeeCollectionTempId > LastFeeCollectionTempId).Count() > 50 ? feeCollectionTempList.Where(x => x.FeeCollectionTempId > LastFeeCollectionTempId).Take(50) : feeCollectionTempList.Where(x => x.FeeCollectionTempId > LastFeeCollectionTempId);
                    List<string> transactionIds = feeCol.Select(x => x.TransactionId).ToList();
                    PayUResponseClass paymentResponses = PayUApi.CallStatusAPI(string.Join("|", transactionIds));
                    if (paymentResponses.status == 0)
                    {
                        paymentResponses.result.ForEach(firstData =>
                        {
                            string status = firstData.status.ToLower();
                            if (status == PayUMessages.Completed.ToLower() || status == PayUMessages.SettlementInProcess.ToLower() || status == PayUMessages.MoneySettled.ToLower() || status == PayUMessages.Success.ToLower() || status == PayUMessages.MoneyWithPayumoney.ToLower())
                            {
                                if (status == PayUMessages.MoneyWithPayumoney.ToLower() && firstData.amount != 0)
                                    feeCollectionTempList.Find(x => x.TransactionId == firstData.merchantTransactionId).FeeCollectionStatus = (byte)FeeCollectionStatus.Paid;
                                else
                                    feeCollectionTempList.Find(x => x.TransactionId == firstData.merchantTransactionId).FeeCollectionStatus = (byte)FeeCollectionStatus.Paid;
                            }
                        });
                    }
                    else
                    {
                        transactionIds.ForEach(transactionId =>
                        {
                            PayUResponseClass res2 = PayUApi.CallStatusAPI(transactionId);
                            if (res2.status == 0)
                            {
                                var firstData = res2.result.First();
                                string status = firstData.status.ToLower();
                                if (status == PayUMessages.Completed.ToLower() || status == PayUMessages.SettlementInProcess.ToLower() || status == PayUMessages.MoneySettled.ToLower() || status == PayUMessages.Success.ToLower() || status == PayUMessages.MoneyWithPayumoney.ToLower())
                                {
                                    if (status == PayUMessages.MoneyWithPayumoney.ToLower() && firstData.amount != 0)
                                        feeCollectionTempList.Find(x => x.TransactionId == firstData.merchantTransactionId).FeeCollectionStatus = (byte)FeeCollectionStatus.Paid;
                                    else
                                        feeCollectionTempList.Find(x => x.TransactionId == firstData.merchantTransactionId).FeeCollectionStatus = (byte)FeeCollectionStatus.Paid;
                                }
                            }
                        });
                    }
                    LastFeeCollectionTempId = feeCol.OrderByDescending(x => x.FeeCollectionTempId).First().FeeCollectionTempId;

                    //string merchantTransactionIds = string.Join("|", feeCol.Select(x => x.TransactionId));
                    //string URI = "https://www.payumoney.com/payment/payment/chkMerchantTxnStatus?";
                    //string myParameters = "merchantKey=" + ConfigurationManager.AppSettings["MerchantKey"] + "&merchantTransactionIds=" + merchantTransactionIds;
                    //using (WebClient wc = new WebClient())
                    //{
                    //    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    //    wc.Headers.Add("Authorization", ConfigurationManager.AppSettings["AuthHeader"]);
                    //    var HtmlResult = wc.UploadString(URI, myParameters);
                    //    paymentResponses = JsonConvert.DeserializeObject<PayUResponseClass>(HtmlResult);

                    //    //else
                    //    //    throw new Exception(paymentResponses.message);

                    //}
                }
                response.FeeCollectionTempList = feeCollectionTempList.Where(x => x.FeeCollectionStatus == (byte)FeeCollectionStatus.Paid).ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("CheckOnlineFeePaymentStatus")]
        public ExpandoObject CheckOnlineFeePaymentStatus(List<FeeCollectionModel> feeCollectionTempList)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;
                //feeCollectionTempList.OrderBy(x => x.FeeCollectionTempId).ToList().ForEach(fct1 =>
                //  {
                //      var feeCollectionTemps = (from f1 in dataContext.FeeCollectionTemps
                //                                where f1.FeeCollectionTempId == fct1.FeeCollectionTempId
                //                                && f1.PaymentDate >= ConstantData.TrasactionIdStartDate
                //                                && f1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending
                //                                select f1);
                //      if (feeCollectionTemps.Any())
                //      {
                //          var feeCollectionTemp = feeCollectionTemps.First();
                //          string URI = "https://www.payumoney.com/payment/payment/chkMerchantTxnStatus?";

                //          string myParameters = "merchantKey=" + ConfigurationManager.AppSettings["MerchantKey"] + "&merchantTransactionIds=" + feeCollectionTemp.TransactionId;
                //          using (WebClient wc = new WebClient())
                //          {
                //              wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                //              wc.Headers.Add("Authorization", ConfigurationManager.AppSettings["AuthHeader"]);
                //              var HtmlResult = wc.UploadString(URI, myParameters);
                //              var data = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(HtmlResult);
                //              if (data["status"] == 0)
                //              {
                //                  var firstData = data["result"][0];
                //                  string status = firstData["status"];
                //                  decimal amount = Convert.ToDecimal(firstData["amount"]);
                //                  status = status.ToLower();
                //                  if (status == PayUMessages.Completed.ToLower() || status == PayUMessages.SettlementInProcess.ToLower() || status == PayUMessages.MoneySettled.ToLower() || status == PayUMessages.Success.ToLower() || status == PayUMessages.MoneyWithPayumoney.ToLower())
                //                  {
                //                      if (status == PayUMessages.MoneyWithPayumoney.ToLower() && amount == 0)
                //                      {
                //                          feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Refunded;
                //                          dataContext.SubmitChanges();
                //                      }
                //                      else
                //                      {
                //                          string PaymentId = firstData["paymentId"];
                //                          int FeeCollectionId = LoadData.SaveFeePayment(dataContext, feeCollectionTemp, PaymentId);
                //                      }
                //                  }
                //                  else if (status == PayUMessages.Failed.ToLower() || status == PayUMessages.CancelledByUser.ToLower())
                //                  {
                //                      feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Failed;
                //                      dataContext.SubmitChanges();
                //                      response.Message = "Payment was failed.";
                //                  }
                //              }
                //          }
                //      }
                //  });

                feeCollectionTempList = feeCollectionTempList.OrderBy(x => x.FeeCollectionTempId).ToList();
                int count = Convert.ToInt32(Math.Ceiling(feeCollectionTempList.Count() * 0.02));
                int LastFeeCollectionTempId = 0;
                for (var i = 0; i < count; i++)
                {
                    var feeCol = feeCollectionTempList.Where(x => x.FeeCollectionTempId > LastFeeCollectionTempId).Count() > 50 ? feeCollectionTempList.Where(x => x.FeeCollectionTempId > LastFeeCollectionTempId).Take(50) : feeCollectionTempList.Where(x => x.FeeCollectionTempId > LastFeeCollectionTempId);

                    List<string> transactionIds = feeCol.Select(x => x.TransactionId).ToList();
                    PayUResponseClass paymentResponses = PayUApi.CallStatusAPI(string.Join("|", transactionIds));
                    if (paymentResponses.status == 0)
                    {
                        paymentResponses.result.ForEach(firstData =>
                        {
                            var feeCollectionTemps = (from f1 in dataContext.FeeCollectionTemps
                                                      where f1.TransactionId == firstData.merchantTransactionId
                                                      && f1.PaymentDate >= ConstantData.TrasactionIdStartDate
                                                      && (f1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending || f1.FeeCollectionStatus == (byte)FeeCollectionStatus.Failed)
                                                      select f1);
                            if (feeCollectionTemps.Any())
                            {
                                var feeCollectionTemp = feeCollectionTemps.First();
                                string status = firstData.status.ToLower();
                                if (status == PayUMessages.Completed.ToLower() || status == PayUMessages.SettlementInProcess.ToLower() || status == PayUMessages.MoneySettled.ToLower() || status == PayUMessages.Success.ToLower() || status == PayUMessages.MoneyWithPayumoney.ToLower())
                                {
                                    if (status == PayUMessages.MoneyWithPayumoney.ToLower() && firstData.amount == 0)
                                    {
                                        feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Refunded;
                                        dataContext.SubmitChanges();
                                    }
                                    else
                                    {
                                        string PaymentId = firstData.paymentId.ToString();
                                        int FeeCollectionId = LoadData.SaveFeePayment(dataContext, feeCollectionTemp, PaymentId);
                                    }
                                }
                                else if (status == PayUMessages.Failed.ToLower() || status == PayUMessages.CancelledByUser.ToLower())
                                {
                                    feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Failed;
                                    dataContext.SubmitChanges();
                                    response.Message = "Payment was failed.";
                                }
                            }
                        });
                    }
                    else
                    {
                        transactionIds.ForEach(transactionId =>
                        {
                            PayUResponseClass res2 = PayUApi.CallStatusAPI(transactionId);
                            if (res2.status == 0)
                            {
                                var firstData = res2.result.First();
                                var feeCollectionTemps = (from f1 in dataContext.FeeCollectionTemps
                                                          where f1.TransactionId == firstData.merchantTransactionId
                                                          && f1.PaymentDate >= ConstantData.TrasactionIdStartDate
                                                          && f1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending
                                                          select f1);
                                if (feeCollectionTemps.Any())
                                {
                                    var feeCollectionTemp = feeCollectionTemps.First();
                                    string status = firstData.status.ToLower();
                                    if (status == PayUMessages.Completed.ToLower() || status == PayUMessages.SettlementInProcess.ToLower() || status == PayUMessages.MoneySettled.ToLower() || status == PayUMessages.Success.ToLower() || status == PayUMessages.MoneyWithPayumoney.ToLower())
                                    {
                                        if (status == PayUMessages.MoneyWithPayumoney.ToLower() && firstData.amount == 0)
                                        {
                                            feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Refunded;
                                            dataContext.SubmitChanges();
                                        }
                                        else
                                        {
                                            string PaymentId = firstData.paymentId.ToString();
                                            int FeeCollectionId = LoadData.SaveFeePayment(dataContext, feeCollectionTemp, PaymentId);
                                        }
                                    }
                                    else if (status == PayUMessages.Failed.ToLower() || status == PayUMessages.CancelledByUser.ToLower())
                                    {
                                        feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Failed;
                                        dataContext.SubmitChanges();
                                        response.Message = "Payment was failed.";
                                    }
                                }
                            }
                        });
                    }
                    //string URI = "https://www.payumoney.com/payment/payment/chkMerchantTxnStatus?";
                    //string myParameters = "merchantKey=" + ConfigurationManager.AppSettings["MerchantKey"] + "&merchantTransactionIds=" + merchantTransactionIds;
                    //using (WebClient wc = new WebClient())
                    //{
                    //    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    //    wc.Headers.Add("Authorization", ConfigurationManager.AppSettings["AuthHeader"]);
                    //    var HtmlResult = wc.UploadString(URI, myParameters);
                    //    paymentResponses = JsonConvert.DeserializeObject<PayUResponseClass>(HtmlResult);
                    //    if (paymentResponses.status == 0)
                    //    {
                    //        paymentResponses.result.ForEach(firstData =>
                    //        {
                    //            var feeCollectionTemps = (from f1 in dataContext.FeeCollectionTemps
                    //                                      where f1.TransactionId == firstData.merchantTransactionId
                    //                                      && f1.PaymentDate >= ConstantData.TrasactionIdStartDate
                    //                                      && f1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending
                    //                                      select f1);
                    //            if (feeCollectionTemps.Any())
                    //            {
                    //                var feeCollectionTemp = feeCollectionTemps.First();
                    //                string status = firstData.status.ToLower();
                    //                if (status == PayUMessages.Completed.ToLower() || status == PayUMessages.SettlementInProcess.ToLower() || status == PayUMessages.MoneySettled.ToLower() || status == PayUMessages.Success.ToLower() || status == PayUMessages.MoneyWithPayumoney.ToLower())
                    //                {
                    //                    if (status == PayUMessages.MoneyWithPayumoney.ToLower() && firstData.amount == 0)
                    //                    {
                    //                        feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Refunded;
                    //                        dataContext.SubmitChanges();
                    //                    }
                    //                    else
                    //                    {
                    //                        string PaymentId = firstData.paymentId.ToString();
                    //                        int FeeCollectionId = LoadData.SaveFeePayment(dataContext, feeCollectionTemp, PaymentId);
                    //                    }
                    //                }
                    //                else if (status == PayUMessages.Failed.ToLower() || status == PayUMessages.CancelledByUser.ToLower())
                    //                {
                    //                    feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Failed;
                    //                    dataContext.SubmitChanges();
                    //                    response.Message = "Payment was failed.";
                    //                }
                    //            }
                    //        });
                    //    }
                    //    //else
                    //    //    throw new Exception(paymentResponses.message);

                    //}
                    LastFeeCollectionTempId = feeCol.OrderByDescending(x => x.FeeCollectionTempId).First().FeeCollectionTempId;
                }

                transaction.Commit();
                IsCompleted = true;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpPost]
        [Route("AutoUpdateFeePaymentStatus")]
        public ExpandoObject AutoUpdateFeePaymentStatus(FeeCollectionModel fct1)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;


                var feeCollectionTemps = (from f1 in dataContext.FeeCollectionTemps
                                          where f1.FeeCollectionTempId == fct1.FeeCollectionTempId
                                          && f1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending
                                          select f1);
                if (feeCollectionTemps.Any())
                {
                    var feeCollectionTemp = feeCollectionTemps.First();
                    string URI = "https://www.payumoney.com/payment/payment/chkMerchantTxnStatus?";

                    string myParameters = "merchantKey=" + ConfigurationManager.AppSettings["MerchantKey"] + "&merchantTransactionIds=" + feeCollectionTemp.TransactionId;
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        wc.Headers.Add("Authorization", ConfigurationManager.AppSettings["AuthHeader"]);
                        var HtmlResult = wc.UploadString(URI, myParameters);
                        var data = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(HtmlResult);
                        if (data["status"] == 0)
                        {
                            var firstData = data["result"][0];
                            string status = firstData["status"];
                            decimal amount = Convert.ToDecimal(firstData["amount"]);
                            status = status.ToLower();
                            if (status == PayUMessages.Completed.ToLower() || status == PayUMessages.SettlementInProcess.ToLower() || status == PayUMessages.MoneySettled.ToLower() || status == PayUMessages.Success.ToLower() || status == PayUMessages.MoneyWithPayumoney.ToLower())
                            {
                                if (status == PayUMessages.MoneyWithPayumoney.ToLower() && amount == 0)
                                {
                                    feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Refunded;
                                    dataContext.SubmitChanges();
                                    response.Message = "Payment was Refunded.";
                                }
                                else
                                {
                                    string PaymentId = firstData["paymentId"];
                                    int FeeCollectionId = LoadData.SaveFeePayment(dataContext, feeCollectionTemp, PaymentId);
                                    response.Message = ConstantData.SuccessMessage;
                                }
                            }
                            else if (status == PayUMessages.Failed.ToLower() || status == PayUMessages.CancelledByUser.ToLower())
                            {
                                feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Failed;
                                dataContext.SubmitChanges();
                                response.Message = "Payment was failed.";
                            }
                            else
                            {
                                response.Message = "Unknown payment response: " + status;
                            }
                        }
                        else
                        {
                            response.Message = "Invalid Payment Detail";
                        }

                    }
                }
                transaction.Commit();
                IsCompleted = true;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpPost]
        [Route("UpdateStudentFeePayment")]
        public ExpandoObject UpdateStudentFeePayment(UpdateFeePaymentStatusModel model)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;
                var feeCollectionTemps = (from f1 in dataContext.FeeCollectionTemps
                                          where f1.FeeCollectionTempId == model.FeeCollectionTempId
                                          && f1.FeeCollectionStatus == (byte)FeeCollectionStatus.Pending
                                          select f1);
                if (feeCollectionTemps.Any())
                {
                    var feeCollectionTemp = feeCollectionTemps.First();
                    if (model.Status == 1)
                    {
                        int FeeCollectionId = LoadData.SaveFeePayment(dataContext, feeCollectionTemp, "");
                        response.FeeCollectionId = FeeCollectionId;
                    }
                    else
                    {
                        feeCollectionTemp.FeeCollectionStatus = (byte)FeeCollectionStatus.Failed;
                        dataContext.SubmitChanges();
                    }
                    response.Message = ConstantData.SuccessMessage;
                }
                else
                    response.Message = "Invalid Detail";
                transaction.Commit();
                IsCompleted = true;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpGet]
        [Route("UpdateTransactionNo")]
        public ExpandoObject UpdateTransactionNo()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from f1 in dataContext.FeeCollections
                            join f2 in dataContext.FeeCollectionTemps
                            on f1.TransactionNo equals f2.FeeCollectionTempId.ToString()
                            where f1.PaymentMode == (byte)PaymentMode.Online
                            select new { f1.PaymentMode, f1.FeeCollectionId, f2.FeeCollectionTempId, f2.TransactionNo }).ToList();
                list.ForEach(fee =>
                {
                    var feeCollection = dataContext.FeeCollections.Where(x => x.FeeCollectionId == fee.FeeCollectionId).First();
                    feeCollection.FeeCollectionTempId = fee.FeeCollectionTempId;
                    feeCollection.TransactionNo = fee.TransactionNo;
                    dataContext.SubmitChanges();
                });
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
