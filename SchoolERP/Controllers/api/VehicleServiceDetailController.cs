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

namespace SchoolERP.Controllers
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/VehicleServiceDetail")]
    public class VehicleServiceDetailController : ApiController
    {
        [HttpGet]
        [Route("VehicleServiceDetailList")]
        public ExpandoObject VehicleServiceDetailList(int VehicleServiceId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.VehicleServiceDetails
                           where p1.VehicleServiceId == VehicleServiceId
                           select new
                           {
                               p1.JobType,
                               p1.Remarks,
                               p1.UnitCost,
                               p1.Quantity,
                               p1.Amount,
                               p1.VehicleServiceDetailId,
                               p1.VehicleServiceId
                           };

                response.VehicleServiceDetailList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }


        [HttpGet]
        [Route("deleteVehicleServiceDetail")]
        public ExpandoObject DeleteVehicleServiceDetail(int VehicleServiceDetailId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var VehicleServiceDetail = dataContext.VehicleServiceDetails.Where(x => x.VehicleServiceDetailId == VehicleServiceDetailId).First();
                dataContext.VehicleServiceDetails.DeleteOnSubmit(VehicleServiceDetail);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Book Type is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("JobTypeList")]
        public ExpandoObject JobTypeList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from p1 in dataContext.VehicleServiceDetails
                            orderby p1.JobType
                            select new { p1.JobType }).Distinct();

                response.JobTypeList = list.ToList();
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
