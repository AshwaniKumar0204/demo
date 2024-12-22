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
    [RoutePrefix("api/ExamCenter")]
    public class ExamCenterController : ApiController
    {
        [HttpGet]
        [Route("ExamCenterList")]
        public ExpandoObject ExamCenterList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from e1 in dataContext.ExamCenters
                            where (Status == 0 ? true : e1.Status == Status)
                            select new
                            {
                                e1.Exam,
                                e1.ExamCenterId,
                                e1.ExamCenterName,
                                e1.ReportingTime,
                                e1.ExamTime,
                                e1.ExamDate,
                                e1.Status
                            }).ToList();
                ResponseMessage.ExamCenterList = list;
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveExamCenter")]
        public ExpandoObject SaveExamCenter(ExamCenter model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ExamCenter ExamCenter = new ExamCenter();
                if (model.ExamCenterId > 0)
                    ExamCenter = dataContext.ExamCenters.Where(x => x.ExamCenterId == model.ExamCenterId).First();
                ExamCenter.ExamCenterName = model.ExamCenterName;
                ExamCenter.ReportingTime = model.ReportingTime;
                ExamCenter.ExamTime = model.ExamTime;
                ExamCenter.ExamDate = model.ExamDate;
                ExamCenter.Exam = model.Exam;
                ExamCenter.Status = model.Status;
                if (ExamCenter.ExamCenterId == 0)
                    dataContext.ExamCenters.InsertOnSubmit(ExamCenter);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Exam Center is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteExamCenter")]
        public ExpandoObject DeleteExamCenter(int ExamCenterId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var ExamCenter = dataContext.ExamCenters.Where(x => x.ExamCenterId == ExamCenterId).First();
                dataContext.ExamCenters.DeleteOnSubmit(ExamCenter);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Exam Center is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
