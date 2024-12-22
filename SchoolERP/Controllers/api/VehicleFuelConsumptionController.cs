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
    [RoutePrefix("api/VehicleFuelConsumption")]
    public class VehicleFuelConsumptionController : ApiController
    {
        [HttpPost]
        [Route("SaveVehicleFuelConsumption")]
        public ExpandoObject SaveVehicleFuelConsumption(VehicleFuelConsumption model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
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

                VehicleFuelConsumption VehicleFuelConsumption = null;
                if (model.VehicleFuelConsumptionId > 0)
                    VehicleFuelConsumption = dataContext.VehicleFuelConsumptions.Where(x => x.VehicleFuelConsumptionId == model.VehicleFuelConsumptionId).First();
                else
                    VehicleFuelConsumption = new VehicleFuelConsumption();
                VehicleFuelConsumption.BusId = model.BusId;
                VehicleFuelConsumption.SupplierId = SupplierId;
                VehicleFuelConsumption.BillDate = model.BillDate;
                VehicleFuelConsumption.BillNo = model.BillNo;
                VehicleFuelConsumption.Material = model.Material;
                VehicleFuelConsumption.UnitId = model.UnitId;
                VehicleFuelConsumption.PreviousKillometeres = model.PreviousKillometeres;
                VehicleFuelConsumption.CurrentKillometers = model.CurrentKillometers;
                VehicleFuelConsumption.Quantity = model.Quantity;
                VehicleFuelConsumption.Rate = model.Rate;
                VehicleFuelConsumption.Remarks = model.Remarks;
                if (model.PreviousKillometeres != null)
                    VehicleFuelConsumption.Average = (model.CurrentKillometers - model.PreviousKillometeres) / model.Quantity;
                VehicleFuelConsumption.Amount = model.Quantity * model.Rate;
                VehicleFuelConsumption.LastUpdatedOn = DateTime.Now;
                VehicleFuelConsumption.LastUpdatedBy = SessionUtilies.GetLoginId();
                if (VehicleFuelConsumption.VehicleFuelConsumptionId == 0)
                    dataContext.VehicleFuelConsumptions.InsertOnSubmit(VehicleFuelConsumption);
                dataContext.SubmitChanges();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("VehicleFuelConsumptionList")]
        public ExpandoObject VehicleFuelConsumptionList(DateFilterModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (model.FromDate != null)
                    model.FromDate = model.FromDate.Value.Date;
                if (model.ToDate != null)
                    model.ToDate = model.ToDate.Value.Date;
                var list = from f1 in dataContext.VehicleFuelConsumptions
                           where (model.BusId == 0 ? true : f1.BusId == model.BusId)
                           && (model.FromDate != null ? f1.BillDate.Date >= model.FromDate : true)
                           && (model.ToDate != null ? f1.BillDate.Date <= model.ToDate : true)
                           && (model.SupplierId == 0 ? true : f1.SupplierId == model.SupplierId)
                           orderby f1.BillDate descending
                           select new
                           {
                               f1.VehicleFuelConsumptionId,
                               f1.BillDate,
                               f1.BillNo,
                               f1.BusId,
                               f1.Bus.BusTitle,
                               f1.Supplier.SupplierName,
                               f1.SupplierId,
                               f1.Material,
                               f1.PreviousKillometeres,
                               f1.CurrentKillometers,
                               f1.Quantity,
                               f1.Unit.UnitName,
                               f1.UnitId,
                               f1.Average,
                               f1.Rate,
                               f1.Amount,
                               f1.Remarks
                           };
                decimal PreviousKillometers = 0;
                if (list.Any())
                    PreviousKillometers = list.First().CurrentKillometers;
                response.VehicleFuelConsumptionList = list.ToList();
                response.PreviousKillometers = PreviousKillometers;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("DeleteVehicleFuelConsumption")]
        public ExpandoObject DeleteVehicleFuelConsumption(int VehicleFuelConsumptionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                VehicleFuelConsumption VehicleFuelConsumption = dataContext.VehicleFuelConsumptions.Where(x => x.VehicleFuelConsumptionId == VehicleFuelConsumptionId).First();
                dataContext.VehicleFuelConsumptions.DeleteOnSubmit(VehicleFuelConsumption);
                dataContext.SubmitChanges();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
