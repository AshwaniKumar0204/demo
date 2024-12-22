using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Dynamic;
using School;
using SchoolERP.Models;
using System.Data.Common;
using System.Web;
using System.IO;
namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/leftstudent")]
    public class LeftStudentController : ApiController
    {
        [HttpPost]
        [Route("leftStudent")]
        public ExpandoObject LeftStudent(LeftStudentModel leftStudentModel)
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
                var admissions = dataContext.Admissions.Where(x => x.AdmissionId == leftStudentModel.AdmissionId && x.AdmissionStatus == (byte)AdmissionStatus.Active);
                if (!admissions.Any())
                {
                    response.Message = "Invalid Admission No";
                    return response;
                }
                var admission = admissions.First();
                admission.AdmissionStatus = (byte)AdmissionStatus.Left;
                admission.LeaveDate = leftStudentModel.LeftDate;
                dataContext.SubmitChanges();

                var student = dataContext.Students.Where(x => x.StudentId == admission.StudentId).First();
                student.StudentStatus = (byte)StudentStatus.Left;
                dataContext.SubmitChanges();

                LeftStudent leftStudent = new LeftStudent
                {
                    LeftDate = leftStudentModel.LeftDate,
                    AdmissionId = leftStudentModel.AdmissionId,
                    DuesAmount = FeeData.GetDueDetails(dataContext, leftStudentModel.AdmissionId, 0,false,0,0).TotalDues,
                    LeftStudentStatus = (byte)LeftStudentStatus.Left,
                    LeftStudentType = leftStudentModel.LeftStudentType,
                    Reason = leftStudentModel.Reason,
                    UpdatedBy = SessionUtilies.GetLoginId(),
                    UpdatedOn = DateTime.Now
                };
                dataContext.LeftStudents.InsertOnSubmit(leftStudent);
                dataContext.SubmitChanges();

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

        [HttpGet]
        [Route("leftStudentList")]
        public ExpandoObject LeftStudentList(byte Status)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var leftStudents = (from ls in dataContext.LeftStudents
                                    join a1 in dataContext.Admissions
                                    on ls.AdmissionId equals a1.AdmissionId
                                    join st in dataContext.Students
                                    on a1.StudentId equals st.StudentId
                                    where Status > 0 ? ls.LeftStudentStatus == Status : true
                                    orderby ls.LeftStudentId descending
                                    select new
                                    {
                                        ls.LeftStudentId,
                                        ls.LeftDate,
                                        ls.AdmissionId,
                                        ls.LeftStudentType,
                                        a1.Section.Class.ClassName,
                                        ls.Reason,
                                        ls.DuesAmount,
                                        st.AdmissionNo,
                                        st.FullName,
                                        st.MobileNo
                                    });
                response.LeftStudentList = leftStudents.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("rejoinLeftStudent")]
        public ExpandoObject RejoinLeftStudent(int LeftStudentId)
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
                var leftStudent = dataContext.LeftStudents.Where(x => x.LeftStudentId == LeftStudentId).First();
                leftStudent.LeftStudentStatus = (byte)LeftStudentStatus.Rejoin;
                dataContext.SubmitChanges();

                var admission = dataContext.Admissions.Where(x => x.AdmissionId == leftStudent.AdmissionId).First();
                admission.AdmissionStatus = (byte)AdmissionStatus.Active;
                admission.LeaveDate = null;
                dataContext.SubmitChanges();

                var student = dataContext.Students.Where(x => x.StudentId == admission.StudentId).First();
                student.StudentStatus = (byte)StudentStatus.Active;
                dataContext.SubmitChanges();

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
