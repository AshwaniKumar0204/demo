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
    [RoutePrefix("api/class")]
    public class ClassController : ApiController
    {
        [HttpGet]
        [Route("classList")]
        public ExpandoObject ClassList(byte ClassStatus)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from st1 in dataContext.Classes
                            where st1.ClassStatus == ClassStatus
                            orderby st1.ClassId
                            select new
                            {
                                st1.ClassId,
                                st1.ClassName,
                                st1.ClassName2,
                                st1.ClassStatus,
                                st1.ClassNo
                            }).ToList();
                response.ClassList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("classListDistinct")]
        public ExpandoObject ClassListDistinct(byte ClassStatus)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int loginId = SessionUtilies.GetLoginId();
                List<int> sectionIds = LoadData.EmployeeSectionIds(dataContext, loginId);
                //List<int> roleIds = dataContext.SystemLoginRoles.Where(x => x.LoginId == SessionUtilies.GetLoginId()).Select(x => x.RoleId).ToList();
                var list = (from st1 in dataContext.Classes
                            join s1 in dataContext.Sections
                            on st1.ClassId equals s1.ClassId
                            //join r1 in dataContext.RoleSections.Where(x => roleIds.Contains(x.RoleId)).Distinct()
                            //on s1.SectionId equals r1.SectionId
                            where st1.ClassStatus == ClassStatus
                            && (sectionIds.Contains(s1.SectionId))
                            select new
                            {
                                ClassName = st1.ClassName2,
                                st1.ClassNo
                            }).Distinct().OrderBy(x => x.ClassNo).ToList();
                response.ClassList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("AvailableStudents")]
        public ExpandoObject AvailableStudents(int AcademicSessionId, int SectionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var totalStudents = (from s1 in dataContext.Admissions
                                     where s1.AcademicSessionId == AcademicSessionId
                                     && s1.SectionId == SectionId
                                     where s1.AdmissionStatus == (byte)AdmissionStatus.Active
                                     select s1).Count();
                if (totalStudents > 0)
                {
                    var lastRollNo = (from s1 in dataContext.Admissions
                                      where s1.AcademicSessionId == AcademicSessionId
                                      && s1.SectionId == SectionId
                                      where s1.AdmissionStatus == (byte)AdmissionStatus.Active
                                      orderby s1.RollNo descending
                                      select s1.RollNo).First();
                    totalStudents = lastRollNo ?? 0;
                }
                response.AvailableStudents = totalStudents;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveClass")]
        public ExpandoObject SaveClass(ClassModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Class Class = new Class();
                if (model.ClassId > 0)
                    Class = dataContext.Classes.Where(x => x.ClassId == model.ClassId).First();
                Class.ClassName = model.ClassName;
                Class.ClassName2 = model.ClassName2;
                //Class.Term = model.Term;
                Class.ClassStatus = model.ClassStatus;
                if (Class.ClassId == 0)
                {
                    Class.ClassNo = model.ClassNo;
                    dataContext.Classes.InsertOnSubmit(Class);
                }
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Class is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteClass")]
        public ExpandoObject DeleteClass(int ClassId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Class = dataContext.Classes.Where(x => x.ClassId == ClassId).First();
                dataContext.Classes.DeleteOnSubmit(Class);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Class is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
