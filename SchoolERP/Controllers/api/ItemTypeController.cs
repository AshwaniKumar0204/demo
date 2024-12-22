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
    [RoutePrefix("api/ItemType")]
    public class ItemTypeController : ApiController
    {
        [HttpGet]
        [Route("ItemTypeList")]
        public ExpandoObject ItemTypeList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.ItemTypeList = StoreData.ItemTypeList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveItemType")]
        public ExpandoObject SaveItemType(ItemTypeModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ItemType ItemType = new ItemType();
                if (model.ItemTypeId > 0)
                    ItemType = dataContext.ItemTypes.Where(x => x.ItemTypeId == model.ItemTypeId).First();
                ItemType.ItemTypeName = model.ItemTypeName;
                ItemType.ItemTypeStatus = model.ItemTypeStatus;
                if (ItemType.ItemTypeId == 0)
                    dataContext.ItemTypes.InsertOnSubmit(ItemType);
                dataContext.SubmitChanges();
                ResponseMessage.ItemTypeList = StoreData.ItemTypeList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Item Type is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteItemType")]
        public ExpandoObject DeleteItemType(int ItemTypeId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var ItemType = dataContext.ItemTypes.Where(x => x.ItemTypeId == ItemTypeId).First();
                dataContext.ItemTypes.DeleteOnSubmit(ItemType);
                dataContext.SubmitChanges();
                ResponseMessage.ItemTypeList = StoreData.ItemTypeList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Item Type is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
