using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.WebHost;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using School;

namespace SchoolERP.Models
{
    public class SessionUtilies
    {
        public static int GetSchoolId(SchoolDataContext dataContext)
        {
            int SchoolId = dataContext.SystemLogins.Where(x => x.LoginId == SessionUtilies.GetLoginId()).First().SchoolId;
            return SchoolId;
        }
        public static void CreateLoginId(int LoginId)
        {
            //HttpContext.Current.Session[SessionVariables.LoginId] = LoginId;
            HttpCookie cCookie = new HttpCookie(CookieVariables.LoginInfo);
            cCookie.Values[CookieVariables.LoginId] = LoginId.ToString();
            cCookie.Expires = DateTime.Now.AddMonths(1);
            HttpContext.Current.Response.Cookies.Add(cCookie);

        }
        public static void LogoutUser()
        {
            //HttpContext.Current.Session[SessionVariables.LoginId] = null;
            HttpCookie cCookie = HttpContext.Current.Request.Cookies[CookieVariables.LoginInfo];
            cCookie.Expires = DateTime.Now.AddDays(-1d);
            HttpContext.Current.Response.Cookies[CookieVariables.LoginId].Expires = DateTime.Now.AddYears(1);
            HttpContext.Current.Request.Cookies[CookieVariables.LoginInfo][CookieVariables.LoginId] = null;
            HttpContext.Current.Response.Cookies.Add(cCookie);
        }
        public static int GetLoginId()
        {
            int LoginId = 0;
            if (HttpContext.Current.Request.Cookies[CookieVariables.LoginInfo] != null)
                LoginId = Convert.ToInt32(HttpContext.Current.Request.Cookies[CookieVariables.LoginInfo][CookieVariables.LoginId]);
            //if (HttpContext.Current.Session[SessionVariables.LoginId] != null)
            //    LoginId = Convert.ToInt32(HttpContext.Current.Session[SessionVariables.LoginId]);
            return LoginId;
        }
        public static int GetTeacherId(SchoolDataContext dataContext)
        {
            var teachers = from m1 in dataContext.SystemLogins
                          join m2 in dataContext.Employees on m1.EmployeeId equals m2.EmployeeId
                          join m3 in dataContext.Teachers on m2.StaffId equals m3.StaffId
                          where m1.LoginId == GetLoginId()
                          select m3;
            if (teachers.Any())
                return teachers.First().TeacherId;
            else
                return 0;
        }
    }
    public class SessionControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public SessionControllerHandler(RouteData routeData) : base(routeData)
        { }
    }

    public class SessionHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new SessionControllerHandler(requestContext.RouteData);
        }
    }
    
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class CheckSessionTimeOutAttribute : ActionFilterAttribute
    {

        List<String> ignoredUrls = new List<string>();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ignoredUrls.Add("index");
            ignoredUrls.Add("layoutadmin");
            ignoredUrls.Add("RegistrationOnlineCount");
            ignoredUrls.Add("PrintBonafideCertificate");
            ignoredUrls.Add("TeacherFeedback");
            ignoredUrls.Add("PrintMarksheetRetake");
            ignoredUrls.Add("SalarySlip");
            ignoredUrls.Add("PrintBookBarCodeWithAccessionNo");
            ignoredUrls.Add("PrintFeeDueReport4");
            ignoredUrls.Add("PrintBarCodeAdmissionNos");
            ignoredUrls.Add("SetSessionForPrintBarcode");
            ignoredUrls.Add("PrintPurchaseInvoiceStore");
            ignoredUrls.Add("PrintAdmintCardForRO");
            ignoredUrls.Add("PrintVehicleServiceReport");
            ignoredUrls.Add("PrintCaseFeeReceipt");
            ignoredUrls.Add("PrintAdmitCard");
            ignoredUrls.Add("PrintMiscellaneousFeeReceipt");
            ignoredUrls.Add("PrintFeeCollectionReport");
            ignoredUrls.Add("PrintUserDCR");
            ignoredUrls.Add("PrintCashbook");
            ignoredUrls.Add("PrintChequebook");
            ignoredUrls.Add("PrintRegistrationReceipt");
            ignoredUrls.Add("ExcelAdmissionList");
            ignoredUrls.Add("PrintFeeDueReport");
            ignoredUrls.Add("PrintCharacterCertificate");
            ignoredUrls.Add("PrintAllCharacterCertificate");
            ignoredUrls.Add("PrintTransferCertificate");
            ignoredUrls.Add("PrintAllTransferCertificate");
            ignoredUrls.Add("PrintFeeDueReport2");
            ignoredUrls.Add("PrintMarksheetEleven");
            ignoredUrls.Add("PrintMarksheetUptoEightAN");
            ignoredUrls.Add("PrintMarksheetUptoEightHY");
            ignoredUrls.Add("PrintAdmissionList");
            ignoredUrls.Add("PrintMarksheetNineAndTenHY");
            ignoredUrls.Add("PrintMarksheetTwelve");
            ignoredUrls.Add("PrintMarksheetHY");
            ignoredUrls.Add("CreateExcel");
            ignoredUrls.Add("ExcelMarksheetEntry");
            ignoredUrls.Add("ExcelForGradeMarks");
            ignoredUrls.Add("ExcelForAttencenceEntry");
            ignoredUrls.Add("PrintRankerList");
            ignoredUrls.Add("PrintRankerAnalysisList");
            ignoredUrls.Add("PrintCorssReport");
            ignoredUrls.Add("PrintCorssReportExamTypeWise");
            ignoredUrls.Add("PrintMarksAnalysis");
            ignoredUrls.Add("PrintRankerListTermwise");
            ignoredUrls.Add("TransportReportForAllStudent");
            ignoredUrls.Add("TrasnportReportGroupWise");
            ignoredUrls.Add("TransportReportBusWise");
            ignoredUrls.Add("PrintRegistrationFeeReceiptOnline");
            ignoredUrls.Add("PrintCorssReportClassEleven");
            ignoredUrls.Add("PrintBookBarCode");
            ignoredUrls.Add("PrintVehicleFuelConsumptionReport");
            ignoredUrls.Add("AttendanceReportShiftwise");
            ignoredUrls.Add("ManageDocumentUpload");
            string actionName = (String)filterContext.RouteData.Values["action"];
            var context = filterContext.HttpContext;
            if (!LoadData.IsUserExist(SessionUtilies.GetLoginId()))
            {
                string redirectTo = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Site/InvalidUrl";
                filterContext.HttpContext.Response.Redirect(redirectTo, true);
            }
            else if (!ignoredUrls.Contains(actionName))// != "Index" && actionName != "ChangePassword")
            {
                if (!SchoolData.CheckMenuAction(actionName))
                {
                    string redirectTo = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Site/InvalidUrl";
                    filterContext.HttpContext.Response.Redirect(redirectTo, true);
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}