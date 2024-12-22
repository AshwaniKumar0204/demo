using OfficeOpenXml;
using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/StaffAvailableLeave")]
    public class StaffAvailableLeaveController : ApiController
    {
        [HttpGet]
        [Route("StaffAvailableLeaveList")]
        public ExpandoObject StaffAvailableLeaveList(int leaveType, int Year)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from s1 in dataContext.Staffs
                           join s2 in dataContext.StaffAvailableLeaves.Join(dataContext.Leaves.Where(x => x.Year == Year && x.LeaveType == leaveType), x => x.LeaveId, y => y.LeaveId, (x, y) => x)
                           on s1.StaffId equals s2.StaffId into subLeaves
                           where (leaveType == (byte)LeaveType.EL ? s1.StaffType == (byte)StaffType.NonTeaching : true)
                           && (leaveType == (byte)LeaveType.HPL ? s1.StaffType == (byte)StaffType.Teacher : true)
                           select new
                           {
                               s1.StaffId,
                               s1.StaffName,
                               s1.StaffCode,
                               s1.Designation.DesignationName,
                               StaffAvailableLeaveId = subLeaves.Any() ? subLeaves.First().StaffAvailableLeaveId : 0,
                               NoOfLeaves = subLeaves.Any() ? subLeaves.First().NoOfLeaves : 0,
                               IsSelected = subLeaves.Any() ? true : false,
                           };
                response.StaffAvailableLeaveList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("StaffAvailableLeaves")]
        public ExpandoObject StaffAvailableLeaves(int StaffId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = SalaryData.GetStaffAvailableLeave(dataContext, StaffId, 0, 0, 0, true);
                response.StaffAvailableLeaves = list.ToList();
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
