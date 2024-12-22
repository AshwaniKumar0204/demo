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
    [RoutePrefix("api/StudentSubjectCombination")]
    public class StudentSubjectCombinationController : ApiController
    {
        [HttpPost]
        [Route("SaveStudentSubjectCombination")]
        public ExpandoObject SaveStudentSubjectCombination(StudentSubjectCombinationModel StudentSubjectCombinationModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StudentSubjectCombination StudentSubjectCombination = null;
                if (StudentSubjectCombinationModel.StudentSubjectCombinationId > 0)
                    StudentSubjectCombination = dataContext.StudentSubjectCombinations.Where(x => x.StudentSubjectCombinationId == StudentSubjectCombinationModel.StudentSubjectCombinationId).First();
                else
                    StudentSubjectCombination = new StudentSubjectCombination();
                StudentSubjectCombination.AdmissionId = StudentSubjectCombinationModel.AdmissionId;
                StudentSubjectCombination.SubjectCombinationId = StudentSubjectCombinationModel.SubjectCombinationId;
                StudentSubjectCombination.StartDate = StudentSubjectCombinationModel.StartDate;
                StudentSubjectCombination.EndDate = StudentSubjectCombinationModel.EndDate;

                if (StudentSubjectCombination.StudentSubjectCombinationId == 0)
                    dataContext.StudentSubjectCombinations.InsertOnSubmit(StudentSubjectCombination);
                dataContext.SubmitChanges();

                response.StudentSubjectCombinationList = LoadData.GetStudentSubjectCombinationList(StudentSubjectCombinationModel.AdmissionId, dataContext);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("StudentSubjectCombinationList")]
        public ExpandoObject StudentSubjectCombinationList(int AdmissionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                response.StudentSubjectCombinationList = LoadData.GetStudentSubjectCombinationList(AdmissionId, dataContext);
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("DeleteStudentSubjectCombination")]
        public ExpandoObject DeleteStudentSubjectCombination(int StudentSubjectCombinationId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                StudentSubjectCombination StudentSubjectCombination = dataContext.StudentSubjectCombinations.Where(x => x.StudentSubjectCombinationId == StudentSubjectCombinationId).First();
                int AdmissionId = StudentSubjectCombination.AdmissionId;
                dataContext.StudentSubjectCombinations.DeleteOnSubmit(StudentSubjectCombination);
                dataContext.SubmitChanges();
                response.StudentSubjectCombinationList = LoadData.GetStudentSubjectCombinationList(AdmissionId, dataContext);
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
