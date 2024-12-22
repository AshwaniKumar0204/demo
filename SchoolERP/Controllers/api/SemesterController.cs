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
    [RoutePrefix("api/Semester")]
    public class SemesterController : ApiController
    {
        [HttpPost]
        [Route("SemesterList")]
        public ExpandoObject SemesterList(Semester model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from es1 in dataContext.Semesters
                           join es in dataContext.Classes
                           on es1.ClassId equals es.ClassId
                           where (model.Status == 0 || es1.Status == model.Status)
                           && (es1.ClassId == model.ClassId)
                           orderby es1.SemesterNo
                           select new
                           {
                               SemesterId = es1.SemesterId,
                               Status = es1.Status,
                               StatusName = Enum.GetName(typeof(Status), es1.Status),
                               ClassId = es1.ClassId,
                               SemesterName = es1.SemesterName,
                               es1.SemesterNo,
                               ClassName = es.ClassName,
                               ClassNo = es.ClassNo,
                           };
                response.SemesterList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveSemester")]
        public ExpandoObject SaveSemester(Semester model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Semester Semester = new Semester();
                if (model.SemesterId > 0)
                    Semester = dataContext.Semesters.Where(x => x.SemesterId == model.SemesterId).First();
                Semester.SemesterId = model.SemesterId;
                Semester.ClassId = model.ClassId;
                Semester.SemesterName = model.SemesterName;
                Semester.SemesterNo = model.SemesterNo;
                Semester.Status = model.Status;
                Semester.UpdatedBy = SessionUtilies.GetLoginId();
                Semester.UpdatedOn = DateTime.Now;
                if (Semester.SemesterId == 0)
                    dataContext.Semesters.InsertOnSubmit(Semester);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {

                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteSemester")]
        public ExpandoObject DeleteSemester(int SemesterId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Semester = dataContext.Semesters.Where(x => x.SemesterId == SemesterId).First();
                int ClassId = Semester.ClassId;
                dataContext.Semesters.DeleteOnSubmit(Semester);
                dataContext.SubmitChanges();
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
