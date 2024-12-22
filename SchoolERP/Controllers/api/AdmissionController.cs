using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SchoolERP.Models;
using System.Dynamic;
using School;
using System.Data.Common;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/admission")]
    public class AdmissionController : ApiController
    {
        [HttpPost]
        [Route("newAdmission")]
        public ExpandoObject NewAdmission(AdmissionModel admissionModel)
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

                var admissionFees = (from sc in dataContext.AdmissionFees
                                     join afh in dataContext.AdmissionFeeHeads
                                     on sc.AdmissionFeeHeadId equals afh.AdmissionFeeHeadId
                                     where afh.AcademicSessionId == admissionModel.AcademicSessionId
                                     && afh.AdmissionType == (byte)AdmissionType.NewAdmission
                                     && afh.ClassId == admissionModel.ClassId
                                     && afh.StudentTypeId == admissionModel.Student.StudentTypeId
                                     && sc.IsActive == true
                                     select sc);
                //if (admissionFees.Count() == 0)
                //{
                //    response.Message = "Admission Fee has not been defined for this class and Session.";
                //    return response;
                //}
                string admissioNo = LoadData.GenerateAdmissionNo(admissionModel.AcademicSessionId.Value, admissionModel.SectionId.Value);
                if (String.IsNullOrEmpty(admissioNo))
                    admissioNo = admissionModel.Student.AdmissionNo;

                // For the Student.
                Student student = new Student
                {
                    AadhaarNo = admissionModel.Student.AadhaarNo,
                    AlternateNo = admissionModel.Student.AlternateNo,
                    StudentTypeId = admissionModel.Student.StudentTypeId,
                    Category = admissionModel.Student.Category,
                    PermanentAddressLine1 = admissionModel.Student.PermanentAddressLine1,
                    PermanentAddressLine2 = admissionModel.Student.PermanentAddressLine2,
                    CorrespondenceCityId = admissionModel.Student.CorrespondenceCityId,
                    CorrespondenceAddressLine1 = admissionModel.Student.CorrespondenceAddressLine1,
                    CorrespondenceAddressLine2 = admissionModel.Student.CorrespondenceAddressLine2,
                    PermanentCityId = admissionModel.Student.PermanentCityId,
                    DateOfBirth = admissionModel.Student.DateOfBirth,
                    Email = admissionModel.Student.Email,
                    EmployeeId = null,
                    EntryDate = DateTime.Now,
                    FamilyAnnualIncome = admissionModel.Student.FamilyAnnualIncome,
                    FatherName = admissionModel.Student.FatherName,
                    FatherOccupation = admissionModel.Student.FatherOccupation,
                    FatherAadhaarNo = admissionModel.Student.FatherAadhaarNo,
                    FullName = admissionModel.Student.FullName,
                    Gender = admissionModel.Student.Gender,
                    JoiningDate = DateTime.Now,
                    MobileNo = admissionModel.Student.MobileNo,
                    MotherName = admissionModel.Student.MotherName,
                    MotherOccupation = admissionModel.Student.MotherOccupation,
                    MotherAadharNo = admissionModel.Student.MotherAadharNo,
                    Nationality = admissionModel.Student.Nationality,
                    Photo = null,
                    RegistrationDate = admissionModel.Student.RegistrationDate,
                    Religion = admissionModel.Student.Religion,
                    StudentStatus = Convert.ToByte(StudentStatus.Active),
                    AdmissionNo = admissioNo,// LoadData.GenerateAdmissionNo(admissionModel.AcademicSessionId.Value, admissionModel.SectionId.Value),
                    BloodGroup = admissionModel.Student.BloodGroup,
                    HouseId = admissionModel.Student.HouseId,
                    TransferCertificateIssue = false,
                    CharacterCertificateIssue = false,
                    PermanentPinCode = admissionModel.Student.PermanentPinCode,
                    CorrespondecePinCode = admissionModel.Student.CorrespondecePinCode,
                    PreviousSchoolName = admissionModel.Student.PreviousSchoolName,
                    PreviousBoardName = admissionModel.Student.PreviousBoardName,
                    LastClassAttend = admissionModel.Student.LastClassAttend,
                    MediumOfInstruction = admissionModel.Student.MediumOfInstruction,
                    PreviousSchoolTCNo = admissionModel.Student.PreviousSchoolTCNo,
                    PreviousSchoolTCIssueDate = admissionModel.Student.PreviousSchoolTCIssueDate,
                    IntermediatePreviousSchoolName=admissionModel.Student.IntermediatePreviousSchoolName,
                    IntermediateLastClassAttend =admissionModel.Student.IntermediateLastClassAttend,
                    IntermediateMediumOfInstruction =admissionModel.Student.IntermediateMediumOfInstruction,
                    IntermediatePreviousBoardName =admissionModel.Student.IntermediatePreviousBoardName,
                    IntermediatePreviousSchoolTCIssueDate= admissionModel.Student.IntermediatePreviousSchoolTCIssueDate,
                    IntermediatePreviousSchoolTCNo=admissionModel.Student.IntermediatePreviousSchoolTCNo,


                    

                    Remarks = admissionModel.Student.Remarks,
                };
                if (dataContext.Houses.Where(x => x.HouseName == admissionModel.Student.HouseName).Any())
                    student.HouseId = dataContext.Houses.Where(x => x.HouseName == admissionModel.Student.HouseName).First().HouseId;
                else if (!string.IsNullOrEmpty(admissionModel.Student.HouseName))
                {
                    House house = new House
                    {
                        HouseName = admissionModel.Student.HouseName,
                        HouseStatus = 1
                    };
                    dataContext.Houses.InsertOnSubmit(house);
                    dataContext.SubmitChanges();
                    student.HouseId = house.HouseId;
                }
                student.LoginPassword = CryptoEngine.Encrypt(student.AdmissionNo);
                dataContext.Students.InsertOnSubmit(student);
                dataContext.SubmitChanges();

                // For Admission
                Admission admission = new Admission
                {
                    StudentId = student.StudentId,
                    AdmissionType = (byte)AdmissionType.NewAdmission,
                    AcademicSessionId = admissionModel.AcademicSessionId,
                    AdmissionDate = admissionModel.AdmissionDate,
                    AdmissionStatus = Convert.ToByte(AdmissionStatus.Active),
                    SectionId = admissionModel.SectionId,
                    LeaveDate = null,
                    StaffWard = admissionModel.StaffWard
                };


                var rolls = (from r1 in dataContext.Admissions
                             where r1.SectionId == admission.SectionId
                             && r1.AcademicSessionId == admission.AcademicSessionId
                             orderby r1.RollNo descending
                             select r1);
                if (rolls.Count() > 0)
                {
                    var rollNo = (from r1 in dataContext.Admissions
                                  where r1.SectionId == admission.SectionId
                                  && r1.AcademicSessionId == admission.AcademicSessionId
                                  orderby r1.RollNo descending
                                  select r1.RollNo).First();
                    admission.RollNo = rollNo + 1;
                }
                else
                {
                    admission.RollNo = 1;
                }
                dataContext.Admissions.InsertOnSubmit(admission);
                dataContext.SubmitChanges();

                if (admissionModel.AdmissionType == (byte)AdmissionType.NewAdmission)
                {
                    foreach (var admissionFee in admissionFees)
                    {
                        if (admissionFee.Amount <= 0)
                            continue;
                        StudentCharge studentCharge = new StudentCharge
                        {
                            AdmissionId = admission.AdmissionId,
                            FeeHeadId = admissionFee.AdmissionFeeHead.FeeHeadId,
                            StudentChargeStatus = (byte)FineStatus.Active,
                            FeeType = (byte)FeeType.Admission,
                            FineAmount = admissionFee.Amount,
                            Comments = "Taken at the time of admission",
                            IsRefundable = admissionFee.IsRefundable,
                            FineDate = admission.AdmissionDate
                        };
                        dataContext.StudentCharges.InsertOnSubmit(studentCharge);
                        dataContext.SubmitChanges();
                    }
                }

                if (admissionModel.StudentRegistrationId > 0)
                {
                    var studentRegistration = dataContext.StudentRegistrations.Where(x => x.StudentRegistrationId == admissionModel.StudentRegistrationId).First();
                    studentRegistration.RegistrationStatus = (byte)RegistrationStatus.Registered;
                    student.RegistrationId = studentRegistration.StudentRegistrationId;
                    dataContext.SubmitChanges();
                }

                //Student Subject Combination
                if (admissionModel.StudentSubjectCombination.SubjectCombinationId > 0)
                {
                    var StudentSubjectCombinationModel = admissionModel.StudentSubjectCombination;
                    StudentSubjectCombination StudentSubjectCombination = new StudentSubjectCombination
                    {
                        AdmissionId = admission.AdmissionId,
                        SubjectCombinationId = StudentSubjectCombinationModel.SubjectCombinationId,
                        StartDate = StudentSubjectCombinationModel.StartDate,
                        EndDate = StudentSubjectCombinationModel.EndDate
                    };
                    dataContext.StudentSubjectCombinations.InsertOnSubmit(StudentSubjectCombination);
                    dataContext.SubmitChanges();
                }

                //Student Transport
                if (admissionModel.StudentTransport.TransportGroupId > 0)
                {
                    var model = admissionModel.StudentTransport;
                    StudentTransport transport = new StudentTransport
                    {
                        StartDate = model.StartDate,
                        BusId = model.BusId,
                        PickupPoint = student.CorrespondenceAddressLine1 != null ? student.CorrespondenceAddressLine1 : (student.PermanentAddressLine1 == null ? "" : student.PermanentAddressLine1),
                        PickupTime = model.PickupTime,
                        EndDate = model.EndDate,
                        Status = model.Status,
                        StudentId = student.StudentId,
                        TransportGroupId = model.TransportGroupId
                    };
                    dataContext.StudentTransports.InsertOnSubmit(transport);
                    dataContext.SubmitChanges();
                }

                if (admissionModel.Student.RegistrationOnlineId > 0)
                {
                    var registrationOnline = dataContext.RegistrationOnlines.First(x => x.RegistrationOnlineId == admissionModel.Student.RegistrationOnlineId);
                    registrationOnline.RegistrationOnlineStatus = (byte)RegistrationOnlineStatus.Admitted;
                    dataContext.SubmitChanges();

                    student.RegistrationOnlineId = admissionModel.Student.RegistrationOnlineId;
                    student.RegistrationDate = registrationOnline.RegistrationDate;
                    dataContext.SubmitChanges();


                    //Check Paid Admission Fee
                    var feeCollections = dataContext.FeeCollections.Where(x => x.RegistrationOnlineId == registrationOnline.RegistrationOnlineId && x.PaymentFor == (byte)PaymentFor.Fees).ToList();
                    feeCollections.ForEach(collection =>
                    {
                        collection.AdmissionId = admission.AdmissionId;
                        collection.RegistrationOnlineId = null;
                        collection.LastUpdatedBy = SessionUtilies.GetLoginId();
                        collection.LastUpdatedOn = DateTime.Now;
                    });
                    dataContext.SubmitChanges();
                }

                transaction.Commit();
                IsCompleted = true;
                response.AdmissionId = admission.AdmissionId;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                if (ex.Message.Contains("CK_Student"))
                    response.Message = "This Student is already exists.";
                else if (ex.Message.Contains("UQ_Student_RegistrationNo"))
                    response.Message = "This Admission no is already exists.";
                else
                    response.Message = ex.Message.ToString();
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpGet]
        [Route("admissionList")]
        public ExpandoObject AdmissionList(int AcademicSessionId, int ClassId, int SectionId, byte AdmissionFrom)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Admissions
                            join sc1 in dataContext.Sections
                            on a1.SectionId equals sc1.SectionId
                            join c1 in dataContext.Classes
                            on sc1.ClassId equals c1.ClassId
                            join s1 in dataContext.Students
                            on a1.StudentId equals s1.StudentId
                            join pc1 in dataContext.Cities
                            on s1.PermanentCityId equals pc1.CityId into subPC
                            from sPC in subPC.DefaultIfEmpty()

                            join stp in dataContext.States
                            on sPC.StateId equals stp.StateId into subPState
                            from stpState in subPState.DefaultIfEmpty()

                            join cc1 in dataContext.Cities
                            on s1.CorrespondenceCityId equals cc1.CityId into subCC
                            from sCC in subCC.DefaultIfEmpty()

                            join stc in dataContext.States
                            on sCC.StateId equals stc.StateId into subCState
                            from stcSt in subCState.DefaultIfEmpty()
                            join h1 in dataContext.Houses
                            on s1.HouseId equals h1.HouseId into subHouses
                            where (AcademicSessionId == 0 || a1.AcademicSessionId == AcademicSessionId)
                            && (ClassId == 0 || c1.ClassId == ClassId)
                            && (SectionId == 0 || sc1.SectionId == SectionId)
                            && s1.StudentStatus != (byte)StudentStatus.Left
                            && (AdmissionFrom == 0 ? true : (AdmissionFrom == (byte)School.AdmissionFrom.Online ? s1.RegistrationOnlineId != null : s1.RegistrationOnlineId == null))
                            orderby a1.AdmissionId descending
                            select new AdmissionModel
                            {
                                StudentId = a1.StudentId,
                                AcademicSessionId = a1.AcademicSessionId,
                                SessionName = a1.AcademicSession.SessionName,
                                SectionId = a1.SectionId,
                                SectionName = sc1.SectionName,
                                ClassId = sc1.ClassId,
                                ClassName = c1.ClassName,
                                RollNo = a1.RollNo,
                                AdmissionId = a1.AdmissionId,
                                AdmissionDate = a1.AdmissionDate,
                                AdmissionStatus = a1.AdmissionStatus,
                                AdmissionType = a1.AdmissionType,
                                LeaveDate = a1.LeaveDate,
                                StaffWard = a1.StaffWard,
                                Student = new StudentModel
                                {
                                    AadhaarNo = s1.AadhaarNo,
                                    AdmissionNo = s1.AdmissionNo,
                                    AlternateNo = s1.AlternateNo,
                                    BloodGroup = s1.BloodGroup,
                                    Category = s1.Category,
                                    CharacterCertificateIssue = s1.CharacterCertificateIssue,
                                    CorrespondenceAddressLine1 = s1.CorrespondenceAddressLine1,
                                    CorrespondenceAddressLine2 = s1.CorrespondenceAddressLine2,
                                    CorrespondenceCityId = s1.CorrespondenceCityId,
                                    CorrespondenceCityName = sCC.CityName,
                                    CorrespondenceStateId = stcSt.StateId,
                                    CorrespondenceStateName = stcSt.StateName,
                                    PermanentStateId = sPC.StateId,
                                    PermanentCityId = s1.PermanentCityId,
                                    PermanentCityName = sPC.CityName,
                                    PermanentStateName = stpState.StateName,
                                    PermanentAddressLine1 = s1.PermanentAddressLine1,
                                    PermanentAddressLine2 = s1.PermanentAddressLine2,
                                    DateOfBirth = s1.DateOfBirth,
                                    Email = s1.Email,
                                    EmployeeId = s1.EmployeeId,
                                    EntryDate = s1.EntryDate,
                                    FamilyAnnualIncome = s1.FamilyAnnualIncome,
                                    FatherAadhaarNo = s1.FatherAadhaarNo,
                                    FatherName = s1.FatherName,
                                    FatherOccupation = s1.FatherOccupation,
                                    FullName = s1.FullName,
                                    Gender = s1.Gender,
                                    HouseId = s1.HouseId,
                                    HouseName = subHouses.Any() ? subHouses.First().HouseName : "",
                                    JoiningDate = s1.JoiningDate,
                                    MobileNo = s1.MobileNo,
                                    MotherAadharNo = s1.MotherAadharNo,
                                    MotherName = s1.MotherName,
                                    MotherOccupation = s1.MotherOccupation,
                                    Nationality = s1.Nationality,
                                    ParentId = s1.ParentId,
                                    RegistrationDate = s1.RegistrationDate,
                                    RegistrationId = s1.RegistrationId,
                                    Religion = s1.Religion,
                                    StudentId = s1.StudentId,
                                    StudentStatus = (byte)s1.StudentStatus,
                                    StudentTypeId = s1.StudentTypeId,
                                    TypeName = s1.StudentType.TypeName,
                                    TransferCertificateIssue = s1.TransferCertificateIssue,
                                    Remarks = s1.Remarks,
                                    PermanentPinCode = s1.PermanentPinCode,
                                    CorrespondecePinCode = s1.CorrespondecePinCode,
                                    PreviousSchoolName = s1.PreviousSchoolName,
                                    PreviousBoardName = s1.PreviousBoardName,
                                    LastClassAttend = s1.LastClassAttend,
                                    MediumOfInstruction = s1.MediumOfInstruction,
                                    PreviousSchoolTCNo = s1.PreviousSchoolTCNo,
                                    PreviousSchoolTCIssueDate = s1.PreviousSchoolTCIssueDate,
                                }
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

        [HttpGet]
        [Route("AdmissionListForLibrary")]
        public ExpandoObject AdmissionListForLibrary(int AcademicSessionId, int ClassNo, int SectionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Admissions
                            join sc1 in dataContext.Sections
                            on a1.SectionId equals sc1.SectionId
                            join c1 in dataContext.Classes
                            on sc1.ClassId equals c1.ClassId
                            join s1 in dataContext.Students
                            on a1.StudentId equals s1.StudentId
                            where a1.AcademicSessionId == AcademicSessionId
                            && (ClassNo == 0 ? true : c1.ClassNo == ClassNo)
                            && (SectionId > 0 ? sc1.SectionId == SectionId : true)
                            && s1.StudentStatus == (byte)StudentStatus.Active
                            && a1.AdmissionStatus == (byte)AdmissionStatus.Active
                            orderby a1.AdmissionId descending
                            select new
                            {
                                a1.AdmissionId,
                                a1.RollNo,
                                sc1.SectionName,
                                c1.ClassName2,
                                s1.StudentId,
                                s1.AdmissionNo,
                                s1.FullName
                            }).Distinct();

                response.AdmissionList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("NewAdmissionList")]
        public ExpandoObject NewAdmissionList(int AcademicSessionId, DateTime? FromDate, DateTime? ToDate)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Admissions
                            join sc1 in dataContext.Sections
                            on a1.SectionId equals sc1.SectionId
                            join c1 in dataContext.Classes
                            on sc1.ClassId equals c1.ClassId
                            join s1 in dataContext.Students
                            on a1.StudentId equals s1.StudentId
                            join pc1 in dataContext.Cities
                            on s1.PermanentCityId equals pc1.CityId into subPC
                            from sPC in subPC.DefaultIfEmpty()

                            join stp in dataContext.States
                            on sPC.StateId equals stp.StateId into subPState
                            from stpState in subPState.DefaultIfEmpty()

                            join cc1 in dataContext.Cities
                            on s1.CorrespondenceCityId equals cc1.CityId into subCC
                            from sCC in subCC.DefaultIfEmpty()

                            join stc in dataContext.States
                            on sCC.StateId equals stc.StateId into subCState
                            from stcSt in subCState.DefaultIfEmpty()
                            where a1.AcademicSessionId == AcademicSessionId
                            && a1.AdmissionType == (byte)AdmissionType.NewAdmission
                            && (FromDate.HasValue ? a1.AdmissionDate >= FromDate : true)
                            && (ToDate.HasValue ? a1.AdmissionDate <= ToDate : true)
                            orderby a1.AdmissionId descending
                            select new AdmissionModel
                            {
                                StudentId = a1.StudentId,
                                AcademicSessionId = a1.AcademicSessionId,
                                SessionName = a1.AcademicSession.SessionName,
                                SectionId = a1.SectionId,
                                SectionName = sc1.SectionName,
                                ClassId = sc1.ClassId,
                                ClassName = c1.ClassName,
                                RollNo = a1.RollNo,
                                AdmissionId = a1.AdmissionId,
                                AdmissionDate = a1.AdmissionDate,
                                AdmissionStatus = a1.AdmissionStatus,
                                AdmissionType = a1.AdmissionType,
                                LeaveDate = a1.LeaveDate,
                                StaffWard = a1.StaffWard,
                                Student = new StudentModel
                                {
                                    AadhaarNo = s1.AadhaarNo,
                                    AdmissionNo = s1.AdmissionNo,
                                    AlternateNo = s1.AlternateNo,
                                    BloodGroup = s1.BloodGroup,
                                    Category = s1.Category,
                                    CharacterCertificateIssue = s1.CharacterCertificateIssue,
                                    CorrespondenceAddressLine1 = s1.CorrespondenceAddressLine1,
                                    CorrespondenceAddressLine2 = s1.CorrespondenceAddressLine2,
                                    CorrespondenceCityId = s1.CorrespondenceCityId,
                                    CorrespondenceCityName = sCC.CityName,
                                    CorrespondenceStateId = stcSt.StateId,
                                    CorrespondenceStateName = stcSt.StateName,
                                    PermanentStateId = sPC.StateId,
                                    PermanentCityId = s1.PermanentCityId,
                                    PermanentCityName = sPC.CityName,
                                    PermanentStateName = stpState.StateName,
                                    PermanentAddressLine1 = s1.PermanentAddressLine1,
                                    PermanentAddressLine2 = s1.PermanentAddressLine2,
                                    DateOfBirth = s1.DateOfBirth,
                                    Email = s1.Email,
                                    EmployeeId = s1.EmployeeId,
                                    EntryDate = s1.EntryDate,
                                    FamilyAnnualIncome = s1.FamilyAnnualIncome,
                                    FatherAadhaarNo = s1.FatherAadhaarNo,
                                    FatherName = s1.FatherName,
                                    FatherOccupation = s1.FatherOccupation,
                                    FullName = s1.FullName,
                                    Gender = s1.Gender,
                                    HouseId = s1.HouseId,
                                    HouseName = s1.House.HouseName,
                                    JoiningDate = s1.JoiningDate,
                                    MobileNo = s1.MobileNo,
                                    MotherAadharNo = s1.MotherAadharNo,
                                    MotherName = s1.MotherName,
                                    MotherOccupation = s1.MotherOccupation,
                                    Nationality = s1.Nationality,
                                    ParentId = s1.ParentId,
                                    RegistrationDate = s1.RegistrationDate,
                                    RegistrationId = s1.RegistrationId,
                                    Religion = s1.Religion,
                                    StudentId = s1.StudentId,
                                    StudentStatus = (byte)s1.StudentStatus,
                                    StudentTypeId = s1.StudentTypeId,
                                    TypeName = s1.StudentType.TypeName,
                                    TransferCertificateIssue = s1.TransferCertificateIssue,
                                    Remarks = s1.Remarks,
                                    PermanentPinCode = s1.PermanentPinCode,
                                    CorrespondecePinCode = s1.CorrespondecePinCode,
                                    PreviousSchoolName = s1.PreviousSchoolName,
                                    PreviousBoardName = s1.PreviousBoardName,
                                    LastClassAttend = s1.LastClassAttend,
                                    MediumOfInstruction = s1.MediumOfInstruction,
                                    PreviousSchoolTCNo = s1.PreviousSchoolTCNo,
                                    PreviousSchoolTCIssueDate = s1.PreviousSchoolTCIssueDate,
                                    IntermediateLastClassAttend=s1.IntermediateLastClassAttend,
                                    IntermediateMediumOfInstruction=s1.IntermediateMediumOfInstruction,
                                    IntermediatePreviousBoardName=s1.IntermediatePreviousBoardName,
                                         IntermediatePreviousSchoolName=s1.IntermediatePreviousSchoolName,               IntermediatePreviousSchoolTCIssueDate=s1.IntermediatePreviousSchoolTCIssueDate,
                                    IntermediatePreviousSchoolTCNo=s1.IntermediatePreviousSchoolTCNo,
                                }
                            });

                response.AdmissionList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("SearchAdmissionList")]
        public ExpandoObject SearchAdmissionList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from s1 in dataContext.Students
                            join a1 in dataContext.Admissions
                            on s1.StudentId equals a1.StudentId
                            join s2 in dataContext.Sections
                            on a1.SectionId equals s2.SectionId into subSection
                            from sSection in subSection.DefaultIfEmpty()
                            join c1 in dataContext.Classes
                            on sSection.ClassId equals c1.ClassId into subClass
                            from sClass in subClass.DefaultIfEmpty()
                            where a1.AdmissionStatus == (byte)AdmissionStatus.Active
                            orderby s1.AdmissionNo descending
                            select new
                            {
                                s1.StudentId,
                                s1.AdmissionNo,
                                SearchAdmission = s1.AdmissionNo + " (" + s1.FullName + (s1.FatherName != null ? (", " + s1.FatherName) : "") + (sClass.ClassName != null ? (", " + sClass.ClassName) : "") + ")",
                                s1.FullName,
                                a1.AdmissionId,
                                sSection.SectionName,
                                a1.RollNo,
                                s1.MobileNo,
                                s1.House.HouseName,
                                sClass.ClassName,
                                sClass.ClassId,
                                a1.AcademicSessionId,
                                a1.AcademicSession.SessionName
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

        [HttpGet]
        [Route("AdmissionDetail")]
        public ExpandoObject AdmissionDetail(int AdmissionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                response.AdmissionDetail = LoadData.GetAdmission(AdmissionId, dataContext);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("studentTypeList")]
        public ExpandoObject StudentTypeList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int SchoolId = dataContext.SystemLogins.Where(x => x.LoginId == SessionUtilies.GetLoginId()).First().SchoolId;
                var list = (from st1 in dataContext.StudentTypes
                            where st1.StudentTypeStatus == (byte)StudentTypeStatus.Active
                            && st1.SchoolId == SchoolId
                            select new
                            {
                                st1.TypeName,
                                st1.StudentTypeId
                            }).ToList();
                response.StudentTypeList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("StudentAdmissionList")]
        public ExpandoObject StudentAdmissionList(int StudentId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Admissions
                            join sc1 in dataContext.Sections
                            on a1.SectionId equals sc1.SectionId
                            join c1 in dataContext.Classes
                            on sc1.ClassId equals c1.ClassId
                            where a1.StudentId == StudentId
                            select new AdmissionModel
                            {
                                StudentId = a1.StudentId,
                                AcademicSessionId = a1.AcademicSessionId,
                                SessionName = a1.AcademicSession.SessionName,
                                SectionId = a1.SectionId,
                                SectionName = sc1.SectionName,
                                ClassId = sc1.ClassId,
                                ClassName = c1.ClassName,
                                RollNo = a1.RollNo,
                                AdmissionId = a1.AdmissionId,
                                AdmissionDate = a1.AdmissionDate,
                                AdmissionStatus = a1.AdmissionStatus,
                                AdmissionType = a1.AdmissionType,
                                LeaveDate = a1.LeaveDate,
                            });
                response.StudentAdmissionList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("updateAdmission")]
        public ExpandoObject UpdateAdmission(AdmissionModel admissionModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var admission = dataContext.Admissions.Where(x => x.AdmissionId == admissionModel.AdmissionId).First();
                admission.RollNo = admissionModel.RollNo;
                admission.SectionId = admissionModel.SectionId;
                admission.AdmissionDate = admissionModel.AdmissionDate;
                admission.AdmissionType = admissionModel.AdmissionType;
                dataContext.SubmitChanges();
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
