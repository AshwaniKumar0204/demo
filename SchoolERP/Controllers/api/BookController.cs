
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
    [RoutePrefix("api/Book")]
    public class BookController : ApiController
    {
        [HttpGet]
        [Route("BookList")]
        public ExpandoObject BookList(byte Status, int ClassNo, int BookTypeId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.Books
                           join cl in dataContext.Classes
                           on b1.ClassNo equals cl.ClassNo into subClasses
                           join cl2 in dataContext.Classes
                           on b1.ClassNo2 equals cl2.ClassNo into subClasses2
                           join b2 in dataContext.BookTypes
                           on b1.BookTypeId equals b2.BookTypeId into subBookTypes
                           from b3 in subBookTypes.DefaultIfEmpty()
                           where (ClassNo == 0 ? true : (b1.ClassNo == ClassNo || b1.ClassNo2 == ClassNo))
                           && (Status == 0 ? true : b1.BookStatus == Status)
                           && (BookTypeId == 0 ? true : b1.BookTypeId == BookTypeId)
                           orderby b1.ClassNo, b1.BookName, b1.Publisher, b1.Author
                           select new
                           {
                               b1.BookId,
                               b1.BookName,
                               b1.Author,
                               b1.Edition,
                               b1.Publisher,
                               b1.PublishingYear,
                               ClassName = (subClasses.Any() ? subClasses.First().ClassName2 : "") + (subClasses2.Any() ? ", " + subClasses2.First().ClassName2 : ""),
                               b1.ClassNo,
                               b1.ClassNo2,
                               b1.TotalPages,
                               b1.Volume,
                               b1.SubjectType,
                               b1.BookStatus,
                               b1.BookTypeId,
                               b3.BookTypeName,
                               b1.BookSubjectId,
                               b1.BookSubject.SubjectName,
                               BookStatusName = Enum.GetName(typeof(BookStatus), b1.BookStatus),
                               IssuedQuantity = dataContext.BookAccessions.Where(x => x.BookId == b1.BookId && x.BookAccessionStatus == (byte)BookAccessionStatus.Issued).Count(),
                               AvailableQuantity = dataContext.BookAccessions.Where(x => x.BookId == b1.BookId && x.BookAccessionStatus == (byte)BookAccessionStatus.Available).Count(),
                           };
                response.BookList = list.Distinct().ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("SearchBookList")]
        public ExpandoObject SearchBookList(byte status)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.Books
                           where b1.BookStatus == status
                           select new
                           {
                               b1.BookId,
                               b1.Author,
                               b1.Publisher,
                               b1.SubjectType,
                               SearchBook = b1.BookName + (b1.Author != null ? " " + b1.Author : "") + (b1.Publisher != null ? " " + b1.Publisher : "") + (b1.PublishingYear != null ? " " + b1.PublishingYear : "")
                           };
                response.BookList = list.Distinct().ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("SearchBookListClasswise")]
        public ExpandoObject SearchBookListClasswise(byte status,int ClassNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.Books
                           where b1.BookStatus == status
                           && b1.ClassNo == ClassNo
                           select new
                           {
                               b1.BookId,
                               b1.Author,
                               b1.Publisher,
                               b1.SubjectType,
                               SearchBook = b1.BookName + (b1.Author != null ? " " + b1.Author : "") + (b1.Publisher != null ? " " + b1.Publisher : "") + (b1.PublishingYear != null ? " " + b1.PublishingYear : "")
                           };
                response.BookList = list.Distinct().ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveBook")]
        public ExpandoObject SaveBook(BookModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //if (model.BookSubjectId == null)
                //{
                if (model.SubjectType != null && model.SubjectType != "")
                {
                    var bookSubjects = dataContext.BookSubjects.Where(x => x.SubjectName == model.SubjectType);
                    if (bookSubjects.Any())
                    {
                        model.BookSubjectId = bookSubjects.First().BookSubjectId;
                    }
                    else
                    {
                        BookSubject bookSubject = new BookSubject
                        {
                            SubjectName = model.SubjectType,
                            SubjectStatus = 1
                        };
                        dataContext.BookSubjects.InsertOnSubmit(bookSubject);
                        dataContext.SubmitChanges();
                        model.BookSubjectId = bookSubject.BookSubjectId;
                    }
                }
                //}
                Book Book = new Book();
                if (model.BookId > 0)
                    Book = dataContext.Books.Where(x => x.BookId == model.BookId).First();
                Book.BookName = model.BookName;
                Book.Author = model.Author;
                Book.Edition = model.Edition;
                Book.Publisher = model.Publisher;
                Book.PublishingYear = model.PublishingYear;
                Book.SubjectType = model.SubjectType;
                Book.TotalPages = model.TotalPages;
                Book.Volume = model.Volume;
                Book.ClassNo = model.ClassNo;
                Book.ClassNo2 = model.ClassNo2;
                if (Book.ClassNo != null)
                    Book.ClassNames = dataContext.Classes.Where(x => x.ClassNo == model.ClassNo).First().ClassName2;
                Book.BookStatus = model.BookStatus;
                Book.BookTypeId = model.BookTypeId;
                Book.BookSubjectId = model.BookSubjectId;
                Book.LastUpdatedBy = SessionUtilies.GetLoginId();
                Book.LastUpdatedOn = DateTime.Now;
                if (Book.BookId == 0)
                {
                    Book.CreatedBy = SessionUtilies.GetLoginId();
                    Book.CreatedOn = DateTime.Now;
                    dataContext.Books.InsertOnSubmit(Book);
                }
                dataContext.SubmitChanges();
                ResponseMessage.BookId = Book.BookId;
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This book is already exist.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteBook")]
        public ExpandoObject DeleteBook(int BookId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Book = dataContext.Books.Where(x => x.BookId == BookId).First();
                dataContext.Books.DeleteOnSubmit(Book);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This record is in use, so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }


            return ResponseMessage;
        }

        [HttpGet]
        [Route("AuthorList")]
        public ExpandoObject AuthorList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = dataContext.Books.Select(x => new { x.Author }).Distinct().OrderBy(x => x.Author).ToList();
                response.AuthorList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("PublisherList")]
        public ExpandoObject PublisherList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = dataContext.Books.Select(x => new { x.Publisher }).Distinct().OrderBy(x => x.Publisher).ToList();
                response.PublisherList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("SubjectTypeList")]
        public ExpandoObject SubjectTypeList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = dataContext.Books.Select(x => new { x.SubjectType }).Distinct().OrderBy(x => x.SubjectType).ToList();
                response.SubjectTypeList = list;
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
