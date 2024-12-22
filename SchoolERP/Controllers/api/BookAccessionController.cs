using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SchoolERP.Controllers
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/BookAccession")]
    public class BookAccessionController : ApiController
    {
        [HttpPost]
        [Route("BookAccessionList")]
        public ExpandoObject BookAccessionList(DateFilterModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                DateTime fromDate = model.FromDate.HasValue ? model.FromDate.Value.Date : DateTime.Now.Date;
                DateTime toDate = model.ToDate.HasValue ? model.ToDate.Value.Date : DateTime.Now.Date;
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.BookAccessions
                           join b1 in dataContext.Books
                           on p1.BookId equals b1.BookId
                           join p31 in dataContext.PurchaseBooks
                           on p1.PurchaseBookId equals p31.PurchaseBookId into subPurchaseBooks
                           from p3 in subPurchaseBooks.DefaultIfEmpty()
                           join p41 in dataContext.Purchases
                           on p3.PurchaseId equals p41.PurchaseId into subPurchase
                           from p4 in subPurchase.DefaultIfEmpty()
                           join s11 in dataContext.Suppliers
                           on p4.SupplierId equals s11.SupplierId into subSuppliers
                           join cl in dataContext.Classes
                           on b1.ClassNo equals cl.ClassNo into subClasses
                           join cl2 in dataContext.Classes
                           on b1.ClassNo2 equals cl2.ClassNo into subClasses2
                           where (model.ClassNo == 0 ? true : (b1.ClassNo == model.ClassNo || b1.ClassNo2 == model.ClassNo))
                           && (model.Status == 0 ? true : p1.BookAccessionStatus == model.Status)
                           && (model.FromDate.HasValue ? p1.CreatedOn >= fromDate : true)
                           && (model.ToDate.HasValue ? p1.CreatedOn <= toDate : true)
                           select new BookModel
                           {
                               BookName = b1.BookName,
                               Author = b1.Author,
                               Edition = b1.Edition,
                               Publisher = b1.Publisher,
                               PublishingYear = b1.PublishingYear,
                               ClassName = (subClasses.Any() ? subClasses.First().ClassName2 : "") + (subClasses2.Any() ? ", " + subClasses2.First().ClassName2 : ""),
                               BookPrice = p3.BookPrice,
                               AccessionNo = p1.AccessionNo,
                               CreatedOn = p1.CreatedOn,
                               TotalPages = b1.TotalPages,
                               Volume = b1.Volume,
                               SupplierName = subSuppliers.Any() ? subSuppliers.First().SupplierName : "",
                               BillNo = p4.BillNo,
                               BillDate = p4.BillDate,
                               BookCost = p3.BookCost,
                               Remarks = p3.Remarks,
                               SubjectType = b1.SubjectType,
                               BookAccessionStatus = p1.BookAccessionStatus,
                               BookAccessionStatusName = Enum.GetName(typeof(BookAccessionStatus), p1.BookAccessionStatus),
                               BookPosition = p1.BookPosition,
                               BookPositionName = p1.BookPosition.HasValue ? Enum.GetName(typeof(BookPosition), p1.BookPosition.Value) : "",
                           };
                response.BookAccessionList = list.Distinct().OrderBy(x => x.AccessionNo).ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("SearchBookAccessionList")]
        public ExpandoObject SearchBookAccessionList(byte BookStatus, byte BookAccessionStatus)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.Books
                           join b2 in dataContext.BookAccessions
                           on b1.BookId equals b2.BookId
                           join cl in dataContext.Classes
                           on b1.ClassNo equals cl.ClassNo into subClasses
                           from c1 in subClasses.DefaultIfEmpty()
                           where (BookStatus == 0 || b1.BookStatus == BookStatus)
                           && (BookAccessionStatus == 0 || b2.BookAccessionStatus == BookAccessionStatus)
                           select new
                           {
                               b2.BookAccessionId,
                               b2.AccessionNo,
                               b1.BookName
                           };
                response.BookAccessionList = list.Distinct().OrderBy(x => x.AccessionNo).ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("BookAccessionDetail")]
        public ExpandoObject BookAccessionDetail(string AccessionNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.BookAccessions
                           join b1 in dataContext.Books
                           on p1.BookId equals b1.BookId
                           join p31 in dataContext.PurchaseBooks
                           on p1.PurchaseBookId equals p31.PurchaseBookId into subPurchaseBooks
                           from p3 in subPurchaseBooks.DefaultIfEmpty()
                           join p41 in dataContext.Purchases
                           on p3.PurchaseId equals p41.PurchaseId into subPurchase
                           from p4 in subPurchase.DefaultIfEmpty()
                           join s11 in dataContext.Suppliers
                           on p4.SupplierId equals s11.SupplierId into subSuppliers
                           join cl in dataContext.Classes
                           on b1.ClassNo equals cl.ClassNo into subClasses
                           from c1 in subClasses.DefaultIfEmpty()
                           join cl2 in dataContext.Classes
                           on b1.ClassNo2 equals cl2.ClassNo into subClasses2
                           where p1.AccessionNo == AccessionNo
                           select new BookModel
                           {
                               BookAccessionId = p1.BookAccessionId,
                               BookName = b1.BookName,
                               BookId = b1.BookId,
                               Author = b1.Author,
                               Edition = b1.Edition,
                               Publisher = b1.Publisher,
                               PublishingYear = b1.PublishingYear,
                               ClassName = (subClasses.Any() ? subClasses.First().ClassName2 : "") + (subClasses2.Any() ? ", " + subClasses2.First().ClassName2 : ""),
                               BookPrice = p3.BookPrice,
                               AccessionNo = p1.AccessionNo,
                               CreatedOn = p1.CreatedOn,
                               TotalPages = b1.TotalPages,
                               Volume = b1.Volume,
                               SupplierName = subSuppliers.Any() ? subSuppliers.First().SupplierName : "",
                               BillNo = p4.BillNo,
                               BillDate = p4.BillDate,
                               BookCost = p3.BookCost,
                               Remarks = p3.Remarks,
                               SubjectType = b1.SubjectType,
                               BookAccessionStatus = p1.BookAccessionStatus,
                               BookAccessionStatusName = Enum.GetName(typeof(BookAccessionStatus), p1.BookAccessionStatus),
                               BookPosition = p1.BookPosition,
                               BookPositionName = p1.BookPosition.HasValue ? Enum.GetName(typeof(BookPosition), p1.BookPosition.Value) : "",
                           };
                if (list.Count() == 0)
                    throw new Exception("Invalid Accession No.");
                response.BookAccession = list.First();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveBookAccession")]
        public ExpandoObject SaveBookAccession(BookAccessionModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                BookAccession BookAccession = null;
                if (model.BookAccessionId > 0)
                {
                    BookAccession = dataContext.BookAccessions.Where(x => x.BookAccessionId == model.BookAccessionId).First();
                    BookAccession.LastUpdatedBy = SessionUtilies.GetLoginId();
                    BookAccession.LastUpdatedOn = DateTime.Now;
                }
                else
                    BookAccession = new BookAccession
                    {
                        CreatedBy = SessionUtilies.GetLoginId(),
                        CreatedOn = DateTime.Now,
                    };
                BookAccession.BookPosition = model.BookPosition;
                BookAccession.BookId = model.BookId;
                //BookAccession.BookAccessionStatus = model.BookAccessionStatus;
                // BookAccession.AccessionNo = model.AccessionNo;
                if (BookAccession.BookAccessionId == 0)
                    dataContext.BookAccessions.InsertOnSubmit(BookAccession);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Accession No. is already exist.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("BookAccessionListWithRack")]
        public ExpandoObject BookAccessionListWithRack(int AccessionNo)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.BookAccessions
                           join b1 in dataContext.Books
                           on p1.BookId equals b1.BookId
                           join cl in dataContext.Classes
                           on b1.ClassNo equals cl.ClassNo into subClasses
                           join cl2 in dataContext.Classes
                           on b1.ClassNo2 equals cl2.ClassNo into subClasses2
                           join b2 in dataContext.BookTypes
                           on b1.BookTypeId equals b2.BookTypeId into subBookTypes
                           from b3 in subBookTypes.DefaultIfEmpty()
                           join r1 in dataContext.BookSubjectRacks
                           on b1.BookSubjectId equals r1.BookSubjectId into bookRacks
                           where (AccessionNo == 0 ? true : p1.BookAccessionId == AccessionNo)
                           select new
                           {
                               b1.BookName,
                               b1.Author,
                               b1.Edition,
                               b1.Publisher,
                               ClassName = (subClasses.Any() ? subClasses.First().ClassName2 : "") + (subClasses2.Any() ? ", " + subClasses2.First().ClassName2 : ""),
                               p1.AccessionNo,
                               b1.TotalPages,
                               b1.Volume,
                               b1.SubjectType,
                               Racks = string.Join(",", (from r1 in bookRacks
                                                            join r2 in dataContext.Racks
                                                            on r1.RackId equals r2.RackId
                                                            where (r1.ClassNo == b1.ClassNo || (r1.ClassNo == b1.ClassNo2) || r1.ClassNo == null)
                                                            select r2.RackName).ToList())
                           };
                ResponseMessage.BookAccessionListWithRack = list.Distinct().OrderBy(x => x.AccessionNo).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

       


    }
}
