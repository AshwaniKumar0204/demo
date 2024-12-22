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
    [RoutePrefix("api/BonafideCertificate")]
    public class BonafideCertificateController : ApiController
    {
        [HttpGet]
        [Route("UnissuedBonafideCertificateAdmissionList")]
        public ExpandoObject UnissuedBonafideCertificateAdmissionList()
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
                           on sc1.ClassId equals c1.ClassId
                           where a1.AdmissionStatus == (byte)AdmissionStatus.Active
                           && !dataContext.BonafideCertificates.Any(x => x.AdmissionId == a1.AdmissionId)
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
                               s1.Gender,
                               sc1.SectionName,
                               s1.DateOfBirth,
                               a1.RollNo,
                               ClassName = c1.ClassName2 + "/" + sc1.SectionName,
                               a1.AcademicSession.SessionName,
                               a1.StudentId,
                               s1.HouseId,
                               s1.House.HouseName,
                               SearchAdmission = s1.AdmissionNo + " (" + s1.FullName + (s1.FatherName != null ? (", " + s1.FatherName) : "") + ", " + c1.ClassName2 + ")"
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
        [Route("BonafideCertificateList")]
        public ExpandoObject BonafideCertificateList(int AcademicSessionId, int ClassId, int SectionId, int BonafideCertificateId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from c1 in dataContext.BonafideCertificates
                            join a1 in dataContext.Admissions
                            on c1.AdmissionId equals a1.AdmissionId
                            join s1 in dataContext.Sections
                            on a1.SectionId equals s1.SectionId
                            where (AcademicSessionId > 0 ? a1.AcademicSessionId == AcademicSessionId : true)
                            && (ClassId > 0 ? s1.ClassId == ClassId : true)
                            && (SectionId > 0 ? s1.SectionId == SectionId : true)
                            && (BonafideCertificateId == 0 || BonafideCertificateId == c1.BonafideCertificateId)
                            select new
                            {
                                c1.IssuePurpose,
                                BonafideCertificateId = c1.BonafideCertificateId,
                                CertificateNo = c1.CertificateNo,
                                AdmissionId = c1.AdmissionId,
                                AdmissionNo = c1.AdmissionNo,
                                StudentName = c1.StudentName,
                                FatherName = c1.FatherName,
                                DateOfBirth = c1.DateOfBirth,
                                ClassName = c1.ClassName,
                                SessionName = c1.SessionName,
                                DateOfIssue = c1.DateOfIssue,
                                ClassId = s1.ClassId,
                                SectionId = s1.SectionId
                            });
                ResponseMessage.BonafideCertificateList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveBonafideCertificate")]
        public ExpandoObject SaveBonafideCertificate(BonafideCertificate model)
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
                BonafideCertificate BonafideCertificate = null;
                if (model.BonafideCertificateId == 0)
                {
                    var admission = (from a1 in dataContext.Admissions
                                     where a1.AdmissionId == model.AdmissionId
                                     select a1).First();

                    BonafideCertificate = new BonafideCertificate
                    {
                        AdmissionId = admission.AdmissionId,
                        CreatedBy = SessionUtilies.GetLoginId(),
                        CreatedOn = DateTime.Now,
                    };
                }
                else
                {
                    BonafideCertificate = dataContext.BonafideCertificates.First(x => x.BonafideCertificateId == model.BonafideCertificateId);
                    BonafideCertificate.LastUpdatedOn = DateTime.Now;
                    BonafideCertificate.LastUpdatedBy = SessionUtilies.GetLoginId();
                }
                BonafideCertificate.CertificateNo = model.CertificateNo;
                BonafideCertificate.StudentName = model.StudentName;
                BonafideCertificate.Gender = model.Gender;
                BonafideCertificate.AdmissionNo = model.AdmissionNo;
                BonafideCertificate.ClassName = model.ClassName;
                BonafideCertificate.SessionName = model.SessionName;
                BonafideCertificate.FatherName = model.FatherName;
                BonafideCertificate.DateOfBirth = model.DateOfBirth;
                BonafideCertificate.IssuePurpose = model.IssuePurpose;
                BonafideCertificate.DateOfIssue = model.DateOfIssue;
                if (BonafideCertificate.BonafideCertificateId == 0)
                    dataContext.BonafideCertificates.InsertOnSubmit(BonafideCertificate);
                dataContext.SubmitChanges();

                transaction.Commit();
                IsCompleted = true;
                response.BonafideCertificateId = BonafideCertificate.BonafideCertificateId;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                if (ex.Message.Contains("IX"))
                    response.Message = "This Bonafide Certificate is already exist";
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
