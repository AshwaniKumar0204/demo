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
    [RoutePrefix("api/PaymentLink")]
    public class PaymentLinkController : ApiController
    {
        [HttpGet]
        [Route("AdmissionFeeListForLink")]
        public ExpandoObject AdmissionFeeListForLink(int AcademicSessionId, int ClassNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int AdmissionFeeHeadId = Convert.ToInt32(ConfigurationManager.AppSettings["AdmissionFeeHeadId"]);
                var AdmissionFeeList = LoadData.AdmissionFeeForGenerateLink(dataContext, ClassNo, AcademicSessionId, AdmissionFeeHeadId);
                if (AdmissionFeeList.Count() == 0)
                    throw new Exception("Admission fee is not available!!");
                response.AdmissionFeeList = AdmissionFeeList;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("RegistrationOnlineListForLinkTwo")]
        public ExpandoObject RegistrationOnlineListForLinkTwo(RegistrationOnline model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //model.ClassId = dataContext.RegistrationOnlines.First(x => x.SectionId == model.SectionId).ClassId;


                ////Admission Fee
                //var admissionFees = (from c1 in dataContext.AdmissionFeeHeads
                //                     join f1 in dataContext.FeeHeads
                //                     on c1.FeeHeadId equals f1.FeeHeadId
                //                     join cf1 in dataContext.AdmissionFees
                //                     on c1.AdmissionFeeHeadId equals cf1.AdmissionFeeHeadId
                //                     where c1.ClassId == model.ClassId
                //                     && c1.StudentTypeId == (byte)StudentTypeIds.General
                //                     && c1.AdmissionType == (byte)AdmissionType.NewAdmission
                //                     && c1.AcademicSessionId == model.AcademicSessionId
                //                     && cf1.Amount > 0
                //                     select new FeeHeadModel
                //                     {
                //                         FeeType = (byte)FeeType.Admission,
                //                         FeeTypeName = FeeType.Admission.ToString(),
                //                         FeeHeadTitle = f1.FeeHeadTitle,
                //                         FeeHeadId = c1.FeeHeadId,
                //                         Amount = cf1.Amount,
                //                     }).ToList();
                ////Class Fee
                //var classFees = (from c1 in dataContext.ClassFeeHeads
                //                 join f1 in dataContext.FeeHeads
                //                 on c1.FeeHeadId equals f1.FeeHeadId
                //                 join cf1 in dataContext.ClassFees
                //                 on c1.ClassFeeHeadId equals cf1.ClassFeeHeadId
                //                 where c1.ClassId == model.ClassId
                //                 && c1.StudentTypeId == (byte)StudentTypeIds.General
                //                 && c1.AcademicSessionId == model.AcademicSessionId
                //                 && c1.MonthNo == (byte)MonthNames.April
                //                 && cf1.Amount > 0
                //                 select new FeeHeadModel
                //                 {
                //                     FeeType = (byte)FeeType.Fee,
                //                     FeeTypeName = FeeType.Fee.ToString(),
                //                     FeeHeadTitle = f1.FeeHeadTitle,
                //                     FeeHeadId = c1.FeeHeadId,
                //                     Amount = cf1.Amount,
                //                 }).ToList();
                //admissionFees.ForEach(x =>
                //{
                //    if (classFees.Any(y => y.FeeHeadId == x.FeeHeadId && y.FeeType == x.FeeType))
                //        classFees.Find(y => y.FeeHeadId == x.FeeHeadId && y.FeeType == x.FeeType).Amount += x.Amount;
                //    else
                //        classFees.Add(x);
                //});

               
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SavePaymentLinkTwo")]
        public ExpandoObject SavePaymentLinkTwo(SavePaymentLinkTwoModel model)
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
                model.RegistrationOnlineList.ForEach(r1 =>
                {
                    PaymentLink paymentLink = new PaymentLink
                    {
                        CreatedBy = SessionUtilies.GetLoginId(),
                        CreatedOn = DateTime.Now,
                        ExpiredDate = model.ExpiredDate.HasValue ? model.ExpiredDate.Value.Date : model.ExpiredDate,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = SessionUtilies.GetLoginId()
                    };
                    dataContext.PaymentLinks.InsertOnSubmit(paymentLink);
                    dataContext.SubmitChanges();

                    r1.FeeList.Where(x => x.Amount > 0).ToList().ForEach(fee =>
                      {
                          PaymentLinkDetail paymentLinkDetail = new PaymentLinkDetail
                          {
                              Amount = fee.Amount,
                              FeeHeadId = fee.FeeHeadId,
                              PaymentLinkId = paymentLink.PaymentLinkId,
                              FeeType = fee.FeeType
                          };
                          dataContext.PaymentLinkDetails.InsertOnSubmit(paymentLinkDetail);
                          dataContext.SubmitChanges();
                      });

                    PaymentLinkRegistration paymentLinkRegistration = new PaymentLinkRegistration
                    {
                        PaymentLinkId = paymentLink.PaymentLinkId,
                        PaymentStatus = (byte)PaymentStatus.Pending,
                        RegistrationOnlineId = r1.RegistrationOnlineId
                    };
                    dataContext.PaymentLinkRegistrations.InsertOnSubmit(paymentLinkRegistration);
                    dataContext.SubmitChanges();
                });

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
        [Route("SavePaymentLink")]
        public ExpandoObject SaveRegistrationOnlineFee(SavePaymentLinkModel model)
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

                //Check Changes in Fee
                bool IsChanges = false;
                int AdmissionFeeHeadId = Convert.ToInt32(ConfigurationManager.AppSettings["AdmissionFeeHeadId"]);
                var AdmissionFeeList = LoadData.AdmissionFeeForGenerateLink(dataContext, model.ClassNo, model.AcademicSessionId, AdmissionFeeHeadId);
                if (AdmissionFeeList.Count() == 0)
                    throw new Exception("Admission fee is not available!!");

                if (AdmissionFeeList.Count() == model.AdmissionFeeList.Count())
                {
                    foreach (var ad in model.AdmissionFeeList)
                    {
                        if (AdmissionFeeList.Where(x => x.AdmissionFeeHeadId == ad.AdmissionFeeHeadId && ad.FeeHeadId == x.FeeHeadId && ad.Amount == x.Amount).Count() == 0)
                        {
                            IsChanges = true;
                            break;
                        }
                    }
                }
                else
                    IsChanges = true;

                PaymentLink paymentLink = new PaymentLink
                {
                    CreatedBy = SessionUtilies.GetLoginId(),
                    CreatedOn = DateTime.Now,
                    ExpiredDate = model.ExpiredDate.HasValue ? model.ExpiredDate.Value.Date : model.ExpiredDate,
                    UpdatedOn = DateTime.Now,
                    UpdatedBy = SessionUtilies.GetLoginId()
                };
                dataContext.PaymentLinks.InsertOnSubmit(paymentLink);
                dataContext.SubmitChanges();

                if (IsChanges)
                {
                    model.AdmissionFeeList.ForEach(fee =>
                    {
                        PaymentLinkDetail paymentLinkDetail = new PaymentLinkDetail
                        {
                            Amount = fee.Amount,
                            FeeHeadId = fee.FeeHeadId,
                            PaymentLinkId = paymentLink.PaymentLinkId,
                            FeeType = (byte)FeeType.Admission
                        };
                        dataContext.PaymentLinkDetails.InsertOnSubmit(paymentLinkDetail);
                        dataContext.SubmitChanges();
                    });
                }

                model.RegistrationOnlineIds.ForEach(registrationOnlineId =>
                {
                    PaymentLinkRegistration paymentLinkRegistration = new PaymentLinkRegistration
                    {
                        PaymentLinkId = paymentLink.PaymentLinkId,
                        PaymentStatus = (byte)PaymentStatus.Pending,
                        RegistrationOnlineId = registrationOnlineId
                    };
                    dataContext.PaymentLinkRegistrations.InsertOnSubmit(paymentLinkRegistration);
                    dataContext.SubmitChanges();
                });

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
        [Route("PaymentLinkList")]
        public ExpandoObject PaymentLinkList(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int AdmissionFeeHeadId = Convert.ToInt32(ConfigurationManager.AppSettings["AdmissionFeeHeadId"]);
                //var AdmissionFeeList = LoadData.AdmissionFeeForGenerateLink(dataContext, model.ClassNo, model.AcademicSessionId, AdmissionFeeHeadId);
                //var list = from p1 in dataContext.PaymentLinks
                //           join p2 in dataContext.PaymentLinkRegistrations
                //           on p1.PaymentLinkId equals p2.PaymentLinkId
                //           join r1 in dataContext.RegistrationOnlines
                //           on p2.RegistrationOnlineId equals r1.RegistrationOnlineId
                //           join so1 in dataContext.SubjectOptions
                //           on r1.SubjectOptionOne equals so1.SubjectOptionId into subSubjectOption
                //           from sSujectOption in subSubjectOption.DefaultIfEmpty()
                //           join st1 in dataContext.ClassStreamTypes
                //           on r1.ClassStreamTypeId equals st1.ClassStreamTypeId into subStreamType
                //           from sStreamType in subStreamType.DefaultIfEmpty()
                //           where (model.AcademicSessionId == 0 || r1.AcademicSessionId == model.AcademicSessionId)
                //           && (model.ClassNo == 0 || r1.ClassNo == model.ClassNo)
                //           && (model.RegistrationFormId == 0 || r1.ConfirmRegistrationFormId == model.RegistrationFormId)
                //           && (model.ClassStreamTypeId > 0 ? sStreamType.ClassStreamTypeId == model.ClassStreamTypeId : true)
                //           && (model.SubjectOptionOne > 0 ? sSujectOption.SubjectOptionId == model.SubjectOptionOne : true)
                //           && p2.PaymentStatus == (byte)PaymentStatus.Pending
                //           select new
                //           {
                //               p1.PaymentLinkId,
                //               p2.PaymentLinkRegistrationId,
                //               r1.RegistrationOnlineId,
                //               p1.ExpiredDate,
                //               r1.RegistrationNo,
                //               r1.StudentName,
                //               r1.FathersName,
                //               r1.MobileNo,
                //               r1.EmailId,
                //               r1.DOB,
                //               r1.IsSameSchool,
                //               ClassName = dataContext.Classes.Where(x => x.ClassNo == r1.ClassNo).First().ClassName2,
                //               sSujectOption.SubjectOptionName,
                //               sStreamType.ClassStreamTypeName,
                //               PaymentLinkDetailList = (from p3 in dataContext.PaymentLinkDetails
                //                                        join f1 in dataContext.FeeHeads
                //                                        on p3.FeeHeadId equals f1.FeeHeadId
                //                                        where p3.PaymentLinkId == p1.PaymentLinkId
                //                                        select new
                //                                        {
                //                                            p3.FeeHeadId,
                //                                            p3.PaymentLinkDetailId,
                //                                            f1.FeeHeadTitle,
                //                                            p3.Amount
                //                                        }).ToList(),
                //           };

                //response.PaymentLinkList = list.Distinct().ToList();
                //response.AdmissionFeeList = AdmissionFeeList.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("UpdatePaymentLink")]
        public ExpandoObject UpdatePaymentLink(UpdatePaymentLinkModel model)
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

                //Check Changes in Fee

                var paymentLinkRegistrations = dataContext.PaymentLinkRegistrations.Where(x => x.PaymentLinkId == model.PaymentLinkId).ToList();
                if (paymentLinkRegistrations.Count() == 1)
                {
                    var PaymentLink = dataContext.PaymentLinks.Where(x => x.PaymentLinkId == model.PaymentLinkId).First();
                    PaymentLink.ExpiredDate = model.ExpiredDate.HasValue ? model.ExpiredDate.Value.Date : model.ExpiredDate;
                    PaymentLink.UpdatedBy = SessionUtilies.GetLoginId();
                    PaymentLink.UpdatedOn = DateTime.Now;
                    dataContext.SubmitChanges();

                    model.PaymentLinkDetailList.ForEach(fee =>
                    {
                        if (fee.PaymentLinkDetailId == 0)
                        {
                            PaymentLinkDetail paymentLinkDetail = new PaymentLinkDetail
                            {
                                Amount = fee.Amount,
                                FeeHeadId = fee.FeeHeadId,
                                PaymentLinkId = PaymentLink.PaymentLinkId
                            };
                            dataContext.PaymentLinkDetails.InsertOnSubmit(paymentLinkDetail);
                            dataContext.SubmitChanges();
                        }
                        else
                        {
                            var paymentLinkDetail = dataContext.PaymentLinkDetails.Where(x => x.PaymentLinkDetailId == fee.PaymentLinkDetailId).First();
                            paymentLinkDetail.Amount = fee.Amount;
                            dataContext.SubmitChanges();
                        }
                    });
                }
                else
                {
                    bool IsChanges = false;
                    foreach (var pl in model.PaymentLinkDetailList)
                    {
                        if (dataContext.PaymentLinkDetails.Where(x => x.Amount == pl.Amount && x.PaymentLinkDetailId == pl.PaymentLinkDetailId).Count() == 0)
                        {
                            IsChanges = true;
                            break;
                        }
                    }
                    if (IsChanges)
                    {
                        model.ExpiredDate = dataContext.PaymentLinks.Where(x => x.PaymentLinkId == model.PaymentLinkId).First().ExpiredDate;
                        PaymentLink paymentLink = new PaymentLink
                        {
                            CreatedBy = SessionUtilies.GetLoginId(),
                            CreatedOn = DateTime.Now,
                            ExpiredDate = model.ExpiredDate,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = SessionUtilies.GetLoginId()
                        };
                        dataContext.PaymentLinks.InsertOnSubmit(paymentLink);
                        dataContext.SubmitChanges();

                        PaymentLinkRegistration paymentLinkRegistration = dataContext.PaymentLinkRegistrations.Where(x => x.PaymentLinkRegistrationId == model.PaymentLinkRegistrationId).First();
                        paymentLinkRegistration.PaymentLinkId = paymentLink.PaymentLinkId;
                        dataContext.SubmitChanges();

                        model.PaymentLinkDetailList.ForEach(fee =>
                        {
                            PaymentLinkDetail paymentLinkDetail = new PaymentLinkDetail
                            {
                                Amount = fee.Amount,
                                FeeHeadId = fee.FeeHeadId,
                                PaymentLinkId = paymentLink.PaymentLinkId
                            };
                            dataContext.PaymentLinkDetails.InsertOnSubmit(paymentLinkDetail);
                            dataContext.SubmitChanges();
                        });
                    }
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
        [Route("PaidPaymentLinkList")]
        public ExpandoObject PaidPaymentLinkList(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //var list = from p2 in dataContext.PaymentLinkRegistrations
                //           join f1 in dataContext.FeeCollections
                //           on p2.FeeCollectionId equals f1.FeeCollectionId
                //           join r1 in dataContext.RegistrationOnlines
                //           on p2.RegistrationOnlineId equals r1.RegistrationOnlineId
                //           join so1 in dataContext.SubjectOptions
                //           on r1.SubjectOptionOne equals so1.SubjectOptionId into subSubjectOption
                //           from sSujectOption in subSubjectOption.DefaultIfEmpty()
                //           join st1 in dataContext.ClassStreamTypes
                //           on r1.ClassStreamTypeId equals st1.ClassStreamTypeId into subStreamType
                //           from sStreamType in subStreamType.DefaultIfEmpty()
                //           join ss1 in dataContext.Students
                //           on r1.PreviousSchoolAdmissionNo equals ss1.AdmissionNo into subStudent
                //           from sStudent in subStudent.DefaultIfEmpty()
                //           join aa1 in (from a1 in dataContext.Admissions
                //                        join ss2 in dataContext.Sections
                //                        on a1.SectionId equals ss2.SectionId
                //                        join c1 in dataContext.Classes
                //                        on ss2.ClassId equals c1.ClassId
                //                        where c1.ClassNo == 10
                //                        select new { a1.StudentId, ss2.SectionName })
                //           on sStudent.StudentId equals aa1.StudentId into subSections
                //           from sSection in subSections.DefaultIfEmpty()
                //           where (model.AcademicSessionId == 0 || r1.AcademicSessionId == model.AcademicSessionId)
                //           && (model.ClassNo == 0 || r1.ClassNo == model.ClassNo)
                //           && (model.RegistrationFormId == 0 || r1.ConfirmRegistrationFormId == model.RegistrationFormId)
                //           && (model.ClassStreamTypeId > 0 ? sStreamType.ClassStreamTypeId == model.ClassStreamTypeId : true)
                //           && (model.SubjectOptionOne > 0 ? sSujectOption.SubjectOptionId == model.SubjectOptionOne : true)
                //           && p2.PaymentStatus == (byte)PaymentStatus.Paid
                //           select new
                //           {
                //               f1.FeeCollectionId,
                //               f1.ReceiptNo,
                //               f1.PaidAmount,
                //               f1.PaymentDate,
                //               p2.PaymentLinkRegistrationId,
                //               r1.RegistrationOnlineId,
                //               r1.RegistrationNo,
                //               r1.StudentName,
                //               r1.FathersName,
                //               r1.MobileNo,
                //               r1.WhatsappNo,
                //               r1.SMSAlertNo,
                //               r1.EmailId,
                //               r1.IsSameSchool,
                //               r1.PreviousSchoolAdmissionNo,
                //               r1.DOB,
                //               PreviousSectionName = sSection.SectionName,
                //               ClassName = dataContext.Classes.Where(x => x.ClassNo == r1.ClassNo).First().ClassName2,
                //               sSujectOption.SubjectOptionName,
                //               sStreamType.ClassStreamTypeName,
                //           };
                //response.PaymentLinkList = list.Distinct().ToList();
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
