using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/Session")]
    public class SessionController : ApiController
    {
        [HttpPost]
        [Route("SetSessionForPrintBarcode")]
        public ExpandoObject SetSessionForPrintBarcode(List<BarCodeModel> list)
        {
            dynamic response = new ExpandoObject();
            try
            {
                HttpContext.Current.Session["BarCodeAccessionNos"] = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        
    }
}
