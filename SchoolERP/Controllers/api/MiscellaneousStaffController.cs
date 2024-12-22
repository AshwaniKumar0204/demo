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
    [RoutePrefix("api/MiscellaneousStaff")]
    public class MiscellaneousStaffController : ApiController
    {
        [HttpGet]
        [Route("MiscellaneousStaffList")]
        public ExpandoObject MiscellaneousStaffList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from m1 in dataContext.MiscellaneousStaffs
                           orderby m1.StaffName
                           select new
                           {
                               m1.MiscellaneousStaffId,
                               m1.StaffName,
                               m1.MobileNo,
                               m1.Designation,
                               m1.Address,
                           };
                ResponseMessage.MiscellaneousStaffList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveMiscellaneousStaff")]
        public ExpandoObject SaveMiscellaneousStaff(MiscellaneousStaffModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                MiscellaneousStaff Staff = new MiscellaneousStaff();
                if (model.MiscellaneousStaffId > 0)
                    Staff = dataContext.MiscellaneousStaffs.Where(x => x.MiscellaneousStaffId == model.MiscellaneousStaffId).First();
                Staff.Address = model.Address;
                Staff.Designation = model.Designation;
                Staff.MobileNo = model.MobileNo;
                Staff.StaffName = model.StaffName;
                if (Staff.MiscellaneousStaffId == 0)
                {
                    Staff.CreatedOn = DateTime.Now;
                    Staff.CreatedBy = SessionUtilies.GetLoginId();
                    dataContext.MiscellaneousStaffs.InsertOnSubmit(Staff);
                }
                else
                {
                    Staff.LastUpdatedBy = SessionUtilies.GetLoginId();
                    Staff.LastUpdatedOn = DateTime.Now;
                }
                dataContext.SubmitChanges();
                ResponseMessage.MiscellaneousStaffId = Staff.MiscellaneousStaffId;
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Staff is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

    }
}
