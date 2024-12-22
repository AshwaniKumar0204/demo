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
    [RoutePrefix("api/Staff")]
    public class StaffController : ApiController
    {
        [HttpGet]
        [Route("SearchStaffList")]
        public ExpandoObject SearchStaffList(int status)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from s1 in dataContext.Staffs
                           where status == 0 || s1.StaffStatus == status
                           select new
                           {
                               s1.StaffId,
                               SearchStaff = s1.StaffCode + " " + s1.StaffName,
                               s1.AttendenceCode,
                               SearchAttendenceStaff = (s1.AttendenceCode != null ? (s1.AttendenceCode + " - ") : "") + s1.StaffName
                           };
                response.StaffList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("StaffList")]
        public ExpandoObject StaffList(byte Status, int DepartmentId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.StaffList = LoadData.StaffList(dataContext, Status, DepartmentId);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveStaff")]
        public ExpandoObject SaveStaff(Staff model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Staff Staff = new Staff();
                if (model.StaffId > 0)
                    Staff = dataContext.Staffs.Where(x => x.StaffId == model.StaffId).First();
                Staff.Address = model.Address;
                Staff.AlternateNo = model.AlternateNo;
                Staff.DateOfBirth = model.DateOfBirth;
                Staff.EmailId = model.EmailId;
                Staff.StaffName = model.StaffName;
                Staff.StaffStatus = model.StaffStatus;
                Staff.DepartmentId = model.DepartmentId;
                Staff.DesignationId = model.DesignationId;
                Staff.Gender = model.Gender;
                Staff.JoinDate = model.JoinDate;
                Staff.MobileNo = model.MobileNo;
               // Staff.AttendenceCode = model.AttendenceCode;
                Staff.StaffType = model.StaffType;
                Staff.PanNo = model.PanNo;
                Staff.BankAccountNo = model.BankAccountNo;
                Staff.PFNo = model.PFNo;
                Staff.ESINo = model.ESINo;
                if (Staff.StaffId == 0)
                {
                    Staff.CreatedOn = DateTime.Now;
                    Staff.CreatedBy = SessionUtilies.GetLoginId();
                    Staff.StaffCode = LoadData.GenerateStaffCode(dataContext);
                    dataContext.Staffs.InsertOnSubmit(Staff);
                }
                else
                {
                    Staff.LastUpdatedBy = SessionUtilies.GetLoginId();
                    Staff.LastUpdatedOn = DateTime.Now;
                }
                dataContext.SubmitChanges();

                ResponseMessage.StaffList = LoadData.StaffList(dataContext, 0, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Staff is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteStaff")]
        public ExpandoObject DeleteStaff(int StaffId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Staff = dataContext.Staffs.Where(x => x.StaffId == StaffId).First();
                dataContext.Staffs.DeleteOnSubmit(Staff);
                dataContext.SubmitChanges();
                ResponseMessage.StaffList = LoadData.StaffList(dataContext, 0, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Staff is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }


    }
}
