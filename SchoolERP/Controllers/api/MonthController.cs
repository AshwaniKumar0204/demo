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
    [RoutePrefix("api/month")]
    public class MonthController : ApiController
    {
        [HttpGet]
        [Route("monthlist")]
        public ExpandoObject MonthList(int ClassId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Months
                            join c1 in dataContext.Classes on a1.ClassId equals c1.ClassId
                            where (ClassId == 0 || a1.ClassId == ClassId)
                            orderby c1.ClassId
                            select new
                            {
                                a1.MonthName,
                                a1.MonthNo,
                                a1.ClassId,
                                c1.ClassName
                            });

                response.MonthList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveMonth")]
        public ExpandoObject SaveMonth(Month model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Month Month = new Month();
                if (model.MonthNo > 0)
                    Month = dataContext.Months.Where(x => x.MonthNo == model.MonthNo).First();
                Month.MonthName = model.MonthName;
                Month.ClassId = model.ClassId;
                if (Month.MonthNo == 0)
                    dataContext.Months.InsertOnSubmit(Month);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Month is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteMonth")]
        public ExpandoObject DeleteMonth(int MonthNo)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Month = dataContext.Months.Where(x => x.MonthNo == MonthNo).First();
                dataContext.Months.DeleteOnSubmit(Month);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Month is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
