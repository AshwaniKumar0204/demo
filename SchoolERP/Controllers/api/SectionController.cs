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
    [RoutePrefix("api/section")]
    public class SectionController : ApiController
    {
        [HttpGet]
        [Route("SectionList")]
        public ExpandoObject SectionList(byte SectionStatus, int ClassId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //List<int> roleIds = dataContext.SystemLoginRoles.Where(x => x.LoginId == SessionUtilies.GetLoginId()).Select(x => x.RoleId).ToList();
                List<int> sectionIds = LoadData.EmployeeSectionIds(dataContext, SessionUtilies.GetLoginId());

               /* var list = from es1 in dataContext.Sections
                           join es in dataContext.Classes
                           on es1.ClassId equals es.ClassId
                           //join r1 in dataContext.RoleSections.Where(x => roleIds.Contains(x.RoleId)).Distinct()
                           //on es1.SectionId equals r1.SectionId
                           where (SectionStatus > 0 ? es1.SectionStatus == SectionStatus : true)
                           && (ClassId > 0 ? es1.ClassId == ClassId : true)
                           && sectionIds.Contains(es1.SectionId)
                           orderby es1.SectionName
                           select new SectionModel
                           {
                               SectionId = es1.SectionId,
                               SectionStatus = es1.SectionStatus,
                               ClassId = es1.ClassId,
                               SectionName = es1.SectionName,
                               ClassName = es.ClassName,
                               ClassNo = es.ClassNo,
                           };
                response.SectionList = list.ToList();*/
                var list = from es1 in dataContext.Sections
                           join es in dataContext.Classes
                           on es1.ClassId equals es.ClassId
                           orderby es1.SectionName
                           select new SectionModel
                           {
                               SectionId = es1.SectionId,
                               SectionStatus = es1.SectionStatus,
                               ClassId = es1.ClassId,
                               SectionName = es1.SectionName,
                               ClassName = es.ClassName,
                               ClassNo = es.ClassNo,
                           };
                response.SectionList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("SectionListForClassTeacher")]
        public ExpandoObject SectionListForClassTeacher(byte SectionStatus, int ClassId, int AcademicSessionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int teacherId = SessionUtilies.GetTeacherId(dataContext);
                List<int> sectionIds = new List<int>();
                if (teacherId == 0)
                {
                    int loginId = SessionUtilies.GetLoginId();
                    sectionIds = LoadData.EmployeeSectionIds(dataContext, loginId);
                }
                //else
                //    sectionIds = dataContext.TeacherSessions.Where(x => x.AcademicSessionId == AcademicSessionId && x.TeacherId == teacherId).Select(x => x.ClassTeacherSectionId ?? 0).ToList();
                var list = from es1 in dataContext.Sections
                           join es in dataContext.Classes
                           on es1.ClassId equals es.ClassId
                           where (SectionStatus > 0 ? es1.SectionStatus == SectionStatus : true)
                           && (ClassId > 0 ? es1.ClassId == ClassId : true)
                           && sectionIds.Contains(es1.SectionId)
                           orderby es1.SectionName
                           select new SectionModel
                           {
                               SectionId = es1.SectionId,
                               SectionStatus = es1.SectionStatus,
                               ClassId = es1.ClassId,
                               SectionName = es1.SectionName,
                               ClassName = es.ClassName,
                               ClassNo = es.ClassNo,
                           };
                response.SectionList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("SectionList2")]
        public ExpandoObject SectionList2(byte SectionStatus, int ClassId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                response.SectionList = LoadData.GetSectionList(dataContext, ClassId, SectionStatus);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }


        [HttpPost]
        [Route("SaveSection")]
        public ExpandoObject SaveSection(SectionModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Section Section = new Section();
                if (model.SectionId > 0)
                    Section = dataContext.Sections.Where(x => x.SectionId == model.SectionId).First();
                Section.SectionId = model.SectionId;
                Section.ClassId = model.ClassId;
                Section.SectionName = model.SectionName;
                Section.SectionStatus = model.SectionStatus;
                if (Section.SectionId == 0)
                    dataContext.Sections.InsertOnSubmit(Section);
                dataContext.SubmitChanges();
                ResponseMessage.SectionList = LoadData.GetSectionList(dataContext, model.ClassId, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {

                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteSection")]
        public ExpandoObject DeleteSection(int SectionId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Section = dataContext.Sections.Where(x => x.SectionId == SectionId).First();
                int ClassId = Section.ClassId;
                dataContext.Sections.DeleteOnSubmit(Section);
                dataContext.SubmitChanges();
                ResponseMessage.SectionList = LoadData.GetSectionList(dataContext, ClassId, 0);
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
