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
    [RoutePrefix("api/VehicleType")]
    public class VehicleTypeController : ApiController
    {
        [HttpGet]
        [Route("VehicleTypeList")]
        public ExpandoObject VehicleTypeList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.VehicleTypes
                           where (Status == 0 ? true : b1.VehicleTypeStatus == Status)
                           select new
                           {
                               b1.VehicleTypeId,
                               b1.VehicleTypeStatus,
                               b1.VehicleTypeName,
                               b1.ServiceDuration
                           };
                ResponseMessage.VehicleTypeList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveVehicleType")]
        public ExpandoObject SaveVehicleType(VehicleType model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                VehicleType VehicleType = new VehicleType();
                if (model.VehicleTypeId > 0)
                    VehicleType = dataContext.VehicleTypes.Where(x => x.VehicleTypeId == model.VehicleTypeId).First();
                VehicleType.VehicleTypeName = model.VehicleTypeName;
                VehicleType.ServiceDuration = model.ServiceDuration;
                VehicleType.VehicleTypeStatus = model.VehicleTypeStatus;
                if (VehicleType.VehicleTypeId == 0)
                    dataContext.VehicleTypes.InsertOnSubmit(VehicleType);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Vehicle Type is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteVehicleType")]
        public ExpandoObject DeleteVehicleType(int VehicleTypeId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var VehicleType = dataContext.VehicleTypes.Where(x => x.VehicleTypeId == VehicleTypeId).First();
                dataContext.VehicleTypes.DeleteOnSubmit(VehicleType);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Vehicle Type is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
