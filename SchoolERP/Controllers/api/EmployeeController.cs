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
    [RoutePrefix("api/Employee")]
    public class EmployeeController : ApiController
    {

        [HttpGet]
        [Route("EmployeeList")]
        public ExpandoObject EmployeeList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.EmployeeList = LoadData.GetEmployeeList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveEmployee")]
        public ExpandoObject SaveEmployee(Employee model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Employee Employee = null;
                if (model.EmployeeId > 0)
                    Employee = dataContext.Employees.Where(x => x.EmployeeId == model.EmployeeId).First();
                else
                {
                    if (dataContext.Employees.Where(x => x.StaffId == model.StaffId).Any() && model.StaffId > 0)
                        throw new Exception("This staff is already available!!");
                    Employee = new Employee();
                }
                Employee.StaffId = model.StaffId;
                Employee.Address = model.Address;
                Employee.AlternateNo = model.AlternateNo;
                Employee.DateOfBirth = model.DateOfBirth;
                Employee.EmailId = model.EmailId;
                Employee.EmployeeName = model.EmployeeName;
                Employee.EmployeeStatus = model.EmployeeStatus;
                Employee.EmployeeTypeId = model.EmployeeTypeId;
                Employee.Gender = model.Gender;
                Employee.IsSuperAdmin = model.IsSuperAdmin;
                Employee.JoinDate = model.JoinDate;
                Employee.MobileNo = model.MobileNo;
                if (Employee.EmployeeId == 0)
                {
                    Employee.EmployeeCode = LoadData.GenerateEmployeeCode(dataContext);
                    dataContext.Employees.InsertOnSubmit(Employee);
                }
                dataContext.SubmitChanges();
                ResponseMessage.EmployeeList = LoadData.GetEmployeeList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Employee is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteEmployee")]
        public ExpandoObject DeleteEmployee(int EmployeeId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Employee = dataContext.Employees.Where(x => x.EmployeeId == EmployeeId).First();
                dataContext.Employees.DeleteOnSubmit(Employee);
                dataContext.SubmitChanges();
                ResponseMessage.EmployeeList = LoadData.GetEmployeeList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Employee is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }


    }
}
