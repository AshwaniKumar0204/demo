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
    [RoutePrefix("api/state")]
    public class StateController : ApiController
    {
        [HttpGet]
        [Route("StateList")]
        public ExpandoObject StateList(byte StateStatus)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                response.StateList = LoadData.GetStateList(dataContext, StateStatus);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveState")]
        public ExpandoObject SaveState(StateModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                State State = new State();
                if (model.StateId > 0)
                    State = dataContext.States.Where(x => x.StateId == model.StateId).First();
                State.StateId = model.StateId;
                State.StateName = model.StateName;
                State.StateStatus = model.StateStatus;
                if (State.StateId == 0)
                    dataContext.States.InsertOnSubmit(State);
                dataContext.SubmitChanges();
                ResponseMessage.StateList = LoadData.GetStateList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {

                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteState")]
        public ExpandoObject DeleteState(int StateId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var State = dataContext.States.Where(x => x.StateId == StateId).First();
                dataContext.States.DeleteOnSubmit(State);
                dataContext.SubmitChanges();
                ResponseMessage.StateList = LoadData.GetStateList(dataContext, 0);
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
