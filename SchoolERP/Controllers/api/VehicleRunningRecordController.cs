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
    [RoutePrefix("api/VehicleRunningRecord")]
    public class VehicleRunningRecordController : ApiController
    {
        [HttpPost]
        [Route("VehicleRunningRecordList")]
        public ExpandoObject VehicleRunningRecordList(DateFilterModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (model.FromDate != null)
                    model.FromDate = model.FromDate.Value.Date;
                if (model.ToDate != null)
                    model.ToDate = model.ToDate.Value.Date;
                var list = from b1 in dataContext.VehicleRunningRecords
                           where (model.BusId == 0 ? true : b1.BusId == model.BusId)
                           && (model.FromDate == null ? true :b1.EntryDate.Date >= model.FromDate)
                           && (model.ToDate == null ? true :b1.EntryDate.Date <= model.ToDate)
                           orderby b1.EntryDate descending
                           select new
                           {
                               b1.VehicleRunningRecordId,
                               b1.Bus.BusTitle,
                               b1.BusId,
                               b1.EntryDate,
                               b1.MeterReading
                           };
                ResponseMessage.VehicleRunningRecordList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveVehicleRunningRecord")]
        public ExpandoObject SaveVehicleRunningRecord(VehicleRunningRecord model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                VehicleRunningRecord VehicleRunningRecord = null;
                if (model.VehicleRunningRecordId > 0)
                    VehicleRunningRecord = dataContext.VehicleRunningRecords.Where(x => x.VehicleRunningRecordId == model.VehicleRunningRecordId).First();
                else
                    VehicleRunningRecord = new VehicleRunningRecord();
                VehicleRunningRecord.MeterReading = model.MeterReading;
                VehicleRunningRecord.BusId = model.BusId;
                VehicleRunningRecord.EntryDate = model.EntryDate;
                VehicleRunningRecord.LastUpdatedOn = DateTime.Now;
                VehicleRunningRecord.LastUpdatedBy = SessionUtilies.GetLoginId();
                if (VehicleRunningRecord.VehicleRunningRecordId == 0)
                    dataContext.VehicleRunningRecords.InsertOnSubmit(VehicleRunningRecord);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This record is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteVehicleRunningRecord")]
        public ExpandoObject DeleteVehicleRunningRecord(int VehicleRunningRecordId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var VehicleRunningRecord = dataContext.VehicleRunningRecords.Where(x => x.VehicleRunningRecordId == VehicleRunningRecordId).First();
                dataContext.VehicleRunningRecords.DeleteOnSubmit(VehicleRunningRecord);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This record is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
