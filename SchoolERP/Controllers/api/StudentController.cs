using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Dynamic;
using School;
using SchoolERP.Models;
using System.Data.Common;
using System.Web;
using System.IO;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/student")]
    public class StudentController : ApiController
    {
        [HttpGet]
        [Route("SearchStudentList")]
        public ExpandoObject SearchStudentList(int status)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from s1 in dataContext.Students
                           where status == 0 || s1.StudentStatus == status
                           select new
                           {
                               s1.StudentId,
                               s1.ParentId,
                               s1.AdmissionNo,
                               s1.FullName,
                               SearchStudent = s1.AdmissionNo + " " + s1.FullName
                           };
                response.StudentList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("GenerateAdmissionNo")]
        public ExpandoObject GenerateAdmissionNo(int AcademicSessionId, int SectionId)
        {
            dynamic response = new ExpandoObject();
            //string AdmissionNo = string.Empty;
            //int admNo = 0;
            try
            {
                //    SchoolDataContext dataContext = new SchoolDataContext();
                //    var list = from s1 in dataContext.Students
                //               join a1 in dataContext.Admissions
                //               on s1.StudentId equals a1.StudentId
                //               where a1.AcademicSessionId == AcademicSessionId
                //               && a1.SectionId == SectionId
                //               orderby s1.StudentId descending
                //               select s1;
                //    if (list.Any())
                //        AdmissionNo = list.First().AdmissionNo;
                //    if (!String.IsNullOrEmpty(AdmissionNo))
                //    {
                //        admNo = Convert.ToInt32(AdmissionNo.Substring(AdmissionNo.Length - 4, 4));
                //        admNo++;

                //        AdmissionNo= AdmissionNo.Substring(0, AdmissionNo.Length - 4) + admNo.ToString("D4");

                //    }
                response.AdmissionNo = LoadData.GenerateAdmissionNo(AcademicSessionId, SectionId);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("updateStudent")]
        public ExpandoObject UpdateStudent(StudentModel studentModel)
        {
            dynamic response = new ExpandoObject();

            SchoolDataContext dataContext = new SchoolDataContext();
            try
            {
                Student student = dataContext.Students.Where(x => x.StudentId == studentModel.StudentId).First();
                student.AadhaarNo = studentModel.AadhaarNo;
                student.AlternateNo = studentModel.AlternateNo;
                student.StudentTypeId = studentModel.StudentTypeId;
                student.Category = studentModel.Category;
                student.PermanentAddressLine1 = studentModel.PermanentAddressLine1;
                student.PermanentAddressLine2 = studentModel.PermanentAddressLine2;
                student.HouseId = studentModel.HouseId;
                //if (dataContext.Houses.Where(x => x.HouseName == studentModel.HouseName).Any())
                //    student.HouseId = dataContext.Houses.Where(x => x.HouseName == studentModel.HouseName).First().HouseId;
                //else if (!string.IsNullOrEmpty(studentModel.HouseName))
                //{
                //    House house = new House
                //    {
                //        HouseName = studentModel.HouseName,
                //        HouseStatus = 1
                //    };
                //    dataContext.Houses.InsertOnSubmit(house);
                //    dataContext.SubmitChanges();
                //    student.HouseId = house.HouseId;
                //}
                student.CorrespondenceCityId = studentModel.CorrespondenceCityId;
                student.CorrespondenceAddressLine1 = studentModel.PermanentAddressLine1;
                student.CorrespondenceAddressLine2 = studentModel.PermanentAddressLine2;
                student.PermanentCityId = studentModel.PermanentCityId;
                student.DateOfBirth = studentModel.DateOfBirth;
                student.Email = studentModel.Email;
                student.EntryDate = DateTime.Now;
                student.FamilyAnnualIncome = studentModel.FamilyAnnualIncome;
                student.FatherName = studentModel.FatherName;
                student.FatherOccupation = studentModel.FatherOccupation;
                student.FatherAadhaarNo = studentModel.FatherAadhaarNo;
                student.FullName = studentModel.FullName;
                student.Gender = studentModel.Gender;
                student.MobileNo = studentModel.MobileNo;
                student.MotherName = studentModel.MotherName;
                student.MotherOccupation = studentModel.MotherOccupation;
                student.MotherAadharNo = studentModel.MotherAadharNo;
                student.Nationality = studentModel.Nationality;
                student.Religion = studentModel.Religion;
                student.BloodGroup = studentModel.BloodGroup;
                //student.HouseId = studentModel.HouseId;
                student.Remarks = studentModel.Remarks;
                student.PermanentPinCode = studentModel.PermanentPinCode;
                student.CorrespondecePinCode = studentModel.CorrespondecePinCode;
                student.PreviousSchoolName = studentModel.PreviousSchoolName;
                student.PreviousBoardName = studentModel.PreviousBoardName;
                student.LastClassAttend = studentModel.LastClassAttend;
                student.MediumOfInstruction = studentModel.MediumOfInstruction;
                student.PreviousSchoolTCNo = studentModel.PreviousSchoolTCNo;
                student.PreviousSchoolTCIssueDate = studentModel.PreviousSchoolTCIssueDate;
                student.IntermediatePreviousSchoolName = studentModel.IntermediatePreviousSchoolName;
                student.IntermediateLastClassAttend = studentModel.IntermediateLastClassAttend;
                student.IntermediateMediumOfInstruction = studentModel.IntermediateMediumOfInstruction;
                student.IntermediatePreviousBoardName = studentModel.IntermediatePreviousBoardName;
                student.IntermediatePreviousSchoolTCIssueDate = studentModel.IntermediatePreviousSchoolTCIssueDate;
                student.IntermediatePreviousSchoolTCNo = studentModel.IntermediatePreviousSchoolTCNo;
                dataContext.SubmitChanges();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("CK_Student"))
                    response.Message = "This Student is already exists.";
                else if (ex.Message.Contains("UQ_Student_RegistrationNo"))
                    response.Message = "This Admission no is already exists.";
                else
                    response.Message = ex.Message.ToString();
            }
            return response;
        }

        [HttpPost]
        [Route("updateStudentPhoto")]
        public ExpandoObject UpdateStudentPhoto()
        {
            dynamic response = new ExpandoObject();

            SchoolDataContext dataContext = new SchoolDataContext();
            try
            {
                int AdmissionId = Convert.ToInt32(HttpContext.Current.Request.Params["AdmissionId"]);
                int StudentId = dataContext.Admissions.Where(x => x.AdmissionId == AdmissionId).First().StudentId;
                Student student = dataContext.Students.Where(x => x.StudentId == StudentId).First();

                var StudentPhoto = HttpContext.Current.Request.Files["StudentPhoto"];
                if (StudentPhoto != null)
                {
                    //if (StudentPhoto.ContentLength < 302400)
                    //{
                    //    response.Message = "Upload status: The file has to be less than 300 kb!";
                    //    return response;
                    //}

                    byte[] buffer = new byte[StudentPhoto.ContentLength];
                    StudentPhoto.InputStream.Read(buffer, 0, StudentPhoto.ContentLength);
                    
                    student.Photo = buffer;
                    dataContext.SubmitChanges();
                    response.AdmissionDetail = LoadData.GetAdmission(AdmissionId, dataContext);
                    response.Message = ConstantData.SuccessMessage;
                }
                else
                {
                    response.Message = "Invalid Student Photo !";
                    return response;
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("CK_Student"))
                    response.Message = "This Student is already exists.";
                else if (ex.Message.Contains("UQ_Student_RegistrationNo"))
                    response.Message = "This Admission no is already exists.";
                else
                    response.Message = ex.Message.ToString();
            }
            return response;
        }

        [HttpGet]
        [Route("StudentPreviousDues")]
        public ExpandoObject StudentPreviousDues(int AdmissionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                response.PreviousDues = FeeData.GetDueDetails(dataContext, AdmissionId, 0, false, 0, 0).TotalDues;
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
