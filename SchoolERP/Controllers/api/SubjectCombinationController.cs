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
    [RoutePrefix("api/SubjectCombination")]
    public class SubjectCombinationController : ApiController
    {
        [HttpPost]
        [Route("SaveSubjectCombination")]
        public ExpandoObject SaveSubjectCombination(SubjectCombinationModel SubjectCombinationModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SubjectCombination SubjectCombination = null;
                if (SubjectCombinationModel.SubjectCombinationId > 0)
                    SubjectCombination = dataContext.SubjectCombinations.Where(x => x.SubjectCombinationId == SubjectCombinationModel.SubjectCombinationId).First();
                else
                    SubjectCombination = new SubjectCombination();
                SubjectCombination.AcademicSessionId = SubjectCombinationModel.AcademicSessionId;
                SubjectCombination.ClassId = SubjectCombinationModel.ClassId;
                SubjectCombination.SubjectCombinationName = SubjectCombinationModel.SubjectCombinationName;

                if (SubjectCombination.SubjectCombinationId == 0)
                    dataContext.SubjectCombinations.InsertOnSubmit(SubjectCombination);
                dataContext.SubmitChanges();

                response.SubjectCombinationList = LoadData.GetSubjectCombinationList(dataContext);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("SubjectCombinationList")]
        public ExpandoObject SubjectCombinationList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                response.SubjectCombinationList = LoadData.GetSubjectCombinationList(dataContext);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("DeleteSubjectCombination")]
        public ExpandoObject DeleteSubjectCombination(int SubjectCombinationId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SubjectCombination SubjectCombination = dataContext.SubjectCombinations.Where(x => x.SubjectCombinationId == SubjectCombinationId).First();
                dataContext.SubjectCombinations.DeleteOnSubmit(SubjectCombination);
                dataContext.SubmitChanges();
                response.SubjectCombinationList = LoadData.GetSubjectCombinationList(dataContext);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    response.Message = "This is in use, so can not delete.";
                else
                    response.Message = ex.Message;
            }
            return response;
        }
    }
}
