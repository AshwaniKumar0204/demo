using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/BoardMarks")]
    public class BoardMarksController : ApiController
    {
        [HttpPost]
        [Route("BoardMarksList")]
        public ExpandoObject BoardMarksList(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //var registrations = (from r1 in dataContext.RegistrationOnlines
                //                     join b1 in dataContext.BoardMarks on r1.RegistrationOnlineId equals b1.RegistrationOnlineId
                //                     join sb1 in dataContext.SchoolBoards on b1.SchoolBoardId equals sb1.SchoolBoardId
                //                     join c1 in dataContext.Classes
                //                     on r1.ClassNo equals c1.ClassNo
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
                //                     //&& (model.ResultType == null || r1.ResultType == model.ResultType)
                //                     && (model.RegistrationOnlineStatus == 0 || r1.RegistrationOnlineStatus == model.RegistrationOnlineStatus)
                //                     && (model.IsSeen == null || b1.IsSeen == model.IsSeen)
                //                     && (model.BoardMarksStatus == 0 || b1.BoardMarksStatus == model.BoardMarksStatus)
                //                     && (model.AdmissionType == 0 || (model.AdmissionType == 1 ? dataContext.PaymentLinkRegistrations.Any(x => x.RegistrationOnlineId == r1.RegistrationOnlineId && x.PaymentStatus == (byte)PaymentStatus.Paid) : dataContext.Students.Any(x => x.RegistrationOnlineId == r1.RegistrationOnlineId)))
                //                     select new
                //                     {
                //                         r1.RegistrationOnlineId,
                //                         b1.IsSeen,
                //                         b1.BoardMarksId,
                //                         sb1.BoardName,
                //                         b1.BoardMarksStatus,
                //                         b1.BoardRollNo,
                //                         b1.English10Total,
                //                         b1.SecondLang10Total,
                //                         b1.Maths10Total,
                //                         b1.Science10Total,
                //                         b1.SST10Total,
                //                         b1.Total10Marks,
                //                         b1.Percentage10,
                //                         b1.FileName,
                //                         b1.Upload10Marksheet,
                //                         b1.SubmittedDate,
                //                         b1.SchoolBoardId,
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
        [Route("UpdateBoardMarksSeen")]
        public ExpandoObject UpdateBoardMarksSeen(List<BoardMark> list)
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
                    var boardMarks = dataContext.BoardMarks.First(x => x.BoardMarksId == model.BoardMarksId);
                    boardMarks.IsSeen = model.IsSeen;
                    boardMarks.BoardMarksStatus = model.BoardMarksStatus;
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


    }
}
