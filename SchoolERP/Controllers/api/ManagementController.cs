using School;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [RoutePrefix("api/Management")]
    public class ManagementController : ApiController
    {
        [HttpGet]
        [Route("DashboardDetail")]
        public ExpandoObject DashboardDetail(string AppKey)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Guid key = new Guid(AppKey);
                var app = dataContext.Applications.Where(x => x.AppKey == key);
                if (app.First().AppKey == key)
                {
                    ManagementDashboardModel model = new ManagementDashboardModel
                    {
                        TotalOnlineRegistration = dataContext.RegistrationOnlines.Where(x => x.RegistrationOnlineStatus != 1).Count(),
                        TotalIssuedBook = dataContext.BookAccessions.Where(x => x.BookAccessionStatus == (byte)BookAccessionStatus.Issued).Count(),
                        TotalAvailableBook = dataContext.BookAccessions.Where(x => x.BookAccessionStatus == (byte)BookAccessionStatus.Available).Count(),
                        TotalStaff = dataContext.Staffs.Where(x => x.StaffStatus == (byte)StaffStatus.Active).Count(),
                        TotalIssuedStoreItem = dataContext.StoreItemIssues.Any() ? dataContext.StoreItemIssues.Sum(x => x.Quantity) : 0,
                        TotalAvailableStoreItem = dataContext.StoreItems.Any() ? dataContext.StoreItems.Sum(x => x.Quantity) : 0,
                        TotalVehicles = dataContext.Bus.Count(),
                        NewAdmissions = dataContext.Admissions.Where(x => x.AdmissionStatus == (byte)AdmissionStatus.Active && x.AdmissionType == (byte)AdmissionType.NewAdmission).Count(),
                        ExistingStudents = dataContext.Admissions.Where(x => x.AdmissionStatus == (byte)AdmissionStatus.Active && x.AdmissionType != (byte)AdmissionType.NewAdmission).Count(),
                        LeftStudents = dataContext.Students.Where(x => x.StudentStatus == (byte)StudentStatus.Left).Count(),
                        PassOutStudents = (from a1 in dataContext.Admissions
                                           join s2 in dataContext.Students on a1.StudentId equals s2.StudentId
                                           join s1 in dataContext.Sections on a1.SectionId equals s1.SectionId
                                           join c1 in dataContext.Classes on s1.ClassId equals c1.ClassId
                                           where (a1.AdmissionStatus != (byte)AdmissionStatus.Active || s2.StudentStatus != (byte)StudentStatus.Left)
                                           && (c1.ClassNo == 10 || c1.ClassNo == 12)
                                           select a1).Count(),
                        TodayFeeCollection = dataContext.FeeCollections.Where(x => x.PaymentDate.Date == DateTime.Now.Date).Any() ? dataContext.FeeCollections.Where(x => x.PaymentDate.Date == DateTime.Now.Date).Sum(x => x.PaidAmount) : 0,
                        CurrentMonthFeeCollection = dataContext.FeeCollections.Where(x => x.PaymentDate.Month == DateTime.Now.Month && x.PaymentDate.Year == DateTime.Now.Year).Any() ? dataContext.FeeCollections.Where(x => x.PaymentDate.Month == DateTime.Now.Month && x.PaymentDate.Year == DateTime.Now.Year).Sum(x => x.PaidAmount) : 0,
                        CurrentYeatFeeCollection = dataContext.FeeCollections.Where(x => x.PaymentDate.Year == DateTime.Now.Year).Any() ? dataContext.FeeCollections.Where(x => x.PaymentDate.Year == DateTime.Now.Year).Sum(x => x.PaidAmount) : 0,
                    };
                    //Teacher Performance
                    var teacherPerformances = (from p1 in dataContext.Performances
                                               join t1 in dataContext.TeacherAttitudes
                                               on p1.PerformanceId equals t1.PerformanceId
                                               join d1 in dataContext.RatingDetails
                                               on t1.RatingDetailId equals d1.RatingDetailId
                                               join d2 in dataContext.Ratings
                                               on d1.RatingId equals d2.RatingId
                                               select new { Value = d1.RatingValue / d2.MaxValue });
                    var subjectPerformances = (from p1 in dataContext.Performances
                                               join t1 in dataContext.PerformanceParameters
                                               on p1.PerformanceId equals t1.PerformanceId
                                               join d1 in dataContext.RatingDetails
                                               on t1.RatingDetailId equals d1.RatingDetailId
                                               join d2 in dataContext.Ratings
                                               on d1.RatingId equals d2.RatingId
                                               select new { Value = d1.RatingValue / d2.MaxValue });
                    model.TeacherPerformance = (teacherPerformances.Any() ? teacherPerformances.Sum(x => x.Value) / teacherPerformances.Count() : 0) + (subjectPerformances.Any() ? subjectPerformances.Sum(x => x.Value) / subjectPerformances.Count() : 0) / (teacherPerformances.Any() && subjectPerformances.Any() ? 2 : 1);

                    //Staff Attendence

                    ///Due Calculation
                    ///Class Fee
                    dataContext.AcademicSessions.OrderBy(x => x.SessionStartDate).ToList().ForEach(session =>
                    {
                        dataContext.Classes.ToList().ForEach(c1 =>
                        {
                            var classFees = from a1 in dataContext.ClassFeeHeads
                                            join a2 in dataContext.ClassFees on a1.ClassFeeHeadId equals a2.ClassFeeHeadId
                                            where a1.ClassId == c1.ClassId
                                            && a1.AcademicSessionId == session.AcademicSessionId
                                            select a2;
                            if (classFees.Any())
                            {
                                int NoOfStudents = (from a1 in dataContext.Admissions
                                                    join s1 in dataContext.Sections on a1.SectionId equals s1.SectionId
                                                    where s1.ClassId == c1.ClassId
                                                    && a1.AcademicSessionId == session.AcademicSessionId
                                                    select a1).Count();
                                model.TotalFee += classFees.Sum(x => x.Amount) * NoOfStudents;
                            }

                        });
                    });
                    //Student Subject Combination
                    var studentSubjectCombinations = from s1 in dataContext.StudentSubjectCombinations
                                                     join s2 in dataContext.SubjectCombinationFeeHeads on s1.SubjectCombinationId equals s2.SubjectCombinationId
                                                     join s3 in dataContext.SubjectCombinationFees on s2.SubjectCombinationFeeHeadId equals s3.SubjectCombinationFeeHeadId
                                                     select s3.Amount;
                    //select (s3.Amount * (s1.EndDate.HasValue ? ((s1.EndDate.Value.Year - s1.StartDate.Year) * 12 + s1.EndDate.Value.Month - s1.StartDate.Month) : 12));
                    if (studentSubjectCombinations.Any())
                        model.TotalFee += studentSubjectCombinations.Sum();

                    ///Charges
                    if (dataContext.StudentCharges.Any())
                        model.TotalFee += dataContext.StudentCharges.Sum(x => x.FineAmount);

                    ///Transport
                    double totalMonths = 365.25 / 12;
                    dataContext.TransportFeePeriods.ToList().ForEach(period =>
                    {
                        if (period.ValidTo.HasValue)
                            period.ValidTo = period.ValidTo.Value.Date;
                        var Transports = (from s1 in dataContext.StudentTransports
                                          join s2 in dataContext.TransportGroups on s1.TransportGroupId equals s2.TransportGroupId
                                          join s3 in dataContext.TransportFees on s2.TransportGroupId equals s3.TransportGroupId
                                          where period.ValidFrom.Date <= s1.StartDate.Date
                                          && (period.ValidTo.HasValue ? period.ValidTo >= s1.StartDate.Date : true)
                                          //&& (period.ValidTo.HasValue ? (s1.EndDate.HasValue ? period.ValidTo.Value.Date >= s1.EndDate.Value.Date : true) : true)
                                          select new
                                          {
                                              s3.Amount,
                                              s1.EndDate,
                                              s1.StartDate,
                                              //   NoOfTimes = (period.ValidTo.HasValue ?
                                              //     (s1.EndDate.HasValue ? (s1.EndDate.Value.Date > period.ValidTo.Value.Date ?
                                              //(period.ValidTo.Value.Date - s1.StartDate.Date).Days / totalMonths :
                                              //(s1.EndDate.Value.Date - s1.StartDate.Date).Days / totalMonths) :
                                              // (period.ValidTo.Value.Date - s1.StartDate.Date).Days / totalMonths) :
                                              //(DateTime.Now.Date - s1.StartDate.Date).Days / totalMonths)
                                          }).ToList();
                        var studentTransports = Transports.Select(s1 => new
                        {
                            s1.Amount,
                            NoOfTimes = (period.ValidTo.HasValue ?
                                        (s1.EndDate.HasValue ? (s1.EndDate.Value.Date > period.ValidTo.Value.Date ?
                                        (period.ValidTo.Value.Date - s1.StartDate.Date).Days / totalMonths :
                                        (s1.EndDate.Value.Date - s1.StartDate.Date).Days / totalMonths) :
                                        (period.ValidTo.Value.Date - s1.StartDate.Date).Days / totalMonths) :
                                        (DateTime.Now.Date - s1.StartDate.Date).Days / totalMonths)
                        });
                        if (studentTransports.Any())
                            model.TotalFee += studentTransports.Sum(x => (x.Amount ?? 0) * Convert.ToInt32(x.NoOfTimes));
                    });
                    ///Student Concession
                    if (dataContext.StudentConcessions.Any())
                        model.TotalConcession = dataContext.StudentConcessions.Sum(x => x.ConcessionAmount);

                    //Paid Amount
                    if (dataContext.FeeCollections.Where(x => x.AdmissionId.HasValue && x.FeeCollectionStatus == (byte)FeeCollectionStatus.Paid).Any())
                        model.TotalPaidFee = dataContext.FeeCollections.Where(x => x.AdmissionId.HasValue && x.FeeCollectionStatus == (byte)FeeCollectionStatus.Paid).Sum(x => x.PaidAmount);
                    model.TotalDues = model.TotalFee - model.TotalConcession - model.TotalPaidFee;

                    List<NameValueModel> nameValueModels = new List<NameValueModel>
                    {
                        new NameValueModel{Key = 1, Name="TotalOnlineRegistration",Value=model.TotalOnlineRegistration},
                        new NameValueModel{Key = 2,Name="TotalIssuedBook",Value=model.TotalIssuedBook},
                        new NameValueModel{Key = 3,Name="TotalAvailableBook",Value=model.TotalAvailableBook},
                        new NameValueModel{Key = 4,Name="TotalBooks",Value=model.TotalBooks},
                        new NameValueModel{Key = 5,Name="TotalStaff",Value=model.TotalStaff},
                        new NameValueModel{Key =6,Name="TotalIssuedStoreItem",Value=model.TotalIssuedStoreItem},
                        new NameValueModel{Key = 7,Name="TotalAvailableStoreItem",Value=model.TotalAvailableStoreItem},
                        new NameValueModel{Key =8 ,Name="TotalStoreItem",Value=model.TotalStoreItem},
                        new NameValueModel{Key = 9,Name="TotalVehicles",Value=model.TotalVehicles},
                        new NameValueModel{Key = 10,Name="TeacherPerformance",Value=model.TeacherPerformance},
                        new NameValueModel{Key = 11,Name="StaffAttendence",Value=model.StaffAttendence},
                        new NameValueModel{Key = 12,Name="NewAdmissions",Value=model.NewAdmissions},
                        new NameValueModel{Key = 13,Name="ExistingStudents",Value=model.ExistingStudents},
                        new NameValueModel{Key =14 ,Name="LeftStudents",Value=model.LeftStudents},
                        new NameValueModel{Key = 15,Name="PassOutStudents",Value=model.PassOutStudents},
                        new NameValueModel{Key = 16,Name="TodayFeeCollection",Value=model.TodayFeeCollection},
                        new NameValueModel{Key = 17,Name="CurrentMonthFeeCollection",Value=model.CurrentMonthFeeCollection},
                        new NameValueModel{Key = 18,Name="CurrentYeatFeeCollection",Value=model.CurrentYeatFeeCollection},
                        new NameValueModel{Key = 19,Name="TotalFee",Value=model.TotalFee},
                        new NameValueModel{Key = 20,Name="TotalConcession",Value=model.TotalConcession},
                        new NameValueModel{Key = 21,Name="TotalPaidFee",Value=model.TotalPaidFee},
                        new NameValueModel{Key = 22,Name="TotalDues",Value=model.TotalDues},
                    };
                    model.DashboardList = nameValueModels;
                    response.DashboardData = model;
                    response.Message = ConstantData.SuccessMessage;
                }
                else
                {
                    response.Message = "Invailid App Key.";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
