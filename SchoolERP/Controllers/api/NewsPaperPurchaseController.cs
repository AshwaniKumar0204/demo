using School;
using SchoolERP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolERP.Controllers.api
{
    [CheckSessionTimeOutAttribute]
    [RoutePrefix("api/NewsPaperPurchase")]
    public class NewsPaperPurchaseController : ApiController
    {
        [HttpPost]
        [Route("SaveNewsPaperPurchase")]
        public ExpandoObject SaveNewsPaperPurchase(List<NewsPaperPurchaseModel> newsPaperPurchaseList)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                newsPaperPurchaseList.ForEach(model =>
                {

                    NewsPaperPurchase NewsPaperPurchase = null;
                    if (model.NewsPaperPurchaseId > 0)
                    {
                        NewsPaperPurchase = dataContext.NewsPaperPurchases.Where(x => x.NewsPaperPurchaseId == model.NewsPaperPurchaseId).First();
                        NewsPaperPurchase.LastUpdatedBy = SessionUtilies.GetLoginId();
                        NewsPaperPurchase.LastUpdatedOn = DateTime.Now;
                    }
                    else
                        NewsPaperPurchase = new NewsPaperPurchase
                        {
                            CreatedBy = SessionUtilies.GetLoginId(),
                            CreatedOn = DateTime.Now,
                        };
                    NewsPaperPurchase.PurchaseDate = model.PurchaseDate;
                    NewsPaperPurchase.NewsPaperId = model.NewsPaperId;
                    NewsPaperPurchase.Price = model.Price;
                    if (NewsPaperPurchase.NewsPaperPurchaseId == 0)
                        dataContext.NewsPaperPurchases.InsertOnSubmit(NewsPaperPurchase);
                    dataContext.SubmitChanges();
                });
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("NewPaperListForPurchase")]
        public ExpandoObject NewPaperListForPurchase(DateTime purchaseDate)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = from p1 in dataContext.NewsPapers
                           join p2 in dataContext.NewsPaperPurchases.Where(x => x.PurchaseDate.Date == purchaseDate.Date)
                           on p1.NewsPaperId equals p2.NewsPaperId into p2List
                           select new
                           {
                               p1.NewsPaperId,
                               p1.NewsPaperName,
                               NewsPaperPurchaseId = p2List.Any() ? p2List.First().NewsPaperPurchaseId : 0,
                               Price = p2List.Any() ? p2List.First().Price : 0,
                               PurchaseDate = purchaseDate,
                           };
                response.NewsPaperPurchaseList = list.ToList();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("NewPaperPurchaseList")]
        public ExpandoObject NewPaperPurchaseList(int MonthNo, int Year)
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                SqlConnection con = new SqlConnection(dataContext.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand("NewsPaperPurchaseList", con);
                cmd.Parameters.Add(new SqlParameter("@MonthNo", MonthNo));
                cmd.Parameters.Add(new SqlParameter("@Year", Year));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                List<DynamicModel> list = new List<DynamicModel>();
                List<DynamicModel> HeaderList = new List<DynamicModel>();
                List<DynamicModel> FooterList = new List<DynamicModel>();
                //Header
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    HeaderList.Add(new DynamicModel { Value = i == 0 ? "Purchase Date" : dt.Columns[i].ColumnName });
                    FooterList.Add(new DynamicModel { Value = i == 0 ? "Total" : "0" });
                }

                //Body
                foreach (DataRow dr in dt.Rows)
                {
                    List<DynamicModel> BodyList = new List<DynamicModel>();
                    for (int i = 0; i < dt.Columns.Count; ++i)
                    {
                        BodyList.Add(new DynamicModel { Value = i == 0 ? Convert.ToDateTime(dr[i]).ToString("dd-MM-yyyy") : dr[i].ToString() });
                        if (i > 0)
                            FooterList[i].Value = (Convert.ToDecimal(FooterList[i].Value) + Convert.ToDecimal(dr[i])).ToString("N");
                    }
                    list.Add(new DynamicModel { DynamicList = BodyList });
                }


                //Footer

                response.NewsPaperPurchaseList = list;
                response.HeaderList = HeaderList;
                response.FooterList = FooterList;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("NewsPaperYearList")]
        public ExpandoObject NewsPaperYearList()
        {
            dynamic response = new ExpandoObject();
            try
            {
                SchoolDataContext dataContext = new SchoolDataContext();
                var list = dataContext.NewsPaperPurchases.Select(x => x.PurchaseDate.Year).Distinct();
                response.YearList = list.ToList();
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
