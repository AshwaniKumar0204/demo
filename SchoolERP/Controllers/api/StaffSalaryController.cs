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
    [RoutePrefix("api/StaffSalary")]
    public class StaffSalaryController : ApiController
    {
        [HttpGet]
        [Route("StaffSalaryList")]
        public ExpandoObject StaffSalaryList(int StaffId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from m1 in dataContext.StaffSalaries
                           join m2 in dataContext.Staffs
                           on m1.StaffId equals m2.StaffId
                           join d1 in dataContext.Designations
                           on m2.DesignationId equals d1.DesignationId into subDes
                           where (StaffId == 0 || m1.StaffId == StaffId)
                           select new
                           {
                               m1.StaffSalaryId,
                               m1.StaffId,
                               m2.StaffName,
                               m2.StaffCode,
                               m1.BasicAmount,
                               m1.Increament,
                               m1.DA,
                               m1.Allowance,
                               m1.Arrear,
                               m1.HRA,
                               m1.MedicalAllowance,
                               m1.SpecialAllowance,
                               m1.GrossTotal,
                               m1.SalaryStartDate,
                               m1.Status,
                               m1.Remarks,
                               DesignationName = subDes.Any() ? subDes.First().DesignationName : "",
                           };
                ResponseMessage.StaffSalaryList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveStaffSalary")]
        public ExpandoObject SaveStaffSalary(StaffSalary model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StaffSalary StaffSalary = null;
                if (model.StaffSalaryId > 0)
                    StaffSalary = dataContext.StaffSalaries.Where(x => x.StaffSalaryId == model.StaffSalaryId).First();
                   
                else
                    StaffSalary = new StaffSalary();
                StaffSalary.StaffId = model.StaffId;
                StaffSalary.BasicAmount = model.BasicAmount;
                StaffSalary.Increament = model.Increament;
                StaffSalary.DA = model.DA;
                StaffSalary.Allowance = model.Allowance;
                StaffSalary.Arrear = model.Arrear;
                StaffSalary.HRA = model.HRA;
                StaffSalary.MedicalAllowance = model.MedicalAllowance;
                StaffSalary.SpecialAllowance = model.SpecialAllowance;
                StaffSalary.GrossTotal = model.GrossTotal;
                StaffSalary.SalaryStartDate = model.SalaryStartDate;
                StaffSalary.Status = model.Status;
                StaffSalary.Remarks = model.Remarks;
                StaffSalary.LastUpdatedBy = SessionUtilies.GetLoginId();
                StaffSalary.LastUpdatedOn = DateTime.Now;
                if (StaffSalary.StaffSalaryId == 0)
                    dataContext.StaffSalaries.InsertOnSubmit(StaffSalary);
                dataContext.SubmitChanges();
                if(model.Status == 1)
                {
                    var activeSalaries = dataContext.StaffSalaries.Where(x => x.StaffSalaryId != StaffSalary.StaffSalaryId && x.StaffId == model.StaffId && x.Status == 1).ToList();
                    activeSalaries.ForEach(x => x.Status = 2);
                    dataContext.SubmitChanges();
                }
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Staff Salary is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteStaffSalary")]
        public ExpandoObject DeleteStaffSalary(int StaffSalaryId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var StaffSalary = dataContext.StaffSalaries.Where(x => x.StaffSalaryId == StaffSalaryId).First();
                dataContext.StaffSalaries.DeleteOnSubmit(StaffSalary);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Staff Salary is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
