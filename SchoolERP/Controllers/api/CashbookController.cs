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
    [RoutePrefix("api/cashbook")]
    public class CashbookController : ApiController
    {
        [HttpGet]
        [Route("cashbookDetail")]
        public ExpandoObject CashbookDetail(string PaymenDate)
        {
            dynamic response = new ExpandoObject();
            try
            {
                DateTime cashbookDate = LoadData.CheckDate(PaymenDate, "Invalid Cashbook Date !");
                SchoolDataContext dataContext = new SchoolDataContext();
                DateTime lastCashbookDate = dataContext.Cashbooks.OrderByDescending(x => x.CashbookDate).First().CashbookDate;
                if (dataContext.Cashbooks.Where(x => x.CashbookDate.Date == cashbookDate.Date).Any())
                {
                    response.Message = "Exist";
                    return response;
                }
                else if (dataContext.Cashbooks.Any())
                {
                    if (lastCashbookDate.Date > cashbookDate.Date)
                    {
                        response.Message = "Date should be more than " + lastCashbookDate.ToString("dd-MM-yyy");
                        return response;
                    }
                }
                var FeeCollectionList = (from fc1 in dataContext.FeeCollections
                                         join fcd in dataContext.FeeCollectionDetails
                                         on fc1.FeeCollectionId equals fcd.FeeCollectionId
                                         join s1 in dataContext.SystemLogins
                                         on fc1.CreatedBy equals s1.LoginId
                                         where fc1.PaymentDate.Date > lastCashbookDate.Date
                                         && fc1.PaymentDate.Date <= cashbookDate.Date
                                         && fc1.FeeCollectionStatus != (byte)FeeCollectionStatus.Cancelled
                                         && fc1.PaymentMode == (byte)PaymentMode.Cash
                                         select new { fcd.FeeHeadTitle, fcd.FeeHeadId, fc1.PaymentMode, fcd.Total, fc1.CreatedBy, s1.UserName });

                List<CashbookModel> cashbookList = new List<CashbookModel>();
                FeeCollectionList.GroupBy(x => x.CreatedBy).ToList().ForEach(x =>
                {
                    var cashbookDetails = (from c1 in dataContext.Cashbooks
                                           where c1.CashbookDate.Date < cashbookDate.Date
                                           && c1.LoginId == x.First().CreatedBy
                                           orderby c1.CashbookDate descending
                                           select c1);
                    decimal BalanceDB = 0, TodayCash = x.Sum(y => y.Total ?? 0);
                    if (cashbookDetails.Any())
                        BalanceDB = cashbookDetails.First().ClosingAmount;
                    cashbookList.Add(new CashbookModel
                    {
                        TodayCashAmount = TodayCash,
                        OpeningAmount = BalanceDB,
                        TotalCashAmount = TodayCash + BalanceDB,
                        DepositAmount = 0,
                        LoginId = x.First().CreatedBy ?? 0,
                        UserName = x.First().UserName,
                        PaymentMode = (byte)PaymentMode.Cash,
                        CashbookDate = cashbookDate,
                        ClosingAmount = TodayCash + BalanceDB
                    });
                });

                //Other Users
                var loginIds = cashbookList.Select(x => x.LoginId);
                var otherCashbooks = (from c1 in dataContext.Cashbooks
                                      where c1.CashbookDate.Date < cashbookDate.Date
                                      && !loginIds.Contains(c1.LoginId)
                                      select c1).GroupBy(x => x.LoginId).ToList();
                otherCashbooks.ForEach(cashbooks =>
                {
                    var cashbook = cashbooks.OrderByDescending(x => x.CashbookDate).First();
                    cashbookList.Add(new CashbookModel
                    {
                        TodayCashAmount = 0,
                        OpeningAmount = cashbook.ClosingAmount,
                        TotalCashAmount = cashbook.ClosingAmount,
                        DepositAmount = 0,
                        LoginId = cashbook.LoginId,
                        UserName = dataContext.SystemLogins.Where(x => x.LoginId == cashbook.LoginId).First().UserName,
                        PaymentMode = (byte)PaymentMode.Cash,
                        CashbookDate = cashbookDate,
                        ClosingAmount = cashbook.ClosingAmount
                    });
                });
                response.CashbookDetailList = cashbookList;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveCashbook")]
        public ExpandoObject SaveCashbook(List<CashbookModel> cashbookList)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (dataContext.Cashbooks.Where(x => x.CashbookDate.Date == cashbookList.First().CashbookDate.Date).Any())
                {
                    response.Message = "Cashbook is already completed.";
                    return response;
                }
                cashbookList.ForEach(cashDetail =>
                {
                    Cashbook cashbook = new Cashbook
                    {
                        CashbookDate = cashDetail.CashbookDate,
                        ClosingAmount = cashDetail.ClosingAmount,
                        DepositAmount = cashDetail.DepositAmount,
                        LoginId = cashDetail.LoginId,
                        OpeningAmount = cashDetail.OpeningAmount
                    };
                    dataContext.Cashbooks.InsertOnSubmit(cashbook);
                    dataContext.SubmitChanges();
                });
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
