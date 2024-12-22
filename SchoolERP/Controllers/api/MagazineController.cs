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
    [RoutePrefix("api/Magazine")]
    public class MagazineController : ApiController
    {
        [HttpGet]
        [Route("MagazineList")]
        public ExpandoObject MagazineList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from m1 in dataContext.Magazines
                           where (Status == 0 || Status == m1.MagazineStatus)
                           select new
                           {
                               m1.MagazineId,
                               m1.MagazineName,
                               m1.MagazineStatus,
                           };

                ResponseMessage.MagazineList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveMagazine")]
        public ExpandoObject SaveMagazine(MagazineModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Magazine Magazine = new Magazine();
                if (model.MagazineId > 0)
                    Magazine = dataContext.Magazines.Where(x => x.MagazineId == model.MagazineId).First();
                Magazine.MagazineName = model.MagazineName;
                Magazine.MagazineStatus = model.MagazineStatus;
                if (Magazine.MagazineId == 0)
                    dataContext.Magazines.InsertOnSubmit(Magazine);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Magazine is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteMagazine")]
        public ExpandoObject DeleteMagazine(int MagazineId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Magazine = dataContext.Magazines.Where(x => x.MagazineId == MagazineId).First();
                dataContext.Magazines.DeleteOnSubmit(Magazine);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Magazine is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
