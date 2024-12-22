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
    [RoutePrefix("api/House")]
    public class HouseController : ApiController
    {
        [HttpGet]
        [Route("HouseList")]
        public ExpandoObject HouseList(int HouseStatus)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from st1 in dataContext.Houses
                            where st1.HouseStatus == HouseStatus
                            orderby st1.HouseId
                            select new
                            {
                                st1.HouseId,
                                st1.HouseName,
                                st1.Description,
                                st1.HouseStatus
                            }).ToList();
                response.HouseList = list;
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
