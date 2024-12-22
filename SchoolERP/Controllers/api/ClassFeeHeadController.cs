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
    [RoutePrefix("api/classFeeHead")]
    public class ClassFeeHeadController : ApiController
    {
        [HttpPost]
        [Route("saveClassFeeHead")]
        public ExpandoObject SaveClassFeeHead(List<MonthModel> monthList)
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
                        if (classFeeHeadModel.IsSelected)
                        {
                            var classFeeHeads = dataContext.ClassFeeHeads.Where(x => x.ClassFeeHeadId == classFeeHeadModel.ClassFeeHeadId);
                            if (!classFeeHeads.Any())
                            {
                                ClassFeeHead classFeeHead = new ClassFeeHead
                                {
                                    StudentTypeId = month.StudentTypeId,
                                    FeeHeadId = classFeeHeadModel.FeeHeadId,
                                    ClassId = month.ClassId,
                                    MonthNo = month.MonthNo,
                                    AcademicSessionId = month.AcademicSessionId
                                };
                                dataContext.ClassFeeHeads.InsertOnSubmit(classFeeHead);
                                dataContext.SubmitChanges();
                            }
                        }
                        else
                        {
                            if (classFeeHeadModel.ClassFeeHeadId > 0)
                            {
                                var classFeeHead = dataContext.ClassFeeHeads.Where(x => x.ClassFeeHeadId == classFeeHeadModel.ClassFeeHeadId).First();
                                dataContext.ClassFeeHeads.DeleteOnSubmit(classFeeHead);
                                dataContext.SubmitChanges();
                            }
                        }
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

        [HttpPost]
        [Route("ClassFeeHeadList")]
        public ExpandoObject ClassFeeHeadList(ClassFeeHeadModel classFeeHeadModel)
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
                                  }).ToList();

                motnthList.ForEach(month =>
                {
                    var classFeeHeads = (from o1 in dataContext.ClassFeeHeads
                                         where o1.ClassId == month.ClassId
                                         && o1.StudentTypeId == month.StudentTypeId
                                         && o1.AcademicSessionId == month.AcademicSessionId
                                         && o1.MonthNo == month.MonthNo
                                         select o1);
                    month.ClassFeeHeadList = (from f1 in dataContext.FeeHeads
                                              join c1 in classFeeHeads
                                              on f1.FeeHeadId equals c1.FeeHeadId into subClassFeeHead2
                                              from sClassFeeHead2 in subClassFeeHead2.DefaultIfEmpty()
                                              select new ClassFeeHeadModel
                                              {
                                                  ClassFeeHeadId = sClassFeeHead2.ClassFeeHeadId > 0 ? sClassFeeHead2.ClassFeeHeadId : 0,
                                                  FeeHeadId = f1.FeeHeadId,
                                                  FeeHeadTitle = f1.FeeHeadTitle,
                                                  IsSelected = subClassFeeHead2.Any(),
                                              }).ToList();
                });

                response.MonthList = motnthList.OrderBy(x => x.MonthId).ToList();
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
