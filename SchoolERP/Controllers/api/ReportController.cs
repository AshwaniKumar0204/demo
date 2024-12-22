using School;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SchoolERP.Models;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using System.Web;
using System.IO;
using System.Web.UI;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/report")]
    public class ReportController : ApiController
    {
        [HttpPost]
        [Route("DailyCollectionReport")]
        public ExpandoObject DailyCollectionReport(DCRRequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                List<int> feeHeadIds = requestModel.FeeHeadList.Where(x => x.IsSelected).Select(x => x.FeeHeadId).ToList();

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection con = new SqlConnection(dataContext.Connection.ConnectionString);

                SqlCommand cmd = new SqlCommand("DailyCollectionReport", con);
                cmd.Parameters.Add(new SqlParameter("@fromDate", requestModel.FromDate.ToString("MM/dd/yyyy")));
                cmd.Parameters.Add(new SqlParameter("@toDate", requestModel.ToDate.AddDays(1).ToString("MM/dd/yyyy")));
                cmd.Parameters.Add(new SqlParameter("@paymentMode", requestModel.PaymentMode));
                cmd.Parameters.Add(new SqlParameter("@createdBy", requestModel.LoginId));
                string xmlParams = StringUtils.ConvertListToGuIXml(feeHeadIds);
                cmd.Parameters.Add(new SqlParameter("@feeHeads", xmlParams));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Remove("CreatedOn");

                DailyCollectionReportModel dailyCollectionReport = new DailyCollectionReportModel
                {
                    TableFooterList = new List<DynamicModel>(),
                    TableItemList = new List<DynamicModel>(),
                    TableHeaderList = new List<DynamicModel>()
                };


                //Table Header
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    string columnName = dt.Columns[i].ColumnName;
                    string[] name = columnName.Split(' ');
                    string colName = string.Empty;
                    if (name.Count() > 1)
                    {
                        if (name[0].Length > 4)
                            colName = name[0].Substring(0, 4) + ".";
                        else
                            colName = name[0];

                        colName = colName + " " + name[1];
                    }
                    else
                        colName = columnName;
                    dailyCollectionReport.TableHeaderList.Add(new DynamicModel { Key = i, Value = colName.Replace("ClassName", "Course").Replace("SectionName", "Branch") });
                }

                //Table body
                foreach (DataRow dr in dt.Rows)
                {
                    DynamicModel tableItem = new DynamicModel
                    {
                        DynamicList = new List<DynamicModel>()
                    };
                    for (int i = 0; i < dt.Columns.Count; ++i)
                    {
                        dynamic itemName = dr[i].ToString();
                        if (i == 6)
                        {
                            if (dt.Columns[i].DataType == typeof(DateTime))
                            {
                                itemName = Convert.ToDateTime(itemName).ToString("dd/MM/yyyy");
                            }
                        }
                        else if (i == 8)
                        {
                            itemName = Enum.GetName(typeof(PaymentMode), Convert.ToByte(itemName));
                        }
                        else if (i > 6)
                        {
                            if (dt.Columns[i].DataType == typeof(decimal))
                            {
                                itemName = Convert.ToDecimal(itemName).ToString("N");
                            }
                        }
                        tableItem.DynamicList.Add(new DynamicModel { Key = i, Value = itemName });
                    }

                    dailyCollectionReport.TableItemList.Add(tableItem);
                }


                //Table Footer
                if (dailyCollectionReport.TableItemList.Any())
                {

                    int j = 0;
                    foreach (DataColumn c in dt.Columns)
                    {
                        if (j == 0)
                        {
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = "Total" });
                        }
                        else if (j > 10)
                        {
                            decimal total = 0;
                            dailyCollectionReport.TableItemList.ForEach(x => total += Convert.ToDecimal(x.DynamicList[j].Value));
                            //decimal total = Convert.ToDecimal(dt.Compute("Sum([" + c.ColumnName + "])", ""));
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = total.ToString("N"), IsHide = total == 0 ? true : false });
                            if (total == 0)
                            {
                                dailyCollectionReport.TableItemList.ForEach(x => x.DynamicList[j].IsHide = true);
                                dailyCollectionReport.TableHeaderList[j].IsHide = true;
                            }
                        }
                        else
                        {
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = "" });
                        }
                        j++;
                    }
                }
                response.DailyCollectionReport = dailyCollectionReport;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("DailyConcessionReport")]
        public ExpandoObject DailyConcessionReport(DCRRequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                List<int> feeHeadIds = requestModel.FeeHeadList.Where(x => x.IsSelected).Select(x => x.FeeHeadId).ToList();

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection con = new SqlConnection(dataContext.Connection.ConnectionString);

                SqlCommand cmd = new SqlCommand("DailyConcessionReport", con);
                cmd.Parameters.Add(new SqlParameter("@fromDate", requestModel.FromDate.ToString("MM/dd/yyyy")));
                cmd.Parameters.Add(new SqlParameter("@toDate", requestModel.ToDate.AddDays(1).ToString("MM/dd/yyyy")));
                cmd.Parameters.Add(new SqlParameter("@paymentMode", requestModel.PaymentMode));
                cmd.Parameters.Add(new SqlParameter("@createdBy", requestModel.LoginId));
                string xmlParams = StringUtils.ConvertListToGuIXml(feeHeadIds);
                cmd.Parameters.Add(new SqlParameter("@feeHeads", xmlParams));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Remove("CreatedOn");

                DailyCollectionReportModel dailyCollectionReport = new DailyCollectionReportModel
                {
                    TableFooterList = new List<DynamicModel>(),
                    TableItemList = new List<DynamicModel>(),
                    TableHeaderList = new List<DynamicModel>()
                };


                //Table Header
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    string columnName = dt.Columns[i].ColumnName;
                    string[] name = columnName.Split(' ');
                    string colName = string.Empty;
                    if (name.Count() > 1)
                    {
                        if (name[0].Length > 4)
                            colName = name[0].Substring(0, 4) + ".";
                        else
                            colName = name[0];

                        colName = colName + " " + name[1];
                    }
                    else
                        colName = columnName;
                    dailyCollectionReport.TableHeaderList.Add(new DynamicModel { Key = i, Value = colName.Replace("ClassName", "Course").Replace("SectionName", "Branch") });
                }

                //Table body
                foreach (DataRow dr in dt.Rows)
                {
                    DynamicModel tableItem = new DynamicModel
                    {
                        DynamicList = new List<DynamicModel>()
                    };
                    for (int i = 0; i < dt.Columns.Count; ++i)
                    {
                        dynamic itemName = dr[i].ToString();
                        if (i == 6)
                        {
                            if (dt.Columns[i].DataType == typeof(DateTime))
                            {
                                itemName = Convert.ToDateTime(itemName).ToString("dd/MM/yyyy");
                            }
                        }
                        else if (i == 8)
                        {
                            itemName = Enum.GetName(typeof(PaymentMode), Convert.ToByte(itemName));
                        }
                        else if (i > 6)
                        {
                            if (dt.Columns[i].DataType == typeof(decimal))
                            {
                                itemName = Convert.ToDecimal(itemName).ToString("N");
                            }
                        }
                        tableItem.DynamicList.Add(new DynamicModel { Key = i, Value = itemName });
                    }

                    dailyCollectionReport.TableItemList.Add(tableItem);
                }


                //Table Footer
                if (dailyCollectionReport.TableItemList.Any())
                {

                    int j = 0;
                    foreach (DataColumn c in dt.Columns)
                    {
                        if (j == 0)
                        {
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = "Total" });
                        }
                        else if (j > 10)
                        {
                            decimal total = 0;
                            dailyCollectionReport.TableItemList.ForEach(x => total += Convert.ToDecimal(x.DynamicList[j].Value));
                            //decimal total = Convert.ToDecimal(dt.Compute("Sum([" + c.ColumnName + "])", ""));
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = total.ToString("N"), IsHide = total == 0 ? true : false });
                            if (total == 0)
                            {
                                dailyCollectionReport.TableItemList.ForEach(x => x.DynamicList[j].IsHide = true);
                                dailyCollectionReport.TableHeaderList[j].IsHide = true;
                            }
                        }
                        else
                        {
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = "" });
                        }
                        j++;
                    }
                }
                response.DailyConcessionReport = dailyCollectionReport;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("DailyCollectionReportStudentwise2")]
        public ExpandoObject DailyCollectionReportStudentwise2(DCRRequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                List<int> feeHeadIds = requestModel.FeeHeadList.Where(x => x.IsSelected).Select(x => x.FeeHeadId).ToList();

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection con = new SqlConnection(dataContext.Connection.ConnectionString);

                SqlCommand cmd = new SqlCommand("DailyCollectionReportStudentwise2", con);
                //cmd.Parameters.Add(new SqlParameter("@fromDate", requestModel.FromDate.ToString("MM/dd/yyyy")));
                //cmd.Parameters.Add(new SqlParameter("@toDate", requestModel.ToDate.AddDays(1).ToString("MM/dd/yyyy")));
                cmd.Parameters.Add(new SqlParameter("@paymentMode", requestModel.PaymentMode));
                cmd.Parameters.Add(new SqlParameter("@classId", requestModel.ClassId));
                cmd.Parameters.Add(new SqlParameter("@sectionId", requestModel.SectionId));
                cmd.Parameters.Add(new SqlParameter("@createdBy", requestModel.LoginId));
                string xmlParams = StringUtils.ConvertListToGuIXml(feeHeadIds);
                cmd.Parameters.Add(new SqlParameter("@feeHeads", xmlParams));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Remove("CreatedOn");

                DailyCollectionReportModel dailyCollectionReport = new DailyCollectionReportModel
                {
                    TableFooterList = new List<DynamicModel>(),
                    TableItemList = new List<DynamicModel>(),
                    TableHeaderList = new List<DynamicModel>()
                };


                //Table Header
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    string columnName = dt.Columns[i].ColumnName;
                    string[] name = columnName.Split(' ');
                    string colName = string.Empty;
                    if (name.Count() > 1)
                    {
                        //if (name[0].Length > 3)
                        //    colName = name[0].Substring(0, 4) + ".";
                        //else
                        colName = name[0];
                        colName = colName + " " + name[1];
                    }
                    else
                        colName = columnName;
                    dailyCollectionReport.TableHeaderList.Add(new DynamicModel { Key = i, Value = colName.Replace("ClassName", "Course").Replace("SectionName", "Branch") });
                }

                //Table body
                foreach (DataRow dr in dt.Rows)
                {
                    DynamicModel tableItem = new DynamicModel
                    {
                        DynamicList = new List<DynamicModel>()
                    };
                    string AdmissionNo = dt.Columns[1].ToString();
                    string ClassName = dt.Columns[3].ToString();
                    string SectionName = dt.Columns[4].ToString();

                    for (int i = 0; i < dt.Columns.Count; ++i)
                    {
                        dynamic itemName = dr[i].ToString(), itemName2 = "";
                        if (i == 5)
                        {
                            if (dt.Columns[i].DataType == typeof(DateTime))
                            {
                                itemName = Convert.ToDateTime(itemName).ToString("dd/MM/yyyy");
                            }
                        }
                        else if (i == 7)
                        {
                            itemName = Enum.GetName(typeof(PaymentMode), Convert.ToByte(itemName));
                        }
                        else if (i > 5)
                        {

                            if (dt.Columns[i].DataType == typeof(decimal))
                            {
                                int FeeHeadId = Convert.ToInt32(dailyCollectionReport.TableHeaderList[i]);
                                itemName = Convert.ToDecimal(itemName).ToString("N");

                            }
                        }
                        tableItem.DynamicList.Add(new DynamicModel { Key = i, Value = itemName });
                    }

                    dailyCollectionReport.TableItemList.Add(tableItem);
                }


                //Table Footer
                if (dailyCollectionReport.TableItemList.Any())
                {

                    int j = 0;
                    foreach (DataColumn c in dt.Columns)
                    {
                        if (j == 0)
                        {
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = "Total" });
                        }
                        else if (j > 9)
                        {
                            decimal total = 0;
                            dailyCollectionReport.TableItemList.ForEach(x => total += Convert.ToDecimal(x.DynamicList[j].Value));
                            //decimal total = Convert.ToDecimal(dt.Compute("Sum([" + c.ColumnName + "])", ""));
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = total.ToString("N"), IsHide = total == 0 ? true : false });
                            if (total == 0)
                            {
                                dailyCollectionReport.TableItemList.ForEach(x => x.DynamicList[j].IsHide = true);
                                dailyCollectionReport.TableHeaderList[j].IsHide = true;
                            }
                        }
                        else
                        {
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = "" });
                        }
                        j++;
                    }
                }
                response.DailyCollectionReport = dailyCollectionReport;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("DailyCollectionReportStudentwise")]
        public ExpandoObject DailyCollectionReportStudentwise(DCRRequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                List<int> feeHeadIds = requestModel.FeeHeadList.Where(x => x.IsSelected).Select(x => x.FeeHeadId).ToList();

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection con = new SqlConnection(dataContext.Connection.ConnectionString);

                SqlCommand cmd = new SqlCommand("DailyCollectionReportStudentwise", con);
                //cmd.Parameters.Add(new SqlParameter("@fromDate", requestModel.FromDate.ToString("MM/dd/yyyy")));
                //cmd.Parameters.Add(new SqlParameter("@toDate", requestModel.ToDate.AddDays(1).ToString("MM/dd/yyyy")));
                cmd.Parameters.Add(new SqlParameter("@paymentMode", requestModel.PaymentMode));
                cmd.Parameters.Add(new SqlParameter("@classId", requestModel.ClassId));
                cmd.Parameters.Add(new SqlParameter("@sectionId", requestModel.SectionId));
                cmd.Parameters.Add(new SqlParameter("@createdBy", requestModel.LoginId));
                string xmlParams = StringUtils.ConvertListToGuIXml(feeHeadIds);
                cmd.Parameters.Add(new SqlParameter("@feeHeads", xmlParams));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //dt.Columns.Remove("CreatedOn");

                DailyCollectionReportModel dailyCollectionReport = new DailyCollectionReportModel
                {
                    TableFooterList = new List<DynamicModel>(),
                    TableItemList = new List<DynamicModel>(),
                    TableHeaderList = new List<DynamicModel>()
                };

                //Table Header
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    string columnName = dt.Columns[i].ColumnName;
                    string[] name = columnName.Split(' ');
                    string colName = string.Empty;
                    if (name.Count() > 1)
                    {
                        //if (name[0].Length > 3)
                        //    colName = name[0].Substring(0, 4) + ".";
                        //else
                        colName = name[0];
                        colName = colName + " " + name[1];
                    }
                    else
                        colName = columnName;
                    dailyCollectionReport.TableHeaderList.Add(new DynamicModel { Key = i, Value = colName.Replace("ClassName", "Course").Replace("SectionName", "Branch") });
                }

                //Table body
                foreach (DataRow dr in dt.Rows)
                {
                    DynamicModel tableItem = new DynamicModel
                    {
                        DynamicList = new List<DynamicModel>()
                    };
                    for (int i = 0; i < dt.Columns.Count; ++i)
                    {
                        dynamic itemName = dr[i].ToString();
                        if (i > 5)
                        {
                            if (dt.Columns[i].DataType == typeof(decimal))
                            {

                                itemName = Convert.ToDecimal(itemName).ToString("N");
                            }
                        }
                        tableItem.DynamicList.Add(new DynamicModel { Key = i, Value = itemName });
                    }

                    dailyCollectionReport.TableItemList.Add(tableItem);
                }


                //Table Footer
                if (dailyCollectionReport.TableItemList.Any())
                {

                    int j = 0;
                    foreach (DataColumn c in dt.Columns)
                    {
                        if (j > 5)
                        {
                            decimal total = 0;
                            dailyCollectionReport.TableItemList.ForEach(x => total += Convert.ToDecimal(x.DynamicList[j].Value));
                            //decimal total = Convert.ToDecimal(dt.Compute("Sum([" + c.ColumnName + "])", ""));
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = total.ToString("N"), IsHide = total == 0 ? true : false });
                            if (total == 0)
                            {
                                dailyCollectionReport.TableItemList.ForEach(x => x.DynamicList[j].IsHide = true);
                                dailyCollectionReport.TableHeaderList[j].IsHide = true;
                            }
                        }
                        else
                        {
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = "" });
                        }
                        j++;
                    }
                }

                response.DailyCollectionReport = dailyCollectionReport;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }




        [HttpPost]
        [Route("DailyConcessionReportStudentwise")]
        public ExpandoObject DailyConcessionReportStudentwise(DCRRequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                List<int> feeHeadIds = requestModel.FeeHeadList.Where(x => x.IsSelected).Select(x => x.FeeHeadId).ToList();

                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection con = new SqlConnection(dataContext.Connection.ConnectionString);

                SqlCommand cmd = new SqlCommand("DailyConcessionReportStudentwise", con);
                //cmd.Parameters.Add(new SqlParameter("@fromDate", requestModel.FromDate.ToString("MM/dd/yyyy")));
                //cmd.Parameters.Add(new SqlParameter("@toDate", requestModel.ToDate.AddDays(1).ToString("MM/dd/yyyy")));
                cmd.Parameters.Add(new SqlParameter("@paymentMode", requestModel.PaymentMode));
                cmd.Parameters.Add(new SqlParameter("@classId", requestModel.ClassId));
                cmd.Parameters.Add(new SqlParameter("@sectionId", requestModel.SectionId));
                cmd.Parameters.Add(new SqlParameter("@createdBy", requestModel.LoginId));
                string xmlParams = StringUtils.ConvertListToGuIXml(feeHeadIds);
                cmd.Parameters.Add(new SqlParameter("@feeHeads", xmlParams));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //dt.Columns.Remove("CreatedOn");

                DailyCollectionReportModel dailyCollectionReport = new DailyCollectionReportModel
                {
                    TableFooterList = new List<DynamicModel>(),
                    TableItemList = new List<DynamicModel>(),
                    TableHeaderList = new List<DynamicModel>()
                };

                //Table Header
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    string columnName = dt.Columns[i].ColumnName;
                    string[] name = columnName.Split(' ');
                    string colName = string.Empty;
                    if (name.Count() > 1)
                    {
                        //if (name[0].Length > 3)
                        //    colName = name[0].Substring(0, 4) + ".";
                        //else
                        colName = name[0];
                        colName = colName + " " + name[1];
                    }
                    else
                        colName = columnName;
                    dailyCollectionReport.TableHeaderList.Add(new DynamicModel { Key = i, Value = colName.Replace("ClassName", "Course").Replace("SectionName", "Branch") });
                }

                //Table body
                foreach (DataRow dr in dt.Rows)
                {
                    DynamicModel tableItem = new DynamicModel
                    {
                        DynamicList = new List<DynamicModel>()
                    };
                    for (int i = 0; i < dt.Columns.Count; ++i)
                    {
                        dynamic itemName = dr[i].ToString();
                        if (i > 5)
                        {
                            if (dt.Columns[i].DataType == typeof(decimal))
                            {

                                itemName = Convert.ToDecimal(itemName).ToString("N");
                            }
                        }
                        tableItem.DynamicList.Add(new DynamicModel { Key = i, Value = itemName });
                    }

                    dailyCollectionReport.TableItemList.Add(tableItem);
                }


                //Table Footer
                if (dailyCollectionReport.TableItemList.Any())
                {

                    int j = 0;
                    foreach (DataColumn c in dt.Columns)
                    {
                        if (j > 5)
                        {
                            decimal total = 0;
                            dailyCollectionReport.TableItemList.ForEach(x => total += Convert.ToDecimal(x.DynamicList[j].Value));
                            //decimal total = Convert.ToDecimal(dt.Compute("Sum([" + c.ColumnName + "])", ""));
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = total.ToString("N"), IsHide = total == 0 ? true : false });
                            if (total == 0)
                            {
                                dailyCollectionReport.TableItemList.ForEach(x => x.DynamicList[j].IsHide = true);
                                dailyCollectionReport.TableHeaderList[j].IsHide = true;
                            }
                        }
                        else
                        {
                            dailyCollectionReport.TableFooterList.Add(new DynamicModel { Key = j, Value = "" });
                        }
                        j++;
                    }
                }

                response.DailyConcessionReport = dailyCollectionReport;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }




        [HttpPost]
        [Route("FeeDueReport")]
        public ExpandoObject FeeDueReport(ReportModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                //var ids = dataContext.StudentConcessions.Select(x => x.AdmissionId).Distinct();
                var list = (from a1 in dataContext.Admissions
                            join s1 in dataContext.Students
                            on a1.StudentId equals s1.StudentId
                            join s2 in dataContext.Sections
                            on a1.SectionId equals s2.SectionId
                            join c1 in dataContext.Classes
                            on s2.ClassId equals c1.ClassId
                            where a1.AcademicSessionId == model.AcademicSessionId
                            && (model.SectionId == 0 || a1.SectionId == model.SectionId)
                            // && a1.AdmissionStatus == (byte)AdmissionStatus.Active
                            && s1.StudentStatus != (byte)StudentStatus.Left
                            // && ids.Contains(a1.AdmissionId)
                            //where a1.AdmissionId == 10952
                            select new
                            {
                                s1.AdmissionNo,
                                s1.FatherName,
                                s1.FullName,
                                a1.RollNo,
                                s1.MobileNo,
                                s2.SectionName,
                                c1.ClassName2,
                                c1.ClassName,
                                s1.MotherName,
                                s1.CorrespondenceAddressLine1,
                                s1.PermanentAddressLine1,
                                DueFeeDetail = FeeData.GetDueDetails(dataContext, a1.AdmissionId, model.UpToSemester, false, model.UpToYear, model.UptoMonthNo)
                            }).ToList();
                response.AdmissionList = list.Where(x => x.DueFeeDetail.TotalDues > 0).ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        //[HttpGet]
        //[Route("UpdateStudentCharge2")]
        //public ExpandoObject UpdateStudentCharge2(int AcademicSessionId)
        //{
        //    dynamic response = new ExpandoObject();
        //    try
        //    {
        //        SchoolDataContext dataContext = new SchoolDataContext();
        //        dataContext.StudentCharge2s.Where(x=>x.AdmissionId== 12116).ToList().ForEach(ch =>
        //        {
        //            var charge = dataContext.StudentCharge2s.Where(x => x.StudentChargeId == ch.StudentChargeId).First();
        //            charge.FineAmount = FeeData.GetDueDetails(dataContext, ch.AdmissionId, 3, false).TotalDues;
        //            dataContext.SubmitChanges();
        //        });
        //        response.Message = ConstantData.SuccessMessage;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //    }
        //    return response;
        //}

        [HttpGet]
        [Route("AllFeeDueReport")]
        public ExpandoObject AllFeeDueReport(int SectionId, int UptoMonthNo, int AcademicSessionId)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Admissions
                            join s1 in dataContext.Students
                            on a1.StudentId equals s1.StudentId
                            join s2 in dataContext.Sections
                            on a1.SectionId equals s2.SectionId
                            join c1 in dataContext.Classes
                            on s2.ClassId equals c1.ClassId
                            where a1.AcademicSessionId == AcademicSessionId
                            && (SectionId == 0 || a1.SectionId == SectionId)
                            //&& a1.AdmissionStatus == (byte)AdmissionStatus.Active
                            && s1.StudentStatus != (byte)StudentStatus.Left
                            select new
                            {
                                s1.AdmissionNo,
                                s1.FatherName,
                                s1.FullName,
                                a1.RollNo,
                                s1.MobileNo,
                                s2.SectionName,
                                c1.ClassName2,
                                c1.ClassName,
                                s1.MotherName,
                                s1.CorrespondenceAddressLine1,
                                s1.PermanentAddressLine1,
                                DueFeeDetail = FeeData.AllDueDetails(dataContext, a1.AdmissionId, UptoMonthNo)
                            }).ToList();
                response.AdmissionList = list.Where(x => x.DueFeeDetail.TotalDues > 0).ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("TuitionFeeDueList")]
        public ExpandoObject TuitionFeeDueList(int SectionId, int UptoMonthNo, int AcademicSessionId, int UptoYear, int UpToMonthDataNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Admissions
                            join s1 in dataContext.Students
                            on a1.StudentId equals s1.StudentId
                            where a1.AcademicSessionId == AcademicSessionId
                            && a1.SectionId == SectionId
                            && s1.StudentStatus == (byte)StudentStatus.Active
                            //&& a1.AdmissionStatus == (byte)AdmissionStatus.Active
                            select new
                            {
                                s1.AdmissionNo,
                                s1.FullName,
                                a1.RollNo,
                                s1.MobileNo,
                                DueFeeDetail = FeeData.GetDueDetails(dataContext, a1.AdmissionId, UptoMonthNo, true, UptoYear, UpToMonthDataNo)
                            }).ToList();
                response.AdmissionList = list.Where(x => x.DueFeeDetail.TotalDues2 > 0).ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("FeeDueReport")]
        public ExpandoObject FeeDueReport(int AdmissionId, int UptoMonthNo, int UpToYear, int UpToMonthDataNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                if (UptoMonthNo == 0)
                    UptoMonthNo = DateTime.Now.Month;
                var FeeDueReport = (from a1 in dataContext.Admissions
                                    join s1 in dataContext.Students
                                    on a1.StudentId equals s1.StudentId
                                    where a1.AdmissionId == AdmissionId
                                    select new
                                    {
                                        s1.AdmissionNo,
                                        s1.FullName,
                                        a1.RollNo,
                                        s1.MobileNo,
                                        DueFeeDetail = FeeData.GetDueDetails(dataContext, a1.AdmissionId, UptoMonthNo, false, UpToYear, UpToMonthDataNo)
                                    }).First();
                response.FeeDueReport = FeeDueReport;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("FeeDueReport2")]
        public ExpandoObject FeeDueReport2(ReportModel model)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Admissions
                            join s1 in dataContext.Students
                            on a1.StudentId equals s1.StudentId
                            where a1.AcademicSessionId == model.AcademicSessionId
                            && a1.SectionId == model.SectionId
                            // && a1.AdmissionStatus == (byte)AdmissionStatus.Active
                            && s1.StudentStatus != (byte)StudentStatus.Left
                            select new
                            {
                                s1.AdmissionNo,
                                s1.FullName,
                                a1.RollNo,
                                s1.MobileNo,
                                DueFeeDetail = FeeData.GetDueDetails2(dataContext, a1.AdmissionId, model.UpToSemester, model.UpToYear, model.UptoMonthNo)
                            }).ToList();
                response.AdmissionList = list.Where(x => x.DueFeeDetail.TotalDues > 0).ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("FeeDueReportExcel")]
        public ExpandoObject FeeDueReportExcel(int SectionId, int UptoMonthNo, int AcademicSessionId, int UpToYear, int UpToMonthDataNo)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = (from a1 in dataContext.Admissions
                            join s1 in dataContext.Students
                            on a1.StudentId equals s1.StudentId
                            join s2 in dataContext.Sections
                            on a1.SectionId equals s2.SectionId
                            join c1 in dataContext.Classes
                            on s2.ClassId equals c1.ClassId
                            where a1.AcademicSessionId == AcademicSessionId
                            && (SectionId == 0 || a1.SectionId == SectionId)
                            // && a1.AdmissionStatus == (byte)AdmissionStatus.Active
                            && s1.StudentStatus != (byte)StudentStatus.Left
                            select new
                            {
                                s1.AdmissionNo,
                                s1.FatherName,
                                s1.FullName,
                                a1.RollNo,
                                s1.MobileNo,
                                s2.SectionName,
                                c1.ClassName2,
                                c1.ClassName,
                                s1.MotherName,
                                s1.CorrespondenceAddressLine1,
                                s1.PermanentAddressLine1,
                                DueFeeDetail = FeeData.GetDueDetails(dataContext, a1.AdmissionId, UptoMonthNo, false, UpToYear, UpToMonthDataNo)
                            }).ToList();

                var students = from s1 in list
                               select new
                               {
                                   StudentName = s1.FullName,
                                   AdmissionNo = s1.AdmissionNo,
                                   Class = s1.ClassName2,
                                   Section = s1.SectionName,
                                   RollNo = s1.RollNo,
                                   FatherName = s1.FatherName,
                                   MotherName = s1.MotherName,
                                   CorrespondenceAddress = s1.CorrespondenceAddressLine1,
                                   PermanentAddress = s1.PermanentAddressLine1,
                                   PhoneNo = s1.MobileNo,
                                   FeeDues = s1.DueFeeDetail.TotalDues
                               };

                if (students.Count() <= 0)
                    throw new ArgumentException("No Records found");
                GridView gv = new GridView();
                gv.DataSource = students;
                gv.DataBind();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Student_List.xls"));
                HttpContext.Current.Response.ContentType = "application/ms-excel";
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
                HttpContext.Current.Response.Write(sw.ToString());
                HttpContext.Current.Response.End();
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
