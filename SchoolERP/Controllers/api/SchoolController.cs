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
    [RoutePrefix("api/school")]
    public class SchoolController : ApiController
    {
        [HttpGet]
        [Route("SchoolList")]
        public ExpandoObject SchoolList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                ResponseMessage.SchoolList = LoadData.GetSchoolList(Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveSchool")]
        public ExpandoObject SaveSchool(SchoolModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                School.School School = new School.School();
                if (model.SchoolId > 0)
                    School = dataContext.Schools.Where(x => x.SchoolId == model.SchoolId).First();
                School.ShortName = model.ShortName;
                School.FullName = model.FullName;
                School.Tagline = model.Tagline;
                School.AffiliationDetails = model.AffiliationDetails;
                School.RegistrationNo = model.RegistrationNo;
                School.RegisteredOffice = model.RegisteredOffice;
                School.Website = model.Website;
                School.Email = model.Email;
                School.ContactNo = model.ContactNo;
                School.AlternateNo = model.AlternateNo;
                School.SchoolStatus = model.SchoolStatus;
                School.BankDetail = model.BankDetail;
                School.LastUpdatedBy = SessionUtilies.GetLoginId();
                School.LastUpdatedOn = DateTime.Now;
                if (School.SchoolId == 0)
                    dataContext.Schools.InsertOnSubmit(School);
                dataContext.SubmitChanges();
                if (!string.IsNullOrEmpty(model.LogoUrl))
                {
                    byte[] imageBytes = Convert.FromBase64String(model.LogoUrl.ToString());
                    School.Logo = imageBytes;
                    dataContext.SubmitChanges();
                }
                ResponseMessage.SchoolList = LoadData.GetSchoolList(0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Employee Type is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteSchool")]
        public ExpandoObject DeleteSchool(int SchoolId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var School = dataContext.Schools.Where(x => x.SchoolId == SchoolId).First();
                dataContext.Schools.DeleteOnSubmit(School);
                dataContext.SubmitChanges();
                ResponseMessage.SchoolList = LoadData.GetSchoolList(0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This School is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("SchoolLogo")]
        public ExpandoObject SchoolLogo(int SchoolId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.SchoolLogo = LoadData.BinaryToPhoto(dataContext.Schools.Where(x => x.SchoolId == SchoolId).First().Logo);
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
