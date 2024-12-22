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
    [RoutePrefix("api/RegistrationFeeHead")]
    public class RegistrationFeeHeadController : ApiController
    {
        [HttpPost]
        [Route("saveRegistrationFeeHead")]
        public ExpandoObject SaveRegistrationFeeHead(SaveRegistrationFeeHeadModel saveRegistrationFeeHead)
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
                saveRegistrationFeeHead.RegistrationFeeHeadList.ForEach(RegistrationFeeHeadModel =>
                {
                    if (RegistrationFeeHeadModel.IsSelected)
                    {
                        var RegistrationFeeHeads = dataContext.RegistrationFeeHeads.Where(x => x.RegistrationFeeHeadId == RegistrationFeeHeadModel.RegistrationFeeHeadId);
                        if (!RegistrationFeeHeads.Any())
                        {
                            RegistrationFeeHead RegistrationFeeHead = new RegistrationFeeHead
                            {
                                StudentTypeId = saveRegistrationFeeHead.StudentTypeId,
                                FeeHeadId = RegistrationFeeHeadModel.FeeHeadId,
                                ClassId = saveRegistrationFeeHead.ClassId,
                                AcademicSessionId = saveRegistrationFeeHead.AcademicSessionId
                            };
                            dataContext.RegistrationFeeHeads.InsertOnSubmit(RegistrationFeeHead);
                            dataContext.SubmitChanges();
                        }
                    }
                    else
                    {
                        if (RegistrationFeeHeadModel.RegistrationFeeHeadId > 0)
                        {
                            var RegistrationFeeHead = dataContext.RegistrationFeeHeads.Where(x => x.RegistrationFeeHeadId == RegistrationFeeHeadModel.RegistrationFeeHeadId).First();
                            dataContext.RegistrationFeeHeads.DeleteOnSubmit(RegistrationFeeHead);
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
        [Route("RegistrationFeeHeadList")]
        public ExpandoObject RegistrationFeeHeadList(SaveRegistrationFeeHeadModel saveRegistrationFeeHead)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var RegistrationFeeHeads = (from o1 in dataContext.RegistrationFeeHeads
                                            where o1.ClassId == saveRegistrationFeeHead.ClassId
                                            && o1.StudentTypeId == saveRegistrationFeeHead.StudentTypeId
                                            && o1.AcademicSessionId == saveRegistrationFeeHead.AcademicSessionId
                                            select o1);
                var RegistrationFeeHeadList = (from f1 in dataContext.FeeHeads
                                               join c1 in RegistrationFeeHeads
                                               on f1.FeeHeadId equals c1.FeeHeadId into subRegistrationFeeHead2
                                               from sRegistrationFeeHead2 in subRegistrationFeeHead2.DefaultIfEmpty()
                                               select new RegistrationFeeHeadModel
                                               {
                                                   RegistrationFeeHeadId = sRegistrationFeeHead2.RegistrationFeeHeadId > 0 ? sRegistrationFeeHead2.RegistrationFeeHeadId : 0,
                                                   FeeHeadId = f1.FeeHeadId,
                                                   FeeHeadTitle = f1.FeeHeadTitle,
                                                   IsSelected = subRegistrationFeeHead2.Any(),
                                               }).ToList();

                response.RegistrationFeeHeadList = RegistrationFeeHeadList;
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
