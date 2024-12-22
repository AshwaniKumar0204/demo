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
    [RoutePrefix("api/StudentConcession")]
    public class StudentConcessionController : ApiController
    {
        [HttpPost]
        [Route("saveStudentConcession")]
        public ExpandoObject saveStudentConcession(SaveFeePaymentModel saveFeePaymentModel)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                //var MonthFeeHeadList = saveFeePaymentModel.MonthFeeHeadList.Where(x => x.IsEnabled).ToList();

                saveFeePaymentModel.MonthFeeHeadList.ForEach(month =>
                {
                    //inserting Concession Amount
                    month.FeeHeadDetails.Where(x => x.Concession > 0).ToList().ForEach(feeHead =>
                      {
                          StudentConcession studentConcession = null;
                          if (feeHead.StudentConcessionId > 0)
                              studentConcession = dataContext.StudentConcessions.Where(x => x.StudentConcessionId == feeHead.StudentConcessionId).First();
                          else
                              studentConcession = new StudentConcession();
                          studentConcession.AdmissionId = saveFeePaymentModel.AdmissionId;
                          studentConcession.FeeHeadId = feeHead.FeeHeadId;
                          studentConcession.FeeType = (byte)feeHead.FeeType;
                          studentConcession.MonthNo = month.MonthNo;
                          studentConcession.ConcessionAmount = feeHead.Concession;
                          studentConcession.Comments = feeHead.Comments;
                          if (studentConcession.StudentConcessionId == 0)
                          {
                              studentConcession.StudentConcessionStatus = (byte)StudentConcessionStatus.Active;
                              dataContext.StudentConcessions.InsertOnSubmit(studentConcession);
                          }
                          dataContext.SubmitChanges();
                      });

                    //Deleting Student Concession which has zero concession amount
                    month.FeeHeadDetails.Where(x => x.Concession == 0 && x.StudentConcessionId > 0).ToList().ForEach(feeHead =>
                      {
                          var studentConcession = dataContext.StudentConcessions.Where(x => x.StudentConcessionId == feeHead.StudentConcessionId).First();
                          dataContext.StudentConcessions.DeleteOnSubmit(studentConcession);
                          dataContext.SubmitChanges();
                      });

                });
                transaction.Commit();
                IsCompleted = true;
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
    }
}
