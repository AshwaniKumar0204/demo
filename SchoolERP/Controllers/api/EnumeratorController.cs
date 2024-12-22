using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/enumerator")]
    public class EnumeratorController : ApiController
    {
        [HttpGet]
        [Route("StatusList")]
        public ExpandoObject StatusList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.StatusList = LoadData.GetValues<Status>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("EntryTypeList")]
        public ExpandoObject EntryTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.EntryTypeList = LoadData.GetValues<EntryType>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("CategoryList")]
        public ExpandoObject CategoryList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.CategoryList = LoadData.GetValues<Category>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("PassingStatusList")]
        public ExpandoObject PassingStatusList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.PassingStatusList = LoadData.GetValues<PassingStatus>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("NationalityList")]
        public ExpandoObject NationalityList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.NationalityList = LoadData.GetValues<Nationality>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("GenderList")]
        public ExpandoObject GenderList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.GenderList = LoadData.GetValues<Gender>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("BookReadTypeList")]
        public ExpandoObject BookReadTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.BookReadTypeList = LoadData.GetValues<BookReadType>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("StaffTypeList")]
        public ExpandoObject StaffTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.StaffTypeList = LoadData.GetValues<StaffType>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("RefundStatusList")]
        public ExpandoObject RefundStatusList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.RefundStatusList = LoadData.GetValues<RefundStatus>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("BoardMarksStatusList")]
        public ExpandoObject BoardMarksStatusList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.BoardMarksStatusList = LoadData.GetValues<BoardMarksStatus>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("AbsentTypeList")]
        public ExpandoObject AbsentTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.AbsentTypeList = LoadData.GetValues<AbsentType>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("LeaveTypeList")]
        public ExpandoObject LeaveTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.LeaveTypeList = LoadData.GetValues<LeaveType>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("AdmissionFromList")]
        public ExpandoObject AdmissionFromList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.AdmissionFromList = LoadData.GetValues<AdmissionFrom>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("ResultTypeList")]
        public ExpandoObject ResultTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.ResultTypeList = LoadData.GetValues<ResultType>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("DeviceTypeList")]
        public ExpandoObject DeviceTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.DeviceTypeList = LoadData.GetValues<DeviceType>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("VehicleServiceTypeList")]
        public ExpandoObject VehicleServiceTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.VehicleServiceTypeList = LoadData.GetValues<VehicleServiceType>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("SupplierTypeList")]
        public ExpandoObject SupplierTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.SupplierTypeList = LoadData.GetValues<SupplierType>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("MediumList")]
        public ExpandoObject MediumList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.MediumList = LoadData.GetValues<Medium>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("MonthList")]
        public ExpandoObject MonthList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.MonthList = LoadData.GetValues<MonthNames>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("BookPositionList")]
        public ExpandoObject BookPositionList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.BookPositionList = LoadData.GetValues<BookPosition>(new int[] { }, false).ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("BookIssueTypeList")]
        public ExpandoObject BookIssueTypeList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.BookIssueTypeList = LoadData.GetValues<BookIssueType>(new int[] { }, false).ToList();
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
