using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using School;
using SchoolERP.Dataset;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
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
        public ActionResult CreateExcel()
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();

                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet ExamDetailSheet = Ep.Workbook.Worksheets.Add("ExamDetail");
                ExamDetailSheet.Cells["A1"].Value = "Examination Id";
                ExamDetailSheet.Cells["B1"].Value = "Examination Name";
                ExamDetailSheet.Cells["C1"].Value = "Exam Type";
                ExamDetailSheet.Cells["D1"].Value = "Max Marks";
                ExamDetailSheet.Cells["E1"].Value = "Is Practical";
                ExamDetailSheet.Cells["A1:XFD1"].Style.Font.Bold = true;
                //ExamDetailSheet.Cells["A1:XFD1"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                //ExamDetailSheet.Cells["A1:XFD1"].Style.Font.Color.SetColor(Color.White);
                ExamDetailSheet.Cells["A1:XFD1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                int row = 2;
                foreach (var item in dataContext.Examinations)
                {

                    ExamDetailSheet.Cells[string.Format("A{0}", row)].Value = item.ExaminationId;
                    ExamDetailSheet.Cells[string.Format("B{0}", row)].Value = item.ExaminationName;
                    ExamDetailSheet.Cells[string.Format("C{0}", row)].Value = item.ExamType;
                    ExamDetailSheet.Cells[string.Format("D{0}", row)].Value = item.MaxMarks;
                    ExamDetailSheet.Cells[string.Format("E{0}", row)].Value = item.IsPractical;
                    row++;
                }
                ExamDetailSheet.Cells.AutoFitColumns();


                ExcelWorksheet ClassSheet = Ep.Workbook.Worksheets.Add("Class Sheet");
                ClassSheet.Cells["A1"].Value = "Class Id";
                ClassSheet.Cells["B1"].Value = "Class Name";
                ClassSheet.Cells["C1"].Value = "Class No";
                ClassSheet.Cells["A1:XFD1"].Style.Font.Bold = true;
                row = 2;
                foreach (var item in dataContext.Classes)
                {

                    ClassSheet.Cells[string.Format("A{0}", row)].Value = item.ClassId;
                    ClassSheet.Cells[string.Format("B{0}", row)].Value = item.ClassName;
                    ClassSheet.Cells[string.Format("C{0}", row)].Value = item.ClassNo;
                    row++;
                }
                ClassSheet.Cells.AutoFitColumns();


                ClassSheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        public ActionResult ExcelMarksheetEntry(int AcademicSessionId, int SectionId, byte ExamTerm, int ExamClassSubjectId, int SubjectType2)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Class = dataContext.Sections.Where(x => x.SectionId == SectionId).First().Class;
                int ExamSubjectId = dataContext.ExamClassSubjects.Where(x => x.ExamClassSubjectId == ExamClassSubjectId).First().ExamSubjectId;
                var examinations = (from ec1 in dataContext.ExamClasses
                                    join e1 in dataContext.Examinations
                                    on ec1.ExaminationId equals e1.ExaminationId
                                    where ec1.ClassNo == Class.ClassNo
                                    && ec1.ExamTerm == ExamTerm
                                    && ec1.ExamClassSubjectId == ExamClassSubjectId
                                    orderby e1.ExamNo
                                    select new
                                    {
                                        ec1.ExamClassId,
                                        ec1.ExaminationId,
                                        e1.ExaminationName,
                                        ec1.ExamTerm,
                                        ec1.TheoryMaxMarks,
                                        ec1.PracticalMaxMarks,
                                        e1.ExamNo
                                    });

                var Admissionlist = (from a1 in dataContext.Admissions
                                     join s2 in dataContext.Students
                            on a1.StudentId equals s2.StudentId
                                     join s1 in dataContext.Sections
                                     on a1.SectionId equals s1.SectionId
                                     join m1 in dataContext.ExamMarksheets
                                     on a1.AdmissionId equals m1.AdmissionId into subMarksheet
                                     from sMarksheet in subMarksheet.DefaultIfEmpty()
                                     join ss1 in dataContext.ExamStudentSubjects
                                     on a1.AdmissionId equals ss1.AdmissionId into StudentSubjects
                                     where a1.AcademicSessionId == AcademicSessionId
                                     && a1.SectionId == SectionId
                                     && s2.StudentStatus == (byte)StudentStatus.Active
                                     orderby a1.RollNo
                                     select new ExcelMarksheetModel
                                     {
                                         AdmissionId = a1.AdmissionId,
                                         AdmissionNo = s2.AdmissionNo,
                                         RollNo = a1.RollNo,
                                         StudentName = s2.FullName,
                                         ExamMarksList = (from e1 in examinations
                                                          join m1 in dataContext.ExamMarksheetDetails.Where(x => x.ExamMarksheetId == sMarksheet.ExamMarksheetId
                                                                                                                 && x.ExamClassSubjectId == ExamClassSubjectId
                                                                                                                 && x.ExamTerm == ExamTerm)
                                                          on e1.ExamClassId equals m1.ExamClassId into subMarksheetDetail
                                                          from sMarksheetDetail in subMarksheetDetail.DefaultIfEmpty()
                                                          orderby e1.ExamNo
                                                          select new ExcelExamMarksModel
                                                          {
                                                              TheoryMarksObtained = sMarksheetDetail != null ? sMarksheetDetail.TheoryMarksObtained : 0,
                                                              PracticalMarksObtained = sMarksheetDetail != null ? sMarksheetDetail.PracticalMarksObtained : 0,
                                                          }).ToList(),
                                         //IsAvailable = (from sj1 in StudentSubjects
                                         //               where (sj1.SubjectOne == ExamSubjectId
                                         //               || sj1.SubjectTwo == ExamSubjectId
                                         //               || sj1.SubjectThree == ExamSubjectId
                                         //               || sj1.SubjectFive == ExamSubjectId
                                         //               || sj1.SubjectSix == ExamSubjectId
                                         //               || sj1.SubjectSeven == ExamSubjectId
                                         //               || sj1.SubjectEight == ExamSubjectId)
                                         //               select sj1).Any(),

                                     }).ToList();
                //if (Class.ClassNo > ConstantData.SubjectChangeClass)
                //{
                Admissionlist.ForEach(x => x.IsAvailable = LoadExamData.GetStudentSubjectuIds(dataContext, x.AdmissionId).Contains(ExamSubjectId));

                Admissionlist = Admissionlist.Where(x => x.IsAvailable).ToList();
                //}
                if (Admissionlist.Count() == 0)
                    throw new Exception("No Data Found.");
                ExcelPackage Ep = new ExcelPackage();

                //Marks Detail
                ExcelWorksheet MaksheetSheet = Ep.Workbook.Worksheets.Add(ExcelConstantData.StudentMarksDetail);
                MaksheetSheet.Cells[1, 1].Value = "Admission  No";
                MaksheetSheet.Cells[1, 2].Value = "StudentName";
                MaksheetSheet.Cells[1, 3].Value = "Roll No";
                int column = 4;
                foreach (var exam in examinations)
                {
                    if (SubjectType2 == (byte)School.SubjectType2.Practical)
                        MaksheetSheet.Cells[1, column].Value = exam.ExaminationName + (exam.PracticalMaxMarks.HasValue ? (" [MM:" + exam.PracticalMaxMarks.Value.ToString("#,0") + "]") : "");
                    else
                        MaksheetSheet.Cells[1, column].Value = exam.ExaminationName + (exam.TheoryMaxMarks.HasValue ? (" [MM:" + exam.TheoryMaxMarks.Value.ToString("#,0") + "]") : "");
                    column++;
                }
                MaksheetSheet.Cells["A1:XFD1"].Style.Font.Bold = true;

                int row = 2;
                foreach (var admission in Admissionlist)
                {
                    MaksheetSheet.Cells[row, 1].Value = admission.AdmissionNo;
                    MaksheetSheet.Cells[row, 2].Value = admission.StudentName;
                    MaksheetSheet.Cells[row, 3].Value = admission.RollNo;
                    column = 4;
                    foreach (var exam in admission.ExamMarksList)
                    {
                        if (SubjectType2 == (byte)School.SubjectType2.Practical)
                            MaksheetSheet.Cells[row, column].Value = exam.PracticalMarksObtained;
                        else
                            MaksheetSheet.Cells[row, column].Value = exam.TheoryMarksObtained;
                        MaksheetSheet.Cells[row, column].Style.Locked = false;
                        column++;
                    }
                    row++;
                }
                while (column > 3)
                {
                    column--;
                    MaksheetSheet.Cells[1, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    MaksheetSheet.Cells[1, column].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    MaksheetSheet.Cells[1, column].Style.Font.Color.SetColor(Color.White);
                }
                MaksheetSheet.Cells.AutoFitColumns();

                //Header Color
                MaksheetSheet.Cells["A1:C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                MaksheetSheet.Cells["A1:C1"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                MaksheetSheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.White);

                //Border
                MaksheetSheet.Cells[MaksheetSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                MaksheetSheet.Cells[MaksheetSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                MaksheetSheet.Cells[MaksheetSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                MaksheetSheet.Cells[MaksheetSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                MaksheetSheet.Protection.IsProtected = true;

                //Section Detail
                string SessionName = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First().SessionName,
                    SectionName = dataContext.Sections.Where(x => x.SectionId == SectionId).First().SectionName,
                    SubjectName = dataContext.ExamSubjects.Where(x => x.ExamSubjectId == ExamSubjectId).First().SubjectName,
                    SubjectType2Name = Enum.GetName(typeof(SubjectType2), SubjectType2),
                    ExamTermName = Enum.GetName(typeof(ExamTerm), ExamTerm);

                ExcelWorksheet ClassDetailSheet = Ep.Workbook.Worksheets.Add(ExcelConstantData.ClassDetail);
                ClassDetailSheet.Cells[1, 1].Value = AcademicSessionId;
                ClassDetailSheet.Cells[2, 1].Value = 0;
                ClassDetailSheet.Cells[3, 1].Value = SectionId;
                ClassDetailSheet.Cells[4, 1].Value = ExamTerm;
                ClassDetailSheet.Cells[5, 1].Value = ExamClassSubjectId;
                ClassDetailSheet.Cells[6, 1].Value = SubjectType2;

                ClassDetailSheet.Cells[1, 2].Value = "Session Name";
                ClassDetailSheet.Cells[2, 2].Value = "Class";
                ClassDetailSheet.Cells[3, 2].Value = "Section";
                ClassDetailSheet.Cells[4, 2].Value = "Term";
                ClassDetailSheet.Cells[5, 2].Value = "Subject";
                ClassDetailSheet.Cells[6, 2].Value = "Subject Type";

                ClassDetailSheet.Cells[1, 3].Value = SessionName;
                ClassDetailSheet.Cells[2, 3].Value = Class.ClassName;
                ClassDetailSheet.Cells[3, 3].Value = SectionName;
                ClassDetailSheet.Cells[4, 3].Value = ExamTermName;
                ClassDetailSheet.Cells[5, 3].Value = SubjectName;
                ClassDetailSheet.Cells[6, 3].Value = SubjectType2Name;

                ClassDetailSheet.Cells.AutoFitColumns();
                ClassDetailSheet.Cells.Style.Font.Bold = true;
                ClassDetailSheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //Border
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                ClassDetailSheet.Cells["A1:C6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ClassDetailSheet.Cells["A1:C6"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                ClassDetailSheet.Cells["A1:C6"].Style.Font.Color.SetColor(Color.White);

                ClassDetailSheet.Protection.IsProtected = true;

                //Other Datas
                ExcelWorksheet examSheet = Ep.Workbook.Worksheets.Add(ExcelConstantData.ExamClassIdSheet);
                examSheet.Cells[1, 1].Value = "Data1";
                row = 2;
                foreach (var exam in examinations)
                {
                    examSheet.Cells[row, 1].Value = exam.ExamClassId;
                    row++;
                }


                examSheet.Cells.AutoFitColumns();
                examSheet.Protection.IsProtected = true;

                string fileName = "MarksEntry_" + SessionName + "_" + Class.ClassName + "_" + SectionName + "_" + SubjectName + "_" + ExamTermName + "_" + SubjectType2Name;

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment: filename=" + "MarksEntry.xls");
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        public ActionResult ExcelForGradeMarks(int AcademicSessionId, int ClassNo, int SectionId, byte ExamTerm, int ExamClassSubjectId)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                Class Class = null;
                if (SectionId == 0)
                    Class = dataContext.Classes.Where(x => x.ClassNo == ClassNo).First();
                else
                    Class = dataContext.Sections.Where(x => x.SectionId == SectionId).First().Class;
                int ExamSubjectId = dataContext.ExamClassSubjects.Where(x => x.ExamClassSubjectId == ExamClassSubjectId).First().ExamSubjectId;
                var Admissionlist = (from a1 in dataContext.Admissions
                                     join s2 in dataContext.Students
                                     on a1.StudentId equals s2.StudentId
                                     join s1 in dataContext.Sections
                                     on a1.SectionId equals s1.SectionId
                                     join c1 in dataContext.Classes
                                     on s1.ClassId equals c1.ClassId
                                     join m1 in dataContext.ExamMarksheets
                                     on a1.AdmissionId equals m1.AdmissionId into subMarksheet
                                     from sMarksheet in subMarksheet.DefaultIfEmpty()
                                     where a1.AcademicSessionId == AcademicSessionId
                                     && (SectionId == 0 ? true : a1.SectionId == SectionId)
                                     && c1.ClassNo == ClassNo
                                     && s2.StudentStatus == (byte)StudentStatus.Active
                                     orderby a1.RollNo
                                     select new
                                     {
                                         s2.AdmissionNo,
                                         a1.RollNo,
                                         StudentName = s2.FullName,
                                         ExamMarksheetGradeList = (from cj in dataContext.ExamClassSubjects
                                                                   join m1 in dataContext.ExamMarksheetGrades.Where(x => x.ExamMarksheetId == sMarksheet.ExamMarksheetId && x.ExamTerm == ExamTerm)
                                                                   on cj.ExamClassSubjectId equals m1.ExamClassSubjectId into subMarksheetDetail
                                                                   from sMarksheetDetail in subMarksheetDetail.DefaultIfEmpty()
                                                                   join g1 in dataContext.ExamGradingScaleDetails
                                                                   on sMarksheetDetail.ExamGradingScaleDetailId equals g1.ExamGradingScaleDetailId into subGrade
                                                                   from sGrade in subGrade.DefaultIfEmpty()
                                                                   where cj.ClassNo == c1.ClassNo
                                                                   && cj.ExamSubjectType == (byte)ExamSubjectType.Grade
                                                                   && cj.ExamClassSubjectId == ExamClassSubjectId
                                                                   select sGrade).ToList()
                                     }).ToList();
                if (Admissionlist.Count() == 0)
                    throw new Exception("No Data Found.");

                string GradingTitle = (from g1 in dataContext.ExamGradingScales
                                       join g2 in dataContext.ExamClassGrades
                                       on g1.ExamGradingScaleId equals g2.ExamGradingScaleId
                                       where g2.AcademicSessionId == AcademicSessionId
                                       && g2.ClassNo == Class.ClassNo
                                       && g2.ExamClassSubjectId == ExamClassSubjectId
                                       && g2.ExamTerm == ExamTerm
                                       select g1.GradingTitle).First();
                ExcelPackage Ep = new ExcelPackage();

                //Grade Marks Detail
                ExcelWorksheet MaksheetSheet = Ep.Workbook.Worksheets.Add(ExcelConstantData.StudentMarksDetail);
                MaksheetSheet.Cells[1, 1].Value = "Admission  No";
                MaksheetSheet.Cells[1, 2].Value = "StudentName";
                MaksheetSheet.Cells[1, 3].Value = "Roll No";
                MaksheetSheet.Cells[1, 4].Value = GradingTitle;
                MaksheetSheet.Cells["A1:XFD1"].Style.Font.Bold = true;

                int row = 2;
                foreach (var admission in Admissionlist)
                {
                    MaksheetSheet.Cells[row, 1].Value = admission.AdmissionNo;
                    MaksheetSheet.Cells[row, 2].Value = admission.StudentName;
                    MaksheetSheet.Cells[row, 3].Value = admission.RollNo;
                    if (admission.ExamMarksheetGradeList.Any())
                        MaksheetSheet.Cells[row, 4].Value = admission.ExamMarksheetGradeList.First() != null ? admission.ExamMarksheetGradeList.First().GradingScale : null;
                    MaksheetSheet.Cells[row, 4].Style.Locked = false;
                    row++;
                }
                MaksheetSheet.Cells.AutoFitColumns();

                //Header Color
                MaksheetSheet.Cells["A1:D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                MaksheetSheet.Cells["A1:D1"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                MaksheetSheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.White);

                //Border
                MaksheetSheet.Cells[MaksheetSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                MaksheetSheet.Cells[MaksheetSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                MaksheetSheet.Cells[MaksheetSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                MaksheetSheet.Cells[MaksheetSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                MaksheetSheet.Protection.IsProtected = true;

                // Section Detail
                string SessionName = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First().SessionName,
                    SectionName = SectionId == 0 ? "All Sections" : dataContext.Sections.Where(x => x.SectionId == SectionId).First().SectionName,
                    SubjectName = dataContext.ExamSubjects.Where(x => x.ExamSubjectId == ExamSubjectId).First().SubjectName,
                    ExamTermName = Enum.GetName(typeof(ExamTerm), ExamTerm);

                ExcelWorksheet ClassDetailSheet = Ep.Workbook.Worksheets.Add(ExcelConstantData.ClassDetail);
                ClassDetailSheet.Cells[1, 1].Value = AcademicSessionId;
                ClassDetailSheet.Cells[2, 1].Value = ClassNo;
                ClassDetailSheet.Cells[3, 1].Value = SectionId;
                ClassDetailSheet.Cells[4, 1].Value = ExamTerm;
                ClassDetailSheet.Cells[5, 1].Value = ExamClassSubjectId;

                ClassDetailSheet.Cells[1, 2].Value = "Session Name";
                ClassDetailSheet.Cells[2, 2].Value = "Class";
                ClassDetailSheet.Cells[3, 2].Value = "Section";
                ClassDetailSheet.Cells[4, 2].Value = "Term";
                ClassDetailSheet.Cells[5, 2].Value = "Subject";

                ClassDetailSheet.Cells[1, 3].Value = SessionName;
                ClassDetailSheet.Cells[2, 3].Value = Class.ClassName;
                ClassDetailSheet.Cells[3, 3].Value = SectionName;
                ClassDetailSheet.Cells[4, 3].Value = ExamTermName;
                ClassDetailSheet.Cells[5, 3].Value = SubjectName;

                ClassDetailSheet.Cells.AutoFitColumns();
                ClassDetailSheet.Cells.Style.Font.Bold = true;
                ClassDetailSheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //Border
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                ClassDetailSheet.Cells["A1:C5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ClassDetailSheet.Cells["A1:C5"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                ClassDetailSheet.Cells["A1:C5"].Style.Font.Color.SetColor(Color.White);

                ClassDetailSheet.Protection.IsProtected = true;

                string fileName = "GradeMarksEntry_" + SessionName + "_" + Class.ClassName + "_" + SectionName + "_" + SubjectName + "_" + ExamTermName;
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        public ActionResult ExcelForAttencenceEntry(int AcademicSessionId, int SectionId)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var Class = dataContext.Sections.Where(x => x.SectionId == SectionId).First().Class;
                var Admissionlist = (from a1 in dataContext.Admissions
                                     join s1 in dataContext.Students
                                     on a1.StudentId equals s1.StudentId
                                     join sec in dataContext.Sections
                                     on a1.SectionId equals sec.SectionId
                                     join cl in dataContext.Classes
                                     on sec.ClassId equals cl.ClassId
                                     join m1 in dataContext.ExamMarksheets
                                     on a1.AdmissionId equals m1.AdmissionId into subMarksheet
                                     from sMarksheet in subMarksheet.DefaultIfEmpty()
                                     where a1.AcademicSessionId == AcademicSessionId
                                     && a1.SectionId == SectionId
                                      && s1.StudentStatus == (byte)StudentStatus.Active
                                     orderby a1.RollNo
                                     select new ExamMarksheetModel
                                     {
                                         AdmissionNo = s1.AdmissionNo,
                                         StudentName = s1.FullName,
                                         RollNo = a1.RollNo,
                                         HYPresentDays = sMarksheet.HYPresentDays,
                                         ANPresentDays = sMarksheet.ANPresentDays,
                                     }).ToList();

                if (Admissionlist.Count() == 0)
                    throw new Exception("No Data Found.");

                var sectionAttendence = (from e1 in dataContext.ExamSectionAttendences
                                         where e1.AcademicSessionId == AcademicSessionId
                                         && e1.SectionId == SectionId
                                         select e1).First();
                ExcelPackage Ep = new ExcelPackage();
                //Attendence Sheet
                ExcelWorksheet attencenceSheet = Ep.Workbook.Worksheets.Add(ExcelConstantData.AttendeceSheet);
                attencenceSheet.Cells[1, 1].Value = "Admission  No";
                attencenceSheet.Cells[1, 2].Value = "StudentName";
                attencenceSheet.Cells[1, 3].Value = "Roll No";
                attencenceSheet.Cells[1, 4].Value = "Half Yearly Present Days [WD:" + sectionAttendence.HYWorkingDays + " ]";
                attencenceSheet.Cells[1, 5].Value = "Annual Present Days [WD:" + sectionAttendence.ANWorkingDays + " ]";
                attencenceSheet.Cells["A1:XFD1"].Style.Font.Bold = true;

                int row = 2;
                foreach (var admission in Admissionlist)
                {
                    attencenceSheet.Cells[row, 1].Value = admission.AdmissionNo;
                    attencenceSheet.Cells[row, 2].Value = admission.StudentName;
                    attencenceSheet.Cells[row, 3].Value = admission.RollNo;
                    attencenceSheet.Cells[row, 4].Value = admission.HYPresentDays;
                    attencenceSheet.Cells[row, 4].Style.Locked = false;

                    attencenceSheet.Cells[row, 5].Value = admission.ANPresentDays;
                    attencenceSheet.Cells[row, 5].Style.Locked = false;
                    row++;
                }
                attencenceSheet.Cells.AutoFitColumns();

                //Header Color
                attencenceSheet.Cells["A1:E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                attencenceSheet.Cells["A1:E1"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                attencenceSheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.White);

                //Border
                attencenceSheet.Cells[attencenceSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                attencenceSheet.Cells[attencenceSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                attencenceSheet.Cells[attencenceSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                attencenceSheet.Cells[attencenceSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                attencenceSheet.Protection.IsProtected = true;

                // Section Detail
                string SessionName = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First().SessionName,
                    SectionName = dataContext.Sections.Where(x => x.SectionId == SectionId).First().SectionName;

                ExcelWorksheet ClassDetailSheet = Ep.Workbook.Worksheets.Add(ExcelConstantData.ClassDetail);
                ClassDetailSheet.Cells[1, 1].Value = AcademicSessionId;
                ClassDetailSheet.Cells[2, 1].Value = 0;
                ClassDetailSheet.Cells[3, 1].Value = SectionId;

                ClassDetailSheet.Cells[1, 2].Value = "Session Name";
                ClassDetailSheet.Cells[2, 2].Value = "Class";
                ClassDetailSheet.Cells[3, 2].Value = "Section";
                ClassDetailSheet.Cells[4, 2].Value = "Half Yearly Working Days";
                ClassDetailSheet.Cells[5, 2].Value = "Annual Working Days";

                ClassDetailSheet.Cells[1, 3].Value = SessionName;
                ClassDetailSheet.Cells[2, 3].Value = Class.ClassName;
                ClassDetailSheet.Cells[3, 3].Value = SectionName;

                ClassDetailSheet.Cells[4, 3].Value = sectionAttendence.HYWorkingDays;

                ClassDetailSheet.Cells[5, 3].Value = sectionAttendence.ANWorkingDays;

                ClassDetailSheet.Cells.AutoFitColumns();
                ClassDetailSheet.Protection.IsProtected = true;
                ClassDetailSheet.Cells.Style.Font.Bold = true;
                ClassDetailSheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //Border
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ClassDetailSheet.Cells[ClassDetailSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                ClassDetailSheet.Cells["A1:C5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ClassDetailSheet.Cells["A1:C5"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                ClassDetailSheet.Cells["A1:C5"].Style.Font.Color.SetColor(Color.White);



                string fileName = "Attendence_Entry_" + SessionName + "_" + Class.ClassName + "_" + SectionName;
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
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

        public ActionResult PrintMarksheetHY(int AcademicSessionId, int SectionId, int AdmissionId)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                int ClassNo = 0;
                if (AdmissionId > 0)
                {
                    ClassNo = (from a1 in dataContext.Admissions
                               join s1 in dataContext.Sections
                               on a1.SectionId equals s1.SectionId
                               join c1 in dataContext.Classes
                               on s1.ClassId equals c1.ClassId
                               where a1.AdmissionId == AdmissionId
                               select c1.ClassNo).First();
                }
                else
                    ClassNo = dataContext.Sections.Where(x => x.SectionId == SectionId).First().Class.ClassNo;

                DataSet ds = new DataSet();
                bool IsSubjectGradeAllow = false;
                if (ClassNo < 9)
                {
                    var marksheetData = Marksheet.MarksheetUptoEightHY(AcademicSessionId, SectionId, AdmissionId);
                    ds = marksheetData.MarksheetDataSet;
                    IsSubjectGradeAllow = marksheetData.IsSubjectGradeAllow;
                }
                else if (ClassNo == 9 || ClassNo == 10)
                    ds = Marksheet.MarksheetNineAndTenHY(AcademicSessionId, SectionId, AdmissionId);
                else if (ClassNo == 11)
                    ds = Marksheet.MarksheetEleven(AcademicSessionId, SectionId, AdmissionId);
                else if (ClassNo == 12)
                    ds = Marksheet.MarksheetTwelve(AcademicSessionId, SectionId, AdmissionId);

                ReportDocument rp = new ReportDocument();
                if (ClassNo < 9)
                {
                    if (ClassNo > 5)
                        rp.Load(Server.MapPath("~/reports/Marksheet_HY_UoToEight.rpt"));
                    else if (IsSubjectGradeAllow)
                        rp.Load(Server.MapPath("~/reports/Marksheet_HY_UoToFive_2.rpt"));
                    else
                        rp.Load(Server.MapPath("~/reports/Marksheet_HY_UoToFive.rpt"));
                }
                else if (ClassNo == 9)
                    rp.Load(Server.MapPath("~/reports/Marksheet_HY_NineAndTen.rpt"));
                else if (ClassNo == 10)
                    rp.Load(Server.MapPath("~/reports/Marksheet_HY_Ten.rpt"));
                else if (ClassNo == 11)
                    rp.Load(Server.MapPath("~/reports/marksheet-eleven-HY.rpt"));
                else if (ClassNo == 12)
                    rp.Load(Server.MapPath("~/reports/marksheet-twelve-HY.rpt"));

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

        public ActionResult PrintMarksheetUptoEightAN(int AcademicSessionId, int SectionId, int AdmissionId)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/Marksheet_AN_UoToEight.rpt"));
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                int ClassNo = 0;
                if (AdmissionId > 0)
                {
                    var admission = (from a1 in dataContext.Admissions
                                     join s1 in dataContext.Sections
                                     on a1.SectionId equals s1.SectionId
                                     join c1 in dataContext.Classes
                                     on s1.ClassId equals c1.ClassId
                                     where a1.AdmissionId == AdmissionId
                                     select new { a1.SectionId, a1.AcademicSessionId, c1.ClassNo }).First();
                    ClassNo = admission.ClassNo;
                    SectionId = admission.SectionId.Value;
                    AcademicSessionId = admission.AcademicSessionId.Value;
                }
                else
                    ClassNo = dataContext.Sections.Where(x => x.SectionId == SectionId).First().Class.ClassNo;

                MarksheetUpToEight ds = new MarksheetUpToEight();
                var marksheetList = (from m1 in dataContext.ExamMarksheets
                                     join m2 in dataContext.ExamMarksUptoEights
                                  on m1.ExamMarksheetId equals m2.ExamMarksheetId into subExams
                                     join a1 in dataContext.Admissions
                                     on m1.AdmissionId equals a1.AdmissionId
                                     join s1 in dataContext.Sections
                                     on a1.SectionId equals s1.SectionId
                                     join c1 in dataContext.Classes
                                     on s1.ClassId equals c1.ClassId
                                     join st1 in dataContext.Students
                                     on a1.StudentId equals st1.StudentId
                                     join ct1 in dataContext.Cities
                                     on st1.PermanentCityId equals ct1.CityId into subCity
                                     from sCity in subCity.DefaultIfEmpty()
                                     join st2 in dataContext.States
                                     on sCity.StateId equals st2.StateId into subState
                                     from sState in subState.DefaultIfEmpty()
                                     join s2 in dataContext.AcademicSessions
                                     on a1.AcademicSessionId equals s2.AcademicSessionId
                                     where a1.AcademicSessionId == AcademicSessionId
                                     && a1.SectionId == SectionId
                                     && (AdmissionId > 0 ? a1.AdmissionId == AdmissionId : true)
                                     && subExams.Any()
                                     select new
                                     {
                                         st1.AdmissionNo,
                                         a1.RollNo,
                                         st1.FullName,
                                         m1.ExamMarksheetId,
                                         s1.SectionName,
                                         c1.ClassName,
                                         c1.ClassId,
                                         s2.SessionName,
                                         st1.DateOfBirth,
                                         st1.FatherName,
                                         st1.MotherName,
                                         st1.House.HouseName,
                                         m1.HYPresentDays,
                                         m1.ANRank,
                                         a1.AcademicSessionId,
                                         a1.SectionId,
                                         m1.ANPresentDays,
                                         m1.ANMarksObtained,
                                         m1.FinalMarksObtained,
                                         Address = st1.CorrespondenceAddressLine1,
                                         sCity.CityName,
                                         c1.ClassNo,
                                         MarksheetGradeList = (from e1 in dataContext.ExamMarksheetGrades
                                                               join cs1 in dataContext.ExamClassSubjects
                                                               on e1.ExamClassSubjectId equals cs1.ExamClassSubjectId
                                                               join s1 in dataContext.ExamSubjects
                                                               on cs1.ExamSubjectId equals s1.ExamSubjectId
                                                               where e1.ExamMarksheetId == m1.ExamMarksheetId
                                                               orderby s1.SubjectName
                                                               select new
                                                               {
                                                                   s1.SubjectName,
                                                                   Grade = e1.ExamGradingScaleDetail.GradingScale,
                                                                   cs1.GradeType,
                                                                   e1.ExamGradingScaleDetail.ExamGradingScale.GradingTitle,
                                                                   e1.ExamTerm
                                                               }).ToList(),
                                     }).ToList();

                var sectionAttendence = dataContext.ExamSectionAttendences.Where(x => x.AcademicSessionId == AcademicSessionId && SectionId == x.SectionId).First();

                marksheetList.ToList().ForEach(marksheet =>
                {
                    decimal TotalMarks = (from m1 in dataContext.ExamClasses
                                          where m1.AcademicSessionId == marksheet.AcademicSessionId
                                          && m1.ClassNo == marksheet.ClassId
                                          group m1 by m1.ExamClassSubjectId into SubjectList
                                          select SubjectList).Count() * 100;
                    decimal FinalMarkOntained = marksheet.FinalMarksObtained.HasValue ? marksheet.FinalMarksObtained.Value : 0;
                    DataTable MarksheetDataTable = ds.Marksheet;
                    DataRow MarksheetDataRow = MarksheetDataTable.NewRow();
                    MarksheetDataRow["StudentName"] = marksheet.FullName;
                    MarksheetDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                    MarksheetDataRow["AdmissionNo"] = marksheet.AdmissionNo;
                    MarksheetDataRow["SessionName"] = marksheet.SessionName;
                    MarksheetDataRow["FatherName"] = marksheet.FatherName;
                    MarksheetDataRow["MotherName"] = marksheet.MotherName;
                    MarksheetDataRow["Address"] = marksheet.Address + " " + marksheet.CityName;
                    MarksheetDataRow["ClassName"] = marksheet.ClassName;
                    MarksheetDataRow["SectionName"] = marksheet.SectionName;
                    MarksheetDataRow["RollNo"] = marksheet.RollNo;
                    MarksheetDataRow["ANRank"] = marksheet.ANRank;
                    MarksheetDataRow["HYWorkingDays"] = sectionAttendence.HYWorkingDays;
                    MarksheetDataRow["HYPresentDays"] = marksheet.HYPresentDays;
                    MarksheetDataRow["ANWorkingDays"] = sectionAttendence.ANWorkingDays;
                    MarksheetDataRow["ANPresentDays"] = marksheet.ANPresentDays;
                    MarksheetDataRow["ANAggregateMarks"] = FinalMarkOntained.ToString("#,0.0") + "/" + TotalMarks.ToString("#,0");
                    if (TotalMarks > 0)
                        MarksheetDataRow["ANPercentage"] = (FinalMarkOntained / TotalMarks * 100).ToString("#,0.0") + " %";
                    MarksheetDataRow["DateOfBirth"] = marksheet.DateOfBirth.HasValue ? marksheet.DateOfBirth.Value.ToString("dd-MM-yyyy") : "";
                    MarksheetDataTable.Rows.Add(MarksheetDataRow);

                    //Grade Subjects
                    marksheet.MarksheetGradeList.GroupBy(x => new { x.GradingTitle, x.GradeType }).OrderByDescending(x => x.Key.GradeType).ToList().ForEach(gradingList =>
                    {
                        string GradingTitle = string.Empty;
                        if (gradingList.Key.GradeType == (byte)GradeType.None)
                            GradingTitle = "Subject " + " [ on a " + gradingList.Key.GradingTitle + " ]";
                        else
                            GradingTitle = System.Text.RegularExpressions.Regex.Replace(Enum.GetName(typeof(GradeType), gradingList.Key.GradeType), "[A-Z]", " $0").Replace("_", " -") + " [ on a " + gradingList.Key.GradingTitle + " ]";
                        DataTable GradeGroupDataTable = ds.GradeGroupDataTable;
                        DataRow ggdDataRow = GradeGroupDataTable.NewRow();
                        ggdDataRow["GradeTitle"] = GradingTitle;
                        ggdDataRow["GradeType"] = gradingList.Key.GradeType;
                        ggdDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                        GradeGroupDataTable.Rows.Add(ggdDataRow);

                        gradingList.GroupBy(x => x.SubjectName).ToList().ForEach(grade =>
                        {
                            DataTable GradeDataTable = ds.GradeDataTable;
                            DataRow GradeDataRow = GradeDataTable.NewRow();
                            GradeDataRow["Subject"] = grade.Key;
                            GradeDataRow["GradeType"] = gradingList.Key.GradeType;
                            GradeDataRow["GradeTitle"] = GradingTitle;
                            GradeDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                            var HY_Grade = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne);
                            if (HY_Grade.Any())
                                GradeDataRow["HY_Grade"] = HY_Grade.First().Grade;
                            var AN_Grade = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermTwo);
                            if (AN_Grade.Any())
                                GradeDataRow["AN_Grade"] = AN_Grade.First().Grade;
                            GradeDataTable.Rows.Add(GradeDataRow);
                        });
                    });
                });

                DataTable MarksDataTable = ds.MarksDataTable;
                SqlCommand cmd = new SqlCommand("ExamMarksUpToEightList", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@AcademicSessionId", AcademicSessionId));
                cmd.Parameters.Add(new SqlParameter("@AdmissionId", AdmissionId));
                cmd.Parameters.Add(new SqlParameter("@SectionId", SectionId));
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds.MarksDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                //grading Sysytem
                var gradingSystems = dataContext.ExamGradingSystems.Where(x => x.ClassNo == ClassNo).OrderByDescending(x => x.FromMarks).ToList();
                gradingSystems.ForEach(sys =>
                {
                    DataTable GradingSystemDataTable = ds.GradingSysytemDataTable;
                    DataRow gsDataRow = GradingSystemDataTable.NewRow();
                    if (sys.FromMarks == 0)
                    {
                        gsDataRow["MarksRange"] = sys.ToMarks.ToString("#,0") + " & Below ";
                        gsDataRow["Grade"] = sys.Grade + " (Needs Improvement)";
                    }
                    else
                    {
                        gsDataRow["MarksRange"] = sys.FromMarks.ToString("#,0") + " - " + sys.ToMarks.ToString("#,0");
                        gsDataRow["Grade"] = sys.Grade;
                    }
                    GradingSystemDataTable.Rows.Add(gsDataRow);
                });

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
        public ActionResult PrintMarksheetUptoEightHY(int AcademicSessionId, int SectionId, int AdmissionId)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                int ClassNo = 0;
                if (AdmissionId > 0)
                {
                    var admission = (from a1 in dataContext.Admissions
                                     join s1 in dataContext.Sections
                                     on a1.SectionId equals s1.SectionId
                                     join c1 in dataContext.Classes
                                     on s1.ClassId equals c1.ClassId
                                     where a1.AdmissionId == AdmissionId
                                     select new { a1.SectionId, a1.AcademicSessionId, c1.ClassNo }).First();
                    ClassNo = admission.ClassNo;
                    SectionId = admission.SectionId.Value;
                    AcademicSessionId = admission.AcademicSessionId.Value;
                }
                else
                    ClassNo = dataContext.Sections.Where(x => x.SectionId == SectionId).First().Class.ClassNo;

                MarksheetUpToEight ds = new MarksheetUpToEight();
                var marksheets = (from m1 in dataContext.ExamMarksheets
                                  join m2 in dataContext.ExamMarksUptoEights
                                  on m1.ExamMarksheetId equals m2.ExamMarksheetId into subExams
                                  join a1 in dataContext.Admissions
                                  on m1.AdmissionId equals a1.AdmissionId
                                  join s1 in dataContext.Sections
                                  on a1.SectionId equals s1.SectionId
                                  join c1 in dataContext.Classes
                                  on s1.ClassId equals c1.ClassId
                                  join st1 in dataContext.Students
                                  on a1.StudentId equals st1.StudentId
                                  join ct1 in dataContext.Cities
                                  on st1.PermanentCityId equals ct1.CityId into subCity
                                  from sCity in subCity.DefaultIfEmpty()
                                  join st2 in dataContext.States
                                  on sCity.StateId equals st2.StateId into subState
                                  from sState in subState.DefaultIfEmpty()
                                  join s2 in dataContext.AcademicSessions
                                  on a1.AcademicSessionId equals s2.AcademicSessionId
                                  where a1.AcademicSessionId == AcademicSessionId
                                  && a1.SectionId == SectionId
                                  && (AdmissionId > 0 ? a1.AdmissionId == AdmissionId : true)
                                  && subExams.Any()
                                  select new
                                  {
                                      st1.AdmissionNo,
                                      a1.RollNo,
                                      st1.FullName,
                                      m1.ExamMarksheetId,
                                      s1.SectionName,
                                      c1.ClassName,
                                      c1.ClassId,
                                      c1.ClassNo,
                                      s2.SessionName,
                                      st1.MobileNo,
                                      st1.DateOfBirth,
                                      st1.FatherName,
                                      st1.MotherName,
                                      st1.House.HouseName,
                                      m1.HYPresentDays,
                                      m1.HYRank,
                                      a1.AcademicSessionId,
                                      m1.HYMarksObtained,
                                      a1.SectionId,
                                      a1.Height,
                                      a1.Weight,
                                      Address = st1.PermanentAddressLine1,
                                      sCity.CityName,
                                      MarksheetGradeList = (from e1 in dataContext.ExamMarksheetGrades
                                                            join cs1 in dataContext.ExamClassSubjects
                                                            on e1.ExamClassSubjectId equals cs1.ExamClassSubjectId
                                                            join s1 in dataContext.ExamSubjects
                                                            on cs1.ExamSubjectId equals s1.ExamSubjectId
                                                            where e1.ExamMarksheetId == m1.ExamMarksheetId
                                                            orderby s1.SubjectName
                                                            select new
                                                            {
                                                                s1.SubjectName,
                                                                Grade = e1.ExamGradingScaleDetail.GradingScale,
                                                                cs1.GradeType,
                                                                e1.ExamGradingScaleDetail.ExamGradingScale.GradingTitle,
                                                                e1.ExamTerm
                                                            }).ToList(),

                                  }).ToList();

                var sectionAttendence = dataContext.ExamSectionAttendences.Where(x => x.SectionId == SectionId && x.AcademicSessionId == AcademicSessionId).First();
                decimal TotalMarks = (from m1 in dataContext.ExamClasses
                                      where m1.AcademicSessionId == AcademicSessionId
                                      && m1.ClassNo == ClassNo
                                      group m1 by m1.ExamClassSubjectId into SubjectList
                                      select SubjectList).Count() * 100;
                bool IsSubjectGradeAllow = false;
                marksheets.ForEach(marksheet =>
                {
                    decimal TotalMarksObtained = marksheet.HYMarksObtained ?? 0;
                    DataTable MarksheetDataTable = ds.Marksheet;
                    DataRow MarksheetDataRow = MarksheetDataTable.NewRow();
                    MarksheetDataRow["StudentName"] = marksheet.FullName;
                    MarksheetDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                    MarksheetDataRow["AdmissionNo"] = marksheet.AdmissionNo;
                    MarksheetDataRow["SessionName"] = marksheet.SessionName;
                    MarksheetDataRow["FatherName"] = marksheet.FatherName;
                    MarksheetDataRow["MotherName"] = marksheet.MotherName;
                    MarksheetDataRow["Address"] = marksheet.Address + " " + marksheet.CityName;
                    MarksheetDataRow["ClassName"] = marksheet.ClassName;
                    MarksheetDataRow["SectionName"] = marksheet.SectionName;
                    MarksheetDataRow["RollNo"] = marksheet.RollNo;
                    MarksheetDataRow["ContactNo"] = marksheet.MobileNo;
                    MarksheetDataRow["Height"] = marksheet.Height;
                    MarksheetDataRow["Weight"] = marksheet.Weight;
                    MarksheetDataRow["ClassNo"] = marksheet.ClassNo;
                    //All Periodic Test Marks
                    decimal HY_AllPeriodicTestMaxMarks = 0;
                    var periodicTests = (from m1 in dataContext.ExamMarksheetDetails
                                         join e1 in dataContext.ExamClasses
                                         on m1.ExamClassId equals e1.ExamClassId
                                         join e2 in dataContext.Examinations
                                         on e1.ExaminationId equals e2.ExaminationId
                                         where m1.ExamMarksheetId == marksheet.ExamMarksheetId
                                         && m1.ExamTerm == (byte)ExamTerm.TermOne
                                         && e2.ExamType == (byte)ExamType.PeriodicTest
                                         && e1.TheoryMaxMarks.HasValue
                                         group e1 by new { e1.ExamClassSubjectId } into examinationList
                                         select examinationList).Distinct().ToList();
                    periodicTests.ForEach(subject =>
                    {
                        decimal maxMarks = subject.Sum(x => x.TheoryMaxMarks.Value);
                        if (HY_AllPeriodicTestMaxMarks < maxMarks)
                            HY_AllPeriodicTestMaxMarks = maxMarks;
                    });
                    MarksheetDataRow["HY_AllPeriodicTestMaxMarks"] = HY_AllPeriodicTestMaxMarks;
                    MarksheetDataRow["HYRank"] = marksheet.HYRank;
                    MarksheetDataRow["HYWorkingDays"] = sectionAttendence.HYWorkingDays;
                    MarksheetDataRow["HYPresentDays"] = marksheet.HYPresentDays;
                    MarksheetDataRow["ANAggregateMarks"] = TotalMarksObtained.ToString("#,0.0") + "/" + TotalMarks.ToString("#,0");
                    if (TotalMarks > 0)
                        MarksheetDataRow["ANPercentage"] = (TotalMarksObtained / TotalMarks * 100).ToString("#,0.0") + " %";
                    MarksheetDataRow["DateOfBirth"] = marksheet.DateOfBirth.HasValue ? marksheet.DateOfBirth.Value.ToString("dd-MM-yyyy") : "";
                    MarksheetDataTable.Rows.Add(MarksheetDataRow);



                    //Grade Subjects
                    marksheet.MarksheetGradeList.GroupBy(x => new { x.GradingTitle, x.GradeType }).ToList().ForEach(gradingList =>
                    {
                        string GradingTitle = string.Empty;
                        if (gradingList.Key.GradeType == (byte)GradeType.None && gradingList.GroupBy(x => x.SubjectName).Count() > 3)
                        {
                            int i = 0;
                            gradingList.GroupBy(x => x.SubjectName).ToList().ForEach(grade =>
                            {
                                IsSubjectGradeAllow = true;
                                if (i % 2 == 0)
                                {
                                    DataTable SubjectGradeDataTable = ds.SubjectGradeDataTable;
                                    DataRow GradeDataRow = SubjectGradeDataTable.NewRow();
                                    GradeDataRow["SubjectName"] = grade.Key;
                                    GradeDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                                    var HY_Grade = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne);
                                    if (HY_Grade.Any())
                                        GradeDataRow["HY_Grade"] = HY_Grade.First().Grade;
                                    SubjectGradeDataTable.Rows.Add(GradeDataRow);
                                }
                                else
                                {
                                    DataTable SubjectGradeDataTable = ds.SubjectGradeDataTableTwo;
                                    DataRow GradeDataRow = SubjectGradeDataTable.NewRow();
                                    GradeDataRow["SubjectName"] = grade.Key;
                                    GradeDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                                    var HY_Grade = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne);
                                    if (HY_Grade.Any())
                                        GradeDataRow["HY_Grade"] = HY_Grade.First().Grade;
                                    SubjectGradeDataTable.Rows.Add(GradeDataRow);
                                }
                                i++;
                            });
                        }
                        else
                        {
                            if (gradingList.Key.GradeType == (byte)GradeType.None)
                                GradingTitle = "SUBJECT NAME";
                            else
                                GradingTitle = System.Text.RegularExpressions.Regex.Replace(Enum.GetName(typeof(GradeType), gradingList.Key.GradeType), "[A-Z]", " $0").Replace("_", " -") + " [ on a " + gradingList.Key.GradingTitle + " ]";
                            DataTable GradeGroupDataTable = ds.GradeGroupDataTable;
                            DataRow ggdDataRow = GradeGroupDataTable.NewRow();
                            ggdDataRow["GradeTitle"] = GradingTitle;
                            ggdDataRow["GradeType"] = gradingList.Key.GradeType;
                            ggdDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;

                            if (gradingList.Key.GradeType == (byte)GradeType.None)
                                ggdDataRow["GradeNo"] = 1;
                            else if (gradingList.Key.GradeType == (byte)GradeType.Co_ScholasticAreas)
                                ggdDataRow["GradeNo"] = 2;
                            else
                                ggdDataRow["GradeNo"] = 3;


                            GradeGroupDataTable.Rows.Add(ggdDataRow);

                            gradingList.GroupBy(x => x.SubjectName).ToList().ForEach(grade =>
                            {
                                DataTable GradeDataTable = ds.GradeDataTable;
                                DataRow GradeDataRow = GradeDataTable.NewRow();
                                GradeDataRow["Subject"] = grade.Key;
                                GradeDataRow["GradeType"] = gradingList.Key.GradeType;
                                GradeDataRow["GradeTitle"] = GradingTitle;
                                GradeDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                                var HY_Grade = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne);
                                if (HY_Grade.Any())
                                    GradeDataRow["HY_Grade"] = HY_Grade.First().Grade;
                                GradeDataTable.Rows.Add(GradeDataRow);
                            });
                        }
                    });
                });

                //Marks Subject
                DataTable MarksDataTable = ds.MarksDataTable;
                SqlCommand cmd = new SqlCommand("select m1.ExamMarksheetId,Subject = s1.SubjectName,m1.HY_All_PeriodicTest,m1.HY_PeriodicTest,HY_NotebookSubmition,HY_SubjectEnreachment,HY_Written,HY_MarksObtained,HY_Grade from ExamMarksUptoEight as m1 join ExamSubject as s1 on m1.ExamSubjectId = s1.ExamSubjectId", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds.MarksDataTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                //grading Sysytem
                var gradingSystems = dataContext.ExamGradingSystems.Where(x => x.ClassNo == ClassNo).OrderByDescending(x => x.FromMarks).ToList();
                gradingSystems.ForEach(sys =>
                {
                    DataTable GradingSystemDataTable = ds.GradingSysytemDataTable;
                    DataRow gsDataRow = GradingSystemDataTable.NewRow();
                    if (sys.FromMarks == 0)
                    {
                        gsDataRow["MarksRange"] = sys.ToMarks.ToString("#,0") + " & Below ";
                        gsDataRow["Grade"] = sys.Grade + " (Needs Improvement)";
                    }
                    else
                    {
                        gsDataRow["MarksRange"] = sys.FromMarks.ToString("#,0") + " - " + sys.ToMarks.ToString("#,0");
                        gsDataRow["Grade"] = sys.Grade;
                    }
                    GradingSystemDataTable.Rows.Add(gsDataRow);
                });
                ReportDocument rp = new ReportDocument();
                if (ClassNo > 5)
                    rp.Load(Server.MapPath("~/reports/Marksheet_HY_UoToEight.rpt"));
                else if (IsSubjectGradeAllow)
                    rp.Load(Server.MapPath("~/reports/Marksheet_HY_UoToFive_2.rpt"));
                else
                    rp.Load(Server.MapPath("~/reports/Marksheet_HY_UoToFive.rpt"));

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

        public ActionResult PrintMarksheetNineAndTenHY(int AcademicSessionId, int SectionId, int AdmissionId)
        {
            try
            {


                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                int ClassNo = 0;
                if (AdmissionId > 0)
                {
                    var admission = (from a1 in dataContext.Admissions
                                     join s1 in dataContext.Sections
                                     on a1.SectionId equals s1.SectionId
                                     join c1 in dataContext.Classes
                                     on s1.ClassId equals c1.ClassId
                                     where a1.AdmissionId == AdmissionId
                                     select new { a1.SectionId, a1.AcademicSessionId, c1.ClassNo }).First();
                    ClassNo = admission.ClassNo;
                    SectionId = admission.SectionId.Value;
                    AcademicSessionId = admission.AcademicSessionId.Value;
                }
                else
                    ClassNo = dataContext.Sections.Where(x => x.SectionId == SectionId).First().Class.ClassNo;

                MarksheetUpToEight ds = new MarksheetUpToEight();
                var marksheets = (from m1 in dataContext.ExamMarksheets
                                  join m2 in dataContext.ExamMarksNineAndTens
                                  on m1.ExamMarksheetId equals m2.ExamMarksheetId into subExams
                                  join a1 in dataContext.Admissions
                                  on m1.AdmissionId equals a1.AdmissionId
                                  join s1 in dataContext.Sections
                                  on a1.SectionId equals s1.SectionId
                                  join c1 in dataContext.Classes
                                  on s1.ClassId equals c1.ClassId
                                  join st1 in dataContext.Students
                                  on a1.StudentId equals st1.StudentId
                                  join ct1 in dataContext.Cities
                                  on st1.PermanentCityId equals ct1.CityId into subCity
                                  from sCity in subCity.DefaultIfEmpty()
                                  join st2 in dataContext.States
                                  on sCity.StateId equals st2.StateId into subState
                                  from sState in subState.DefaultIfEmpty()
                                  join s2 in dataContext.AcademicSessions
                                  on a1.AcademicSessionId equals s2.AcademicSessionId
                                  where a1.AcademicSessionId == AcademicSessionId
                                  && a1.SectionId == SectionId
                                  && (AdmissionId > 0 ? a1.AdmissionId == AdmissionId : true)
                                  && subExams.Any()
                                  select new
                                  {
                                      st1.AdmissionNo,
                                      a1.RollNo,
                                      st1.FullName,
                                      m1.ExamMarksheetId,
                                      s1.SectionName,
                                      st1.MobileNo,
                                      c1.ClassName,
                                      c1.ClassId,
                                      s2.SessionName,
                                      st1.DateOfBirth,
                                      st1.FatherName,
                                      st1.MotherName,
                                      st1.House.HouseName,
                                      m1.HYPresentDays,
                                      m1.HYRank,
                                      a1.AcademicSessionId,
                                      m1.HYMarksObtained,
                                      a1.SectionId,
                                      Address = st1.PermanentAddressLine1,
                                      sCity.CityName,
                                      MarksheetGradeList = (from e1 in dataContext.ExamMarksheetGrades
                                                            join cs1 in dataContext.ExamClassSubjects
                                                            on e1.ExamClassSubjectId equals cs1.ExamClassSubjectId
                                                            join s1 in dataContext.ExamSubjects
                                                            on cs1.ExamSubjectId equals s1.ExamSubjectId
                                                            where e1.ExamMarksheetId == m1.ExamMarksheetId
                                                            orderby s1.SubjectName
                                                            select new
                                                            {
                                                                s1.SubjectName,
                                                                Grade = e1.ExamGradingScaleDetail.GradingScale,
                                                                cs1.GradeType,
                                                                e1.ExamGradingScaleDetail.ExamGradingScale.GradingTitle,
                                                                e1.ExamTerm
                                                            })

                                  }).ToList();

                var sectionAttendence = dataContext.ExamSectionAttendences.Where(x => x.SectionId == SectionId && x.AcademicSessionId == AcademicSessionId).First();
                decimal TotalMarks = (from m1 in dataContext.ExamClasses
                                      where m1.AcademicSessionId == AcademicSessionId
                                      && m1.ClassNo == ClassNo
                                      group m1 by m1.ExamClassSubjectId into SubjectList
                                      select SubjectList).Count() * 100;
                marksheets.ForEach(marksheet =>
                {
                    decimal TotalMarksObtained = marksheet.HYMarksObtained ?? 0;
                    DataTable MarksheetDataTable = ds.Marksheet;
                    DataRow MarksheetDataRow = MarksheetDataTable.NewRow();
                    MarksheetDataRow["StudentName"] = marksheet.FullName;
                    MarksheetDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                    MarksheetDataRow["AdmissionNo"] = marksheet.AdmissionNo;
                    MarksheetDataRow["SessionName"] = marksheet.SessionName;
                    MarksheetDataRow["FatherName"] = marksheet.FatherName;
                    MarksheetDataRow["MotherName"] = marksheet.MotherName;
                    MarksheetDataRow["ContactNo"] = marksheet.MobileNo;
                    MarksheetDataRow["Address"] = marksheet.Address + " " + marksheet.CityName;
                    MarksheetDataRow["ClassName"] = marksheet.ClassName;
                    MarksheetDataRow["SectionName"] = marksheet.SectionName;
                    MarksheetDataRow["RollNo"] = marksheet.RollNo;
                    MarksheetDataRow["HYRank"] = marksheet.HYRank;
                    MarksheetDataRow["HYWorkingDays"] = sectionAttendence.HYWorkingDays;
                    MarksheetDataRow["HYPresentDays"] = marksheet.HYPresentDays;
                    MarksheetDataRow["ANAggregateMarks"] = TotalMarksObtained.ToString("#,0.0") + "/" + TotalMarks.ToString("#,0");
                    if (TotalMarks > 0)
                        MarksheetDataRow["ANPercentage"] = (TotalMarksObtained / TotalMarks * 100).ToString("#,0.0") + " %";
                    MarksheetDataRow["DateOfBirth"] = marksheet.DateOfBirth.HasValue ? marksheet.DateOfBirth.Value.ToString("dd-MM-yyyy") : "";

                    //All Periodic Test Marks
                    decimal HY_AllPeriodicTestMaxMarks = 0;
                    var periodicTests = (from m1 in dataContext.ExamMarksheetDetails
                                         join e1 in dataContext.ExamClasses
                                         on m1.ExamClassId equals e1.ExamClassId
                                         join e2 in dataContext.Examinations
                                         on e1.ExaminationId equals e2.ExaminationId
                                         where m1.ExamMarksheetId == marksheet.ExamMarksheetId
                                         && m1.ExamTerm == (byte)ExamTerm.TermOne
                                         && e2.ExamType == (byte)ExamType.PeriodicTest
                                         && e1.TheoryMaxMarks.HasValue
                                         group e1 by new { e1.ExamClassSubjectId } into examinationList
                                         select examinationList).Distinct().ToList();
                    periodicTests.ForEach(subject =>
                      {
                          decimal maxMarks = subject.Sum(x => x.TheoryMaxMarks.Value);
                          if (HY_AllPeriodicTestMaxMarks < maxMarks)
                              HY_AllPeriodicTestMaxMarks = maxMarks;
                      });

                    MarksheetDataRow["HY_AllPeriodicTestMaxMarks"] = Convert.ToInt32(HY_AllPeriodicTestMaxMarks);

                    MarksheetDataTable.Rows.Add(MarksheetDataRow);



                    //Grade Subjects 
                    marksheet.MarksheetGradeList.GroupBy(x => new { x.GradingTitle, x.GradeType }).OrderByDescending(x => x.Key.GradeType).ToList().ForEach(gradingList =>
                    {
                        string GradingTitle = string.Empty;
                        if (gradingList.Key.GradeType == (byte)GradeType.None)
                            GradingTitle = "Subject " + " [ on a " + gradingList.Key.GradingTitle + " ]";
                        else
                            GradingTitle = System.Text.RegularExpressions.Regex.Replace(Enum.GetName(typeof(GradeType), gradingList.Key.GradeType), "[A-Z]", " $0").Replace("_", " -") + " [ on a " + gradingList.Key.GradingTitle + " ]";
                        DataTable GradeGroupDataTable = ds.GradeGroupDataTable;
                        DataRow ggdDataRow = GradeGroupDataTable.NewRow();
                        if (gradingList.Key.GradeType == (byte)GradeType.Co_ScholasticAreas)
                            GradingTitle = "Part:2 " + GradingTitle;
                        else
                            GradingTitle = "Part:3 " + GradingTitle;
                        ggdDataRow["GradeTitle"] = GradingTitle;
                        ggdDataRow["GradeType"] = gradingList.Key.GradeType;
                        ggdDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                        GradeGroupDataTable.Rows.Add(ggdDataRow);

                        gradingList.GroupBy(x => x.SubjectName).ToList().ForEach(grade =>
                        {
                            DataTable GradeDataTable = ds.GradeDataTable;
                            DataRow GradeDataRow = GradeDataTable.NewRow();
                            GradeDataRow["Subject"] = grade.Key;
                            GradeDataRow["GradeType"] = gradingList.Key.GradeType;
                            GradeDataRow["GradeTitle"] = GradingTitle;
                            GradeDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                            var HY_Grade = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne);
                            if (HY_Grade.Any())
                                GradeDataRow["HY_Grade"] = HY_Grade.First().Grade;
                            GradeDataTable.Rows.Add(GradeDataRow);
                        });
                    });

                    //Periodic Test List
                    //var priodicTestList = (from m1 in dataContext.ExamMarksheetDetails
                    //                       join e1 in dataContext.ExamClasses
                    //                       on m1.ExamClassId equals e1.ExamClassId
                    //                       join e2 in dataContext.Examinations
                    //                       on e1.ExaminationId equals e2.ExaminationId
                    //                       join s1 in dataContext.ExamClassSubjects
                    //                       on m1.ExamClassSubjectId equals s1.ExamClassSubjectId
                    //                       join s2 in dataContext.ExamSubjects
                    //                       on s1.ExamSubjectId equals s2.ExamSubjectId
                    //                       where m1.ExamMarksheetId == marksheet.ExamMarksheetId
                    //                       && m1.ExamTerm == (byte)ExamTerm.TermOne
                    //                       && e2.ExamType == (byte)ExamType.PeriodicTest
                    //                       select new
                    //                       {
                    //                           e2.ExaminationName,
                    //                           s2.SubjectName,
                    //                           m1.TheoryMarksObtained,
                    //                           m1.ExamMarksheetId,
                    //                           e1.TheoryMaxMarks,
                    //                           e1.ExaminationId
                    //                       }).ToList();
                    //foreach (var exam in priodicTestList)
                    //{
                    //    DataTable examinationDataTable = ds.ExaminationDataTable;
                    //    DataRow examRow = examinationDataTable.NewRow();
                    //    examRow["ExaminationName"] = exam.ExaminationName;
                    //    examRow["ExamMarksheetId"] = exam.ExamMarksheetId;
                    //    examRow["ExaminationId"] = exam.ExaminationId;
                    //    examRow["SubjectName"] = exam.SubjectName;
                    //    examRow["TheoryMarksObtained"] = exam.TheoryMarksObtained;
                    //    examRow["TheoryMaxMarks"] = exam.TheoryMaxMarks;
                    //    ds.ExaminationDataTable.Rows.Add(examRow);
                    //}
                });

                //Marks Subject
                SqlCommand cmd = new SqlCommand("ExamMarksNineAndTenList", connection);
                cmd.Parameters.Add(new SqlParameter("@AcademicSessionId", AcademicSessionId));
                cmd.Parameters.Add(new SqlParameter("@AdmissionId", AdmissionId));
                cmd.Parameters.Add(new SqlParameter("@SectionId", SectionId));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds.MarksNineAndTen);
                dataAdapter.Dispose();
                cmd.Dispose();

                //grading Sysytem
                var gradingSystems = dataContext.ExamGradingSystems.Where(x => x.ClassNo == ClassNo).OrderByDescending(x => x.FromMarks).ToList();
                gradingSystems.ForEach(sys =>
                {
                    DataTable GradingSystemDataTable = ds.GradingSysytemDataTable;
                    DataRow gsDataRow = GradingSystemDataTable.NewRow();
                    if (sys.FromMarks == 0)
                    {
                        gsDataRow["MarksRange"] = sys.ToMarks.ToString("#,0") + " & Below ";
                        gsDataRow["Grade"] = sys.Grade + " (Failed)";
                    }
                    else
                    {
                        gsDataRow["MarksRange"] = sys.FromMarks.ToString("#,0") + " - " + sys.ToMarks.ToString("#,0");
                        gsDataRow["Grade"] = sys.Grade;
                    }
                    GradingSystemDataTable.Rows.Add(gsDataRow);
                });
                ReportDocument rp = new ReportDocument();
                if (ClassNo == 10)
                    rp.Load(Server.MapPath("~/reports/Marksheet_HY_Ten.rpt"));
                else
                    rp.Load(Server.MapPath("~/reports/Marksheet_HY_NineAndTen.rpt"));
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
        public ActionResult PrintMarksheetEleven(int AcademicSessionId, int SectionId, int AdmissionId, byte Term)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                if (Term == (byte)ExamTerm.TermOne)
                    rp.Load(Server.MapPath("~/reports/marksheet-eleven-HY.rpt"));
                else
                    rp.Load(Server.MapPath("~/reports/marksheet-eleven-AN.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                MarksheetElevenDataset ds = new MarksheetElevenDataset();
                var marksheetList = (from m1 in dataContext.ExamMarksheets
                                     join a1 in dataContext.Admissions
                                     on m1.AdmissionId equals a1.AdmissionId
                                     join s1 in dataContext.Sections
                                     on a1.SectionId equals s1.SectionId
                                     join c1 in dataContext.Classes
                                     on s1.ClassId equals c1.ClassId
                                     join st1 in dataContext.Students
                                     on a1.StudentId equals st1.StudentId
                                     join s2 in dataContext.AcademicSessions
                                     on a1.AcademicSessionId equals s2.AcademicSessionId
                                     join ct1 in dataContext.Cities
                                     on st1.PermanentCityId equals ct1.CityId into subCity
                                     from sCity in subCity.DefaultIfEmpty()
                                     where AdmissionId > 0 ? a1.AdmissionId == AdmissionId : (a1.AcademicSessionId == AcademicSessionId && a1.SectionId == SectionId)
                                     select new
                                     {
                                         a1.AdmissionId,
                                         st1.AdmissionNo,
                                         a1.RollNo,
                                         c1.ClassId,
                                         st1.MobileNo,
                                         st1.FullName,
                                         m1.ExamMarksheetId,
                                         s1.SectionName,
                                         c1.ClassName2,
                                         s2.SessionName,
                                         st1.DateOfBirth,
                                         st1.FatherName,
                                         m1.HYPresentDays,
                                         m1.ANPresentDays,
                                         m1.ANRank,
                                         m1.HYRank,
                                         m1.HYMarksObtained,
                                         a1.AcademicSessionId,
                                         a1.SectionId,
                                         m1.ANMarksObtained,
                                         m1.FinalMarksObtained,
                                         st1.MotherName,
                                         st1.PermanentAddressLine1,
                                         sCity.CityName,
                                         MarksheetGradeList = (from e1 in dataContext.ExamMarksheetGrades
                                                               join cs1 in dataContext.ExamClassSubjects
                                                               on e1.ExamClassSubjectId equals cs1.ExamClassSubjectId
                                                               join s1 in dataContext.ExamSubjects
                                                               on cs1.ExamSubjectId equals s1.ExamSubjectId
                                                               where e1.ExamMarksheetId == m1.ExamMarksheetId
                                                               orderby s1.SubjectName
                                                               select new
                                                               {
                                                                   s1.SubjectName,
                                                                   Grade = e1.ExamGradingScaleDetail.GradingScale,
                                                                   e1.ExamTerm
                                                               }).Distinct(),
                                     }).ToList();
                var sectionAttendences = dataContext.ExamSectionAttendences.Where(x => x.SectionId == SectionId && x.AcademicSessionId == AcademicSessionId);
                if (sectionAttendences.Any())
                {
                    ViewBag.Message = "Attendence is required!!";
                    return View();
                }
                var sectionAttendence = sectionAttendences.First();
                marksheetList.ForEach(marksheet =>
                {
                    decimal TotalMarks = 500, FinalMarksObtainedAN = marksheet.ANMarksObtained ?? 0, FinalMarksObtainedHY = marksheet.HYMarksObtained ?? 0;
                    DataTable MarksheetDataTable = ds.Marksheet;
                    DataRow MarksheetDataRow = MarksheetDataTable.NewRow();
                    MarksheetDataRow["StudentName"] = marksheet.FullName;
                    MarksheetDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                    MarksheetDataRow["AdmissionNo"] = marksheet.AdmissionNo;
                    MarksheetDataRow["SessionName"] = marksheet.SessionName;
                    MarksheetDataRow["FatherName"] = marksheet.FatherName;
                    MarksheetDataRow["MotherName"] = marksheet.MotherName;
                    MarksheetDataRow["Address"] = marksheet.PermanentAddressLine1 + " " + marksheet.CityName;
                    MarksheetDataRow["ClassName"] = marksheet.ClassName2;
                    MarksheetDataRow["SectionName"] = marksheet.SectionName;
                    MarksheetDataRow["RollNo"] = marksheet.RollNo;
                    MarksheetDataRow["MobileNo"] = marksheet.MobileNo;
                    MarksheetDataRow["DateOfBirth"] = marksheet.DateOfBirth.HasValue ? marksheet.DateOfBirth.Value.ToString("dd-MM-yyyy") : "";

                    //Annual Data
                    MarksheetDataRow["ANRank"] = marksheet.ANRank;
                    MarksheetDataRow["ANWorkingDays"] = sectionAttendence.ANWorkingDays;
                    MarksheetDataRow["ANPresentDays"] = marksheet.ANPresentDays;

                    //HY Data
                    MarksheetDataRow["HYWorkingDays"] = sectionAttendence.HYWorkingDays;
                    MarksheetDataRow["HYPresentDays"] = marksheet.HYPresentDays;
                    MarksheetDataRow["HYRank"] = marksheet.HYRank;

                    if (TotalMarks > 0)
                    {
                        MarksheetDataRow["ANPercentage"] = (FinalMarksObtainedAN / TotalMarks * 100).ToString("#,0.0") + " %";
                        MarksheetDataRow["ANAggregateMarks"] = FinalMarksObtainedAN.ToString("#,0.0") + "/" + TotalMarks.ToString("#,0");
                        MarksheetDataRow["HY_AggregateMarks"] = FinalMarksObtainedHY.ToString("#,0.0") + "/" + TotalMarks.ToString("#,0.0");
                        MarksheetDataRow["HY_Percentage"] = (FinalMarksObtainedHY / TotalMarks * 100).ToString("#,0.0") + " %";
                    }
                    MarksheetDataTable.Rows.Add(MarksheetDataRow);

                    //Grade Subjects
                    marksheet.MarksheetGradeList.GroupBy(x => x.SubjectName).ToList().ForEach(grade =>
            {
                DataTable GradeDataTable = ds.GradeDataTable;
                DataRow GradeDataRow = GradeDataTable.NewRow();
                GradeDataRow["Subject"] = grade.Key;
                GradeDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                if (grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne).Any())
                    GradeDataRow["HY_Grade"] = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne).First().Grade;
                if (grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermTwo).Any())
                    GradeDataRow["AN_Grade"] = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermTwo).First().Grade;
                GradeDataTable.Rows.Add(GradeDataRow);
            });
                });

                //Marks Subject
                DataTable MarksDataTable = ds.MarksDataTable;
                SqlCommand cmd = new SqlCommand("select m2.ExamMarksheetId, Subject=s1.SubjectName,m1.HY_MT_1,m1.HY_MT_2,m1.HY_Theory,HY_Practical,HY_MarksObtained,AN_MT_1,AN_MT_2,AN_Theory,AN_Practical,AN_MarksObtained,MT_Weightage_Marks,HY_Theory_Weightage_Marks,HY_Practical_Weightage_Marks,AN_Theory_Weightage_Marks,AN_Practical_Weightage_Marks,FinalMarks,AN_HighestMarks ,HY_HighestMarks from ExamMarksEleven as m1 "
                                                + " join ExamSubject as s1 on m1.ExamSubjectId = s1.ExamSubjectId "
                                                + " join ExamMarksheet as m2 on m1.ExamMarksheetId = m2.ExamMarksheetId "
                                                + " join Admission as a1 on m2.AdmissionId = a1.AdmissionId "
                                                + " left join(select * from ExamHighestMarks where AcademicSessionId = " + AcademicSessionId + " and sectionId = " + SectionId + ") as hm1 on s1.ExamSubjectId = hm1.ExamSubjectId "
                                                 + (AdmissionId > 0 ? (" where a1.AdmissionId = " + AdmissionId) : ("where a1.AcademicSessionId = " + AcademicSessionId + " and a1.SectionId= " + SectionId)), connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds.MarksDataTable);
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
        public ActionResult PrintMarksheetTwelve(int AcademicSessionId, int SectionId, int AdmissionId, byte Term)
        {
            try
            {
                ReportDocument rp = new ReportDocument();
                if (Term == (byte)ExamTerm.TermOne)
                    rp.Load(Server.MapPath("~/reports/marksheet-Twelve-HY.rpt"));
                else
                    rp.Load(Server.MapPath("~/reports/marksheet-Twelve-AN.rpt"));

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                MarksheetElevenDataset ds = new MarksheetElevenDataset();
                var marksheetList = (from m1 in dataContext.ExamMarksheets
                                     join a1 in dataContext.Admissions
                                     on m1.AdmissionId equals a1.AdmissionId
                                     join s1 in dataContext.Sections
                                     on a1.SectionId equals s1.SectionId
                                     join c1 in dataContext.Classes
                                     on s1.ClassId equals c1.ClassId
                                     join st1 in dataContext.Students
                                     on a1.StudentId equals st1.StudentId
                                     join s2 in dataContext.AcademicSessions
                                     on a1.AcademicSessionId equals s2.AcademicSessionId
                                     join ct1 in dataContext.Cities
                                     on st1.PermanentCityId equals ct1.CityId into subCity
                                     from sCity in subCity.DefaultIfEmpty()
                                     where AdmissionId > 0 ? a1.AdmissionId == AdmissionId : (a1.AcademicSessionId == AcademicSessionId && a1.SectionId == SectionId)
                                     select new
                                     {
                                         a1.AdmissionId,
                                         st1.AdmissionNo,
                                         a1.RollNo,
                                         c1.ClassId,
                                         st1.MobileNo,
                                         st1.FullName,
                                         m1.ExamMarksheetId,
                                         s1.SectionName,
                                         c1.ClassName2,
                                         s2.SessionName,
                                         st1.DateOfBirth,
                                         st1.FatherName,
                                         m1.HYPresentDays,
                                         m1.ANPresentDays,
                                         m1.ANRank,
                                         m1.HYRank,
                                         m1.HYMarksObtained,
                                         a1.AcademicSessionId,
                                         a1.SectionId,
                                         m1.ANMarksObtained,
                                         m1.FinalMarksObtained,
                                         st1.MotherName,
                                         st1.PermanentAddressLine1,
                                         sCity.CityName,
                                         MarksheetGradeList = (from e1 in dataContext.ExamMarksheetGrades
                                                               join cs1 in dataContext.ExamClassSubjects
                                                               on e1.ExamClassSubjectId equals cs1.ExamClassSubjectId
                                                               join s1 in dataContext.ExamSubjects
                                                               on cs1.ExamSubjectId equals s1.ExamSubjectId
                                                               where e1.ExamMarksheetId == m1.ExamMarksheetId
                                                               orderby s1.SubjectName
                                                               select new
                                                               {
                                                                   s1.SubjectName,
                                                                   Grade = e1.ExamGradingScaleDetail.GradingScale,
                                                                   e1.ExamTerm
                                                               }).Distinct(),
                                     }).ToList();
                var sectionAttendences = dataContext.ExamSectionAttendences.Where(x => x.SectionId == SectionId && x.AcademicSessionId == AcademicSessionId);
                if (!sectionAttendences.Any())
                {
                    ViewBag.Message = "Attendence is required!!";
                    return View();
                }
                var sectionAttendence = sectionAttendences.First();
                decimal TotalHYMarks = 250, TotalANMarks = 500;
                marksheetList.ForEach(marksheet =>
                {
                    decimal FinalMarksObtainedAN = marksheet.ANMarksObtained ?? 0, FinalMarksObtainedHY = marksheet.HYMarksObtained ?? 0;
                    DataTable MarksheetDataTable = ds.Marksheet;
                    DataRow MarksheetDataRow = MarksheetDataTable.NewRow();
                    MarksheetDataRow["StudentName"] = marksheet.FullName;
                    MarksheetDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                    MarksheetDataRow["AdmissionNo"] = marksheet.AdmissionNo;
                    MarksheetDataRow["SessionName"] = marksheet.SessionName;
                    MarksheetDataRow["FatherName"] = marksheet.FatherName;
                    MarksheetDataRow["MotherName"] = marksheet.MotherName;
                    MarksheetDataRow["Address"] = marksheet.PermanentAddressLine1 + " " + marksheet.CityName;
                    MarksheetDataRow["ClassName"] = marksheet.ClassName2;
                    MarksheetDataRow["SectionName"] = marksheet.SectionName;
                    MarksheetDataRow["RollNo"] = marksheet.RollNo;
                    MarksheetDataRow["MobileNo"] = marksheet.MobileNo;
                    MarksheetDataRow["DateOfBirth"] = marksheet.DateOfBirth.HasValue ? marksheet.DateOfBirth.Value.ToString("dd-MM-yyyy") : "";


                    //HY Data
                    MarksheetDataRow["HYWorkingDays"] = sectionAttendence.HYWorkingDays;
                    MarksheetDataRow["HYPresentDays"] = marksheet.HYPresentDays;
                    MarksheetDataRow["HYRank"] = marksheet.HYRank;
                    if (TotalHYMarks > 0)
                    {
                        MarksheetDataRow["HY_AggregateMarks"] = FinalMarksObtainedHY.ToString("#,0.0") + "/" + TotalHYMarks.ToString("#,0.0");
                        MarksheetDataRow["HY_Percentage"] = (FinalMarksObtainedHY / TotalHYMarks * 100).ToString("#,0.0") + " %";
                    }

                    //Annual Data
                    MarksheetDataRow["ANRank"] = marksheet.ANRank;
                    MarksheetDataRow["ANWorkingDays"] = sectionAttendence.ANWorkingDays;
                    MarksheetDataRow["ANPresentDays"] = marksheet.ANPresentDays;
                    if (TotalANMarks > 0)
                    {
                        MarksheetDataRow["ANPercentage"] = (FinalMarksObtainedAN / TotalANMarks * 100).ToString("#,0.0") + " %";
                        MarksheetDataRow["ANAggregateMarks"] = FinalMarksObtainedAN.ToString("#,0.0") + "/" + TotalANMarks.ToString("#,0");
                    }

                    MarksheetDataTable.Rows.Add(MarksheetDataRow);

                    //Grade Subjects
                    marksheet.MarksheetGradeList.GroupBy(x => x.SubjectName).ToList().ForEach(grade =>
            {
                DataTable GradeDataTable = ds.GradeDataTable;
                DataRow GradeDataRow = GradeDataTable.NewRow();
                GradeDataRow["Subject"] = grade.Key;
                GradeDataRow["ExamMarksheetId"] = marksheet.ExamMarksheetId;
                if (grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne).Any())
                    GradeDataRow["HY_Grade"] = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne).First().Grade;
                if (grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermTwo).Any())
                    GradeDataRow["AN_Grade"] = grade.Where(x => x.ExamTerm == (byte)ExamTerm.TermTwo).First().Grade;
                GradeDataTable.Rows.Add(GradeDataRow);
            });
                });

                //Marks Subject
                SqlCommand cmd = new SqlCommand("ExamMarksTwelveList", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@AcademicSessionId", AcademicSessionId));
                cmd.Parameters.Add(new SqlParameter("@AdmissionId", AdmissionId));
                cmd.Parameters.Add(new SqlParameter("@SectionId", SectionId));
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ds.ExamMarksTwelve);
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
        public ActionResult PrintRankerList(int AcademicSessionId, byte ExamTerm, int SectionId, int ExaminationId, byte docType)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);

                //Calculation Of max marks
                var section = dataContext.Sections.Where(x => x.SectionId == SectionId).First();
                int ClassNo = section.Class.ClassNo;
                //decimal MaxMarks = 0;


                //Calculation Of Marks
                var marksDetail = (from m1 in dataContext.ExamMarksheetDetails
                                   join m2 in dataContext.ExamMarksheets
                                   on m1.ExamMarksheetId equals m2.ExamMarksheetId
                                   join a1 in dataContext.Admissions
                                   on m2.AdmissionId equals a1.AdmissionId
                                   join s1 in dataContext.Students
                                   on a1.StudentId equals s1.StudentId
                                   join e1 in dataContext.ExamClasses
                                   on m1.ExamClassId equals e1.ExamClassId
                                   join e2 in dataContext.Examinations
                                   on e1.ExaminationId equals e2.ExaminationId
                                   join cs1 in dataContext.ExamClassSubjects
                                   on m1.ExamClassSubjectId equals cs1.ExamClassSubjectId
                                   where a1.AcademicSessionId == AcademicSessionId
                                   && a1.SectionId == SectionId
                                   && cs1.SubjectType == (byte)SubjectType.Compulsory
                                   && (ExaminationId > 0 ? (e1.ExaminationId == ExaminationId && m1.ExamTerm == ExamTerm) : (ClassNo == 10 ? e2.ExamType == (byte)ExamType.HalfYearly : true))
                                   select new
                                   {
                                       s1.FullName,
                                       a1.RollNo,
                                       a1.AdmissionId,
                                       s1.AdmissionNo,
                                       m1.TheoryMarksObtained,
                                       m1.PracticalMarksObtained,
                                       m2.HYMarksObtained,
                                       m2.ANMarksObtained,
                                       m2.FinalMarksObtained
                                   }).GroupBy(x => new { x.FullName, x.RollNo, x.AdmissionId, x.AdmissionNo, x.HYMarksObtained, x.ANMarksObtained }).ToList();

                var rankerList = (from m1 in marksDetail
                                  select new
                                  {
                                      m1.Key.FullName,
                                      m1.Key.RollNo,
                                      m1.Key.AdmissionNo,
                                      m1.Key.AdmissionId,
                                      MarksObtained = ExaminationId > 0 ? m1.Sum(x => x.TheoryMarksObtained + x.PracticalMarksObtained) : (ExamTerm == (byte)School.ExamTerm.TermOne ? (m1.Key.HYMarksObtained ?? 0) : (ExamTerm == (byte)School.ExamTerm.TermTwo ? (m1.Key.ANMarksObtained ?? 0) : (m1.Key.HYMarksObtained ?? 0) + (m1.Key.ANMarksObtained ?? 0)))
                                  }).ToList();

                ExamReportDataset ds = new ExamReportDataset();
                int rank = 1;
                rankerList.GroupBy(x => x.MarksObtained).OrderByDescending(x => x.Key).ToList().ForEach(studentList =>
                  {
                      studentList.ToList().ForEach(student =>
                      {
                          decimal MaxMarks = 0;
                          var studentSubjectIds = LoadExamData.GetStudentSubjectuIds(dataContext, student.AdmissionId);
                          //if (ExaminationId > 0)
                          //{
                          var examClasses = (from e1 in dataContext.ExamClasses
                                             join e3 in dataContext.Examinations
                                             on e1.ExaminationId equals e3.ExaminationId
                                             join e2 in dataContext.ExamClassSubjects.Where(x => x.ClassNo == ClassNo && x.SubjectType == (byte)SubjectType.Compulsory)
                                             on e1.ExamClassSubjectId equals e2.ExamClassSubjectId
                                             where e1.AcademicSessionId == AcademicSessionId
                                             && e1.ClassNo == ClassNo
                                             && e1.ExamTerm == ExamTerm
                                             && (ExaminationId > 0 ? e1.ExaminationId == ExaminationId : (ClassNo == 10 ? e3.ExamType == (byte)ExamType.HalfYearly : true))
                                             && studentSubjectIds.Contains(e2.ExamSubjectId)

                                             select e1).ToList();
                          if (examClasses.Any())
                              MaxMarks = examClasses.Sum(x => (x.TheoryMaxMarks ?? 0) + (x.PracticalMaxMarks ?? 0));
                          //}
                          //else
                          //{
                          //    var weitages = (from m1 in dataContext.ExamClassWeightages
                          //                    join cs1 in dataContext.ExamClassSubjects
                          //                    on m1.ExamClassSubjectId equals cs1.ExamClassSubjectId
                          //                    where m1.AcademicSessionId == AcademicSessionId
                          //                    && m1.ClassNo == ClassNo
                          //                    && m1.ExamTerm == ExamTerm
                          //                    && studentSubjectIds.Contains(cs1.ExamSubjectId)
                          //                    && cs1.SubjectType == (byte)SubjectType.Compulsory
                          //                    && (ClassNo == 10 ? (m1.ExamType == (byte)ExamType.HalfYearly) : true)
                          //                    select new { TotalMarks = (m1.Weightage ?? 0) + (ClassNo == 10 ? 0 : m1.PracticalWeightage ?? 0) });
                          //    if (weitages.Any())
                          //        MaxMarks = weitages.Sum(x => x.TotalMarks);
                          //}

                          DataTable rankerListDataTable = ds.RankerListDataTable;
                          DataRow rankerDataRow = rankerListDataTable.NewRow();
                          rankerDataRow["StudentName"] = student.FullName;
                          rankerDataRow["RollNo"] = student.RollNo;
                          rankerDataRow["AdmissionNo"] = student.AdmissionNo;
                          rankerDataRow["HY_Aggregate"] = student.MarksObtained.ToString("#,0.0") + " / " + Convert.ToInt32(MaxMarks);
                          if (MaxMarks > 0)
                              rankerDataRow["HY_Percentage"] = (student.MarksObtained / MaxMarks * 100).ToString("#,0.0");
                          rankerDataRow["HY_Rank"] = rank;
                          rankerListDataTable.Rows.Add(rankerDataRow);
                      });
                      rank++;
                  });

                DataTable headerDataTable = ds.HeaderDataTable;
                DataRow headerRow = headerDataTable.NewRow();
                headerRow["SessionName"] = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First().SessionName;
                headerRow["SectionName"] = section.SectionName;
                headerRow["ClassName"] = section.Class.ClassName2;
                if (ExaminationId > 0)
                    headerRow["ExaminationName"] = dataContext.Examinations.Where(x => x.ExaminationId == ExaminationId).First().ExaminationName;
                else if (ExamTerm == (byte)School.ExamTerm.TermOne)
                    headerRow["ExaminationName"] = "Half Yearly (Term One)";
                else if (ExamTerm == (byte)School.ExamTerm.TermTwo)
                    headerRow["ExaminationName"] = "Annual (Term Two)";
                else
                    headerRow["ExaminationName"] = "Final";
                headerDataTable.Rows.Add(headerRow);

                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.School);
                adapter.Dispose();
                cmd.Dispose();

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/ranker_list.rpt"));
                rp.SetDataSource(ds);
                Stream ms = null;
                if (docType == (byte)DocType.Excel)
                    ms = rp.ExportToStream(ExportFormatType.Excel);
                else
                    ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);

                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                if (docType == (byte)DocType.Excel)
                    Response.ContentType = "application/vnd.ms-excel";
                else
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

        public ActionResult PrintRankerListTermwise(int AcademicSessionId, byte ExamTerm, int SectionId, byte docType)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);

                //Calculation Of max marks
                var section = dataContext.Sections.Where(x => x.SectionId == SectionId).First();
                int ClassNo = section.Class.ClassNo;
                //decimal MaxMarks = 0;
                ExamReportDataset ds = new ExamReportDataset();
                if (ClassNo == 11 && ExamTerm == (byte)School.ExamTerm.TermOne)
                {
                    var marksheetList = LoadExamData.ClassElevenHYMarksheet(dataContext, AcademicSessionId, SectionId);
                    marksheetList.OrderBy(x => x.RollNo).ToList().ForEach(student =>
                    {
                        DataTable rankerListDataTable = ds.RankerListDataTable;
                        DataRow rankerDataRow = rankerListDataTable.NewRow();
                        rankerDataRow["StudentName"] = student.StudentName;
                        rankerDataRow["RollNo"] = student.RollNo;
                        rankerDataRow["AdmissionNo"] = student.AdmissionNo;
                        rankerDataRow["HY_Aggregate"] = student.TotalHYMarksObtained.ToString("#,0.0") + " / " + 250;
                        rankerDataRow["HY_Percentage"] = student.HYPercentage.ToString("N") + "%";
                        rankerDataRow["HY_Rank"] = student.HYRank;
                        rankerListDataTable.Rows.Add(rankerDataRow);
                    });
                }
                else
                {
                    //Calculation Of Marks
                    var rankerList = (from m2 in dataContext.ExamMarksheets
                                      join a1 in dataContext.Admissions
                                      on m2.AdmissionId equals a1.AdmissionId
                                      join s1 in dataContext.Students
                                      on a1.StudentId equals s1.StudentId
                                      where a1.AcademicSessionId == AcademicSessionId
                                      && a1.SectionId == SectionId
                                      select new
                                      {
                                          s1.FullName,
                                          a1.RollNo,
                                          a1.AdmissionId,
                                          s1.AdmissionNo,
                                          MarksObtained = ExamTerm == (byte)School.ExamTerm.TermOne ? m2.HYMarksObtained ?? 0 : m2.ANMarksObtained ?? 0,
                                      }).ToList();
                    int rank = 1;
                    rankerList.GroupBy(x => x.MarksObtained).OrderByDescending(x => x.Key).ToList().ForEach(studentList =>
                    {
                        studentList.ToList().ForEach(student =>
                        {
                            decimal MaxMarks = 0;
                            var studentSubjectIds = LoadExamData.GetStudentSubjectuIds(dataContext, student.AdmissionId);

                            var weitages = (from m1 in dataContext.ExamClassWeightages
                                            join cs1 in dataContext.ExamClassSubjects
                                            on m1.ExamClassSubjectId equals cs1.ExamClassSubjectId
                                            where m1.AcademicSessionId == AcademicSessionId
                                            && m1.ClassNo == ClassNo
                                            && m1.ExamTerm == ExamTerm
                                            && studentSubjectIds.Contains(cs1.ExamSubjectId)
                                            && cs1.SubjectType == (byte)SubjectType.Compulsory
                                            && (ClassNo == 10 ? (m1.ExamType == (byte)ExamType.HalfYearly) : true)
                                            select new { TotalMarks = (m1.Weightage ?? 0) + (ClassNo == 10 ? 0 : m1.PracticalWeightage ?? 0) });
                            if (weitages.Any())
                                MaxMarks = weitages.Sum(x => x.TotalMarks);

                            DataTable rankerListDataTable = ds.RankerListDataTable;
                            DataRow rankerDataRow = rankerListDataTable.NewRow();
                            rankerDataRow["StudentName"] = student.FullName;
                            rankerDataRow["RollNo"] = student.RollNo;
                            rankerDataRow["AdmissionNo"] = student.AdmissionNo;
                            rankerDataRow["HY_Aggregate"] = student.MarksObtained.ToString("#,0.0") + " / " + Convert.ToInt32(MaxMarks);
                            if (MaxMarks > 0)
                                rankerDataRow["HY_Percentage"] = (student.MarksObtained / MaxMarks * 100).ToString("#,0.0");
                            rankerDataRow["HY_Rank"] = rank;
                            rankerListDataTable.Rows.Add(rankerDataRow);
                        });
                        rank++;
                    });
                }

                DataTable headerDataTable = ds.HeaderDataTable;
                DataRow headerRow = headerDataTable.NewRow();
                headerRow["SessionName"] = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First().SessionName;
                headerRow["SectionName"] = section.SectionName;
                headerRow["ClassName"] = section.Class.ClassName2;
                if (ExamTerm == (byte)School.ExamTerm.TermOne)
                    headerRow["ExaminationName"] = "Half Yearly (Term One)";
                else if (ExamTerm == (byte)School.ExamTerm.TermTwo)
                    headerRow["ExaminationName"] = "Annual (Term Two)";
                else
                    headerRow["ExaminationName"] = "Final";
                headerDataTable.Rows.Add(headerRow);

                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.School);
                adapter.Dispose();
                cmd.Dispose();

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/ranker_list.rpt"));
                rp.SetDataSource(ds);
                Stream ms = null;
                if (docType == (byte)DocType.Excel)
                    ms = rp.ExportToStream(ExportFormatType.Excel);
                else
                    ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);

                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                if (docType == (byte)DocType.Excel)
                    Response.ContentType = "application/vnd.ms-excel";
                else
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
        public ActionResult PrintRankerAnalysisList(int AcademicSessionId, int SectionId)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);

                //Calculation Of max marks
                var section = dataContext.Sections.Where(x => x.SectionId == SectionId).First();
                decimal AN_MaxMarks = 0, HY_MaxMarks = 0, FinalMaxMarks = 0;

                var examWeightages = from e1 in dataContext.ExamClassWeightages
                                     where e1.AcademicSessionId == AcademicSessionId
                                     && e1.ClassNo == section.Class.ClassNo
                                     select e1;
                if (examWeightages.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne).Any())
                    HY_MaxMarks = examWeightages.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne).Sum(x => (x.Weightage ?? 0) + (x.PracticalWeightage ?? 0));
                if (examWeightages.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne).Any())
                    AN_MaxMarks = examWeightages.Where(x => x.ExamTerm == (byte)ExamTerm.TermOne).Sum(x => (x.Weightage ?? 0) + (x.PracticalWeightage ?? 0));

                FinalMaxMarks = AN_MaxMarks + HY_MaxMarks;

                //Calculation Of Marks
                var marksDetail = (from m2 in dataContext.ExamMarksheets
                                   join a1 in dataContext.Admissions
                                   on m2.AdmissionId equals a1.AdmissionId
                                   join s1 in dataContext.Students
                                   on a1.StudentId equals s1.StudentId
                                   where a1.AcademicSessionId == AcademicSessionId
                                   && a1.SectionId == SectionId
                                   select new RankerAnalysisModel
                                   {
                                       StudentName = s1.FullName,
                                       RollNo = a1.RollNo,
                                       AdmissionNo = s1.AdmissionNo,
                                       HYMarksObtained = m2.HYMarksObtained ?? 0,
                                       ANMarksObtained = m2.ANMarksObtained ?? 0,
                                       FinalMarksObtained = (m2.HYMarksObtained ?? 0) + (m2.ANMarksObtained ?? 0),
                                       HY_Rank = m2.HYRank,
                                       AN_Rank = m2.ANRank,
                                   }).OrderByDescending(x => x.FinalMarksObtained).ToList();

                int rank = 1;
                //HY Rank Calculation
                marksDetail.GroupBy(x => x.HYMarksObtained).OrderByDescending(x => x.Key).ToList().ForEach(students =>
                {
                    students.ToList().ForEach(student => marksDetail.Find(x => x.AdmissionNo == student.AdmissionNo).HY_Rank = rank);
                    rank++;
                });

                //AN Rank Calculation
                rank = 1;
                marksDetail.GroupBy(x => x.ANMarksObtained).OrderByDescending(x => x.Key).ToList().ForEach(students =>
                {
                    students.ToList().ForEach(student => marksDetail.Find(x => x.AdmissionNo == student.AdmissionNo).AN_Rank = rank);
                    rank++;
                });

                //Final Rank Calculation
                rank = 1;
                marksDetail.GroupBy(x => x.FinalMarksObtained).OrderByDescending(x => x.Key).ToList().ForEach(students =>
                {
                    students.ToList().ForEach(student => marksDetail.Find(x => x.AdmissionNo == student.AdmissionNo).Final_Rank = rank);
                    rank++;
                });


                ExamReportDataset ds = new ExamReportDataset();
                marksDetail.OrderBy(x => x.RollNo).ToList().ForEach(student =>
                  {
                      DataTable rankerListDataTable = ds.RankerListDataTable;
                      DataRow rankerDataRow = rankerListDataTable.NewRow();
                      rankerDataRow["StudentName"] = student.StudentName;
                      rankerDataRow["RollNo"] = student.RollNo;
                      rankerDataRow["AdmissionNo"] = student.AdmissionNo;
                      rankerDataRow["HY_Aggregate"] = student.HYMarksObtained.ToString("#,0.0") + " / " + Convert.ToInt32(HY_MaxMarks);
                      if (HY_MaxMarks > 0)
                          rankerDataRow["HY_Percentage"] = (student.HYMarksObtained / HY_MaxMarks * 100).ToString("#,0.0");
                      rankerDataRow["HY_Rank"] = student.HY_Rank;

                      rankerDataRow["AN_Aggregate"] = student.ANMarksObtained.ToString("#,0.0") + " / " + Convert.ToInt32(AN_MaxMarks);
                      if (HY_MaxMarks > 0)
                          rankerDataRow["AN_Percentage"] = (student.ANMarksObtained / AN_MaxMarks * 100).ToString("#,0.0");
                      rankerDataRow["AN_Rank"] = student.AN_Rank;

                      rankerDataRow["Final_Aggregate"] = student.FinalMarksObtained.ToString("#,0.0") + " / " + Convert.ToInt32(FinalMaxMarks);
                      if (HY_MaxMarks > 0)
                          rankerDataRow["Final_Percentage"] = (student.FinalMarksObtained / FinalMaxMarks * 100).ToString("#,0.0");
                      rankerDataRow["Final_Rank"] = student.Final_Rank;

                      rankerListDataTable.Rows.Add(rankerDataRow);
                      rank++;
                  });

                DataTable headerDataTable = ds.HeaderDataTable;
                DataRow headerRow = headerDataTable.NewRow();
                headerRow["SessionName"] = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First().SessionName;
                headerRow["SectionName"] = section.SectionName;
                headerRow["ClassName"] = section.Class.ClassName2;
                headerDataTable.Rows.Add(headerRow);

                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.School);
                adapter.Dispose();
                cmd.Dispose();

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/ranker_analysis.rpt"));
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

        public ActionResult PrintCorssReport(int AcademicSessionId, byte ExamTerm, int SectionId, int ExaminationId)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                ExamReportDataset ds = new ExamReportDataset();
                var section = (from s1 in dataContext.Sections
                               join c1 in dataContext.Classes
                               on s1.ClassId equals c1.ClassId
                               where s1.SectionId == SectionId
                               select new { s1.SectionName, c1.ClassName2, c1.ClassNo }).First();
                var Examination = dataContext.Examinations.Where(x => x.ExaminationId == ExaminationId).First();
                var admissionList = (from a1 in dataContext.Admissions
                                     join s1 in dataContext.Students
                                     on a1.StudentId equals s1.StudentId
                                     join m1 in dataContext.ExamMarksheets
                                     on a1.AdmissionId equals m1.AdmissionId
                                     where a1.AcademicSessionId == AcademicSessionId
                                     && a1.SectionId == SectionId
                                     orderby a1.RollNo
                                     select new { a1.AdmissionId, a1.RollNo, s1.FullName, s1.AdmissionNo, m1.ExamMarksheetId }).ToList();

                var classSubjects = (from cs1 in dataContext.ExamClassSubjects
                                     join s1 in dataContext.ExamSubjects
                                     on cs1.ExamSubjectId equals s1.ExamSubjectId
                                     join m1 in dataContext.ExamMarksheetDetails
                                     on cs1.ExamClassSubjectId equals m1.ExamClassSubjectId
                                     join m2 in dataContext.ExamMarksheets
                                     on m1.ExamMarksheetId equals m2.ExamMarksheetId
                                     join a1 in dataContext.Admissions
                                     on m2.AdmissionId equals a1.AdmissionId
                                     join c1 in dataContext.ExamClasses
                                     on m1.ExamClassId equals c1.ExamClassId
                                     where a1.AcademicSessionId == AcademicSessionId
                                     && a1.SectionId == SectionId
                                     && c1.ExaminationId == ExaminationId
                                     && c1.ExamTerm == ExamTerm
                                     orderby cs1.ExamClassSubjectId
                                     select new { cs1.ExamClassSubjectId, s1.SubjectName, c1.TheoryMaxMarks, c1.PracticalMaxMarks, c1.ExamClassId }).Distinct().ToList();

                int NoOfSubjects = classSubjects.Count();

                DataTable crossReportSubject = ds.CrossReportSubject;
                DataRow sDataRow = crossReportSubject.NewRow();
                if (NoOfSubjects > 0)
                {
                    var subject = classSubjects[0];
                    sDataRow["SubjectOneName"] = subject.SubjectName.ToUpper();
                    sDataRow["SubjectOneMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                }
                if (NoOfSubjects > 1)
                {
                    var subject = classSubjects[1];
                    sDataRow["SubjectTwoName"] = subject.SubjectName.ToUpper();
                    sDataRow["SubjectTwoMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                }
                if (NoOfSubjects > 2)
                {
                    var subject = classSubjects[2];
                    sDataRow["SubjectThreeName"] = subject.SubjectName.ToUpper();
                    sDataRow["SubjectThreeMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                }
                if (NoOfSubjects > 3)
                {
                    var subject = classSubjects[3];
                    sDataRow["SubjectFourName"] = subject.SubjectName.ToUpper();
                    sDataRow["SubjectFourMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                }
                if (NoOfSubjects > 4)
                {
                    var subject = classSubjects[4];
                    sDataRow["SubjectFiveName"] = subject.SubjectName.ToUpper();
                    sDataRow["SubjectFiveMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                }
                if (NoOfSubjects > 5)
                {
                    var subject = classSubjects[5];
                    sDataRow["SubjectSixName"] = subject.SubjectName.ToUpper();
                    sDataRow["SubjectSixMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                }
                if (NoOfSubjects > 6)
                {
                    var subject = classSubjects[6];
                    sDataRow["SubjectSevenName"] = subject.SubjectName.ToUpper();
                    sDataRow["SubjectSevenMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                }
                crossReportSubject.Rows.Add(sDataRow);

                admissionList.ForEach(admission =>
                {
                    var markList = (from cs1 in classSubjects
                                    join m1 in dataContext.ExamMarksheetDetails.Where(x => x.ExamMarksheetId == admission.ExamMarksheetId)
                                    on cs1.ExamClassId equals m1.ExamClassId into subMarksDetail
                                    from sMarksDetail in subMarksDetail.DefaultIfEmpty()
                                    join w1 in dataContext.ExamClassWeightages.Where(x => x.AcademicSessionId == AcademicSessionId && x.ClassNo == section.ClassNo && x.ExamTerm == ExamTerm && x.ExamType == Examination.ExamType)
                                    on cs1.ExamClassSubjectId equals w1.ExamClassSubjectId into subWeightage
                                    from sWeightage in subWeightage.DefaultIfEmpty()
                                    orderby cs1.ExamClassSubjectId
                                    select new
                                    {
                                        cs1.PracticalMaxMarks,
                                        cs1.TheoryMaxMarks,
                                        sMarksDetail.TheoryMarksObtained,
                                        sMarksDetail.PracticalMarksObtained,
                                        sWeightage.Weightage,
                                        sWeightage.PracticalWeightage
                                    }).ToList();

                    DataTable crossReportDataTable = ds.CrossReportDataTable;
                    DataRow crossReportDataRow = crossReportDataTable.NewRow();
                    crossReportDataRow["RollNo"] = admission.RollNo;
                    crossReportDataRow["AdmissionNo"] = admission.AdmissionNo;
                    crossReportDataRow["StudentName"] = admission.FullName.ToUpper();
                    decimal GrossTotalC = 0;
                    if (NoOfSubjects > 0)
                    {
                        var subject = markList[0];
                        crossReportDataRow["SubjectOneMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                        decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                        crossReportDataRow["SubjectOneMOW"] = MOW;
                        GrossTotalC += MOW;
                    }
                    if (NoOfSubjects > 1)
                    {
                        var subject = markList[1];
                        crossReportDataRow["SubjectTwoMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                        decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                        crossReportDataRow["SubjectTwoMOW"] = MOW;
                        GrossTotalC += MOW;
                    }
                    if (NoOfSubjects > 2)
                    {
                        var subject = markList[2];
                        crossReportDataRow["SubjectThreeMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                        decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                        crossReportDataRow["SubjectThreeMOW"] = MOW;
                        GrossTotalC += MOW;
                    }
                    if (NoOfSubjects > 3)
                    {
                        var subject = markList[3];
                        crossReportDataRow["SubjectFourMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                        decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                        crossReportDataRow["SubjectFourMOW"] = MOW;
                        GrossTotalC += MOW;
                    }
                    if (NoOfSubjects > 4)
                    {
                        var subject = markList[4];
                        crossReportDataRow["SubjectFiveMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                        decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                        crossReportDataRow["SubjectFiveMOW"] = MOW;
                        GrossTotalC += MOW;
                    }
                    if (NoOfSubjects > 5)
                    {
                        var subject = markList[5];
                        crossReportDataRow["SubjectSixMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                        decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                        crossReportDataRow["SubjectSixMOW"] = MOW;
                        GrossTotalC += MOW;
                    }
                    if (NoOfSubjects > 6)
                    {
                        var subject = markList[6];
                        crossReportDataRow["SubjectSevenMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                        decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                        crossReportDataRow["SubjectSevenMOW"] = MOW;
                        GrossTotalC += MOW;
                    }
                    crossReportDataRow["GrossTotal"] = markList.Sum(x => x.TheoryMarksObtained + x.PracticalMarksObtained);
                    crossReportDataRow["GrossTotalC"] = GrossTotalC;
                    crossReportDataTable.Rows.Add(crossReportDataRow);

                });


                DataTable headerDataTable = ds.HeaderDataTable;
                DataRow headerRow = headerDataTable.NewRow();
                headerRow["SessionName"] = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First().SessionName;
                headerRow["SectionName"] = section.SectionName;
                headerRow["ClassName"] = section.ClassName2;
                if (ExaminationId > 0)
                    headerRow["ExaminationName"] = Examination.ExaminationName;
                headerDataTable.Rows.Add(headerRow);

                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.School);
                adapter.Dispose();
                cmd.Dispose();

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/cross_report_examiwise.rpt"));
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

        public ActionResult PrintCorssReportExamTypeWise(int AcademicSessionId, byte ExamTerm, int SectionId)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                ExamReportDataset ds = new ExamReportDataset();
                var section = (from s1 in dataContext.Sections
                               join c1 in dataContext.Classes
                               on s1.ClassId equals c1.ClassId
                               where s1.SectionId == SectionId
                               select new { s1.SectionName, c1.ClassName2, c1.ClassNo }).First();
                var admissionList = (from a1 in dataContext.Admissions
                                     join s1 in dataContext.Students
                                     on a1.StudentId equals s1.StudentId
                                     join m1 in dataContext.ExamMarksheets
                                     on a1.AdmissionId equals m1.AdmissionId
                                     where a1.AcademicSessionId == AcademicSessionId
                                     && a1.SectionId == SectionId
                                     orderby a1.RollNo
                                     select new { a1.AdmissionId, a1.RollNo, s1.FullName, s1.AdmissionNo, m1.ExamMarksheetId }).ToList();

                var classSubjects = (from cs1 in dataContext.ExamClassSubjects
                                     join s1 in dataContext.ExamSubjects
                                     on cs1.ExamSubjectId equals s1.ExamSubjectId
                                     join m1 in dataContext.ExamMarksheetDetails
                                     on cs1.ExamClassSubjectId equals m1.ExamClassSubjectId
                                     join m2 in dataContext.ExamMarksheets
                                     on m1.ExamMarksheetId equals m2.ExamMarksheetId
                                     join a1 in dataContext.Admissions
                                     on m2.AdmissionId equals a1.AdmissionId
                                     join c1 in dataContext.ExamClasses
                                     on m1.ExamClassId equals c1.ExamClassId
                                     where a1.AcademicSessionId == AcademicSessionId
                                     && a1.SectionId == SectionId
                                     && c1.ExamTerm == ExamTerm
                                     orderby cs1.ExamClassSubjectId
                                     select new { cs1.ExamClassSubjectId, s1.SubjectName, c1.TheoryMaxMarks, c1.PracticalMaxMarks, c1.ExamClassId })
                                     .GroupBy(x => new { x.ExamClassSubjectId, x.SubjectName })
                                     .OrderBy(x => x.Key.ExamClassSubjectId).Distinct().ToList();

                int NoOfSubjects = classSubjects.Count();
                decimal FullMarks = LoadExamData.HYFullMarks(section.ClassNo);

                DataTable crossReportSubject = ds.CrossReportSubject;
                DataRow sDataRow = crossReportSubject.NewRow();
                if (NoOfSubjects > 0)
                {
                    var subject = classSubjects[0];
                    sDataRow["SubjectOneName"] = subject.Key.SubjectName.ToUpper();
                }
                if (NoOfSubjects > 1)
                {
                    var subject = classSubjects[1];
                    sDataRow["SubjectTwoName"] = subject.Key.SubjectName.ToUpper();
                }
                if (NoOfSubjects > 2)
                {
                    var subject = classSubjects[2];
                    sDataRow["SubjectThreeName"] = subject.Key.SubjectName.ToUpper();
                }
                if (NoOfSubjects > 3)
                {
                    var subject = classSubjects[3];
                    sDataRow["SubjectFourName"] = subject.Key.SubjectName.ToUpper();
                }
                if (NoOfSubjects > 4)
                {
                    var subject = classSubjects[4];
                    sDataRow["SubjectFiveName"] = subject.Key.SubjectName.ToUpper();
                }
                if (NoOfSubjects > 5)
                {
                    var subject = classSubjects[5];
                    sDataRow["SubjectSixName"] = subject.Key.SubjectName.ToUpper();
                }
                if (NoOfSubjects > 6)
                {
                    var subject = classSubjects[6];
                    sDataRow["SubjectSevenName"] = subject.Key.SubjectName.ToUpper();
                }
                crossReportSubject.Rows.Add(sDataRow);

                admissionList.ForEach(admission =>
                {
                    var examMarksDetails = (from m1 in dataContext.ExamMarksheetDetails
                                            join c1 in dataContext.ExamClasses
                                            on m1.ExamClassId equals c1.ExamClassId
                                            join e1 in dataContext.Examinations
                                            on c1.ExaminationId equals e1.ExaminationId
                                            join m2 in dataContext.ExamMarksheets
                                            on m1.ExamMarksheetId equals m2.ExamMarksheetId
                                            where m2.AdmissionId == admission.AdmissionId
                                            && c1.ExamTerm == ExamTerm
                                            select new { e1.ExamType, m1.TheoryMarksObtained, m1.PracticalMarksObtained, c1.TheoryMaxMarks, c1.PracticalMaxMarks, c1.ExamClassSubjectId })
                                           .GroupBy(x => x.ExamType).ToList();
                    CrossReportModel crossReportModel = new CrossReportModel();
                    crossReportModel.AdmissionNo = admission.AdmissionNo;
                    examMarksDetails.ForEach(examMarks =>
                    {
                        var markList = (from cs1 in classSubjects
                                        join e1 in examMarks
                                        on cs1.Key.ExamClassSubjectId equals e1.ExamClassSubjectId into subMarksDetail
                                        from sMarksDetail in subMarksDetail.DefaultIfEmpty()
                                        join w1 in dataContext.ExamClassWeightages.Where(x => x.AcademicSessionId == AcademicSessionId && x.ClassNo == section.ClassNo && x.ExamTerm == ExamTerm && x.ExamType == examMarks.Key)
                                        on cs1.Key.ExamClassSubjectId equals w1.ExamClassSubjectId into subWeightage
                                        from sWeightage in subWeightage.DefaultIfEmpty()
                                        select new CrossReportModelTwo
                                        {
                                            ExamClassSubjectId = cs1.Key.ExamClassSubjectId,
                                            PracticalMaxMarks = sMarksDetail != null ? sMarksDetail.PracticalMaxMarks : 0,
                                            TheoryMaxMarks = sMarksDetail != null ? sMarksDetail.TheoryMaxMarks : 0,
                                            TheoryMarksObtained = sMarksDetail != null ? sMarksDetail.TheoryMarksObtained : 0,
                                            PracticalMarksObtained = sMarksDetail != null ? sMarksDetail.PracticalMarksObtained : 0,
                                            Weightage = sWeightage != null ? sWeightage.Weightage : 0,
                                            PracticalWeightage = sWeightage != null ? sWeightage.PracticalWeightage : 0,
                                        }).ToList();

                        markList = markList.GroupBy(x => x.ExamClassSubjectId).Select(x => new CrossReportModelTwo
                        {
                            ExamClassSubjectId = x.Key,
                            PracticalMaxMarks = x.Sum(y => y.PracticalMaxMarks ?? 0),
                            TheoryMaxMarks = x.Sum(y => y.TheoryMaxMarks ?? 0),
                            TheoryMarksObtained = x.Sum(y => y.TheoryMarksObtained),
                            PracticalMarksObtained = x.Sum(y => y.PracticalMarksObtained),
                            Weightage = x.First().Weightage,
                            PracticalWeightage = x.First().PracticalWeightage,
                        }).OrderBy(x => x.ExamClassSubjectId).ToList();

                        //var weitages = from w1 in dataContext.ExamClassWeightages
                        //               where w1.ClassNo == section.ClassNo
                        //               && w1.AcademicSessionId == AcademicSessionId
                        //               && w1.ExamTerm == ExamTerm
                        //               && w1.ExamType == examMarks.Key
                        //               se
                        DataTable crossReportDataTable = ds.CrossReportDataTable;
                        DataRow crossReportDataRow = crossReportDataTable.NewRow();
                        crossReportDataRow["RollNo"] = admission.RollNo;
                        crossReportDataRow["AdmissionNo"] = admission.AdmissionNo;
                        crossReportDataRow["AdmissionNo"] = admission.AdmissionNo;
                        crossReportDataRow["StudentName"] = admission.FullName.ToUpper();
                        crossReportDataRow["ExamName"] = Enum.GetName(typeof(ExamTypeShortName), examMarks.Key);
                        if (examMarks.Key == (byte)ExamType.PeriodicTest)
                            crossReportDataRow["ExamNo"] = 1;
                        else if (examMarks.Key == (byte)ExamType.NotebookSubmission)
                            crossReportDataRow["ExamNo"] = 2;
                        else if (examMarks.Key == (byte)ExamType.SubjectEnrichmentActivity)
                            crossReportDataRow["ExamNo"] = 3;
                        else if (examMarks.Key == (byte)ExamType.HalfYearly)
                            crossReportDataRow["ExamNo"] = 4;
                        else
                            crossReportDataRow["ExamNo"] = 5;

                        decimal GrossTotalC = 0;
                        if (NoOfSubjects > 0)
                        {
                            var subject = markList[0];
                            crossReportDataRow["SubjectOneMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                            crossReportDataRow["SubjectOneMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                            decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                            crossReportDataRow["SubjectOneMOW"] = MOW;
                            GrossTotalC += MOW;
                            crossReportModel.SubjectOneMM += Convert.ToInt32(crossReportDataRow["SubjectOneMM"]);
                            crossReportModel.SubjectOneMO += Convert.ToInt32(crossReportDataRow["SubjectOneMO"]);
                            crossReportModel.SubjectOneMOW += Convert.ToInt32(crossReportDataRow["SubjectOneMOW"]);
                        }
                        if (NoOfSubjects > 1)
                        {
                            var subject = markList[1];
                            crossReportDataRow["SubjectTwoMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                            crossReportDataRow["SubjectTwoMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                            decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                            crossReportDataRow["SubjectTwoMOW"] = MOW;
                            GrossTotalC += MOW;

                            crossReportModel.SubjectTwoMM += Convert.ToInt32(crossReportDataRow["SubjectTwoMM"]);
                            crossReportModel.SubjectTwoMO += Convert.ToInt32(crossReportDataRow["SubjectTwoMO"]);
                            crossReportModel.SubjectTwoMOW += Convert.ToInt32(crossReportDataRow["SubjectTwoMOW"]);
                        }
                        if (NoOfSubjects > 2)
                        {
                            var subject = markList[2];
                            crossReportDataRow["SubjectThreeMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                            crossReportDataRow["SubjectThreeMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                            decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                            crossReportDataRow["SubjectThreeMOW"] = MOW;
                            GrossTotalC += MOW;

                            crossReportModel.SubjectThreeMM += Convert.ToInt32(crossReportDataRow["SubjectThreeMM"]);
                            crossReportModel.SubjectThreeMO += Convert.ToInt32(crossReportDataRow["SubjectThreeMO"]);
                            crossReportModel.SubjectThreeMOW += Convert.ToInt32(crossReportDataRow["SubjectThreeMOW"]);
                        }
                        if (NoOfSubjects > 3)
                        {
                            var subject = markList[3];
                            crossReportDataRow["SubjectFourMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                            crossReportDataRow["SubjectFourMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                            decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                            crossReportDataRow["SubjectFourMOW"] = MOW;
                            GrossTotalC += MOW;

                            crossReportModel.SubjectFourMM += Convert.ToInt32(crossReportDataRow["SubjectFourMM"]);
                            crossReportModel.SubjectFourMO += Convert.ToInt32(crossReportDataRow["SubjectFourMO"]);
                            crossReportModel.SubjectFourMOW += Convert.ToInt32(crossReportDataRow["SubjectFourMOW"]);
                        }
                        if (NoOfSubjects > 4)
                        {
                            var subject = markList[4];
                            crossReportDataRow["SubjectFiveMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                            crossReportDataRow["SubjectFiveMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                            decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                            crossReportDataRow["SubjectFiveMOW"] = MOW;
                            GrossTotalC += MOW;

                            crossReportModel.SubjectFiveMM += Convert.ToInt32(crossReportDataRow["SubjectFiveMM"]);
                            crossReportModel.SubjectFiveMO += Convert.ToInt32(crossReportDataRow["SubjectFiveMO"]);
                            crossReportModel.SubjectFiveMOW += Convert.ToInt32(crossReportDataRow["SubjectFiveMOW"]);
                        }
                        if (NoOfSubjects > 5)
                        {
                            var subject = markList[5];
                            crossReportDataRow["SubjectSixMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                            crossReportDataRow["SubjectSixMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                            decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                            crossReportDataRow["SubjectSixMOW"] = MOW;
                            GrossTotalC += MOW;

                            crossReportModel.SubjectSixMM += Convert.ToInt32(crossReportDataRow["SubjectSixMM"]);
                            crossReportModel.SubjectSixMO += Convert.ToInt32(crossReportDataRow["SubjectSixMO"]);
                            crossReportModel.SubjectSixMOW += Convert.ToInt32(crossReportDataRow["SubjectSixMOW"]);
                        }
                        if (NoOfSubjects > 6)
                        {
                            var subject = markList[6];
                            crossReportDataRow["SubjectSevenMM"] = Convert.ToInt32((subject.TheoryMaxMarks ?? 0) + (subject.PracticalMaxMarks ?? 0));
                            crossReportDataRow["SubjectSevenMO"] = subject.TheoryMarksObtained + subject.PracticalMarksObtained;
                            decimal MOW = LoadExamData.CalculateMarksObtainedAfterWeightage(subject.TheoryMarksObtained, subject.TheoryMaxMarks, subject.Weightage, subject.PracticalMarksObtained, subject.PracticalWeightage, subject.PracticalWeightage);
                            crossReportDataRow["SubjectSevenMOW"] = MOW;
                            GrossTotalC += MOW;

                            crossReportModel.SubjectSevenMM += Convert.ToInt32(crossReportDataRow["SubjectSevenMM"]);
                            crossReportModel.SubjectSevenMO += Convert.ToInt32(crossReportDataRow["SubjectSevenMO"]);
                            crossReportModel.SubjectSevenMOW += Convert.ToInt32(crossReportDataRow["SubjectSevenMOW"]);
                        }
                        crossReportDataRow["GrossTotal"] = markList.Sum(x => x.TheoryMarksObtained + x.PracticalMarksObtained);
                        crossReportDataRow["GrossTotalC"] = GrossTotalC;
                        crossReportDataTable.Rows.Add(crossReportDataRow);
                    });

                    DataTable grossDataTable = ds.GrossDataTable;
                    DataRow grossDataRow = grossDataTable.NewRow();
                    grossDataRow["AdmissionNo"] = admission.AdmissionNo;
                    if (NoOfSubjects > 0)
                    {
                        grossDataRow["SubjectOneMM"] = crossReportModel.SubjectOneMM;
                        grossDataRow["SubjectOneMO"] = crossReportModel.SubjectOneMO;
                        grossDataRow["SubjectOneMOW"] = crossReportModel.SubjectOneMOW;
                        grossDataRow["SubjectOneGrade"] = LoadExamData.GetMarksGrade(crossReportModel.SubjectOneMOW, section.ClassNo, FullMarks);
                    }
                    if (NoOfSubjects > 1)
                    {
                        grossDataRow["SubjectTwoMM"] = crossReportModel.SubjectTwoMM;
                        grossDataRow["SubjectTwoMO"] = crossReportModel.SubjectTwoMO;
                        grossDataRow["SubjectTwoMOW"] = crossReportModel.SubjectTwoMOW;
                        grossDataRow["SubjectTwoGrade"] = LoadExamData.GetMarksGrade(crossReportModel.SubjectTwoMOW, section.ClassNo, FullMarks);
                    }
                    if (NoOfSubjects > 2)
                    {
                        grossDataRow["SubjectThreeMM"] = crossReportModel.SubjectThreeMM;
                        grossDataRow["SubjectThreeMO"] = crossReportModel.SubjectThreeMO;
                        grossDataRow["SubjectThreeMOW"] = crossReportModel.SubjectThreeMOW;
                        grossDataRow["SubjectThreeGrade"] = LoadExamData.GetMarksGrade(crossReportModel.SubjectThreeMOW, section.ClassNo, FullMarks);
                    }
                    if (NoOfSubjects > 3)
                    {
                        grossDataRow["SubjectFourMM"] = crossReportModel.SubjectFourMM;
                        grossDataRow["SubjectFourMO"] = crossReportModel.SubjectFourMO;
                        grossDataRow["SubjectFourMOW"] = crossReportModel.SubjectFourMOW;
                        grossDataRow["SubjectFourGrade"] = LoadExamData.GetMarksGrade(crossReportModel.SubjectFourMOW, section.ClassNo, FullMarks);
                    }
                    if (NoOfSubjects > 4)
                    {
                        grossDataRow["SubjectFiveMM"] = crossReportModel.SubjectFiveMM;
                        grossDataRow["SubjectFiveMO"] = crossReportModel.SubjectFiveMO;
                        grossDataRow["SubjectFiveMOW"] = crossReportModel.SubjectFiveMOW;
                        grossDataRow["SubjectFiveGrade"] = LoadExamData.GetMarksGrade(crossReportModel.SubjectFiveMOW, section.ClassNo, FullMarks);
                    }
                    if (NoOfSubjects > 5)
                    {
                        grossDataRow["SubjectSixMM"] = crossReportModel.SubjectSixMM;
                        grossDataRow["SubjectSixMO"] = crossReportModel.SubjectSixMO;
                        grossDataRow["SubjectSixMOW"] = crossReportModel.SubjectSixMOW;
                        grossDataRow["SubjectSixGrade"] = LoadExamData.GetMarksGrade(crossReportModel.SubjectSixMOW, section.ClassNo, FullMarks);
                    }
                    if (NoOfSubjects > 6)
                    {
                        grossDataRow["SubjectSevenMM"] = crossReportModel.SubjectSevenMM;
                        grossDataRow["SubjectSevenMO"] = crossReportModel.SubjectSevenMO;
                        grossDataRow["SubjectSevenMOW"] = crossReportModel.SubjectSevenMOW;
                        grossDataRow["SubjectSevenGrade"] = LoadExamData.GetMarksGrade(crossReportModel.SubjectSevenMOW, section.ClassNo, FullMarks);
                    }
                    grossDataTable.Rows.Add(grossDataRow);
                });


                DataTable headerDataTable = ds.HeaderDataTable;
                DataRow headerRow = headerDataTable.NewRow();
                headerRow["SessionName"] = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First().SessionName;
                headerRow["SectionName"] = section.SectionName;
                headerRow["ClassName"] = section.ClassName2;
                headerRow["TotalMarks"] = 50;
                if (ExamTerm == (byte)School.ExamTerm.TermOne)
                    headerRow["ExaminationName"] = "TERM-1";
                else
                    headerRow["ExaminationName"] = "TERM-2";
                headerDataTable.Rows.Add(headerRow);

                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.School);
                adapter.Dispose();
                cmd.Dispose();

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/cross_report_termiwise.rpt"));
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

        public ActionResult PrintMarksAnalysis(int AcademicSessionId, byte ExamTerm, int SectionId, string CustomRange)
        {
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection connection = new SqlConnection(dataContext.Connection.ConnectionString);
                ExamReportDataset ds = new ExamReportDataset();
                var section = (from s1 in dataContext.Sections
                               join c1 in dataContext.Classes
                               on s1.ClassId equals c1.ClassId
                               where s1.SectionId == SectionId
                               select new { s1.SectionName, c1.ClassName2, c1.ClassNo }).First();

                var classSubjects = (from c1 in dataContext.ExamClassSubjects
                                     join s1 in dataContext.ExamSubjects
                                     on c1.ExamSubjectId equals s1.ExamSubjectId
                                     where c1.ClassNo == section.ClassNo
                                     && c1.ExamSubjectType == (byte)ExamSubjectType.Marks
                                     orderby c1.Position
                                     select new { s1.SubjectName, s1.ExamSubjectId }).ToList();

                DataTable marksAnalysisDataTable = ds.MarksAnalysisDataTable;
                classSubjects.ForEach(classSubject =>
                {
                    DataRow marksDataRow = marksAnalysisDataTable.NewRow();
                    marksDataRow["SubjectName"] = classSubject.SubjectName.ToUpper();
                    List<decimal> HyMarksList = new List<decimal>();
                    if (section.ClassNo < 9)
                    {
                        HyMarksList = (from m1 in dataContext.ExamMarksUptoEights
                                       join m2 in dataContext.ExamMarksheets
                                       on m1.ExamMarksheetId equals m2.ExamMarksheetId
                                       join a1 in dataContext.Admissions
                                       on m2.AdmissionId equals a1.AdmissionId
                                       where a1.AcademicSessionId == AcademicSessionId
                                       && a1.SectionId == SectionId
                                       && m1.ExamSubjectId == classSubject.ExamSubjectId
                                       select m1.HY_MarksObtained ?? 0).ToList();
                    }
                    else if (section.ClassNo == 9 || section.ClassNo == 10)
                    {
                        HyMarksList = (from m1 in dataContext.ExamMarksNineAndTens
                                       join m2 in dataContext.ExamMarksheets
                                       on m1.ExamMarksheetId equals m2.ExamMarksheetId
                                       join a1 in dataContext.Admissions
                                       on m2.AdmissionId equals a1.AdmissionId
                                       where a1.AcademicSessionId == AcademicSessionId
                                       && a1.SectionId == SectionId
                                       && m1.ExamSubjectId == classSubject.ExamSubjectId
                                       select m1.HY_MarksObtained ?? 0).ToList();
                    }
                    else if (section.ClassNo == 11)
                    {
                        HyMarksList = (from m1 in dataContext.ExamMarksElevens
                                       join m2 in dataContext.ExamMarksheets
                                       on m1.ExamMarksheetId equals m2.ExamMarksheetId
                                       join a1 in dataContext.Admissions
                                       on m2.AdmissionId equals a1.AdmissionId
                                       where a1.AcademicSessionId == AcademicSessionId
                                       && a1.SectionId == SectionId
                                       && m1.ExamSubjectId == classSubject.ExamSubjectId
                                       select m1.HY_MarksObtained ?? 0).ToList();
                    }
                    else if (section.ClassNo == 11)
                    {
                        HyMarksList = (from m1 in dataContext.ExamMarksTwelves
                                       join m2 in dataContext.ExamMarksheets
                                       on m1.ExamMarksheetId equals m2.ExamMarksheetId
                                       join a1 in dataContext.Admissions
                                       on m2.AdmissionId equals a1.AdmissionId
                                       where a1.AcademicSessionId == AcademicSessionId
                                       && a1.SectionId == SectionId
                                       && m1.ExamSubjectId == classSubject.ExamSubjectId
                                       select m1.HY_MarksObtained ?? 0).ToList();
                    }

                    if (HyMarksList.Any())
                    {
                        int TotalStudents = HyMarksList.Count();
                        marksDataRow["HighestMarks"] = HyMarksList.Max(x => x);
                        marksDataRow["AverageMarks"] = HyMarksList.Sum(x => x) / TotalStudents;
                        marksDataRow["LowestMarks"] = HyMarksList.Min(x => x);

                        var marksDetails = HyMarksList.Where(x => x >= 90 && x <= 100);
                        marksDataRow["91_100_Count"] = marksDetails.Count();
                        marksDataRow["91_100_Percentage"] = marksDetails.Count() * 100 / TotalStudents;

                        marksDetails = HyMarksList.Where(x => x >= 80 && x < 90);
                        marksDataRow["81_90_Count"] = marksDetails.Count();
                        marksDataRow["81_90_Percentage"] = marksDetails.Count() * 100 / TotalStudents;

                        marksDetails = HyMarksList.Where(x => x >= 70 && x < 80);
                        marksDataRow["71_80_Count"] = marksDetails.Count();
                        marksDataRow["71_80_Percentage"] = marksDetails.Count() * 100 / TotalStudents;

                        marksDetails = HyMarksList.Where(x => x >= 60 && x < 70);
                        marksDataRow["61_70_Count"] = marksDetails.Count();
                        marksDataRow["61_70_Percentage"] = marksDetails.Count() * 100 / TotalStudents;

                        marksDetails = HyMarksList.Where(x => x >= 50 && x < 60);
                        marksDataRow["51_60_Count"] = marksDetails.Count();
                        marksDataRow["51_60_Percentage"] = marksDetails.Count() * 100 / TotalStudents;

                        marksDetails = HyMarksList.Where(x => x >= 40 && x < 50);
                        marksDataRow["41_50_Count"] = marksDetails.Count();
                        marksDataRow["41_50_Percentage"] = marksDetails.Count() * 100 / TotalStudents;

                        marksDetails = HyMarksList.Where(x => x >= 33 && x < 40);
                        marksDataRow["33_40_Count"] = marksDetails.Count();
                        marksDataRow["33_40_Percentage"] = marksDetails.Count() * 100 / TotalStudents;

                        marksDetails = HyMarksList.Where(x => x >= 0 && x < 33);
                        marksDataRow["0_32_Count"] = marksDetails.Count();
                        marksDataRow["0_32_Percentage"] = marksDetails.Count() * 100 / TotalStudents;
                    }
                    marksAnalysisDataTable.Rows.Add(marksDataRow);
                });

                var allMarksUptoEight = (from m2 in dataContext.ExamMarksheets
                                         join m1 in dataContext.ExamMarksUptoEights
                                         on m2.ExamMarksheetId equals m1.ExamMarksheetId into subExamMarks
                                         join a1 in dataContext.Admissions
                                         on m2.AdmissionId equals a1.AdmissionId
                                         where a1.AcademicSessionId == AcademicSessionId
                                         && a1.SectionId == SectionId
                                         select new { HY_MarksObtained = subExamMarks.Any() ? (Convert.ToInt32(m2.HYMarksObtained ?? 0) / subExamMarks.Count()) : 0 }).ToList();
                DataTable rangeAnalysisDataTable = ds.RangeAnalysisDataTable;
                ConstantValue.ExamRangList.ForEach(range =>
                {
                    DataRow rDataRow = rangeAnalysisDataTable.NewRow();
                    rDataRow["Rang"] = range.FromMarks + " - " + (range.ToMarks != 100 ? range.ToMarks - 1 : range.ToMarks);

                    var marksDetails = allMarksUptoEight.Where(x => x.HY_MarksObtained >= range.FromMarks && x.HY_MarksObtained < range.ToMarks);
                    rDataRow["Count"] = marksDetails.Count();
                    rDataRow["Percentage"] = marksDetails.Count() * 100 / allMarksUptoEight.Count();
                    rangeAnalysisDataTable.Rows.Add(rDataRow);
                });

                if (CustomRange != "0")
                {
                    DataTable customAnalysisDataTable = ds.CustomAnalysisDataTable;
                    string[] CustomRangeList = CustomRange.Split('N');
                    foreach (var ranges in CustomRangeList)
                    {
                        string[] range = ranges.Split('_');
                        decimal FromMarks = Convert.ToInt32(range[0]), ToMarks = Convert.ToInt32(range[1]);
                        DataRow rDataRow = customAnalysisDataTable.NewRow();
                        rDataRow["Rang"] = FromMarks + " - " + ToMarks;

                        var marksDetails = allMarksUptoEight.Where(x => x.HY_MarksObtained >= FromMarks && x.HY_MarksObtained <= ToMarks);
                        rDataRow["Count"] = marksDetails.Count();
                        rDataRow["Percentage"] = marksDetails.Count() * 100 / allMarksUptoEight.Count();
                        customAnalysisDataTable.Rows.Add(rDataRow);

                    }
                }

                DataTable headerDataTable = ds.HeaderDataTable;
                DataRow headerRow = headerDataTable.NewRow();
                headerRow["SessionName"] = dataContext.AcademicSessions.Where(x => x.AcademicSessionId == AcademicSessionId).First().SessionName;
                headerRow["SectionName"] = section.SectionName;
                headerRow["ClassName"] = section.ClassName2;
                headerRow["TotalMarks"] = 50;
                if (ExamTerm == (byte)School.ExamTerm.TermOne)
                    headerRow["ExaminationName"] = "Half Yearly";
                else
                    headerRow["ExaminationName"] = "Annual";
                headerDataTable.Rows.Add(headerRow);

                SqlCommand cmd = new SqlCommand("select * from School", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.School);
                adapter.Dispose();
                cmd.Dispose();

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/reports/marks_analysis_termiwise.rpt"));
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

        public ActionResult TransportReportForAllStudent(int sessionId)
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
                                select cl.ClassId).Distinct();

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
                                  select sc.SectionId).Distinct();

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
                cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1 and a.AcademicSessionId=" + sessionId + " order by cl.ClassId asc", connection);
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
                throw new Exception(ex.Message);
            }
            return View();
        }
        public ActionResult TrasnportReportGroupWise(int transportGroupId, int sessionId)
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
                                select cl.ClassId).Distinct();

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
                                  select sc.SectionId).Distinct();

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
                    cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1 and st.TransportGroupId=" + transportGroupId + " and a.AcademicSessionId=" + sessionId + " order by cl.ClassId asc", connection);
                else
                    cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1 and a.AcademicSessionId=" + sessionId + " order by cl.ClassId asc", connection);
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
                throw new Exception(ex.Message);
                // MessageBox.Show(this, ex.Message);
            }
            return View();
        }
        public ActionResult TransportReportBusWise(int busId, int sessionId)
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
                                select cl.ClassId).Distinct();

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
                                  select sc.SectionId).Distinct();

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
                    cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1 and st.BusId=" + busId + " and a.AcademicSessionId=" + sessionId + " order by cl.ClassId asc", connection);
                else
                    cmd = new SqlCommand("select st.* from StudentTransport st join Student s on st.StudentId=s.StudentId join Admission a on s.StudentId=a.StudentId join Section sc on a.SectionId=sc.SectionId join Class cl on sc.ClassId=cl.ClassId where a.AdmissionStatus=1 and st.Status=1  and a.AcademicSessionId=" + sessionId + " order by cl.ClassId asc", connection);
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
                                        ClassStreamTypeId = m1.ClassStreamTypeId ?? 0,
                                        SubjectOptionOne = m1.SubjectOptionOne ?? 0,
                                        SubjectOptionTwo = m1.SubjectOptionTwo ?? 0
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

                DataTable ClassStreamTypeTable = new DataTable("ClassStreamType");
                cmd = new SqlCommand("select * from ClassStreamType where ClassStreamTypeId=" + registration.ClassStreamTypeId, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(ClassStreamTypeTable);
                ds.Tables.Add(ClassStreamTypeTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable SubjectOptionTable = new DataTable("SubjectOption");
                cmd = new SqlCommand("select * from SubjectOption where SubjectOptionId=" + registration.SubjectOptionOne, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(SubjectOptionTable);
                ds.Tables.Add(SubjectOptionTable);
                dataAdapter.Dispose();
                cmd.Dispose();

                DataTable SubjectOptionTwoTable = new DataTable("Command_2");
                cmd = new SqlCommand("select * from SubjectOption where SubjectOptionId=" + registration.SubjectOptionTwo, connection);
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(SubjectOptionTwoTable);
                ds.Tables.Add(SubjectOptionTwoTable);
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
                cmd = new SqlCommand("select distinct * from Class where ClassNo='" + registration.ClassNo + "'", connection);
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