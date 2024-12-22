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
    [RoutePrefix("api/TaskMaster")]
    public class TaskMasterController : ApiController
    {
        [HttpGet]
        [Route("TaskMasterList")]
        public ExpandoObject TaskMasterList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.TaskMasterList = LoadData.GetTaskMasterList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveTaskMaster")]
        public ExpandoObject SaveTaskMaster(TaskMasterModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                TaskMaster TaskMaster = new TaskMaster();
                if (model.TaskMasterId > 0)
                    TaskMaster = dataContext.TaskMasters.Where(x => x.TaskMasterId == model.TaskMasterId).First();
                TaskMaster.TaskGroupId = model.TaskGroupId;
                TaskMaster.ControllerName = model.ControllerName;
                TaskMaster.ActionName = model.ActionName;
                TaskMaster.TaskMasterStatus = model.TaskMasterStatus;
                if (TaskMaster.TaskMasterId == 0)
                    dataContext.TaskMasters.InsertOnSubmit(TaskMaster);
                dataContext.SubmitChanges();
                ResponseMessage.TaskMasterList = LoadData.GetTaskMasterList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Task is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteTaskMaster")]
        public ExpandoObject DeleteTaskMaster(int TaskMasterId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var TaskMaster = dataContext.TaskMasters.Where(x => x.TaskMasterId == TaskMasterId).First();
                dataContext.TaskMasters.DeleteOnSubmit(TaskMaster);
                dataContext.SubmitChanges();
                ResponseMessage.TaskMasterList = LoadData.GetTaskMasterList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Task is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
