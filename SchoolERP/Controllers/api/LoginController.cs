using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Dynamic;
using School;
using SchoolERP.Models;

namespace SchoolERP.Controllers.api
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("systemLogin")]
        public ExpandoObject SystemLogin(string UserId, string Password)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            LoginLog loginLog = new LoginLog();
            try
            {
                loginLog.IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (HttpContext.Current.Request.Browser.IsMobileDevice)
                    loginLog.LoginDevice = "Mobile";
                else
                    loginLog.LoginDevice = "Computer";
                if (HttpContext.Current.Request.UrlReferrer != null)
                    loginLog.ReferrerUrl = HttpContext.Current.Request.UrlReferrer.ToString();

                HttpBrowserCapabilities bcr = HttpContext.Current.Request.Browser;
                loginLog.UserName = UserId;
                loginLog.LoginPassword = CryptoEngine.Encrypt(Password);
                loginLog.ClientBrowser = bcr.Browser;
                loginLog.LoginTime = DateTime.Now;
                loginLog.LastUpdatedOn = DateTime.Now;
                loginLog.LoginResult = (byte)LoginResult.Error;

                int countEmployee = (from e1 in dataContext.SystemLogins
                                     where (e1.UserName == UserId || e1.LoginName == UserId)
                                     select e1).Count();
                if (countEmployee <= 0)
                {
                    loginLog.LoginResult = (byte)LoginResult.WrongUserName;
                    throw new Exception("Invalid User Id or Password");
                }
                var systemLogin = (from e1 in dataContext.SystemLogins
                                   where (e1.UserName == UserId || e1.LoginName == UserId)
                                   select e1).First();

                if (systemLogin.LoginStatus == (byte)LoginStatus.Inactive || systemLogin.Employee.EmployeeStatus == (byte)EmployeeStatus.Inactive)
                {
                    loginLog.LoginResult = (byte)LoginResult.AccountNotActive;
                    throw new Exception("Invalid User Id or Password");
                }
                if (systemLogin.LoginPassword != CryptoEngine.Encrypt(Password))
                {
                    loginLog.LoginResult = (byte)LoginResult.WrongPassword;
                    throw new Exception("Invalid User Id or Password");
                }

                SessionUtilies.CreateLoginId(systemLogin.LoginId);
                loginLog.LoginResult = (byte)LoginResult.Successful;

                response.UserDetail = LoadData.GetSystemLoginDetail(systemLogin.LoginId, dataContext);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                dataContext.LoginLogs.InsertOnSubmit(loginLog);
                dataContext.SubmitChanges();
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
