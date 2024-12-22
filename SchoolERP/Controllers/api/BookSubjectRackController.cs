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
    [RoutePrefix("api/BookSubjectRack")]
    public class BookSubjectRackController : ApiController
    {
        [HttpGet]
        [Route("BookSubjectRackList")]
        public ExpandoObject BookSubjectRackList(int BookSubjectId, int RackId, int ClassNo)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from r1 in dataContext.BookSubjectRacks
                           join c1 in dataContext.Classes
                           on r1.ClassNo equals c1.ClassNo into subClasses
                           where (ClassNo == 0 ? true : r1.ClassNo == ClassNo)
                           && (RackId == 0 ? true : r1.RackId == RackId)
                           && (BookSubjectId == 0 ? true : r1.BookSubjectId == BookSubjectId)
                           select new BookSubjectRackModel
                           {
                               RackId = r1.RackId,
                               ClassName = subClasses.Any() ? subClasses.First().ClassName2 : "",
                               ClassNo = r1.ClassNo,
                               BookSubjectId = r1.BookSubjectId,
                               BookSubjectRackId = r1.BookSubjectRackId,
                               RackName = r1.Rack.RackName,
                               SubjectName = r1.BookSubject.SubjectName
                           };
                ResponseMessage.BookSubjectRackList = list.ToList();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveBookSubjectRack")]
        public ExpandoObject SaveBookSubjectRack(BookSubjectRackModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                BookSubjectRack BookSubjectRack = new BookSubjectRack();
                if (model.BookSubjectRackId > 0)
                    BookSubjectRack = dataContext.BookSubjectRacks.Where(x => x.BookSubjectRackId == model.BookSubjectRackId).First();
                BookSubjectRack.RackId = model.RackId;
                BookSubjectRack.BookSubjectId = model.BookSubjectId;
                BookSubjectRack.ClassNo = model.ClassNo;
                if (BookSubjectRack.BookSubjectRackId == 0)
                    dataContext.BookSubjectRacks.InsertOnSubmit(BookSubjectRack);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This record is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteBookSubjectRack")]
        public ExpandoObject DeleteBookSubjectRack(int BookSubjectRackId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var BookSubjectRack = dataContext.BookSubjectRacks.Where(x => x.BookSubjectRackId == BookSubjectRackId).First();
                dataContext.BookSubjectRacks.DeleteOnSubmit(BookSubjectRack);
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This record is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }
    }
}
