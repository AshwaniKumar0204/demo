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
    [RoutePrefix("api/StaffLeave")]
    public class StaffLeaveController : ApiController
    {
        [HttpPost]
        [Route("StaffLeaveList")]
        public ExpandoObject StaffLeaveList(StaffLeave model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from m1 in dataContext.StaffLeaves
                           join m2 in dataContext.Staffs
                           on m1.StaffId equals m2.StaffId
                           join d1 in dataContext.Designations
                           on m2.DesignationId equals d1.DesignationId into subDes
                           where (model.StaffId == 0 || m1.StaffId == model.StaffId)
                           && (model.MonthNo == 0 || m1.MonthNo == model.MonthNo)
                           && (model.Year == 0 || m1.Year == model.Year)
                           orderby m1.Year descending, m1.MonthNo descending, m1.StaffLeaveId descending
                           select new
                           {
                               m1.StaffLeaveId,
                               m1.StaffId,
                               m2.StaffName,
                               m2.StaffCode,
                               m1.Remarks,
                               m1.MonthNo,
                               m1.Year,
                               m1.NoOfLeaves,
                               m1.LeaveType,
                               MonthName = Enum.GetName(typeof(MonthNames), m1.MonthNo),
                               LeaveTypeName = Enum.GetName(typeof(LeaveType), m1.LeaveType),
                               DesignationName = subDes.Any() ? subDes.First().DesignationName : "",
                           };
                ResponseMessage.StaffLeaveList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveStaffLeave")]
        public ExpandoObject SaveStaffLeave(StaffLeave model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int NoOfAvailableLeaves = 0;
                if (model.LeaveType != (byte)LeaveType.LWP)
                {
                    var leaves = SalaryData.GetStaffAvailableLeave(dataContext, model.StaffId, 0, 0, model.LeaveType,true);
                    NoOfAvailableLeaves = leaves.First().NoOfLeaves;
                }

                StaffLeave StaffLeave = null;
                if (model.StaffLeaveId > 0)
                {
                    StaffLeave = dataContext.StaffLeaves.Where(x => x.StaffLeaveId == model.StaffLeaveId).First();
                    if (model.LeaveType != (byte)LeaveType.LWP)
                    {
                        NoOfAvailableLeaves += StaffLeave.NoOfLeaves;
                        if (NoOfAvailableLeaves < model.NoOfLeaves)
                            throw new Exception("No of leaves Should not be more than " + NoOfAvailableLeaves);
                    }

                }
                else
                {
                    if (model.LeaveType != (byte)LeaveType.LWP)
                    {
                        if (NoOfAvailableLeaves < 1)
                            throw new Exception("No Leave is available.");
                        if (NoOfAvailableLeaves < model.NoOfLeaves)
                            throw new Exception("No of leaves Should not be more than available leave.");
                    }
                    StaffLeave = new StaffLeave();
                }
                StaffLeave.StaffId = model.StaffId;
                StaffLeave.MonthNo = model.MonthNo;
                StaffLeave.Year = model.Year;
                StaffLeave.NoOfLeaves = model.NoOfLeaves;
                StaffLeave.LeaveType = model.LeaveType;
                StaffLeave.Remarks = model.Remarks;
                StaffLeave.LastUpdatedBy = SessionUtilies.GetLoginId();
                StaffLeave.LastUpdatedOn = DateTime.Now;
                if (StaffLeave.StaffLeaveId == 0)
                    dataContext.StaffLeaves.InsertOnSubmit(StaffLeave);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Staff Leave is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteStaffLeave")]
        public ExpandoObject DeleteStaffLeave(int StaffLeaveId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var StaffLeave = dataContext.StaffLeaves.Where(x => x.StaffLeaveId == StaffLeaveId).First();
                dataContext.StaffLeaves.DeleteOnSubmit(StaffLeave);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Staff Leave is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
