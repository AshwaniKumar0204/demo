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
    [RoutePrefix("api/Rack")]
    public class RackController : ApiController
    {
        [HttpGet]
        [Route("RackList")]
        public ExpandoObject RackList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.RackList = LibraryData.RackList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveRack")]
        public ExpandoObject SaveRack(RackModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Rack Rack = new Rack();
                if (model.RackId > 0)
                    Rack = dataContext.Racks.Where(x => x.RackId == model.RackId).First();
                Rack.RackName = model.RackName;
                Rack.RackStatus = model.RackStatus;
                if (Rack.RackId == 0)
                    dataContext.Racks.InsertOnSubmit(Rack);
                dataContext.SubmitChanges();
                ResponseMessage.RackList = LibraryData.RackList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Rack is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteRack")]
        public ExpandoObject DeleteRack(int RackId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Rack = dataContext.Racks.Where(x => x.RackId == RackId).First();
                dataContext.Racks.DeleteOnSubmit(Rack);
                dataContext.SubmitChanges();
                ResponseMessage.RackList = LibraryData.RackList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Rack is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
