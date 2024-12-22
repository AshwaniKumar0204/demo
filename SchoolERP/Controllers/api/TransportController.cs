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
    [RoutePrefix("api/transport")]
    public class TransportController : ApiController
    {

        //-------Transport Period Start-----------------
        [HttpGet]
        [Route("transportperiodList")]
        public ExpandoObject TransportFeePeriodList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                int schoolId = SessionUtilies.GetSchoolId(dataContext);

                var periods = (from d in dataContext.TransportFeePeriods
                               where d.SchoolId == schoolId
                               select new TransportFeePeriodModel
                               {
                                   TransportFeePeriodId = d.TransportFeePeriodId,
                                   ValidFrom = d.ValidFrom,
                                   ValidTo = d.ValidTo
                               }).ToList();

                response.TransportFeePeriods = periods;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveTransportFeePeriod")]
        public ExpandoObject SaveTransportFeePeriod(TransportFeePeriodModel model)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();

            try
            {
                if (model.TransportFeePeriodId > 0)
                {
                    var period = (from d in dataContext.TransportFeePeriods
                                  where d.TransportFeePeriodId == model.TransportFeePeriodId
                                  select d).First();

                    period.ValidFrom = model.ValidFrom;

                    if (!string.IsNullOrWhiteSpace(model.ValidTo.ToString()))
                        period.ValidTo = model.ValidTo.Value;

                    dataContext.SubmitChanges();

                    response.Message = ConstantData.SuccessMessage;
                }
                else
                {

                    TransportFeePeriod period = new TransportFeePeriod();

                    period.ValidFrom = model.ValidFrom.AddDays(1);
                    period.SchoolId = SessionUtilies.GetSchoolId(dataContext);


                    if (!string.IsNullOrWhiteSpace(model.ValidTo.ToString()))
                        period.ValidTo = model.ValidTo.Value.AddDays(1);

                    dataContext.TransportFeePeriods.InsertOnSubmit(period);
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
        [Route("deleteTransportPeriod")]
        public ExpandoObject DeleteTransportPeriod(int periodId)
        {

            SchoolDataContext dataContext = new SchoolDataContext();
            dynamic response = new ExpandoObject();
            try
            {
                dataContext.TransportFeePeriods.DeleteOnSubmit(dataContext.TransportFeePeriods.Where(x => x.TransportFeePeriodId == periodId).First());
                dataContext.SubmitChanges();

                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = "Can't be deleted!";
            }
            return response;
        }

        //--------Transport Period End----------------------------------






        //----------Transport Group Start--------------------------


        [HttpGet]
        [Route("transportGroupList")]
        public ExpandoObject TrasportGroupList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                int schoolID = SessionUtilies.GetSchoolId(dataContext);

                var groups = (from d in dataContext.TransportGroups
                              where d.SchoolId == schoolID
                              select new TransportGroupModel
                              {
                                  TransportGroupId = d.TransportGroupId,
                                  TransportGroupTitle = d.TransportGroupTitle,
                              }).ToList();

                response.TransportGroups = groups;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveTransportGroup")]
        public ExpandoObject SaveTransportGroup(TransportGroup model)
        {
            dynamic response = new ExpandoObject();
            SchoolDataContext dataContext = new SchoolDataContext();

            try
            {
                if (model.TransportGroupId > 0)
                {
                    var group = (from d in dataContext.TransportGroups
                                 where d.TransportGroupId == model.TransportGroupId
                                 select d).First();

                    group.TransportGroupTitle = model.TransportGroupTitle.Trim();

                    dataContext.SubmitChanges();

                    response.Message = ConstantData.SuccessMessage;
                }
                else
                {
                    TransportGroup group = new TransportGroup();

                    group.SchoolId = SessionUtilies.GetSchoolId(dataContext);
                    group.TransportGroupTitle = model.TransportGroupTitle.Trim();

                    dataContext.TransportGroups.InsertOnSubmit(group);
                    dataContext.SubmitChanges();

                    response.Message = ConstantData.SuccessMessage;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX_TransportGroup"))
                    response.Message = "This Group Name already exists.";
                else
                    response.Message = ex.Message;
            }
            return response;
        }

        [HttpDelete]
        [Route("deleteTransportGroup")]
        public ExpandoObject DeleteTransportGroup(int groupId)
        {

            SchoolDataContext dataContext = new SchoolDataContext();
            dynamic response = new ExpandoObject();
            try
            {
                dataContext.TransportGroups.DeleteOnSubmit(dataContext.TransportGroups.Where(x => x.TransportGroupId == groupId).First());
                dataContext.SubmitChanges();

                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = "Can't be deleted!";
            }
            return response;
        }

        //---------Transport Group End-----------------------------





        //-------------------Transport Fee Head-------------------------------------------------
        [HttpGet]
        [Route("trabsportFeeHeadList")]
        public ExpandoObject TrasportFeeHeadList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                List<TransportFeeHeadModel> cblFeeHead = new List<TransportFeeHeadModel>();
                SchoolDataContext dataContext = new SchoolDataContext();

                var feeHeadMasters = (from f1 in dataContext.FeeHeads
                                      select f1);

                var classFeeHeads = (from f1 in dataContext.TransportFeeHeads
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
        [Route("SavetrabsportFeeHead")]
        public ExpandoObject SaveTramsportFeeHead(List<TransportFeeHeadModel> newClassFineHead)
        {
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            dynamic response = new ExpandoObject();
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                var oldClassFineHeads = (from o1 in dataContext.TransportFeeHeads
                                         select o1);

                foreach (var i in oldClassFineHeads)
                {
                    var fineH = (from f1 in dataContext.TransportFeeHeads
                                 where
                                 f1.FeeHeadId == i.FeeHeadId
                                 select f1
                                    ).First();

                    dataContext.TransportFeeHeads.DeleteOnSubmit(fineH);
                    dataContext.SubmitChanges();
                }
                // foreach
                foreach (var i in newClassFineHead)
                {
                    TransportFeeHead classFineHead = new TransportFeeHead();

                    classFineHead.FeeHeadId = i.FeeHeadId;
                    dataContext.TransportFeeHeads.InsertOnSubmit(classFineHead);
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

        //-------------------Transport Fee Head End----------------------------------------------




        //-------------------Transport Fee-------------------------------------------------------

        [HttpGet]
        [Route("transportFeeList")]
        public ExpandoObject TrasportFeeList(int periodId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                if (periodId > 0)
                {
                    var list = (from g in dataContext.TransportGroups
                                select new TransportGroupModel
                                {
                                    FeeList = (from tfh1 in dataContext.TransportFeeHeads
                                               join fh1 in dataContext.FeeHeads
                                               on tfh1.FeeHeadId equals fh1.FeeHeadId
                                               join tf in
                                                   (from tfee in dataContext.TransportFees where tfee.TransportGroupId == g.TransportGroupId && tfee.TransportFeePeriodId == periodId select tfee)
                                               on tfh1.TransportFeeHeadId equals tf.TransportFeeHeadId into semFee
                                               from subFee in semFee.DefaultIfEmpty()
                                               select
                                               new TranportFeeModel
                                               {
                                                   Amount = (subFee == null ? 0 : subFee.Amount.Value),
                                                   FeeHeadTitle = fh1.FeeHeadTitle,
                                                   Remarks = (subFee == null ? "" : subFee.Remarks),
                                                   TransportFeeHeadId = tfh1.TransportFeeHeadId,
                                                   TransportFeeId = (subFee == null ? 0 : subFee.TransportFeeId),
                                                   TransportFeeStatus = (subFee == null ? true : subFee.TransportFeeStatus.Value)
                                               }).ToList(),

                                    TransportGroupId = g.TransportGroupId,
                                    TransportGroupTitle = g.TransportGroupTitle
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
        [Route("SavetrabsportFee")]
        public ExpandoObject SaveTramsportFee(TransportFeePeriodModel models)
        {
            SchoolDataContext dataContext = new SchoolDataContext();
            DbTransaction transaction = null;
            dynamic response = new ExpandoObject();
            try
            {
                dataContext.Connection.Open();
                transaction = dataContext.Connection.BeginTransaction();
                dataContext.Transaction = transaction;

                foreach (var gr in models.GroupList)
                {
                    foreach (var fee in gr.FeeList)
                    {
                        if (fee.TransportFeeId == 0)
                        {
                            TransportFee transportfee = new TransportFee();

                            transportfee.Amount = fee.Amount;
                            transportfee.TransportFeePeriodId = models.TransportFeePeriodId;
                            transportfee.TransportFeeHeadId = fee.TransportFeeHeadId;
                            transportfee.TransportGroupId = gr.TransportGroupId;
                            transportfee.Remarks = fee.Remarks;
                            transportfee.TransportFeeStatus = fee.TransportFeeStatus;

                            dataContext.TransportFees.InsertOnSubmit(transportfee);
                            dataContext.SubmitChanges();
                        }
                        else
                        {
                            var transportfee = (from sf in dataContext.TransportFees
                                                where sf.TransportFeeId == fee.TransportFeeId
                                                select sf).First();
                            transportfee.TransportFeePeriodId = models.TransportFeePeriodId;

                            transportfee.Amount = fee.Amount;
                            transportfee.TransportFeeHeadId = fee.TransportFeeHeadId;
                            transportfee.TransportGroupId = gr.TransportGroupId;
                            transportfee.Remarks = fee.Remarks;
                            transportfee.TransportFeeStatus = fee.TransportFeeStatus;
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


        //----------------------------Transport Fee End------------------------------------------











        //-------------------------- Transport Bus/Auto---------------------------------------------

        [HttpGet]
        [Route("transportBusList")]
        public ExpandoObject BusList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                int schoolId = SessionUtilies.GetSchoolId(dataContext);

                var buses = (from d in dataContext.Bus
                             where d.SchoolId == schoolId
                             select new
                             {
                                 d.BusId,
                                 d.BusTitle,
                                 d.Description,
                                 d.Status,
                                 d.VehicleType.VehicleTypeName,
                                 d.VehicleTypeId
                             }).ToList();

                response.Message = ConstantData.SuccessMessage;
                response.Buses = buses;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveBusDetails")]
        public ExpandoObject SaveBus(Bus model)
        {
            SchoolDataContext dataContext = new SchoolDataContext();
            dynamic response = new ExpandoObject();
            try
            {
                if (string.IsNullOrWhiteSpace(model.BusTitle))
                    throw new Exception("Vehicle Title is required.");

                if (model.Status <= 0)
                    throw new Exception("Status is required.");

                if (model.BusId > 0)
                {
                    var bus = (from d in dataContext.Bus
                               where d.BusId == model.BusId
                               select d).First();

                    bus.BusTitle = model.BusTitle;
                    bus.Description = model.Description;
                    bus.Status = (byte)model.Status;
                    bus.VehicleTypeId = model.VehicleTypeId;
                    dataContext.SubmitChanges();
                }
                else
                {
                    Bus bus = new Bus();
                    bus.SchoolId = SessionUtilies.GetSchoolId(dataContext);
                    bus.BusTitle = model.BusTitle;
                    bus.Description = model.Description;
                    bus.Status = (byte)model.Status;
                    bus.VehicleTypeId = model.VehicleTypeId;
                    dataContext.Bus.InsertOnSubmit(bus);
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

        //--------------------------- Transport Bus/Auto End----------------------------------------











        //------------------------ Transport Student Add ------------------------------------------


        [HttpGet]
        [Route("studentTransportDetails")]
        public ExpandoObject TransportDetails(int admissionId)
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

                var transport = (from t in dataContext.StudentTransports.Where(x => x.StudentId == studentId)
                                 join g in dataContext.TransportGroups
                                 on t.TransportGroupId equals g.TransportGroupId
                                 join b in dataContext.Bus
                                 on t.BusId equals b.BusId
                                 select new StudentTransportModel
                                 {
                                     TransportGroupTitle = g.TransportGroupTitle,
                                     TransportGroupId = g.TransportGroupId,
                                     BusId = b.BusId,
                                     BusTitle = b.BusTitle,
                                     PickupPoint = t.PickupPoint,
                                     PickupTime = t.PickupTime,
                                     StartDate = t.StartDate,
                                     EndDate = t.EndDate,
                                     Status = t.Status,
                                     StudentId = t.StudentId,
                                     StudentTransportId = t.StudentTransportId
                                 }).ToList();

                response.Message = ConstantData.SuccessMessage;
                response.StudentDetails = student;
                response.TransportList = transport;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("saveStudentTransport")]
        public ExpandoObject SaveStudentTransport(StudentTransportModel model)
        {
            SchoolDataContext dataContext = new SchoolDataContext();
            dynamic response = new ExpandoObject();
            try
            {

                if (model.StudentId <= 0)
                    throw new Exception("Student ID is required.");

                if (model.TransportGroupId <= 0)
                    throw new Exception("Group ID is required.");

                if (model.BusId <= 0)
                    throw new Exception("BUS ID is required.");

                if (model.Status <= 0)
                    throw new Exception("Status is required.");

                if (model.StudentTransportId == 0)
                {
                    //New Insert
                    StudentTransport transport = new StudentTransport();

                    transport.StartDate = model.StartDate;
                    transport.BusId = model.BusId;
                    transport.PickupPoint = model.PickupPoint;
                    transport.PickupTime = model.PickupTime;
                    transport.StartDate = model.StartDate;
                    transport.EndDate = model.EndDate;
                    transport.Status = (byte)model.Status;
                    transport.StudentId = model.StudentId;
                    transport.TransportGroupId = model.TransportGroupId;

                    dataContext.StudentTransports.InsertOnSubmit(transport);
                    dataContext.SubmitChanges();
                }
                else
                {
                    //Update

                    var transport = (from d in dataContext.StudentTransports
                                     where d.StudentTransportId == model.StudentTransportId
                                     select d).First();

                    transport.StartDate = model.StartDate;
                    transport.BusId = model.BusId;
                    transport.PickupPoint = model.PickupPoint;
                    transport.PickupTime = model.PickupTime;
                    transport.StartDate = model.StartDate;
                    transport.EndDate = model.EndDate;
                    transport.Status = (byte)model.Status;
                    transport.StudentId = model.StudentId;
                    transport.TransportGroupId = model.TransportGroupId;

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




        //------------------------- Transport Student ENd------------------------------------------
    }
}
