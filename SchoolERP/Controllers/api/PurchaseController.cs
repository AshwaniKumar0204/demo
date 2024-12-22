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

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/Purchase")]
    public class PurchaseController : ApiController
    {
        [HttpPost]
        [Route("SavePurchase")]
        public ExpandoObject SavePurchase(PurchaseModel model)
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

                Purchase Purchase = new Purchase
                {
                    SupplierId = model.SupplierId,
                    BillDate = model.BillDate,
                    BillNo = model.BillNo,
                    PurchaseType = model.PurchaseBookList != null ? (byte)PurchaseType.Library : (byte)PurchaseType.Store,
                    Remarks = model.Remarks,
                    TotalAmount = model.PurchaseBookList != null ? model.PurchaseBookList.Sum(x => x.TotalAmount) : model.PurchaseStoreItemList.Sum(x => x.TaxableAmount),
                    TotalCGST = model.PurchaseStoreItemList != null ? model.PurchaseStoreItemList.Sum(x => x.CGSTAmount) : 0,
                    TotalSGST = model.PurchaseStoreItemList != null ? model.PurchaseStoreItemList.Sum(x => x.SGSTAmount) : 0,
                    TotalIGST = model.PurchaseStoreItemList != null ? model.PurchaseStoreItemList.Sum(x => x.IGSTAmount) : 0,
                    PurchaseStatus = (byte)PurcahseStatus.Active,
                    CreatedBy = loginId,
                    CreatedOn = DateTime.Now,
                };
                Purchase.GrandTotal = Purchase.TotalAmount + Purchase.TotalCGST + Purchase.TotalSGST + Purchase.TotalIGST;
                dataContext.Purchases.InsertOnSubmit(Purchase);
                dataContext.SubmitChanges();

                if (model.PurchaseBookList != null)
                {
                    model.PurchaseBookList.ForEach(x =>
                    {
                        PurchaseBook purchaseBook = new PurchaseBook
                        {
                            PurchaseId = Purchase.PurchaseId,
                            BookId = x.BookId,
                            DiscountAmount = x.DiscountAmount,
                            DiscountRate = x.DiscountRate,
                            GrossAmount = x.GrossAmount,
                            BookPrice = x.BookPrice,
                            Quantity = x.Quantity,
                            Remarks = x.Remarks,
                            TotalAmount = x.TotalAmount,
                            BookCost = x.TotalAmount / x.Quantity,
                        };
                        dataContext.PurchaseBooks.InsertOnSubmit(purchaseBook);
                        dataContext.SubmitChanges();

                        for (var i = 0; i < x.Quantity; i++)
                        {
                            BookAccession bookAccession = new BookAccession
                            {
                                AccessionNo = LoadData.GenerateAccessionCode(dataContext, x.ClassNo),
                                BookAccessionStatus = (byte)BookAccessionStatus.Available,
                                BookId = x.BookId,
                                PurchaseBookId = purchaseBook.PurchaseBookId,
                                CreatedOn = DateTime.Now,
                                CreatedBy = loginId,
                            };
                            dataContext.BookAccessions.InsertOnSubmit(bookAccession);
                            dataContext.SubmitChanges();
                        }
                    });
                }

                if (model.PurchaseStoreItemList != null)
                {
                    model.PurchaseStoreItemList.ForEach(x =>
                    {
                        PurchaseStoreItem purchaseStoreItem = new PurchaseStoreItem
                        {
                            CGSTAmount = x.CGSTAmount,
                            GSTId = x.GSTId,
                            IGSTAmount = x.IGSTAmount,
                            LineTotal = x.LineTotal,
                            PurchaseId = Purchase.PurchaseId,
                            Quantity = x.Quantity,
                            Rate = x.Rate,
                            DiscountAmount = x.DiscountAmount,
                            SGSTAmount = x.SGSTAmount,
                            StoreItemId = x.StoreItemId,
                            TaxableAmount = x.TaxableAmount,
                        };
                        dataContext.PurchaseStoreItems.InsertOnSubmit(purchaseStoreItem);
                        dataContext.SubmitChanges();

                        var storeItem = dataContext.StoreItems.Where(y => y.StoreItemId == x.StoreItemId).First();
                        storeItem.Quantity += x.Quantity;
                        dataContext.SubmitChanges();
                    });
                }
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
        [Route("PurchaseList")]
        public ExpandoObject PurchaseList(DateTime? fromDate, DateTime? toDate, int SupplierId, byte PurchaseType)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.Purchases
                           join s1 in dataContext.Suppliers
                           on p1.SupplierId equals s1.SupplierId
                           join s2 in dataContext.SystemLogins
                           on p1.CreatedBy equals s2.LoginId into createdByList
                           from createdBy in createdByList.DefaultIfEmpty()
                           join s3 in dataContext.SystemLogins
                           on p1.LastUpdatedBy equals s3.LoginId into lastUpdatedByList
                           from lastUpdatedBy in lastUpdatedByList.DefaultIfEmpty()
                           where (SupplierId == 0 ? true : s1.SupplierId == SupplierId)
                           && (fromDate.HasValue ? fromDate.Value.Date <= p1.BillDate.Date : true)
                           && (toDate.HasValue ? toDate.Value.Date >= p1.BillDate.Date : true)
                           && (PurchaseType == 0 || p1.PurchaseType == PurchaseType)
                           select new
                           {
                               s1.SupplierId,
                               s1.SupplierName,
                               p1.BillDate,
                               p1.BillNo,
                               p1.TotalAmount,
                               p1.TotalCGST,
                               p1.TotalIGST,
                               p1.TotalSGST,
                               p1.PurchaseId,
                               CreatedByName = createdBy.UserName,
                               p1.CreatedOn,
                               LastUpdatedByName = lastUpdatedBy.UserName,
                               p1.LastUpdatedOn,
                               p1.GrandTotal,
                               p1.PurchaseStatus
                           };

                response.PurchaseList = list.ToList();
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
