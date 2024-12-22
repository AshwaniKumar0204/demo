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
    [RoutePrefix("api/PurchaseStoreItem")]
    public class PurchaseStoreItemController : ApiController
    {

        [HttpGet]
        [Route("PurchaseStoreItemList")]
        public ExpandoObject PurchaseStoreItemList(int PurchaseId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.PurchaseStoreItems
                           join s1 in dataContext.StoreItems
                           on p1.StoreItemId equals s1.StoreItemId
                           where p1.PurchaseId == PurchaseId
                           select new
                           {
                               p1.PurchaeStoreItemId,
                               s1.ItemCode,
                               s1.ItemName,
                               s1.Unit.UnitName,
                               s1.ItemType.ItemTypeName,
                               p1.Quantity,
                               p1.Rate,
                               p1.TaxableAmount,
                               p1.GST.GSTTitle,
                               p1.SGSTAmount,
                               p1.CGSTAmount,
                               p1.IGSTAmount,
                               p1.LineTotal,
                           };

                response.PurchaseStoreItemList = list.ToList();
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
