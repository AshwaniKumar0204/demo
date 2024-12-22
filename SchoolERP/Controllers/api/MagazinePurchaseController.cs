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
    [RoutePrefix("api/MagazinePurchase")]
    public class MagazinePurchaseController : ApiController
    {
        [HttpGet]
        [Route("MagazinePurchaseList")]
        public ExpandoObject MagazinePurchaseList(int MagazineId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from m1 in dataContext.MagazinePurchases
                           join m2 in dataContext.Magazines
                           on m1.MagazineId equals m2.MagazineId
                           where (MagazineId == 0 || m1.MagazineId == MagazineId)
                           select new
                           {
                               m1.MagazinePurchaseId,
                               m1.MagazineId,
                               m1.PublishDate,
                               m1.ReceiveDate,
                               m1.Remarks,
                               m1.Amount,
                               m1.Source,
                               m2.MagazineName,
                               m1.Medium,
                               MediumName = Enum.GetName(typeof(Medium), m1.Medium),
                           };
                ResponseMessage.MagazinePurchaseList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveMagazinePurchase")]
        public ExpandoObject SaveMagazinePurchase(MagazinePurchaseModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                MagazinePurchase MagazinePurchase = null;
                if (model.MagazinePurchaseId > 0)
                {
                    MagazinePurchase = dataContext.MagazinePurchases.Where(x => x.MagazinePurchaseId == model.MagazinePurchaseId).First();
                    MagazinePurchase.LastUpdatedBy = SessionUtilies.GetLoginId();
                    MagazinePurchase.LastUpdatedOn = DateTime.Now;
                }
                else
                    MagazinePurchase = new MagazinePurchase
                    {
                        CreatedBy = SessionUtilies.GetLoginId(),
                        CreatedOn = DateTime.Now
                    };
                MagazinePurchase.MagazineId = model.MagazineId;
                MagazinePurchase.PublishDate = model.PublishDate;
                MagazinePurchase.ReceiveDate = model.ReceiveDate;
                MagazinePurchase.Medium = model.Medium;
                MagazinePurchase.Source = model.Source;
                MagazinePurchase.Amount = model.Amount;
                MagazinePurchase.Remarks = model.Remarks;
                if (MagazinePurchase.MagazinePurchaseId == 0)
                    dataContext.MagazinePurchases.InsertOnSubmit(MagazinePurchase);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Magazine Purchase is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteMagazinePurchase")]
        public ExpandoObject DeleteMagazinePurchase(int MagazinePurchaseId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var MagazinePurchase = dataContext.MagazinePurchases.Where(x => x.MagazinePurchaseId == MagazinePurchaseId).First();
                dataContext.MagazinePurchases.DeleteOnSubmit(MagazinePurchase);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Magazine Purchase is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
