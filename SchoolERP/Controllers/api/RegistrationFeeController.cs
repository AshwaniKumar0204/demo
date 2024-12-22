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
    [RoutePrefix("api/RegistrationFee")]
    public class RegistrationFeeController : ApiController
    {
        [HttpPost]
        [Route("RegistrationFeeList")]
        public ExpandoObject RegistrationFeeList(RegistrationFeeHeadModel RegistrationFeeHeadModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();


                var RegistrationFeeList = (from c1 in dataContext.RegistrationFeeHeads
                                           join f1 in dataContext.FeeHeads
                                           on c1.FeeHeadId equals f1.FeeHeadId
                                           join cf1 in dataContext.RegistrationFees
                                           on c1.RegistrationFeeHeadId equals cf1.RegistrationFeeHeadId into subRegistrationFee
                                           from sRegistrationFee in subRegistrationFee.DefaultIfEmpty()
                                           where c1.ClassId == RegistrationFeeHeadModel.ClassId
                                           && c1.StudentTypeId == RegistrationFeeHeadModel.StudentTypeId
                                           && c1.AcademicSessionId == RegistrationFeeHeadModel.AcademicSessionId
                                           select new RegistrationFeeModel
                                           {
                                               RegistrationFeeHeadId = c1.RegistrationFeeHeadId,
                                               FeeHeadTitle = f1.FeeHeadTitle,
                                               FeeHeadId = c1.FeeHeadId,
                                               RegistrationFeeId = sRegistrationFee.RegistrationFeeId > 0 ? sRegistrationFee.RegistrationFeeId : 0,
                                               Amount = sRegistrationFee.Amount > 0 ? sRegistrationFee.Amount : 0,
                                               Remarks = sRegistrationFee.Remarks,
                                               IsActive = sRegistrationFee.IsActive != null ? sRegistrationFee.IsActive : false
                                           }).ToList();
                response.RegistrationFeeList = RegistrationFeeList;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveRegistrationFee")]
        public ExpandoObject SaveRegistrationFee(List<RegistrationFeeModel> RegistrationFeeList)
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

                RegistrationFeeList.ForEach(RegistrationFeeModel =>
                {

                    RegistrationFee RegistrationFee = null;
                    if (RegistrationFeeModel.RegistrationFeeId > 0)
                        RegistrationFee = dataContext.RegistrationFees.Where(x => x.RegistrationFeeId == RegistrationFeeModel.RegistrationFeeId).First();
                    else
                        RegistrationFee = new RegistrationFee();
                    //RegistrationFee.IsActive = RegistrationFeeHeadModel.RegistrationFee.IsActive;
                    RegistrationFee.IsActive = true;
                    RegistrationFee.Amount = RegistrationFeeModel.Amount;
                    RegistrationFee.Remarks = RegistrationFeeModel.Remarks;
                    if (RegistrationFee.RegistrationFeeId == 0 && RegistrationFeeModel.Amount > 0)
                    {
                        RegistrationFee.RegistrationFeeHeadId = RegistrationFeeModel.RegistrationFeeHeadId;
                        dataContext.RegistrationFees.InsertOnSubmit(RegistrationFee);
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

        [HttpGet]
        [Route("RegistrationFeeListForRegistration")]
        public ExpandoObject RegistrationFeeListForRegistration(int ClassId, int StudentTypeId, int AcademicSessionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();


                var RegistrationFeeList = (from c1 in dataContext.RegistrationFeeHeads
                                           join f1 in dataContext.FeeHeads
                                           on c1.FeeHeadId equals f1.FeeHeadId
                                           join cf1 in dataContext.RegistrationFees
                                           on c1.RegistrationFeeHeadId equals cf1.RegistrationFeeHeadId
                                           where c1.ClassId == ClassId
                                           && c1.StudentTypeId == StudentTypeId
                                           && c1.AcademicSessionId == AcademicSessionId
                                           where cf1.IsActive
                                           select new RegistrationFeeModel
                                           {
                                               RegistrationFeeHeadId = c1.RegistrationFeeHeadId,
                                               FeeHeadTitle = f1.FeeHeadTitle,
                                               FeeHeadId = c1.FeeHeadId,
                                               RegistrationFeeId = cf1.RegistrationFeeId,
                                               Amount = cf1.Amount,
                                               Remarks = cf1.Remarks,
                                           }).ToList();
                response.RegistrationFeeList = RegistrationFeeList;
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
