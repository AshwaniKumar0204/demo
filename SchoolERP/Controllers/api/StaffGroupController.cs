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
    [RoutePrefix("api/StaffGroup")]
    public class StaffGroupController : ApiController
    {
        [HttpGet]
        [Route("StaffGroupList")]
        public ExpandoObject StaffGroupList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.StaffGroups
                           where (Status == 0 ? true : b1.Status == Status)
                           select new
                           {
                               b1.GroupName,
                               b1.Status,
                               b1.StaffGroupId,
                               NoOfStaffs = dataContext.StaffGroupDetails.Where(x=>x.StaffGroupId ==b1.StaffGroupId).Count(),
                               StaffGroupDetailList = (from s1 in dataContext.StaffGroupDetails
                                                       where s1.StaffGroupId == b1.StaffGroupId
                                                       select new
                                                       {
                                                           s1.StaffGroupDetailId,
                                                           s1.StaffId
                                                       })
                           };
                ResponseMessage.StaffGroupList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveStaffGroup")]
        public ExpandoObject SaveStaffGroup(StaffGroupModel model)
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
                int loginId = SessionUtilies.GetLoginId();

                StaffGroup StaffGroup = null;
                if (model.StaffGroupId > 0)
                    StaffGroup = dataContext.StaffGroups.Where(x => x.StaffGroupId == model.StaffGroupId).First();
                else
                    StaffGroup = new StaffGroup();
                StaffGroup.GroupName = model.GroupName;
                StaffGroup.LastUpdatedBy = loginId;
                StaffGroup.LastUpdatedOn = DateTime.Now;
                StaffGroup.Status = model.Status;
                if (StaffGroup.StaffGroupId == 0)
                    dataContext.StaffGroups.InsertOnSubmit(StaffGroup);
                dataContext.SubmitChanges();

                model.StaffGroupDetailList.Where(x => x.IsSelected).ToList().ForEach(d1 =>
                {
                    StaffGroupDetail staffGroupDetail = null;
                    if (d1.StaffGroupDetailId > 0)
                        staffGroupDetail = dataContext.StaffGroupDetails.Where(x => x.StaffGroupDetailId == d1.StaffGroupDetailId).First();
                    else
                        staffGroupDetail = new StaffGroupDetail();
                    staffGroupDetail.StaffId = d1.StaffId;
                    staffGroupDetail.StaffGroupId = StaffGroup.StaffGroupId;
                    if (staffGroupDetail.StaffGroupDetailId == 0)
                        dataContext.StaffGroupDetails.InsertOnSubmit(staffGroupDetail);
                    dataContext.SubmitChanges();
                });

                model.StaffGroupDetailList.Where(x => x.IsSelected == false && x.StaffGroupDetailId != 0).ToList().ForEach(d1 =>
                {
                    var staffGroupDetail = dataContext.StaffGroupDetails.Where(x => x.StaffGroupDetailId == d1.StaffGroupDetailId).First();
                    dataContext.StaffGroupDetails.DeleteOnSubmit(staffGroupDetail);
                });
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
        [Route("deleteStaffGroup")]
        public ExpandoObject DeleteStaffGroup(int StaffGroupId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var StaffGroup = dataContext.StaffGroups.Where(x => x.StaffGroupId == StaffGroupId).First();
                dataContext.StaffGroups.DeleteOnSubmit(StaffGroup);
                dataContext.SubmitChanges();
                //ResponseMessage.StaffGroupList = StoreData.StaffGroupList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Item is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("StaffGroup")]
        public ExpandoObject StaffGroup(int StaffGroupId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.StaffGroups
                           where b1.StaffGroupId == StaffGroupId
                           select new
                           {
                               b1.GroupName,
                               b1.Status,
                               b1.StaffGroupId,
                               StaffGroupDetailList = (from s1 in dataContext.StaffGroupDetails
                                                       where s1.StaffGroupId == b1.StaffGroupId
                                                       select new
                                                       {
                                                           s1.StaffGroupDetailId,
                                                           s1.StaffId
                                                       })
                           };
                ResponseMessage.StaffGroupList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
