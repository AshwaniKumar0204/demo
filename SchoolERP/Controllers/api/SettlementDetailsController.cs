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
    [RoutePrefix("api/SettlementDetails")]
    public class SettlementDetailsController : ApiController
    {

      /*  [HttpPost]
        [Route("GetSettlementDetails")]
        public ExpandoObject GetSettlementDetails(SDRequest model)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                if (model.ToDate.Date >= DateTime.Now.Date)
                    model.ToDate = DateTime.Now.Date.AddDays(-1);

                if (model.FromDate.Date > model.ToDate.Date)
                {
                    IsCompleted = true;
                    throw new Exception("From Date Must be Less than To Date.");
                }
                DateTime? nullDate = null;
                List<SettlementRoot> settlementRoots = new List<SettlementRoot>();
                SchoolDataContext schoolDataContext = new SchoolDataContext();
                for (DateTime date = model.FromDate.Date; date <= model.ToDate.Date; date = date.AddDays(1))
                {
                    var exists = (schoolDataContext.Settlements.Any(x => x.Dated.Value.Date == date.Date));
                    if (!exists)
                    {
                        SettlementRoot settlementRoot = PayUApi.SettlementDetails(date.ToString("yyyy-MM-dd"));
                        settlementRoots.Add(settlementRoot);
                    }
                }
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                if (settlementRoots.Count > 0)
                {

                    foreach (SettlementRoot settlementRoot in settlementRoots)
                    {
                        Settlement settlement = new Settlement();
                        settlement.Dated = Convert.ToDateTime(settlementRoot.date);
                        if (settlementRoot.Txn_details.Any())
                        {
                            settlement.SalesAmount = settlementRoot.Txn_details.Sum(x => x.amountDecimal);
                            //settlement.RefundAmount = settlementRoot.Txn_details.Where(x => x.requestaction == "refund").Sum(x => x.mer_net_amount_decimal);
                            settlement.CapturedAmount = settlementRoot.Txn_details.Where(x => x.requestaction == "capture").Sum(x => x.mer_net_amount_decimal);
                            settlement.ServiceFee = settlementRoot.Txn_details.Sum(x => x.mer_service_fee_decimal);
                            settlement.ServiceFeeGST = settlementRoot.Txn_details.Sum(x => x.mer_service_tax_decimal);

                            settlement.Chargeback = settlementRoot.Txn_details.Where(x => x.requestaction == "chargeback").Sum(x => x.mer_net_amount_decimal);
                            settlement.ChargebackReturned = settlementRoot.Txn_details.Where(x => x.requestaction == "chargebackreversal").Sum(x => x.mer_net_amount_decimal);
                            //chargeback return pending
                            //refund return pending
                            //adjustment pending
                            settlement.SettlementAmount = settlementRoot.Txn_details.Sum(x => x.mer_net_amount_decimal);
                            settlement.UtrNumber = settlementRoot.Txn_details[0].mer_utr;
                            settlement.TotalTransactions = settlementRoot.Txn_details.Count;
                        }
                        settlement.FetchDate = DateTime.Now;
                        dataContext.Settlements.InsertOnSubmit(settlement);
                        dataContext.SubmitChanges();



                        foreach (TxnDetail txnDetail in settlementRoot.Txn_details)
                        {
                            SettlementDetail settlementDetail = new SettlementDetail();
                            settlementDetail.SettlementId = settlement.SettlementId;
                            settlementDetail.PayUId = txnDetail.payuid;
                            settlementDetail.MerchantTransactionId = txnDetail.txnid;
                            settlementDetail.TransactionDate = String.IsNullOrEmpty(txnDetail.txndate) ? nullDate : Convert.ToDateTime(txnDetail.txndate);
                            settlementDetail.Mode = txnDetail.mode;
                            settlementDetail.Amount = txnDetail.amountDecimal;
                            settlementDetail.RequestId = txnDetail.requestid;
                            settlementDetail.RequestDate = String.IsNullOrEmpty(txnDetail.requestdate) ? Convert.ToDateTime(txnDetail.requestdate) : nullDate;
                            settlementDetail.RequestAction = txnDetail.requestaction;
                            settlementDetail.RequestAmount = txnDetail.requestamount_decimal;
                            settlementDetail.MerchantURTNumber = txnDetail.mer_utr;
                            settlementDetail.MerchantURTNumber = txnDetail.mer_utr;
                            settlementDetail.MerchantServiceFee = txnDetail.mer_service_fee_decimal;
                            settlementDetail.MerchantServiceTax = txnDetail.mer_service_tax_decimal;
                            settlementDetail.MerchantNetAmount = txnDetail.mer_net_amount_decimal;
                            settlementDetail.AmountDec = txnDetail.amountDec_decimal;
                            settlementDetail.BankName = txnDetail.bank_name;
                            settlementDetail.IssuingBank = txnDetail.issuing_bank;
                            settlementDetail.MerchantSubventionAmount = txnDetail.merchant_subvention_amount;
                            settlementDetail.SGST = txnDetail.sgst_decimal;
                            settlementDetail.CGST = txnDetail.cgst_decimal;
                            settlementDetail.IGST = txnDetail.igst_decimal;
                            settlementDetail.PaymentGatewayType = txnDetail.PG_TYPE;
                            settlementDetail.CardType = txnDetail.CardType;
                            dataContext.SettlementDetails.InsertOnSubmit(settlementDetail);
                            dataContext.SubmitChanges();
                        }
                    }
                    transaction.Commit();
                    IsCompleted = true;
                }

                SchoolDataContext schoolDataContext1 = new SchoolDataContext();
                var settlements = (from trn in schoolDataContext1.Settlements
                                   where trn.Dated >= model.FromDate && trn.Dated < model.ToDate.AddDays(1)
                                   orderby trn.Dated descending
                                   select new SettlementModel { SettlementId = trn.SettlementId, Dated = trn.Dated, CapturedAmount = trn.CapturedAmount, SalesAmount = trn.SalesAmount, RefundAmount = trn.RefundAmount, ServiceFee = trn.ServiceFee, ServiceFeeGST = trn.ServiceFeeGST, Chargeback = trn.Chargeback, ChargebackReturned = trn.ChargebackReturned, RefundReturn = trn.RefundReturn, Adjustment = trn.Adjustment, SettlementAmount = trn.SettlementAmount, UtrNumber = trn.UtrNumber, TotalTransaction = trn.TotalTransactions }).ToList();
                response.Settlements = settlements;

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
        [Route("GetTransactionList")]
        public ExpandoObject GetTransactionList(int SettlementId)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            try
            {

                SchoolDataContext schoolDataContext1 = new SchoolDataContext();
                var settlements = (from trn in schoolDataContext1.SettlementDetails
                                   join fc in schoolDataContext1.FeeCollections
                                   on trn.PayUId equals fc.TransactionNo into subFeeColl
                                   from feeColl in subFeeColl.DefaultIfEmpty()

                                   join adm in schoolDataContext1.Admissions
                                   on feeColl.AdmissionId equals adm.AdmissionId into subAdmissionColl
                                   from sAdm in subAdmissionColl.DefaultIfEmpty()
                                   join std in schoolDataContext1.Students
                                   on sAdm.StudentId equals std.StudentId into subStudentColl
                                   from sStd in subStudentColl.DefaultIfEmpty()

                                   join ro in schoolDataContext1.RegistrationOnlines
                                   on feeColl.RegistrationOnlineId equals ro.RegistrationOnlineId into subRegistrationOnlines
                                   from sRegOnline in subRegistrationOnlines.DefaultIfEmpty()

                                   where trn.SettlementId == SettlementId

                                   select new SettlementDetailModel { StudentName = (subStudentColl != null ? sStd.FullName + " (" + sStd.AdmissionNo + ")" : (subRegistrationOnlines != null ? sRegOnline.StudentName + "(" + sRegOnline.RegistrationNo + ")" : "")), ReceiptNo = feeColl.ReceiptNo, SettlementDetailId = trn.SettlementDetailId, SettlementId = trn.SettlementId, MerchantTransactionId = trn.MerchantTransactionId, PayUId = trn.PayUId, TransactionDate = trn.TransactionDate, Mode = trn.Mode, RequestAction = trn.RequestAction, Amount = trn.Amount, MerchantServiceFee = trn.MerchantServiceFee, MerchantServiceTax = trn.MerchantServiceTax, MerchantNetAmount = trn.MerchantNetAmount }).ToList();

                response.TransactionList = settlements;

                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }



        [HttpPost]
        [Route("GetSettlementReceiptDetails")]
        public ExpandoObject GetSettlementReceiptDetails(SDRequest model)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                if (model.ToDate.Date >= DateTime.Now.Date)
                    model.ToDate = DateTime.Now.Date.AddDays(-1);

                if (model.FromDate.Date > model.ToDate.Date)
                {
                    IsCompleted = true;
                    throw new Exception("From Date Must be Less than To Date.");
                }
                DateTime? nullDate = null;
                List<SettlementRoot> settlementRoots = new List<SettlementRoot>();
                SchoolDataContext schoolDataContext = new SchoolDataContext();
                for (DateTime date = model.FromDate.Date; date <= model.ToDate.Date; date = date.AddDays(1))
                {
                    var exists = (schoolDataContext.Settlements.Any(x => x.Dated.Value.Date == date.Date));
                    if (!exists)
                    {
                        SettlementRoot settlementRoot = PayUApi.SettlementDetails(date.ToString("yyyy-MM-dd"));
                        settlementRoots.Add(settlementRoot);
                    }
                }
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                if (settlementRoots.Count > 0)
                {

                    foreach (SettlementRoot settlementRoot in settlementRoots)
                    {
                        Settlement settlement = new Settlement();
                        settlement.Dated = Convert.ToDateTime(settlementRoot.date);
                        if (settlementRoot.Txn_details.Any())
                        {
                            settlement.SalesAmount = settlementRoot.Txn_details.Sum(x => x.amountDecimal);
                            //settlement.RefundAmount = settlementRoot.Txn_details.Where(x => x.requestaction == "refund").Sum(x => x.mer_net_amount_decimal);
                            settlement.CapturedAmount = settlementRoot.Txn_details.Where(x => x.requestaction == "capture").Sum(x => x.mer_net_amount_decimal);
                            settlement.ServiceFee = settlementRoot.Txn_details.Sum(x => x.mer_service_fee_decimal);
                            settlement.ServiceFeeGST = settlementRoot.Txn_details.Sum(x => x.mer_service_tax_decimal);

                            settlement.Chargeback = settlementRoot.Txn_details.Where(x => x.requestaction == "chargeback").Sum(x => x.mer_net_amount_decimal);
                            settlement.ChargebackReturned = settlementRoot.Txn_details.Where(x => x.requestaction == "chargebackreversal").Sum(x => x.mer_net_amount_decimal);
                            //chargeback return pending
                            //refund return pending
                            //adjustment pending
                            settlement.SettlementAmount = settlementRoot.Txn_details.Sum(x => x.mer_net_amount_decimal);
                            settlement.UtrNumber = settlementRoot.Txn_details[0].mer_utr;
                            settlement.TotalTransactions = settlementRoot.Txn_details.Count;
                        }
                        settlement.FetchDate = DateTime.Now;
                        dataContext.Settlements.InsertOnSubmit(settlement);
                        dataContext.SubmitChanges();



                        foreach (TxnDetail txnDetail in settlementRoot.Txn_details)
                        {
                            SettlementDetail settlementDetail = new SettlementDetail();
                            settlementDetail.SettlementId = settlement.SettlementId;
                            settlementDetail.PayUId = txnDetail.payuid;
                            settlementDetail.MerchantTransactionId = txnDetail.txnid;
                            settlementDetail.TransactionDate = String.IsNullOrEmpty(txnDetail.txndate) ? nullDate : Convert.ToDateTime(txnDetail.txndate);
                            settlementDetail.Mode = txnDetail.mode;
                            settlementDetail.Amount = txnDetail.amountDecimal;
                            settlementDetail.RequestId = txnDetail.requestid;
                            settlementDetail.RequestDate = String.IsNullOrEmpty(txnDetail.requestdate) ? Convert.ToDateTime(txnDetail.requestdate) : nullDate;
                            settlementDetail.RequestAction = txnDetail.requestaction;
                            settlementDetail.RequestAmount = txnDetail.requestamount_decimal;
                            settlementDetail.MerchantURTNumber = txnDetail.mer_utr;
                            settlementDetail.MerchantURTNumber = txnDetail.mer_utr;
                            settlementDetail.MerchantServiceFee = txnDetail.mer_service_fee_decimal;
                            settlementDetail.MerchantServiceTax = txnDetail.mer_service_tax_decimal;
                            settlementDetail.MerchantNetAmount = txnDetail.mer_net_amount_decimal;
                            settlementDetail.AmountDec = txnDetail.amountDec_decimal;
                            settlementDetail.BankName = txnDetail.bank_name;
                            settlementDetail.IssuingBank = txnDetail.issuing_bank;
                            settlementDetail.MerchantSubventionAmount = txnDetail.merchant_subvention_amount;
                            settlementDetail.SGST = txnDetail.sgst_decimal;
                            settlementDetail.CGST = txnDetail.cgst_decimal;
                            settlementDetail.IGST = txnDetail.igst_decimal;
                            settlementDetail.PaymentGatewayType = txnDetail.PG_TYPE;
                            settlementDetail.CardType = txnDetail.CardType;
                            dataContext.SettlementDetails.InsertOnSubmit(settlementDetail);
                            dataContext.SubmitChanges();
                        }
                    }
                    transaction.Commit();
                    IsCompleted = true;
                }

                SchoolDataContext schoolDataContext1 = new SchoolDataContext();


                var settlementsOnline = (from trn in schoolDataContext1.SettlementDetails
                                         join fc in schoolDataContext1.FeeCollections
                                         on trn.PayUId equals fc.TransactionNo into subFeeColl
                                         from feeColl in subFeeColl.DefaultIfEmpty()

                                         join stl in schoolDataContext1.Settlements
                                         on trn.SettlementId equals stl.SettlementId

                                         join adm in schoolDataContext1.Admissions
                                         on feeColl.AdmissionId equals adm.AdmissionId into subAdmissionColl
                                         from sAdm in subAdmissionColl.DefaultIfEmpty()
                                         join std in schoolDataContext1.Students
                                         on sAdm.StudentId equals std.StudentId into subStudentColl
                                         from sStd in subStudentColl.DefaultIfEmpty()

                                         join ro in schoolDataContext1.RegistrationOnlines
                                         on feeColl.RegistrationOnlineId equals ro.RegistrationOnlineId into subRegistrationOnlines
                                         from sRegOnline in subRegistrationOnlines.DefaultIfEmpty()

                                         where stl.Dated >= model.FromDate && stl.Dated < model.ToDate.AddDays(1)
                                         orderby stl.Dated ascending

                                         select new SettlementReceiptModel { StudentName = (subStudentColl != null ? sStd.FullName + " (" + sStd.AdmissionNo + ")" : (subRegistrationOnlines != null ? sRegOnline.StudentName + "(" + sRegOnline.RegistrationNo + ")" : "")), ReceiptNo = feeColl.ReceiptNo, SettlementDetailId = trn.SettlementDetailId, SettlementId = trn.SettlementId, MerchantTransactionId = trn.MerchantTransactionId, PayUId = trn.PayUId, TransactionDate = trn.TransactionDate, Mode = trn.Mode, RequestAction = trn.RequestAction, Amount = trn.Amount, MerchantServiceFee = trn.MerchantServiceFee, MerchantServiceTax = trn.MerchantServiceTax, MerchantNetAmount = trn.MerchantNetAmount, MerchantURTNumber = trn.MerchantURTNumber, ReceiptDate = feeColl.PaymentDate, SettlementDate = stl.Dated }).ToList();


                var settlementsReceipt = (from fc in schoolDataContext1.FeeCollections
                                          join trn in schoolDataContext1.SettlementDetails
                                         on fc.TransactionNo equals trn.PayUId into subSettlementDetails
                                          from sSettlement in subSettlementDetails.DefaultIfEmpty()

                                          join stl in schoolDataContext1.Settlements
                                          on sSettlement.SettlementId equals stl.SettlementId

                                          join adm in schoolDataContext1.Admissions
                                          on fc.AdmissionId equals adm.AdmissionId into subAdmissionColl
                                          from sAdm in subAdmissionColl.DefaultIfEmpty()
                                          join std in schoolDataContext1.Students
                                          on sAdm.StudentId equals std.StudentId into subStudentColl
                                          from sStd in subStudentColl.DefaultIfEmpty()

                                          join ro in schoolDataContext1.RegistrationOnlines
                                          on fc.RegistrationOnlineId equals ro.RegistrationOnlineId into subRegistrationOnlines
                                          from sRegOnline in subRegistrationOnlines.DefaultIfEmpty()

                                          where fc.OnlinePaymentDate >= model.FromDate && fc.OnlinePaymentDate < model.ToDate.AddDays(1)
                                          && fc.PaymentMode == (byte)PaymentMode.Online
                                          orderby fc.OnlinePaymentDate ascending

                                          select new SettlementReceiptModel { StudentName = (subStudentColl != null ? sStd.FullName + " (" + sStd.AdmissionNo + ")" : (subRegistrationOnlines != null ? sRegOnline.StudentName + "(" + sRegOnline.RegistrationNo + ")" : "")), ReceiptNo = fc.ReceiptNo, SettlementDetailId = sSettlement.SettlementDetailId, SettlementId = stl.SettlementId, MerchantTransactionId = sSettlement.MerchantTransactionId, PayUId = fc.TransactionNo, TransactionDate = fc.OnlinePaymentDate, Mode = sSettlement.Mode, RequestAction = sSettlement.RequestAction, Amount = fc.PaidAmount, MerchantServiceFee = sSettlement.MerchantServiceFee, MerchantServiceTax = sSettlement.MerchantServiceTax, MerchantNetAmount = sSettlement.MerchantNetAmount, MerchantURTNumber = sSettlement.MerchantURTNumber, ReceiptDate = fc.PaymentDate, SettlementDate = stl.Dated }).ToList();



                response.TransactionList = settlementsOnline.Union(settlementsReceipt).ToList();

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
        }*/


    }
}
