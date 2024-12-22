using Microsoft.Ajax.Utilities;
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
    [RoutePrefix("api/StudentRegistration")]
    public class StudentRegistrationController : ApiController
    {
        [HttpPost]
        [Route("saveStudentRegistration")]
        public ExpandoObject SaveStudentRegistration(SaveStudentRegistrationModel saveStudentRegistration)
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

                //Student Registration
                var studentRegistrationModel = saveStudentRegistration.StudentRegistration;
                StudentRegistration studentRegistration = new StudentRegistration
                {
                    AadharNo = studentRegistrationModel.AadharNo,
                    AcademicSessionId = studentRegistrationModel.AcademicSessionId,
                    Address = studentRegistrationModel.Address,
                    PermanentCityId = studentRegistrationModel.PermanentCityId,
                    ClassId = studentRegistrationModel.ClassId,
                    DOB = studentRegistrationModel.DOB,
                    EmailId = studentRegistrationModel.EmailId,
                    FatherName = studentRegistrationModel.FatherName,
                    Gender = studentRegistrationModel.Gender,
                    MobileNo = studentRegistrationModel.MobileNo,
                    MotherName = studentRegistrationModel.MotherName,
                    Name = studentRegistrationModel.Name,
                    RegistrationDate = studentRegistrationModel.RegistrationDate,
                    RegistrationNo = LoadData.GenerateRegNo(dataContext, studentRegistrationModel.AcademicSessionId),
                    RegistrationStatus = (byte)RegistrationStatus.Active,
                    Remarks = studentRegistrationModel.Remarks,
                    StudentTypeId = studentRegistrationModel.StudentTypeId
                };

                dataContext.StudentRegistrations.InsertOnSubmit(studentRegistration);
                dataContext.SubmitChanges();

                //Fee Collection
                var feeCollectionModel = saveStudentRegistration.FeeCollection;
                PaymentMode paymentMode = (PaymentMode)Convert.ToByte(feeCollectionModel.PaymentMode);
                decimal LineTotal = saveStudentRegistration.RegistrationFeeList.Sum(x => x.Amount);
                FeeCollection feeCollection = new FeeCollection
                {
                    ReceiptNo = FeeData.GenerateReceiptNo(dataContext),
                    RegistrationId = studentRegistration.StudentRegistrationId,
                    AcademicSessionId = studentRegistration.AcademicSessionId,
                    PaymentDate = feeCollectionModel.PaymentDate,
                    PaymentMode = feeCollectionModel.PaymentMode,
                    SchoolBankId = null,
                    PaymentFor = (byte)PaymentFor.Registration,
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
                };
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
                else if (paymentMode == PaymentMode.UPI || paymentMode == PaymentMode.Online)
                {
                    feeCollection.TransactionNo = feeCollectionModel.TransactionNo;
                }
                dataContext.FeeCollections.InsertOnSubmit(feeCollection);
                dataContext.SubmitChanges();

                //Fee Collection detail
                saveStudentRegistration.RegistrationFeeList.ForEach(registrationFee =>
                {
                    var feeHead = (from fh in dataContext.FeeHeads
                                   where fh.FeeHeadId == registrationFee.FeeHeadId
                                   select fh).First();

                    FeeCollectionDetail feeCollectionDetail = new FeeCollectionDetail
                    {
                        FeeCollectionId = feeCollection.FeeCollectionId,
                        FeeHeadId = feeHead.FeeHeadId,
                        IsIncluded = false,
                        FeeHeadTitle = feeHead.FeeHeadTitle,
                        FeeType = (byte)FeeType.Registration,
                        Amount = registrationFee.Amount,
                        Concession = 0,
                        Total = registrationFee.Amount,
                        Comment = registrationFee.Remarks,
                        LastUpdatedOn = DateTime.Now,
                        LastUpdatedBy = SessionUtilies.GetLoginId()
                    };
                    dataContext.FeeCollectionDetails.InsertOnSubmit(feeCollectionDetail);
                    dataContext.SubmitChanges();

                });

                transaction.Commit();
                response.StudentRegistrationId = studentRegistration.StudentRegistrationId;
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

        [HttpGet]
        [Route("StudentRegistrationList")]
        public ExpandoObject StudentRegistrationList(int AcademicSessionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from s1 in dataContext.StudentRegistrations
                            join as1 in dataContext.AcademicSessions
                            on s1.AcademicSessionId equals as1.AcademicSessionId
                            join c1 in dataContext.Classes on s1.ClassId equals c1.ClassId
                            join sc1 in dataContext.StudentTypes
                            on s1.StudentTypeId equals sc1.StudentTypeId
                            where s1.AcademicSessionId == AcademicSessionId
                            orderby s1.RegistrationNo descending
                            select new
                            {
                                s1.StudentRegistrationId,
                                s1.RegistrationNo,
                                s1.Name,
                                s1.DOB,
                                s1.Gender,
                                s1.FatherName,
                                s1.MotherName,
                                s1.Address,
                                s1.EmailId,
                                s1.MobileNo,
                                s1.RegistrationDate,
                                s1.Remarks,
                                c1.ClassName,
                                sc1.TypeName,
                                as1.SessionName,
                                s1.RegistrationStatus,
                                s1.City.CityName,
                                s1.PermanentCityId,
                                s1.City.StateId,
                                s1.ClassId,
                                s1.StudentTypeId,
                                s1.AcademicSessionId,
                                s1.City.State.StateName,
                                s1.AadharNo,
                            });
                response.StudentRegistrationList = list.ToList();
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("StudentRegistrationListForSearch")]
        public ExpandoObject StudentRegistrationListForSearch(byte RegistrationStatus)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from s1 in dataContext.StudentRegistrations
                            join as1 in dataContext.AcademicSessions
                            on s1.AcademicSessionId equals as1.AcademicSessionId
                            join c1 in dataContext.Classes on s1.ClassId equals c1.ClassId
                            join sc1 in dataContext.StudentTypes
                            on s1.StudentTypeId equals sc1.StudentTypeId
                            where (RegistrationStatus > 0 ? s1.RegistrationStatus == RegistrationStatus : true)
                            orderby s1.RegistrationNo descending
                            select new
                            {
                                s1.StudentRegistrationId,
                                s1.RegistrationNo,
                                SearchField = s1.RegistrationNo + " (" + s1.Name + ")",
                                s1.Name,
                                s1.DOB,
                                s1.Gender,
                                s1.FatherName,
                                s1.MotherName,
                                s1.Address,
                                s1.EmailId,
                                s1.MobileNo,
                                s1.RegistrationDate,
                                s1.Remarks,
                                c1.ClassName,
                                sc1.TypeName,
                                as1.SessionName,
                                s1.RegistrationStatus,
                                s1.City.CityName,
                                s1.PermanentCityId,
                                s1.City.StateId,
                                s1.ClassId,
                                s1.StudentTypeId,
                                s1.AcademicSessionId,
                                s1.City.State.StateName,
                                s1.AadharNo,
                            });
                response.StudentRegistrationList = list.ToList();
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [Route("UpdateStudentRegistration")]
        public ExpandoObject UpdateStudentRegistration(StudentRegistrationModel studentRegistrationModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StudentRegistration studentRegistration = dataContext.StudentRegistrations.Where(x => x.StudentRegistrationId == studentRegistrationModel.StudentRegistrationId).First();
                studentRegistration.AadharNo = studentRegistrationModel.AadharNo;
                studentRegistration.AcademicSessionId = studentRegistrationModel.AcademicSessionId;
                studentRegistration.Address = studentRegistrationModel.Address;
                studentRegistration.PermanentCityId = studentRegistrationModel.PermanentCityId;
                studentRegistration.ClassId = studentRegistrationModel.ClassId;
                studentRegistration.DOB = studentRegistrationModel.DOB;
                studentRegistration.EmailId = studentRegistrationModel.EmailId;
                studentRegistration.FatherName = studentRegistrationModel.FatherName;
                studentRegistration.Gender = studentRegistrationModel.Gender;
                studentRegistration.MobileNo = studentRegistrationModel.MobileNo;
                studentRegistration.MotherName = studentRegistrationModel.MotherName;
                studentRegistration.Name = studentRegistrationModel.Name;
                studentRegistration.RegistrationDate = studentRegistrationModel.RegistrationDate;
                studentRegistration.Remarks = studentRegistrationModel.Remarks;
                studentRegistration.StudentTypeId = studentRegistrationModel.StudentTypeId;
                dataContext.SubmitChanges();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {

                response.Message = ex.Message;
            }

            return response;
        }

        //[HttpGet]
        //[Route("UpdateRegistrationNo")]
        //public ExpandoObject UpdateRegistrationNo()
        //{
        //    dynamic response = new ExpandoObject();
        //    try
        //    {
        //        SchoolDataContext dataContext = new SchoolDataContext();
        //        var academicSessionIds = dataContext.AcademicSessions.Select(x => x.AcademicSessionId).ToList();

        //        foreach (var aId in academicSessionIds)
        //        {
        //            int serialNo = 1;
        //            var classNo = (from ass in dataContext.AcademicSessions
        //                           join cr in dataContext.Classes
        //                           on ass.ClassId equals cr.ClassId
        //                           where ass.AcademicSessionId == aId
        //                           select cr.ClassNo).Distinct();
        //            var registrations = dataContext.StudentRegistrations.Where(x => x.AcademicSessionId == aId).OrderBy(x => x.StudentRegistrationId).ToList();
        //            foreach (var reg in registrations)
        //            {
        //                StudentRegistration studentRegistration = (from st in dataContext.StudentRegistrations
        //                                                           where st.StudentRegistrationId == reg.StudentRegistrationId
        //                                                           select st).First();
        //                string sessionName = dataContext.GetSessionForRegNo(aId);
        //                studentRegistration.RegistrationNo = "R" + classNo.First() + dataContext.GetSessionForRegNo(aId).ToString() + serialNo.ToString("D" + 4);
        //                dataContext.SubmitChanges();
        //                serialNo++;
        //            }
        //        }

        //        response.Message = ConstantData.SuccessMessage;
        //    }
        //    catch (Exception ex)
        //    {

        //        response.Message = ex.Message;
        //    }

        //    return response;
        //}
    }
}
