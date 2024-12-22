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
    [RoutePrefix("api/chequebook")]
    public class ChequebookController : ApiController
    {
        //NOTE:
        //This entire controller has been copied from ChequebookController and hence all the variables named as 'chequebook' actually refer to the entity 
        //'checkbook' for the sake of saving time while coding and are supposed to be changed to 'checkbook' later on for better readability.

        [HttpGet]
        [Route("chequebookDetail")]
        public ExpandoObject ChequebookDetail(string PaymenDate)
        {
            dynamic response = new ExpandoObject();
            try
            {
                DateTime chequebookDate = LoadData.CheckDate(PaymenDate, "Invalid Chequebook Date !");
                SchoolDataContext dataContext = new SchoolDataContext();
                //DateTime lastChequebookDate = dataContext.Chequebooks.OrderByDescending(x => x.ChequebookDate).First().ChequebookDate;
                DateTime lastChequebookDate = new DateTime(1800,1,1);
                if (dataContext.Chequebooks.Where(x => x.ChequebookDate.Date == chequebookDate.Date).Any())
                {
                    response.Message = "Exist";
                    return response;
                }
                else if (dataContext.Chequebooks.Any())
                {
                    lastChequebookDate = dataContext.Chequebooks.OrderByDescending(x => x.ChequebookDate).First().ChequebookDate;
                    if (lastChequebookDate.Date > chequebookDate.Date)
                    {
                        response.Message = "Date should be more than " + lastChequebookDate.ToString("dd-MM-yyy");
                        return response;
                    }
                }
                var FeeCollectionList = (from fc1 in dataContext.FeeCollections
                                         join fcd in dataContext.FeeCollectionDetails
                                         on fc1.FeeCollectionId equals fcd.FeeCollectionId
                                         join s1 in dataContext.SystemLogins
                                         on fc1.CreatedBy equals s1.LoginId
                                         where fc1.PaymentDate.Date > lastChequebookDate.Date
                                         && fc1.PaymentDate.Date <= chequebookDate.Date
                                         && fc1.FeeCollectionStatus != (byte)FeeCollectionStatus.Cancelled
                                         && fc1.PaymentMode == (byte)PaymentMode.Cheque
                                         select new { fcd.FeeHeadTitle, fcd.FeeHeadId, fc1.PaymentMode, fcd.Total, fc1.CreatedBy, s1.UserName });
                //List<FeeHeadModel> feeHeadList = new List<FeeHeadModel>();
                //FeeCollectionList.GroupBy(x => x.FeeHeadId).ToList().ForEach(x => feeHeadList.Add(new FeeHeadModel
                //{
                //    FeeHeadId = x.First().FeeHeadId ?? 0,
                //    FeeHeadTitle = x.First().FeeHeadTitle,
                //    Amount = x.Sum(y => y.Total ?? 0)
                //}));

                List<ChequebookModel> chequebookList = new List<ChequebookModel>();
                FeeCollectionList.GroupBy(x => x.CreatedBy).ToList().ForEach(x =>
                {
                    var chequebookDetails = (from c1 in dataContext.Chequebooks
                                           where c1.ChequebookDate.Date < chequebookDate.Date
                                           && c1.LoginId == x.First().CreatedBy
                                           orderby c1.ChequebookDate descending
                                           select c1);
                    decimal BalanceDB = 0, TodayCheque = x.Sum(y => y.Total ?? 0);
                    if (chequebookDetails.Any())
                        BalanceDB = chequebookDetails.First().ClosingAmount;
                    chequebookList.Add(new ChequebookModel
                    {
                        TodayChequeAmount = TodayCheque,
                        OpeningAmount = BalanceDB,
                        TotalChequeAmount = TodayCheque + BalanceDB,
                        DepositAmount = 0,
                        LoginId = x.First().CreatedBy ?? 0,
                        UserName = x.First().UserName,
                        PaymentMode = (byte)PaymentMode.Cheque,
                        ChequebookDate = chequebookDate,
                        ClosingAmount = TodayCheque + BalanceDB
                    });
                });

                //Other Users
                var loginIds = chequebookList.Select(x => x.LoginId);
                var otherChequebooks = (from c1 in dataContext.Chequebooks
                                      where c1.ChequebookDate.Date < chequebookDate.Date
                                      && !loginIds.Contains(c1.LoginId)
                                      select c1).GroupBy(x => x.LoginId).ToList();
                otherChequebooks.ForEach(chequebooks =>
                {
                    var chequebook = chequebooks.OrderByDescending(x => x.ChequebookDate).First();
                    chequebookList.Add(new ChequebookModel
                    {
                        TodayChequeAmount = 0,
                        OpeningAmount = chequebook.ClosingAmount,
                        TotalChequeAmount = chequebook.ClosingAmount,
                        DepositAmount = 0,
                        LoginId = chequebook.LoginId,
                        UserName = dataContext.SystemLogins.Where(x => x.LoginId == chequebook.LoginId).First().UserName,
                        PaymentMode = (byte)PaymentMode.Cheque,
                        ChequebookDate = chequebookDate,
                        ClosingAmount = chequebook.ClosingAmount
                    });
                });
                response.ChequebookDetailList = chequebookList;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveChequebook")]
        public ExpandoObject SaveChequebook(List<ChequebookModel> chequebookList)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (dataContext.Chequebooks.Where(x => x.ChequebookDate.Date == chequebookList.First().ChequebookDate.Date).Any())
                {
                    response.Message = "Chequebook is already completed.";
                    return response;
                }
                chequebookList.ForEach(chequeDetail =>
                {
                    Chequebook chequebook = new Chequebook
                    {
                        ChequebookDate = chequeDetail.ChequebookDate,
                        ClosingAmount = chequeDetail.ClosingAmount,
                        DepositAmount = chequeDetail.DepositAmount,
                        LoginId = chequeDetail.LoginId,
                        OpeningAmount = chequeDetail.OpeningAmount
                    };
                    dataContext.Chequebooks.InsertOnSubmit(chequebook);
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
