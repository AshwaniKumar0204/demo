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
    [RoutePrefix("api/AdmissionFee")]
    public class AdmissionFeeController : ApiController
    {
        [HttpPost]
        [Route("AdmissionFeeList")]
        public ExpandoObject AdmissionFeeList(AdmissionFeeHeadModel AdmissionFeeHeadModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();


                var AdmissionFeeList = (from c1 in dataContext.AdmissionFeeHeads
                                        join f1 in dataContext.FeeHeads
                                        on c1.FeeHeadId equals f1.FeeHeadId
                                        join cf1 in dataContext.AdmissionFees
                                        on c1.AdmissionFeeHeadId equals cf1.AdmissionFeeHeadId into subAdmissionFee
                                        from sAdmissionFee in subAdmissionFee.DefaultIfEmpty()
                                        where c1.ClassId == AdmissionFeeHeadModel.ClassId
                                        && c1.StudentTypeId == AdmissionFeeHeadModel.StudentTypeId
                                        && c1.AdmissionType == AdmissionFeeHeadModel.AdmissionType
                                        && c1.AcademicSessionId == AdmissionFeeHeadModel.AcademicSessionId
                                        select new AdmissionFeeModel
                                        {
                                            AdmissionFeeHeadId = c1.AdmissionFeeHeadId,
                                            FeeHeadTitle = f1.FeeHeadTitle,
                                            FeeHeadId = c1.FeeHeadId,
                                            AdmissionFeeId = sAdmissionFee.AdmissionFeeId > 0 ? sAdmissionFee.AdmissionFeeId : 0,
                                            Amount = sAdmissionFee.Amount > 0 ? sAdmissionFee.Amount : 0,
                                            Remarks = sAdmissionFee.Remarks,
                                            IsActive = sAdmissionFee.IsActive != null ? sAdmissionFee.IsActive : false,
                                            IsRefundable = sAdmissionFee.IsRefundable != null ? sAdmissionFee.IsRefundable : false
                                        }).ToList();
                response.AdmissionFeeList = AdmissionFeeList;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveAdmissionFee")]
        public ExpandoObject SaveAdmissionFee(List<AdmissionFeeModel> AdmissionFeeList)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            int y = 0;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                AdmissionFeeList.ForEach(AdmissionFeeModel =>
                {

                    AdmissionFee AdmissionFee = null;
                    if (AdmissionFeeModel.AdmissionFeeId > 0)
                        AdmissionFee = dataContext.AdmissionFees.Where(x => x.AdmissionFeeId == AdmissionFeeModel.AdmissionFeeId).First();
                    else
                        AdmissionFee = new AdmissionFee();
                    //AdmissionFee.IsActive = AdmissionFeeHeadModel.AdmissionFee.IsActive;
                    AdmissionFee.IsActive = true;
                    AdmissionFee.Amount = AdmissionFeeModel.Amount;
                    AdmissionFee.IsRefundable = AdmissionFeeModel.IsRefundable;
                    AdmissionFee.Remarks = AdmissionFeeModel.Remarks;
                    if (AdmissionFee.AdmissionFeeId == 0 && AdmissionFeeModel.Amount > 0)
                    {
                        AdmissionFee.AdmissionFeeHeadId = AdmissionFeeModel.AdmissionFeeHeadId;
                        dataContext.AdmissionFees.InsertOnSubmit(AdmissionFee);
                    }
                    dataContext.SubmitChanges();
                });
                transaction.Commit();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (y != 1)
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
    }
}
