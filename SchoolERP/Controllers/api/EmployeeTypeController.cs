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
    [RoutePrefix("api/EmployeeType")]
    public class EmployeeTypeController : ApiController
    {
        [HttpGet]
        [Route("EmployeeTypeList")]
        public ExpandoObject EmployeeTypeList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.EmployeeTypeList = LoadData.GetEmployeeTypeList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveEmployeeType")]
        public ExpandoObject SaveEmployeeType(EmployeeTypeModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                EmployeeType EmployeeType = new EmployeeType();
                if (model.EmployeeTypeId > 0)
                    EmployeeType = dataContext.EmployeeTypes.Where(x => x.EmployeeTypeId == model.EmployeeTypeId).First();
                EmployeeType.EmployeeTypeStatus = model.EmployeeTypeStatus;
                EmployeeType.LastUpdatedBy = SessionUtilies.GetLoginId();
                EmployeeType.LastUpdatedOn = DateTime.Now;
                EmployeeType.TypeName = model.TypeName;
                EmployeeType.SchoolId = model.SchoolId;
                if (EmployeeType.EmployeeTypeId == 0)
                    dataContext.EmployeeTypes.InsertOnSubmit(EmployeeType);
                dataContext.SubmitChanges();
                ResponseMessage.EmployeeTypeList = LoadData.GetEmployeeTypeList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Employee Type is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteEmployeeType")]
        public ExpandoObject DeleteEmployeeType(int EmployeeTypeId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var EmployeeType = dataContext.EmployeeTypes.Where(x => x.EmployeeTypeId == EmployeeTypeId).First();
                dataContext.EmployeeTypes.DeleteOnSubmit(EmployeeType);
                dataContext.SubmitChanges();
                ResponseMessage.EmployeeTypeList = LoadData.GetEmployeeTypeList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Employee Type is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
