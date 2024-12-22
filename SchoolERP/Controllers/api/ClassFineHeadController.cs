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
    [RoutePrefix("api/ClassFineHead")]
    public class ClassFineHeadController : ApiController
    {
        [HttpGet]
        [Route("ClassFineHeadList")]
        public ExpandoObject ClassFineHeadList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from cf1 in dataContext.ClassFineHeads
                            join fh1 in dataContext.FeeHeads
                            on cf1.FeeHeadId equals fh1.FeeHeadId
                            orderby fh1.FeeHeadTitle
                            select new
                            {
                                fh1.FeeHeadId,
                                fh1.FeeHeadTitle
                            });
                response.ClassFineHeadList = list.ToList();
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
