using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/PaymentLink")]
    public class PaymentLinkTwoController : ApiController
    {
        [HttpGet]
        [Route("FeeListForLinkTwo")]
        public ExpandoObject FeeListForLinkTwo(int AcademicSessionId, int ClassNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int AdmissionFeeHeadId = Convert.ToInt32(ConfigurationManager.AppSettings["AdmissionFeeHeadId"]);
                var AdmissionFeeList = LoadData.AdmissionFeeForGenerateLink(dataContext, ClassNo, AcademicSessionId, AdmissionFeeHeadId);
                response.AdmissionFeeList = AdmissionFeeList;
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
