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
    [RoutePrefix("api/StoreItemIssue")]
    public class StoreItemIssueController : ApiController
    {
        [HttpGet]
        [Route("StoreItemIssueHistory")]
        public ExpandoObject StoreItemIssueHistory(DateTime FromDate, DateTime ToDate, byte IssueStatus)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                var list = from b1 in dataContext.StoreItemIssues
                           join b2 in dataContext.StoreItems
                           on b1.StoreItemId equals b2.StoreItemId
                           join s1 in dataContext.Staffs
                           on b1.StaffId equals s1.StaffId into subStaff
                           join s2 in dataContext.Students
                           on b1.StudentId equals s2.StudentId into subStudent
                           join m1 in dataContext.MiscellaneousStaffs
                           on b1.MiscellaneousStaffId equals m1.MiscellaneousStaffId into subMiscellaneousStaff
                           where b1.DateOfIssue.Date >= FromDate.Date && b1.DateOfIssue.Date <= ToDate.Date
                           && (IssueStatus == 0 || (IssueStatus == 1 ? b1.ReturnDate == null : b1.ReturnDate != null))
                           orderby b1.DateOfIssue descending
                           select new
                           {

                               b1.DateOfIssue,
                               b1.StoreItemIssueId,
                               b1.Remarks,
                               b1.ReturnDate,
                               b2.ItemCode,
                               b2.ItemName,
                               b1.Quantity,
                               b2.Unit.UnitName,
                               IsReturn = b1.ReturnDate.HasValue,
                               Staff = subStaff.Any() ? subStaff.Select(x => new { x.StaffName, x.StaffCode }).First() : null,
                               Student = subStudent.Any() ? subStudent.Select(x => new { x.FullName, x.AdmissionNo }).First() : null,
                               MiscellaneousStaff = subMiscellaneousStaff.Any() ? subMiscellaneousStaff.Select(x => new { x.StaffName, x.MobileNo, x.Designation }).First() : null
                           };

                ResponseMessage.StoreItemIssueList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("StoreItemIssueList")]
        public ExpandoObject StoreItemIssueList(int StudentId, int StaffId, int MiscellaneousStaffId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.StoreItemIssues
                           join b2 in dataContext.StoreItems
                           on b1.StoreItemId equals b2.StoreItemId
                           where StudentId == 0 ? (MiscellaneousStaffId == 0 ? b1.StaffId == StaffId : b1.MiscellaneousStaffId == MiscellaneousStaffId) : b1.StudentId == StudentId
                           orderby b1.ReturnDate, b1.DateOfIssue
                           select new
                           {
                               b2.ItemCode,
                               b2.ItemName,
                               b2.Unit.UnitName,
                               b2.ItemType.ItemTypeName,
                               b1.DateOfIssue,
                               b1.ReturnDate,
                               b1.StoreItemIssueId,
                               b1.StoreItemId,
                               b1.StaffId,
                               b1.StudentId,
                               b1.Quantity,
                               b1.Remarks,
                           };

                ResponseMessage.StoreItemIssueList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("StoreItemIssue")]
        public ExpandoObject StoreItemIssue(StoreItemIssueModel model)
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
                var storeItem = dataContext.StoreItems.Where(x => x.StoreItemId == model.StoreItemId).First();
                if (storeItem.Quantity <= 0)
                    throw new Exception("This item has no stock");



                StoreItemIssue StoreItemIssue = null;
                if (model.StoreItemIssueId > 0)
                {
                    StoreItemIssue = dataContext.StoreItemIssues.Where(x => x.StoreItemIssueId == model.StoreItemIssueId).First();
                    StoreItemIssue.LastUpdatedBy = SessionUtilies.GetLoginId();
                    StoreItemIssue.LastUpdatedOn = DateTime.Now;
                    if (StoreItemIssue.Quantity != model.Quantity)
                    {
                        storeItem.Quantity += model.Quantity - StoreItemIssue.Quantity;
                        dataContext.SubmitChanges();
                    }
                    if (storeItem.Quantity <= 0)
                        throw new Exception("Item quantity should not be more than " + storeItem.Quantity);
                }
                else
                {
                    if (storeItem.Quantity < model.Quantity)
                        throw new Exception("Item quantity should not be more than " + storeItem.Quantity);

                    StoreItemIssue = new StoreItemIssue
                    {
                        //DateOfIssue = DateTime.Now,
                        CreatedBy = SessionUtilies.GetLoginId(),
                        CreatedOn = DateTime.Now
                    };

                    storeItem.Quantity -= model.Quantity;
                    dataContext.SubmitChanges();
                }
                StoreItemIssue.DateOfIssue = model.DateOfIssue;
                StoreItemIssue.Remarks = model.Remarks;
                StoreItemIssue.Quantity = model.Quantity;
                StoreItemIssue.StoreItemId = model.StoreItemId;
                StoreItemIssue.StudentId = model.StudentId;
                StoreItemIssue.MiscellaneousStaffId = model.MiscellaneousStaffId;
                StoreItemIssue.StaffId = model.StaffId;
                if (StoreItemIssue.StoreItemIssueId == 0)
                    dataContext.StoreItemIssues.InsertOnSubmit(StoreItemIssue);
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

        [HttpPost]
        [Route("StoreItemReturn")]
        public ExpandoObject StoreItemReturn(StoreItemIssueModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //Check StoreItem
                StoreItemIssue StoreItemIssue = dataContext.StoreItemIssues.Where(x => x.StoreItemIssueId == model.StoreItemIssueId).First();
                if (StoreItemIssue.ReturnDate != null)
                    throw new Exception("ThiseItem is already returned.");

                StoreItemIssue.LastUpdatedBy = SessionUtilies.GetLoginId();
                StoreItemIssue.LastUpdatedOn = DateTime.Now;
                StoreItemIssue.ReturnDate = DateTime.Now;
                dataContext.SubmitChanges();

                var StoreItem = dataContext.StoreItems.Where(x => x.StoreItemId == StoreItemIssue.StoreItemId).First();
                StoreItem.Quantity += StoreItemIssue.Quantity;
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteStoreItemIssue")]
        public ExpandoObject DeleteStoreItemIssue(int StoreItemIssueId)
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
                var StoreItemIssue = dataContext.StoreItemIssues.Where(x => x.StoreItemIssueId == StoreItemIssueId).First();

                var StoreItem = dataContext.StoreItems.Where(x => x.StoreItemId == StoreItemIssue.StoreItemId).First();
                StoreItem.Quantity += StoreItemIssue.Quantity;
                dataContext.StoreItemIssues.DeleteOnSubmit(StoreItemIssue);
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
