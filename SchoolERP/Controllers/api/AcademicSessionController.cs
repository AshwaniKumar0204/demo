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
    [RoutePrefix("api/AcademicSession")]
    public class AcademicSessionController : ApiController
    {
        [HttpPost]
        [Route("AcademicSessionList")]
        public ExpandoObject AcademicSessionList(AcademicSessionModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (model.ClassNo != 0)
                    model.ClassId = dataContext.Classes.First(x => x.ClassNo == model.ClassNo).ClassId;
                var list = (from st1 in dataContext.AcademicSessions
                            join es in dataContext.Classes
                            on st1.ClassId equals es.ClassId
                            where (model.Status == 0 || st1.Status == model.Status)
                            && (model.ClassId == 0 || st1.ClassId == model.ClassId)
                            orderby st1.ClassId, st1.SessionStartDate descending
                            select new
                            {
                                st1.AcademicSessionId,
                                es.ClassName,
                                st1.ClassId,
                                st1.SessionName,
                                st1.SessionStartDate,
                                st1.SessionEndDate,
                                st1.Status,
                                StatusName = Enum.GetName(typeof(Status), st1.Status),
                            }).ToList();
                response.AcademicSessionList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveAcademicSession")]
        public ExpandoObject SaveAcademicSession(AcademicSession model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                AcademicSession AcademicSession = null;
                if (model.AcademicSessionId > 0)
                    AcademicSession = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == model.AcademicSessionId).First();
                else
                    AcademicSession = new AcademicSession();
                AcademicSession.AcademicSessionId = model.AcademicSessionId;
                AcademicSession.ClassId = model.ClassId;
                AcademicSession.SessionName = model.SessionName;
                AcademicSession.SessionStartDate = model.SessionStartDate;
                AcademicSession.SessionEndDate = model.SessionEndDate;
                AcademicSession.Status = model.Status;
                AcademicSession.UpdatedBy = SessionUtilies.GetLoginId();
                AcademicSession.UpdatedOn = DateTime.Now;
                if (AcademicSession.AcademicSessionId == 0)
                    dataContext.AcademicSessions.InsertOnSubmit(AcademicSession);
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
        [Route("deleteAcademicSession")]
        public ExpandoObject DeleteAcademicSession(int AcademicSessionId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var AcademicSession = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First();
                dataContext.AcademicSessions.DeleteOnSubmit(AcademicSession);
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
