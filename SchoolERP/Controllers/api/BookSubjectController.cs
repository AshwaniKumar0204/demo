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
    [RoutePrefix("api/BookSubject")]
    public class BookSubjectController : ApiController
    {
        [HttpGet]
        [Route("BookSubjectList")]
        public ExpandoObject BookSubjectList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.BookSubjectList = LibraryData.BookSubjectList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveBookSubject")]
        public ExpandoObject SaveBookSubject(BookSubjectModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                BookSubject BookSubject = new BookSubject();
                if (model.BookSubjectId > 0)
                    BookSubject = dataContext.BookSubjects.Where(x => x.BookSubjectId == model.BookSubjectId).First();
                BookSubject.BookSubjectId = model.BookSubjectId;
                BookSubject.SubjectName = model.SubjectName;
                BookSubject.SubjectStatus = model.SubjectStatus;
                if (BookSubject.BookSubjectId == 0)
                    dataContext.BookSubjects.InsertOnSubmit(BookSubject);
                dataContext.SubmitChanges();
                ResponseMessage.BookSubjectList = LibraryData.BookSubjectList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Subject is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteBookSubject")]
        public ExpandoObject DeleteBookSubject(int BookSubjectId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var BookSubject = dataContext.BookSubjects.Where(x => x.BookSubjectId == BookSubjectId).First();
                dataContext.BookSubjects.DeleteOnSubmit(BookSubject);
                dataContext.SubmitChanges();
                ResponseMessage.BookSubjectList = LibraryData.BookSubjectList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Subject is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
