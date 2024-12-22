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
    [RoutePrefix("api/RegistrationOnline")]
    public class RegistrationOnlineController : ApiController
    {
        [HttpPost]
        [Route("saveRegistrationOffline")]
        public ExpandoObject SaveRegistrationOffline(RegistrationOnline model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var registrationForm = dataContext.RegistrationForms.First(x => x.RegistrationFormId == model.RegistrationFormId);
                if (registrationForm.FromDOB.HasValue)
                {
                    if (registrationForm.FromDOB.Value > model.DOB.Date)
                    {
                        response.Message = "Date Of Birth Should not be less than " + registrationForm.FromDOB.Value.ToString("dd-MM-yyyy");
                        return response;
                    }
                }

                if (registrationForm.ToDOB.HasValue)
                {
                    if (registrationForm.ToDOB.Value < model.DOB.Date)
                    {
                        response.Message = "Date Of Birth Should not be more than " + registrationForm.ToDOB.Value.ToString("dd-MM-yyyy");
                        return response;
                    }
                }
                RegistrationOnline registrationOnline = null;
                if (model.RegistrationOnlineId > 0)
                    registrationOnline = dataContext.RegistrationOnlines.First(x => x.RegistrationOnlineId == model.RegistrationOnlineId);
                else
                {
                    Random r = new Random();
                    registrationOnline = new RegistrationOnline
                    {
                        RegistrationFrom = (byte)RegistrationFrom.Online,
                        AcademicSessionId = registrationForm.AcademicSessionId,
                        ClassNo = registrationForm.ClassNo,
                        RegistrationNo = LoadData.GenerateRegistrationNo(dataContext, registrationForm.AcademicSessionId, registrationForm.ClassNo),
                        RegistrationDate = DateTime.Now,
                        EntryType = model.EntryType,
                        RegistrationFormId = model.RegistrationFormId,
                        RegistrationOnlineStatus = (byte)RegistrationOnlineStatus.Pending,
                        LoginPassword = CryptoEngine.Encrypt(r.Next(100000, 999999).ToString())
                    };
                }
                registrationOnline.StudentName = model.StudentName;
                registrationOnline.DOB = model.DOB;
                registrationOnline.Gender = model.Gender;
                registrationOnline.BloodGroup = model.BloodGroup;
                registrationOnline.Religion = model.Religion;
                registrationOnline.Nationality = model.Nationality;
                registrationOnline.Category = model.Category;
                registrationOnline.EmailId = model.EmailId;
                registrationOnline.MobileNo = model.MobileNo;
                registrationOnline.MothersName = model.MothersName;
                registrationOnline.FathersName = model.FathersName;
                registrationOnline.PinCode = model.PinCode;
                registrationOnline.CityId = model.CityId;
                registrationOnline.StateId = model.StateId;
                registrationOnline.CorrespondenceAddress = model.CorrespondenceAddress;
                registrationOnline.MetricBoardName = model.MetricBoardName;
                registrationOnline.MetricPassingYear = model.MetricPassingYear;
                registrationOnline.MetricPercentage = model.MetricPercentage;
                registrationOnline.InterPassingStatus = model.InterPassingStatus;
                registrationOnline.PassingYear = model.PassingYear;
                registrationOnline.PhysicsMarks = model.PhysicsMarks;
                registrationOnline.ChemistryMarks = model.ChemistryMarks;
                registrationOnline.MathMarks = model.MathMarks;
                registrationOnline.PCMPercentage = model.PCMPercentage;
                registrationOnline.SubjectStream = model.SubjectStream;
                registrationOnline.TotalPercentage = model.TotalPercentage;
                registrationOnline.BoardOrUniversity = model.BoardOrUniversity;
                registrationOnline.PhoneNo = model.PhoneNo;
                if (registrationOnline.RegistrationOnlineId == 0)
                    dataContext.RegistrationOnlines.InsertOnSubmit(registrationOnline);
                dataContext.SubmitChanges();

                model.RegistrationOnlineSections.ToList().ForEach(r1 =>
                {
                    RegistrationOnlineSection registrationOnlineSection = new RegistrationOnlineSection
                    {
                        Priority = r1.Priority,
                        RegistrationOnlineId = registrationOnline.RegistrationOnlineId,
                        SectionId = r1.SectionId
                    };
                    dataContext.RegistrationOnlineSections.InsertOnSubmit(registrationOnlineSection);
                    dataContext.SubmitChanges();
                });
                if (model.RegistrationOnlineId == 0)
                {
                    response.RegistrationNo = registrationOnline.RegistrationNo;
                    response.RegistrationOnlineId = registrationOnline.RegistrationOnlineId;
                    response.LoginPassword = CryptoEngine.Decrypt(registrationOnline.LoginPassword);
                }
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("RegistrationOnlineList")]
        public ExpandoObject RegistrationOnlineList(RegistrationOnline model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from r1 in dataContext.RegistrationOnlines
                           join c1 in dataContext.Classes on r1.ClassNo equals c1.ClassNo
                           join a1 in dataContext.AcademicSessions on r1.AcademicSessionId equals a1.AcademicSessionId
                           where r1.RegistrationOnlineStatus != (byte)RegistrationOnlineStatus.Pending
                           && (model.AcademicSessionId == 0 || r1.AcademicSessionId == model.AcademicSessionId)
                           && (model.ClassNo == 0 || r1.ClassNo == model.ClassNo)
                           && (model.RegistrationOnlineStatus == 0 || r1.RegistrationOnlineStatus == model.RegistrationOnlineStatus)
                           && (model.RegistrationFormId == 0 || r1.RegistrationFormId == model.RegistrationFormId)
                           select new
                           {
                               r1.RegistrationOnlineId,
                               r1.AcademicSessionId,
                               r1.ClassNo,
                               c1.ClassName2,
                               r1.EntryType,
                               EntryTypeName = r1.EntryType.HasValue ? Enum.GetName(typeof(EntryType), r1.EntryType) : null,
                               r1.StudentName,
                               r1.RegistrationNo,
                               r1.RollNo,
                               r1.DOB,
                               r1.Gender,
                               GenderName = Enum.GetName(typeof(Gender), r1.Gender),
                               r1.BloodGroup,
                               r1.Religion,
                               r1.Nationality,
                               NationalityName = r1.Nationality.HasValue ? Enum.GetName(typeof(Nationality), r1.Nationality) : null,
                               r1.Category,
                               CategoryName = r1.Category.HasValue ? Enum.GetName(typeof(Category), r1.Category) : null,
                               r1.MobileNo,
                               r1.PhoneNo,
                               r1.EmailId,
                               r1.StudentPhoto,
                               r1.FathersName,
                               r1.MothersName,
                           };
                response.RegistrationOnlineList = list.ToList();
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("RegistrationOnlineListForGenerateLink")]
        public ExpandoObject RegistrationOnlineListForGenerateLink(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                if (model.ClassStreamTypeId == null)
                    model.ClassStreamTypeId = 0;

                if (model.SubjectOptionOne == null)
                    model.SubjectOptionOne = 0;

                if (model.SubjectOptionTwo == null)
                    model.SubjectOptionTwo = 0;

                if (model.ResultType == null)
                    model.ResultType = 0;

                SchoolDataContext dataContext = new SchoolDataContext();
                //var registrationOnlineList = dataContext.RegistrationOnlineListForGenerateLink(model.AcademicSessionId, model.ClassNo, model.RegistrationFormId, model.ClassStreamTypeId, model.SubjectOptionOne, model.SubjectOptionTwo, model.ResultType);
                //response.RegistrationOnlineList = registrationOnlineList;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("RegistrationOnlineListForGenerateAdmitCard")]
        public ExpandoObject RegistrationOnlineListForGenerateAdmitCard(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                if (model.ClassStreamTypeId == null)
                    model.ClassStreamTypeId = 0;

                if (model.SubjectOptionOne == null)
                    model.SubjectOptionOne = 0;

                if (model.SubjectOptionTwo == null)
                    model.SubjectOptionTwo = 0;

                SchoolDataContext dataContext = new SchoolDataContext();
                //var registrationOnlineList = dataContext.RegistrationOnlineListForGenerateAdmitCard(model.AcademicSessionId, model.ClassNo, model.RegistrationFormId, model.ClassStreamTypeId, model.SubjectOptionOne, model.SubjectOptionTwo);
                //response.RegistrationOnlineList = registrationOnlineList;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("RegistrationOnlineListForAdmitCard")]
        public ExpandoObject RegistrationOnlineListForAdmitCard(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //var registrationOnlineList = dataContext.RegistrationOnlineListForPrintAdmitCard(model.AcademicSessionId, model.ClassNo, model.RegistrationFormId, model.ClassStreamTypeId, model.SubjectOptionOne, model.SubjectOptionTwo, model.AllocatedClassStreamTypeId, model.AllocatedSubjectOptionId, model.ExamCenterId);
                //response.RegistrationOnlineList = registrationOnlineList;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("SearchRegistrationOnlineList")]
        public ExpandoObject SearchRegistrationOnlineList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from r1 in dataContext.RegistrationOnlines
                           where r1.RegistrationOnlineStatus != (byte)RegistrationOnlineStatus.Pending
                           select new
                           {
                               SearchRegistration = r1.RegistrationNo + "-" + r1.StudentName,
                               r1.RegistrationOnlineId,
                           };
                response.RegistrationOnlineList = list.ToList();
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("RegistrationOnlineListForAdmissionDetail")]
        public ExpandoObject RegistrationOnlineDetail(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //var registration = (from r1 in dataContext.RegistrationOnlines
                //                    join c1 in dataContext.Classes
                //                    on r1.ClassNo equals c1.ClassNo
                //                    join so1 in dataContext.SubjectOptions
                //                    on r1.SubjectOptionOne equals so1.SubjectOptionId into subSubjectOptionOne
                //                    from sSubjectOptionOne in subSubjectOptionOne.DefaultIfEmpty()
                //                    join so2 in dataContext.SubjectOptions
                //                    on r1.SubjectOptionTwo equals so2.SubjectOptionId into subSubjectOptionTwo
                //                    from sSubjectOptionTwo in subSubjectOptionTwo.DefaultIfEmpty()
                //                    join ps1 in dataContext.Students
                //                    on r1.PreviousSchoolAdmissionNo equals ps1.AdmissionNo into subPStudents
                //                    from sPreviousStudent in subPStudents.DefaultIfEmpty()
                //                    join a1 in dataContext.Admissions
                //                    on sPreviousStudent.StudentId equals a1.StudentId into subPAdmission
                //                    from sPAdmission in subPAdmission.DefaultIfEmpty()
                //                    join s1 in dataContext.Sections
                //                    on sPAdmission.SectionId equals s1.SectionId into subPSection
                //                    join st1 in dataContext.Students
                //                    on r1.RegistrationOnlineId equals st1.RegistrationOnlineId into subStudent
                //                    from sStudent in subStudent.DefaultIfEmpty()
                //                    join ad1 in dataContext.Admissions.Where(x => x.AcademicSessionId == model.AcademicSessionId)
                //                    on sStudent.StudentId equals ad1.StudentId into subAdmission
                //                    from sAdmission in subAdmission.DefaultIfEmpty()
                //                    join ss1 in dataContext.Sections
                //                    on sAdmission.SectionId equals ss1.SectionId into subSection
                //                    from sSection in subSection.DefaultIfEmpty()
                //                    join cl1 in dataContext.Classes
                //                    on sSection.ClassId equals cl1.ClassId into subClasses
                //                    from sClass in subClasses.DefaultIfEmpty()
                //                    where r1.RegistrationOnlineStatus != (byte)RegistrationOnlineStatus.Pending
                //                    && (model.AcademicSessionId == 0 || r1.AcademicSessionId == model.AcademicSessionId)
                //                    && (model.ClassNo == 0 || r1.ClassNo == model.ClassNo)
                //                    && (model.ClassStreamTypeId == null || r1.ClassStreamTypeId == model.ClassStreamTypeId)
                //                    && (model.SubjectOptionOne == null || r1.SubjectOptionOne == model.SubjectOptionOne)
                //                    && (model.SubjectOptionTwo == null || r1.SubjectOptionOne == model.SubjectOptionTwo)
                //                    && (model.IsSameSchool == null || r1.IsSameSchool == model.IsSameSchool)
                //                    && (model.RegistrationOnlineStatus == 0 || r1.RegistrationOnlineStatus == model.RegistrationOnlineStatus)
                //                    select new
                //                    {
                //                        r1.RegistrationOnlineId,
                //                        r1.AcademicSession.SessionName,
                //                        //BloodGroup = r1.BloodGroup > 0 ?Enum.GetName(typeof(BloodGroup), r1.BloodGroup).Replace("__","+").Replace("_","-"):"",
                //                        Gender = Enum.GetName(typeof(Gender), r1.Gender),
                //                        Category = Enum.GetName(typeof(Category), r1.Category).Replace("_", "/"),
                //                        ClassName = c1.ClassName2,
                //                        r1.ClassStreamType.ClassStreamTypeName,
                //                        r1.DOB,
                //                        r1.EmailId,
                //                        r1.FatherPresentAddress,
                //                        r1.FathersAnnualIncome,
                //                        r1.FathersDesignation,
                //                        r1.FathersMobileNo,
                //                        r1.FathersName,
                //                        r1.FathersOccupation,
                //                        r1.FathersOccupationDetail,
                //                        r1.FathersOrMothersEmail,
                //                        r1.FathersPhoto,
                //                        r1.FathersQualification,
                //                        r1.GuardiansAddress,
                //                        r1.GuardiansMobileNo,
                //                        r1.GuardiansName,
                //                        r1.IsSameSchool,
                //                        r1.MobileNo,
                //                        r1.MothersAnnualIncome,
                //                        r1.MothersMobileNo,
                //                        r1.MothersName,
                //                        r1.MothersOccupation,
                //                        r1.MothersPhoto,
                //                        r1.MothersQualification,
                //                        Nationality =Enum.GetName(typeof(Nationality), r1.Nationality),
                //                        r1.PreviousSchoolAdmissionNo,
                //                        PreviousSectionName = subPSection.Any() ? subPSection.First().SectionName : "",
                //                        sAdmission.RollNo,
                //                        sStudent.AdmissionNo,
                //                        sSection.SectionName,
                //                        NewClass = sClass.ClassName2,
                //                        r1.PreviousSchoolMedium,
                //                        r1.PreviousSchoolName,
                //                        r1.RegistrationDate,
                //                        r1.RegistrationNo,
                //                        Religion = Enum.GetName(typeof(Religion), r1.Religion),
                //                        r1.SMSAlertNo,
                //                        r1.StudentName,
                //                        r1.StudentPhoto,
                //                        r1.SubjectOptionOne,
                //                        SubjectOptionOneName = sSubjectOptionOne.SubjectOptionName,
                //                        r1.SubjectOptionTwo,
                //                        SubjectOptionTwoName = sSubjectOptionTwo.SubjectOptionName,
                //                        r1.WhatsappNo,
                //                    }).Distinct().ToList();
                //response.RegistrationOnlineList = registration;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("RegistrationOnlineDetail")]
        public ExpandoObject RegistrationOnlineDetail(int RegistrationOnlineId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var registration = (from r1 in dataContext.RegistrationOnlines
                                    join r2 in dataContext.RegistrationForms
                                    on r1.RegistrationFormId equals r2.RegistrationFormId
                                    join c1 in dataContext.Classes
                                    on r2.ClassNo equals c1.ClassNo
                                    where r1.RegistrationOnlineId == RegistrationOnlineId
                                    select new
                                    {
                                        r1.AcademicSessionId,
                                        r1.RegistrationOnlineId,
                                        r1.AcademicSession.SessionName,
                                        r1.BloodGroup,
                                        r1.Gender,
                                        GenderName = Enum.GetName(typeof(Gender), r1.Gender),
                                        r1.Category,
                                        CategoryName = Enum.GetName(typeof(Category), r1.Category).Replace("_", "/"),
                                        r2.ClassNo,
                                        c1.ClassName2,
                                        c1.ClassId,
                                        r2.PhaseName,
                                        r1.DOB,
                                        r1.EmailId,
                                        r1.FathersName,
                                        r1.MobileNo,
                                        r1.MothersName,
                                        r1.Nationality,
                                        r1.RegistrationDate,
                                        r1.RegistrationNo,
                                        r1.Religion,
                                        r1.StudentName,
                                        r1.StudentPhoto,
                                    }).First();
                response.RegistrationOnline = registration;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveRegistrationOnline")]
        public ExpandoObject SaveRegistrationOnline(RegistrationOnline model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                RegistrationOnline registrationOnline = dataContext.RegistrationOnlines.First(x => x.RegistrationOnlineId == model.RegistrationOnlineId);
                var registrationForm = dataContext.RegistrationForms.First(x => x.RegistrationFormId == model.RegistrationFormId);
                if (registrationForm.FromDOB.HasValue)
                {
                    if (registrationForm.FromDOB.Value > model.DOB.Date)
                    {
                        response.Message = "Date Of Birth Should not be less than " + registrationForm.FromDOB.Value.ToString("dd-MM-yyyy");
                        return response;
                    }
                }

                if (registrationForm.ToDOB.HasValue)
                {
                    if (registrationForm.ToDOB.Value < model.DOB.Date)
                    {
                        response.Message = "Date Of Birth Should not be more than " + registrationForm.ToDOB.Value.ToString("dd-MM-yyyy");
                        return response;
                    }
                }
                registrationOnline.DOB = model.DOB;
                registrationOnline.StudentName = model.StudentName;
                registrationOnline.Gender = model.Gender;
                registrationOnline.EmailId = model.EmailId;
                registrationOnline.MobileNo = model.MobileNo;
                registrationOnline.BloodGroup = model.BloodGroup;
                registrationOnline.Religion = model.Religion;
                registrationOnline.Nationality = model.Nationality;
                registrationOnline.Category = model.Category;
                registrationOnline.FathersName = model.FathersName;
                registrationOnline.MothersName = model.MothersName;
                dataContext.SubmitChanges();

                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }


        [HttpPost]
        [Route("GenerateAdmitCardOR")]
        public ExpandoObject GenerateAdmitCardOR(List<RegistrationOnlineModel> registrationOnlines)
        {
            dynamic response = new ExpandoObject();
            try
            {
                //SchoolDataContext dataContext = new SchoolDataContext();
                //registrationOnlines.Where(x => x.IsChecked).ToList().ForEach(r1 =>
                //  {
                //      //var registrationOnline = dataContext.RegistrationOnlines.Where(x => r1.RegistrationOnlineId == x.RegistrationOnlineId).First();
                //      //if (registrationOnline.AllocatedSubjectOptionId == null)
                //      //{
                //      //    registrationOnline.AllocatedSubjectOptionId = registrationOnline.SubjectOptionOne;
                //      //    dataContext.SubmitChanges();
                //      //}
                //      //int classStreamTypeId = dataContext.SubjectOptions.Where(x => x.SubjectOptionId == registrationOnline.AllocatedSubjectOptionId).First().ClassStreamTypeId;
                //      //int classNo = 0;
                //      //if (registrationOnline.ClassNo > 0)
                //      //    classNo = registrationOnline.ClassNo;
                //      //registrationOnline.ExamCenterId = r1.ExamCenterId;
                //      //registrationOnline.RollNo = dataContext.GenerateRollNoForRO(classStreamTypeId, registrationOnline.AcademicSessionId, classNo);
                //      //if (registrationOnline.ResultType == 3)
                //      //    registrationOnline.ResultType = null;
                //      //registrationOnline.AdmitCardGeneratedDate = DateTime.Now;
                //      //dataContext.SubmitChanges();

                //  });

                //registrationOnlines.Where(x => x.IsChecked).ToList().ForEach(r1 =>
                //{
                //    var registrationOnline = dataContext.RegistrationOnlines.Where(x => r1.RegistrationOnlineId == x.RegistrationOnlineId).First();
                //    var examCenter = dataContext.ExamCenters.Where(x => x.ExamCenterId == r1.ExamCenterId).First();
                //    string msg = "Dear%20XI%20Applicant%2C%20Please%20visit%20school%20website%20to%20download%20admit%20card.%20Test%20is%20on%20" + examCenter.ExamDate.ToString("dd-MM-yyyy") + "%20.%20Reporting%20time%20" + examCenter.ReportingTime.Replace(" ", "%20") + "%20%3A%20GGPS%2C%20Bokaro";
                //    SMS.SendSmsNew(registrationOnline.SMSAlertNo, msg);
                //});
                //response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SendAdmitCardSMSRO")]
        public ExpandoObject SendAdmitCardSMSRO(RegistrationOnlineModel r1)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var registrationOnline = dataContext.RegistrationOnlines.Where(x => r1.RegistrationOnlineId == x.RegistrationOnlineId).First();
                var examCenter = dataContext.ExamCenters.Where(x => x.ExamCenterId == r1.ExamCenterId).First();
                //"Dear XI Applicant, Please visit school website to download admit card. Test is on %% . Reporting time %% : GGPS, Bokaro"
                string msg = "Dear%20XI%20Applicant%2C%20Please%20visit%20school%20website%20to%20download%20admit%20card.%20Test%20is%20on%20" + examCenter.ExamDate.ToString("dd-MM-yyyy") + "%20.%20Reporting%20time%20" + examCenter.ReportingTime.Replace(" ", "%20") + "%20%3A%20GGPS%2C%20Bokaro";
                SMS.SendSmsNew(registrationOnline.MobileNo, msg);
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SendAdmitCardSMSROALL")]
        public ExpandoObject SendAdmitCardSMSROALL(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from r1 in dataContext.RegistrationOnlines
                           where r1.RegistrationOnlineStatus == (byte)RegistrationOnlineStatus.Registered
                           && r1.RollNo != null
                           && r1.AcademicSessionId == model.AcademicSessionId
                           && (model.ClassNo == 0 || r1.ClassNo == model.ClassNo)
                           && (r1.ResultType == null || r1.ResultType == 3)
                           select r1;
                list.ToList().ForEach(registrationOnline =>
                {
                    var examCenter = dataContext.ExamCenters.Where(x => x.ExamCenterId == registrationOnline.ExamCenterId).First();
                    string msg = "Dear%20XI%20Applicant%2C%20Please%20visit%20school%20website%20to%20download%20admit%20card.%20Test%20is%20on%20" + examCenter.ExamDate.ToString("dd-MM-yyyy") + "%20.%20Reporting%20time%20" + examCenter.ReportingTime.Replace(" ", "%20") + "%20%3A%20GGPS%2C%20Bokaro";
                    SMS.SendSmsNew(registrationOnline.MobileNo, msg);
                });


                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("RegistrationOnlineListForResult")]
        public ExpandoObject RegistrationOnlineListForResult(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //var list = from r1 in dataContext.RegistrationOnlines
                //           join st in dataContext.ClassStreamTypes
                //           on r1.ClassStreamTypeId equals st.ClassStreamTypeId into st2
                //           join sp1 in dataContext.SubjectOptions
                //           on r1.SubjectOptionOne equals sp1.SubjectOptionId into spp1
                //           join sp2 in dataContext.SubjectOptions
                //            on r1.SubjectOptionTwo equals sp2.SubjectOptionId into spp2
                //           where r1.RegistrationOnlineStatus != (byte)RegistrationOnlineStatus.Pending
                //           && r1.RollNo != null
                //           && r1.AcademicSessionId == model.AcademicSessionId
                //           && (model.ClassNo == 0 || r1.ClassNo == model.ClassNo)
                //           && (model.ClassStreamTypeId == null || r1.ClassStreamTypeId == model.ClassStreamTypeId)
                //           && (model.SubjectOptionOne == null || r1.SubjectOptionOne == model.SubjectOptionOne)
                //           && (model.SubjectOptionTwo == null || r1.SubjectOptionTwo == model.SubjectOptionTwo)
                //           && (model.RegistrationFormId == 0 || r1.ConfirmRegistrationFormId == model.RegistrationFormId)
                //           && (model.IsSameSchool.HasValue ? r1.IsSameSchool == model.IsSameSchool : true)
                //           select new
                //           {
                //               r1.RegistrationOnlineId,
                //               r1.RollNo,
                //               r1.ResultType,
                //               r1.StudentName,
                //               r1.FathersName,
                //               r1.RegistrationNo,
                //               r1.ClassStreamTypeId,
                //               r1.SubjectOptionOne,
                //               r1.SubjectOptionTwo,
                //               r1.ClassNo,
                //               ClassName = dataContext.Classes.First(x => x.ClassNo == r1.ClassNo).ClassName2,
                //               r1.ClassStreamType.ClassStreamTypeName,
                //               SubjectOptionOneName = spp1.Any() ? spp1.First().SubjectOptionName : "",
                //               SubjectOptionTwoName = spp2.Any() ? spp2.First().SubjectOptionName : "",
                //               AllocatedClassStreamTypeName = r1.AllocatedSubjectOptionId.HasValue ? dataContext.SubjectOptions.First(x => x.SubjectOptionId == r1.AllocatedSubjectOptionId).ClassStreamType.ClassStreamTypeName : "",
                //               AllocatedSubjectOptionName = r1.AllocatedSubjectOptionId.HasValue ? dataContext.SubjectOptions.First(x => x.SubjectOptionId == r1.AllocatedSubjectOptionId).SubjectOptionName : "",
                //           };
                //response.RegistrationOnlineList = list.ToList();
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveRegistrationOnlineResult")]
        public ExpandoObject SaveRegistrationOnlineResult(List<RegistrationOnlineModel> registrationOnlines)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                registrationOnlines.ForEach(r1 =>
                {
                    var registrationOnline = dataContext.RegistrationOnlines.Where(x => r1.RegistrationOnlineId == x.RegistrationOnlineId).First();
                    registrationOnline.ResultType = r1.ResultType;
                    dataContext.SubmitChanges();
                });
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("RegistrationOnlineResultList")]
        public ExpandoObject RegistrationOnlineResultList(RegistrationOnlineModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //var list = from r1 in dataContext.RegistrationOnlines
                //           join st in dataContext.ClassStreamTypes
                //           on r1.ClassStreamTypeId equals st.ClassStreamTypeId into st2
                //           join sp1 in dataContext.SubjectOptions
                //           on r1.SubjectOptionOne equals sp1.SubjectOptionId into spp1
                //           join sp2 in dataContext.SubjectOptions
                //            on r1.SubjectOptionTwo equals sp2.SubjectOptionId into spp2
                //           where r1.RegistrationOnlineStatus != (byte)RegistrationOnlineStatus.Pending
                //           //&& r1.RollNo != null
                //           && r1.ResultType.HasValue
                //           && r1.AcademicSessionId == model.AcademicSessionId
                //           && (model.ClassNo == 0 || r1.ClassNo == model.ClassNo)
                //           && (model.ClassStreamTypeId == null || r1.ClassStreamTypeId == model.ClassStreamTypeId)
                //           && (model.SubjectOptionOne == null || r1.SubjectOptionOne == model.SubjectOptionOne)
                //           && (model.SubjectOptionTwo == null || r1.SubjectOptionTwo == model.SubjectOptionTwo)
                //           && (model.RegistrationFormId == 0 || r1.ConfirmRegistrationFormId == model.RegistrationFormId)
                //           && (model.IsSameSchool.HasValue ? r1.IsSameSchool == model.IsSameSchool : true)
                //           && (model.ResultType == null || model.ResultType == r1.ResultType)
                //           select new
                //           {
                //               r1.RegistrationOnlineId,
                //               r1.RollNo,
                //               r1.ResultType,
                //               r1.StudentName,
                //               r1.FathersName,
                //               r1.RegistrationNo,
                //               r1.ClassStreamTypeId,
                //               r1.SubjectOptionOne,
                //               r1.SubjectOptionTwo,
                //               r1.ClassNo,
                //               r1.MobileNo,
                //               r1.SMSAlertNo,
                //               r1.WhatsappNo,
                //               ClassName = dataContext.Classes.First(x => x.ClassNo == r1.ClassNo).ClassName2,
                //               r1.ClassStreamType.ClassStreamTypeName,
                //               SubjectOptionOneName = spp1.Any() ? spp1.First().SubjectOptionName : "",
                //               SubjectOptionTwoName = spp2.Any() ? spp2.First().SubjectOptionName : "",
                //               AllocatedClassStreamTypeName = r1.AllocatedSubjectOptionId.HasValue ? dataContext.SubjectOptions.First(x => x.SubjectOptionId == r1.AllocatedSubjectOptionId).ClassStreamType.ClassStreamTypeName : "",
                //               AllocatedSubjectOptionName = r1.AllocatedSubjectOptionId.HasValue ? dataContext.SubjectOptions.First(x => x.SubjectOptionId == r1.AllocatedSubjectOptionId).SubjectOptionName : "",
                //           };
                //response.RegistrationOnlineList = list.ToList();
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

    }
}
