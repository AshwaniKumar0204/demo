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
    [RoutePrefix("api/Subject")]
    public class SubjectController : ApiController
    {
        [HttpGet]
        [Route("SubjectList")]
        public ExpandoObject SubjectList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.SubjectList = LoadData.GetSubjectList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveSubject")]
        public ExpandoObject SaveSubject(SubjectModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Subject Subject = new Subject();
                if (model.SubjectId > 0)
                    Subject = dataContext.Subjects.Where(x => x.SubjectId == model.SubjectId).First();
                Subject.SubjectId = model.SubjectId;
                Subject.SubjectName = model.SubjectName;
                Subject.SubjectStatus = model.SubjectStatus;
                Subject.SubjectCode = model.SubjectCode;
                Subject.ParentSubjectId = model.ParentSubjectId;
                if (Subject.SubjectId == 0)
                    dataContext.Subjects.InsertOnSubmit(Subject);
                dataContext.SubmitChanges();
                ResponseMessage.SubjectList = LoadData.GetSubjectList(dataContext, 0);
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
        [Route("deleteSubject")]
        public ExpandoObject DeleteSubject(int SubjectId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Subject = dataContext.Subjects.Where(x => x.SubjectId == SubjectId).First();
                dataContext.Subjects.DeleteOnSubmit(Subject);
                dataContext.SubmitChanges();
                ResponseMessage.SubjectList = LoadData.GetSubjectList(dataContext, 0);
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
    }
}
