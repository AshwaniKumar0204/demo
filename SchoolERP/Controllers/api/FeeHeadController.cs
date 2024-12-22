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
    [RoutePrefix("api/feeHead")]
    public class FeeHeadController : ApiController
    {
        [HttpGet]
        [Route("FeeHeadList")]
        public ExpandoObject FeeHeadList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var FeeHeadList = (from f1 in dataContext.FeeHeads
                                   orderby f1.FeeHeadTitle
                                   select new FeeHeadModel
                                   {
                                       FeeHeadId = f1.FeeHeadId,
                                       FeeHeadTitle = f1.FeeHeadTitle,
                                       IsSelected = true
                                   });

                response.FeeHeadList = FeeHeadList.ToList();
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
