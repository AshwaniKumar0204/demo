using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/RegistrationForm")]
    public class RegistrationFormController : ApiController
    {
        [HttpGet]
        [Route("RegistrationFormList")]
        public ExpandoObject RegistrationFormList(byte Status, int AcademicSessionId, int ClassNo)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.RegistrationFormList = LoadData.GetRegistrationFormList(dataContext, Status, AcademicSessionId, ClassNo);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("SearchRegistrationFormList")]
        public ExpandoObject SearchRegistrationFormList(byte Status, int AcademicSessionId, int ClassNo)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from r1 in dataContext.RegistrationForms
                           where (AcademicSessionId == 0 ? true : r1.AcademicSessionId == AcademicSessionId)
                           && (ClassNo == 0 ? true : r1.ClassNo == ClassNo)
                           && (Status == 0 ? true : r1.RegistrationFormStatus == Status)
                           select new { r1.RegistrationFormId, r1.PhaseName, r1.AllowSameSchool };
                ResponseMessage.RegistrationFormList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveRegistrationForm")]
        public ExpandoObject SaveRegistrationForm(RegistrationFormModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                RegistrationForm RegistrationForm = null;
                if (model.RegistrationFormId > 0)
                    RegistrationForm = dataContext.RegistrationForms.Where(x => x.RegistrationFormId == model.RegistrationFormId).First();
                else
                    RegistrationForm = new RegistrationForm();
                RegistrationForm.AcademicSessionId = model.AcademicSessionId;
                RegistrationForm.ClassNo = model.ClassNo;
                RegistrationForm.PhaseName = model.PhaseName;
                RegistrationForm.FromDate = model.FromDate;
                RegistrationForm.ToDate = model.ToDate;
                RegistrationForm.FromDOB = model.FromDOB;
                RegistrationForm.ToDOB = model.ToDOB;
                RegistrationForm.FormCharge = model.FormCharge;
                RegistrationForm.AllowStudentPhoto = model.AllowStudentPhoto;
                RegistrationForm.AllowFathersPhoto = model.AllowFathersPhoto;
                RegistrationForm.AllowMothersPhoto = model.AllowMothersPhoto;
                RegistrationForm.AllowSameSchool = model.AllowSameSchool;
                RegistrationForm.NoticeDetail = model.NoticeDetail;
                RegistrationForm.SameSchoolFormCharge = model.SameSchoolFormCharge;
                RegistrationForm.LastUpdatedBy = SessionUtilies.GetLoginId();
                RegistrationForm.LastUpdatedOn = DateTime.Now;

                string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];

                if (!string.IsNullOrEmpty(model.ThumbnailPhoto) && model.ThumbnailPhoto != RegistrationForm.ThumbnailPhoto)
                {
                    var path = HttpContext.Current.Server.MapPath("~/Content/Photos/");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    string pic = Guid.NewGuid() + ".jpg";
                    byte[] imageBytes = Convert.FromBase64String(model.ThumbnailPhoto.ToString());

                    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

                    image.Save(path + pic);
                    RegistrationForm.ThumbnailPhoto = BaseUrl + "/Content/Photos/" + pic;
                }

                RegistrationForm.RegistrationFormStatus = (byte)RegistrationFormStatus.Active;
                if (model.ToDate.HasValue)
                    if (model.ToDate.Value.Date < DateTime.Now.Date)
                        RegistrationForm.RegistrationFormStatus = (byte)RegistrationFormStatus.Inactive;

                if (RegistrationForm.RegistrationFormId == 0)
                {
                    RegistrationForm.CreatedBy = SessionUtilies.GetLoginId();
                    RegistrationForm.CreatedOn = DateTime.Now;
                    dataContext.RegistrationForms.InsertOnSubmit(RegistrationForm);
                }
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Registration Form is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteRegistrationForm")]
        public ExpandoObject DeleteRegistrationForm(int RegistrationFormId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var RegistrationForm = dataContext.RegistrationForms.Where(x => x.RegistrationFormId == RegistrationFormId).First();
                int AcademicSessionId = RegistrationForm.AcademicSessionId;
                dataContext.RegistrationForms.DeleteOnSubmit(RegistrationForm);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Registration Form is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
