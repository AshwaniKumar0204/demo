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
    [RoutePrefix("api/Menu")]
    public class MenuController : ApiController
    {
        [HttpGet]
        [Route("UserMenu")]
        public ExpandoObject UserMenu()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int LoginId = SessionUtilies.GetLoginId();
                var TaskList = (from sr1 in dataContext.SystemLoginRoles
                                join r1 in dataContext.Roles
                                on sr1.RoleId equals r1.RoleId
                                join rt1 in dataContext.RoleTasks
                                on r1.RoleId equals rt1.RoleId
                                join t1 in dataContext.TaskMasters
                                on rt1.TaskMasterId equals t1.TaskMasterId
                                join m1 in dataContext.Menus
                                on rt1.TaskMasterId equals m1.TaskMasterId
                                where sr1.LoginId == LoginId
                                && t1.TaskMasterStatus == (byte)TaskMasterStatus.Active
                                && r1.RoleStatus == (byte)RoleStatus.Active
                                && rt1.RoleTaskStatus == (byte)RoleTaskStatus.Active
                                select new MenuModel
                                {
                                    IconClass = m1.IconClass,
                                    MenuId = m1.MenuId,
                                    MenuPosition = m1.MenuPosition,
                                    MenuTitle = m1.MenuTitle,
                                    ParentMenuId = m1.ParentMenuId,
                                    TaskMasterId = m1.TaskMasterId,
                                    CanDelete = rt1.CanDelete,
                                    CanEdit = rt1.CanEdit,
                                    ActionName = t1.ActionName
                                }).Distinct();

                var list = (from m1 in dataContext.Menus.Where(x => x.ParentMenuId == null)
                            join t2 in TaskList
                            on m1.MenuId equals t2.MenuId into subTaskList2
                            from sTask2 in subTaskList2.DefaultIfEmpty()
                            where (sTask2.MenuId != null || m1.TaskMasterId == null)
                            orderby m1.MenuPosition
                            select new MenuModel
                            {
                                IconClass = m1.IconClass,
                                MenuId = m1.MenuId,
                                MenuPosition = m1.MenuPosition,
                                MenuTitle = m1.MenuTitle,
                                ParentMenuId = m1.ParentMenuId,
                                TaskMasterId = m1.TaskMasterId,
                                CanEdit = sTask2.CanEdit != null ? sTask2.CanEdit : false,
                                CanDelete = sTask2.CanDelete != null ? sTask2.CanDelete : false,
                                ActionName = sTask2.ActionName != null ? sTask2.ActionName : null,
                                MenuList = (from m2 in dataContext.Menus.Where(x => x.ParentMenuId != null && x.ParentMenuId == m1.MenuId)
                                            join t1 in TaskList
                                            on m2.MenuId equals t1.MenuId into subTaskList
                                            from sTask in subTaskList.DefaultIfEmpty()
                                            where (sTask.MenuId != null || m2.TaskMasterId == null)
                                            orderby m2.MenuPosition
                                            select new MenuModel
                                            {
                                                IconClass = m2.IconClass,
                                                MenuId = m2.MenuId,
                                                MenuPosition = m2.MenuPosition,
                                                MenuTitle = m2.MenuTitle,
                                                ParentMenuId = m2.ParentMenuId,
                                                TaskMasterId = m2.TaskMasterId,
                                                CanEdit = sTask.CanEdit != null ? sTask.CanEdit : false,
                                                CanDelete = sTask.CanDelete != null ? sTask.CanDelete : false,
                                                ActionName = sTask.ActionName != null ? sTask.ActionName : null,
                                                MenuList = (from m3 in TaskList
                                                            where m3.ParentMenuId == m2.MenuId
                                                            orderby m3.MenuPosition
                                                            select new MenuModel
                                                            {
                                                                IconClass = m3.IconClass,
                                                                MenuId = m3.MenuId,
                                                                MenuPosition = m3.MenuPosition,
                                                                MenuTitle = m3.MenuTitle,
                                                                ParentMenuId = m3.ParentMenuId,
                                                                TaskMasterId = m3.TaskMasterId,
                                                                CanDelete = m3.CanDelete,
                                                                CanEdit = m3.CanEdit,
                                                                ActionName = m3.ActionName
                                                            }).ToList()
                                            }).ToList()
                            }).ToList();
                list.ForEach(menuOne => menuOne.MenuList = menuOne.MenuList.Where(x => x.MenuList.Any() || x.TaskMasterId != null).ToList());
                list = list.Where(x => x.MenuList.Any()).ToList();
                ResponseMessage.MenuList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }


        [HttpGet]
        [Route("MenuList")]
        public ExpandoObject MenuList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.MenuList = SchoolData.GetMenuList(dataContext);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveMenu")]
        public ExpandoObject SaveMenu(MenuModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Menu Menu = new Menu();
                if (model.MenuId > 0)
                    Menu = dataContext.Menus.Where(x => x.MenuId == model.MenuId).First();
                Menu.MenuTitle = model.MenuTitle;
                Menu.IconClass = model.IconClass;
                Menu.ParentMenuId = model.ParentMenuId;
                Menu.TaskMasterId = model.TaskMasterId;
                if (Menu.MenuId == 0)
                {
                    var menus = dataContext.Menus.Where(x => x.ParentMenuId == model.ParentMenuId).OrderByDescending(x => x.MenuPosition);
                    if (menus.Any())
                        Menu.MenuPosition = menus.First().MenuPosition + 1;
                    else
                        Menu.MenuPosition = 1;
                    dataContext.Menus.InsertOnSubmit(Menu);
                }
                dataContext.SubmitChanges();
                ResponseMessage.MenuList = SchoolData.GetMenuList(dataContext);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Menu is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteMenu")]
        public ExpandoObject DeleteMenu(int MenuId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Menu = dataContext.Menus.Where(x => x.MenuId == MenuId).First();
                dataContext.Menus.DeleteOnSubmit(Menu);
                dataContext.SubmitChanges();
                ResponseMessage.MenuList = SchoolData.GetMenuList(dataContext);
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

        [HttpGet]
        [Route("MenuUp")]
        public ExpandoObject MenuUp(int MenuId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Menu = dataContext.Menus.Where(x => x.MenuId == MenuId).First();
                Menu.MenuPosition -= 1;
                dataContext.SubmitChanges();

                var oldMenus = (from m1 in dataContext.Menus
                                where m1.MenuId != MenuId
                                && m1.MenuPosition == Menu.MenuPosition
                                && m1.ParentMenuId == Menu.ParentMenuId
                                select m1);
                if (oldMenus.Any())
                {
                    var OldMenu = oldMenus.First();
                    OldMenu.MenuPosition += 1;
                    dataContext.SubmitChanges();
                }

                ResponseMessage.MenuList = SchoolData.GetMenuList(dataContext);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("MenuDown")]
        public ExpandoObject MenuDown(int MenuId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Menu = dataContext.Menus.Where(x => x.MenuId == MenuId).First();
                Menu.MenuPosition += 1;
                dataContext.SubmitChanges();

                var oldMenus = (from m1 in dataContext.Menus
                                where m1.MenuId != MenuId
                                && m1.MenuPosition == Menu.MenuPosition
                                && m1.ParentMenuId == Menu.ParentMenuId
                                select m1);
                if (oldMenus.Any())
                {
                    var OldMenu = oldMenus.First();
                    OldMenu.MenuPosition -= 1;
                    dataContext.SubmitChanges();
                }

                ResponseMessage.MenuList = SchoolData.GetMenuList(dataContext);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("TaskMasterListForMenu")]
        public ExpandoObject TaskMasterListForMenu()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from t1 in dataContext.TaskMasters
                           where !(from m1 in dataContext.Menus
                                   where m1.TaskMasterId != null
                                   select m1.TaskMasterId).Contains(t1.TaskMasterId)
                           orderby t1.ActionName
                           select new TaskMasterModel
                           {
                               TaskMasterId = t1.TaskMasterId,
                               TaskGroupId = t1.TaskGroupId,
                               TaskMasterStatus = t1.TaskMasterStatus,
                               ControllerName = t1.ControllerName,
                               ActionName = System.Text.RegularExpressions.Regex.Replace(t1.ActionName, "[A-Z]", " $0"),
                               GroupName = t1.TaskGroup.GroupName
                           };

                ResponseMessage.TaskMasterList = list.ToList();
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
