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
    [RoutePrefix("api/Parent")]
    public class ParentController : ApiController
    {
        [HttpGet]
        [Route("ParentList")]
        public ExpandoObject ParentList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.Parents
                           where (Status == 0 || p1.ParentStatus == Status)
                           orderby p1.ParentId
                           select new
                           {
                               p1.ParentId,
                               p1.ParentCode,
                               p1.MotherMobileNo,
                               p1.FatherMobileNo,
                               p1.EmailId,
                               p1.ParentName,
                               LoginPassword = CryptoEngine.Decrypt(p1.LoginPassword),
                               p1.ParentStatus,
                               StudentList = from s1 in dataContext.Students
                                             where s1.ParentId == p1.ParentId
                                             select new
                                             {
                                                 s1.StudentId,
                                                 s1.ParentId,
                                                 s1.AdmissionNo,
                                                 s1.FullName,
                                             }
                           };
                ResponseMessage.ParentList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveParent")]
        public ExpandoObject SaveParent(ParentModel model)
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
                Parent Parent = null;
                if (model.ParentId > 0)
                    Parent = dataContext.Parents.Where(x => x.ParentId == model.ParentId).First();
                else
                    Parent = new Parent
                    {
                        ParentCode = LoadData.GenerateParentCode(dataContext)
                    };
                Parent.LoginPassword = CryptoEngine.Encrypt(model.LoginPassword);
                Parent.FatherMobileNo = model.FatherMobileNo;
                Parent.MotherMobileNo = model.MotherMobileNo;
                Parent.ParentName = model.ParentName;
                Parent.EmailId = model.EmailId;
                Parent.ParentStatus = model.ParentStatus;
                if (Parent.ParentId == 0)
                    dataContext.Parents.InsertOnSubmit(Parent);
                dataContext.SubmitChanges();

                //remove parents
                var studentList = from s1 in dataContext.Students
                                  where s1.ParentId == Parent.ParentId
                                  && !model.StudentList.Select(x => x.StudentId).Contains(s1.StudentId)
                                  select s1;
                studentList.ToList().ForEach(student => student.ParentId = null);
                dataContext.SubmitChanges();

                model.StudentList.ForEach(x =>
                {
                    var student = dataContext.Students.Where(y => y.StudentId == x.StudentId).First();
                    student.ParentId = Parent.ParentId;
                    dataContext.SubmitChanges();
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

        [HttpGet]
        [Route("deleteParent")]
        public ExpandoObject DeleteParent(int ParentId)
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

                var students = dataContext.Students.Where(x => x.ParentId == ParentId).ToList();
                students.ForEach(student => student.ParentId = null);
                dataContext.SubmitChanges();

                var Parent = dataContext.Parents.Where(x => x.ParentId == ParentId).First();
                dataContext.Parents.DeleteOnSubmit(Parent);
                dataContext.SubmitChanges();
                transaction.Commit();
                IsCompleted = true;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                if (ex.Message.Contains("FK"))
                    response.Message = "This Parent is in use.so can't delete.";
                else
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
