
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
    [RoutePrefix("api/city")]
    public class CityController : ApiController
    {
        [HttpGet]
        [Route("CityList")]
        public ExpandoObject CityList(byte CityStatus)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                response.CityList = LoadData.GetCityList(dataContext, CityStatus);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveCity")]
        public ExpandoObject SaveCity(CityModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                City City = new City();
                if (model.CityId > 0)
                    City = dataContext.Cities.Where(x => x.CityId == model.CityId).First();
                City.CityId = model.CityId;
                City.StateId = model.StateId;
                City.CityName = model.CityName;
                City.CityStatus = model.CityStatus;
                if (City.CityId == 0)
                    dataContext.Cities.InsertOnSubmit(City);
                dataContext.SubmitChanges();
                ResponseMessage.CityList = LoadData.GetCityList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {

                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteCity")]
        public ExpandoObject DeleteCity(int CityId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var City = dataContext.Cities.Where(x => x.CityId == CityId).First();
                int StateId = City.StateId;
                byte? CityStatus = City.CityStatus;
                dataContext.Cities.DeleteOnSubmit(City);
                dataContext.SubmitChanges();
                ResponseMessage.CityList = LoadData.GetCityList(dataContext, 0);
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
