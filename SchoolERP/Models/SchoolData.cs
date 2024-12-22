using School;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SchoolERP.Models
{
    public class SchoolData
    {
        

        public static List<MenuModel> GetMenuList(SchoolDataContext dataContext)
        {
            var list = from m1 in dataContext.Menus
                       where m1.ParentMenuId == null
                       orderby m1.MenuPosition
                       select new MenuModel
                       {
                           IconClass = m1.IconClass,
                           MenuId = m1.MenuId,
                           MenuPosition = m1.MenuPosition,
                           MenuTitle = m1.MenuTitle,
                           ParentMenuId = m1.ParentMenuId,
                           TaskMasterId = m1.TaskMasterId,
                           MenuList = (from m2 in dataContext.Menus
                                       where m2.ParentMenuId == m1.MenuId
                                       orderby m2.MenuPosition
                                       select new MenuModel
                                       {
                                           IconClass = m2.IconClass,
                                           MenuId = m2.MenuId,
                                           MenuPosition = m2.MenuPosition,
                                           MenuTitle = m2.MenuTitle,
                                           ParentMenuId = m2.ParentMenuId,
                                           TaskMasterId = m2.TaskMasterId,
                                           MenuList = (from m3 in dataContext.Menus
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
                                                       }).ToList()
                                       }).ToList()
                       };
            return list.ToList();
        }

        public static bool CheckMenuAction(string actionName)
        {
            SchoolDataContext dataContext = new SchoolDataContext();
            var sysytemLogin = dataContext.SystemLogins.Where(x => x.LoginId == SessionUtilies.GetLoginId()).First();
            if (sysytemLogin.Employee.IsSuperAdmin)
                return true;

            bool IsExist = (from sr1 in dataContext.SystemLoginRoles
                            join r1 in dataContext.Roles
                            on sr1.RoleId equals r1.RoleId
                            join rt1 in dataContext.RoleTasks
                            on r1.RoleId equals rt1.RoleId
                            join t1 in dataContext.TaskMasters
                            on rt1.TaskMasterId equals t1.TaskMasterId
                            where sr1.LoginId == sysytemLogin.LoginId
                            && t1.TaskMasterStatus == (byte)TaskMasterStatus.Active
                            && r1.RoleStatus == (byte)RoleStatus.Active
                            && rt1.RoleTaskStatus == (byte)RoleTaskStatus.Active
                            && t1.ActionName == actionName
                            select t1).Any();
            return IsExist;
        }

        public static List<StudentRemarkModel> GetStudentRemarkList(SchoolDataContext dataContext, int StudentId)
        {
            var list = from s1 in dataContext.StudentRemarks
                       where s1.StudentId == StudentId
                       orderby s1.RemarksDate descending
                       select new StudentRemarkModel
                       {
                           StudentId = s1.StudentId,
                           Remarks = s1.Remarks,
                           StudentRemarkId = s1.StudentRemarkId,
                           RemarksDate = s1.RemarksDate
                       };
            return list.ToList();
        }
    }
}