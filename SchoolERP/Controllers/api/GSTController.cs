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
    [RoutePrefix("api/GST")]
    public class GSTController : ApiController
    {
        [HttpGet]
        [Route("GSTList")]
        public ExpandoObject GSTList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from g1 in dataContext.GSTs
                           where g1.GSTStatus == 1
                           orderby g1.GSTPercentage
                           select new
                           {
                               g1.GSTPercentage,
                               g1.GSTTitle,
                               g1.GSTId,
                               g1.GSTStatus
                           };
                ResponseMessage.GSTList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
