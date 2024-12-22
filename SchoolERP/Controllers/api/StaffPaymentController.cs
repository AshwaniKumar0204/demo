using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/staffpayment")]
    public class StaffPaymentController : ApiController
    {
        [HttpPost]
        [Route("StaffPaymentDetail")]
        public ExpandoObject StaffPaymentDetail(StaffPayment model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (dataContext.StaffPayments.Where(x => (x.StaffId == model.StaffId && x.MonthNo == model.MonthNo && x.Year == model.Year) || x.StaffPaymentId == model.StaffPaymentId).Any())
                {
                    var StaffPayment = (from s1 in dataContext.StaffPayments
                                        join m2 in dataContext.Staffs
                                        on s1.StaffId equals m2.StaffId
                                        where (s1.StaffId == model.StaffId
                                        && s1.MonthNo == model.MonthNo
                                        && s1.Year == model.Year)
                                        || s1.StaffPaymentId == model.StaffPaymentId
                                        select new
                                        {
                                            StaffCode = m2.StaffCode,
                                            StaffName = m2.StaffName,
                                            DesignationName = m2.Designation.DesignationName,
                                            s1.StaffSalaryId,
                                            s1.StaffPaymentId,
                                            s1.StaffId,
                                            s1.SalaryDate,
                                            s1.MonthNo,
                                            s1.Year,
                                            s1.BankAccountNo,
                                            s1.PFNo,
                                            s1.ESINo,
                                            s1.LWPAvailed,
                                            s1.CLAvailed,
                                            s1.ELAvailed,
                                            s1.MLAvailed,
                                            s1.HPLAvailed,
                                            s1.BasicAmount,
                                            s1.Increament,
                                            s1.DA,
                                            s1.Allowance,
                                            s1.Arrear,
                                            s1.HRA,
                                            s1.MedicalAllowance,
                                            s1.SpecialAllowance,
                                            s1.GrossTotal,
                                            s1.EPF,
                                            s1.ESI,
                                            s1.TDS,
                                            s1.ProfessionalTax,
                                            s1.SalaryAdvanced,
                                            s1.OtherDeduction,
                                            s1.HouseRent,
                                            s1.ElectricityCharge,
                                            s1.LWP,
                                            s1.TotalDeduction,
                                            s1.NetPayment,
                                            s1.Remarks
                                        }).First();
                    response.StaffPayment = StaffPayment;
                }
                else
                {
                    if (!dataContext.StaffSalaries.Where(x => x.StaffId == model.StaffId && x.Status == (byte)Status.Active).Any())
                        throw new Exception("This staff has no active salary");
                    var StaffPayment = (from m1 in dataContext.StaffSalaries
                                        join m2 in dataContext.Staffs
                                        on m1.StaffId equals m2.StaffId
                                        join d1 in dataContext.Designations
                                        on m2.DesignationId equals d1.DesignationId into subDes
                                        join m3 in dataContext.StaffLeaves.Where(x => x.MonthNo == model.MonthNo && x.Year == model.Year)
                                        on m1.StaffId equals m3.StaffId into subStaffLeaves
                                        where m1.StaffId == model.StaffId
                                        && m1.Status == (byte)Status.Active
                                        select new StaffPaymentModel
                                        {
                                            StaffSalaryId = m1.StaffSalaryId,
                                            StaffId = m1.StaffId,
                                            StaffCode = m2.StaffCode,
                                            StaffName = m2.StaffName,
                                            DesignationName = m2.Designation.DesignationName,
                                            MonthNo = model.MonthNo,
                                            Year = model.Year,
                                            BankAccountNo = m2.BankAccountNo,
                                            PFNo = m2.PFNo,
                                            ESINo = m2.ESINo,
                                            LWPAvailed = subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.LWP).Any() ? subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.LWP).Sum(x => x.NoOfLeaves) : 0,
                                            CLAvailed = subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.CL).Any() ? subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.CL).Sum(x => x.NoOfLeaves) : 0,
                                            ELAvailed = subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.EL).Any() ? subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.EL).Sum(x => x.NoOfLeaves) : 0,
                                            MLAvailed = subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.ML).Any() ? subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.ML).Sum(x => x.NoOfLeaves) : 0,
                                            HPLAvailed = subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.HPL).Any() ? subStaffLeaves.Where(x => x.LeaveType == (byte)LeaveType.HPL).Sum(x => x.NoOfLeaves) : 0,
                                            BasicAmount = m1.BasicAmount,
                                            Increament = m1.Increament,
                                            DA = m1.DA,
                                            Allowance = m1.Allowance,
                                            Arrear = m1.Arrear,
                                            HRA = m1.HRA,
                                            MedicalAllowance = m1.MedicalAllowance,
                                            SpecialAllowance = m1.SpecialAllowance,
                                            GrossTotal = m1.GrossTotal,
                                        }).First();
                    var advanceSalary = dataContext.StaffAdvancePayments.Where(x => x.StaffId == model.StaffId && x.MonthNo == model.MonthNo && x.Year == model.Year);
                    if (advanceSalary.Any())
                        StaffPayment.SalaryAdvanced = advanceSalary.Sum(x => x.AdvanceAmount);
                    //var previousAvailableLeaves = SalaryData.GetStaffAvailableLeave(dataContext, model.StaffId, (model.MonthNo == 1 ? 12 : model.MonthNo - 1), (model.MonthNo == 1 ? model.Year - 1 : model.Year));

                    //StaffPayment.CLExtraLeave = previousAvailableLeaves.Where(x => x.LeaveType == (byte)LeaveType.CL).First().NoOfLeaves;
                    //StaffPayment.ELExtraLeave = previousAvailableLeaves.Where(x => x.LeaveType == (byte)LeaveType.EL).First().NoOfLeaves;
                    //StaffPayment.MLExtraLeave = previousAvailableLeaves.Where(x => x.LeaveType == (byte)LeaveType.ML).First().NoOfLeaves;
                    //StaffPayment.HPLExtraLeave = previousAvailableLeaves.Where(x => x.LeaveType == (byte)LeaveType.HPL).First().NoOfLeaves;
                    //StaffPayment.HPLExtraLeave = previousAvailableLeaves.Where(x => x.LeaveType == (byte)LeaveType.HPL).First().NoOfLeaves;

                    response.StaffPayment = StaffPayment;
                }
                response.Message = ConstantData.SuccessMessage;

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    response.Message = "This record is already exist.";
                else
                    response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveStaffPayment")]
        public ExpandoObject SaveStaffPayment(StaffPayment model)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                StaffPayment StaffPayment = new StaffPayment();
                if (model.StaffPaymentId > 0)
                    StaffPayment = dataContext.StaffPayments.Where(x => x.StaffPaymentId == model.StaffPaymentId).First();
                StaffPayment.StaffId = model.StaffId;
                StaffPayment.SalaryDate = DateTime.Now;
                StaffPayment.MonthNo = model.MonthNo;
                StaffPayment.Year = model.Year;
                StaffPayment.BankAccountNo = model.BankAccountNo;
                StaffPayment.PFNo = model.PFNo;
                StaffPayment.ESINo = model.ESINo;
                StaffPayment.LWPAvailed = model.LWPAvailed;
                StaffPayment.CLAvailed = model.CLAvailed;
                StaffPayment.ELAvailed = model.ELAvailed;
                StaffPayment.MLAvailed = model.MLAvailed;
                StaffPayment.HPLAvailed = model.HPLAvailed;
                StaffPayment.BasicAmount = model.BasicAmount;
                StaffPayment.Increament = model.Increament;
                StaffPayment.DA = model.DA;
                StaffPayment.Allowance = model.Allowance;
                StaffPayment.Arrear = model.Arrear;
                StaffPayment.HRA = model.HRA;
                StaffPayment.MedicalAllowance = model.MedicalAllowance;
                StaffPayment.SpecialAllowance = model.SpecialAllowance;
                StaffPayment.GrossTotal = model.GrossTotal;
                StaffPayment.EPF = model.EPF;
                StaffPayment.TDS = model.TDS;
                StaffPayment.ESI = model.ESI;
                StaffPayment.ProfessionalTax = model.ProfessionalTax;
                StaffPayment.SalaryAdvanced = model.SalaryAdvanced;
                StaffPayment.OtherDeduction = model.OtherDeduction;
                StaffPayment.HouseRent = model.HouseRent;
                StaffPayment.ElectricityCharge = model.ElectricityCharge;
                StaffPayment.LWP = model.LWP;
                StaffPayment.TotalDeduction = model.TotalDeduction;
                StaffPayment.NetPayment = model.NetPayment;
                StaffPayment.StaffSalaryId = model.StaffSalaryId;
                StaffPayment.Remarks = model.Remarks;
                if (StaffPayment.StaffPaymentId == 0)
                    dataContext.StaffPayments.InsertOnSubmit(StaffPayment);
                dataContext.SubmitChanges();

                transaction.Commit();
                IsCompleted = true;
                response.StaffPaymentId = StaffPayment.StaffPaymentId;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }

        [HttpPost]
        [Route("StaffPaymentList")]
        public ExpandoObject StaffPaymentList(StaffPayment model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var StaffPayment = (from s1 in dataContext.StaffPayments
                                    join m2 in dataContext.Staffs
                                    on s1.StaffId equals m2.StaffId
                                    where (model.StaffId == 0 || s1.StaffId == model.StaffId)
                                    && (model.MonthNo == 0 || s1.MonthNo == model.MonthNo)
                                    && (model.Year == 0 || s1.Year == model.Year)
                                    select new
                                    {
                                        StaffCode = m2.StaffCode,
                                        StaffName = m2.StaffName,
                                        DesignationName = m2.Designation.DesignationName,
                                        s1.StaffSalaryId,
                                        s1.StaffPaymentId,
                                        s1.StaffId,
                                        s1.SalaryDate,
                                        s1.MonthNo,
                                        MonthName = Enum.GetName(typeof(MonthNames), s1.MonthNo),
                                        s1.Year,
                                        s1.BankAccountNo,
                                        s1.PFNo,
                                        s1.ESINo,
                                        s1.LWPAvailed,
                                        s1.CLAvailed,
                                        s1.ELAvailed,
                                        s1.MLAvailed,
                                        s1.HPLAvailed,
                                        s1.BasicAmount,
                                        s1.Increament,
                                        s1.DA,
                                        s1.Allowance,
                                        s1.Arrear,
                                        s1.HRA,
                                        s1.MedicalAllowance,
                                        s1.SpecialAllowance,
                                        s1.GrossTotal,
                                        s1.EPF,
                                        s1.ESI,
                                        s1.TDS,
                                        s1.ProfessionalTax,
                                        s1.SalaryAdvanced,
                                        s1.OtherDeduction,
                                        s1.HouseRent,
                                        s1.ElectricityCharge,
                                        s1.LWP,
                                        s1.TotalDeduction,
                                        s1.NetPayment,
                                        s1.Remarks
                                    });
                response.StaffPaymentList = StaffPayment.ToList();
                response.Message = ConstantData.SuccessMessage;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("deleteStaffPayment")]
        public ExpandoObject DeleteStaffPayment(int StaffPaymentId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var StaffPayment = dataContext.StaffPayments.Where(x => x.StaffPaymentId == StaffPaymentId).First();
                dataContext.StaffPayments.DeleteOnSubmit(StaffPayment);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This record is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

    }
}
