using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [RoutePrefix("api/StudentDocument")]
    public class StudentDocumentController : ApiController
    {
        [HttpGet]
        [Route("Studentlist")]
        public ExpandoObject Studentlist()
        {
            dynamic responseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var data = (from s1 in dataContext.Students
                            select new
                            {
                                s1.StudentId,
                                s1.AdmissionNo,
                            }).ToList(); 
                responseMessage.documetlist = data;
                responseMessage.Message = "Success";
            }
            catch(Exception ex)
            {
                responseMessage.Message = ex.Message;
            }
            return responseMessage;

        }
        [HttpPost]
        [Route("saveDocument")]
        public ExpandoObject saveDocument(DocumentModel Model)//SaveAd
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StudentDocument table = new StudentDocument();
                if (Model.DocumentID > 0)
                {
                    table = dataContext.StudentDocuments.Where(x => x.DocumentID == Model.DocumentID).FirstOrDefault();
                    table.StudentId = Model.StudentId;
                    
                    if (!string.IsNullOrEmpty(Model.AddharCard))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.AddharCard.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.AddharCard = "/Content/Files/" + pic;
                        table.AddharCard = Model.AddharCard;
                    }
                  
                    if (!string.IsNullOrEmpty(Model.MatrixCertifiacate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.MatrixCertifiacate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.MatrixCertifiacate = "/Content/Files/" + pic;
                        table.MatrixCertifiacate = Model.MatrixCertifiacate;
                    }
                  
                    if (!string.IsNullOrEmpty(Model.IntermediateCertificate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.IntermediateCertificate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.IntermediateCertificate = "/Content/Files/" + pic;
                        table.IntermediateCertificate = Model.IntermediateCertificate;
                    }
                    if (!string.IsNullOrEmpty(Model.MigrationCertificate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.MigrationCertificate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.MigrationCertificate = "/Content/Files/" + pic;
                        table.MigrationCertificate = Model.MigrationCertificate;
                    }



                    if (!string.IsNullOrEmpty(Model.CLC))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.CLC.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.CLC = "/Content/Files/" + pic;
                        table.CLC = Model.CLC;
                    }


                    if (!string.IsNullOrEmpty(Model.TwelvePassingCertificate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.TwelvePassingCertificate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.TwelvePassingCertificate = "/Content/Files/" + pic;
                        table.TwelvePassingCertificate = Model.TwelvePassingCertificate;
                    }

                    if (!string.IsNullOrEmpty(Model.TenPassingCertificate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.TenPassingCertificate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.TenPassingCertificate = "/Content/Files/" + pic;
                        table.TenPassingCertificate = Model.TenPassingCertificate;
                    }
                    table.DocumentStatus = Model.DocumentStatus;

                }
                else
                {
                    table.StudentId = Model.StudentId;
                    table.DocumentStatus = Model.DocumentStatus;
                    if (!string.IsNullOrEmpty(Model.AddharCard))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.AddharCard.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.AddharCard = "/Content/Files/" + pic;
                        table.AddharCard = Model.AddharCard;

                    }

                    if (!string.IsNullOrEmpty(Model.MatrixCertifiacate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.MatrixCertifiacate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.MatrixCertifiacate = "/Content/Files/" + pic;
                        table.MatrixCertifiacate = Model.MatrixCertifiacate;
                    }

                    if (!string.IsNullOrEmpty(Model.IntermediateCertificate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.IntermediateCertificate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.IntermediateCertificate = "/Content/Files/" + pic;
                        table.IntermediateCertificate = Model.IntermediateCertificate;
                    }


                    if (!string.IsNullOrEmpty(Model.MigrationCertificate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.MigrationCertificate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.MigrationCertificate = "/Content/Files/" + pic;
                        table.MigrationCertificate = Model.MigrationCertificate;
                    }



                    if (!string.IsNullOrEmpty(Model.CLC))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.CLC.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.CLC = "/Content/Files/" + pic;
                        table.CLC = Model.CLC;
                    }


                    if (!string.IsNullOrEmpty(Model.TwelvePassingCertificate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.TwelvePassingCertificate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.TwelvePassingCertificate = "/Content/Files/" + pic;
                        table.TwelvePassingCertificate = Model.TwelvePassingCertificate;
                    }

                    if (!string.IsNullOrEmpty(Model.TenPassingCertificate))
                    {
                        var path = HttpContext.Current.Server.MapPath("~/Content/Files/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string pic = Guid.NewGuid() + "." + Model.UploadFileFormat;
                        byte[] imageBytes = Convert.FromBase64String(Model.TenPassingCertificate.ToString());

                        var fileStream = new FileStream(path + pic, FileMode.Create, FileAccess.ReadWrite);
                        fileStream.Write(imageBytes, 0, imageBytes.Length);
                        fileStream.Close();
                        Model.TenPassingCertificate = "/Content/Files/" + pic;
                        table.TenPassingCertificate = Model.TenPassingCertificate;
                    }
                    dataContext.StudentDocuments.InsertOnSubmit(table);
                };

                dataContext.SubmitChanges();
                response.Message = "Success";

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }


        [HttpGet]
        [Route("deleteDocument")]

        public ExpandoObject deleteDocument(int DocumentID)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            try
            {

                StudentDocument table = new StudentDocument();
                table = dataContext.StudentDocuments.Where(x => x.DocumentID == DocumentID).First();
                dataContext.StudentDocuments.DeleteOnSubmit(table);
                dataContext.SubmitChanges();
                response.message = "Success";
            }
            catch (Exception Ex)
            {
                response.message = Ex.Message;
            }
            return response;
        }


        [HttpGet]
        [Route("StudentDocumentList")]
        public ExpandoObject StudentDocumentList()
        {
            dynamic responseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var data = (from s1 in dataContext.StudentDocuments
                            join sd in dataContext.Students on s1.StudentId equals sd.StudentId
                            select new
                            {
                             
                              sd.StudentId,
                                sd.AdmissionNo,
                                s1.DocumentID,
                                sd.FullName,
                                s1.IntermediateCertificate,
                                s1.MatrixCertifiacate,
                                s1.AddharCard,
                                s1.DocumentStatus,
                                s1.CLC,
                                s1.MigrationCertificate,
                                s1.TenPassingCertificate,
                                s1.TwelvePassingCertificate,

                            }).ToList();
                responseMessage.Documentlist = data;
                responseMessage.Message = "Success";
            }
            catch (Exception ex)
            {
                responseMessage.Message = ex.Message;
            }
            return responseMessage;

        }
    }
}
