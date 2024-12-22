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
    [RoutePrefix("api/PurchaseBook")]
    public class PurchaseBookController : ApiController
    {

        [HttpGet]
        [Route("PurchaseBookList")]
        public ExpandoObject PurchaseBookList(int PurchaseId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.PurchaseBooks
                           join b1 in dataContext.Books
                           on p1.BookId equals b1.BookId
                           where p1.PurchaseId == PurchaseId
                           select new
                           {
                               b1.BookName,
                               b1.Author,
                               b1.Edition,
                               b1.Publisher,
                               b1.PublishingYear,
                               ClassName = b1.ClassNo.HasValue ? dataContext.Classes.First(x => x.ClassNo == b1.ClassNo).ClassName2 : "",
                               p1.BookPrice,
                               p1.Quantity,
                               p1.GrossAmount,
                               p1.DiscountRate,
                               p1.DiscountAmount,
                               p1.TotalAmount,
                               p1.BookCost
                           };

                response.PurchaseBookList = list.ToList();
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
