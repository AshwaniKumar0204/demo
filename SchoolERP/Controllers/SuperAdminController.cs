using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolERP.Controllers
{
    [CheckSessionTimeOutAttribute]
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        public ActionResult LayoutSuperAdmin()
        {
            return View();
        }
        public ActionResult FeeDueReport()
        {
            return View();
        }
        public ActionResult ManageTaskGroup()
        {
            return View();
        }
        public ActionResult ManageMenu()
        {
            return View();
        }
        public ActionResult ManageRoleTask()
        {
            return View();
        }
        public ActionResult ManageRoleSection()
        {
            return View();
        }
        public ActionResult ManageRole()
        {
            return View();
        }
        public ActionResult ManageTaskMaster()
        {
            return View();
        }
        public ActionResult ManageEmployee()
        {
            return View();
        }
        public ActionResult ManageSysytemLogin()
        {
            return View();
        }
        public ActionResult ManageSchool()
        {
            return View();
        }
        public ActionResult EmployeeType()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
    }
}