using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using School;

namespace SchoolERP.Controllers
{
    public class SiteController : Controller
    {
        // GET: Site
        public ActionResult SystemLogin()
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var school = dataContext.Schools.Where(x => x.SchoolStatus == (byte)SchoolStatus.Active).OrderByDescending(x => x.SchoolId).First();
                ViewBag.SchoolDetail = school;
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintA4Portrait()
        {
            return View();
        }
        public ActionResult PrintA4Landscape()
        {
            return View();
        }
        public ActionResult InvalidUrl()
        {
            return View();
        }
    }
}