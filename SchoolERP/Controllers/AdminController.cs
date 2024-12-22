using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using OfficeOpenXml;
using School;
using School.DataSets;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SchoolERP.Controllers
{
    [CheckSessionTimeOutAttribute]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult LayoutAdmin()
        {
            return View();
        }
        public ActionResult ManageAcademicSession()
        {
            return View();
        }
        public ActionResult RegistrationOffline()
        {
            return View();
        }
        public ActionResult ManageHostelFeePeriod()
        {
            return View();
        }
        public ActionResult ManageHostelFeeHead()
        {
            return View();
        }
        public ActionResult ManageHostelFee()
        {
            return View();
        }
        public ActionResult ManageStudentHostelFee()
        {
            return View();
        }
        public ActionResult ManageHostel()
        {
            return View();
        }
        public ActionResult ManageSemester()
        {
            return View();
        }
        public ActionResult ManageRefund()
        {
            return View();
        }
        public ActionResult BoardMarksList()
        {
            return View();
        }
        public ActionResult RefundList()
        {
            return View();
        }
        public ActionResult BonafideCertificateList()
        {
            return View();
        }
        public ActionResult GeneratePaymentLinkTwo()
        {
            return View();
        }
        public ActionResult TeacherFeedbackList()
        {
            return View();
        }
        public ActionResult TeacherFeedbackDetail()
        {
            return View();
        }
        public ActionResult TeacherFeedbackForm()
        {
            return View();
        }
        public ActionResult AttendanceReport()
        {
            return View();
        }
        public ActionResult PrintMarksheetRetake()
        {
            return View();
        }
        public ActionResult RetakeMarksEntry()
        {
            return View();
        }
        public ActionResult UpdateSuccessFeePaymentStatus()
        {
            return View();
        }
        public ActionResult UpdateAttendenceCode()
        {
            return View();
        }
        public ActionResult LeaveReport()
        {
            return View();
        }
        public ActionResult BankStatement()
        {
            return View();
        }
        public ActionResult StaffPaymentList()
        {
            return View();
        }
        public ActionResult ManageStaffAdvancePayment()
        {
            return View();
        }
        public ActionResult StaffPayment()
        {
            return View();
        }
        public ActionResult ManageStaffLeave()
        {
            return View();
        }
        public ActionResult GenerateTimeTableTwo()
        {
            return View();
        }
        public ActionResult ManageStaffSalary()
        {
            return View();
        }
        public ActionResult ManageLeave()
        {
            return View();
        }
        public ActionResult ExamResultPublish()
        {
            return View();
        }
        public ActionResult AllowMarksheetDownload()
        {
            return View();
        }
        public ActionResult ExamFullMarksEntry()
        {
            return View();
        }
        public ActionResult RegistrationOnlineListForAdmissionDetail()
        {
            return View();
        }
        public ActionResult AttendenceList()
        {
            return View();
        }
        public ActionResult AllStudentDetailUpdate()
        {
            return View();
        }
        public ActionResult SearchRegistrationOnline()
        {
            return View();
        }
        public ActionResult ManageStaffGroupShift()
        {
            return View();
        }
        public ActionResult ManageDevice()
        {
            return View();
        }
        public ActionResult ManageShift()
        {
            return View();
        }
        public ActionResult ManageStaffGroup()
        {
            return View();
        }
        public ActionResult RegistrationOnlineResultList()
        {
            return View();
        }
        public ActionResult RegistrationOnlineResult()
        {
            return View();
        }
        public ActionResult PaidPaymentLinkList()
        {
            return View();
        }
        public ActionResult PaymentLinkList()
        {
            return View();
        }
        public ActionResult AdmitCardListForOR()
        {
            return View();
        }
        public ActionResult AdmissionNoPrint()
        {
            return View();
        }
        public ActionResult GenerateAdmitCard()
        {
            return View();
        }
        public ActionResult ManageExamCenter()
        {
            return View();
        }
        public ActionResult UpdateStudentStreamType()
        {
            return View();
        }
        public ActionResult AdmitCardPrint()
        {
            return View();
        }
        public ActionResult TeacherPerformanceList()
        {
            return View();
        }
        public ActionResult StudentFeedBackList()
        {
            return View();
        }
        public ActionResult ManageDesignation()
        {
            return View();
        }
        public ActionResult RegistrationOnline()
        {
            return View();
        }
        public ActionResult GenerateAdmissionPaymentLink()
        {
            return View();
        }
        public ActionResult ParentFeedBackList()
        {
            return View();
        }
        public ActionResult ManageVehicleRunningRecord()
        {
            return View();
        }
        public ActionResult VehicleServiceList()
        {
            return View();
        }
        public ActionResult VehicleService()
        {
            return View();
        }
        public ActionResult VehicleServiceEdit(string id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult BookAccessionListWithRack()
        {
            return View();
        }
        public ActionResult VehicleFuelConsumptionList()
        {
            return View();
        }
        public ActionResult VehicleFuelConsumption()
        {
            return View();
        }
        public ActionResult ManageVehicleType()
        {
            return View();
        }
        public ActionResult ManageParent()
        {
            return View();
        }
        public ActionResult ManageMagazine()
        {
            return View();
        }
        public ActionResult DailyCollectionReportStudentwise()
        {
            return View();
        }
        public ActionResult DailyCollectionReportStudentwise2()
        {
            return View();
        }
        public ActionResult ManageBookSubjectRack()
        {
            return View();
        }
        public ActionResult ManageRack()
        {
            return View();
        }
        public ActionResult ManageBookSubject()
        {
            return View();
        }
        public ActionResult StoreItemIssue()
        {
            return View();
        }
        public ActionResult StoreItemIssueReport()
        {
            return View();
        }
        public ActionResult StoreItemReturnReport()
        {
            return View();
        }
        public ActionResult PurchaseStoreItem()
        {
            return View();
        }
        public ActionResult PurchaseStoreItemList()
        {
            return View();
        }
        public ActionResult ManageStoreItem()
        {
            return View();
        }
        public ActionResult NewsPaperPurchase()
        {
            return View();
        }
        public ActionResult ManageUnit()
        {
            return View();
        }
        public ActionResult NewsPaperPurchaseList()
        {
            return View();
        }
        public ActionResult BookIssueReport()
        {
            return View();
        }
        public ActionResult BookReturnReport()
        {
            return View();
        }
        public ActionResult ManageBookAccession()
        {
            return View();
        }
        public ActionResult MagazinePurchase()
        {
            return View();
        }
        public ActionResult PrintBookBarCode()
        {
            return View();
        }
        public ActionResult PrintBookBarCode2()
        {
            return View();
        }
        public ActionResult InvalidUrl()
        {
            return View();
        }
        public ActionResult ManageItemType()
        {
            return View();
        }
        public ActionResult BookReIssue()
        {
            return View();
        }
        public ActionResult BookIssue()
        {
            return View();
        }
        public ActionResult BookIssueHistory()
        {
            return View();
        }
        public ActionResult BookReturn()
        {
            return View();
        }
        public ActionResult ManageStaff()
        {
            return View();
        }
        public ActionResult ManageBookType()
        {
            return View();
        }
        public ActionResult ManageDepartment()
        {
            return View();
        }
        public ActionResult TeacherArragement()
        {
            return View();
        }
        public ActionResult BookList()
        {
            return View();
        }
        public ActionResult BookAccessionList()
        {
            return View();
        }
        public ActionResult ManageSupplier()
        {
            return View();
        }
        public ActionResult PurchaseList()
        {
            return View();
        }
        public ActionResult BookPurchase()
        {
            return View();
        }
        public ActionResult OnlineAdmission()
        {
            return View();
        }
        public ActionResult RegistrationOnlineList()
        {
            return View();
        }
        public ActionResult MarksAnalysis()
        {
            return View();
        }
        public ActionResult ManageRegistrationForm()
        {
            return View();
        }
        public ActionResult ExamRankerAnalysisList()
        {
            return View();
        }
        public ActionResult CrossReportTermwise()
        {
            return View();
        }
        public ActionResult CrossReportExamwise()
        {
            return View();
        }
        public ActionResult ExamRankerList()
        {
            return View();
        }
        public ActionResult ExamRankerListTermWise()
        {
            return View();
        }
        public ActionResult EditReceipt()
        {
            return View();
        }
        public ActionResult ManageSection()
        {
            return View();
        }
        public ActionResult ManageDocumentUpload()
        {
            return View();
        }
        public ActionResult ManageClassStreamType()
        {
            return View();
        }
        public ActionResult ManageSubjectOption()
        {
            return View();
        }
        public ActionResult AdmissionListWithFields()
        {
            return View();
        }
        public ActionResult ExamAttendenceEntryByExcel()
        {
            return View();
        }
        public ActionResult ExamAttendenceEntry()
        {
            return View();
        }
        public ActionResult ExamAchievementEntry()
        {
            return View();
        }
        public ActionResult ExamSectionAttendenceEntry()
        {
            return View();
        }
        public ActionResult MarksEntryByExcel()
        {
            return View();
        }
        public ActionResult ManageExamination()
        {
            return View();
        }
        public ActionResult GenerateMarksheet()
        {
            return View();
        }
        public ActionResult ManageCity()
        {
            return View();
        }
        public ActionResult ManageState()
        {
            return View();
        }
        public ActionResult ManageExamSubSubject()
        {
            return View();
        }
        public ActionResult ManageExamGradingSystem()
        {
            return View();
        }
        public ActionResult ManageClassWeightage()
        {
            return View();
        }
        public ActionResult TuitionFeeDueList()
        {
            return View();
        }
        public ActionResult PrintMarksheet()
        {
            return View();
        }
        public ActionResult ManageExamClassGrade()
        {
            return View();
        }
        public ActionResult ManageClassExam()
        {
            return View();
        }
        public ActionResult ClassSubjectTeacherList()
        {
            return View();
        }
        public ActionResult ManageExamStudentSubjectClasswise()
        {
            return View();
        }
        public ActionResult ScholasticMarksEntry()
        {
            return View();
        }
        public ActionResult MarksheetPrint()
        {
            return View();
        }
        public ActionResult SendStudentMatksheet()
        {
            return View();
        }
        public ActionResult MarksGradeEntry()
        {
            return View();
        }
        public ActionResult MarksGradeEntryByExcel()
        {
            return View();
        }
        public ActionResult MarksEntry()
        {
            return View();
        }
        public ActionResult TeacherPeriodCombination()
        {
            return View();
        }
        public ActionResult ManageExamStudentSubject()
        {
            return View();
        }
        public ActionResult ManageExamSubject()
        {
            return View();
        }
        public ActionResult ManageExamClassSubject()
        {
            return View();
        }
        public ActionResult SectionPeriodList()
        {
            return View();
        }
        public ActionResult ManageLab()
        {
            return View();
        }
        public ActionResult ManageStudentConcession()
        {
            return View();
        }
        public ActionResult UpdateFeePaymentStatus()
        {
            return View();
        }
        public ActionResult UpdateStudentFeePayment()
        {
            return View();
        }
        public ActionResult GenerateTimeTable()
        {
            return View();
        }
        public ActionResult ManageSectionCombination()
        {
            return View();
        }
        public ActionResult FeePaymentHistory()
        {
            return View();
        }
        public ActionResult IssueTransferCertificate()
        {
            return View();
        }
        public ActionResult TransferCertificateList()
        {
            return View();
        }
        public ActionResult IssueCharacterCertificate()
        {
            return View();
        }
        public ActionResult CharacterCertificateList()
        {
            return View();
        }
        public ActionResult TimeTableEdit()
        {
            return View();
        }
        public ActionResult ClassTeacherList()
        {
            return View();
        }
        public ActionResult ClassTimeTableList()
        {
            return View();
        }
        public ActionResult TeacherTimeTable()
        {
            return View();
        }
        public ActionResult TimeTable()
        {
            return View();
        }
        public ActionResult TeacherSession()
        {
            return View();
        }
        public ActionResult StudentAdmissionList()
        {
            return View();
        }
        public ActionResult ManageTeacher()
        {
            return View();
        }
        public ActionResult ManageSubject()
        {
            return View();
        }
        public ActionResult ManageLibrarySubject()
        {
            return View();
        }
        public ActionResult ManageClassSubject()
        {
            return View();
        }
        public ActionResult StudentLeft()
        {
            return View();
        }
        public ActionResult StudentLeftList()
        {
            return View();
        }
        public ActionResult Cashbook()
        {
            return View();
        }
        public ActionResult Chequebook()
        {
            return View();
        }
        public ActionResult FeeDueReport()
        {
            return View();
        }
        public ActionResult FeeDueReport2()
        {
            return View();
        }
        public ActionResult FeePaymentDelete()
        {
            return View();
        }
        public ActionResult FeePaymentCancle()
        {
            return View();
        }
        public ActionResult AdmissionNoUpdate()
        {
            return View();
        }
        public ActionResult UpdateStudentRollNo()
        {
            return View();
        }
        public ActionResult DCRUserwise()
        {
            return View();
        }
        public ActionResult MiscellaneousFee()
        {
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult ManageStudentSubjectCombination()
        {
            return View();
        }
        public ActionResult FeeCollectionReport()
        {
            return View();
        }
        public ActionResult SubjectCombinationFee()
        {
            return View();
        }
        public ActionResult StudentRegistrationUpdate()
        {
            return View();
        }
        public ActionResult StudentRegistrationList()
        {
            return View();
        }
        public ActionResult StudentRegistration()
        {
            return View();
        }
        public ActionResult ManageStudentCharge()
        {
            return View();
        }
        public ActionResult DailyCollectionReport()
        {
            return View();
        }
        public ActionResult DailyConcessionReport()
        {
            return View();
        }
        public ActionResult DailyConcessionReportStudentwise()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ClassPromotion()
        {
            return View();
        }
        public ActionResult NewAdmission()
        {
            return View();
        }
        public ActionResult AdmissionList()
        {
            return View();
        }
        public ActionResult StudentPhotoUpdate()
        {
            return View();
        }
        public ActionResult StudentDetailUpdate()
        {
            return View();
        }
        public ActionResult ClassFeeHead()
        {
            return View();
        }
        public ActionResult ClassFee()
        {
            return View();
        }
        public ActionResult RegistrationFeeHead()
        {
            return View();
        }
        public ActionResult RegistrationFee()
        {
            return View();
        }
        public ActionResult AdmissionFeeHead()
        {
            return View();
        }
        public ActionResult AdmissionFee()
        {
            return View();
        }
        public ActionResult FeePayment()
        {
            return View();
        }
        public ActionResult FeePaymentWithEdit()
        {
            return View();
        }        
        
        public ActionResult SettlementDetails()
        {
            return View();
        }
        
        public ActionResult SettlementReceiptWise()
        {
            return View();
        }




        //------Transport---------------

        public ActionResult manage_transport_fee_period()
        {
            return View();
        }

        public ActionResult manage_transport_group()
        {
            return View();
        }
        public ActionResult manage_transport_fee_head()
        {
            return View();
        }

        public ActionResult manage_transport_fee()
        {
            return View();
        }

        public ActionResult manage_bus()
        {
            return View();
        }


        public ActionResult manage_student_transport()
        {
            return View();
        }

        public ActionResult student_transport_report()
        {
            return View();
        }
        public ActionResult TransportReportForClassWise(int sectionId)
        {
            return View();
        }
        //-------------------Transport End---------------------------

        public ActionResult PrintTransferCertificate(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/transfer-certificate.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                var SchoolId = dataContext.TransferCertificates.Where(x => x.TransferCertificateId == Convert.ToInt32(id)).First().Admission.Student.StudentType.SchoolId;

                DataSet ds = new DataSet();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School where SchoolId=" + SchoolId, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable ccDataTable = new DataTable("TransferCertificate");
                cmd = new SqlCommand("select * from TransferCertificate where TransferCertificateId=" + id, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ccDataTable);
                ds.Tables.Add(ccDataTable);
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

        public ActionResult PrintAllTransferCertificate(int AcademicSessionId, int ClassId, int SectionId)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/Transfer-certificate.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                var SchoolId = dataContext.Schools.Where(x => x.SchoolStatus == (byte)SchoolStatus.Active).First().SchoolId;

                DataSet ds = new DataSet();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School where SchoolId=" + SchoolId, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable ccDataTable = new DataTable("TransferCertificate");
                cmd = new SqlCommand("select cc.AdmissionDate,cc.AdmissionId,cc.AdmissionNo,cc.AnyFeeConcessionAvailed,cc.ClassName,cc.DateOfApplicationOfCertiticate,cc.DateOfBirth,cc.DateOfIssue,cc.ExamPassedClass, cc.ExtraCurricularActivities, cc.FatherName,cc.GeneralConduct,cc.LastStudiedClass,cc.MonthUptoPaid,cc.MotherName,cc.Nationality,cc.QualifiedForClass, cc.ReasonForLeaving,cc.Remarks,cc.StudentName,cc.SubjectStudied1,cc.SubjectStudied2,cc.SubjectStudied3,cc.SubjectStudied4,cc.SubjectStudied5,cc.SubjectStudied6,cc.TotalPresentDays,cc.TotalWorkingDays,cc.TransferCertificateId,cc.TransferCertificateNo,cc.WhetherFailedTwice,cc.WhetherNCCCadet,cc.WhetherQualifiedForPromotion,cc.WhetherSCOrST from TransferCertificate  as cc" +
                                     " join Admission as a1 on cc.AdmissionId = a1.AdmissionId" +
                                     " join Section as s1 on a1.SectionId = s1.SectionId" +
                                     " where (a1.AcademicSessionId =" + AcademicSessionId + " or " + AcademicSessionId + "=0)" +
                                     " and (s1.ClassId =" + ClassId + " or " + ClassId + "=0)" +
                                     " and (s1.SectionId =" + SectionId + " or " + SectionId + "=0)", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ccDataTable);
                ds.Tables.Add(ccDataTable);
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

        public ActionResult PrintAdmissionList(int AcademicSessionId, int ClassId, int SectionId)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/admission-list.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DataSet ds = new DataSet();

                DataTable StudentDataTable = new DataTable("Student");
                SqlCommand cmd = new SqlCommand("select * from Student where StudentStatus != 10", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(StudentDataTable);
                ds.Tables.Add(StudentDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable AdmissionDataTable = new DataTable("Admission");
                cmd = new SqlCommand("select * from Admission where AcademicSessionId=" + AcademicSessionId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(AdmissionDataTable);
                ds.Tables.Add(AdmissionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable ClassDataTable = new DataTable("Class");
                if (ClassId > 0)
                    cmd = new SqlCommand("select * from Class where ClassId=" + ClassId, connection);
                else
                    cmd = new SqlCommand("select * from Class", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ClassDataTable);
                ds.Tables.Add(ClassDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable SectionDataTable = new DataTable("Section");
                if (SectionId > 0)
                    cmd = new SqlCommand("select * from Section where SectionId=" + SectionId, connection);
                else
                    cmd = new SqlCommand("select * from Section", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(SectionDataTable);
                ds.Tables.Add(SectionDataTable);
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

        public ActionResult PrintCharacterCertificate(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/character-certificate.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                var SchoolId = dataContext.CharacterCertificates.Where(x => x.CharacterCertificateId == Convert.ToInt32(id)).First().Admission.Student.StudentType.SchoolId;

                DataSet ds = new DataSet();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School where SchoolId=" + SchoolId, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable ccDataTable = new DataTable("CharacterCertificate");
                cmd = new SqlCommand("select * from CharacterCertificate where CharacterCertificateId=" + id, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ccDataTable);
                ds.Tables.Add(ccDataTable);
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

        public ActionResult PrintBonafideCertificate(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/bonafide-certificate.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                var SchoolId = dataContext.BonafideCertificates.Where(x => x.BonafideCertificateId == Convert.ToInt32(id)).First().Admission.Student.StudentType.SchoolId;

                DataSet ds = new DataSet();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School where SchoolId=" + SchoolId, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable ccDataTable = new DataTable("BonafideCertificate");
                cmd = new SqlCommand("select * from BonafideCertificate where BonafideCertificateId=" + id, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ccDataTable);
                ds.Tables.Add(ccDataTable);
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

        public ActionResult PrintAllCharacterCertificate(int AcademicSessionId, int ClassId, int SectionId)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/character-certificate.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                var SchoolId = dataContext.Schools.Where(x => x.SchoolStatus == (byte)SchoolStatus.Active).First().SchoolId;

                DataSet ds = new DataSet();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School where SchoolId=" + SchoolId, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable ccDataTable = new DataTable("CharacterCertificate");
                cmd = new SqlCommand("select cc.AdmissionId,cc.AdmissionNo,cc.CharacterCertificateId,cc.ClassName,cc.DateOfBirth,cc.FatherName,cc.FromDate,cc.Gender,cc.IssueDate,cc.MotherName,cc.StudentName,cc.ToDate from CharacterCertificate  as cc" +
                                     " join Admission as a1 on cc.AdmissionId = a1.AdmissionId" +
                                     " join Section as s1 on a1.SectionId = s1.SectionId" +
                                     " where (a1.AcademicSessionId =" + AcademicSessionId + " or " + AcademicSessionId + "=0)" +
                                     " and (s1.ClassId =" + ClassId + " or " + ClassId + "=0)" +
                                     " and (s1.SectionId =" + SectionId + " or " + SectionId + "=0)", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ccDataTable);
                ds.Tables.Add(ccDataTable);
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

        public ActionResult PrintCaseFeeReceipt(string id)
        {
            try
            {
               
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);

                DataSet ds = new DataSet();
                var feeCollection = (from fc in dataContext.FeeCollections
                                     where fc.FeeCollectionId == Convert.ToInt32(id)
                                     select fc).First();

                var imageTypeId = dataContext.Classes.First(x => x.ClassId == feeCollection.Admission.Section.ClassId).CourseImageId;



                ReportDocument rp = new ReportDocument();
                if (imageTypeId == 1)
                {
                    rp.Load(Server.MapPath("~/reports/cash-fee-receipt_image1.rpt"));
                }
                else if (imageTypeId == 2)
                {
                    rp.Load(Server.MapPath("~/reports/cash-fee-receipt_image2.rpt"));
                }
                else if (imageTypeId == 3)
                {
                    rp.Load(Server.MapPath("~/reports/cash-fee-receipt_image3.rpt"));
                }
                else
                {
                    rp.Load(Server.MapPath("~/reports/cash-fee-receipt_image.rpt")); 
                }

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School where SchoolId=" + feeCollection.Admission.Student.StudentType.SchoolId, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable studentDataTable = new DataTable("Student");
                cmd = new SqlCommand("select * from Student where StudentId=" + feeCollection.Admission.StudentId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(studentDataTable);
                ds.Tables.Add(studentDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable SystemLoginDataTable = new DataTable("SystemLogin");
                if (feeCollection.CreatedBy.HasValue)
                    cmd = new SqlCommand("select * from SystemLogin where LoginId=" + feeCollection.CreatedBy, connection);
                else
                    cmd = new SqlCommand("select * from SystemLogin where LoginId= 0", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(SystemLoginDataTable);
                ds.Tables.Add(SystemLoginDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();
                DataTable admissionDataTable = new DataTable("Admission");
                cmd = new SqlCommand("select * from Admission where AdmissionId=" + feeCollection.AdmissionId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(admissionDataTable);
                ds.Tables.Add(admissionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable academicSessionDataTable = new DataTable("AcademicSession");
                cmd = new SqlCommand("select * from AcademicSession where AcademicSessionId='" + feeCollection.AcademicSessionId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(academicSessionDataTable);
                ds.Tables.Add(academicSessionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable classDataTable = new DataTable("Class");
                cmd = new SqlCommand("select * from Class where ClassId='" + feeCollection.Admission.Section.ClassId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(classDataTable);
                ds.Tables.Add(classDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable feeCollectionDataTable = new DataTable("FeeCollection");
                cmd = new SqlCommand("select * from FeeCollection where FeeCollectionId='" + feeCollection.FeeCollectionId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(feeCollectionDataTable);
                ds.Tables.Add(feeCollectionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable feeCollectionDetailsDataTable = new DataTable("FeeCollectionDetails");
                cmd = new SqlCommand("select * from FeeCollectionDetails where FeeCollectionId='" + feeCollection.FeeCollectionId + "' order by FeeHeadTitle asc", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(feeCollectionDetailsDataTable);
                ds.Tables.Add(feeCollectionDetailsDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable collegeBankDataTable = new DataTable("Section");
                cmd = new SqlCommand("select * from Section where SectionId='" + feeCollection.Admission.SectionId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(collegeBankDataTable);
                ds.Tables.Add(collegeBankDataTable);
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

        public ActionResult PrintMiscellaneousFeeReceipt(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/cash-fee-receipt-miscellaneous.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DataSet ds = new DataSet();

                var feeCollection = (from fc in dataContext.FeeCollections
                                     where fc.FeeCollectionId == Convert.ToInt32(id)
                                     select fc).First();
                DataTable SystemLoginDataTable = new DataTable("SystemLogin"); ;
                SqlCommand cmd = new SqlCommand("select * from SystemLogin where LoginId=" + feeCollection.CreatedBy, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(SystemLoginDataTable);
                ds.Tables.Add(SystemLoginDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable schoolDataTable = new DataTable("School");
                cmd = new SqlCommand("select * from School where SchoolId=" + feeCollection.Miscellaneous.SchoolId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable MiscellaneousDataTable = new DataTable("Miscellaneous");
                cmd = new SqlCommand("select * from Miscellaneous where MiscellaneousId=" + feeCollection.MisscellaneousId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(MiscellaneousDataTable);
                ds.Tables.Add(MiscellaneousDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable classDataTable = new DataTable("Class");
                cmd = new SqlCommand("select * from Class where ClassId='" + 0 + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(classDataTable);
                ds.Tables.Add(classDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable feeCollectionDataTable = new DataTable("FeeCollection");
                cmd = new SqlCommand("select * from FeeCollection where FeeCollectionId='" + feeCollection.FeeCollectionId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(feeCollectionDataTable);
                ds.Tables.Add(feeCollectionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable feeCollectionDetailsDataTable = new DataTable("FeeCollectionDetails");
                cmd = new SqlCommand("select * from FeeCollectionDetails where FeeCollectionId='" + feeCollection.FeeCollectionId + "' order by FeeHeadTitle asc", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(feeCollectionDetailsDataTable);
                ds.Tables.Add(feeCollectionDetailsDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable collegeBankDataTable = new DataTable("Section");
                cmd = new SqlCommand("select * from Section where SectionId='" + feeCollection.Miscellaneous.SectionId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(collegeBankDataTable);
                ds.Tables.Add(collegeBankDataTable);
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

        public ActionResult PrintFeeCollectionReport(string FromDate, string ToDate, string Pmodes, int session)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/fee-collection-report.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DateTime DateFrom = LoadData.CheckDate(FromDate, "Invalid From Date"), DateTo = LoadData.CheckDate(ToDate, "Invalid To Date");
                List<byte> paymentModes = new List<byte>();
                Pmodes.Split('_').ToList().ForEach(x =>
                {
                    if (x != null)
                        paymentModes.Add(Convert.ToByte(x));
                });

                FeeCollectionReportDataSet ds = new FeeCollectionReportDataSet();

                //Fee Collection By AdmissionId
                var FeeCollectionList = (from fc1 in dataContext.FeeCollections
                                         join a1 in dataContext.Admissions
                                         on fc1.AdmissionId equals a1.AdmissionId
                                         where fc1.PaymentDate.Date >= DateFrom.Date
                                         && fc1.PaymentDate.Date <= DateTo.Date
                                         && fc1.AdmissionId > 0
                                         && paymentModes.Contains(fc1.PaymentMode)
                                         && (session == 0 ? true : fc1.AcademicSessionId == session)
                                         select new PrintFeeCollectionModel
                                         {
                                             ReceiptNo = fc1.ReceiptNo,
                                             AdmissionNo = fc1.Admission.Student.AdmissionNo,
                                             FullName = fc1.Admission.Student.FullName,
                                             ClassName = fc1.Admission.Section.Class.ClassName,
                                             SectionName = fc1.Admission.Section.SectionName,
                                             PaidAmount = fc1.PaidAmount,
                                             PaymentMode = fc1.PaymentMode
                                         }).ToList();


                //Fee Collection By RegistrationId
                FeeCollectionList.AddRange((from fc1 in dataContext.FeeCollections
                                            join sr in dataContext.StudentRegistrations
                                            on fc1.RegistrationId equals sr.StudentRegistrationId
                                            where fc1.PaymentDate.Date >= DateFrom.Date
                                            && fc1.PaymentDate.Date <= DateTo.Date
                                            && fc1.RegistrationId > 0
                                            && paymentModes.Contains(fc1.PaymentMode)
                                            && (session == 0 ? true : sr.AcademicSessionId == session)
                                            select new PrintFeeCollectionModel
                                            {
                                                ReceiptNo = fc1.ReceiptNo,
                                                AdmissionNo = sr.RegistrationNo,
                                                FullName = sr.Name,
                                                ClassName = sr.Class.ClassName,
                                                SectionName = "",
                                                PaidAmount = fc1.PaidAmount,
                                                PaymentMode = fc1.PaymentMode
                                            }).ToList());

                //Fee Collection By Miscellaneous
                FeeCollectionList.AddRange((from fc1 in dataContext.FeeCollections
                                            join sr in dataContext.Miscellaneous
                                            on fc1.MisscellaneousId equals sr.MiscellaneousId
                                            where fc1.PaymentDate.Date >= DateFrom.Date
                                            && fc1.PaymentDate.Date <= DateTo.Date
                                            && fc1.MisscellaneousId > 0
                                            && paymentModes.Contains(fc1.PaymentMode)
                                            && (session == 0 ? true : fc1.AcademicSessionId == session)
                                            select new PrintFeeCollectionModel
                                            {
                                                ReceiptNo = fc1.ReceiptNo,
                                                AdmissionNo = sr.AdmissionNo,
                                                FullName = sr.FullName,
                                                ClassName = sr.Section.Class.ClassName,
                                                SectionName = sr.Section.SectionName,
                                                PaidAmount = fc1.PaidAmount,
                                                PaymentMode = fc1.PaymentMode
                                            }).ToList());

                //Fee Collection By Registration Online
                FeeCollectionList.AddRange((from fc1 in dataContext.FeeCollections
                                            join sr in dataContext.RegistrationOnlines
                                            on fc1.RegistrationOnlineId equals sr.RegistrationOnlineId
                                            join c1 in dataContext.Classes
                                            on sr.ClassNo equals c1.ClassNo
                                            where fc1.PaymentDate.Date >= DateFrom.Date
                                            && fc1.PaymentDate.Date <= DateTo.Date
                                            && fc1.RegistrationOnlineId > 0
                                            && paymentModes.Contains(fc1.PaymentMode)
                                            && (session == 0 ? true : fc1.AcademicSessionId == session)
                                            select new PrintFeeCollectionModel
                                            {
                                                ReceiptNo = fc1.ReceiptNo,
                                                AdmissionNo = sr.RegistrationNo,
                                                FullName = sr.StudentName,
                                                ClassName = c1.ClassName2,
                                                PaidAmount = fc1.PaidAmount,
                                                PaymentMode = fc1.PaymentMode
                                            }).Distinct().ToList());
                DataTable FeeCollectionReportDataTable = ds.FeeCollectionReportDataTable;
                FeeCollectionList.Select(x => new { rp = Convert.ToInt32(x.ReceiptNo), x }).OrderBy(x => x.rp).Select(x => x.x).ToList().ForEach(feeCollection =>
                        {
                            DataRow fcdr = FeeCollectionReportDataTable.NewRow();
                            fcdr["ReceiptNo"] = feeCollection.ReceiptNo;
                            fcdr["AdmissionNo"] = feeCollection.AdmissionNo;
                            fcdr["FullName"] = feeCollection.FullName;
                            fcdr["ClassName"] = feeCollection.ClassName + " / " + feeCollection.SectionName;
                            fcdr["PaymentMode"] = Enum.GetName(typeof(PaymentMode), feeCollection.PaymentMode);
                            fcdr["LineTotal"] = feeCollection.PaidAmount.ToString("N");
                            FeeCollectionReportDataTable.Rows.Add(fcdr);
                        });

                DataTable HeaderDataTable = ds.HeaderDataTable;
                DataRow HeaderDataRow = HeaderDataTable.NewRow();
                HeaderDataRow["FromDate"] = DateFrom.ToString("dd-MM-yyyy");
                HeaderDataRow["ToDate"] = DateTo.ToString("dd-MM-yyyy");
                HeaderDataRow["TotalAmount"] = FeeCollectionList.Any() ? FeeCollectionList.Sum(x => x.PaidAmount).ToString("N") : "0.00";
                if (session == 0)
                    HeaderDataRow["SessionName"] = "All Session";
                else
                    HeaderDataRow["SessionName"] = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == session).First().SessionName;
                HeaderDataTable.Rows.Add(HeaderDataRow);

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

        public ActionResult PrintUserDCR(string FromDate, string ToDate)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/dcr-user-report.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DateTime DateFrom = LoadData.CheckDate(FromDate, "Invalid From Date"), DateTo = LoadData.CheckDate(ToDate, "Invalid To Date");


                UserWiseDCR ds = new UserWiseDCR();

                //Fee Collection By AdmissionId

                var FeeCollectionList = (from fc1 in dataContext.FeeCollections
                                         join fcd in dataContext.FeeCollectionDetails
                                         on fc1.FeeCollectionId equals fcd.FeeCollectionId
                                         where fc1.PaymentDate.Date >= DateFrom.Date
                                         && fc1.PaymentDate.Date <= DateTo.Date
                                         //&& fc1.FeeCollectionStatus != (byte)FeeCollectionStatus.Cancelled
                                         select new { fcd.FeeHeadTitle, fcd.FeeHeadId, fc1.PaymentMode, fcd.Total, fc1.CreatedBy });

                DataTable FeeCollectionDataTable = ds.FeeCollection;
                FeeCollectionList.GroupBy(x => x.FeeHeadId).ToList().ForEach(x =>
                {
                    decimal CashAmount = 0, ChequeAmount = 0, POSAmount = 0, OnlineAmount = 0, QRCodeAmount = 0;


                    var cashAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.Cash);
                    if (cashAmounts.Any())
                        CashAmount = cashAmounts.Sum(y => y.Total ?? 0);

                    var chequeAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.Cheque);
                    if (chequeAmounts.Any())
                        ChequeAmount = chequeAmounts.Sum(y => y.Total ?? 0);

                    var posAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.UPI);
                    if (posAmounts.Any())
                        POSAmount = posAmounts.Sum(y => y.Total ?? 0);

                    var onlineAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.Online);
                    if (onlineAmounts.Any())
                        OnlineAmount = onlineAmounts.Sum(y => y.Total ?? 0);

                    var qrCodeAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.QRCode);
                    if (qrCodeAmounts.Any())
                        QRCodeAmount = qrCodeAmounts.Sum(y => y.Total ?? 0);

                    decimal LineTotal = CashAmount + ChequeAmount + POSAmount + OnlineAmount + QRCodeAmount;
                    if (LineTotal != 0)
                    {
                        DataRow fcdr = FeeCollectionDataTable.NewRow();
                        fcdr["FeeHeadTitle"] = x.First().FeeHeadTitle;
                        fcdr["CashAmount"] = CashAmount.ToString("N");
                        fcdr["ChequeAmount"] = ChequeAmount.ToString("N");
                        fcdr["POSAmount"] = POSAmount.ToString("N");
                        fcdr["OnlineAmount"] = OnlineAmount.ToString("N");
                        fcdr["QRCodeAmount"] = QRCodeAmount.ToString("N");
                        fcdr["LineTotal"] = (LineTotal).ToString("N");
                        FeeCollectionDataTable.Rows.Add(fcdr);
                    }
                });

                decimal TotalCashAmount = 0, TotalChequeAmount = 0, TotalPOSAmount = 0, TotalOnlineAmount = 0, TotalQRCodeAmount = 0;
                DataTable UserFeeCollectionDataTable = ds.UserFeeCollection;
                FeeCollectionList.GroupBy(x => x.CreatedBy).ToList().ForEach(x =>
                {
                    decimal CashAmount = 0, ChequeAmount = 0, POSAmount = 0, OnlineAmount = 0, QRCodeAmount = 0;


                    var cashAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.Cash);
                    if (cashAmounts.Any())
                        CashAmount = cashAmounts.Sum(y => y.Total ?? 0);

                    var chequeAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.Cheque);
                    if (chequeAmounts.Any())
                        ChequeAmount = chequeAmounts.Sum(y => y.Total ?? 0);

                    var posAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.UPI);
                    if (posAmounts.Any())
                        POSAmount = posAmounts.Sum(y => y.Total ?? 0);

                    var onlineAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.Online);
                    if (onlineAmounts.Any())
                        OnlineAmount = onlineAmounts.Sum(y => y.Total ?? 0);

                    var qRCodeAmountAmounts = x.Where(y => y.PaymentMode == (byte)PaymentMode.QRCode);
                    if (qRCodeAmountAmounts.Any())
                        QRCodeAmount = qRCodeAmountAmounts.Sum(y => y.Total ?? 0);


                    DataRow fcdr = UserFeeCollectionDataTable.NewRow();
                    var createdUsers = dataContext.SystemLogins.Where(y => y.LoginId == x.Key);
                    if (createdUsers.Any())
                        fcdr["UserName"] = createdUsers.First().UserName;

                    fcdr["CashAmount"] = CashAmount.ToString("N");
                    fcdr["ChequeAmount"] = ChequeAmount.ToString("N");
                    fcdr["POSAmount"] = POSAmount.ToString("N");
                    fcdr["OnlineAmount"] = OnlineAmount.ToString("N");
                    fcdr["QRCodeAmount"] = QRCodeAmount.ToString("N");
                    fcdr["LineTotal"] = (CashAmount + ChequeAmount + POSAmount + OnlineAmount + QRCodeAmount).ToString("N");
                    UserFeeCollectionDataTable.Rows.Add(fcdr);

                    TotalCashAmount += CashAmount;
                    TotalChequeAmount += ChequeAmount;
                    TotalOnlineAmount += OnlineAmount;
                    TotalPOSAmount += POSAmount;
                    TotalQRCodeAmount += QRCodeAmount;
                });

                DataTable HeaderDataTable = ds.HeaderDataTable;
                DataRow HeaderDataRow = HeaderDataTable.NewRow();
                HeaderDataRow["FromDate"] = DateFrom.ToString("dd-MM-yyyy");
                HeaderDataRow["ToDate"] = DateTo.ToString("dd-MM-yyyy");
                HeaderDataRow["TotalCashAmount"] = TotalCashAmount.ToString("N");
                HeaderDataRow["TotalChequeAmount"] = TotalChequeAmount.ToString("N");
                HeaderDataRow["TotalPOSAmount"] = TotalPOSAmount.ToString("N");
                HeaderDataRow["TotalOnlineAmount"] = TotalOnlineAmount.ToString("N");
                HeaderDataRow["TotalQRCodeAmount"] = TotalQRCodeAmount.ToString("N");
                HeaderDataRow["TotalAmount"] = (TotalCashAmount + TotalChequeAmount + TotalPOSAmount + TotalOnlineAmount + TotalQRCodeAmount).ToString("N");
                HeaderDataTable.Rows.Add(HeaderDataRow);
                var School = dataContext.Schools.First();

                DataTable SchoolDataTable = ds.School;
                DataRow SchoolDataRow = SchoolDataTable.NewRow();
                SchoolDataRow["SchoolName"] = School.FullName;
                SchoolDataRow["MobileNo"] = School.ContactNo;
                SchoolDataRow["Address"] = School.RegisteredOffice;
                SchoolDataRow["Logo"] = School.Logo.ToArray();
                SchoolDataTable.Rows.Add(SchoolDataRow);



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

        public ActionResult PrintCashbook(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/cashbook.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DateTime cashbookDate = LoadData.CheckDate(id, "Invalid Cassbook Date");


                CashbookDataSet ds = new CashbookDataSet();

                //Fee Collection By AdmissionId
                var FeeCollectionList = (from fc1 in dataContext.FeeCollections
                                         join fcd in dataContext.FeeCollectionDetails
                                         on fc1.FeeCollectionId equals fcd.FeeCollectionId
                                         where fc1.PaymentDate.Date == cashbookDate.Date
                                         && fc1.FeeCollectionStatus != (byte)FeeCollectionStatus.Cancelled
                                         && fc1.PaymentMode == (byte)PaymentMode.Cash
                                         select new { fcd.FeeHeadTitle, fcd.FeeHeadId, fc1.PaymentMode, fcd.Total, fc1.CreatedBy });

                var cashBooks = dataContext.Cashbooks.Where(x => x.CashbookDate.Date == cashbookDate.Date);
                decimal FeeHeadTotalAmount = 0, UserTotalAmount = 0;

                DataTable FeeHeadDataTable = ds.FeeHead;
                //Balance B/d
                DataRow fhdt1 = FeeHeadDataTable.NewRow();
                fhdt1["PaymentDate"] = cashbookDate.ToString("dd-MM-yyyy");
                fhdt1["FeeHeadTitle"] = "To, Balance B/d";
                if (cashBooks.Any())
                    fhdt1["Amount"] = cashBooks.Sum(x => x.OpeningAmount).ToString("N");
                else
                    fhdt1["Amount"] = "0.00";
                FeeHeadDataTable.Rows.Add(fhdt1);
                FeeHeadTotalAmount += cashBooks.Sum(x => x.OpeningAmount);

                //Fee Heads
                FeeCollectionList.GroupBy(x => x.FeeHeadId).ToList().ForEach(x =>
                {
                    DataRow fhdt2 = FeeHeadDataTable.NewRow();
                    fhdt2["FeeHeadTitle"] = x.First().FeeHeadTitle.ToUpper();
                    fhdt2["Amount"] = x.Sum(y => y.Total ?? 0).ToString("N");
                    fhdt2["PaymentDate"] = cashbookDate.ToString("dd-MM-yyyy");
                    FeeHeadDataTable.Rows.Add(fhdt2);
                    FeeHeadTotalAmount += x.Sum(y => y.Total ?? 0);
                });
                var School = dataContext.Schools.First();

                DataTable UserCashbookDataTable = ds.UserCashbook;
                FeeCollectionList.Where(x => x.PaymentMode != (byte)PaymentMode.Online).GroupBy(x => new { x.CreatedBy, x.PaymentMode }).ToList().ForEach(x =>
                  {
                      string UserName = "";
                      var users = dataContext.SystemLogins.Where(y => y.LoginId == x.Key.CreatedBy);
                      if (users.Any())
                          UserName = users.First().UserName;
                      string PaymentModeString = Enum.GetName(typeof(PaymentMode), x.Key.PaymentMode);
                      decimal UserAmount = x.Sum(y => y.Total ?? 0);

                      var userCashbooks = cashBooks.Where(y => y.LoginId == x.Key.CreatedBy);
                      if (x.Key.PaymentMode == (byte)PaymentMode.Cash && userCashbooks.Any())
                      {
                          UserAmount = userCashbooks.First().DepositAmount;
                      }

                      string BankDetail = string.Empty;
                      if (x.Key.PaymentMode == (byte)PaymentMode.Cash)
                          BankDetail = School.CashBankDetail;
                      else if (x.Key.PaymentMode == (byte)PaymentMode.UPI)
                          BankDetail = School.POSBankDetail;
                      else
                          BankDetail = School.BankDetail;

                      DataRow ucdt1 = UserCashbookDataTable.NewRow();
                      ucdt1["CashbookDate"] = cashbookDate.ToString("dd-MM-yyyy");
                      ucdt1["Particulars"] = (BankDetail + " / " + UserName + " / " + PaymentModeString).ToUpper();
                      ucdt1["Amount"] = UserAmount.ToString("N");
                      UserCashbookDataTable.Rows.Add(ucdt1);

                      UserTotalAmount += UserAmount;
                  });

                //Balance C/d
                decimal ClosingAmount = 0;
                if (cashBooks.Any())
                    ClosingAmount = cashBooks.Sum(x => x.ClosingAmount);
                DataRow ucdt2 = UserCashbookDataTable.NewRow();
                ucdt2["CashbookDate"] = cashbookDate.ToString("dd-MM-yyyy");
                ucdt2["Particulars"] = "To, Balance C/d";
                ucdt2["Amount"] = ClosingAmount.ToString("N");
                UserCashbookDataTable.Rows.Add(ucdt2);
                UserTotalAmount += ClosingAmount;

                DataTable HeaderDataTable = ds.HeaderDataTable;
                DataRow HeaderDataRow = HeaderDataTable.NewRow();
                HeaderDataRow["FeeHeadTotalAmount"] = FeeHeadTotalAmount.ToString("N");
                HeaderDataRow["UserTotalAmount"] = UserTotalAmount.ToString("N");
                HeaderDataRow["CashbookDate"] = cashbookDate.ToString("dd-MM-yyyy");
                HeaderDataTable.Rows.Add(HeaderDataRow);

                DataTable SchoolDataTable = ds.School;
                DataRow SchoolDataRow = SchoolDataTable.NewRow();
                SchoolDataRow["SchoolName"] = School.FullName;
                SchoolDataRow["MobileNo"] = School.ContactNo;
                SchoolDataRow["Address"] = School.RegisteredOffice;
                SchoolDataRow["Logo"] = School.Logo.ToArray();
                SchoolDataTable.Rows.Add(SchoolDataRow);

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

        public ActionResult PrintChequebook(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/chequebook.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                DateTime chequebookDate = LoadData.CheckDate(id, "Invalid Chequebook Date");


                CashbookDataSet ds = new CashbookDataSet();

                //Fee Collection By AdmissionId
                var FeeCollectionList = (from fc1 in dataContext.FeeCollections
                                         join fcd in dataContext.FeeCollectionDetails
                                         on fc1.FeeCollectionId equals fcd.FeeCollectionId
                                         where fc1.PaymentDate.Date == chequebookDate.Date
                                         && fc1.FeeCollectionStatus != (byte)FeeCollectionStatus.Cancelled
                                         && fc1.PaymentMode == (byte)PaymentMode.Cheque
                                         select new { fcd.FeeHeadTitle, fcd.FeeHeadId, fc1.PaymentMode, fcd.Total, fc1.CreatedBy });

                var chequeBooks = dataContext.Chequebooks.Where(x => x.ChequebookDate.Date == chequebookDate.Date);
                decimal FeeHeadTotalAmount = 0, UserTotalAmount = 0;

                DataTable FeeHeadDataTable = ds.FeeHead;
                //Balance B/d
                DataRow fhdt1 = FeeHeadDataTable.NewRow();
                fhdt1["PaymentDate"] = chequebookDate.ToString("dd-MM-yyyy");
                fhdt1["FeeHeadTitle"] = "To, Balance B/d";
                if (chequeBooks.Any())
                    fhdt1["Amount"] = chequeBooks.Sum(x => x.OpeningAmount).ToString("N");
                else
                    fhdt1["Amount"] = "0.00";
                FeeHeadDataTable.Rows.Add(fhdt1);
                FeeHeadTotalAmount += chequeBooks.Sum(x => x.OpeningAmount);

                //Fee Heads
                FeeCollectionList.GroupBy(x => x.FeeHeadId).ToList().ForEach(x =>
                {
                    DataRow fhdt2 = FeeHeadDataTable.NewRow();
                    fhdt2["FeeHeadTitle"] = x.First().FeeHeadTitle.ToUpper();
                    fhdt2["Amount"] = x.Sum(y => y.Total ?? 0).ToString("N");
                    fhdt2["PaymentDate"] = chequebookDate.ToString("dd-MM-yyyy");
                    FeeHeadDataTable.Rows.Add(fhdt2);
                    FeeHeadTotalAmount += x.Sum(y => y.Total ?? 0);
                });
                var School = dataContext.Schools.First();

                DataTable UserCashbookDataTable = ds.UserCashbook;
                FeeCollectionList.Where(x => x.PaymentMode != (byte)PaymentMode.Online).GroupBy(x => new { x.CreatedBy, x.PaymentMode }).ToList().ForEach(x =>
                {
                    string UserName = "";
                    var users = dataContext.SystemLogins.Where(y => y.LoginId == x.Key.CreatedBy);
                    if (users.Any())
                        UserName = users.First().UserName;
                    string PaymentModeString = Enum.GetName(typeof(PaymentMode), x.Key.PaymentMode);
                    decimal UserAmount = x.Sum(y => y.Total ?? 0);

                    var userCashbooks = chequeBooks.Where(y => y.LoginId == x.Key.CreatedBy);
                    if (x.Key.PaymentMode == (byte)PaymentMode.Cash && userCashbooks.Any())
                    {
                        UserAmount = userCashbooks.First().DepositAmount;
                    }

                    string BankDetail = string.Empty;
                    if (x.Key.PaymentMode == (byte)PaymentMode.Cash)
                        BankDetail = School.CashBankDetail;
                    //else if (x.Key.PaymentMode == (byte)PaymentMode.POS)
                    //    BankDetail = School.POSBankDetail;
                    else
                        BankDetail = School.BankDetail;

                    DataRow ucdt1 = UserCashbookDataTable.NewRow();
                    ucdt1["CashbookDate"] = chequebookDate.ToString("dd-MM-yyyy");
                    ucdt1["Particulars"] = (BankDetail + " / " + UserName + " / " + PaymentModeString).ToUpper();
                    ucdt1["Amount"] = UserAmount.ToString("N");
                    UserCashbookDataTable.Rows.Add(ucdt1);

                    UserTotalAmount += UserAmount;
                });

                //Balance C/d
                decimal ClosingAmount = 0;
                if (chequeBooks.Any())
                    ClosingAmount = chequeBooks.Sum(x => x.ClosingAmount);
                DataRow ucdt2 = UserCashbookDataTable.NewRow();
                ucdt2["CashbookDate"] = chequebookDate.ToString("dd-MM-yyyy");
                ucdt2["Particulars"] = "To, Balance C/d";
                ucdt2["Amount"] = ClosingAmount.ToString("N");
                UserCashbookDataTable.Rows.Add(ucdt2);
                UserTotalAmount += ClosingAmount;

                DataTable HeaderDataTable = ds.HeaderDataTable;
                DataRow HeaderDataRow = HeaderDataTable.NewRow();
                HeaderDataRow["FeeHeadTotalAmount"] = FeeHeadTotalAmount.ToString("N");
                HeaderDataRow["UserTotalAmount"] = UserTotalAmount.ToString("N");
                HeaderDataRow["CashbookDate"] = chequebookDate.ToString("dd-MM-yyyy");
                HeaderDataTable.Rows.Add(HeaderDataRow);

                DataTable SchoolDataTable = ds.School;
                DataRow SchoolDataRow = SchoolDataTable.NewRow();
                SchoolDataRow["SchoolName"] = School.FullName;
                SchoolDataRow["MobileNo"] = School.ContactNo;
                SchoolDataRow["Address"] = School.RegisteredOffice;
                SchoolDataRow["Logo"] = School.Logo.ToArray();
                SchoolDataTable.Rows.Add(SchoolDataRow);

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
        public ActionResult PrintFeeDueReport(int SectionId, int MonthNo, int ReportNo, int AcademicSessionId, int UpToYear, int MonthDataNo)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                if (ReportNo == 1)
                    rp.Load(Server.MapPath("~/reports/due-fee-report.rpt"));
                else if (ReportNo == 3)
                    rp.Load(Server.MapPath("~/reports/due-fee-report-3.rpt"));
                else if (ReportNo == 2)
                    rp.Load(Server.MapPath("~/reports/due-fee-report-2.rpt"));
                else if (ReportNo == 4)
                    rp.Load(Server.MapPath("~/reports/due-fee-report-4.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                var dueList = (from a1 in dataContext.Admissions
                               join s1 in dataContext.Students
                               on a1.StudentId equals s1.StudentId
                               where a1.SectionId == SectionId
                               && a1.AcademicSessionId == AcademicSessionId
                               && s1.StudentStatus != (byte)StudentStatus.Left
                               //&& a1.AdmissionStatus == (byte)AdmissionStatus.Active
                               select new
                               {
                                   s1.AdmissionNo,
                                   s1.FullName,
                                   a1.RollNo,
                                   s1.MobileNo,
                                   DueFeeDetail = ReportNo == 3 ? FeeData.GetDueDetails2(dataContext, a1.AdmissionId, MonthNo, UpToYear, MonthDataNo) : FeeData.GetDueDetails(dataContext, a1.AdmissionId, MonthNo, ReportNo == 4 ? true : false, UpToYear, MonthDataNo)
                               }).ToList();
                dueList = dueList.Where(x => (ReportNo == 4 ? x.DueFeeDetail.TotalDues2 > 0 : x.DueFeeDetail.TotalDues > 0)).ToList();

                FeeDueReportDataSet ds = new FeeDueReportDataSet();

                DataTable DueListDataTable = ds.DueList;
                dueList.ForEach(admision =>
                {
                    DataRow dldr = DueListDataTable.NewRow();
                    dldr["FullName"] = admision.FullName.ToUpper();
                    dldr["AdmissionNo"] = admision.AdmissionNo;
                    dldr["RollNo"] = admision.RollNo;
                    dldr["MobileNo"] = admision.MobileNo;
                    dldr["NotPaidMonthNames"] = admision.DueFeeDetail.NotPaidMonthNames;
                    dldr["NotPaidSemesterNames"] = admision.DueFeeDetail.NotPaidSemesterNames;
                    dldr["TuitionFee"] = admision.DueFeeDetail.TuitionFee.ToString("N");
                    dldr["ClassFee"] = admision.DueFeeDetail.ClassFee.ToString("N");
                    dldr["TransportFee"] = admision.DueFeeDetail.TransportFee.ToString("N");
                    dldr["AnnualFee"] = admision.DueFeeDetail.AnnualFee.ToString("N");
                    dldr["ChargeAmount"] = admision.DueFeeDetail.ChargeAmount.ToString("N");
                    dldr["ArrearAmount"] = admision.DueFeeDetail.ArrearAmount.ToString("N");
                    dldr["HostelCharge"] = admision.DueFeeDetail.HostelCharge.ToString("N");
                    dldr["FineAmount"] = admision.DueFeeDetail.FineAmount.ToString("N");
                    dldr["PreviousDues"] = admision.DueFeeDetail.PreviousDues.ToString("N");
                    dldr["SchoolDiaryCharge"] = admision.DueFeeDetail.SchoolDiaryCharge.ToString("N");
                    dldr["TotalDues"] = (ReportNo == 3 || ReportNo == 4) ? admision.DueFeeDetail.TotalDues2.ToString("N") : admision.DueFeeDetail.TotalDues.ToString("N");
                    DueListDataTable.Rows.Add(dldr);
                });


                DataTable HeaderDataTable = ds.HeaderDataTable;
                DataRow HeaderDataRow = HeaderDataTable.NewRow();
                HeaderDataRow["ClassName"] = dataContext.Sections.Where(x => x.SectionId == SectionId).First().Class.ClassName;
                HeaderDataRow["SectionName"] = dataContext.Sections.Where(x => x.SectionId == SectionId).First().SectionName;
                HeaderDataRow["Semester"] = dataContext.Months.Any(x => x.MonthNo == MonthNo) ? dataContext.Months.First(x => x.MonthNo == MonthNo).MonthName : "";
                if (MonthDataNo > 0)
                    HeaderDataRow["MonthName"] = Enum.GetName(typeof(MonthNames), MonthDataNo);
                HeaderDataRow["Year"] = UpToYear;
                HeaderDataRow["TotalClassFee"] = dueList.Sum(x => x.DueFeeDetail.ClassFee).ToString("N");
                HeaderDataRow["TotalTransportFee"] = dueList.Sum(x => x.DueFeeDetail.TransportFee).ToString("N");
                HeaderDataRow["TotalChargeAmount"] = dueList.Sum(x => x.DueFeeDetail.ChargeAmount).ToString("N");
                HeaderDataRow["TotalArrearAmount"] = dueList.Sum(x => x.DueFeeDetail.ArrearAmount).ToString("N");
                HeaderDataRow["TotalHostalCharge"] = dueList.Sum(x => x.DueFeeDetail.HostelCharge).ToString("N");
                HeaderDataRow["TotalFineAmount"] = dueList.Sum(x => x.DueFeeDetail.FineAmount).ToString("N");
                HeaderDataRow["TotalPrevioueDues"] = dueList.Sum(x => x.DueFeeDetail.PreviousDues).ToString("N");
                HeaderDataRow["NetTotalDues"] = (ReportNo == 3 || ReportNo == 4) ? dueList.Sum(x => x.DueFeeDetail.TotalDues2).ToString("N") : dueList.Sum(x => x.DueFeeDetail.TotalDues).ToString("N");
                HeaderDataRow["TotalTuitionFee"] = dueList.Sum(x => x.DueFeeDetail.TuitionFee).ToString("N");
                HeaderDataTable.Rows.Add(HeaderDataRow);
                var School = dataContext.Schools.First();

                DataTable SchoolDataTable = ds.School;
                DataRow SchoolDataRow = SchoolDataTable.NewRow();
                SchoolDataRow["SchoolName"] = School.FullName;
                SchoolDataRow["MobileNo"] = School.ContactNo;
                SchoolDataRow["Address"] = School.RegisteredOffice;
                SchoolDataRow["Logo"] = School.Logo.ToArray();
                SchoolDataTable.Rows.Add(SchoolDataRow);

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

        public ActionResult PrintFeeDueReport4(int SectionId, int MonthNo, int ReportNo, int AcademicSessionId, int Year, int MonthDataNo)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/due-fee-report-4.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                var dueList = (from a1 in dataContext.Admissions
                               join s1 in dataContext.Students
                               on a1.StudentId equals s1.StudentId
                               where a1.AcademicSessionId == AcademicSessionId
                               && a1.SectionId == SectionId
                               && s1.StudentStatus == (byte)StudentStatus.Active
                               && a1.AdmissionStatus == (byte)AdmissionStatus.Active
                               select new
                               {
                                   s1.AdmissionNo,
                                   s1.FullName,
                                   a1.RollNo,
                                   s1.MobileNo,
                                   DueFeeDetail = FeeData.GetDueDetails(dataContext, a1.AdmissionId, MonthNo, true, Year, MonthDataNo)
                               }).ToList();
                dueList = dueList.Where(x => x.DueFeeDetail.TotalDues2 > 0).ToList();

                FeeDueReportDataSet ds = new FeeDueReportDataSet();

                DataTable DueListDataTable = ds.DueList;
                dueList.ForEach(admision =>
                {
                    DataRow dldr = DueListDataTable.NewRow();
                    dldr["FullName"] = admision.FullName.ToUpper();
                    dldr["AdmissionNo"] = admision.AdmissionNo;
                    dldr["RollNo"] = admision.RollNo;
                    dldr["MobileNo"] = admision.MobileNo;
                    dldr["NotPaidMonthNames"] = admision.DueFeeDetail.NotPaidMonthNames;
                    dldr["TuitionFee"] = admision.DueFeeDetail.TuitionFee.ToString("N");
                    dldr["ClassFee"] = admision.DueFeeDetail.ClassFee.ToString("N");
                    dldr["TransportFee"] = admision.DueFeeDetail.TransportFee.ToString("N");
                    dldr["AnnualFee"] = admision.DueFeeDetail.AnnualFee.ToString("N");
                    dldr["ChargeAmount"] = admision.DueFeeDetail.ChargeAmount.ToString("N");
                    dldr["ArrearAmount"] = admision.DueFeeDetail.ArrearAmount.ToString("N");
                    dldr["HostelCharge"] = admision.DueFeeDetail.HostelCharge.ToString("N");
                    dldr["FineAmount"] = admision.DueFeeDetail.FineAmount.ToString("N");
                    dldr["PreviousDues"] = admision.DueFeeDetail.PreviousDues.ToString("N");
                    dldr["SchoolDiaryCharge"] = admision.DueFeeDetail.SchoolDiaryCharge.ToString("N");
                    dldr["TotalDues"] = (ReportNo == 3 || ReportNo == 4) ? admision.DueFeeDetail.TotalDues2.ToString("N") : admision.DueFeeDetail.TotalDues.ToString("N");
                    DueListDataTable.Rows.Add(dldr);
                });


                DataTable HeaderDataTable = ds.HeaderDataTable;
                DataRow HeaderDataRow = HeaderDataTable.NewRow();
                HeaderDataRow["ClassName"] = dataContext.Sections.Where(x => x.SectionId == SectionId).First().Class.ClassName;
                HeaderDataRow["SectionName"] = dataContext.Sections.Where(x => x.SectionId == SectionId).First().SectionName;
                HeaderDataRow["MonthName"] = dataContext.Months.Where(x => x.MonthNo == MonthNo).First().MonthName;
                HeaderDataRow["TotalClassFee"] = dueList.Sum(x => x.DueFeeDetail.ClassFee).ToString("N");
                HeaderDataRow["TotalTransportFee"] = dueList.Sum(x => x.DueFeeDetail.TransportFee).ToString("N");
                HeaderDataRow["TotalChargeAmount"] = dueList.Sum(x => x.DueFeeDetail.ChargeAmount).ToString("N");
                HeaderDataRow["TotalArrearAmount"] = dueList.Sum(x => x.DueFeeDetail.ArrearAmount).ToString("N");
                HeaderDataRow["TotalHostalCharge"] = dueList.Sum(x => x.DueFeeDetail.HostelCharge).ToString("N");
                HeaderDataRow["TotalFineAmount"] = dueList.Sum(x => x.DueFeeDetail.FineAmount).ToString("N");
                HeaderDataRow["TotalPrevioueDues"] = dueList.Sum(x => x.DueFeeDetail.PreviousDues).ToString("N");
                HeaderDataRow["NetTotalDues"] = (ReportNo == 3 || ReportNo == 4) ? dueList.Sum(x => x.DueFeeDetail.TotalDues2).ToString("N") : dueList.Sum(x => x.DueFeeDetail.TotalDues).ToString("N");
                HeaderDataRow["TotalTuitionFee"] = dueList.Sum(x => x.DueFeeDetail.TuitionFee).ToString("N");
                HeaderDataTable.Rows.Add(HeaderDataRow);
                var School = dataContext.Schools.First();

                DataTable SchoolDataTable = ds.School;
                DataRow SchoolDataRow = SchoolDataTable.NewRow();
                SchoolDataRow["SchoolName"] = School.FullName;
                SchoolDataRow["MobileNo"] = School.ContactNo;
                SchoolDataRow["Address"] = School.RegisteredOffice;
                SchoolDataRow["Logo"] = School.Logo.ToArray();
                SchoolDataTable.Rows.Add(SchoolDataRow);

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

        public ActionResult PrintRegistrationReceipt(string id)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/registration-receipt.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);

                DataSet ds = new DataSet();
                var feeCollection = (from fc in dataContext.FeeCollections
                                     where fc.RegistrationId == Convert.ToInt32(id)
                                     select fc).First();

                DataTable schoolDataTable = new DataTable("School");
                SqlCommand cmd = new SqlCommand("select * from School where SchoolId=" + feeCollection.StudentRegistration.StudentType.SchoolId, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(schoolDataTable);
                ds.Tables.Add(schoolDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable StudentRegistrationDataTable = new DataTable("StudentRegistration");
                cmd = new SqlCommand("select * from StudentRegistration where StudentRegistrationId=" + feeCollection.RegistrationId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(StudentRegistrationDataTable);
                ds.Tables.Add(StudentRegistrationDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable academicSessionDataTable = new DataTable("AcademicSession");
                cmd = new SqlCommand("select * from AcademicSession where AcademicSessionId='" + feeCollection.AcademicSessionId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(academicSessionDataTable);
                ds.Tables.Add(academicSessionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable classDataTable = new DataTable("Class");
                cmd = new SqlCommand("select * from Class where ClassId='" + feeCollection.StudentRegistration.ClassId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(classDataTable);
                ds.Tables.Add(classDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();


                DataTable feeCollectionDataTable = new DataTable("FeeCollection");
                cmd = new SqlCommand("select * from FeeCollection where FeeCollectionId='" + feeCollection.FeeCollectionId + "'", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(feeCollectionDataTable);
                ds.Tables.Add(feeCollectionDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable feeCollectionDetailsDataTable = new DataTable("FeeCollectionDetails");
                cmd = new SqlCommand("select * from FeeCollectionDetails where FeeCollectionId='" + feeCollection.FeeCollectionId + "' order by FeeHeadTitle asc", connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(feeCollectionDetailsDataTable);
                ds.Tables.Add(feeCollectionDetailsDataTable);
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

        public ActionResult ExcelAdmissionList(int AcademicSessionId, int ClassId, int SectionId)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var students = (from s1 in dataContext.Students
                                join a1 in dataContext.Admissions
                                on s1.StudentId equals a1.StudentId
                                join sc1 in dataContext.Sections
                                on a1.SectionId equals sc1.SectionId
                                join c1 in dataContext.Classes
                                on sc1.ClassId equals c1.ClassId
                                where a1.AcademicSessionId == AcademicSessionId
                                && (ClassId > 0 ? c1.ClassId == ClassId : true)
                                && (SectionId > 0 ? a1.SectionId == SectionId : true)
                                && s1.StudentStatus != (byte)StudentStatus.Left
                                orderby s1.AdmissionNo descending
                                select new ExcelStudent
                                {
                                    AdmissionNo = s1.AdmissionNo,
                                    FullName = s1.FullName,
                                    DateOfBirth = s1.DateOfBirth.HasValue ? s1.DateOfBirth.Value.ToString() : "",
                                    Gender = Enum.GetName(typeof(Gender), s1.Gender),
                                    FatherName = s1.FatherName,
                                    MotherName = s1.MotherName,
                                    FatherOccupation = s1.FatherOccupation,
                                    SectionName = sc1.SectionName,
                                    ClassName = c1.ClassName,
                                    RollNo = a1.RollNo,
                                    FamilyAnnualIncome = s1.FamilyAnnualIncome,
                                    StudentStatus = Enum.GetName(typeof(StudentStatus), s1.StudentStatus),
                                    Category = s1.Category.HasValue ? s1.Category.Value.ToString() : "0",
                                    BloodGroup = s1.BloodGroup.HasValue ? s1.BloodGroup.Value.ToString() : "0",
                                    HouseName = s1.House.HouseName,
                                    CorrespondenceAddressLine1 = s1.CorrespondenceAddressLine1,
                                    CorrespondenceAddressLine2 = s1.CorrespondenceAddressLine2,
                                    CorrespondenceCity = s1.City1.CityName,
                                    CorrespondenceState = s1.City1.State.StateName,
                                    CorrespondecePinCode = s1.CorrespondecePinCode,
                                    PermanentAddressLine1 = s1.PermanentAddressLine1,
                                    PermanentAddressLine2 = s1.PermanentAddressLine2,
                                    PermanentCity = s1.City.CityName,
                                    PermanentState = s1.City.State.StateName,
                                    PermanentPinCode = s1.PermanentPinCode,
                                    MobileNo = s1.MobileNo,
                                    AlternateNo = s1.AlternateNo,
                                    Email = s1.Email,
                                    Photo = s1.Photo,
                                    AadhaarNo = s1.AadhaarNo,
                                    AdmissionDate = s1.RegistrationDate,
                                    PreviousSchoolName = s1.PreviousSchoolName,
                                    PreviousBoardName = s1.PreviousBoardName,
                                    LastClassAttend = s1.LastClassAttend,
                                    MediumOfInstruction = s1.MediumOfInstruction,
                                    PreviousSchoolTCNo = s1.PreviousSchoolTCNo,
                                    PreviousSchoolTCIssueDate = s1.PreviousSchoolTCIssueDate,
                                    JoinDate = s1.JoiningDate,
                                    Remarks = s1.Remarks,
                                }).ToList();

                students.ForEach(x =>
                {
                    x.DateOfBirth = (String.IsNullOrEmpty(x.DateOfBirth) ? "" : Convert.ToDateTime(x.DateOfBirth).ToString("dd/MM/yyyy"));
                    x.Category = (x.Category != "0" ? ConstantValue.Category[Convert.ToInt32(x.Category)] : "N/A");
                    x.BloodGroup = (x.BloodGroup != "0" ? ConstantValue.BloodGroup[Convert.ToInt32(x.BloodGroup)] : "N/A");
                });

                if (students.Count() <= 0)
                    throw new ArgumentException("No Records found");
                GridView gv = new GridView();
                gv.DataSource = students;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Student_List.xls"));
                Response.ContentType = "application/ms-excel";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.AllowPaging = false;
                gv.DataBind();
                gv.HeaderRow.Style.Add("background-color", "#FFFFFF");
                for (int i = 0; i < gv.HeaderRow.Cells.Count; i++)
                {
                    gv.HeaderRow.Cells[i].Style.Add("background-color", "#507CD1");
                    gv.HeaderRow.Cells[i].Style.Add("color", "#fff");

                }
                int j = 1;
                foreach (GridViewRow gvrow in gv.Rows)
                {
                    gvrow.BackColor = System.Drawing.Color.White;
                    if (j <= gv.Rows.Count)
                    {
                        if (j % 2 != 0)
                        {
                            for (int k = 0; k < gvrow.Cells.Count; k++)
                            {
                                gvrow.Cells[k].Style.Add("background-color", "#EFF3FB");
                            }
                        }
                    }
                    j++;
                }
                gv.RenderControl(htw);
                Response.Write(sw.ToString());
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }



        //public ActionResult CreateExcel2()
        //{
        //    using (MemoryStream mem = new MemoryStream())
        //    {
        //        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook);

        //        Add a WorkbookPart to the document.
        //        WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
        //        workbookpart.Workbook = new Workbook();

        //        Add a WorksheetPart to the WorkbookPart.
        //        WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
        //        worksheetPart.Worksheet = new Worksheet(new SheetData());

        //        Add Sheets to the Workbook.
        //       Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

        //        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

        //        Add a row to the cell table.
        //       Row row;
        //        row = new Row() { RowIndex = 1 };
        //        sheetData.Append(row);

        //        In the new row, find the column location to insert a cell in A1.
        //       Cell refCell = null;
        //        foreach (Cell cell in row.Elements<Cell>())
        //        {
        //            if (string.Compare(cell.CellReference.Value, "A1", true) > 0)
        //            {
        //                refCell = cell;
        //                break;
        //            }
        //        }

        //        Add the cell to the cell table at A1.
        //       Cell newCell = new Cell() { CellReference = "A1" };
        //        row.InsertBefore(newCell, refCell);

        //        Set the cell value to be a numeric value of 100.
        //       newCell.CellValue = new CellValue("100");
        //        newCell.DataType = new EnumValue<CellValues>(CellValues.Number);

        //        Append a new worksheet and associate it with the workbook.
        //        Sheet sheet = new Sheet()
        //        {
        //            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
        //            SheetId = 1,
        //            Name = "mySheet"
        //        };

        //        sheets.Append(sheet);

        //        workbookpart.Workbook.Save();
        //        spreadsheetDocument.Close();

        //        return File(mem.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "text.xlsx");
        //    }
        //    return View();
        //}
    }
}