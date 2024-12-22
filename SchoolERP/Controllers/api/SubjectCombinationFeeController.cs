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
    [RoutePrefix("api/SubjectCombinationFee")]
    public class SubjectCombinationFeeController : ApiController
    {

        [HttpGet]
        [Route("SubjectCombinationFeeList")]
        public ExpandoObject SubjectCombinationFeeList(int ClassId, int AcademicSessionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from scfh in dataContext.SubjectCombinationFeeHeads
                            join scf in dataContext.SubjectCombinationFees
                            on scfh.SubjectCombinationFeeHeadId equals scf.SubjectCombinationFeeHeadId into subSFee
                            from sSFee in subSFee.DefaultIfEmpty()
                            join sc1 in dataContext.SubjectCombinations
                            on scfh.SubjectCombinationId equals sc1.SubjectCombinationId
                            where sc1.ClassId == ClassId
                            && sc1.AcademicSessionId == AcademicSessionId
                            select new SubjectCombinationFeeModel
                            {
                                Amount = sSFee.Amount != null ? sSFee.Amount : 0,
                                FeeHeadId = scfh.FeeHeadId,
                                FeeHeadTitle = scfh.FeeHead.FeeHeadTitle,
                                SubjectCombinationFeeHeadId = scfh.SubjectCombinationFeeHeadId,
                                SubjectCombinationFeeId = sSFee.SubjectCombinationFeeId != null ? sSFee.SubjectCombinationFeeId : 0,
                                SubjectCombinationId = sc1.SubjectCombinationId,
                                SubjectCombinationName = sc1.SubjectCombinationName
                            });

                response.SubjectCombinationFeeList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveSubjectCombinationFee")]
        public ExpandoObject SaveSubjectCombinationFee(List<SubjectCombinationFeeModel> SubjectCombinationFeeList)
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

                SubjectCombinationFeeList.ForEach(SubjectCombinationFeeModel =>
                {

                    SubjectCombinationFee SubjectCombinationFee = null;
                    if (SubjectCombinationFeeModel.SubjectCombinationFeeId > 0)
                        SubjectCombinationFee = dataContext.SubjectCombinationFees.Where(x => x.SubjectCombinationFeeId == SubjectCombinationFeeModel.SubjectCombinationFeeId).First();
                    else
                        SubjectCombinationFee = new SubjectCombinationFee();
                    SubjectCombinationFee.Amount = SubjectCombinationFeeModel.Amount;
                    if (SubjectCombinationFee.SubjectCombinationFeeId == 0 && SubjectCombinationFeeModel.Amount > 0)
                    {
                        SubjectCombinationFee.SubjectCombinationFeeHeadId = SubjectCombinationFeeModel.SubjectCombinationFeeHeadId;
                        dataContext.SubjectCombinationFees.InsertOnSubmit(SubjectCombinationFee);
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
