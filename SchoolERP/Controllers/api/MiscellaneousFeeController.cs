using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/MiscellaneousFee")]
    public class MiscellaneousFeeController : ApiController
    {
        [HttpGet]
        [Route("checkAdmissionNo")]
        public ExpandoObject CheckAdmissionNo(string AdmissionNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                MiscellaneousModel miscellaneous = null;
                if (AdmissionNo != null)
                    AdmissionNo = AdmissionNo.ToLower();
                if (dataContext.Students.Where(x => x.AdmissionNo == AdmissionNo).Any())  //Check If Admission
                {
                    miscellaneous = (from a1 in dataContext.Admissions
                                     join s1 in dataContext.Students
                                     on a1.StudentId equals s1.StudentId
                                     join s2 in dataContext.Sections
                                     on a1.SectionId equals s2.SectionId into subSection
                                     from sSection in subSection.DefaultIfEmpty()
                                     join c1 in dataContext.Classes
                                     on sSection.ClassId equals c1.ClassId into subClass
                                     from sClass in subClass.DefaultIfEmpty()
                                     where a1.AdmissionStatus == (byte)AdmissionStatus.Active
                                     && s1.AdmissionNo.ToLower() == AdmissionNo
                                     select new MiscellaneousModel
                                     {
                                         AdmissionId = a1.AdmissionId,
                                         AdmissionNo = s1.AdmissionNo,
                                         ClassId = sClass.ClassId,
                                         ClassName = sClass.ClassName,
                                         MobileNo = s1.MobileNo,
                                         SectionId = sSection.SectionId,
                                         SectionName = sSection.SectionName,
                                         FullName = s1.FullName,
                                         FatherName = s1.FatherName,
                                         AcademicSessionId = a1.AcademicSessionId.Value,
                                         FeeCollectionList = (from fc in dataContext.FeeCollections
                                                              where fc.AdmissionId == a1.AdmissionId
                                                              && fc.FeeCollectionStatus != (byte)FeeCollectionStatus.Cancelled
                                                              orderby fc.PaymentDate descending
                                                              select new FeeCollectionModel
                                                              {
                                                                  PaymentDate = fc.PaymentDate,
                                                                  PaymentMode = fc.PaymentMode,
                                                                  PaidAmount = fc.PaidAmount,
                                                                  ReceiptNo = fc.ReceiptNo,
                                                                  Remarks = fc.Remarks,
                                                                  FeeCollectionId = fc.FeeCollectionId
                                                              }).ToList()
                                     }).First();

                }
                else if (dataContext.Miscellaneous.Where(x => x.AdmissionNo == AdmissionNo).Any()) //Check Miscellaneous
                {
                    miscellaneous = (from m1 in dataContext.Miscellaneous
                                     join s2 in dataContext.Sections
                                     on m1.SectionId equals s2.SectionId into subSection
                                     from sSection in subSection.DefaultIfEmpty()
                                     join c1 in dataContext.Classes
                                     on sSection.ClassId equals c1.ClassId into subClass
                                     from sClass in subClass.DefaultIfEmpty()
                                     where m1.AdmissionNo.ToLower() == AdmissionNo
                                     select new MiscellaneousModel
                                     {
                                         MiscellaneousId = m1.MiscellaneousId,
                                         ClassId = sClass.ClassId,
                                         AdmissionNo = m1.AdmissionNo,
                                         ClassName = sClass.ClassName,
                                         MobileNo = m1.MobileNo,
                                         SectionId = sSection.SectionId,
                                         SectionName = sSection.SectionName,
                                         FullName = m1.FullName,
                                         PaymentCategory = m1.PaymentCategory,
                                         FatherName = m1.FatherName,
                                         SchoolId = m1.SchoolId,
                                         AcademicSessionId = m1.AcademicSessionId,
                                         FeeCollectionList = (from fc in dataContext.FeeCollections
                                                              where fc.MisscellaneousId == m1.MiscellaneousId
                                                              orderby fc.PaymentDate descending
                                                              select new FeeCollectionModel
                                                              {
                                                                  PaymentDate = fc.PaymentDate,
                                                                  PaymentMode = fc.PaymentMode,
                                                                  PaidAmount = fc.PaidAmount,
                                                                  ReceiptNo = fc.ReceiptNo,
                                                                  Remarks = fc.Remarks,
                                                                  FeeCollectionId = fc.FeeCollectionId,
                                                                  AdmissionId = fc.AdmissionId,
                                                                  MisscellaneousId = fc.MisscellaneousId
                                                              }).ToList()
                                     }).First();
                }
                else
                {
                    miscellaneous = new MiscellaneousModel();
                }
                response.Miscellaneous = miscellaneous;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveMiscellaneousFee")]
        public ExpandoObject SaveMiscellaneous(SaveMiscellaneousFeeModel saveMiscellaneous)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            int y = 0;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                int AcademicSessionId = 0;
                var sessions = dataContext.AcademicSessions.Where(x => x.SessionStartDate <= saveMiscellaneous.FeeCollection.PaymentDate.Date && x.SessionStartDate.AddMonths(12) >= saveMiscellaneous.FeeCollection.PaymentDate.AddDays(1).Date);
                if (sessions.Any())
                    AcademicSessionId = sessions.OrderByDescending(x => x.SessionStartDate).First().AcademicSessionId;
                else
                    throw new Exception("Session for this payment date is not available.");

                //If New AdmissionNo
                var MiscellaneousModel = saveMiscellaneous.Miscellaneous;
                if (MiscellaneousModel.MiscellaneousId == 0 && MiscellaneousModel.AdmissionId == 0)
                {
                    Miscellaneous Miscellaneous = new Miscellaneous
                    {
                        FatherName = MiscellaneousModel.FatherName,
                        MobileNo = MiscellaneousModel.MobileNo,
                        AdmissionNo = MiscellaneousModel.AdmissionNo,
                        PaymentCategory = MiscellaneousModel.PaymentCategory,
                        SectionId = MiscellaneousModel.SectionId,
                        FullName = MiscellaneousModel.FullName,
                        AcademicSessionId = AcademicSessionId,
                        SchoolId = 1
                    };
                    dataContext.Miscellaneous.InsertOnSubmit(Miscellaneous);
                    dataContext.SubmitChanges();
                    MiscellaneousModel.MiscellaneousId = Miscellaneous.MiscellaneousId;
                    MiscellaneousModel.AcademicSessionId = Miscellaneous.AcademicSessionId;
                }

                //Fee Collection
                var feeCollectionModel = saveMiscellaneous.FeeCollection;
                PaymentMode paymentMode = (PaymentMode)Convert.ToByte(feeCollectionModel.PaymentMode);
                decimal LineTotal = saveMiscellaneous.FeeHeadList.Sum(x => x.Amount);
                FeeCollection feeCollection = new FeeCollection
                {
                    ReceiptNo = FeeData.GenerateReceiptNo(dataContext),
                    PaymentDate = DateTime.Now,
                    PaymentMode = feeCollectionModel.PaymentMode,
                    SchoolBankId = null,
                    PaymentFor = (byte)PaymentFor.Miscellaneous,
                    Remarks = feeCollectionModel.Remarks,
                    RemarksForConcession = null,
                    CreatedBy = SessionUtilies.GetLoginId(),
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now,
                    LastUpdatedBy = SessionUtilies.GetLoginId(),
                    FeeCollectionStatus = (byte)FeeCollectionStatus.Paid,
                    PreviousDues = 0,
                    DuesAmount = 0,
                    LineTotal = LineTotal,
                    Concession = 0,
                    Subtotal = LineTotal,
                    PaidAmount = LineTotal,
                    AcademicSessionId = AcademicSessionId
                };
                if (MiscellaneousModel.AdmissionId > 0)
                    feeCollection.AdmissionId = MiscellaneousModel.AdmissionId;
                else
                    feeCollection.MisscellaneousId = MiscellaneousModel.MiscellaneousId;
                if (paymentMode == PaymentMode.Cheque)
                {
                    feeCollection.ChequeNo = feeCollectionModel.ChequeNo;
                    feeCollection.ChequeDate = feeCollectionModel.ChequeDate;
                    feeCollection.ChequeBank = feeCollectionModel.ChequeBank;
                    feeCollection.ChequeBankBranch = feeCollectionModel.ChequeBankBranch;
                }
                //else if (paymentMode == PaymentMode.DemandDraft)
                //{
                //    feeCollection.DemandDraftNo = feeCollectionModel.DemandDraftNo;
                //    feeCollection.DemandDraftDate = feeCollectionModel.DemandDraftDate;
                //    feeCollection.DemandDraftBank = feeCollectionModel.DemandDraftBank;
                //}
                else if (paymentMode == PaymentMode.UPI || paymentMode == PaymentMode.Online || paymentMode == PaymentMode.QRCode)
                {
                    feeCollection.TransactionNo = feeCollectionModel.TransactionNo;
                }
                dataContext.FeeCollections.InsertOnSubmit(feeCollection);
                dataContext.SubmitChanges();

                //Fee Collection detail
                saveMiscellaneous.FeeHeadList.ForEach(feeHead =>
                {
                    FeeCollectionDetail feeCollectionDetail = new FeeCollectionDetail
                    {
                        FeeCollectionId = feeCollection.FeeCollectionId,
                        FeeHeadId = feeHead.FeeHeadId,
                        IsIncluded = false,
                        FeeHeadTitle = feeHead.FeeHeadTitle,
                        FeeType = (byte)FeeType.Miscellaneous,
                        Amount = feeHead.Amount,
                        Concession = 0,
                        Total = feeHead.Amount,
                        Comment = null,
                        LastUpdatedOn = DateTime.Now,
                        LastUpdatedBy = SessionUtilies.GetLoginId()
                    };
                    dataContext.FeeCollectionDetails.InsertOnSubmit(feeCollectionDetail);
                    dataContext.SubmitChanges();
                });

                transaction.Commit();
                y = 1;
                response.ReceiptNo = feeCollection.ReceiptNo;
                response.FeeCollectionId = feeCollection.FeeCollectionId;
                response.MiscellaneousId = feeCollection.MisscellaneousId;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (y != 1)
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
    }
}
