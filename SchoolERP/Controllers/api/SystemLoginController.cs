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
    [RoutePrefix("api/systemLogin")]
    public class SystemLoginController : ApiController
    {
        [HttpGet]
        [Route("SystemLoginList")]
        public ExpandoObject SystemLoginList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var SystemLoginList = (from s1 in dataContext.SystemLogins
                                       orderby s1.UserName
                                       select new
                                       {
                                           s1.LoginId,
                                           s1.LoginName,
                                           s1.UserName
                                       });

                response.SystemLoginList = SystemLoginList.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("Logout")]
        public ExpandoObject Logout()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SessionUtilies.LogoutUser();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("changePassword")]
        public ExpandoObject ChangePassword(ChangePasswordModel changePasswordModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var SystemLogin = dataContext.SystemLogins.Where(x => x.LoginId == SessionUtilies.GetLoginId()).First();
                if (CryptoEngine.Encrypt(changePasswordModel.OldPassword) == SystemLogin.LoginPassword)
                {
                    SystemLogin.LoginPassword = CryptoEngine.Encrypt(changePasswordModel.NewPassword);
                    dataContext.SubmitChanges();
                }
                else
                {
                    response.Message = "Current password is invalid!!";
                    return response;
                }
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("checkSysytemLogin")]
        public ExpandoObject CheckSysytemLogin()
        {
            dynamic response = new ExpandoObject();
            try
            {
                response.LoginId = SessionUtilies.GetLoginId();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        //Manage System Logins

        [HttpGet]
        [Route("SystemLoginList")]
        public ExpandoObject SystemLoginList(byte Status, int EmployeeId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from s1 in dataContext.SystemLogins
                           join e1 in dataContext.Employees
                           on s1.EmployeeId equals e1.EmployeeId
                           where (EmployeeId != 0 ? s1.EmployeeId == EmployeeId : true)
                           && (Status != 0 ? s1.LoginStatus == Status : true)
                           select new SystemLoginModel
                           {
                               UserName = s1.UserName,
                               LoginId = s1.LoginId,
                               LoginName = s1.LoginName,
                               LoginPassword = CryptoEngine.Decrypt(s1.LoginPassword),
                               LoginStatus = s1.LoginStatus,
                               SchoolId = s1.SchoolId,
                               EmployeeId = s1.EmployeeId,
                               RoleList = (from r1 in dataContext.Roles
                                           join r2 in dataContext.SystemLoginRoles.Where(x => x.LoginId == s1.LoginId)
                                           on r1.RoleId equals r2.RoleId into subLoginRoles
                                           from sLoginRoles in subLoginRoles.DefaultIfEmpty()
                                           select new RoleModel
                                           {
                                               IsSelected = sLoginRoles.LoginId != null ? true : false,
                                               RoleId = r1.RoleId,
                                               RoleTitle = r1.RoleTitle,
                                               SystemLoginRoleId = sLoginRoles.SystemLoginRoleId != null ? sLoginRoles.SystemLoginRoleId : 0
                                           }).ToList(),
                               Employee = new EmployeeModel { EmployeeName = e1.EmployeeName, EmployeeCode = e1.EmployeeCode, IsSuperAdmin = e1.IsSuperAdmin }
                           };
                ResponseMessage.SystemLoginList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveSystemLogin")]
        public ExpandoObject SaveSystemLogin(SystemLoginModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SystemLogin SystemLogin = new SystemLogin();
                if (model.LoginId > 0)
                    SystemLogin = dataContext.SystemLogins.Where(x => x.LoginId == model.LoginId).First();
                SystemLogin.EmployeeId = model.EmployeeId;
                SystemLogin.SchoolId = model.SchoolId;
                SystemLogin.UserName = model.UserName;
                SystemLogin.LoginPassword = CryptoEngine.Encrypt(model.LoginPassword);
                SystemLogin.LastModifiedOn = DateTime.Now;
                SystemLogin.LastUpdatedBy = SessionUtilies.GetLoginId();
                SystemLogin.LoginStatus = model.LoginStatus;

                if (SystemLogin.LoginId == 0)
                {
                    Random r = new Random();
                    SystemLogin.LoginName = "E" + r.Next(0000, 9999);
                    SystemLogin.CreatedBy = SessionUtilies.GetLoginId();
                    SystemLogin.CreatedOn = DateTime.Now;
                    dataContext.SystemLogins.InsertOnSubmit(SystemLogin);
                }
                dataContext.SubmitChanges();

                model.RoleList.ForEach(role =>
                {
                    if (!role.IsSelected && role.SystemLoginRoleId != 0)
                    {
                        var systemLogin = dataContext.SystemLoginRoles.Where(x => x.SystemLoginRoleId == role.SystemLoginRoleId).First();
                        dataContext.SystemLoginRoles.DeleteOnSubmit(systemLogin);
                        dataContext.SubmitChanges();
                    }
                    else if (role.IsSelected && role.SystemLoginRoleId == 0)
                    {
                        SystemLoginRole systemLoginRole = new SystemLoginRole
                        {
                            LastUpdatedBy = SessionUtilies.GetLoginId(),
                            LastUpdatedOn = DateTime.Now,
                            LoginId = SystemLogin.LoginId,
                            RoleId = role.RoleId,
                            SystemLoginRoleStatus = (byte)SystemLoginRoleStatus.Active
                        };
                        dataContext.SystemLoginRoles.InsertOnSubmit(systemLoginRole);
                        dataContext.SubmitChanges();
                    }
                });

                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This User Name is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteSystemLogin")]
        public ExpandoObject DeleteSystemLogin(int LoginId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var SystemLogin = dataContext.SystemLogins.Where(x => x.LoginId == LoginId).First();
                int EmployeeId = SystemLogin.EmployeeId.Value;
                dataContext.SystemLogins.DeleteOnSubmit(SystemLogin);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This SystemLogin is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
