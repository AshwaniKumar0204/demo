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

namespace SchoolERP.Controllers
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/VehicleService")]
    public class VehicleServiceController : ApiController
    {

        [HttpPost]
        [Route("VehicleServiceList")]
        public ExpandoObject VehicleServiceList(DateFilterModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (model.FromDate != null)
                    model.FromDate = model.FromDate.Value.Date;
                if (model.ToDate != null)
                    model.ToDate = model.ToDate.Value.Date;
                var list = from f1 in dataContext.VehicleServices
                           where (model.BusId == 0 ? true : f1.BusId == model.BusId)
                           && (model.FromDate != null ? f1.BillDate.Date >= model.FromDate : true)
                           && (model.ToDate != null ? f1.BillDate.Date <= model.ToDate : true)
                           && (model.SupplierId == 0 ? true : f1.SupplierId == model.SupplierId)
                           orderby f1.BillDate descending
                           select new
                           {
                               f1.VehicleServiceId,
                               f1.BillDate,
                               f1.BillNo,
                               f1.BusId,
                               f1.Bus.BusTitle,
                               f1.Supplier.SupplierName,
                               f1.SupplierId,
                               f1.MeterReading,
                               f1.BillAmount,
                               f1.Discount,
                               f1.PaidAmount,
                               f1.Remarks,
                               f1.VehicleServiceType,
                               VehicleServiceTypeName = Enum.GetName(typeof(VehicleServiceType), f1.VehicleServiceType)
                           };
                response.VehicleServiceList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveVehicleService")]
        public ExpandoObject SaveVehicleService(VehicleService model)
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
                int loginId = SessionUtilies.GetLoginId();

                int SupplierId = model.SupplierId;
                if (model.SupplierId == 0 && !string.IsNullOrEmpty(model.Supplier.SupplierName))
                {
                    var suppliers = dataContext.Suppliers.Where(x => x.SupplierName == model.Supplier.SupplierName && x.SupplierType == (byte)SupplierType.Trasport);
                    if (suppliers.Count() == 0)
                    {
                        Supplier supplier = new Supplier
                        {
                            SupplierName = model.Supplier.SupplierName,
                            SupplierStatus = 1,
                            StateId = 1,
                            CreatedBy = SessionUtilies.GetLoginId(),
                            CreatedOn = DateTime.Now,
                            SupplierType = (byte)SupplierType.Trasport
                        };
                        dataContext.Suppliers.InsertOnSubmit(supplier);
                        dataContext.SubmitChanges();
                        SupplierId = supplier.SupplierId;
                    }
                    else
                        SupplierId = suppliers.First().SupplierId;
                }

                VehicleService VehicleService = null;
                if (model.VehicleServiceId > 0)
                    VehicleService = dataContext.VehicleServices.Where(x => x.VehicleServiceId == model.VehicleServiceId).First();
                else
                    VehicleService = new VehicleService();
                VehicleService.SupplierId = SupplierId;
                VehicleService.BillDate = model.BillDate;
                VehicleService.BillNo = model.BillNo;
                VehicleService.Remarks = model.Remarks;
                VehicleService.BusId = model.BusId;
                VehicleService.BillAmount = model.VehicleServiceDetails.Sum(x => x.Amount);
                VehicleService.Discount = model.Discount;
                VehicleService.PaidAmount = VehicleService.BillAmount - VehicleService.Discount;
                VehicleService.MeterReading = model.MeterReading;
                VehicleService.LastUpdatedBy = loginId;
                VehicleService.VehicleServiceType = model.VehicleServiceType;
                VehicleService.LastUpdatedOn = DateTime.Now;
                if (VehicleService.VehicleServiceId == 0)
                    dataContext.VehicleServices.InsertOnSubmit(VehicleService);
                dataContext.SubmitChanges();

                //delete Data 
                var deleteVehicleServiceDetails = dataContext.VehicleServiceDetails.Where(y => y.VehicleServiceId == model.VehicleServiceId && !model.VehicleServiceDetails.Select(z => z.VehicleServiceDetailId).Contains(y.VehicleServiceDetailId));
                dataContext.VehicleServiceDetails.DeleteAllOnSubmit(deleteVehicleServiceDetails);
                dataContext.SubmitChanges();


                model.VehicleServiceDetails.ToList().ForEach(x =>
                 {
                     VehicleServiceDetail vehicleServiceDetail = null;
                     if (x.VehicleServiceDetailId > 0)
                         vehicleServiceDetail = dataContext.VehicleServiceDetails.Where(y => y.VehicleServiceDetailId == x.VehicleServiceDetailId).First();
                     else
                         vehicleServiceDetail = new VehicleServiceDetail();
                     vehicleServiceDetail.Remarks = x.Remarks;
                     vehicleServiceDetail.Amount = x.Amount;
                     vehicleServiceDetail.UnitCost = x.UnitCost;
                     vehicleServiceDetail.Quantity = x.Quantity;
                     vehicleServiceDetail.JobType = x.JobType;
                     vehicleServiceDetail.VehicleServiceId = VehicleService.VehicleServiceId;
                     if (vehicleServiceDetail.VehicleServiceDetailId == 0)
                         dataContext.VehicleServiceDetails.InsertOnSubmit(vehicleServiceDetail);
                     dataContext.SubmitChanges();
                 });
                transaction.Commit();
                IsCompleted = true;
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

        [HttpGet]
        [Route("VehicleServiceDetail")]
        public ExpandoObject VehicleServiceDetail(int VehicleServiceId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from f1 in dataContext.VehicleServices
                           where f1.VehicleServiceId == VehicleServiceId
                           select new
                           {
                               f1.VehicleServiceId,
                               f1.BillDate,
                               f1.BillNo,
                               f1.BusId,
                               f1.Bus.BusTitle,
                               f1.Supplier.SupplierName,
                               f1.SupplierId,
                               f1.MeterReading,
                               f1.BillAmount,
                               f1.Discount,
                               f1.PaidAmount,
                               f1.Remarks,
                               f1.VehicleServiceType,
                               VehicleServiceDetails = f1.VehicleServiceDetails.Select(x => new
                               {
                                   x.VehicleServiceId,
                                   x.VehicleServiceDetailId,
                                   x.UnitCost,
                                   x.Quantity,
                                   x.Amount,
                                   x.Remarks,
                                   x.JobType
                               })
                           };
                response.VehicleService = list.First();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("deleteVehicleService")]
        public ExpandoObject DeleteVehicleService(int VehicleServiceId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var VehicleService = dataContext.VehicleServices.Where(x => x.VehicleServiceId == VehicleServiceId).First();
                dataContext.VehicleServices.DeleteOnSubmit(VehicleService);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Book Type is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
