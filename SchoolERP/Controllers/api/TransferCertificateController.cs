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
    [RoutePrefix("api/TransferCertificate")]
    public class TransferCertificateController : ApiController
    {
        [HttpGet]
        [Route("UnissuedTransferCertificateAdmissionList")]
        public ExpandoObject UnissuedTransferCertificateAdmissionList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //var data = from s1 in dataContext.Students
                //           join a1 in dataContext.Admissions
                //           on s1.StudentId equals a1.StudentId
                //           join sc1 in dataContext.Sections
                //           on a1.SectionId equals sc1.SectionId
                //           join c1 in dataContext.Classes
                //           on sc1.ClassId equals c1.ClassId into subClass
                //           from sClass in subClass.DefaultIfEmpty()
                //           join ss1 in dataContext.ExamStudentSubjects
                //           on a1.AdmissionId equals ss1.AdmissionId into subStatudentSubjects
                //           from sStudentSubject in subStatudentSubjects.DefaultIfEmpty()
                //           where a1.AdmissionStatus == (byte)AdmissionStatus.Active
                //           && s1.TransferCertificateIssue == false
                //           orderby a1.AdmissionId descending
                //           select new
                //           {
                //               a1.AdmissionId,
                //               a1.AdmissionDate,
                //               s1.FullName,
                //               s1.MotherName,
                //               s1.FatherName,
                //               s1.AdmissionNo,
                //               s1.MobileNo,
                //               sc1.SectionName,
                //               s1.DateOfBirth,
                //               a1.RollNo,
                //               sc1.Class.ClassName,
                //               a1.StudentId,
                //               s1.HouseId,
                //               s1.House.HouseName,
                //               s1.RegistrationDate,
                //               s1.Religion,
                //               s1.Category,
                //               s1.Nationality,
                //               SearchAdmission = s1.AdmissionNo + " (" + s1.FullName + (s1.FatherName != null ? (", " + s1.FatherName) : "") + (sClass.ClassName != null ? (", " + sClass.ClassName) : "") + ")",
                //               SubjectStudied1 = sStudentSubject.SubjectOne.HasValue ? dataContext.ExamSubjects.First(x => x.ExamSubjectId == sStudentSubject.SubjectOne).SubjectName : "",
                //               SubjectStudied2 = sStudentSubject.SubjectTwo.HasValue ? dataContext.ExamSubjects.First(x => x.ExamSubjectId == sStudentSubject.SubjectTwo).SubjectName : "",
                //               SubjectStudied3 = sStudentSubject.SubjectThree.HasValue ? dataContext.ExamSubjects.First(x => x.ExamSubjectId == sStudentSubject.SubjectThree).SubjectName : "",
                //               SubjectStudied4 = sStudentSubject.SubjectFour.HasValue ? dataContext.ExamSubjects.First(x => x.ExamSubjectId == sStudentSubject.SubjectFour).SubjectName : "",
                //               SubjectStudied5 = sStudentSubject.SubjectFive.HasValue ? dataContext.ExamSubjects.First(x => x.ExamSubjectId == sStudentSubject.SubjectFive).SubjectName : "",
                //               SubjectStudied6 = sStudentSubject.SubjectSix.HasValue ? dataContext.ExamSubjects.First(x => x.ExamSubjectId == sStudentSubject.SubjectSix).SubjectName : "",
                //           };
                //ResponseMessage.AdmissionList = data.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("StudentConcessionDetail")]
        public ExpandoObject StudentConcessionDetail(int StudentId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var admissionIds = (from s1 in dataContext.Admissions
                                    where s1.StudentId == StudentId
                                    select s1.AdmissionId);

                var concessions = (from cs1 in dataContext.StudentConcessions
                                   join a1 in dataContext.Admissions
                                   on cs1.AdmissionId equals a1.AdmissionId
                                   where cs1.StudentConcessionStatus == Convert.ToByte(ConcessionStatus.Active)
                                   && admissionIds.Contains(cs1.AdmissionId)
                                   select cs1);
                if (concessions.Count() > 0)
                    ResponseMessage.Concession = 1;
                else
                    ResponseMessage.Concession = 2;

                ResponseMessage.AdmissionClassName = (from c1 in dataContext.Admissions
                                                      where c1.StudentId == StudentId
                                                      orderby c1.AdmissionId ascending
                                                      select c1).First().Section.Class.ClassName;
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("TransferCertificateList")]
        public ExpandoObject TransferCertificateList(int AcademicSessionId, int ClassId, int SectionId,int TransferCertificateId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from c1 in dataContext.TransferCertificates
                            join a1 in dataContext.Admissions
                            on c1.AdmissionId equals a1.AdmissionId
                            join s1 in dataContext.Sections
                            on a1.SectionId equals s1.SectionId
                            where (AcademicSessionId > 0 ? a1.AcademicSessionId == AcademicSessionId : true)
                            && (ClassId > 0 ? s1.ClassId == ClassId : true)
                            && (SectionId > 0 ? s1.SectionId == SectionId : true)
                            && (TransferCertificateId == 0 || TransferCertificateId == c1.TransferCertificateId)
                            select new TransferCertificateModel
                            {
                                AdmissionNo = c1.AdmissionNo,
                                TransferCertificateId = c1.TransferCertificateId,
                                AdmissionId = c1.AdmissionId,
                                ClassName = c1.ClassName,
                                DateOfBirth = c1.DateOfBirth,
                                FatherName = c1.FatherName,
                                MotherName = c1.MotherName,
                                StudentName = c1.StudentName,
                                ClassId = s1.ClassId,
                                Category = c1.Category,
                                SectionId = s1.SectionId,
                                AdmissionDate = c1.AdmissionDate,
                                AnyFeeConcessionAvailed = c1.AnyFeeConcessionAvailed,
                                DateOfApplicationOfCertiticate = c1.DateOfApplicationOfCertiticate,
                                DateOfIssue = c1.DateOfIssue,
                                ExamPassedClass = c1.ExamPassedClass,
                                ExtraCurricularActivities = c1.ExtraCurricularActivities,
                                GeneralConduct = c1.GeneralConduct,
                                LastStudiedClass = c1.LastStudiedClass,
                                MonthUptoPaid = c1.MonthUptoPaid,
                                Nationality = c1.Nationality,
                                QualifiedForClass = c1.QualifiedForClass,
                                ReasonForLeaving = c1.ReasonForLeaving,
                                Remarks = c1.Remarks,
                                SubjectStudied1 = c1.SubjectStudied1,
                                SubjectStudied2 = c1.SubjectStudied2,
                                SubjectStudied3 = c1.SubjectStudied3,
                                SubjectStudied4 = c1.SubjectStudied4,
                                SubjectStudied5 = c1.SubjectStudied5,
                                SubjectStudied6 = c1.SubjectStudied6,
                                TotalPresentDays = c1.TotalPresentDays,
                                TotalWorkingDays = c1.TotalWorkingDays,
                                TransferCertificateNo = c1.TransferCertificateNo,
                                WhetherFailedTwice = c1.WhetherFailedTwice,
                                WhetherNCCCadet = c1.WhetherNCCCadet,
                                WhetherQualifiedForPromotion = c1.WhetherQualifiedForPromotion,
                                WhetherSCOrST = c1.WhetherSCOrST
                            });
                ResponseMessage.TransferCertificateList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveTransferCertificate")]
        public ExpandoObject SaveTransferCertificate(TransferCertificateModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                TransferCertificate transferCertificate = null;
                if (model.TransferCertificateId == 0)
                {
                    var admission = (from a1 in dataContext.Admissions
                                     where a1.AdmissionId == model.AdmissionId
                                     select a1).First();

                    var student = (from s1 in dataContext.Students
                                   where s1.StudentId == admission.StudentId
                                   select s1).First();
                    student.TransferCertificateIssue = true;
                    if (student.CharacterCertificateIssue.HasValue)
                        if (student.CharacterCertificateIssue.Value)
                            student.StudentStatus = (byte)StudentStatus.Inactive;
                    dataContext.SubmitChanges();

                    transferCertificate = new TransferCertificate();
                    transferCertificate.AdmissionNo = admission.Student.AdmissionNo;
                    transferCertificate.AdmissionId = admission.AdmissionId;
                }
                else
                    transferCertificate = dataContext.TransferCertificates.First(x => x.TransferCertificateId == model.TransferCertificateId);
                transferCertificate.StudentName = model.StudentName;
                transferCertificate.FatherName = model.FatherName;
                transferCertificate.MotherName = model.MotherName;
                transferCertificate.Nationality = model.Nationality;
                transferCertificate.WhetherSCOrST = model.WhetherSCOrST;
                transferCertificate.Category = model.Category;
                transferCertificate.AdmissionDate = model.AdmissionDate;
                transferCertificate.DateOfBirth = model.DateOfBirth;
                transferCertificate.LastStudiedClass = model.LastStudiedClass;
                transferCertificate.ClassName = model.ClassName;
                transferCertificate.ExamPassedClass = model.ExamPassedClass;
                transferCertificate.WhetherFailedTwice = model.WhetherFailedTwice;
                transferCertificate.SubjectStudied1 = model.SubjectStudied1;
                transferCertificate.SubjectStudied2 = model.SubjectStudied2;
                transferCertificate.SubjectStudied3 = model.SubjectStudied3;
                transferCertificate.SubjectStudied4 = model.SubjectStudied4;
                transferCertificate.SubjectStudied5 = model.SubjectStudied5;
                transferCertificate.SubjectStudied6 = model.SubjectStudied6;
                transferCertificate.WhetherQualifiedForPromotion = model.WhetherQualifiedForPromotion;
                transferCertificate.QualifiedForClass = model.QualifiedForClass;
                transferCertificate.MonthUptoPaid = model.MonthUptoPaid;
                transferCertificate.AnyFeeConcessionAvailed = model.AnyFeeConcessionAvailed;
                transferCertificate.TotalWorkingDays = model.TotalWorkingDays;
                transferCertificate.TotalPresentDays = model.TotalPresentDays;
                transferCertificate.WhetherNCCCadet = model.WhetherNCCCadet;
                transferCertificate.ExtraCurricularActivities = model.ExtraCurricularActivities;
                transferCertificate.GeneralConduct = model.GeneralConduct;
                transferCertificate.DateOfApplicationOfCertiticate = model.DateOfApplicationOfCertiticate;
                transferCertificate.DateOfIssue = model.DateOfIssue;
                transferCertificate.ReasonForLeaving = model.ReasonForLeaving;
                transferCertificate.Remarks = model.Remarks;

                var tcs = (from tc1 in dataContext.TransferCertificates
                           orderby tc1.TransferCertificateNo descending
                           select tc1);

                if (tcs.Count() > 0)
                {
                    var transferCertificateNo = tcs.First().TransferCertificateNo;
                    int n = Convert.ToInt32(transferCertificateNo) + 1;
                    transferCertificate.TransferCertificateNo = n.ToString();
                }
                else
                {
                    transferCertificate.TransferCertificateNo = "1001";
                }
                if (transferCertificate.TransferCertificateId == 0)
                    dataContext.TransferCertificates.InsertOnSubmit(transferCertificate);
                dataContext.SubmitChanges();

                ResponseMessage.TransferCertificateId = transferCertificate.TransferCertificateId;
                ResponseMessage.Message = ConstantData.SuccessMessage;

                transaction.Commit();
                IsCompleted = true;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Transfer Certificate is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return ResponseMessage;
        }
    }
}
