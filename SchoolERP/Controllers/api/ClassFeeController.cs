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
    [RoutePrefix("api/classFee")]
    public class ClassFeeController : ApiController
    {
        [HttpPost]
        [Route("classFeeList")]
        public ExpandoObject ClassFeeList(ClassFeeHeadModel classFeeHeadModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var motnthList = (from m1 in dataContext.Months
                                  where m1.ClassId == classFeeHeadModel.ClassId
                                  select new MonthModel
                                  {
                                      MonthNo = m1.MonthNo,
                                      MonthName = m1.MonthName,
                                      AcademicSessionId = classFeeHeadModel.AcademicSessionId,
                                      StudentTypeId = classFeeHeadModel.StudentTypeId,
                                      ClassId = classFeeHeadModel.ClassId,
                                      ClassFeeHeadList = (from c1 in dataContext.ClassFeeHeads
                                                          join f1 in dataContext.FeeHeads
                                                          on c1.FeeHeadId equals f1.FeeHeadId
                                                          join cf1 in dataContext.ClassFees
                                                          on c1.ClassFeeHeadId equals cf1.ClassFeeHeadId into subClassFee
                                                          from sClassFee in subClassFee.DefaultIfEmpty()
                                                          where c1.ClassId == classFeeHeadModel.ClassId
                                                          && c1.StudentTypeId == classFeeHeadModel.StudentTypeId
                                                          && c1.AcademicSessionId == classFeeHeadModel.AcademicSessionId
                                                          && c1.MonthNo == m1.MonthNo
                                                          select new ClassFeeHeadModel
                                                          {
                                                              ClassFeeHeadId = c1.ClassFeeHeadId,
                                                              FeeHeadId = f1.FeeHeadId,
                                                              FeeHeadTitle = f1.FeeHeadTitle,
                                                              ClassFee = new ClassFeeModel
                                                              {
                                                                  ClassFeeId = sClassFee.ClassFeeId > 0 ? sClassFee.ClassFeeId : 0,
                                                                  Amount = sClassFee.Amount > 0 ? sClassFee.Amount : 0,
                                                                  ClassFeeHeadId = sClassFee.ClassFeeHeadId > 0 ? sClassFee.ClassFeeHeadId : 0,
                                                                  Comments = sClassFee.Comments,
                                                                  IsActive = sClassFee.IsActive != null ? sClassFee.IsActive : false,
                                                                  IsCompulsory = sClassFee.IsCompulsory != null ? sClassFee.IsCompulsory : false,
                                                              }
                                                          }).ToList()
                                  }).ToList();
                response.MonthList = motnthList.OrderBy(x => x.MonthId).ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveClassFee")]
        public ExpandoObject SaveClassFee(List<MonthModel> monthList)
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
                monthList.ForEach(month =>
                {
                    month.ClassFeeHeadList.ForEach(classFeeHeadModel =>
                    {

                        ClassFee classFee = null;
                        if (classFeeHeadModel.ClassFee.ClassFeeId > 0)
                            classFee = dataContext.ClassFees.Where(x => x.ClassFeeId == classFeeHeadModel.ClassFee.ClassFeeId).First();
                        else
                            classFee = new ClassFee();
                        classFee.IsActive = true;
                        classFee.Amount = classFeeHeadModel.ClassFee.Amount;
                        classFee.Comments = classFeeHeadModel.ClassFee.Comments;
                        classFee.IsCompulsory = classFeeHeadModel.ClassFee.IsCompulsory;
                        if (classFee.ClassFeeId == 0 && classFeeHeadModel.ClassFee.Amount > 0)
                        {
                            classFee.ClassFeeHeadId = classFeeHeadModel.ClassFeeHeadId;
                            dataContext.ClassFees.InsertOnSubmit(classFee);
                        }
                        dataContext.SubmitChanges();
                    });
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
