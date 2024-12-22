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
    [RoutePrefix("api/Designation")]
    public class DesignationController : ApiController
    {
        [HttpGet]
        [Route("DesignationList")]
        public ExpandoObject DesignationList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.DesignationList = StoreData.DesignationList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveDesignation")]
        public ExpandoObject SaveDesignation(DesignationModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Designation Designation = new Designation();
                if (model.DesignationId > 0)
                    Designation = dataContext.Designations.Where(x => x.DesignationId == model.DesignationId).First();
                Designation.DesignationName = model.DesignationName;
                Designation.DesignationStatus = model.DesignationStatus;
                if (Designation.DesignationId == 0)
                    dataContext.Designations.InsertOnSubmit(Designation);
                dataContext.SubmitChanges();
                ResponseMessage.DesignationList = StoreData.DesignationList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Designation is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteDesignation")]
        public ExpandoObject DeleteDesignation(int DesignationId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Designation = dataContext.Designations.Where(x => x.DesignationId == DesignationId).First();
                dataContext.Designations.DeleteOnSubmit(Designation);
                dataContext.SubmitChanges();
                ResponseMessage.DesignationList = StoreData.DesignationList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Designation is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
