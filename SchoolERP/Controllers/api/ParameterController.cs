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
    [RoutePrefix("api/Parameter")]
    public class ParameterController : ApiController
    {
        [HttpPost]
        [Route("ParameterList")]
        public ExpandoObject ParameterList(Parameter model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.Parameters
                           where (model.ParameterStatus == 0 ? true : b1.ParameterStatus == model.ParameterStatus)
                           && (model.ParentParameterId == 0 ? true : b1.ParentParameterId == model.ParentParameterId)
                           select new
                           {
                               b1.ParameterId,
                               b1.ParameterStatus,
                               b1.ParameterName,
                           };
                ResponseMessage.ParameterList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteParameter")]
        public ExpandoObject DeleteParameter(int ParameterId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Parameter = dataContext.Parameters.Where(x => x.ParameterId == ParameterId).First();
                dataContext.Parameters.DeleteOnSubmit(Parameter);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Parameter is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
