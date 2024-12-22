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
    [RoutePrefix("api/RoleTask")]
    public class RoleTaskTaskController : ApiController
    {
        [HttpGet]
        [Route("AllRoleTaskList")]
        public ExpandoObject AllRoleTaskList(byte Status, int RoleId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from tg1 in dataContext.TaskGroups
                           orderby tg1.GroupName
                           select new
                           {
                               tg1.GroupName,
                               tg1.TaskGroupId,
                               RoleTaskList = from tm1 in dataContext.TaskMasters
                                              join m1 in dataContext.Menus
                                              on tm1.TaskMasterId equals m1.TaskMasterId
                                              join r1 in dataContext.RoleTasks.Where(x=>x.RoleId == RoleId)
                                              on tm1.TaskMasterId equals r1.TaskMasterId into RoleTaskList
                                              from roleTask in RoleTaskList.DefaultIfEmpty()
                                              where tm1.TaskGroupId == tg1.TaskGroupId
                                              && (Status > 0 ? tm1.TaskMasterStatus == Status : true)
                                                && (tm1.TaskMasterStatus == 2 ? false : tm1.TaskMasterStatus != 2)

                                              select new RoleTaskModel
                                              {
                                                  IsSelected = roleTask.RoleId != null ? true : false,
                                                  TaskMasterId = tm1.TaskMasterId,
                                                  MenuTitle = m1.MenuTitle,
                                                  RoleTaskId = roleTask.RoleTaskId != null ? roleTask.RoleTaskId : 0,
                                                  CanEdit = roleTask.CanEdit != null ? roleTask.CanEdit : false,
                                                  CanDelete = roleTask.CanDelete != null ? roleTask.CanDelete : false
                                              }
                           };
                ResponseMessage.TaskGroupList = list.Where(x => x.RoleTaskList.Any()).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveRoleTask")]
        public ExpandoObject SaveRoleTask(RoleModel role)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                role.RoleTaskList.ForEach(model =>
                {
                    if (model.RoleTaskId > 0 && !model.IsSelected)
                    {
                        var RoleTask = dataContext.RoleTasks.Where(x => x.RoleTaskId == model.RoleTaskId).First();
                        dataContext.RoleTasks.DeleteOnSubmit(RoleTask);
                        dataContext.SubmitChanges();
                    }
                    else if (model.IsSelected)
                    {
                        RoleTask RoleTask = null;
                        if (model.RoleTaskId > 0)
                            RoleTask = dataContext.RoleTasks.Where(x => x.RoleTaskId == model.RoleTaskId).First();
                        else
                            RoleTask = new RoleTask
                            {
                                RoleId = role.RoleId,
                                TaskMasterId = model.TaskMasterId,
                                RoleTaskStatus = (byte)RoleTaskStatus.Active,
                            };
                        RoleTask.LastUpdatedBy = SessionUtilies.GetLoginId();
                        RoleTask.LastUpdatedOn = DateTime.Now;
                        RoleTask.CanDelete = model.CanDelete;
                        RoleTask.CanEdit = model.CanEdit;
                        if (RoleTask.RoleTaskId == 0)
                            dataContext.RoleTasks.InsertOnSubmit(RoleTask);
                        dataContext.SubmitChanges();
                    }
                });

                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This RoleTask is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
