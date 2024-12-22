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
    [RoutePrefix("api/BookType")]
    public class BookTypeController : ApiController
    {
        [HttpGet]
        [Route("BookTypeList")]
        public ExpandoObject BookTypeList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.BookTypeList = LibraryData.BookTypeList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveBookType")]
        public ExpandoObject SaveBookType(BookTypeModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                BookType BookType = new BookType();
                if (model.BookTypeId > 0)
                    BookType = dataContext.BookTypes.Where(x => x.BookTypeId == model.BookTypeId).First();
                BookType.BookTypeId = model.BookTypeId;
                BookType.BookTypeName = model.BookTypeName;
                BookType.BookTypeStatus = model.BookTypeStatus;
                if (BookType.BookTypeId == 0)
                    dataContext.BookTypes.InsertOnSubmit(BookType);
                dataContext.SubmitChanges();
                ResponseMessage.BookTypeList = LibraryData.BookTypeList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Book Type is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteBookType")]
        public ExpandoObject DeleteBookType(int BookTypeId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var BookType = dataContext.BookTypes.Where(x => x.BookTypeId == BookTypeId).First();
                dataContext.BookTypes.DeleteOnSubmit(BookType);
                dataContext.SubmitChanges();
                ResponseMessage.BookTypeList = LibraryData.BookTypeList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Book Type is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
