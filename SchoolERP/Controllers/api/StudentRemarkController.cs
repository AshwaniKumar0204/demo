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
    [RoutePrefix("api/StudentRemark")]
    public class StudentRemarkController : ApiController
    {
        [HttpPost]
        [Route("SaveStudentRemark")]
        public ExpandoObject SaveStudentRemark(StudentRemarkModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StudentRemark StudentRemark = new StudentRemark
                {
                    Remarks = model.Remarks,
                    StudentId = model.StudentId,
                    RemarksDate = DateTime.Now,
                };
                dataContext.StudentRemarks.InsertOnSubmit(StudentRemark);
                dataContext.SubmitChanges();

                response.StudentRemarkList = SchoolData.GetStudentRemarkList(dataContext, model.StudentId);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("StudentRemarkList")]
        public ExpandoObject StudentRemarkList(int StudentId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                response.StudentRemarkList = SchoolData.GetStudentRemarkList(dataContext, StudentId);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("DeleteStudentRemark")]
        public ExpandoObject DeleteStudentRemark(int StudentRemarkId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StudentRemark StudentRemark = dataContext.StudentRemarks.Where(x => x.StudentRemarkId == StudentRemarkId).First();
                int StudentId = StudentRemark.StudentId;
                dataContext.StudentRemarks.DeleteOnSubmit(StudentRemark);
                dataContext.SubmitChanges();
                response.StudentRemarkList = SchoolData.GetStudentRemarkList(dataContext, StudentId);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
