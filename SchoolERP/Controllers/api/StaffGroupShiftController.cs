using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using School;

namespace MarketingSoftware.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/StaffGroupShift")]
    public class StaffGroupShiftController : ApiController
    {
        [HttpGet]
        [Route("StaffGroupShiftList")]
        public ExpandoObject StaffGroupShiftList(int StaffGroupId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var lastStaffGroupShifts = dataContext.StaffGroupShifts.Where(x => x.StaffGroupId == StaffGroupId).OrderByDescending(x => x.ShiftDate);
                if (lastStaffGroupShifts.Any())
                    ResponseMessage.LastStaffGroupShift = lastStaffGroupShifts.Select(x => new { x.StaffGroupId, x.ShiftId, x.ShiftDate, x.StaffGroupShiftId }).First();
                else
                    ResponseMessage.LastStaffGroupShift = null;

                var list = (from s1 in dataContext.StaffGroupShifts
                            join s2 in dataContext.Shifts
                            on s1.ShiftId equals s2.ShiftId
                            where s1.StaffGroupId == StaffGroupId
                            orderby s1.ShiftDate descending
                            select new
                            {
                                s1.StaffGroupId,
                                s1.StaffGroupShiftId,
                                s1.ShiftDate,
                                s1.ShiftId,
                                s2.ShiftName,
                                ShiftDurationString = LoadData.DurationString(s2.ShiftDuration),
                                s2.InTime,
                                s2.OutTime,
                                s2.GapeTime,

                            }).ToList();

                ResponseMessage.StaffGroupShiftList = list;
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveStaffGroupShift")]
        public ExpandoObject SaveStaffGroupShift(SaveStaffGroupShiftModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int loginId = SessionUtilies.GetLoginId();
                model.StaffGroupShiftList.ForEach(shiftModel =>
                {
                    //if (shiftModel.FromDate.Date == shiftModel.ToDate.Date)
                    //{
                    //    StaffGroupShift StaffGroupShift = new StaffGroupShift
                    //    {
                    //        StaffGroupId = model.StaffGroupId,
                    //        ShiftId = shiftModel.ShiftId,
                    //        ShiftDate = shiftModel.FromDate,
                    //        LastUpdatedBy = loginId,
                    //        LastUpdatedOn = DateTime.Now
                    //    };
                    //    dataContext.StaffGroupShifts.InsertOnSubmit(StaffGroupShift);
                    //    dataContext.SubmitChanges();
                    //}
                    //else
                    //{
                    while (shiftModel.FromDate.Date <= shiftModel.ToDate.Date)
                    {
                        StaffGroupShift StaffGroupShift = null;
                        var staffGroupShifts = dataContext.StaffGroupShifts.Where(x => x.ShiftDate.Date == shiftModel.FromDate.Date && x.StaffGroupId == model.StaffGroupId);
                        if (staffGroupShifts.Any())
                            StaffGroupShift = staffGroupShifts.First();
                        else
                            StaffGroupShift = new StaffGroupShift
                            {
                                StaffGroupId = model.StaffGroupId,
                                ShiftDate = shiftModel.FromDate,
                            };
                        StaffGroupShift.ShiftId = shiftModel.ShiftId;
                        StaffGroupShift.LastUpdatedBy = loginId;
                        StaffGroupShift.LastUpdatedOn = DateTime.Now;

                        if (StaffGroupShift.StaffGroupShiftId == 0)
                            dataContext.StaffGroupShifts.InsertOnSubmit(StaffGroupShift);
                        dataContext.SubmitChanges();
                        shiftModel.FromDate = shiftModel.FromDate.AddDays(1);
                    }
                    //}

                });
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteStaffGroupShift")]
        public ExpandoObject DeleteStaffGroupShift(int StaffGroupShiftId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var StaffGroupShift = dataContext.StaffGroupShifts.Where(x => x.StaffGroupShiftId == StaffGroupShiftId).First();
                dataContext.StaffGroupShifts.DeleteOnSubmit(StaffGroupShift);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Staff Group Shift is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        //[HttpGet]
        //[Route("StaffGroupShiftListMonthwise")]
        //public ExpandoObject StaffGroupShiftListMonthwise(int Month, int Year)
        //{
        //    dynamic ResponseMessage = new ExpandoObject();
        //    try
        //    {
        //        SchoolDataContext dataContext = new SchoolDataContext();
        //        DateTime newDate = new DateTime(Year, Month, 1);
        //        int DaysInMonth = DateTime.DaysInMonth(Year, Month);

        //        List<DateTime> DaysList = new List<DateTime>();
        //        for (var i = 1; i <= DaysInMonth; i++)
        //        {
        //            DaysList.Add(newDate);
        //            newDate = newDate.AddDays(1);
        //        }
        //        var list = (from e1 in dataContext.StaffGroups
        //                    orderby e1.StaffGroupId
        //                    select new AllStaffGroupShiftModel
        //                    {
        //                        StaffGroupId = e1.StaffGroupId,
        //                        StaffGroupName = e1.StaffGroupName,
        //                        GatepassId = e1.GatepassId,
        //                        WeekOffDay = e1.WeekOffDay,
        //                    }).ToList();

        //        list.ForEach(x => x.SubStaffGroupShiftList = DaysList.Select(d1 => new SubStaffGroupShiftModel
        //        {
        //            ShiftDate = d1,
        //            ShiftDay = (byte)d1.DayOfWeek + 1,
        //            StaffGroupShift = LoadData.GetStaffGroupShift(dataContext, d1, x.StaffGroupId)
        //        }).ToList());

        //        ResponseMessage.DayList = DaysList.Select(x => new { Date = x }).ToList();
        //        ResponseMessage.StaffGroupShiftList = list;
        //        ResponseMessage.Message = ConstantData.SuccessMessage;
        //    }
        //    catch (Exception ex)
        //    {
        //        ResponseMessage.Message = ex.Message;
        //    }
        //    return ResponseMessage;
        //}

        [HttpGet]
        [Route("StaffGroupShiftYearList")]
        public ExpandoObject StaffGroupShiftYearList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = dataContext.StaffGroupShifts.GroupBy(x => x.ShiftDate.Year).Select(x => new { Key = x.First().ShiftDate.Year, Value = x.First().ShiftDate.Year }).OrderBy(x => x.Key);
                ResponseMessage.YearList = list.ToList();
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
