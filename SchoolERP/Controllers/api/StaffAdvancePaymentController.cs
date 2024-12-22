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
    [RoutePrefix("api/StaffAdvancePayment")]
    public class StaffAdvancePaymentController : ApiController
    {
        [HttpPost]
        [Route("StaffAdvancePaymentList")]
        public ExpandoObject StaffAdvancePaymentList(StaffAdvancePayment model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from m1 in dataContext.StaffAdvancePayments
                           join m2 in dataContext.Staffs
                           on m1.StaffId equals m2.StaffId
                           join d1 in dataContext.Designations
                           on m2.DesignationId equals d1.DesignationId into subDes
                           where (model.StaffId == 0 || m1.StaffId == model.StaffId)
                           && (model.MonthNo == 0 || m1.MonthNo == model.MonthNo)
                           && (model.Year == 0 || m1.Year == model.Year)
                           select new
                           {
                               m1.StaffAdvancePaymentId,
                               m1.StaffId,
                               m2.StaffName,
                               m2.StaffCode,
                               m1.Remarks,
                               m1.MonthNo,
                               MonthName = Enum.GetName(typeof(MonthNames), m1.MonthNo),
                               m1.Year,
                               m1.AdvanceAmount,
                               m1.AdvanceDate,
                               DesignationName = subDes.Any() ? subDes.First().DesignationName : "",
                           };
                ResponseMessage.StaffAdvancePaymentList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveStaffAdvancePayment")]
        public ExpandoObject SaveStaffAdvancePayment(StaffAdvancePayment model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StaffAdvancePayment StaffAdvancePayment = null;
                if (model.StaffAdvancePaymentId > 0)
                    StaffAdvancePayment = dataContext.StaffAdvancePayments.Where(x => x.StaffAdvancePaymentId == model.StaffAdvancePaymentId).First();

                else
                    StaffAdvancePayment = new StaffAdvancePayment();
                StaffAdvancePayment.StaffId = model.StaffId;
                StaffAdvancePayment.MonthNo = model.MonthNo;
                StaffAdvancePayment.Year = model.Year;
                StaffAdvancePayment.AdvanceDate = model.AdvanceDate;
                StaffAdvancePayment.AdvanceAmount = model.AdvanceAmount;
                StaffAdvancePayment.Remarks = model.Remarks;
                StaffAdvancePayment.LastUpdatedBy = SessionUtilies.GetLoginId();
                StaffAdvancePayment.LastUpdatedOn = DateTime.Now;
                if (StaffAdvancePayment.StaffAdvancePaymentId == 0)
                    dataContext.StaffAdvancePayments.InsertOnSubmit(StaffAdvancePayment);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This staff advance payment is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteStaffAdvancePayment")]
        public ExpandoObject DeleteStaffAdvancePayment(int StaffAdvancePaymentId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var StaffAdvancePayment = dataContext.StaffAdvancePayments.Where(x => x.StaffAdvancePaymentId == StaffAdvancePaymentId).First();
                dataContext.StaffAdvancePayments.DeleteOnSubmit(StaffAdvancePayment);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This staff advance payment is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
