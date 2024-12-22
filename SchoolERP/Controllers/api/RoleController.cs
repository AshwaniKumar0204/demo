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
    [RoutePrefix("api/Role")]
    public class RoleController : ApiController
    {
        [HttpGet]
        [Route("RoleList")]
        public ExpandoObject RoleList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.RoleList = LoadData.GetRoleList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveRole")]
        public ExpandoObject SaveRole(RoleModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Role Role = new Role();
                if (model.RoleId > 0)
                    Role = dataContext.Roles.Where(x => x.RoleId == model.RoleId).First();
                Role.SchoolId = model.SchoolId;
                Role.RoleTitle = model.RoleTitle;
                Role.RoleStatus = model.RoleStatus;
                Role.LastUpdatedBy = SessionUtilies.GetLoginId();
                Role.LastUpdatedOn = DateTime.Now;
                if (Role.RoleId == 0)
                    dataContext.Roles.InsertOnSubmit(Role);
                dataContext.SubmitChanges();
                ResponseMessage.RoleList = LoadData.GetRoleList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Role is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteRole")]
        public ExpandoObject DeleteRole(int RoleId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Role = dataContext.Roles.Where(x => x.RoleId == RoleId).First();
                dataContext.Roles.DeleteOnSubmit(Role);
                dataContext.SubmitChanges();
                ResponseMessage.RoleList = LoadData.GetRoleList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Role is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
