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
    [RoutePrefix("api/Hostel")]
    public class HostelController : ApiController
    {

        //-------Hostel Period Start-----------------
        [HttpGet]
        [Route("HostelperiodList")]
        public ExpandoObject HostelFeePeriodList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                //int schoolId = SessionUtilies.GetSchoolId(dataContext);

                var periods = (from d in dataContext.HostelFeePeriods
                                   //where d.SchoolId == schoolId
                               select new
                               {
                                   HostelFeePeriodId = d.HostelFeePeriodId,
                                   ValidFrom = d.ValidFrom,
                                   ValidTo = d.ValidTo,
                               }).ToList();

                response.HostelFeePeriods = periods;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveHostelFeePeriod")]
        public ExpandoObject SaveHostelFeePeriod(HostelFeePeriod model)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();

            try
            {
                if (model.HostelFeePeriodId > 0)
                {
                    var period = (from d in dataContext.HostelFeePeriods
                                  where d.HostelFeePeriodId == model.HostelFeePeriodId
                                  select d).First();

                    period.ValidFrom = model.ValidFrom;

                    if (!string.IsNullOrWhiteSpace(model.ValidTo.ToString()))
                        period.ValidTo = model.ValidTo.Value;

                    dataContext.SubmitChanges();

                    response.Message = ConstantData.SuccessMessage;
                }
                else
                {

                    HostelFeePeriod period = new HostelFeePeriod();

                    period.ValidFrom = model.ValidFrom.AddDays(1);
                    period.SchoolId = SessionUtilies.GetSchoolId(dataContext);


                    if (!string.IsNullOrWhiteSpace(model.ValidTo.ToString()))
                        period.ValidTo = model.ValidTo.Value.AddDays(1);

                    dataContext.HostelFeePeriods.InsertOnSubmit(period);
                    dataContext.SubmitChanges();

                    response.Message = ConstantData.SuccessMessage;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpDelete]
        [Route("deleteHostelPeriod")]
        public ExpandoObject DeleteHostelPeriod(int periodId)
        {

            SchoolDataContext dataContext = new SchoolDataContext();
            dynamic response = new ExpandoObject();
            try
            {
                dataContext.HostelFeePeriods.DeleteOnSubmit(dataContext.HostelFeePeriods.Where(x => x.HostelFeePeriodId == periodId).First());
                dataContext.SubmitChanges();

                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = "Can't be deleted!";
            }
            return response;
        }

        //--------Hostel Period End----------------------------------






        //----------Hostel Hostel Start--------------------------


        [HttpGet]
        [Route("HostelList")]
        public ExpandoObject HostelList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                int schoolID = SessionUtilies.GetSchoolId(dataContext);

                var Hostels = (from d in dataContext.Hostels
                                   //where d.SchoolId == schoolID
                               select new
                               {
                                   HostelId = d.HostelId,
                                   HostelTitle = d.HostelTitle,
                               }).ToList();

                response.Hostels = Hostels;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveHostel")]
        public ExpandoObject SaveHostel(Hostel model)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();

            try
            {
                if (model.HostelId > 0)
                {
                    var Hostel = (from d in dataContext.Hostels
                                  where d.HostelId == model.HostelId
                                  select d).First();

                    Hostel.HostelTitle = model.HostelTitle.Trim();

                    dataContext.SubmitChanges();

                    response.Message = ConstantData.SuccessMessage;
                }
                else
                {
                    Hostel Hostel = new Hostel();

                    Hostel.SchoolId = SessionUtilies.GetSchoolId(dataContext);
                    Hostel.HostelTitle = model.HostelTitle.Trim();

                    dataContext.Hostels.InsertOnSubmit(Hostel);
                    dataContext.SubmitChanges();

                    response.Message = ConstantData.SuccessMessage;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX_Hostel"))
                    response.Message = "This Hostel Name already exists.";
                else
                    response.Message = ex.Message;
            }
            return response;
        }

        [HttpDelete]
        [Route("deleteHostel")]
        public ExpandoObject DeleteHostel(int HostelId)
        {

            SchoolDataContext dataContext = new SchoolDataContext();
            dynamic response = new ExpandoObject();
            try
            {
                dataContext.Hostels.DeleteOnSubmit(dataContext.Hostels.Where(x => x.HostelId == HostelId).First());
                dataContext.SubmitChanges();

                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = "Can't be deleted!";
            }
            return response;
        }

        //---------Hostel Hostel End-----------------------------





        //-------------------Hostel Fee Head-------------------------------------------------
        [HttpGet]
        [Route("HostelFeeHeadList")]
        public ExpandoObject TrasportFeeHeadList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                List<TransportFeeHeadModel> cblFeeHead = new List<TransportFeeHeadModel>();
                SchoolDataContext dataContext = new SchoolDataContext();

                var feeHeadMasters = (from f1 in dataContext.FeeHeads
                                      select f1);

                var classFeeHeads = (from f1 in dataContext.HostelFeeHeads
                                     select f1.FeeHeadId
                                        );
                foreach (var ts in feeHeadMasters)
                {
                    TransportFeeHeadModel listItem = new TransportFeeHeadModel();
                    listItem.FeeHeadTitle = ts.FeeHeadTitle;
                    listItem.FeeHeadId = ts.FeeHeadId;

                    if (classFeeHeads.Contains(ts.FeeHeadId))
                        listItem.isSelected = true;
                    cblFeeHead.Add(listItem);
                }
                response.Message = ConstantData.SuccessMessage;
                response.FeeHeadLists = cblFeeHead;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }


        [HttpPost]
        [Route("SaveHostelFeeHead")]
        public ExpandoObject SaveHostelFeeHead(List<TransportFeeHeadModel> newClassFineHead)
        {
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            dynamic response = new ExpandoObject();
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                var oldClassFineHeads = (from o1 in dataContext.HostelFeeHeads
                                         select o1);

                foreach (var i in oldClassFineHeads)
                {
                    var fineH = (from f1 in dataContext.HostelFeeHeads
                                 where
                                 f1.FeeHeadId == i.FeeHeadId
                                 select f1
                                    ).First();

                    dataContext.HostelFeeHeads.DeleteOnSubmit(fineH);
                    dataContext.SubmitChanges();
                }
                // foreach
                foreach (var i in newClassFineHead)
                {
                    HostelFeeHead classFineHead = new HostelFeeHead();

                    classFineHead.FeeHeadId = i.FeeHeadId;
                    dataContext.HostelFeeHeads.InsertOnSubmit(classFineHead);
                    dataContext.SubmitChanges();
                }

                transaction.Commit();

                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (dataContext.Connection != null)
                {
                    dataContext.Connection.Close();
                }
            }
            return response;
        }

        //-------------------Hostel Fee Head End----------------------------------------------




        //-------------------Hostel Fee-------------------------------------------------------

        [HttpGet]
        [Route("HostelFeeList")]
        public ExpandoObject TrasportFeeList(int periodId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                if (periodId > 0)
                {
                    var list = (from g in dataContext.Hostels
                                select new
                                {
                                    FeeList = (from tfh1 in dataContext.HostelFeeHeads
                                               join fh1 in dataContext.FeeHeads
                                               on tfh1.FeeHeadId equals fh1.FeeHeadId
                                               join tf in
                                                   (from tfee in dataContext.HostelFees where tfee.HostelId == g.HostelId && tfee.HostelFeePeriodId == periodId select tfee)
                                               on tfh1.HostelFeeHeadId equals tf.HostelFeeHeadId into semFee
                                               from subFee in semFee.DefaultIfEmpty()
                                               select
                                               new
                                               {
                                                   Amount = (subFee == null ? 0 : subFee.Amount.Value),
                                                   FeeHeadTitle = fh1.FeeHeadTitle,
                                                   Remarks = (subFee == null ? "" : subFee.Remarks),
                                                   HostelFeeHeadId = tfh1.HostelFeeHeadId,
                                                   HostelFeeId = (subFee == null ? 0 : subFee.HostelFeeId),
                                                   HostelFeeStatus = (subFee == null ? true : subFee.HostelFeeStatus.Value)
                                               }).ToList(),

                                    HostelId = g.HostelId,
                                    HostelTitle = g.HostelTitle
                                }).ToList();
                    response.List = list;
                    response.Message = ConstantData.SuccessMessage;
                }
                else
                    response.Message = "Select Fee Period!";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveHostelFee")]
        public ExpandoObject SaveHostelFee(HostelFeePeriodModel models)
        {
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            dynamic response = new ExpandoObject();
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                foreach (var gr in models.HostelList)
                {
                    foreach (var fee in gr.FeeList)
                    {
                        if (fee.HostelFeeId == 0)
                        {
                            HostelFee Hostelfee = new HostelFee();

                            Hostelfee.Amount = fee.Amount;
                            Hostelfee.HostelFeePeriodId = models.HostelFeePeriodId;
                            Hostelfee.HostelFeeHeadId = fee.HostelFeeHeadId;
                            Hostelfee.HostelId = gr.HostelId;
                            Hostelfee.Remarks = fee.Remarks;
                            Hostelfee.HostelFeeStatus = fee.HostelFeeStatus;

                            dataContext.HostelFees.InsertOnSubmit(Hostelfee);
                            dataContext.SubmitChanges();
                        }
                        else
                        {
                            var Hostelfee = (from sf in dataContext.HostelFees
                                             where sf.HostelFeeId == fee.HostelFeeId
                                             select sf).First();
                            Hostelfee.HostelFeePeriodId = models.HostelFeePeriodId;

                            Hostelfee.Amount = fee.Amount;
                            Hostelfee.HostelFeeHeadId = fee.HostelFeeHeadId;
                            Hostelfee.HostelId = gr.HostelId;
                            Hostelfee.Remarks = fee.Remarks;
                            Hostelfee.HostelFeeStatus = fee.HostelFeeStatus;
                            dataContext.SubmitChanges();
                        }
                    }
                }

                transaction.Commit();

                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.Message = ex.Message;
            }
            finally
            {
                if (dataContext.Connection != null)
                {
                    dataContext.Connection.Close();
                }
            }
            return response;
        }


        //----------------------------Hostel Fee End------------------------------------------


        //------------------------ Hostel Student Add ------------------------------------------


        [HttpGet]
        [Route("studentHostelDetails")]
        public ExpandoObject HostelDetails(int admissionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                var studentId = (from d in dataContext.Admissions
                                 where d.AdmissionId == admissionId
                                 select d.StudentId).First();

                var student = (from d in dataContext.Students
                               join a in dataContext.Admissions
                                 on d.StudentId equals a.StudentId
                               join s in dataContext.Sections
                               on a.SectionId equals s.SectionId
                               join c in dataContext.Classes
                               on s.ClassId equals c.ClassId
                               where d.StudentId == studentId
                               && a.AdmissionStatus == (byte)AdmissionStatus.Active
                               && d.StudentStatus == (byte)StudentStatus.Active
                               select new
                               {
                                   d.StudentId,
                                   a.AdmissionId,
                                   d.AdmissionNo,
                                   d.FatherName,
                                   d.FullName,
                                   d.MobileNo,
                                   c.ClassName,
                                   s.SectionName
                               }).ToList();

                var Hostel = (from t in dataContext.StudentHostels.Where(x => x.StudentId == studentId)
                              join g in dataContext.Hostels
                              on t.HostelId equals g.HostelId
                              select new
                              {
                                  HostelTitle = g.HostelTitle,
                                  HostelId = g.HostelId,
                                  StartDate = t.StartDate,
                                  EndDate = t.EndDate,
                                  Status = t.Status,
                                  StudentId = t.StudentId,
                                  StudentHostelId = t.StudentHostelId
                              }).ToList();

                response.Message = ConstantData.SuccessMessage;
                response.StudentDetails = student;
                response.HostelList = Hostel;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveStudentHostel")]
        public ExpandoObject SaveStudentHostel(StudentHostel model)
        {
            SchoolDataContext dataContext = new SchoolDataContext();
            dynamic response = new ExpandoObject();
            try
            {

                if (model.StudentId <= 0)
                    throw new Exception("Student ID is required.");

                if (model.HostelId <= 0)
                    throw new Exception("Hostel ID is required.");

                if (model.Status <= 0)
                    throw new Exception("Status is required.");

                if (model.StudentHostelId == 0)
                {
                    //New Insert
                    StudentHostel Hostel = new StudentHostel();

                    Hostel.StartDate = model.StartDate;
                    Hostel.EndDate = model.EndDate;
                    Hostel.Status = (byte)model.Status;
                    Hostel.StudentId = model.StudentId;
                    Hostel.HostelId = model.HostelId;

                    dataContext.StudentHostels.InsertOnSubmit(Hostel);
                    dataContext.SubmitChanges();
                }
                else
                {
                    //Update

                    var Hostel = (from d in dataContext.StudentHostels
                                  where d.StudentHostelId == model.StudentHostelId
                                  select d).First();

                    Hostel.StartDate = model.StartDate;
                    Hostel.EndDate = model.EndDate;
                    Hostel.Status = (byte)model.Status;
                    Hostel.StudentId = model.StudentId;
                    Hostel.HostelId = model.HostelId;

                    dataContext.SubmitChanges();
                }

                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }




        //------------------------- Hostel Student ENd------------------------------------------
    }
}
