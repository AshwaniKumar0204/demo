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
    [RoutePrefix("api/AdmissionFeeHead")]
    public class AdmissionFeeHeadController : ApiController
    {
        [HttpPost]
        [Route("saveAdmissionFeeHead")]
        public ExpandoObject SaveAdmissionFeeHead(SaveAdmissionFeeHeadModel saveAdmissionFeeHead)
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
                saveAdmissionFeeHead.AdmissionFeeHeadList.ForEach(AdmissionFeeHeadModel =>
                {
                    if (AdmissionFeeHeadModel.IsSelected)
                    {
                        var AdmissionFeeHeads = dataContext.AdmissionFeeHeads.Where(x => x.AdmissionFeeHeadId == AdmissionFeeHeadModel.AdmissionFeeHeadId);
                        if (!AdmissionFeeHeads.Any())
                        {
                            AdmissionFeeHead AdmissionFeeHead = new AdmissionFeeHead
                            {
                                StudentTypeId = saveAdmissionFeeHead.StudentTypeId,
                                FeeHeadId = AdmissionFeeHeadModel.FeeHeadId,
                                ClassId = saveAdmissionFeeHead.ClassId,
                                AcademicSessionId = saveAdmissionFeeHead.AcademicSessionId,
                                AdmissionType = saveAdmissionFeeHead.AdmissionType
                            };
                            dataContext.AdmissionFeeHeads.InsertOnSubmit(AdmissionFeeHead);
                            dataContext.SubmitChanges();
                        }
                    }
                    else
                    {
                        if (AdmissionFeeHeadModel.AdmissionFeeHeadId > 0)
                        {
                            var AdmissionFeeHead = dataContext.AdmissionFeeHeads.Where(x => x.AdmissionFeeHeadId == AdmissionFeeHeadModel.AdmissionFeeHeadId).First();
                            dataContext.AdmissionFeeHeads.DeleteOnSubmit(AdmissionFeeHead);
                            dataContext.SubmitChanges();
                        }
                    }
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
        [Route("AdmissionFeeHeadList")]
        public ExpandoObject AdmissionFeeHeadList(SaveAdmissionFeeHeadModel saveAdmissionFeeHead)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var AdmissionFeeHeads = (from o1 in dataContext.AdmissionFeeHeads
                                            where o1.ClassId == saveAdmissionFeeHead.ClassId
                                            && o1.StudentTypeId == saveAdmissionFeeHead.StudentTypeId
                                            && o1.AcademicSessionId == saveAdmissionFeeHead.AcademicSessionId
                                            && o1.AdmissionType == saveAdmissionFeeHead.AdmissionType
                                            select o1);
                var AdmissionFeeHeadList = (from f1 in dataContext.FeeHeads
                                               join c1 in AdmissionFeeHeads
                                               on f1.FeeHeadId equals c1.FeeHeadId into subAdmissionFeeHead2
                                               from sAdmissionFeeHead2 in subAdmissionFeeHead2.DefaultIfEmpty()
                                               select new AdmissionFeeHeadModel
                                               {
                                                   AdmissionFeeHeadId = sAdmissionFeeHead2.AdmissionFeeHeadId > 0 ? sAdmissionFeeHead2.AdmissionFeeHeadId : 0,
                                                   FeeHeadId = f1.FeeHeadId,
                                                   FeeHeadTitle = f1.FeeHeadTitle,
                                                   IsSelected = subAdmissionFeeHead2.Any(),
                                               }).ToList();

                response.AdmissionFeeHeadList = AdmissionFeeHeadList;
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
