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
    [RoutePrefix("api/Refund")]
    public class RefundController : ApiController
    {
        [HttpPost]
        [Route("RefundList")]
        public ExpandoObject RefundList(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //var registrations = (from r1 in dataContext.RegistrationOnlines
                //                     join b1 in dataContext.Refunds on r1.RegistrationOnlineId equals b1.RegistrationOnlineId
                //                     join c1 in dataContext.Classes
                //                     on r1.ClassNo equals c1.ClassNo
                //                     join so1 in dataContext.SubjectOptions
                //                     on r1.SubjectOptionOne equals so1.SubjectOptionId into subSubjectOptionOne
                //                     from sSubjectOptionOne in subSubjectOptionOne.DefaultIfEmpty()
                //                     join so2 in dataContext.SubjectOptions
                //                     on r1.SubjectOptionTwo equals so2.SubjectOptionId into subSubjectOptionTwo
                //                     from sSubjectOptionTwo in subSubjectOptionTwo.DefaultIfEmpty()
                //                     join ps1 in dataContext.Students
                //                     on r1.PreviousSchoolAdmissionNo equals ps1.AdmissionNo into subPStudents
                //                     from sPreviousStudent in subPStudents.DefaultIfEmpty()
                //                     join a1 in dataContext.Admissions
                //                     on sPreviousStudent.StudentId equals a1.StudentId into subPAdmission
                //                     from sPAdmission in subPAdmission.DefaultIfEmpty()
                //                     join s1 in dataContext.Sections
                //                     on sPAdmission.SectionId equals s1.SectionId into subPSection
                //                     join st1 in dataContext.Students
                //                     on r1.RegistrationOnlineId equals st1.RegistrationOnlineId into subStudent
                //                     from sStudent in subStudent.DefaultIfEmpty()
                //                     join ad1 in dataContext.Admissions.Where(x => x.AcademicSessionId == model.AcademicSessionId)
                //                     on sStudent.StudentId equals ad1.StudentId into subAdmission
                //                     from sAdmission in subAdmission.DefaultIfEmpty()
                //                     join ss1 in dataContext.Sections
                //                     on sAdmission.SectionId equals ss1.SectionId into subSection
                //                     from sSection in subSection.DefaultIfEmpty()
                //                     join cl1 in dataContext.Classes
                //                     on sSection.ClassId equals cl1.ClassId into subClasses
                //                     from sClass in subClasses.DefaultIfEmpty()
                //                     where r1.RegistrationOnlineStatus != (byte)RegistrationOnlineStatus.Pending
                //                     && (model.AcademicSessionId == 0 || r1.AcademicSessionId == model.AcademicSessionId)
                //                     && (model.ClassNo == 0 || r1.ClassNo == model.ClassNo)
                //                     && (model.ClassStreamTypeId == null || r1.ClassStreamTypeId == model.ClassStreamTypeId)
                //                     && (model.SubjectOptionOne == null || r1.SubjectOptionOne == model.SubjectOptionOne)
                //                     && (model.SubjectOptionTwo == null || r1.SubjectOptionOne == model.SubjectOptionTwo)
                //                     && (model.IsSameSchool == null || r1.IsSameSchool == model.IsSameSchool)
                //                     && (model.RegistrationOnlineStatus == 0 || r1.RegistrationOnlineStatus == model.RegistrationOnlineStatus)
                //                     && (model.RefundStatus == 0 || b1.RefundStatus == model.RefundStatus)
                //                     && (model.RegistrationOnlineId == 0 || b1.RegistrationOnlineId == model.RegistrationOnlineId)
                //                     && (model.AdmissionType == 0 || (model.AdmissionType == 1 ? dataContext.PaymentLinkRegistrations.Any(x => x.RegistrationOnlineId == r1.RegistrationOnlineId && x.PaymentStatus == (byte)PaymentStatus.Paid) : dataContext.Students.Any(x => x.RegistrationOnlineId == r1.RegistrationOnlineId)))
                //                     select new
                //                     {
                //                         b1.RefundId,
                //                         b1.RegistrationOnlineId,
                //                         b1.BankName,
                //                         b1.BankBranch,
                //                         b1.AccountHolderName,
                //                         b1.AccountNo,
                //                         b1.IFSC,
                //                         b1.ApplicationDate,
                //                         b1.RefundStatus,
                //                         b1.ApplicationReason,
                //                         b1.RejectedReason,
                //                         b1.RefundAmount,
                //                         b1.RefundDate,
                //                         r1.AcademicSession.SessionName,
                //                         Gender = Enum.GetName(typeof(Gender), r1.Gender),
                //                         Category = Enum.GetName(typeof(Category), r1.Category).Replace("_", "/"),
                //                         ClassName = c1.ClassName2,
                //                         r1.ClassStreamType.ClassStreamTypeName,
                //                         r1.DOB,
                //                         r1.EmailId,
                //                         r1.FatherPresentAddress,
                //                         r1.FathersAnnualIncome,
                //                         r1.FathersDesignation,
                //                         r1.FathersMobileNo,
                //                         r1.FathersName,
                //                         r1.FathersOccupation,
                //                         r1.FathersOccupationDetail,
                //                         r1.FathersOrMothersEmail,
                //                         r1.FathersPhoto,
                //                         r1.FathersQualification,
                //                         r1.GuardiansAddress,
                //                         r1.GuardiansMobileNo,
                //                         r1.GuardiansName,
                //                         r1.IsSameSchool,
                //                         r1.MobileNo,
                //                         r1.MothersAnnualIncome,
                //                         r1.MothersMobileNo,
                //                         r1.MothersName,
                //                         r1.MothersOccupation,
                //                         r1.MothersPhoto,
                //                         r1.MothersQualification,
                //                         Nationality = Enum.GetName(typeof(Nationality), r1.Nationality),
                //                         r1.PreviousSchoolAdmissionNo,
                //                         PreviousSectionName = subPSection.Any() ? subPSection.First().SectionName : "",
                //                         sAdmission.RollNo,
                //                         sStudent.AdmissionNo,
                //                         sSection.SectionName,
                //                         NewClass = sClass.ClassName2,
                //                         r1.PreviousSchoolMedium,
                //                         r1.PreviousSchoolName,
                //                         r1.RegistrationDate,
                //                         r1.RegistrationNo,
                //                         Religion = Enum.GetName(typeof(Religion), r1.Religion),
                //                         r1.SMSAlertNo,
                //                         r1.StudentName,
                //                         r1.StudentPhoto,
                //                         r1.SubjectOptionOne,
                //                         SubjectOptionOneName = sSubjectOptionOne.SubjectOptionName,
                //                         r1.SubjectOptionTwo,
                //                         SubjectOptionTwoName = sSubjectOptionTwo.SubjectOptionName,
                //                         r1.WhatsappNo,
                //                     }).GroupBy(x => x.RegistrationOnlineId).Select(x => x.First()).ToList();
                //response.RegistrationOnlineList = registrations;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveRefund")]
        public ExpandoObject SaveRefund(Refund model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Refund Refund = null;
                if (model.RefundId > 0)
                    Refund = dataContext.Refunds.First(x => x.RefundId == model.RefundId);
                else
                    Refund = new Refund
                    {
                        RegistrationOnlineId = model.RegistrationOnlineId,
                        ApplicationDate = DateTime.Now,
                        RefundStatus = (byte)RefundStatus.Pending,
                    };
                Refund.BankName = model.BankName;
                Refund.BankBranch = model.BankBranch;
                Refund.AccountHolderName = model.AccountHolderName;
                Refund.AccountNo = model.AccountNo;
                Refund.IFSC = model.IFSC;
                Refund.ApplicationReason = model.ApplicationReason;
                Refund.RejectedReason = model.RejectedReason;
                Refund.RefundAmount = model.RefundAmount;
                Refund.RefundDate = model.RefundDate;
                Refund.RefundStatus = model.RefundStatus;
                Refund.UpdatedBy = SessionUtilies.GetLoginId();
                Refund.UpdatedOn = DateTime.Now;
                if (Refund.RefundId == 0)
                    dataContext.Refunds.InsertOnSubmit(Refund);
                dataContext.SubmitChanges();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }
            return response;
        }

        [HttpPost]
        [Route("updateRefundStatus")]
        public ExpandoObject UpdateRefundStatus(List<Refund> list)
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

                list.ForEach(model =>
                {
                    if (dataContext.Refunds.Any(x => x.RefundId == model.RefundId && x.RefundStatus != model.RefundStatus))
                    {
                        Refund Refund = dataContext.Refunds.First(x => x.RefundId == model.RefundId);
                        Refund.RefundStatus = model.RefundStatus;
                        Refund.UpdatedBy = SessionUtilies.GetLoginId();
                        Refund.UpdatedOn = DateTime.Now;
                        dataContext.SubmitChanges();
                    }
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
    }
}
