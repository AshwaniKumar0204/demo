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
    [RoutePrefix("api/Device")]
    public class DeviceController : ApiController
    {
        [HttpGet]
        [Route("DeviceList")]
        public ExpandoObject DeviceList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.Devices
                           where (Status == 0 ? true : b1.DeviceStatus == Status)
                           select new
                           {
                               DeviceId = b1.DeviceId,
                               DeviceStatus = b1.DeviceStatus,
                               DeviceName = b1.DeviceName,
                               b1.DeviceNo,
                               b1.DeviceSerailNo,
                               b1.DeviceType,
                               DeviceTypeName = Enum.GetName(typeof(DeviceType), b1.DeviceType)
                           };
                ResponseMessage.DeviceList = list.ToList(); ;
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveDevice")]
        public ExpandoObject SaveDevice(Device model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Device Device = new Device();
                if (model.DeviceId > 0)
                    Device = dataContext.Devices.Where(x => x.DeviceId == model.DeviceId).First();
                Device.DeviceName = model.DeviceName;
                Device.DeviceStatus = model.DeviceStatus;
                Device.DeviceNo = model.DeviceNo;
                Device.DeviceSerailNo = model.DeviceSerailNo;
                Device.DeviceStatus = model.DeviceStatus;
                Device.DeviceType = model.DeviceType;
                Device.LastUpdatedBy = SessionUtilies.GetLoginId();
                Device.LastUpdatedOn = DateTime.Now;
                if (Device.DeviceId == 0)
                    dataContext.Devices.InsertOnSubmit(Device);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Device is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteDevice")]
        public ExpandoObject DeleteDevice(int DeviceId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Device = dataContext.Devices.Where(x => x.DeviceId == DeviceId).First();
                dataContext.Devices.DeleteOnSubmit(Device);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Device is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
