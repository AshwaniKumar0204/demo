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
    [RoutePrefix("api/Department")]
    public class DepartmentController : ApiController
    {
        [HttpGet]
        [Route("DepartmentList")]
        public ExpandoObject DepartmentList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.DepartmentList = LoadData.GetDepartmentList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveDepartment")]
        public ExpandoObject SaveDepartment(DepartmentModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Department Department = new Department();
                if (model.DepartmentId > 0)
                    Department = dataContext.Departments.Where(x => x.DepartmentId == model.DepartmentId).First();
                Department.DepartmentId = model.DepartmentId;
                Department.DepartmentName = model.DepartmentName;
                Department.DepartmentStatus = model.DepartmentStatus;
                if (Department.DepartmentId == 0)
                    dataContext.Departments.InsertOnSubmit(Department);
                dataContext.SubmitChanges();
                ResponseMessage.DepartmentList = LoadData.GetDepartmentList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Department is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteDepartment")]
        public ExpandoObject DeleteDepartment(int DepartmentId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Department = dataContext.Departments.Where(x => x.DepartmentId == DepartmentId).First();
                dataContext.Departments.DeleteOnSubmit(Department);
                dataContext.SubmitChanges();
                ResponseMessage.DepartmentList = LoadData.GetDepartmentList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Department is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
