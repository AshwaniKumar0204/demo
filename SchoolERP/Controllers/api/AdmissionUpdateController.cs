using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/admissionUpdate")]
    public class AdmissionUpdateController : ApiController
    {
        [HttpGet]
        [Route("admissionList")]
        public ExpandoObject AdmissionList(int AcademicSessionId, int ClassId, int SectionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Admissions
                            join s1 in dataContext.Students
                            on a1.StudentId equals s1.StudentId
                            join sec in dataContext.Sections
                            on a1.SectionId equals sec.SectionId
                            where a1.AcademicSessionId == AcademicSessionId
                            && sec.ClassId == ClassId
                            && a1.SectionId == SectionId
                            && s1.StudentStatus != (byte)StudentStatus.Left
                            orderby s1.AdmissionNo
                            select new UpdateAdmissionNoModel
                            {
                                AdmissionId = a1.AdmissionId,
                                FullName = s1.FullName,
                                StudentId = s1.StudentId,
                                AdmissionNo = s1.AdmissionNo,
                                NewAdmissionNo = s1.AdmissionNo,
                                RollNo = a1.RollNo,
                            });

                response.AdmissionList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("updateAdmissionNos")]
        public ExpandoObject UpdateAdmissionNos(List<UpdateAdmissionNoModel> admissionList)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                admissionList.ForEach(admission =>
                {
                    if (dataContext.Students.Where(x => x.AdmissionNo == admission.NewAdmissionNo).Any())
                    {
                        throw new Exception("Admission no :" + admission.NewAdmissionNo + " is already exist.");
                    }
                    Student student = dataContext.Students.Where(x => x.StudentId == admission.StudentId).First();
                    student.AdmissionNo = admission.NewAdmissionNo;
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

        [HttpPost]
        [Route("updateStudentRollNo")]
        public ExpandoObject UpdateStudentRollNo(List<UpdateAdmissionNoModel> model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                model.ForEach(admissionModel =>
                {
                    var admission = dataContext.Admissions.Where(x => x.AdmissionId == admissionModel.AdmissionId).First();
                    admission.RollNo = admissionModel.NewRollNo;
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

        [HttpPost]
        [Route("updateStudentDetails")]
        public ExpandoObject UpdateStudentDetails(List<AdmissionModel> model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                model.ForEach(admissionModel =>
                {
                    var student = dataContext.Students.Where(x => x.StudentId == admissionModel.StudentId).First();
                    //student.FullName = admissionModel.Student.FullName;
                    student.FatherName = admissionModel.Student.FatherName;
                    student.MotherName = admissionModel.Student.MotherName;
                    student.DateOfBirth = admissionModel.Student.DateOfBirth;
                    student.CorrespondenceAddressLine1 = admissionModel.Student.CorrespondenceAddressLine1;
                    student.PermanentAddressLine1 = admissionModel.Student.PermanentAddressLine1;
                    student.Email = admissionModel.Student.Email;
                    student.MobileNo = admissionModel.Student.MobileNo;
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
