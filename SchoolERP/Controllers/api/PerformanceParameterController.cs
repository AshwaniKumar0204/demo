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
    [RoutePrefix("api/PerformanceParameter")]
    public class PerformanceParameterController : ApiController
    {
        [HttpGet]
        [Route("ParentPerformanceParameterList")]
        public ExpandoObject ParentPerformanceParameterList(int PerformanceId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.PerformanceParameters
                           join p2 in dataContext.Parameters
                           on p1.ParameterId equals p2.ParameterId
                           join r1 in dataContext.RatingDetails
                           on p1.RatingDetailId equals r1.RatingDetailId
                           join p3 in dataContext.Parameters
                           on p2.ParentParameterId equals p3.ParameterId into subPPrams
                           where PerformanceId == p1.PerformanceId
                           select new
                           {
                               p1.RatingDetailId,
                               p1.PerformanceParameterId,
                               p1.PerformanceId,
                               p2.ParameterName,
                               p1.ParameterId,
                               r1.Title,
                               r1.RatingValue,
                               p2.ParentParameterId,
                               ParentParameterName = subPPrams.Any() ? subPPrams.First().ParameterName : null,
                           };


                var teacherAttitudeList = from t1 in dataContext.TeacherAttitudes
                                          join a1 in dataContext.Admissions
                                          on t1.AdmissionId equals a1.AdmissionId
                                          join ss1 in dataContext.Sections
                                          on a1.SectionId equals ss1.SectionId
                                          join cl in dataContext.Classes
                                          on ss1.ClassId equals cl.ClassId
                                          join s1 in dataContext.Students
                                          on a1.StudentId equals s1.StudentId
                                          join t2 in dataContext.Teachers
                                          on t1.TeacherId equals t2.TeacherId
                                          join r1 in dataContext.RatingDetails
                                          on t1.RatingDetailId equals r1.RatingDetailId
                                          where t1.PerformanceId == PerformanceId
                                          select new
                                          {
                                              t1.AdmissionId,
                                              t1.TeacherAttitudeId,
                                              s1.AdmissionNo,
                                              s1.FullName,
                                              t2.TeacherName,
                                              t2.TeacherCode,
                                              r1.RatingValue,
                                              r1.Title
                                          };
                ResponseMessage.PerformanceParameterList = list.Distinct().GroupBy(x => new { x.ParentParameterId, x.ParentParameterName }).Select(x => new { x.Key.ParentParameterId, x.Key.ParentParameterName, ParameterList = x.ToList() }).ToList();
                ResponseMessage.TeacherAttitudeList = teacherAttitudeList.Distinct().GroupBy(x => new { x.AdmissionId, x.AdmissionNo, x.FullName }).Select(x => new { x.Key.AdmissionNo, x.Key.AdmissionId, x.Key.FullName, TeacherList = x.ToList() }).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("StudentPerformanceParameterList")]
        public ExpandoObject StudentPerformanceParameterList(int PerformanceId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //Non-Subject Parametes
                var list = from p1 in dataContext.PerformanceParameters
                           join p2 in dataContext.Parameters
                           on p1.ParameterId equals p2.ParameterId
                           join r1 in dataContext.RatingDetails
                           on p1.RatingDetailId equals r1.RatingDetailId
                           join p3 in dataContext.Parameters
                           on p2.ParentParameterId equals p3.ParameterId into subPPrams
                           where PerformanceId == p1.PerformanceId
                           && p1.SubjectId == null
                           select new
                           {
                               p1.RatingDetailId,
                               p1.PerformanceParameterId,
                               p1.PerformanceId,
                               p2.ParameterName,
                               p1.ParameterId,
                               r1.Title,
                               r1.RatingValue,
                               p2.ParentParameterId,
                               ParentParameterName = subPPrams.Any() ? subPPrams.First().ParameterName : null,
                           };

                //Subject Parameter
                var subjectParams = from p1 in dataContext.PerformanceParameters
                                    join s1 in dataContext.Subjects
                                    on p1.SubjectId equals s1.SubjectId
                                    join p2 in dataContext.Parameters
                                    on p1.ParameterId equals p2.ParameterId
                                    join r1 in dataContext.RatingDetails
                                    on p1.RatingDetailId equals r1.RatingDetailId
                                    join p3 in dataContext.Parameters
                                    on p2.ParentParameterId equals p3.ParameterId into subPPrams
                                    where PerformanceId == p1.PerformanceId
                                    select new
                                    {
                                        p1.RatingDetailId,
                                        p1.PerformanceParameterId,
                                        p1.PerformanceId,
                                        p2.ParameterName,
                                        p1.ParameterId,
                                        r1.Title,
                                        r1.RatingValue,
                                        p2.ParentParameterId,
                                        ParentParameterName = subPPrams.Any() ? subPPrams.First().ParameterName : null,
                                        s1.SubjectName,
                                        s1.SubjectCode
                                    };

                ResponseMessage.PerformanceParameterList = list.Distinct().GroupBy(x => new { x.ParentParameterId, x.ParentParameterName }).Select(x => new { x.Key.ParentParameterId, x.Key.ParentParameterName, ParameterList = x.ToList() }).ToList();
                ResponseMessage.SubjectPerformanceParameterList = subjectParams.Distinct().GroupBy(x => new { x.SubjectCode, x.SubjectName }).Select(y => new { y.Key.SubjectName, y.Key.SubjectCode, PerformanceParameterList = y.GroupBy(x => new { x.ParentParameterId, x.ParentParameterName }).Select(x => new { x.Key.ParentParameterId, ParameterName = x.Key.ParentParameterName, ParameterList = x.ToList() }).ToList() }).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("TeacherPerformanceParameterList")]
        public ExpandoObject TeacherPerformanceParameterList(int PerformanceId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.PerformanceParameters
                           join p2 in dataContext.Parameters
                           on p1.ParameterId equals p2.ParameterId
                           join p3 in dataContext.Parameters
                           on p2.ParentParameterId equals p3.ParameterId into subPPrams
                           where PerformanceId == p1.PerformanceId
                           && p2.ParameterFor == (byte)ParameterFor.Teacher
                           select new
                           {
                               p1.RatingDetailId,
                               p1.PerformanceParameterId,
                               p1.PerformanceId,
                               p2.ParameterName,
                               p1.ParameterId,
                               p2.ParentParameterId,
                               p1.Remarks,
                               p2.FieldType,
                               ParentParameterName = subPPrams.Any() ? subPPrams.First().ParameterName : null,
                           };


                ResponseMessage.AcademicSession = dataContext.Performances.Where(x => x.PerformanceId == PerformanceId).Select(x => new { x.AcademicSessionId, x.AcademicSession.SessionName, x.TeacherId }).First();
                ResponseMessage.TeacherExperienceList = dataContext.TeacherExperiences.Where(x => x.PerformanceParameterId == list.First(y => y.ParameterId == ParameterConstant.TeachingExprienceId).PerformanceParameterId).Select(x => new { x.Year, x.SchoolName }).ToList();
                ResponseMessage.TeacherCoScholasticList = dataContext.TeacherCoScholastics.Where(x => x.PerformanceParameterId == list.First(y => y.ParameterId == ParameterConstant.CoScholastic).PerformanceParameterId).Select(x => new { x.Events, x.EventDate, x.TeacherRole }).ToList();
                ResponseMessage.TrainingProgrammeList = dataContext.TrainingProgrammes.Where(x => x.PerformanceParameterId == list.First(y => y.ParameterId == ParameterConstant.TrainingProgrammes).PerformanceParameterId).Select(x => new { x.TrainingDate, x.Subject, x.Place }).ToList();
                ResponseMessage.TeachingLearningAidList = (from d1 in dataContext.TeachingLearningAids.Where(x => x.PerformanceParameterId == list.First(y => y.ParameterId == ParameterConstant.TeacherLearningAids).PerformanceParameterId)
                                                          join c1 in dataContext.Classes on d1.ClassNo equals c1.ClassNo
                                                          join s1 in dataContext.Subjects on d1.SubjectId equals s1.SubjectId
                                                          orderby d1.ClassNo
                                                          select new { d1.ClassNo, c1.ClassName2, s1.SubjectName, d1.Topics, d1.Aids, d1.ReferanceBooks }).Distinct().ToList();
                ResponseMessage.TeacherBookReadList = dataContext.TeacherBookReads.Where(x => x.PerformanceParameterId == list.First(y => y.ParameterId == ParameterConstant.BookReading).PerformanceParameterId).Select(x => new { x.BookName, x.Author, x.BookReadType, BookReadTypeName = Enum.GetName(typeof(BookReadType), x.BookReadType) }).ToList();

                ResponseMessage.PerformanceParameterList = list.Distinct().GroupBy(x => new { x.ParentParameterId, x.ParentParameterName }).Select(x => new { x.Key.ParentParameterId, x.Key.ParentParameterName, ParameterList = x.ToList() }).ToList();
                ResponseMessage.BookReading = ParameterConstant.BookReading;
                ResponseMessage.ClassTaught = ParameterConstant.ClassTaught;
                ResponseMessage.CoScholastic = ParameterConstant.CoScholastic;
                ResponseMessage.TeacherLearningAids = ParameterConstant.TeacherLearningAids;
                ResponseMessage.TeachingExprienceId = ParameterConstant.TeachingExprienceId;
                ResponseMessage.TrainingProgrammePrameterId = ParameterConstant.TrainingProgrammes;
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("TeacherFeedbackListFromPricipal")]
        public ExpandoObject TeacherFeedbackListFromPricipal(int PerformanceId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //Non-Subject Parametes
                var list = from p2 in dataContext.Parameters
                           join p3 in dataContext.Parameters on p2.ParentParameterId equals p3.ParameterId into subPPrams
                           join p1 in dataContext.PerformanceParameters.Where(x => x.PerformanceId == PerformanceId) on p2.ParameterId equals p1.ParameterId into subPerformanceParam
                           where p2.ParameterFor == (byte)ParameterFor.Principal
                           && p2.ParentParameterId.HasValue
                           select new
                           {
                               RatingDetailId = subPerformanceParam.Any() ? subPerformanceParam.First().RatingDetailId : null,
                               Remarks = subPerformanceParam.Any() ? subPerformanceParam.First().Remarks : "",
                               PerformanceParameterId = subPerformanceParam.Any() ? subPerformanceParam.First().PerformanceParameterId : 0,
                               PerformanceId,
                               p2.ParameterName,
                               p2.ParameterId,
                               p2.ParentParameterId,
                               ParentParameterName = subPPrams.Any() ? subPPrams.First().ParameterName : null,
                               RatingDetailList = (from rr1 in dataContext.RatingDetails
                                                   where (rr1.RatingDetailStatus == rr1.RatingDetailStatus)
                                                   && rr1.RatingId == p2.RatingId
                                                   orderby rr1.RatingNo
                                                   select new RatingDetailModel
                                                   {
                                                       RatingDetailId = rr1.RatingDetailId,
                                                       Title = rr1.Title,
                                                       RatingValue = rr1.RatingValue,
                                                       //UniqueId = rr1.RatingDetailId + "_" + ss2.ParameterId
                                                   }).ToList()
                           };
                ResponseMessage.PerformanceParameterList = list.Distinct().GroupBy(x => new { x.ParentParameterId, x.ParentParameterName }).Select(x => new { x.Key.ParentParameterId, x.Key.ParentParameterName, ParameterList = x.ToList() }).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

    }
}
