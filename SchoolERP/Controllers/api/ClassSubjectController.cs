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
    [RoutePrefix("api/ClassSubject")]
    public class ClassSubjectController : ApiController
    {
        [HttpGet]
        [Route("ClassSubjectList")]
        public ExpandoObject ClassSubjectList(int SectionId, int Status, int AcademicSessionId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.ClassSubjectList = LoadData.GetClassSubjectList(dataContext, SectionId, Status, AcademicSessionId);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveClassSubject")]
        public ExpandoObject SaveClassSubject(ClassSubjectModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ClassSubject ClassSubject = new ClassSubject();
                if (model.ClassSubjectId > 0)
                    ClassSubject = dataContext.ClassSubjects.Where(x => x.ClassSubjectId == model.ClassSubjectId).First();
                ClassSubject.SectionId = model.SectionId;
                ClassSubject.SubjectId = model.SubjectId;
                ClassSubject.ClassSubjectStatus = model.ClassSubjectStatus;
                ClassSubject.SubjectType = (byte)SubjectType.Compulsory;
                ClassSubject.PracticalPeriods = model.PracticalPeriods;
                ClassSubject.IsIncluded = model.IsIncluded;
                ClassSubject.TheoryPeriods = model.TheoryPeriods;
                ClassSubject.AcademicSessionId = model.AcademicSessionId;
                //ClassSubject.ParentClassSubjectId = model.ParentClassSubjectId;
                if (ClassSubject.ClassSubjectId == 0)
                {
                    int Position = 1;
                    var lastClasses = dataContext.ClassSubjects.Where(x => x.SectionId == model.SectionId);
                    if (lastClasses.Any())
                        Position = lastClasses.OrderByDescending(x => x.Position).First().Position + 1;
                    ClassSubject.Position = Position;
                    dataContext.ClassSubjects.InsertOnSubmit(ClassSubject);
                }
                dataContext.SubmitChanges();

                if (model.IsChangeInOtherSections)
                {
                    var classSubjects = (from m1 in dataContext.ClassSubjects
                                         join s1 in dataContext.Sections
                                         on m1.SectionId equals s1.SectionId
                                         where s1.ClassId == ClassSubject.Section.ClassId
                                         && m1.SubjectId == ClassSubject.SubjectId
                                         && m1.AcademicSessionId == ClassSubject.AcademicSessionId
                                         select m1).ToList();
                    //var parentSubject = dataContext.ClassSubjects.Where(x => x.ClassSubjectId == ClassSubject.ParentClassSubjectId).FirstOrDefault();
                    classSubjects.ForEach(cl =>
                    {
                        cl.ClassSubjectStatus = model.ClassSubjectStatus;
                        cl.SubjectType = model.SubjectType;
                        cl.PracticalPeriods = model.PracticalPeriods;
                        cl.TheoryPeriods = model.TheoryPeriods;
                        cl.IsIncluded = model.IsIncluded;
                        //if (model.ParentClassSubjectId > 0)
                        //{
                        //    var classSubjectIds = dataContext.ClassSubjects.Where(x => x.SectionId == cl.SectionId && x.SubjectId == parentSubject.SubjectId);
                        //    if (classSubjectIds.Any())
                        //        cl.ParentClassSubjectId = classSubjectIds.First().ClassSubjectId;
                        //}
                    });
                    dataContext.SubmitChanges();
                }
                //ResponseMessage.ClassSubjectList = LoadData.GetClassSubjectList(dataContext, model.SectionId, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Subject is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteClassSubject")]
        public ExpandoObject DeleteClassSubject(int ClassSubjectId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var ClassSubject = dataContext.ClassSubjects.Where(x => x.ClassSubjectId == ClassSubjectId).First();
                //int SectionId = ClassSubject.SectionId;
                dataContext.ClassSubjects.DeleteOnSubmit(ClassSubject);
                dataContext.SubmitChanges();
                //ResponseMessage.ClassSubjectList = LoadData.GetClassSubjectList(dataContext, SectionId, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Subject is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("changePositionUp")]
        public ExpandoObject ChangePositionUp(int ClassSubjectId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var ClassSubject = dataContext.ClassSubjects.Where(x => x.ClassSubjectId == ClassSubjectId).First();
                ClassSubject.Position -= 1;
                dataContext.SubmitChanges();

                var UpClassSubjects = dataContext.ClassSubjects.Where(x => x.SectionId == ClassSubject.SectionId && x.Position == ClassSubject.Position && x.ClassSubjectId != ClassSubjectId);
                if (UpClassSubjects.Any())
                {
                    var UpClassSubject = UpClassSubjects.First();
                    UpClassSubject.Position += 1;
                    dataContext.SubmitChanges();
                }

                //ResponseMessage.ClassSubjectList = LoadData.GetClassSubjectList(dataContext, ClassSubject.SectionId, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Class Subject is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("changePositionDown")]
        public ExpandoObject ChangePositionDown(int ClassSubjectId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var ClassSubject = dataContext.ClassSubjects.Where(x => x.ClassSubjectId == ClassSubjectId).First();
                ClassSubject.Position += 1;
                dataContext.SubmitChanges();

                var UpClassSubjects = dataContext.ClassSubjects.Where(x => x.SectionId == ClassSubject.SectionId && x.Position == ClassSubject.Position && x.ClassSubjectId != ClassSubjectId);
                if (UpClassSubjects.Any())
                {
                    var UpClassSubject = UpClassSubjects.First();
                    UpClassSubject.Position -= 1;
                    dataContext.SubmitChanges();
                }

                //ResponseMessage.ClassSubjectList = LoadData.GetClassSubjectList(dataContext, ClassSubject.SectionId, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Class Subject is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("CopyClassSubjects")]
        public ExpandoObject CopyClassSubjects(int SectionId, int AcademicSessionId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int classId = dataContext.Sections.Where(x => x.SectionId == SectionId).First().ClassId;
                var sectionList = dataContext.Sections.Where(x => x.ClassId == classId).ToList();

                var subjectList = dataContext.ClassSubjects.Where(x => x.SectionId == SectionId && x.AcademicSessionId == AcademicSessionId).ToList();
                sectionList.ForEach(section =>
                {
                    subjectList.ForEach(subject =>
                    {
                        var classSubjects = dataContext.ClassSubjects.Where(x => x.SectionId == section.SectionId && x.SubjectId == subject.SubjectId && x.AcademicSessionId == AcademicSessionId);
                        if (!classSubjects.Any())
                        {
                            ClassSubject ClassSubject = new ClassSubject
                            {
                                SectionId = section.SectionId,
                                SubjectId = subject.SubjectId,
                                ClassSubjectStatus = subject.ClassSubjectStatus,
                                SubjectType = subject.SubjectType,
                                PracticalPeriods = subject.PracticalPeriods,
                                TheoryPeriods = subject.TheoryPeriods,
                                AcademicSessionId = subject.AcademicSessionId
                            };
                            int Position = 1;
                            var lastClasses = dataContext.ClassSubjects.Where(x => x.SectionId == section.SectionId && x.AcademicSessionId == AcademicSessionId);
                            if (lastClasses.Any())
                                Position = lastClasses.OrderByDescending(x => x.Position).First().Position + 1;
                            ClassSubject.Position = Position;
                            dataContext.ClassSubjects.InsertOnSubmit(ClassSubject);
                            dataContext.SubmitChanges();
                        }
                    });
                });
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Subject is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

    }
}
