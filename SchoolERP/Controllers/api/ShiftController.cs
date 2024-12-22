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
    [RoutePrefix("api/shift")]
    public class ShiftController : ApiController
    {
        [HttpGet]
        [Route("ShiftList")]
        public ExpandoObject ShiftList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from s1 in dataContext.Shifts
                            orderby s1.ShiftId
                            select new
                            {
                                InTime = s1.InTime,
                                OutTime = s1.OutTime,
                                ShiftDuration = s1.ShiftDuration,
                                ShiftDurationString = LoadData.DurationString(s1.ShiftDuration),
                                ShiftId = s1.ShiftId,
                                ShiftName = s1.ShiftName,
                                s1.GapeTime,
                                s1.Status,
                            }).ToList();
                ResponseMessage.ShiftList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveShift")]
        public ExpandoObject SaveShift(Shift model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Shift Shift = new Shift();
                if (model.ShiftId > 0)
                    Shift = dataContext.Shifts.Where(x => x.ShiftId == model.ShiftId).First();
                Shift.ShiftName = model.ShiftName;
                Shift.InTime = model.InTime;
                Shift.OutTime = model.OutTime;
                Shift.ShiftDuration = model.ShiftDuration;
                Shift.GapeTime = model.GapeTime;
                Shift.Status = model.Status;
                Shift.LastUpdatedBy = SessionUtilies.GetLoginId();
                Shift.LastUpdatedOn = DateTime.Now;
                if (Shift.ShiftId == 0)
                    dataContext.Shifts.InsertOnSubmit(Shift);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Shift is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteShift")]
        public ExpandoObject DeleteShift(int ShiftId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Shift = dataContext.Shifts.Where(x => x.ShiftId == ShiftId).First();
                dataContext.Shifts.DeleteOnSubmit(Shift);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Shift is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
