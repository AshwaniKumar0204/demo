using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Dynamic;
using School;
using SchoolERP.Models;
using System.Data.Common;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/ClassPromotion")]
    public class ClassPromotionController : ApiController
    {
        [HttpGet]
        [Route("StudentPromatableList")]
        public ExpandoObject StudentPromatableList(int CurrentSectionId, int CurrentSessionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var AdmissionList = (from a1 in dataContext.Admissions
                                     join s1 in dataContext.Students
                                     on a1.StudentId equals s1.StudentId
                                     where a1.SectionId == CurrentSectionId
                                     && a1.AcademicSessionId == CurrentSessionId
                                     && s1.StudentStatus == (byte)StudentStatus.Active
                                     && a1.AdmissionStatus == (byte)AdmissionStatus.Active
                                     orderby a1.RollNo ascending
                                     select new StudentPromotionModel
                                     {
                                         AdmissionId = a1.AdmissionId,
                                         AdmissionNo = s1.AdmissionNo,
                                         RollNo = a1.RollNo,
                                         FullName = s1.FullName,
                                         IsSelected = true
                                     });
                response.AdmissionList = AdmissionList.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("VeriyfyClassPromotion")]
        public ExpandoObject VeriyfyClassPromotion(SaveClassPromotion saveClassPromotion)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var oldSection = (from os in dataContext.Sections
                                  where os.SectionId == saveClassPromotion.OldSectionId
                                  select os).First();
                var list = saveClassPromotion.StudentPromotionList;
                list.ForEach(studentPromotion =>
                {
                    var newSection = (from os in dataContext.Sections
                                      where os.SectionId == studentPromotion.NewSectionId
                                      select os).First();
                    studentPromotion.PromotionStatus = studentPromotion.IsSelected ? PromotionStatus.Promoted : PromotionStatus.NotPromoted;
                    studentPromotion.NewClassName = studentPromotion.IsSelected ? newSection.Class.ClassName : oldSection.Class.ClassName;
                    studentPromotion.NewSectionName = studentPromotion.IsSelected ? newSection.SectionName : oldSection.SectionName;
                });

                var countPromoted = (from l1 in list
                                     where l1.PromotionStatus == PromotionStatus.Promoted
                                     select l1.NewRollNo).Distinct().Count();

                var countNotPromoted = (from l1 in list
                                        where l1.PromotionStatus == PromotionStatus.NotPromoted
                                        select l1.NewRollNo).Distinct().Count();

                if (countPromoted != list.Where(x => x.PromotionStatus == PromotionStatus.Promoted).Count())
                    throw new Exception("New Assigned Roll Nos are not unique.");

                if (countNotPromoted != list.Where(x => x.PromotionStatus == PromotionStatus.NotPromoted).Count())
                    throw new Exception("New Assigned Roll Nos are not unique.");

                var arrearFeeHeads = (from afh in dataContext.ArrearFeeHeads
                                      select afh);
                if (arrearFeeHeads.Count() == 0)
                    throw new Exception("Arrear Fee Head has not been defined");
                int? ArrearFeeHeadId = arrearFeeHeads.First().FeeHeadId;

                DateTime duesDate = LoadData.CheckDate("31/03/" + DateTime.Now.Year.ToString(), "");
                list.ForEach(sp => list.Find(delegate (StudentPromotionModel s1) { return s1.AdmissionId == sp.AdmissionId; }).ArrearDues = FeeData.GetDueDetails(dataContext, sp.AdmissionId, 0, false, DateTime.Now.Year, (byte)MonthNames.March).TotalDues);
                //list.ForEach(sp => list.Find(delegate (StudentPromotionModel s1) { return s1.AdmissionId == sp.AdmissionId; }).ArrearDues = 0);
                response.ArrearFeeHeadId = ArrearFeeHeadId;
                response.StudentPromotionList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("SaveClassPromotion")]
        public ExpandoObject SaveClassPromotion(SaveClassPromotion saveClassPromotion)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            bool IsCompleted = false;
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;
                saveClassPromotion.StudentPromotionList.ForEach(studentPromotionModel =>
                {
                    var oldAdmission = (from a1 in dataContext.Admissions
                                        where a1.AdmissionId == studentPromotionModel.AdmissionId
                                        select a1).First();
                    Admission newAdmission = new Admission
                    {
                        StudentId = oldAdmission.StudentId,
                        AcademicSessionId = saveClassPromotion.NewAcademicSessionId,
                        RollNo = studentPromotionModel.RollNo,
                        AdmissionDate = DateTime.Now,
                        LeaveDate = null,
                        AdmissionStatus = (byte)AdmissionStatus.Active
                    };
                    if (studentPromotionModel.PromotionStatus == PromotionStatus.Promoted)
                    {
                        newAdmission.AdmissionType = (byte)AdmissionType.Promoted;
                        newAdmission.SectionId = studentPromotionModel.NewSectionId;
                    }
                    else
                    {
                        newAdmission.AdmissionType = (byte)AdmissionType.ClassBack;
                        newAdmission.SectionId = saveClassPromotion.OldSectionId;
                    }
                    dataContext.Admissions.InsertOnSubmit(newAdmission);
                    dataContext.SubmitChanges();

                    oldAdmission.LeaveDate = newAdmission.AdmissionDate;
                    oldAdmission.AdmissionStatus = (byte)AdmissionStatus.Inactive;
                    dataContext.SubmitChanges();


                    //    var admissionFee = (from af in dataContext.AdmissionFees
                    //                        join afh in dataContext.AdmissionFeeHeads
                    //                        on af.AdmissionFeeHeadId equals afh.AdmissionFeeHeadId
                    //                        where afh.AdmissionType == newAdmission.AdmissionType
                    //                        && afh.AcademicSessionId == saveClassPromotion.NewAcademicSessionId
                    //                        && afh.ClassId == newAdmission.Section.ClassId
                    //                        && afh.StudentTypeId == oldAdmission.Student.StudentTypeId
                    //                        select new { afh.FeeHeadId, af.IsRefundable, af.Amount });
                    //    admissionFee.ToList().ForEach(aFee =>
                    //    {
                    //        StudentCharge studentCharge = new StudentCharge
                    //        {
                    //            AdmissionId = newAdmission.AdmissionId,
                    //            FeeHeadId = aFee.FeeHeadId,
                    //            IsRefundable = aFee.IsRefundable,
                    //            FineAmount = aFee.Amount,
                    //            FeeType = (byte)FeeType.ReAdmission,
                    //            Comments = "Re Admission",
                    //            StudentChargeStatus = (byte)FineStatus.Active,
                    //            FineDate = (from s1 in dataContext.AcademicSessions
                    //                        where s1.AcademicSessionId == saveClassPromotion.NewAcademicSessionId
                    //                        select s1.SessionStartDate).First()
                    //        };
                    //        dataContext.StudentCharges.InsertOnSubmit(studentCharge);
                    //        dataContext.SubmitChanges();
                    //    });


                    if (studentPromotionModel.ArrearDues > 0)
                    {
                        StudentCharge studentCharge = new StudentCharge
                        {
                            AdmissionId = newAdmission.AdmissionId,
                            FeeHeadId = saveClassPromotion.ArrearFeeHeadId,
                            IsRefundable = false,
                            FineAmount = studentPromotionModel.ArrearDues,
                            FineDate = LoadData.CheckDate("01/04/" + DateTime.Now.Year.ToString(), ""),
                            FeeType = (byte)FeeType.ArrearDue,
                            Comments = "Dues of Previous Session or Class.",
                            StudentChargeStatus = (byte)FineStatus.Active
                        };
                        dataContext.StudentCharges.InsertOnSubmit(studentCharge);
                        dataContext.SubmitChanges();
                    }
                });

                transaction.Commit();
                IsCompleted = true;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (!IsCompleted)
                    transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (null != dataContext.Connection)
                    dataContext.Connection.Close();
            }
            return response;
        }
    }
}
