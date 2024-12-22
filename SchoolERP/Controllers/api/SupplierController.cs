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
    [RoutePrefix("api/Supplier")]
    public class SupplierController : ApiController
    {
        [HttpGet]
        [Route("SupplierList")]
        public ExpandoObject SupplierList(byte Status, byte SupplierType)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from s1 in dataContext.Suppliers
                           where (Status == 0 ? true : s1.SupplierStatus == Status)
                           && (SupplierType == 0 ? true : s1.SupplierType == SupplierType)
                           select new
                           {
                               s1.Address,
                               s1.CreatedBy,
                               s1.CreatedOn,
                               s1.Email,
                               s1.Fax,
                               s1.GSTNo,
                               s1.LastUpdatedBy,
                               s1.LastUpdatedOn,
                               s1.MobileNo,
                               s1.StateId,
                               s1.SupplierId,
                               s1.State.StateName,
                               s1.SupplierName,
                               s1.SupplierStatus,
                               s1.SupplierType,
                               SupplierTypeName = Enum.GetName(typeof(SupplierType), s1.SupplierType),
                           };
                response.SupplierList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveSupplier")]
        public ExpandoObject SaveSupplier(SupplierModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Supplier Supplier = new Supplier();
                if (model.SupplierId > 0)
                    Supplier = dataContext.Suppliers.Where(x => x.SupplierId == model.SupplierId).First();
                Supplier.SupplierId = model.SupplierId;
                Supplier.SupplierName = model.SupplierName;
                Supplier.MobileNo = model.MobileNo;
                Supplier.Email = model.Email;
                Supplier.Fax = model.Fax;
                Supplier.Address = model.Address;
                Supplier.StateId = model.StateId;
                Supplier.GSTNo = model.GSTNo;
                Supplier.SupplierStatus = model.SupplierStatus;
                Supplier.SupplierType = model.SupplierType;
                if (Supplier.SupplierId == 0)
                {
                    Supplier.CreatedBy = SessionUtilies.GetLoginId();
                    Supplier.CreatedOn = DateTime.Now;
                    dataContext.Suppliers.InsertOnSubmit(Supplier);
                }
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {

                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteSupplier")]
        public ExpandoObject DeleteSupplier(int SupplierId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Supplier = dataContext.Suppliers.Where(x => x.SupplierId == SupplierId).First();
                dataContext.Suppliers.DeleteOnSubmit(Supplier);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This record is in use, so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }


            return ResponseMessage;
        }
    }
}
