using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/Teacher")]
    public class TeacherController : ApiController
    {
        [HttpGet]
        [Route("TeacherList")]
        public ExpandoObject TeacherList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                ResponseMessage.TeacherList = LoadData.GetTeacherList(dataContext, Status);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }


        [HttpGet]
        [Route("SearchTeacherList")]
        public ExpandoObject SearchTeacherList(byte Status)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from t1 in dataContext.Teachers
                            where Status > 0 ? t1.TeacherStatus == Status : true
                            orderby t1.TeacherName
                            select new
                            {
                                SearchTeacher = t1.TeacherName + " (" + t1.TeacherCode + ")",
                                t1.TeacherCode,
                                t1.TeacherName,
                                t1.TeacherId,
                            });
                ResponseMessage.TeacherList = list;
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

        [HttpPost]
        [Route("saveTeacher")]
        public ExpandoObject SaveTeacher(TeacherModel model)
        {
            dynamic ResponseMessage = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            int y = 0;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                Staff Staff = new Staff();
                if (model.StaffId > 0)
                    Staff = dataContext.Staffs.Where(x => x.StaffId == model.StaffId).First();
                Staff.Address = model.CurrentAddress;
                Staff.AlternateNo = model.AlternateNo;
                Staff.DateOfBirth = model.DateOfBirth;
                Staff.EmailId = model.Email;
                Staff.StaffName = model.TeacherName;
                Staff.StaffStatus = model.TeacherStatus;
                Staff.DepartmentId = Convert.ToInt32(model.DepartmentId);
                Staff.Gender =Convert.ToInt32(model.Gender);
                Staff.JoinDate = model.JoinDate;
                Staff.MobileNo = model.MobileNo;
                Staff.StaffType = (Byte)StaffType.Teacher;
                if (Staff.StaffId == 0)
                {
                    Staff.CreatedOn = DateTime.Now;
                    Staff.CreatedBy = SessionUtilies.GetLoginId();
                    Staff.StaffCode = LoadData.GenerateStaffCode(dataContext);
                    dataContext.Staffs.InsertOnSubmit(Staff);
                }
                else
                {
                    Staff.LastUpdatedBy = SessionUtilies.GetLoginId();
                    Staff.LastUpdatedOn = DateTime.Now;
                }
                dataContext.SubmitChanges();
                ResponseMessage.Message = ConstantData.SuccessMessage;

                Teacher Teacher = new Teacher();
                if (model.TeacherId > 0)
                    Teacher = dataContext.Teachers.Where(x => x.TeacherId == model.TeacherId).First();

                Teacher.TeacherId = model.TeacherId;
                Teacher.StaffId = Staff.StaffId;
                Teacher.TeacherName = model.TeacherName;
                Teacher.TeacherCode = model.TeacherCode;
                Teacher.TeacherStatus = model.TeacherStatus;
                Teacher.Arrangement = model.Arrangement;
                //Teacher.AlternateNo = model.AlternateNo;
                Teacher.CurrentAddress = model.CurrentAddress;
                //Teacher.Email = model.Email;
                Teacher.Qualification = model.Qualification;
                Teacher.FatherName = model.FatherName;
                //Teacher.Gender = model.Gender;
                //Teacher.MobileNo = model.MobileNo;
                Teacher.PermanentAddress = model.PermanentAddress;
                Teacher.UpdatedBy = SessionUtilies.GetLoginId();
                Teacher.UpdatedOn = DateTime.Now;
                Teacher.JoinDate = model.JoinDate;
                Teacher.IsPartTimeTeacher = model.IsPartTimeTeacher;

                if (Teacher.TeacherId == 0)
                    dataContext.Teachers.InsertOnSubmit(Teacher);
                dataContext.SubmitChanges();

                //model.TeacherSubjectList.ForEach(teacherSubjectModel =>
                //{
                //    TeacherSubject teacherSubject = null;
                //    if (teacherSubjectModel.TeacherSubjectId > 0)
                //        teacherSubject = dataContext.TeacherSubjects.Where(x => x.TeacherSubjectId == teacherSubjectModel.TeacherSubjectId).First();
                //    else
                //        teacherSubject = new TeacherSubject();
                //    teacherSubject.AcademicSessionId = 3;
                //    teacherSubject.SubjectId = teacherSubjectModel.SubjectId;
                //    teacherSubject.TeacherId = Teacher.TeacherId;
                //    teacherSubject.SectionId = teacherSubjectModel.SectionId;
                //    teacherSubject.NoOfPeriods = teacherSubjectModel.NoOfPeriods;
                //    teacherSubject.PracticalPeriods = teacherSubjectModel.PracticalPeriods;
                //    teacherSubject.TeacherSubjectStatus = teacherSubjectModel.TeacherSubjectStatus;
                //    teacherSubject.IsIncluded = teacherSubjectModel.IsIncluded;
                //    if (teacherSubject.TeacherSubjectId == 0)
                //        dataContext.TeacherSubjects.InsertOnSubmit(teacherSubject);
                //    dataContext.SubmitChanges();
                //});

                //model.TeacherPeriodList.Where(x=>x.TeacherPeriodId == 0).ToList().ForEach(teacherPriodModel =>
                //{
                //    TeacherPeriod TeacherPeriod = new TeacherPeriod
                //    {
                //        TeacherId = Teacher.TeacherId,
                //        PeriodNo = teacherPriodModel.PeriodNo,
                //        AcademicSessionId = 3
                //    };
                //    dataContext.TeacherPeriods.InsertOnSubmit(TeacherPeriod);
                //    dataContext.SubmitChanges();
                //});

                ResponseMessage.TeacherList = LoadData.GetTeacherList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
                transaction.Commit();
                y = 1;
            }
            catch (Exception ex)
            {
                if (y != 1)
                    transaction.Rollback();
                if (ex.Message.Contains("IX_ClassSubject"))
                    ResponseMessage.Message = "One Subject is already added";
                else if (ex.Message.Contains("IX"))
                    ResponseMessage.Message = "This Teacher Code is already exist";
                else
                    ResponseMessage.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return ResponseMessage;
        }

        [HttpGet]
        [Route("deleteTeacher")]
        public ExpandoObject DeleteTeacher(int TeacherId)
        {
            dynamic ResponseMessage = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Teacher = dataContext.Teachers.Where(x => x.TeacherId == TeacherId).First();
                dataContext.Teachers.DeleteOnSubmit(Teacher);
                dataContext.SubmitChanges();
                ResponseMessage.TeacherList = LoadData.GetTeacherList(dataContext, 0);
                ResponseMessage.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                    ResponseMessage.Message = "This Teacher is in use.so can't delete.";
                else
                    ResponseMessage.Message = ex.Message;
            }
            return ResponseMessage;
        }

    }
}
