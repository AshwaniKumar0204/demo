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
    //[CheckSessionTimeOutAttribute]
    //[RoutePrefix("api/StoreItem")]
    //public class StoreItemController : ApiController
    //{
    //    [HttpGet]
    //    [Route("StoreItemList")]
    //    public ExpandoObject StoreItemList(byte Status,int ItemTypeId)
    //    {
    //        dynamic ResponseMessage = new ExpandoObject();
    //        try
    //        {
    //            SchoolDataContext dataContext = new SchoolDataContext();
    //            var list = from b1 in dataContext.StoreItems
    //                       where (Status == 0 ? true : b1.StoreItemStatus == Status)
    //                       && (ItemTypeId == 0 ? true : b1.ItemTypeId == ItemTypeId)
    //                       select new StoreItemModel
    //                       {
    //                           StoreItemId = b1.StoreItemId,
    //                           StoreItemStatus = b1.StoreItemStatus,
    //                           ItemName = b1.ItemName,
    //                           ItemCode = b1.ItemCode,
    //                           ItemTypeId = b1.ItemTypeId,
    //                           ItemTypeName = b1.ItemType.ItemTypeName,
    //                           Quantity = b1.Quantity,
    //                           UnitId = b1.UnitId,
    //                           UnitName = b1.Unit.UnitName,
    //                       };
    //            ResponseMessage.StoreItemList = list.ToList();
    //            ResponseMessage.Message = ConstantData.SuccessMessage;
    //        }
    //        catch (Exception ex)
    //        {
    //            ResponseMessage.Message = ex.Message;
    //        }
    //        return ResponseMessage;
    //    }

    //    [HttpPost]
    //    [Route("saveStoreItem")]
    //    public ExpandoObject SaveStoreItem(StoreItemModel model)
    //    {
    //        dynamic ResponseMessage = new ExpandoObject();
    //        try
    //        {
    //            SchoolDataContext dataContext = new SchoolDataContext();
    //            StoreItem StoreItem = null;
    //            if (model.StoreItemId > 0)
    //            {
    //                StoreItem = dataContext.StoreItems.Where(x => x.StoreItemId == model.StoreItemId).First();
    //                StoreItem.LastUpdatedBy = SessionUtilies.GetLoginId();
    //                StoreItem.LastUpdatedOn = DateTime.Now;
    //            }
    //            else
    //            {
    //                StoreItem = new StoreItem
    //                {
    //                    CreatedBy = SessionUtilies.GetLoginId(),
    //                    CreatedOn = DateTime.Now,
    //                    Quantity = model.Quantity,
    //                    ItemCode = LoadData.GenerateStoreItemCode(dataContext)
    //                };
    //            }
    //            StoreItem.ItemName = model.ItemName;
    //            StoreItem.ItemTypeId = model.ItemTypeId;
    //            StoreItem.UnitId = model.UnitId;
    //            StoreItem.StoreItemStatus = model.StoreItemStatus;
    //            if (StoreItem.StoreItemId == 0)
    //                dataContext.StoreItems.InsertOnSubmit(StoreItem);
    //            dataContext.SubmitChanges();
    //            //ResponseMessage.StoreItemList = StoreData.StoreItemList(dataContext, 0);
    //            ResponseMessage.StoreItemId = StoreItem.StoreItemId;
    //            ResponseMessage.Message = ConstantData.SuccessMessage;
    //        }
    //        catch (Exception ex)
    //        {
    //            if (ex.Message.Contains("IX"))
    //                ResponseMessage.Message = "This Item is already exist";
    //            else
    //                ResponseMessage.Message = ex.Message;
    //        }
    //        return ResponseMessage;
    //    }

    //    [HttpGet]
    //    [Route("deleteStoreItem")]
    //    public ExpandoObject DeleteStoreItem(int StoreItemId)
    //    {
    //        dynamic ResponseMessage = new ExpandoObject();
    //        try
    //        {
    //            SchoolDataContext dataContext = new SchoolDataContext();
    //            var StoreItem = dataContext.StoreItems.Where(x => x.StoreItemId == StoreItemId).First();
    //            dataContext.StoreItems.DeleteOnSubmit(StoreItem);
    //            dataContext.SubmitChanges();
    //            //ResponseMessage.StoreItemList = StoreData.StoreItemList(dataContext, 0);
    //            ResponseMessage.Message = ConstantData.SuccessMessage;
    //        }
    //        catch (Exception ex)
    //        {
    //            if (ex.Message.Contains("FK"))
    //                ResponseMessage.Message = "This Item is in use.so can't delete.";
    //            else
    //                ResponseMessage.Message = ex.Message;
    //        }
    //        return ResponseMessage;
    //    }
    //}
}
