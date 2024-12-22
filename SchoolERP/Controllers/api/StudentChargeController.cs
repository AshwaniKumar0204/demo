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
    [RoutePrefix("api/StudentCharge")]
    public class StudentChargeController : ApiController
    {
        [HttpPost]
        [Route("SaveStudentCharge")]
        public ExpandoObject SaveStudentCharge(StudentChargeModel studentChargeModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StudentCharge studentCharge = null;
                if (studentChargeModel.StudentChargeId > 0)
                    studentCharge = dataContext.StudentCharges.Where(x => x.StudentChargeId == studentChargeModel.StudentChargeId).First();
                else
                    studentCharge = new StudentCharge
                    {
                        IsRefundable = false,
                        FeeType = (byte)FeeType.ExtraCharge,
                        StudentChargeStatus = (byte)StudentChargeStatus.Active,
                        AdmissionId = studentChargeModel.AdmissionId,
                    };

                studentCharge.FeeHeadId = studentChargeModel.FeeHeadId;
                studentCharge.FineAmount = studentChargeModel.FineAmount;
                studentCharge.Comments = studentChargeModel.Comments;
                studentCharge.IsCompulsory = studentChargeModel.IsCompulsory;
                studentCharge.FineDate = studentChargeModel.FineDate;

                if (studentCharge.StudentChargeId == 0)
                    dataContext.StudentCharges.InsertOnSubmit(studentCharge);
                dataContext.SubmitChanges();

                response.StudentChargeList = LoadData.GetStudentChargeList(studentChargeModel.AdmissionId, dataContext);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("StudentChargeList")]
        public ExpandoObject StudentChargeList(int AdmissionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                response.StudentChargeList = LoadData.GetStudentChargeList(AdmissionId, dataContext);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("DeleteStudentCharge")]
        public ExpandoObject DeleteStudentCharge(int StudentChargeId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StudentCharge studentCharge = dataContext.StudentCharges.Where(x => x.StudentChargeId == StudentChargeId).First();
                int AdmissionId = studentCharge.AdmissionId;
                dataContext.StudentCharges.DeleteOnSubmit(studentCharge);
                dataContext.SubmitChanges();
                response.StudentChargeList = LoadData.GetStudentChargeList(AdmissionId, dataContext);
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
