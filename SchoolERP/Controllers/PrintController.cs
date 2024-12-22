using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using School;
using School.DataSets;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolERP.Controllers
{
    [CheckSessionTimeOutAttribute]
    public class PrintController : Controller
    {
        public ActionResult RegistrationOnlineCount(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/attendence_report_staffwise.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                //var subjectOptions = from r1 in dataContext.RegistrationOnlines
                //                     join s2 in dataContext.SubjectOptions on r1.SubjectOptionOne equals

                //                     join s1 in dataContext.ClassStreamTypes on r1.ClassStreamTypeId equals s1.ClassStreamTypeId

                //DataS ds = new AttendanceDataSet();

                //DataTable attendenceDataTable = ds.AttendenceDataTable;
                //var list = AttendanceData.AttendanceList(dataContext, model);
                //list.ForEach(att =>
                //{
                //    DataRow aRow = attendenceDataTable.NewRow();
                //    aRow["StaffId"] = att.StaffId;
                //    aRow["GroupName"] = att.GroupName;
                //    aRow["AttendenceCode"] = att.AttendenceCode;
                //    aRow["FullName"] = att.FullName;
                //    aRow["MobileNo"] = att.MobileNo;
                //    aRow["DepartmentName"] = att.DepartmentName;
                //    aRow["DesignationName"] = att.DesignationName;
                //    aRow["EntryDateTime"] = att.EntryDateTime.ToString("dd-MM-yyyy");
                //    aRow["InTime"] = att.InTime;
                //    aRow["InGapeTime"] = att.InGapeTime;
                //    aRow["OverInTime"] = att.OverInTime;
                //    aRow["OutTime"] = att.OutTime;
                //    aRow["OutGapeTime"] = att.OutGapeTime;
                //    aRow["OverOutTime"] = att.OverOutTime;
                //    aRow["OverOutTime"] = att.OverOutTime;
                //    if (att.Shift != null)
                //    {
                //        aRow["ShiftName"] = att.Shift.ShiftName;
                //        aRow["ShiftInTime"] = att.Shift.InTime;
                //        aRow["ShiftOutTime"] = att.Shift.OutTime;
                //        aRow["ShiftGapeTime"] = att.Shift.GapeTime;
                //        aRow["ShiftDurationString"] = att.Shift.ShiftDurationString;
                //        aRow["IsDeay"] = att.OverInTime > att.Shift.GapeTime;
                //        aRow["IsBefore"] = att.OverOutTime > att.Shift.GapeTime;
                //    }
                //    attendenceDataTable.Rows.Add(aRow);
                //});

                //SqlCommand cmd = new SqlCommand("select * from School", connection);
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                //dataAdapter.Fill(ds.School);
                //dataAdapter.Dispose();
                //cmd.Dispose();

                //DataRow dataRow = ds.HeaderDataTable.NewRow();
                //dataRow["FromDate"] = model.FromDate.HasValue ? model.FromDate.Value.ToString("dd-MM-yyyy") : "All Date";
                //dataRow["ToDate"] = model.ToDate.HasValue ? model.ToDate.Value.ToString("dd-MM-yyyy") : "All Date";
                //ds.HeaderDataTable.Rows.Add(dataRow);

                //rp.SetDataSource(ds);
                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(LoadData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult AttendanceReportShiftwise(string fromDate, string toData, int shiftId, int groupId, string code, int no)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                if (no == 1)
                    rp.Load(Server.MapPath("~/reports/attendence_report_shiftwise.rpt"));
                else if (no == 2)
                    rp.Load(Server.MapPath("~/reports/attendence_report_groupwise.rpt"));
                else if (no == 3)
                    rp.Load(Server.MapPath("~/reports/attendence_report_staffwise.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                AttendanceDataSet ds = new AttendanceDataSet();

                DateFilterModel model = new DateFilterModel();
                if (!string.IsNullOrEmpty(fromDate))
                    model.FromDate = LoadData.CheckDate(fromDate, "Invalid From Date");
                if (!string.IsNullOrEmpty(toData))
                    model.ToDate = LoadData.CheckDate(toData, "Invalid To Date");
                model.ShiftId = shiftId;
                model.Id = code;
                model.StaffGroupId = groupId;

                DataTable attendenceDataTable = ds.AttendenceDataTable;
                var list = AttendanceData.AttendanceList(dataContext, model);
                list.ForEach(att =>
                {
                    DataRow aRow = attendenceDataTable.NewRow();
                    aRow["StaffId"] = att.StaffId;
                    aRow["GroupName"] = att.GroupName;
                    aRow["AttendenceCode"] = att.AttendenceCode;
                    aRow["FullName"] = att.FullName;
                    aRow["MobileNo"] = att.MobileNo;
                    aRow["DepartmentName"] = att.DepartmentName;
                    aRow["DesignationName"] = att.DesignationName;
                    aRow["EntryDateTime"] = att.EntryDateTime.ToString("dd-MM-yyyy");
                    aRow["InTime"] = att.InTime;
                    aRow["InGapeTime"] = att.InGapeTime;
                    aRow["OverInTime"] = att.OverInTime;
                    aRow["OutTime"] = att.OutTime;
                    aRow["OutGapeTime"] = att.OutGapeTime;
                    aRow["OverOutTime"] = att.OverOutTime;
                    aRow["OverOutTime"] = att.OverOutTime;
                    if (att.Shift != null)
                    {
                        aRow["ShiftName"] = att.Shift.ShiftName;
                        aRow["ShiftInTime"] = att.Shift.InTime;
                        aRow["ShiftOutTime"] = att.Shift.OutTime;
                        aRow["ShiftGapeTime"] = att.Shift.GapeTime;
                        aRow["ShiftDurationString"] = att.Shift.ShiftDurationString;
                        aRow["IsDeay"] = att.OverInTime > att.Shift.GapeTime;
                        aRow["IsBefore"] = att.OverOutTime > att.Shift.GapeTime;
                    }
                    attendenceDataTable.Rows.Add(aRow);
                });

                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds.School);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataRow dataRow = ds.HeaderDataTable.NewRow();
                dataRow["FromDate"] = model.FromDate.HasValue ? model.FromDate.Value.ToString("dd-MM-yyyy") : "All Date";
                dataRow["ToDate"] = model.ToDate.HasValue ? model.ToDate.Value.ToString("dd-MM-yyyy") : "All Date";
                ds.HeaderDataTable.Rows.Add(dataRow);

                //cmd = new SqlCommand("select FromDate=" + model.FromDate + " , ToDate=" + model.ToDate, connection);
                //dataAdapter = new SqlDataAdapter(cmd);
                //dataAdapter.Fill(ds.HeaderDataTable);
                //dataAdapter.Dispose();
                //cmd.Dispose();

                rp.SetDataSource(ds);
                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(LoadData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult SalarySlip(string ids, int no)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                if (no == 1)
                    rp.Load(Server.MapPath("~/reports/salary_slip.rpt"));
                else if (no == 2)
                    rp.Load(Server.MapPath("~/reports/bank_statement.rpt"));
                else if (no == 3)
                    rp.Load(Server.MapPath("~/reports/leave_report.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DataSet ds = new DataSet();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable StaffPayment = new DataTable("StaffPayment");
                cmd = new SqlCommand("select * from StaffPayment where StaffPaymentId in (" + ids + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(StaffPayment);
                ds.Tables.Add(StaffPayment);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable Staff = new DataTable("Staff");
                cmd = new SqlCommand("select * from Staff", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(Staff);
                ds.Tables.Add(Staff);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable Designation = new DataTable("Designation");
                cmd = new SqlCommand("select * from Designation", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(Designation);
                ds.Tables.Add(Designation);
                dataAdapter.Dispose();
                cmd.Dispose();

                rp.SetDataSource(ds);
                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(LoadData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintAdmintCardForRO(int sessionId, int classNo, int id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/admit_card_registration.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);

                DataSet ds = new DataSet();
                int? examCenterId = 0;
                if (id > 0)
                {
                    var registration = (from m1 in dataContext.RegistrationOnlines
                                        where m1.RegistrationOnlineId == Convert.ToInt32(id)
                                        select new
                                        {
                                            m1.RegistrationOnlineId,
                                            m1.AcademicSessionId,
                                            m1.ClassNo,
                                            m1.StudentPhoto,
                                            m1.ExamCenterId
                                        }).FirstOrDefault();
                    classNo = registration.ClassNo;
                    sessionId = registration.AcademicSessionId;
                    examCenterId = registration.ExamCenterId;
                }

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable ExamCenterOnlineDataTable = new DataTable("ExamCenter");
                cmd = new SqlCommand("select * from ExamCenter where ExamCenterId=" + examCenterId + " or 0=" + examCenterId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ExamCenterOnlineDataTable);
                ds.Tables.Add(ExamCenterOnlineDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable RegistrationOnlineDataTable = new DataTable("RegistrationOnline");
                cmd = new SqlCommand("select * from RegistrationOnline where RegistrationOnlineId=" + id + " or 0=" + id, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(RegistrationOnlineDataTable);
                ds.Tables.Add(RegistrationOnlineDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable ClassStreamTypeTable = new DataTable("ClassStreamType");
                cmd = new SqlCommand("select * from ClassStreamType", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ClassStreamTypeTable);
                ds.Tables.Add(ClassStreamTypeTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable SubjectOptionTable = new DataTable("SubjectOption");
                cmd = new SqlCommand("select * from SubjectOption", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(SubjectOptionTable);
                ds.Tables.Add(SubjectOptionTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable academicSessionDataTable = new DataTable("AcademicSession");
                cmd = new SqlCommand("select * from AcademicSession where AcademicSessionId=" + sessionId + " or 0=" + sessionId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(academicSessionDataTable);
                ds.Tables.Add(academicSessionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable classDataTable = new DataTable("Class");
                cmd = new SqlCommand("select distinct ClassNo,ClassName2 from Class where ClassNo=" + classNo + "or 0=" + classNo, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(classDataTable);
                ds.Tables.Add(classDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable qCode = new DataTable("Command");
                qCode.Columns.Add(new DataColumn("SPhoto", typeof(byte[])));
                qCode.Columns.Add(new DataColumn("RegistrationOnlineId", typeof(int)));

                var registrations = (from r1 in dataContext.RegistrationOnlines
                                     where (sessionId == 0 || r1.AcademicSessionId == sessionId)
                                     && (classNo == 0 || r1.ClassNo == classNo)
                                     && (id == 0 || r1.RegistrationOnlineId == id)
                                     && r1.ExamCenterId.HasValue
                                     && r1.RollNo != null
                                     && r1.StudentPhoto != null
                                     select new { r1.RegistrationOnlineId, r1.StudentPhoto }).ToList();
                registrations.ForEach(registration =>
                {
                    DataRow qRow = qCode.NewRow();
                    qRow["SPhoto"] = LoadData.GetImageFromUrl(registration.StudentPhoto);
                    qRow["RegistrationOnlineId"] = registration.RegistrationOnlineId;
                    qCode.Rows.Add(qRow);
                });
                ds.Tables.Add(qCode);

                rp.SetDataSource(ds);
                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(LoadData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintPurchaseInvoiceStore(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/purchase_invoice_store_item.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DataSet ds = new DataSet();
                //int purchaseId = Convert.ToInt32(QueoryCryptoEngine.Decrypt(id));
                int purchaseId = Convert.ToInt32(id);

                var purchase = dataContext.Purchases.Where(x => x.PurchaseId == purchaseId).First();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable PurchaseDataTable = new DataTable("Purchase");
                cmd = new SqlCommand("select * from Purchase where PurchaseId = " + purchase.PurchaseId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(PurchaseDataTable);
                ds.Tables.Add(PurchaseDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable PurchaseStoreItemDataTable = new DataTable("PurchaseStoreItem");
                cmd = new SqlCommand("select * from PurchaseStoreItem where PurchaseId = " + purchase.PurchaseId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(PurchaseStoreItemDataTable);
                ds.Tables.Add(PurchaseStoreItemDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable supplierDataTable = new DataTable("Supplier");
                cmd = new SqlCommand("select * from Supplier where SupplierId = " + purchase.SupplierId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(supplierDataTable);
                ds.Tables.Add(supplierDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable unitDataTable = new DataTable("Unit");
                cmd = new SqlCommand("select * from Unit", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(unitDataTable);
                ds.Tables.Add(unitDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable GSTDataTable = new DataTable("GST");
                cmd = new SqlCommand("select * from GST", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(GSTDataTable);
                ds.Tables.Add(GSTDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable ItemTypeDataTable = new DataTable("ItemType");
                cmd = new SqlCommand("select * from ItemType", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ItemTypeDataTable);
                ds.Tables.Add(ItemTypeDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable StoreItemDataTable = new DataTable("StoreItem");
                cmd = new SqlCommand("select * from StoreItem", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(StoreItemDataTable);
                ds.Tables.Add(StoreItemDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                rp.SetDataSource(ds);
                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(LoadData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintVehicleServiceReport(int BusId, int SupplierId, DateTime? FromDate, DateTime? ToDate, int VehicleServiceId)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                if (VehicleServiceId != 0)
                    rp.Load(Server.MapPath("~/reports/vehicle_service_report_invoice.rpt"));
                else
                    rp.Load(Server.MapPath("~/reports/vehicle_service_report.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DataSet ds = new DataSet();

                if (VehicleServiceId > 0)
                {
                    var vehicleService = dataContext.VehicleServices.Where(x => x.VehicleServiceId == VehicleServiceId).First();
                    SupplierId = vehicleService.SupplierId;
                    BusId = vehicleService.BusId;
                }

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable SupplierDataTable = new DataTable("Supplier");
                cmd = new SqlCommand("select * from Supplier" + (SupplierId == 0 ? "" : " where SupplierId=" + SupplierId), connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(SupplierDataTable);
                ds.Tables.Add(SupplierDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable VehicleServiceDataTable = new DataTable("VehicleService");
                cmd = new SqlCommand("select * from VehicleService where 0=0"
                    + (SupplierId == 0 ? "" : " and SupplierId=" + SupplierId)
                    + (VehicleServiceId == 0 ? "" : " and VehicleServiceId=" + VehicleServiceId)
                    + (BusId == 0 ? "" : " and BusId=" + BusId)
                    + (FromDate == null ? "" : " and cast(BillDate as Date)>='" + FromDate.Value.ToString("yyyy-MM-dd") + "'")
                    + (ToDate == null ? "" : " and cast(BillDate as Date)<='" + ToDate.Value.ToString("yyyy-MM-dd") + "'"), connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(VehicleServiceDataTable);
                ds.Tables.Add(VehicleServiceDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable VehicleServiceDetail = new DataTable("VehicleServiceDetail");
                cmd = new SqlCommand("select * from VehicleServiceDetail where 0=0"
                    + (VehicleServiceId == 0 ? "" : " and VehicleServiceId=" + VehicleServiceId)
                    , connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(VehicleServiceDetail);
                ds.Tables.Add(VehicleServiceDetail);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable Command = new DataTable("Command");
                cmd = new SqlCommand("SELECT FROMDATE='" + (FromDate == null ? "" : FromDate.Value.ToString("dd-MMM-yyyy")) + "', TODATE='"
                    + (ToDate == null ? "" : ToDate.Value.ToString("dd-MMM-yyyy")) + "',SupplierTitle=null"
                    , connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(Command);
                ds.Tables.Add(Command);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable BusDataTable = new DataTable("Bus");
                cmd = new SqlCommand("select * from Bus" + (BusId == 0 ? "" : " where BusId=" + BusId), connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(BusDataTable);
                ds.Tables.Add(BusDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                rp.SetDataSource(ds);
                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(LoadData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintAdmitCard(int ac, int cl, int se, int em)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/admit_card.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DataSet ds = new DataSet();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable studentDataTable = new DataTable("Student");
                cmd = new SqlCommand("select * from Student where StudentStatus = 1", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(studentDataTable);
                ds.Tables.Add(studentDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable admissionDataTable = new DataTable("Admission");
                cmd = new SqlCommand("select * from Admission where AdmissionStatus=1 and AcademicSessionId =" + ac + "order by RollNo", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(admissionDataTable);
                ds.Tables.Add(admissionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable academicSessionDataTable = new DataTable("AcademicSession");
                cmd = new SqlCommand("select * from AcademicSession where AcademicSessionId='" + ac + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(academicSessionDataTable);
                ds.Tables.Add(academicSessionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable classDataTable = new DataTable("Class");
                cmd = new SqlCommand("select * from Class" +
                      (cl != 0 ? " where ClassNo =" + cl : ""), connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(classDataTable);
                ds.Tables.Add(classDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable sectionDataTable = new DataTable("Section");
                cmd = new SqlCommand("select * from Section" +
                    (se != 0 ? " where SectionId='" + se + "'" : ""), connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(sectionDataTable);
                ds.Tables.Add(sectionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable examinationDataTable = new DataTable("Examination");
                cmd = new SqlCommand("select * from Examination" +
                    (em != 0 ? " where ExaminationId='" + em + "'" : ""), connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(examinationDataTable);
                ds.Tables.Add(examinationDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                rp.SetDataSource(ds);
                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(LoadData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintVehicleFuelConsumptionReport(int BusId, int SupplierId, DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/vehicle_fuel_consumption.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DataSet ds = new DataSet();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable SupplierDataTable = new DataTable("Supplier");
                cmd = new SqlCommand("select * from Supplier" + (SupplierId == 0 ? "" : " where SupplierId=" + SupplierId), connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(SupplierDataTable);
                ds.Tables.Add(SupplierDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();
                cmd.Dispose();


                DataTable VehicleFuelConsumption = new DataTable("VehicleFuelConsumption");
                cmd = new SqlCommand("select * from VehicleFuelConsumption where 0=0"
                    + (SupplierId == 0 ? "" : " and SupplierId=" + SupplierId)
                    + (BusId == 0 ? "" : " and BusId=" + BusId)
                    + (FromDate == null ? "" : " and cast(BillDate as Date)>='" + FromDate.Value.ToString("yyyy-MM-dd") + "'")
                    + (ToDate == null ? "" : " and cast(BillDate as Date)<='" + ToDate.Value.ToString("yyyy-MM-dd") + "'"), connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(VehicleFuelConsumption);
                ds.Tables.Add(VehicleFuelConsumption);
                dataAdapter.Dispose();
                cmd.Dispose();
                cmd.Dispose();


                DataTable Unit = new DataTable("Unit");
                cmd = new SqlCommand("select * from Unit", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(Unit);
                ds.Tables.Add(Unit);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable Command = new DataTable("Command");
                cmd = new SqlCommand("SELECT FROMDATE='" + (FromDate == null ? "" : FromDate.Value.ToString("dd-MMM-yyyy")) + "', TODATE='"
                    + (ToDate == null ? "" : ToDate.Value.ToString("dd-MMM-yyyy")) + "',SupplierTitle=(select top 1 SupplierName from Supplier where SupplierId=" + SupplierId + ")"
                    , connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(Command);
                ds.Tables.Add(Command);
                dataAdapter.Dispose();
                cmd.Dispose();



                DataTable BusDataTable = new DataTable("Bus");
                cmd = new SqlCommand("select * from Bus" + (BusId == 0 ? "" : " where BusId=" + BusId), connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(BusDataTable);
                ds.Tables.Add(BusDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                rp.SetDataSource(ds);
                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(LoadData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintBarCodeAdmissionNos()
        {
            try
            {
                if (Session["BarCodeAccessionNos"] == null)
                    throw new Exception("Session Expired");
                var accessionNos = (List<BarCodeModel>)Session["BarCodeAccessionNos"];
                LibraryDataSet ds = new LibraryDataSet();
                DataTable barcodeDataTable = ds.BarCode;
                accessionNos.ToList().ForEach(x =>
                {
                    var bImage = BarCodeGenerator.GetBarCode(x.id.ToString());
                    for (var i = 0; i < x.NoOfQuantity; i++)
                    {
                        DataRow bRow = barcodeDataTable.NewRow();
                        bRow["AdmissionNo"] = x.id;
                        bRow["BarCodeImage"] = bImage;
                        barcodeDataTable.Rows.Add(bRow);
                    }
                });

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/admission_no_bar_code_50x20.rpt"));
                rp.SetDataSource(ds);
                System.IO.Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintBookBarCodeWithAccessionNo()
        {
            try
            {
                if (Session["BarCodeAccessionNos"] == null)
                    throw new Exception("Session Expired");
                var accessionNos = (List<BarCodeModel>)Session["BarCodeAccessionNos"];
                LibraryDataSet ds = new LibraryDataSet();
                DataTable barcodeDataTable = ds.BarCode;
                accessionNos.ToList().ForEach(x =>
                {
                    var bImage = BarCodeGenerator.GetBarCode(x.id.ToString());
                    for (var i = 0; i < x.NoOfQuantity; i++)
                    {
                        DataRow bRow = barcodeDataTable.NewRow();
                        bRow["AccessionNo"] = x.id;
                        bRow["BarCodeImage"] = bImage;
                        barcodeDataTable.Rows.Add(bRow);
                    }
                });

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/book_bar_code_50x20.rpt"));
                rp.SetDataSource(ds);
                System.IO.Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintBookBarCode(int FromAccessionNo, int ToAccessionNo, int ReportNo, int Quantity, int ClassNo)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var accessionNos = (from a1 in dataContext.BookAccessions
                                    join b1 in dataContext.Books on a1.BookId equals b1.BookId
                                    where b1.ClassNo == ClassNo
                                    //where a1.AccessionNo >= FromAccessionNo
                                    //&& a1.AccessionNo <= ToAccessionNo
                                    select a1.AccessionNo);


                LibraryDataSet ds = new LibraryDataSet();
                DataTable barcodeDataTable = ds.BarCode;
                accessionNos.ToList().ForEach(x =>
                {
                    if (x.Length > 3)
                    {
                        var no =LoadData.CheckInt(x.Substring(3));
                        if (no >= FromAccessionNo && no <= ToAccessionNo)
                        {
                            var bImage = BarCodeGenerator.GetBarCode(x);
                            for (var i = 0; i < Quantity; i++)
                            {
                                DataRow bRow = barcodeDataTable.NewRow();
                                bRow["AccessionNo"] = x;
                                bRow["BarCodeImage"] = bImage;
                                barcodeDataTable.Rows.Add(bRow);
                            }
                        }
                    }
                    // barcodeDataTable.Rows.Add(bRow);
                });

                ReportDocument rp = new ReportDocument();
                if (ReportNo == 1)
                    rp.Load(Server.MapPath("~/reports/book_bar_code.rpt"));
                else if (ReportNo == 2)
                    rp.Load(Server.MapPath("~/reports/book_bar_code_A4.rpt"));
                else
                    rp.Load(Server.MapPath("~/reports/book_bar_code_50x20.rpt"));

                rp.SetDataSource(ds);
                System.IO.Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public ActionResult TransportReportForAllStudent(int sessionId, int classId, int sectionId)
        {
            try
            {

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/transport-all-student-list-bus.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                var classIds = (from st in dataContext.StudentTransports
                                join s in dataContext.Students
                                on st.StudentId equals s.StudentId
                                join a in dataContext.Admissions
                                on s.StudentId equals a.StudentId
                                join sc in dataContext.Sections
                                on a.SectionId equals sc.SectionId
                                join cl in dataContext.Classes
                                on sc.ClassId equals cl.ClassId
                                where a.AdmissionStatus == 1
                                && a.AcademicSessionId == sessionId
                                && st.Status == 1
                                && (classId == 0 || classId == cl.ClassId)
                                select cl.ClassId).Distinct();
                if (classIds.Count() == 0)
                    throw new Exception("No data found");

                var sectionIds = (from st in dataContext.StudentTransports
                                  join s in dataContext.Students
                                  on st.StudentId equals s.StudentId
                                  join a in dataContext.Admissions
                                  on s.StudentId equals a.StudentId
                                  join sc in dataContext.Sections
                                  on a.SectionId equals sc.SectionId
                                  join cl in dataContext.Classes
                                  on sc.ClassId equals cl.ClassId
                                  where a.AdmissionStatus == 1
                                  && a.AcademicSessionId == sessionId
                                  && st.Status == 1
                                  && (classId == 0 || classId == cl.ClassId)
                                  && (sectionId == 0 || sectionId == sc.SectionId)
                                  select sc.SectionId).Distinct();
                if (sectionIds.Count() == 0)
                    throw new Exception("No data found");

                var studentIds = (from st in dataContext.StudentTransports
                                  join s in dataContext.Students
                                  on st.StudentId equals s.StudentId
                                  join a in dataContext.Admissions
                                  on s.StudentId equals a.StudentId
                                  join sc in dataContext.Sections
                                  on a.SectionId equals sc.SectionId
                                  join cl in dataContext.Classes
                                  on sc.ClassId equals cl.ClassId
                                  where a.AdmissionStatus == 1
                                  && a.AcademicSessionId == sessionId
                                  && st.Status == 1
                                  select s.StudentId).Distinct();


                var admissionIds = (from st in dataContext.StudentTransports
                                    join s in dataContext.Students
                                    on st.StudentId equals s.StudentId
                                    join a in dataContext.Admissions
                                    on s.StudentId equals a.StudentId
                                    join sc in dataContext.Sections
                                    on a.SectionId equals sc.SectionId
                                    join cl in dataContext.Classes
                                    on sc.ClassId equals cl.ClassId
                                    where a.AdmissionStatus == 1
                                    && a.AcademicSessionId == sessionId
                                    && st.Status == 1
                                    select a.AdmissionId).Distinct();

                int schoolId = SessionUtilies.GetSchoolId(dataContext);

                DataSet ds = new DataSet();
                SqlCommand cmd = null;
                DataTable classDataTable = new DataTable("Class");
                cmd = new SqlCommand("select * from Class where ClassId in (" + string.Join(",", classIds) + ")", connection);


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(classDataTable);
                ds.Tables.Add(classDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable schoolDataTable = new DataTable("School");
                cmd = new SqlCommand("select * from School where SchoolStatus=" + schoolId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable studentTransportDataTable = new DataTable("StudentTransport");
                cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1 and a.AcademicSessionId=" + sessionId + " and (st.EndDate is null or st.EndDate >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "') order by cl.ClassId asc", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(studentTransportDataTable);
                ds.Tables.Add(studentTransportDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable sectionBankDataTable = new DataTable("Section");
                cmd = new SqlCommand("select * from Section where SectionId in (" + string.Join(",", sectionIds) + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(sectionBankDataTable);
                ds.Tables.Add(sectionBankDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable transportGroupDataTable = new DataTable("TransportGroup");
                cmd = new SqlCommand("select * from TransportGroup", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(transportGroupDataTable);
                ds.Tables.Add(transportGroupDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable studentDataTable = new DataTable("Student");
                cmd = new SqlCommand("select * from Student where StudentId  in (" + string.Join(",", studentIds) + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(studentDataTable);
                ds.Tables.Add(studentDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable admissionDataTable = new DataTable("Admission");
                cmd = new SqlCommand("select * from Admission where AdmissionId in (" + string.Join(",", admissionIds) + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(admissionDataTable);
                ds.Tables.Add(admissionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable busDataTable = new DataTable("Bus");
                cmd = new SqlCommand("select * from Bus", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(busDataTable);
                ds.Tables.Add(busDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                rp.SetDataSource(ds);

                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult TrasnportReportGroupWise(int transportGroupId, int sessionId, int classId, int sectionId)
        {
            try
            {
                ReportDocument rp = new ReportDocument();

                rp.Load(Server.MapPath("~/Reports/transport-all-student-list-pickup-point.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                var classIds = (from st in dataContext.StudentTransports
                                join s in dataContext.Students
                                on st.StudentId equals s.StudentId
                                join a in dataContext.Admissions
                                on s.StudentId equals a.StudentId
                                join sc in dataContext.Sections
                                on a.SectionId equals sc.SectionId
                                join cl in dataContext.Classes
                                on sc.ClassId equals cl.ClassId
                                where a.AdmissionStatus == 1
                                && a.AcademicSessionId == sessionId
                                && st.Status == 1
                                && (classId == 0 || classId == cl.ClassId)
                                select cl.ClassId).Distinct();
                if (classIds.Count() == 0)
                    throw new Exception("No data found");

                var sectionIds = (from st in dataContext.StudentTransports
                                  join s in dataContext.Students
                                  on st.StudentId equals s.StudentId
                                  join a in dataContext.Admissions
                                  on s.StudentId equals a.StudentId
                                  join sc in dataContext.Sections
                                  on a.SectionId equals sc.SectionId
                                  join cl in dataContext.Classes
                                  on sc.ClassId equals cl.ClassId
                                  where a.AdmissionStatus == 1
                                  && a.AcademicSessionId == sessionId
                                  && st.Status == 1
                                  && (classId == 0 || classId == cl.ClassId)
                                  && (sectionId == 0 || sectionId == sc.SectionId)
                                  select sc.SectionId).Distinct();
                if (sectionIds.Count() == 0)
                    throw new Exception("No data found");

                var studentIds = (from st in dataContext.StudentTransports
                                  join s in dataContext.Students
                                  on st.StudentId equals s.StudentId
                                  join a in dataContext.Admissions
                                  on s.StudentId equals a.StudentId
                                  join sc in dataContext.Sections
                                  on a.SectionId equals sc.SectionId
                                  join cl in dataContext.Classes
                                  on sc.ClassId equals cl.ClassId
                                  where a.AdmissionStatus == 1
                                  && a.AcademicSessionId == sessionId
                                  && st.Status == 1
                                  select s.StudentId).Distinct();


                var admissionIds = (from st in dataContext.StudentTransports
                                    join s in dataContext.Students
                                    on st.StudentId equals s.StudentId
                                    join a in dataContext.Admissions
                                    on s.StudentId equals a.StudentId
                                    join sc in dataContext.Sections
                                    on a.SectionId equals sc.SectionId
                                    join cl in dataContext.Classes
                                    on sc.ClassId equals cl.ClassId
                                    where a.AdmissionStatus == 1
                                    && a.AcademicSessionId == sessionId
                                    && st.Status == 1
                                    select a.AdmissionId).Distinct();

                DataSet ds = new DataSet();
                SqlCommand cmd = null;
                DataTable classDataTable = new DataTable("Class");
                cmd = new SqlCommand("select * from Class where ClassId in (" + string.Join(",", classIds) + ")", connection);


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(classDataTable);
                ds.Tables.Add(classDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable schoolDataTable = new DataTable("School");
                cmd = new SqlCommand("select * from School where SchoolStatus=1", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();



                DataTable studentTransportDataTable = new DataTable("StudentTransport");
                if (transportGroupId > 0)
                    cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1 and st.TransportGroupId=" + transportGroupId + " and a.AcademicSessionId=" + sessionId + " and (st.EndDate is null or st.EndDate >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "') order by cl.ClassId asc", connection);
                else
                    cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1 and a.AcademicSessionId=" + sessionId + " and (st.EndDate is null or st.EndDate >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "') order by cl.ClassId asc", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(studentTransportDataTable);
                ds.Tables.Add(studentTransportDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable sectionBankDataTable = new DataTable("Section");
                cmd = new SqlCommand("select * from Section where SectionId in (" + string.Join(",", sectionIds) + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(sectionBankDataTable);
                ds.Tables.Add(sectionBankDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable transportGroupDataTable = new DataTable("TransportGroup");
                cmd = new SqlCommand("select * from TransportGroup", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(transportGroupDataTable);
                ds.Tables.Add(transportGroupDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable studentDataTable = new DataTable("Student");
                cmd = new SqlCommand("select * from Student where StudentId  in (" + string.Join(",", studentIds) + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(studentDataTable);
                ds.Tables.Add(studentDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable admissionDataTable = new DataTable("Admission");
                cmd = new SqlCommand("select * from Admission where AdmissionId in (" + string.Join(",", admissionIds) + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(admissionDataTable);
                ds.Tables.Add(admissionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable busDataTable = new DataTable("Bus");
                cmd = new SqlCommand("select * from Bus", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(busDataTable);
                ds.Tables.Add(busDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                rp.SetDataSource(ds);

                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;

            }
            return View();
        }
        public ActionResult TransportReportBusWise(int busId, int sessionId, int classId, int sectionId)
        {
            try
            {
                ReportDocument rp = new ReportDocument();

                rp.Load(Server.MapPath("~/Reports/transport-all-student-list-bus.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);

                var classIds = (from st in dataContext.StudentTransports
                                join s in dataContext.Students
                                on st.StudentId equals s.StudentId
                                join a in dataContext.Admissions
                                on s.StudentId equals a.StudentId
                                join sc in dataContext.Sections
                                on a.SectionId equals sc.SectionId
                                join cl in dataContext.Classes
                                on sc.ClassId equals cl.ClassId
                                where a.AdmissionStatus == 1
                                && a.AcademicSessionId == sessionId
                                && st.Status == 1
                                && (classId == 0 || classId == cl.ClassId)
                                select cl.ClassId).Distinct();
                if (classIds.Count() == 0)
                    throw new Exception("No data found");

                var sectionIds = (from st in dataContext.StudentTransports
                                  join s in dataContext.Students
                                  on st.StudentId equals s.StudentId
                                  join a in dataContext.Admissions
                                  on s.StudentId equals a.StudentId
                                  join sc in dataContext.Sections
                                  on a.SectionId equals sc.SectionId
                                  join cl in dataContext.Classes
                                  on sc.ClassId equals cl.ClassId
                                  where a.AdmissionStatus == 1
                                  && a.AcademicSessionId == sessionId
                                  && st.Status == 1
                                  && (classId == 0 || classId == cl.ClassId)
                                  && (sectionId == 0 || sectionId == sc.SectionId)
                                  select sc.SectionId).Distinct();
                if (sectionIds.Count() == 0)
                    throw new Exception("No data found");
                var studentIds = (from st in dataContext.StudentTransports
                                  join s in dataContext.Students
                                  on st.StudentId equals s.StudentId
                                  join a in dataContext.Admissions
                                  on s.StudentId equals a.StudentId
                                  join sc in dataContext.Sections
                                  on a.SectionId equals sc.SectionId
                                  join cl in dataContext.Classes
                                  on sc.ClassId equals cl.ClassId
                                  where a.AdmissionStatus == 1
                                  && a.AcademicSessionId == sessionId
                                  && st.Status == 1
                                  select s.StudentId).Distinct();


                var admissionIds = (from st in dataContext.StudentTransports
                                    join s in dataContext.Students
                                    on st.StudentId equals s.StudentId
                                    join a in dataContext.Admissions
                                    on s.StudentId equals a.StudentId
                                    join sc in dataContext.Sections
                                    on a.SectionId equals sc.SectionId
                                    join cl in dataContext.Classes
                                    on sc.ClassId equals cl.ClassId
                                    where a.AdmissionStatus == 1
                                    && a.AcademicSessionId == sessionId
                                    && st.Status == 1
                                    select a.AdmissionId).Distinct();

                DataSet ds = new DataSet();
                SqlCommand cmd = null;
                DataTable classDataTable = new DataTable("Class");
                cmd = new SqlCommand("select * from Class where ClassId in (" + string.Join(",", classIds) + ")", connection);


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(classDataTable);
                ds.Tables.Add(classDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable schoolDataTable = new DataTable("School");
                cmd = new SqlCommand("select * from School where SchoolStatus=1", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();



                DataTable studentTransportDataTable = new DataTable("StudentTransport");

                if (busId > 0)
                    cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1 and st.BusId=" + busId + " and a.AcademicSessionId=" + sessionId + " and (st.EndDate is null or st.EndDate >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "') order by cl.ClassId asc", connection);
                else
                    cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1  and a.AcademicSessionId=" + sessionId + " and (st.EndDate is null or st.EndDate >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "') order by cl.ClassId asc", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(studentTransportDataTable);
                ds.Tables.Add(studentTransportDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable sectionBankDataTable = new DataTable("Section");
                cmd = new SqlCommand("select * from Section where SectionId in (" + string.Join(",", sectionIds) + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(sectionBankDataTable);
                ds.Tables.Add(sectionBankDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable transportGroupDataTable = new DataTable("TransportGroup");
                cmd = new SqlCommand("select * from TransportGroup", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(transportGroupDataTable);
                ds.Tables.Add(transportGroupDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable studentDataTable = new DataTable("Student");
                cmd = new SqlCommand("select * from Student where StudentId  in (" + string.Join(",", studentIds) + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(studentDataTable);
                ds.Tables.Add(studentDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable admissionDataTable = new DataTable("Admission");
                cmd = new SqlCommand("select * from Admission where AdmissionId in (" + string.Join(",", admissionIds) + ")", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(admissionDataTable);
                ds.Tables.Add(admissionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable busDataTable = new DataTable("Bus");
                cmd = new SqlCommand("select * from Bus", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(busDataTable);
                ds.Tables.Add(busDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                rp.SetDataSource(ds);

                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        public ActionResult PrintRegistrationFeeReceiptOnline(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/registration-receipt-online.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);

                DataSet ds = new DataSet();
                var registration = (from m1 in dataContext.RegistrationOnlines
                                    join f1 in dataContext.FeeCollections
                                    on m1.RegistrationOnlineId equals f1.RegistrationOnlineId
                                    where m1.RegistrationOnlineId == Convert.ToInt32(id)
                                    select new
                                    {
                                        f1.FeeCollectionId,
                                        m1.RegistrationOnlineId,
                                        m1.AcademicSessionId,
                                        m1.ClassNo,
                                        m1.StudentPhoto,
                                    }).FirstOrDefault();
                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable RegistrationOnlineDataTable = new DataTable("RegistrationOnline");
                cmd = new SqlCommand("select * from RegistrationOnline where RegistrationOnlineId=" + registration.RegistrationOnlineId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(RegistrationOnlineDataTable);
                ds.Tables.Add(RegistrationOnlineDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable RegistrationOnlineSiblingsDataTable = new DataTable("RegistrationSiblings");
                cmd = new SqlCommand("select * from RegistrationSiblings where RegistrationOnlineId=" + registration.RegistrationOnlineId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(RegistrationOnlineSiblingsDataTable);
                ds.Tables.Add(RegistrationOnlineSiblingsDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable FeeCollectionTable = new DataTable("FeeCollection");
                cmd = new SqlCommand("select * from FeeCollection where FeeCollectionId=" + registration.FeeCollectionId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(FeeCollectionTable);
                ds.Tables.Add(FeeCollectionTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable academicSessionDataTable = new DataTable("AcademicSession");
                cmd = new SqlCommand("select * from AcademicSession where AcademicSessionId='" + registration.AcademicSessionId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(academicSessionDataTable);
                ds.Tables.Add(academicSessionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable classDataTable = new DataTable("Class");
                cmd = new SqlCommand("select distinct ClassName = ClassName2,ClassNo from Class where ClassNo='" + registration.ClassNo + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(classDataTable);
                ds.Tables.Add(classDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable CommandDataTable = new DataTable("Command");
                cmd = new SqlCommand("select r1.RegistrationOnlineId,r1.StudentName,ClassName =c1.ClassName2,SPhoto=cast(null as image) from RegistrationSiblings as r1 join Class as c1 on r1.ClassId = c1.ClassId where r1.RegistrationOnlineId = " + registration.RegistrationOnlineId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(CommandDataTable);
                ds.Tables.Add(CommandDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                //DataTable Command_1DataTable = new DataTable("Command_1");
                //cmd = new SqlCommand("select SPhoto=cast(null as image) from RegistrationOnline where RegistrationOnlineId = " + registration.RegistrationOnlineId, connection);
                //dataAdapter = new SqlDataAdapter(cmd);
                //dataAdapter.Fill(Command_1DataTable);
                //ds.Tables.Add(Command_1DataTable);
                //dataAdapter.Dispose();
                //cmd.Dispose();


                //foreach (DataRow dr in Command_1DataTable.Rows)
                //    dr["SPhoto"] = LoadData.GetImageFromUrl(registration.StudentPhoto);

                DataTable qCode = new DataTable("Command_1");
                DataColumn dataColumn = new DataColumn("SPhoto", typeof(byte[]));
                qCode.Columns.Add(dataColumn);
                qCode.Rows.Add(new object[] { LoadData.GetImageFromUrl(registration.StudentPhoto) });
                ds.Tables.Add(qCode);
                dataAdapter.Dispose();
                cmd.Dispose();

                dataAdapter.Dispose();
                cmd.Dispose();

                rp.SetDataSource(ds);
                Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(LoadData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }


    }
}