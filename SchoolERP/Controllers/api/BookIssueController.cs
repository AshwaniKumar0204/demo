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
    [RoutePrefix("api/BookIssue")]
    public class BookIssueController : ApiController
    {
        [HttpGet]
        [Route("BookIssueHistory")]
        public ExpandoObject BookIssueHistory(DateTime FromDate, DateTime ToDate, byte BookAccessionStatus)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                var list = from b1 in dataContext.BookIssues
                           join b2 in dataContext.BookAccessions
                           on b1.BookAccessionId equals b2.BookAccessionId
                           join b3 in dataContext.Books
                           on b2.BookId equals b3.BookId
                           //join cl in dataContext.Classes
                           //on b3.ClassNo equals cl.ClassNo into subClasses
                           join s1 in dataContext.Staffs
                           on b1.StaffId equals s1.StaffId into subStaff
                           join s2 in dataContext.Students
                           on b1.StudentId equals s2.StudentId into subStudent
                           where b1.DateOfIssue.Date >= FromDate.Date && b1.DateOfIssue.Date <= ToDate.Date
                           && (BookAccessionStatus == 0 || (BookAccessionStatus == 1 ? b1.ReturnDate == null : b1.ReturnDate != null))
                           orderby b1.ReturnDate
                           select new
                           {
                               Book = b3.BookName + (b3.Author != null ? " - " + b3.Author : "") + (b3.Publisher != null ? " - " + b3.Publisher : "") + (b3.PublishingYear != null ? " - " + b3.PublishingYear : ""),
                               b3.BookName,
                               b3.Author,
                               b3.Publisher,
                               b3.PublishingYear,
                               b3.Edition,
                               b1.DateOfIssue,
                               b1.DueDate,
                               b2.AccessionNo,
                               b1.BookIssueId,
                               b1.IssueRemarks,
                               b1.ReturnDate,
                               b1.ReturnRemarks,
                               b1.FineAmount,
                               IsReturn = b1.ReturnDate.HasValue,
                               Staff = subStaff.Any() ? subStaff.Select(x => new { x.StaffName, x.StaffCode }).First() : null,
                               Student = subStudent.Any() ? subStudent.Select(x => new { x.FullName, x.AdmissionNo }).First() : null,
                               BookIssueTypeName = Enum.GetName(typeof(BookIssueType), b1.BookIssueType),
                               b1.BookIssueType
                           };

                ResponseMessage.BookIssueList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
        [HttpGet]
        [Route("SearchBookIssueList")]
        public ExpandoObject SearchBookIssueList()
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.BookIssues
                           join b2 in dataContext.BookAccessions
                           on b1.BookAccessionId equals b2.BookAccessionId
                           join b3 in dataContext.Books
                           on b2.BookId equals b3.BookId
                           join cl in dataContext.Classes
                           on b3.ClassNo equals cl.ClassNo into subClasses
                           join s1 in dataContext.Staffs
                           on b1.StaffId equals s1.StaffId into subStaff
                           join s2 in dataContext.Students
                           on b1.StudentId equals s2.StudentId into subStudent
                           where b1.ReturnDate == null
                           orderby b2.AccessionNo
                           select new
                           {
                               SearchBook = b2.AccessionNo + " (" + b3.BookName + (subClasses.Any() ? " " + subClasses.First().ClassName2 : "") + (b3.Author != null ? " " + b3.Author : "") + (b3.Publisher != null ? " " + b3.Publisher : "") + (b3.PublishingYear != null ? " " + b3.PublishingYear : "") + ")",
                               b3.BookName,
                               b3.Author,
                               b3.Publisher,
                               b3.PublishingYear,
                               b3.Edition,
                               b1.DateOfIssue,
                               b1.DueDate,
                               b2.AccessionNo,
                               b1.BookIssueId,
                               b1.IssueRemarks,
                               b1.StudentId,
                               b1.StaffId,
                               DueDays = DateTime.Now.DayOfYear - b1.DueDate.DayOfYear,
                               IsReturn = b1.ReturnDate.HasValue,
                               Staff = subStaff.Any() ? subStaff.Select(x => new { x.StaffName, x.StaffCode }).First() : null,
                               Student = subStudent.Any() ? subStudent.Select(x => new { x.FullName, x.AdmissionNo }).First() : null,
                               dataContext.BookIssueDetails.First(x => x.IssuedBy == (b1.StudentId.HasValue ? (byte)IssuedBy.Student : (byte)IssuedBy.Staff)).FinePerDay
                           };
                ResponseMessage.BookIssueList = list.Distinct().ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("BookIssueList")]
        public ExpandoObject BookIssueList(int StudentId, int StaffId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from b1 in dataContext.BookIssues
                           join b2 in dataContext.BookAccessions
                           on b1.BookAccessionId equals b2.BookAccessionId
                           join b3 in dataContext.Books
                           on b2.BookId equals b3.BookId
                           join cl in dataContext.Classes
                           on b3.ClassNo equals cl.ClassNo into subClasses
                           where StudentId == 0 ? b1.StaffId == StaffId : b1.StudentId == StudentId
                           orderby b1.ReturnDate, b1.DateOfIssue
                           select new
                           {
                               Book = b3.BookName + (b3.Author != null ? " " + b3.Author : "") + (b3.Publisher != null ? " " + b3.Publisher : "") + (b3.PublishingYear != null ? " " + b3.PublishingYear : ""),
                               b1.DateOfIssue,
                               b1.DueDate,
                               b1.ReturnDate,
                               b1.FineAmount,
                               b2.AccessionNo,
                               IsRetutrn = b1.ReturnDate.HasValue,
                               b1.IssueRemarks,
                               b1.ReturnRemarks,
                               b1.BookIssueId,
                               b1.BookAccessionId,
                               b1.StaffId,
                               b1.StudentId,
                               BookIssueTypeName = Enum.GetName(typeof(BookIssueType), b1.BookIssueType),
                               b1.BookIssueType
                           };

                ResponseMessage.BookIssueList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("BookIssue")]
        public ExpandoObject BookIssue(BookIssueModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                BookIssue BookIssue = null;
                if (model.BookIssueId > 0)
                {
                    BookIssue = dataContext.BookIssues.Where(x => x.BookIssueId == model.BookIssueId).First();
                    BookIssue.LastUpdatedBy = SessionUtilies.GetLoginId();
                    BookIssue.LastUpdatedOn = DateTime.Now;
                }
                else
                {
                    //Check Book
                    var bookAccession = dataContext.BookAccessions.Where(x => x.BookAccessionId == model.BookAccessionId).First();
                    if (bookAccession.BookAccessionStatus != (byte)BookAccessionStatus.Available)
                        throw new Exception("This book is already issued.");
                    if (bookAccession.BookPosition == (byte)BookPosition.Damage)
                        throw new Exception("This book is Damage.");
                    if (bookAccession.BookPosition == (byte)BookPosition.Lost)
                        throw new Exception("This book is Lost.");
                    if (bookAccession.BookPosition == (byte)BookPosition.WriteOff)
                        throw new Exception("This book is Write Off.");
                    int DueDays = 0;
                    if (model.StudentId.HasValue)
                        DueDays = dataContext.BookIssueDetails.First(x => x.IssuedBy == (byte)IssuedBy.Student).DueDays;
                    else
                        DueDays = dataContext.BookIssueDetails.First(x => x.IssuedBy == (byte)IssuedBy.Staff).DueDays;

                    BookIssue = new BookIssue
                    {
                        BookAccessionId = model.BookAccessionId,
                        StudentId = model.StudentId,
                        StaffId = model.StaffId,
                        BookIssueType = (byte)BookIssueType.Issue,
                        DateOfIssue = model.DateOfIssue,
                        DueDate = model.DateOfIssue.AddDays(DueDays),
                        CreatedBy = SessionUtilies.GetLoginId(),
                        CreatedOn = DateTime.Now
                    };
                    bookAccession.BookAccessionStatus = (byte)BookAccessionStatus.Issued;
                }
                //BookIssue.BookIssueType = model.BookIssueType;
                BookIssue.IssueRemarks = model.IssueRemarks;
                if (BookIssue.BookIssueId == 0)
                    dataContext.BookIssues.InsertOnSubmit(BookIssue);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("BookReturn")]
        public ExpandoObject BookReturn(BookIssueModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //Check Book
                BookIssue BookIssue = dataContext.BookIssues.Where(x => x.BookIssueId == model.BookIssueId).First();

                var bookAccession = dataContext.BookAccessions.Where(x => x.BookAccessionId == BookIssue.BookAccessionId).First();
                if (bookAccession.BookAccessionStatus == (byte)BookAccessionStatus.Available)
                    throw new Exception("This book is not issued.");

                BookIssue.LastUpdatedBy = SessionUtilies.GetLoginId();
                BookIssue.LastUpdatedOn = DateTime.Now;
                BookIssue.ReturnDate = model.ReturnDate;
                BookIssue.FineAmount = model.FineAmount;
                BookIssue.ReturnRemarks = model.ReturnRemarks;
                dataContext.SubmitChanges();

                bookAccession.BookAccessionStatus = (byte)BookAccessionStatus.Available;
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("BookReissue")]
        public ExpandoObject BookReissue(BookIssueModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //Check Book
                BookIssue BookIssue = dataContext.BookIssues.Where(x => x.BookIssueId == model.BookIssueId).First();
                var bookAccession = dataContext.BookAccessions.Where(x => x.BookAccessionId == BookIssue.BookAccessionId).First();
                if (bookAccession.BookAccessionStatus == (byte)BookAccessionStatus.Available)
                    throw new Exception("This book is not issued.");

                BookIssue.LastUpdatedBy = SessionUtilies.GetLoginId();
                BookIssue.LastUpdatedOn = DateTime.Now;
                BookIssue.ReturnDate = model.ReturnDate;
                BookIssue.FineAmount = model.FineAmount;
                BookIssue.ReturnRemarks = model.ReturnRemarks;
                dataContext.SubmitChanges();

                int DueDays = 0;
                if (BookIssue.StudentId.HasValue)
                    DueDays = dataContext.BookIssueDetails.First(x => x.IssuedBy == (byte)IssuedBy.Student).DueDays;
                else
                    DueDays = dataContext.BookIssueDetails.First(x => x.IssuedBy == (byte)IssuedBy.Staff).DueDays;

                BookIssue NewBookIssue = new BookIssue
                {
                    BookAccessionId = BookIssue.BookAccessionId,
                    StudentId = BookIssue.StudentId,
                    StaffId = BookIssue.StaffId,
                    BookIssueType = (byte)BookIssueType.Reissue,
                    DateOfIssue = model.ReturnDate,
                    DueDate = model.ReturnDate.AddDays(DueDays),
                    CreatedBy = SessionUtilies.GetLoginId(),
                    CreatedOn = DateTime.Now,
                    IssueRemarks = model.ReturnRemarks
                };
                dataContext.BookIssues.InsertOnSubmit(NewBookIssue);
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
        [Route("deleteBookIssue")]
        public ExpandoObject DeleteBookIssue(int BookIssueId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var BookIssue = dataContext.BookIssues.Where(x => x.BookIssueId == BookIssueId).First();
                dataContext.BookIssues.DeleteOnSubmit(BookIssue);

                var bookAccession = dataContext.BookAccessions.Where(x => x.BookAccessionId == BookIssue.BookAccessionId).First();
                bookAccession.BookAccessionStatus = (byte)BookAccessionStatus.Available;
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Record is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
