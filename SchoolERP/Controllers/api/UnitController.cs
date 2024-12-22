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
    [RoutePrefix("api/Unit")]
    public class UnitController : ApiController
    {
        [HttpGet]
        [Route("UnitList")]
        public ExpandoObject UnitList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.UnitList = StoreData.UnitList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveUnit")]
        public ExpandoObject SaveUnit(UnitModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Unit Unit = new Unit();
                if (model.UnitId > 0)
                    Unit = dataContext.Units.Where(x => x.UnitId == model.UnitId).First();
                Unit.UnitName = model.UnitName;
                Unit.UnitStatus = model.UnitStatus;
                if (Unit.UnitId == 0)
                    dataContext.Units.InsertOnSubmit(Unit);
                dataContext.SubmitChanges();
                ResponseMessage.UnitList = StoreData.UnitList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Unit is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteUnit")]
        public ExpandoObject DeleteUnit(int UnitId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Unit = dataContext.Units.Where(x => x.UnitId == UnitId).First();
                dataContext.Units.DeleteOnSubmit(Unit);
                dataContext.SubmitChanges();
                ResponseMessage.UnitList = StoreData.UnitList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Unit is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
