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
    [RoutePrefix("api/Performance")]
    public class PerformanceController : ApiController
    {
        [HttpPost]
        [Route("ParentPerformanceList")]
        public ExpandoObject ParentPerformanceList(DateFilterModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                //if (model.FromDate.HasValue)
                //    model.FromDate = model.FromDate.Value.Date;
                //if (model.ToDate.HasValue)
                //    model.ToDate = model.ToDate.Value.Date;
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.Performances
                           join p2 in dataContext.Parents
                           on p1.ParentId equals p2.ParentId
                           join a1 in dataContext.AcademicSessions
                           on p1.AcademicSessionId equals a1.AcademicSessionId
                           where (model.AcademicSessionId == 0 ? true : p1.AcademicSessionId == model.AcademicSessionId)
                           //&& (model.FromDate.HasValue ? model.FromDate <= p1.EntryDate.Date : true)
                           //&& (model.ToDate.HasValue ? model.ToDate >= p1.EntryDate.Date : true)
                           && (model.MonthNo > 0 ? model.MonthNo == p1.EntryDate.Month : true)
                           && (model.Year > 0 ? model.Year >= p1.EntryDate.Year : true)
                           orderby p1.EntryDate descending
                           select new
                           {
                               p1.PerformanceId,
                               p2.ParentCode,
                               p1.EntryDate,
                               p1.AcademicSessionId,
                               a1.SessionName,
                               p1.ParentId,
                               StudentList = (from s1 in dataContext.Students where s1.ParentId == p1.ParentId select new { s1.FullName, s1.AdmissionNo })
                           };

                ResponseMessage.PerformanceList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("StudentPerformanceList")]
        public ExpandoObject StudentPerformanceList(DateFilterModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.Performances
                           join a2 in dataContext.Admissions
                           on p1.AdmissionId equals a2.AdmissionId
                           join s2 in dataContext.Sections
                           on a2.SectionId equals s2.SectionId
                           join c1 in dataContext.Classes
                           on s2.ClassId equals c1.ClassId
                           join s1 in dataContext.Students
                           on a2.StudentId equals s1.StudentId
                           join a1 in dataContext.AcademicSessions
                           on p1.AcademicSessionId equals a1.AcademicSessionId
                           where (model.AcademicSessionId == 0 ? true : p1.AcademicSessionId == model.AcademicSessionId)
                           && (model.MonthNo > 0 ? model.MonthNo == p1.EntryDate.Month : true)
                           && (model.Year > 0 ? model.Year >= p1.EntryDate.Year : true)
                           orderby p1.EntryDate descending
                           select new
                           {
                               p1.PerformanceId,
                               s1.FullName,
                               s1.AdmissionNo,
                               a2.RollNo,
                               c1.ClassName,
                               c1.ClassName2,
                               s2.SectionName,
                               p1.EntryDate,
                               p1.AcademicSessionId,
                               a1.SessionName,
                               p1.ParentId,
                           };

                ResponseMessage.PerformanceList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("TeacherPerformanceList")]
        public ExpandoObject TeacherPerformanceList(DateFilterModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var teacherList = (from t1 in dataContext.Teachers
                                   //join t2 in dataContext.TeacherSessions
                                   //on t1.TeacherId equals t2.TeacherId
                                   //where t2.AcademicSessionId == model.AcademicSessionId
                                   select new TeacherPerformanceModel { TeacherCode = t1.TeacherCode, TeacherId = t1.TeacherId, TeacherName = t1.TeacherName }).ToList();
                teacherList.ForEach(teacher =>
                {

                    var performances = (from p1 in dataContext.Performances
                                        join t1 in dataContext.TeacherAttitudes
                                        on p1.PerformanceId equals t1.PerformanceId
                                        join d1 in dataContext.RatingDetails
                                        on t1.RatingDetailId equals d1.RatingDetailId
                                        join d2 in dataContext.Ratings
                                        on d1.RatingId equals d2.RatingId
                                        where p1.AcademicSessionId == model.AcademicSessionId
                                        && t1.TeacherId == teacher.TeacherId
                                        && (model.MonthNo > 0 ? model.MonthNo == p1.EntryDate.Month : true)
                                        select new { d1.RatingValue, d2.MaxValue });
                    if (performances.Any())
                    {
                        teacher.MaxValue = performances.Sum(x => x.MaxValue);
                        teacher.RatingValue = performances.Sum(x => x.RatingValue);
                    }
                });


                decimal RatingPercentages = 0;
                if (teacherList.Any())
                    RatingPercentages = teacherList.OrderByDescending(x => x.MaxValue).Select(x => x.MaxValue).First();
                if (RatingPercentages > 0)
                    teacherList.ForEach(x => x.RatingPercentages = x.RatingValue / RatingPercentages * 100);
                ResponseMessage.TeacherPerformanceList = teacherList.OrderByDescending(x => x.RatingValue).ToList();
                ResponseMessage.RatingPercentages = RatingPercentages;
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("TeacherAttitudeList")]
        public ExpandoObject TeacherAttitudeList(DateFilterModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var TeacherAttitudeList = (from p1 in dataContext.Performances
                                           join t1 in dataContext.TeacherAttitudes
                                           on p1.PerformanceId equals t1.PerformanceId
                                           join d1 in dataContext.RatingDetails
                                           on t1.RatingDetailId equals d1.RatingDetailId
                                           join d2 in dataContext.Ratings
                                           on d1.RatingId equals d2.RatingId
                                           join a1 in dataContext.Admissions
                                           on t1.AdmissionId equals a1.AdmissionId
                                           join s1 in dataContext.Students
                                           on a1.StudentId equals s1.StudentId
                                           join s2 in dataContext.Sections
                                           on a1.SectionId equals s2.SectionId
                                           join c1 in dataContext.Classes
                                           on s2.ClassId equals c1.ClassId
                                           join parent in dataContext.Parents
                                           on s1.ParentId equals parent.ParentId into subParent
                                           where p1.AcademicSessionId == model.AcademicSessionId
                                           && t1.TeacherId == model.TeacherId
                                           && (model.MonthNo > 0 ? model.MonthNo == p1.EntryDate.Month : true)
                                           select new
                                           {
                                               d1.RatingValue,
                                               d2.MaxValue,
                                               d1.Title,
                                               a1.RollNo,
                                               s1.FullName,
                                               s1.AdmissionNo,
                                               s2.SectionName,
                                               c1.ClassName,
                                               Parent = subParent.Select(x => new { x.ParentCode }).FirstOrDefault()
                                           });
                ResponseMessage.TeacherAttitudeList = TeacherAttitudeList.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("PerformanceYearList")]
        public ExpandoObject PerformanceYearList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = dataContext.Performances.Select(x => new { x.EntryDate.Year }).Distinct().OrderByDescending(x => x.Year);
                ResponseMessage.YearList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        //Teacher Fee Back
        [HttpGet]
        [Route("TeacherFeedBackFromPrincipal")]
        public ExpandoObject TeacherFeedBackFromPrincipal()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var ParameterList = (from s1 in dataContext.Parameters
                                     where s1.ParentParameterId == null
                                     && (s1.ParameterFor == (byte)ParameterFor.Principal)
                                     orderby s1.ParameterNo
                                     select new ParameterModel
                                     {
                                         ParameterNo = s1.ParameterNo,
                                         ParameterId = s1.ParameterId,
                                         ParameterName = s1.ParameterName,
                                         FieldType = s1.FieldType,
                                         ChildPrameterList = (from s2 in dataContext.Parameters
                                                              where s2.ParentParameterId == s1.ParameterId
                                                              && (s2.ParameterFor == (byte)ParameterFor.Teacher || s2.ParameterFor == (byte)ParameterFor.All)
                                                              orderby s2.ParameterNo
                                                              select new ParameterModel
                                                              {
                                                                  ParameterNo = s2.ParameterNo,
                                                                  ParameterId = s2.ParameterId,
                                                                  ParentParameterId = s2.ParentParameterId,
                                                                  ParameterName = s2.ParameterName,
                                                                  FieldType = s2.FieldType,
                                                              }).ToList()
                                     });

                response.ParameterList = ParameterList.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("TeacherFeedbackList")]
        public ExpandoObject TeacherFeedbackList(DateFilterModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.Performances
                           join t1 in dataContext.Teachers
                           on p1.TeacherId equals t1.TeacherId
                           join a1 in dataContext.AcademicSessions
                           on p1.AcademicSessionId equals a1.AcademicSessionId
                           where (model.AcademicSessionId == 0 ? true : p1.AcademicSessionId == model.AcademicSessionId)
                           && (model.TeacherId == 0 || model.TeacherId == t1.TeacherId)
                           orderby p1.EntryDate descending
                           select new
                           {
                               p1.PerformanceId,
                               p1.EntryDate,
                               p1.AcademicSessionId,
                               a1.SessionName,
                               t1.TeacherCode,
                               t1.TeacherName,
                           };

                ResponseMessage.PerformanceList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("TeacherParameterList")]
        public ExpandoObject TeacherParameterList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //Check Submition
                int TeacherId = SessionUtilies.GetTeacherId(dataContext);
                var performances = from p1 in dataContext.Performances
                                   where p1.TeacherId == TeacherId
                                   && p1.EntryDate.Year == DateTime.Now.Year
                                   select p1;
                if (performances.Any())
                {
                    //throw new Exception("Performance is already submitted");
                    response.Message = "submitted";
                    return response;
                }

                var ParameterList = (from s1 in dataContext.Parameters
                                     where s1.ParentParameterId == null
                                     && (s1.ParameterFor == (byte)ParameterFor.Teacher || s1.ParameterFor == (byte)ParameterFor.All)
                                     orderby s1.ParameterNo
                                     select new ParameterModel
                                     {
                                         ParameterNo = s1.ParameterNo,
                                         ParameterId = s1.ParameterId,
                                         ParameterName = s1.ParameterName,
                                         FieldType = s1.FieldType,
                                         ChildPrameterList = (from s2 in dataContext.Parameters
                                                              where s2.ParentParameterId == s1.ParameterId
                                                              && (s2.ParameterFor == (byte)ParameterFor.Teacher || s2.ParameterFor == (byte)ParameterFor.All)
                                                              orderby s2.ParameterNo
                                                              select new ParameterModel
                                                              {
                                                                  ParameterNo = s2.ParameterNo,
                                                                  ParameterId = s2.ParameterId,
                                                                  ParentParameterId = s2.ParentParameterId,
                                                                  ParameterName = s2.ParameterName,
                                                                  FieldType = s2.FieldType,
                                                                  //ChildPrameterList = (from s3 in dataContext.Parameters
                                                                  //                     where s3.ParentParameterId == s2.ParameterId
                                                                  //                     && (s3.ParameterFor == (byte)ParameterFor.Teacher || s3.ParameterFor == (byte)ParameterFor.All)
                                                                  //                     orderby s3.ParameterNo
                                                                  //                     select new ParameterModel
                                                                  //                     {
                                                                  //                         ParameterNo = s3.ParameterNo,
                                                                  //                         ParameterId = s3.ParameterId,
                                                                  //                         ParentParameterId = s3.ParentParameterId,
                                                                  //                         ParameterName = s3.ParameterName,
                                                                  //                         FieldType = s3.FieldType,
                                                                  //                     }).ToList()
                                                              }).ToList()
                                     });
                var sessions = from a1 in dataContext.AcademicSessions
                               where a1.SessionStartDate.Date <= DateTime.Now.Date
                               orderby a1.SessionStartDate descending
                               select a1;
                if (sessions.Any())
                    response.AcademicSession = sessions.Select(x => new { x.AcademicSessionId, x.SessionName }).First();
                else
                    response.AcademicSession = dataContext.AcademicSessions.OrderByDescending(x => x.SessionStartDate).Select(x => new { x.AcademicSessionId, x.SessionName }).First();

                response.ParameterList = ParameterList.ToList();
                response.BookReading = ParameterConstant.BookReading;
                response.ClassTaught = ParameterConstant.ClassTaught;
                response.CoScholastic = ParameterConstant.CoScholastic;
                response.TeacherLearningAids = ParameterConstant.TeacherLearningAids;
                response.TeachingExprienceId = ParameterConstant.TeachingExprienceId;
                response.TrainingProgrammes = ParameterConstant.TrainingProgrammes;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("TeacherSubjectListForFeedback")]
        public ExpandoObject TeacherSubjectListForFeedback(int AcademicSessionId, int TeacherId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (TeacherId == 0)
                    TeacherId = SessionUtilies.GetTeacherId(dataContext);
                //var TeacherSubjectList = (from t1 in dataContext.ClassTimeTables
                //                          join t2 in dataContext.ClassTimeTablePeriods
                //                          on t1.ClassTimeTableId equals t2.ClassTimeTableId
                //                          join t3 in dataContext.ClassTimeTableDetails
                //                          on t2.ClassTimeTablePeriodId equals t3.ClassTimeTablePeriodId
                //                          join s1 in dataContext.Sections
                //                          on t1.SectionId equals s1.SectionId
                //                          join s3 in dataContext.Subjects
                //                          on t3.SubjectId equals s3.SubjectId
                //                          where t3.TeacherId == TeacherId
                //                          && t1.AcademicSessionId == AcademicSessionId
                //                          select new
                //                          {
                //                              t3.TeacherId,
                //                              t3.SubjectId,
                //                              s1.Class.ClassName2,
                //                              s1.Class.ClassNo,
                //                              s1.SectionName,
                //                              s1.SectionId,
                //                              s3.SubjectName,
                //                              s3.SubjectCode
                //                          }).GroupBy(x => new { x.SectionId, x.SectionName, x.ClassName2, x.SubjectId, x.SubjectName })
                //                   .Select(s2 => new { s2.Key.SubjectName, s2.Key.ClassName2, s2.Key.SectionName, NoOfPeriods = s2.Count() }).ToList();

                //response.TeacherSubjectList = TeacherSubjectList;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("TeachingLearningAidsDetail")]
        public ExpandoObject TeachingLearningAidsDetail(int AcademicSessionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                //SchoolDataContext dataContext = new SchoolDataContext();
                //int TeacherId = SessionUtilies.GetTeacherId(dataContext);
                //var TeacherSubjectList = (from t1 in dataContext.ClassTimeTables
                //                          join t2 in dataContext.ClassTimeTablePeriods
                //                          on t1.ClassTimeTableId equals t2.ClassTimeTableId
                //                          join t3 in dataContext.ClassTimeTableDetails
                //                          on t2.ClassTimeTablePeriodId equals t3.ClassTimeTablePeriodId
                //                          join s1 in dataContext.Sections
                //                          on t1.SectionId equals s1.SectionId
                //                          join s3 in dataContext.Subjects
                //                          on t3.SubjectId equals s3.SubjectId
                //                          where t3.TeacherId == TeacherId
                //                          && t1.AcademicSessionId == AcademicSessionId
                //                          select new
                //                          {
                //                              s1.Class.ClassName2,
                //                              s1.Class.ClassNo,
                //                              t3.SubjectId,
                //                              s3.SubjectName,
                //                          }).Distinct().ToList();

                //response.TeachingLearningAidsList = TeacherSubjectList;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveTeacherPerformance")]
        public ExpandoObject SaveTeacherPerformance(SaveTeacherFeedbackModel model)
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
                //Check Submition
                int TeacherId = SessionUtilies.GetTeacherId(dataContext);
                var performances = from p1 in dataContext.Performances
                                   where p1.TeacherId == TeacherId
                                   && p1.EntryDate.Year == DateTime.Now.Year
                                   select p1;
                if (performances.Any())
                {
                    //throw new Exception("Performance is already submitted");
                    response.Message = "submitted";
                    return response;
                }

                Performance performance = new Performance
                {
                    EntryDate = DateTime.Now,
                    TeacherId = TeacherId,
                    AcademicSessionId = model.AcademicSessionId,
                };
                //var sessions = from a1 in dataContext.AcademicSessions
                //               where a1.SessionStartDate.Date <= DateTime.Now.Date
                //               orderby a1.SessionStartDate descending
                //               select a1;
                //if (sessions.Any())
                //    performance.AcademicSessionId = sessions.First().AcademicSessionId;
                //else
                //    performance.AcademicSessionId = dataContext.AcademicSessions.OrderByDescending(x => x.SessionStartDate).First().AcademicSessionId;
                dataContext.Performances.InsertOnSubmit(performance);
                dataContext.SubmitChanges();

                model.PerformanceParameterList.ForEach(p =>
                {
                    PerformanceParameter performanceParameter = new PerformanceParameter
                    {
                        PerformanceId = performance.PerformanceId,
                        Remarks = p.Remarks,
                        ParameterId = p.ParameterId,
                    };
                    dataContext.PerformanceParameters.InsertOnSubmit(performanceParameter);
                    dataContext.SubmitChanges();
                    if (p.ParameterId == ParameterConstant.TeachingExprienceId)
                    {
                        model.TeacherExperienceList.ForEach(te1 =>
                        {
                            TeacherExperience teacherExperience = new TeacherExperience
                            {
                                SchoolName = te1.SchoolName,
                                Year = te1.Year,
                                PerformanceParameterId = performanceParameter.PerformanceParameterId
                            };
                            dataContext.TeacherExperiences.InsertOnSubmit(teacherExperience);
                            dataContext.SubmitChanges();
                        });
                    }
                    else if (p.ParameterId == ParameterConstant.CoScholastic)
                    {
                        model.TeacherCoScholasticList.ForEach(te1 =>
                        {
                            TeacherCoScholastic TeacherCoScholastic = new TeacherCoScholastic
                            {
                                PerformanceParameterId = performanceParameter.PerformanceParameterId,
                                Events = te1.Events,
                                EventDate = te1.EventDate,
                                TeacherRole = te1.TeacherRole,
                            };
                            dataContext.TeacherCoScholastics.InsertOnSubmit(TeacherCoScholastic);
                            dataContext.SubmitChanges();
                        });
                    }
                    else if (p.ParameterId == ParameterConstant.TrainingProgrammes)
                    {
                        model.TrainingProgrammeList.ForEach(te1 =>
                        {
                            TrainingProgramme TrainingProgramme = new TrainingProgramme
                            {
                                PerformanceParameterId = performanceParameter.PerformanceParameterId,
                                TrainingDate = te1.TrainingDate,
                                Subject = te1.Subject,
                                Place = te1.Place,
                            };
                            dataContext.TrainingProgrammes.InsertOnSubmit(TrainingProgramme);
                            dataContext.SubmitChanges();
                        });
                    }
                    else if (p.ParameterId == ParameterConstant.TeacherLearningAids)
                    {
                        model.TeachingLearningAidList.ForEach(te1 =>
                        {
                            TeachingLearningAid TeachingLearningAid = new TeachingLearningAid
                            {
                                PerformanceParameterId = performanceParameter.PerformanceParameterId,
                                ClassNo = te1.ClassNo,
                                SubjectId = te1.SubjectId,
                                Topics = te1.Topics,
                                Aids = te1.Aids,
                                ReferanceBooks = te1.ReferanceBooks,
                            };
                            dataContext.TeachingLearningAids.InsertOnSubmit(TeachingLearningAid);
                            dataContext.SubmitChanges();
                        });
                    }
                    else if (p.ParameterId == ParameterConstant.BookReading)
                    {
                        model.TeacherBookReadList.ForEach(te1 =>
                        {
                            TeacherBookRead TeacherBookRead = new TeacherBookRead
                            {
                                PerformanceParameterId = performanceParameter.PerformanceParameterId,
                                BookReadType = te1.BookReadType,
                                BookName = te1.BookName,
                                Author = te1.Author,
                            };
                            dataContext.TeacherBookReads.InsertOnSubmit(TeacherBookRead);
                            dataContext.SubmitChanges();
                        });
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

        [HttpPost]
        [Route("SaveTeacherPerformanceByPrincipal")]
        public ExpandoObject SaveTeacherPerformanceByPrincipal(List<PerformanceParameter> PerformanceParameterList)
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
                PerformanceParameterList.ForEach(p =>
                {
                    PerformanceParameter performanceParameter = null;
                    if (p.PerformanceParameterId == 0)
                        performanceParameter = new PerformanceParameter
                        {
                            PerformanceId = p.PerformanceId
                        };
                    else
                        performanceParameter = dataContext.PerformanceParameters.Where(x => x.PerformanceParameterId == p.PerformanceParameterId).First();
                    performanceParameter.Remarks = p.Remarks;
                    performanceParameter.ParameterId = p.ParameterId;
                    performanceParameter.RatingDetailId = p.RatingDetailId;
                    if (performanceParameter.PerformanceParameterId == 0)
                        dataContext.PerformanceParameters.InsertOnSubmit(performanceParameter);
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
