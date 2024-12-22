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
    [RoutePrefix("api/TaskGroup")]
    public class TaskGroupController : ApiController
    {
        [HttpGet]
        [Route("TaskGroupList")]
        public ExpandoObject TaskGroupList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from t1 in dataContext.TaskGroups
                           select new
                           {
                               t1.TaskGroupId,
                               t1.GroupName
                           };
                ResponseMessage.TaskGroupList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveTaskGroup")]
        public ExpandoObject SaveTaskGroup(TaskGroupModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                TaskGroup TaskGroup = new TaskGroup();
                if (model.TaskGroupId > 0)
                    TaskGroup = dataContext.TaskGroups.Where(x => x.TaskGroupId == model.TaskGroupId).First();
                TaskGroup.GroupName = model.GroupName;
                if (TaskGroup.TaskGroupId == 0)
                    dataContext.TaskGroups.InsertOnSubmit(TaskGroup);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Task Group is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteTaskGroup")]
        public ExpandoObject DeleteTaskGroup(int TaskGroupId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var TaskGroup = dataContext.TaskGroups.Where(x => x.TaskGroupId == TaskGroupId).First();
                dataContext.TaskGroups.DeleteOnSubmit(TaskGroup);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Task Group is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

    }
}
