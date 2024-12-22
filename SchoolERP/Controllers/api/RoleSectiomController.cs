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
    [RoutePrefix("api/RoleSection")]
    public class RoleSectiomController : ApiController
    {
        [HttpGet]
        [Route("AllRoleSectionList")]
        public ExpandoObject AllRoleSectionList(int RoleId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var classList = from c1 in dataContext.Classes
                                group c1 by new { c1.ClassNo, c1.ClassName2 } into subClasses
                                select new ClassModel
                                {
                                    ClassName2 = subClasses.Key.ClassName2,
                                    ClassNo = subClasses.Key.ClassNo,
                                    RoleSectionList = (from s1 in dataContext.Sections
                                                       join c2 in dataContext.Classes
                                                       on s1.ClassId equals c2.ClassId
                                                       join r1 in dataContext.RoleSections.Where(x => x.RoleId == RoleId)
                                                       on s1.SectionId equals r1.SectionId into subRoleSections
                                                       from sRoleSection in subRoleSections.DefaultIfEmpty()
                                                       where c2.ClassNo == subClasses.Key.ClassNo
                                                       select new RoleSectionModel
                                                       {
                                                           SectionId = s1.SectionId,
                                                           SectionName = s1.SectionName,
                                                           IsSelected = sRoleSection != null ? true : false,
                                                           RoleId = RoleId,
                                                           RoleSectionId = sRoleSection != null ? sRoleSection.RoleSectionId : 0
                                                       }).ToList(),
                                };
                ResponseMessage.ClassList = classList.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveRoleSection")]
        public ExpandoObject SaveRoleSection(RoleModel role)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                role.RoleSectionList.ForEach(model =>
                {
                    if (model.RoleSectionId > 0 && !model.IsSelected)
                    {
                        var RoleSection = dataContext.RoleSections.Where(x => x.RoleSectionId == model.RoleSectionId).First();
                        dataContext.RoleSections.DeleteOnSubmit(RoleSection);
                        dataContext.SubmitChanges();
                    }
                    else if (model.IsSelected)
                    {
                        RoleSection RoleSection = null;
                        if (model.RoleSectionId > 0)
                            RoleSection = dataContext.RoleSections.Where(x => x.RoleSectionId == model.RoleSectionId).First();
                        else
                            RoleSection = new RoleSection
                            {
                                RoleId = role.RoleId,
                                SectionId = model.SectionId
                            };
                        RoleSection.LastUpdatedBy = SessionUtilies.GetLoginId();
                        RoleSection.LastUpdatedOn = DateTime.Now;
                        if (RoleSection.RoleSectionId == 0)
                            dataContext.RoleSections.InsertOnSubmit(RoleSection);
                        dataContext.SubmitChanges();
                    }
                });

                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Role Section is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
