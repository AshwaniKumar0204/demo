using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/Leave")]
    public class LeaveController : ApiController
    {
        [HttpGet]
        [Route("LeaveList")]
        public ExpandoObject LeaveList(byte Status, byte staffType, int Year)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from m1 in dataContext.Leaves
                           join login in dataContext.SystemLogins
                           on m1.LastUpdatedBy equals login.LoginId into sublogins
                           where (Status == 0 || Status == m1.Status)
                           && (Year == 0 || m1.Year == Year)
                           && (staffType == 0 || (staffType == (byte)StaffType.NonTeaching ? m1.LeaveType != (byte)LeaveType.HPL : m1.LeaveType != (byte)LeaveType.EL))
                           orderby m1.Year descending, m1.LeaveType
                           select new
                           {
                               m1.LeaveId,
                               m1.LeaveType,
                               LeaveTypeName = Enum.GetName(typeof(LeaveType), m1.LeaveType),
                               m1.NoOfDays,
                               m1.Status,
                               m1.Year,
                               m1.LastUpatedOn,
                               LastUpdatedByName = sublogins.Any() ? sublogins.First().UserName : "",
                           };
                ResponseMessage.LeaveList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveLeave")]
        public ExpandoObject SaveLeave(Leave model)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;
                Leave Leave = new Leave();
                if (model.LeaveId > 0)
                    Leave = dataContext.Leaves.Where(x => x.LeaveId == model.LeaveId).First();
                Leave.LeaveType = model.LeaveType;
                Leave.NoOfDays = model.NoOfDays;
                Leave.Year = model.Year;
                Leave.Status = model.Status;
                Leave.LastUpatedOn = DateTime.Now;
                Leave.LastUpdatedBy = SessionUtilies.GetLoginId();
                if (Leave.LeaveId == 0)
                    dataContext.Leaves.InsertOnSubmit(Leave);
                dataContext.SubmitChanges();

                model.StaffAvailableLeaves.Where(x => x.NoOfLeaves > 0).ToList().ForEach(sl =>
                  {
                      StaffAvailableLeave staffAvailableLeave = null;
                      if (sl.StaffAvailableLeaveId != 0)
                          staffAvailableLeave = dataContext.StaffAvailableLeaves.Where(x => x.StaffAvailableLeaveId == sl.StaffAvailableLeaveId).First();
                      else
                          staffAvailableLeave = new StaffAvailableLeave();
                      staffAvailableLeave.LeaveId = Leave.LeaveId;
                      staffAvailableLeave.StaffId = sl.StaffId;
                      staffAvailableLeave.NoOfLeaves = sl.NoOfLeaves;
                      if (staffAvailableLeave.StaffAvailableLeaveId == 0)
                          dataContext.StaffAvailableLeaves.InsertOnSubmit(staffAvailableLeave);
                      dataContext.SubmitChanges();
                  });

                model.StaffAvailableLeaves.Where(x => x.NoOfLeaves == 0 && x.StaffAvailableLeaveId != 0).ToList().ForEach(sl =>
                 {
                     StaffAvailableLeave staffAvailableLeave = dataContext.StaffAvailableLeaves.Where(x => x.StaffAvailableLeaveId == sl.StaffAvailableLeaveId).First();
                     dataContext.StaffAvailableLeaves.DeleteOnSubmit(staffAvailableLeave);
                     dataContext.SubmitChanges();
                 });


                transaction.Commit();
                IsCompleted = true;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                if (ex.Message.Contains("IX"))
                    response.Message = "This Leave is already exist";
                else response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpGet]
        [Route("deleteLeave")]
        public ExpandoObject DeleteLeave(int LeaveId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Leave = dataContext.Leaves.Where(x => x.LeaveId == LeaveId).First();
                dataContext.Leaves.DeleteOnSubmit(Leave);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Leave is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("LeaveYearList")]
        public ExpandoObject LeaveYearList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                List<int> list = dataContext.Leaves.Select(x => x.Year).OrderByDescending(x => x).Distinct().ToList();
                if (list.Any())
                    list.Add(list.First() + 1);
                else
                {
                    list.Add(DateTime.Now.Year + 1);
                    list.Add(DateTime.Now.Year);
                    list.Add(DateTime.Now.Year - 1);
                }
                ResponseMessage.YearList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Leave is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
