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
    [RoutePrefix("api/Attendence")]
    public class AttendenceController : ApiController
    {
        [HttpPost]
        [Route("AttendenceList")]
        public ExpandoObject AttendenceList(DateFilterModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.AttendenceList = AttendanceData.AttendanceList(dataContext, model);
                ResponseMessage.Message = ConstantData.SuccessMessage;
                //List<AttendenceModel> list = new List<AttendenceModel>();
                //var Attendences = from s1 in dataContext.Attendences
                //                  where (model.FromDate.HasValue ? s1.EntryDateTime.Date >= model.FromDate : true)
                //                  && (model.ToDate.HasValue ? s1.EntryDateTime.Date <= model.ToDate : true)
                //                  && (model.Id != null ? s1.AttendenceCode == model.Id : true)
                //                  group s1 by s1.AttendenceCode into subAttendence
                //                  select subAttendence;
                //foreach (var s1 in Attendences)
                //{
                //    var staffs = from a1 in dataContext.Staffs
                //                 join a2 in (from g1 in dataContext.StaffGroupDetails
                //                             join g2 in dataContext.StaffGroups
                //                             on g1.StaffGroupId equals g2.StaffGroupId
                //                             where g2.Status == (byte)Status.Active
                //                             select new { g1.StaffId, g2.GroupName, g2.StaffGroupId })
                //                 on a1.StaffId equals a2.StaffId into subStaffGroups
                //                 from a3 in subStaffGroups.DefaultIfEmpty()
                //                 join sf1 in dataContext.StaffGroupShifts on a3.StaffGroupId equals sf1.StaffGroupId into subStaffShifts
                //                 from sStaffShift in subStaffShifts.DefaultIfEmpty()
                //                 where a1.AttendenceCode == s1.Key
                //                 && (model.StaffGroupId == 0 || a3.StaffGroupId == model.StaffGroupId)
                //                 && (model.ShiftId == 0 || sStaffShift.ShiftId == model.ShiftId)
                //                 select new { a1, a3.GroupName };

                //    // where (model.StaffGroupId == 0 || subStaffGroups.Any())

                //    if (staffs.Count() == 0)
                //        continue;
                //    var staff = staffs.First().a1;
                //    var GroupName = staffs.First().GroupName;
                //    foreach (var s2 in s1.GroupBy(x => x.EntryDateTime.Date))
                //    {
                //        AttendenceModel att = new AttendenceModel
                //        {
                //            AttendenceCode = staff.AttendenceCode,
                //            FullName = staff.StaffName,
                //            MobileNo = staff.MobileNo,
                //            EntryDateTime = s2.Key,
                //            InTime = s2.OrderBy(x => x.EntryTime).First().EntryTime,
                //            OutTime = s2.OrderByDescending(x => x.EntryTime).First().EntryTime,
                //            DesignationName = staff.Designation.DesignationName,
                //        };
                //        if (att.InTime == att.OutTime)
                //            att.OutTime = null;

                //        //Staff Shift
                //        var shifts = from a1 in dataContext.StaffGroupDetails
                //                     join a2 in dataContext.StaffGroups
                //                     on a1.StaffGroupId equals a2.StaffGroupId
                //                     join a3 in dataContext.StaffGroupShifts
                //                     on a2.StaffGroupId equals a3.StaffGroupId
                //                     join a4 in dataContext.Shifts
                //                     on a3.ShiftId equals a4.ShiftId
                //                     where a1.StaffId == staff.StaffId
                //                     && a2.Status == 1
                //                     && a4.Status == 1
                //                     && a3.ShiftDate.Date == s2.Key.Date
                //                     select a4;
                //        if (shifts.Any())
                //        {
                //            var shift = shifts.First();
                //            att.Shift = new ShiftModel
                //            {
                //                GapeTime = shift.GapeTime,
                //                InTime = shift.InTime,
                //                OutTime = shift.OutTime,
                //                ShiftDurationString = LoadData.DurationString(shift.ShiftDuration),
                //                ShiftName = shift.ShiftName
                //            };
                //            att.GroupName = GroupName;
                //            att.InGapeTime = att.InTime - shift.InTime;
                //            att.OverInTime = att.InGapeTime - shift.GapeTime;
                //            att.OutGapeTime = shift.OutTime - att.OutTime;
                //            att.OverOutTime = att.OutGapeTime - shift.GapeTime;
                //        }

                //        list.Add(att);
                //    }
                //}

            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("AttendenceCodeList")]
        public ExpandoObject AttendenceCodeList(Staff model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from s1 in dataContext.Staffs
                           select new
                           {
                               s1.StaffId,
                               s1.StaffCode,
                               s1.Designation.DesignationName,
                               s1.Department.DepartmentName,
                               s1.AttendenceCode,
                               s1.StaffName
                           };

                ResponseMessage.StaffList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("updateAttendenceCode")]
        public ExpandoObject UpdateAttendenceCode(List<Staff> staffList)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                staffList.ForEach(st =>
                {
                    var staff = dataContext.Staffs.Where(x => x.StaffId == st.StaffId).First();
                    if (staff.AttendenceCode != st.AttendenceCode)
                    {
                        if (dataContext.Staffs.Where(x => x.AttendenceCode == st.AttendenceCode).Any() && !string.IsNullOrEmpty(st.AttendenceCode))
                            throw new Exception("Attendence Code '" + st.AttendenceCode + "' is already assign to another staff.");
                        staff.AttendenceCode = st.AttendenceCode;
                        dataContext.SubmitChanges();
                    }
                });

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
