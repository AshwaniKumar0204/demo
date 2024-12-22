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
    [RoutePrefix("api/CharacterCertificate")]
    public class CharacterCertificateController : ApiController
    {
        [HttpGet]
        [Route("UnissuedCharacterCertificateAdmissionList")]
        public ExpandoObject UnissuedCharacterCertificateAdmissionList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var data = from s1 in dataContext.Students
                           join a1 in dataContext.Admissions
                           on s1.StudentId equals a1.StudentId
                           join sc1 in dataContext.Sections
                           on a1.SectionId equals sc1.SectionId
                           join c1 in dataContext.Classes
                           on sc1.ClassId equals c1.ClassId into subClass
                           from sClass in subClass.DefaultIfEmpty()
                           where a1.AdmissionStatus == (byte)AdmissionStatus.Active
                           && s1.CharacterCertificateIssue == false
                           orderby a1.AdmissionId descending
                           select new
                           {
                               a1.AdmissionId,
                               a1.AdmissionDate,
                               s1.FullName,
                               s1.MotherName,
                               s1.FatherName,
                               s1.AdmissionNo,
                               s1.MobileNo,
                               sc1.SectionName,
                               s1.DateOfBirth,
                               a1.RollNo,
                               sc1.Class.ClassName,
                               a1.StudentId,
                               s1.HouseId,
                               s1.House.HouseName,
                               SearchAdmission = s1.AdmissionNo + " (" + s1.FullName + (s1.FatherName != null ? (", " + s1.FatherName) : "") + (sClass.ClassName != null ? (", " + sClass.ClassName) : "") + ")"
                           };
                ResponseMessage.AdmissionList = data.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("CharacterCertificateList")]
        public ExpandoObject CharacterCertificateList(int AcademicSessionId, int ClassId, int SectionId, int CharacterCertificateId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from c1 in dataContext.CharacterCertificates
                            join a1 in dataContext.Admissions
                            on c1.AdmissionId equals a1.AdmissionId
                            join s1 in dataContext.Sections
                            on a1.SectionId equals s1.SectionId
                            where (AcademicSessionId > 0 ? a1.AcademicSessionId == AcademicSessionId : true)
                            && (ClassId > 0 ? s1.ClassId == ClassId : true)
                            && (SectionId > 0 ? s1.SectionId == SectionId : true)
                            && (CharacterCertificateId == 0 || CharacterCertificateId == c1.CharacterCertificateId)
                            select new CharacterCertificateModel
                            {
                                AdmissionNo = c1.AdmissionNo,
                                CharacterCertificateId = c1.CharacterCertificateId,
                                AdmissionId = c1.AdmissionId,
                                ClassName = c1.ClassName,
                                DateOfBirth = c1.DateOfBirth,
                                FatherName = c1.FatherName,
                                FromDate = c1.FromDate,
                                Gender = c1.Gender,
                                IssueDate = c1.IssueDate,
                                MotherName = c1.MotherName,
                                StudentName = c1.StudentName,
                                ToDate = c1.ToDate,
                                ClassId = s1.ClassId,
                                SectionId = s1.SectionId
                            });
                ResponseMessage.CharacterCertificateList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveCharacterCertificate")]
        public ExpandoObject SaveCharacterCertificate(CharacterCertificateModel model)
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
                CharacterCertificate characterCertificate = null;
                if (model.CharacterCertificateId == 0)
                {
                    characterCertificate = new CharacterCertificate();
                    var admission = (from a1 in dataContext.Admissions
                                     where a1.AdmissionId == model.AdmissionId
                                     select a1).First();
                    characterCertificate.AdmissionId = admission.AdmissionId;
                    characterCertificate.Gender = admission.Student.Gender;

                    // updating student
                    var student = (from s1 in dataContext.Students
                                   where s1.StudentId == admission.StudentId
                                   select s1).First();
                    student.CharacterCertificateIssue = true;
                    if (student.TransferCertificateIssue.HasValue)
                        if (student.TransferCertificateIssue.Value)
                            student.StudentStatus = (byte)StudentStatus.Inactive;
                    dataContext.SubmitChanges();
                }
                else
                    characterCertificate = dataContext.CharacterCertificates.First(x => x.CharacterCertificateId == model.CharacterCertificateId);
                characterCertificate.StudentName = model.StudentName;
                characterCertificate.AdmissionNo = model.AdmissionNo;
                characterCertificate.ClassName = model.ClassName;
                characterCertificate.FatherName = model.FatherName;
                characterCertificate.MotherName = model.MotherName;
                characterCertificate.FromDate = model.FromDate;
                characterCertificate.ToDate = model.ToDate;
                characterCertificate.IssueDate = model.IssueDate;
                characterCertificate.DateOfBirth = model.DateOfBirth;
                if (characterCertificate.CharacterCertificateId == 0)
                    dataContext.CharacterCertificates.InsertOnSubmit(characterCertificate);
                dataContext.SubmitChanges();

                transaction.Commit();
                IsCompleted = true;
                response.CharacterCertificateId = characterCertificate.CharacterCertificateId;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                if (ex.Message.Contains("IX"))
                    response.Message = "This Character Certificate is already exist";
                else
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
