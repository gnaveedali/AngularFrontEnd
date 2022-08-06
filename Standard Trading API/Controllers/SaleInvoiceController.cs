using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Standard_Trading_API.Model;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static Standard_Trading_API.Model.InvoiceDataType;
using static Standard_Trading_API.Model.SegmentManagementDataType;
using DBEntity = Data_Access_Layer.DBEntity;
using Utility = Data_Access_Layer.Utility;
using System.Linq;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SaleInvoiceController : Controller
    {
        string msg = string.Empty;
        string sSQL = string.Empty;
        public DataTable dt;
        [HttpPost]
        public IActionResult GetInvoiceNo([FromBody] Ar_Invoice Instance)
        {
            if (Instance != null)
            {
                dt = new DataTable();
                SqlCommand com = new SqlCommand("sp_GetMaxNo");
                com.Parameters.AddWithValue("@Date", Instance.InvoiceDate.ToString(Utility.dateFormat));
                com.Parameters.AddWithValue("@InvoiceType", Instance.InvoiceType);
                //com.Parameters.AddWithValue("@ComanyID", Utility.CompanyID);
                com.Parameters.AddWithValue("@ComanyID", 1);
                com.Parameters.AddWithValue("@Type", "INV");
                dt = DBEntity.GetDataTableBySP(com, out msg);

                return Ok(dt);

            }
            return Ok(null);

        }

        [HttpGet]
        public IActionResult GetCustomerInform()
        {
            dt = new DataTable();
            SqlCommand com = new SqlCommand("sp_GetCustomerAndSupplierInform");
            com.Parameters.AddWithValue("@Type", "Customers");
            dt = DBEntity.GetDataTableBySP(com, out msg);
            return Ok(dt);
        }

        [HttpPost]
        public IActionResult GetItems([FromBody] GetItems Instance)
        {
            if (Instance != null)
            {
                dt = new DataTable();
                SqlCommand com = new SqlCommand("sp_GetSalesItems");
                com.Parameters.AddWithValue("@Date", Instance.invoiceDate);
                com.Parameters.AddWithValue("@WareHouse", Instance.wareHouse);
                com.Parameters.AddWithValue("@ItemCode", Instance.itemCode);
                com.Parameters.AddWithValue("@Type", Instance.type);
                dt = DBEntity.GetDataTableBySP(com, out msg);
                return Ok(dt);
            }
            return null;

        }

        [HttpPost]

        public IActionResult SaveMaster([FromBody] Ar_Invoice Instance)
        {
            if(Instance != null)
            {

                dt = new DataTable();
                int InvoiceID = 0;

                msg = string.Empty;
                sSQL = string.Empty;

                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);

                SqlCommand com = new SqlCommand("SP_AddOrEdit_Ar_Invoice");
                com.Parameters.AddWithValue("@InvoiceId", Instance.InvoiceId);
                com.Parameters.AddWithValue("@InvoiceNo", Convert.ToInt32(Instance.InvoiceNo));
                com.Parameters.AddWithValue("@InvoiceDate", Instance.InvoiceDate.ToString(Utility.dateFormat));
                com.Parameters.AddWithValue("@TransactionType", "INV");
                com.Parameters.AddWithValue("@WareHouse", Instance.WareHouse);
                com.Parameters.AddWithValue("@Saleman", Instance.Saleman);
                com.Parameters.AddWithValue("@CustomerID", Instance.CustomerId);
                com.Parameters.AddWithValue("@PeriodID", Instance.PeriodId);
                com.Parameters.AddWithValue("@InvoiceAmount", Instance.InvoiceAmount);
                com.Parameters.AddWithValue("@FreightAmount", Instance.Freight_Amount);
                com.Parameters.AddWithValue("@RefrenceNo", Instance.RefrenceNo);
                com.Parameters.AddWithValue("@Remarks", Instance.Remarks);
                com.Parameters.AddWithValue("@CustomerName", Instance.CustomerName);
                com.Parameters.AddWithValue("@CustmerCell", Instance.CustomerCell);
                com.Parameters.AddWithValue("@CompanyId", Utility.CompanyID);
                com.Parameters.AddWithValue("@CreatedBy", Utility.User_ID);
                com.Parameters.AddWithValue("@SalesTaxPercentage", Instance.SalesTaxPercentage);
                com.Parameters.AddWithValue("@SalesTaxAmt", Instance.SalesTaxAmount);
                com.Parameters.AddWithValue("@DiscountPercentage", Instance.DiscountPercentage);
                com.Parameters.AddWithValue("@DiscountAmt", Instance.DiscountAmount);
                com.Parameters.AddWithValue("@InvoiceType", Instance.InvoiceType);
                com.Parameters.AddWithValue("@AddorEdit", Instance.AddorEdit);

                dt = DBEntity.GetDataTableBySP(com, out msg);

                if (msg == string.Empty)
                {
                    if (Instance.AddorEdit == "ADD")
                    {
                        InvoiceID = Convert.ToInt32(dt.Rows[0]["InvoiceId"]);
                    }
                    else
                    {
                        DBEntity.ExecuteNonQuery("Delete from AR_Invoice_Items where Invoice_ID=" + Instance.InvoiceId + "", out msg);
                        InvoiceID = Convert.ToInt32(Instance.InvoiceId);
                    }

                    foreach (var Item in Instance.InvoiceItems)
                    {
                        SqlCommand detail = new SqlCommand("SP_AddorEdit_AR_Invoice_Items");
                        detail.Parameters.AddWithValue("@InvoiceId", InvoiceID);
                        detail.Parameters.AddWithValue("@Itemcode", Item.ItemCode);
                        detail.Parameters.AddWithValue("@RateEnt", Item.RateEnt);
                        detail.Parameters.AddWithValue("@Rate", Item.Rate);
                        detail.Parameters.AddWithValue("@Qty", Item.Qty);
                        detail.Parameters.AddWithValue("@IssueRate", Item.IssueRate);
                        detail.Parameters.AddWithValue("@Cost_Of_Slaes", Item.Cost_Of_Sales);
                        detail.Parameters.AddWithValue("@Discount_Percentage", Item.DiscountPercentage);
                        detail.Parameters.AddWithValue("@Discount_Amt", Item.DiscountAmount);
                        detail.Parameters.AddWithValue("@Value", Item.Value);
                        detail.Parameters.AddWithValue("@NetValue", Item.Net_Value);
                        detail.Parameters.AddWithValue("@Sales_Order_Item_ID", Item.Sales_Order_Item_ID);

                        var data = DBEntity.GetDataTableBySP(detail, out msg);

                        if (msg != string.Empty)
                        {
                            break;
                        }
                    }
                    if (msg == string.Empty)
                    {
                        sSQL = string.Empty;
                        sSQL = "COMMIT TRANSACTION;";
                        DBEntity.GetSqlData(sSQL, out msg);
                        return Ok(new { Message = "Sale Invoice Save successfully" });
                    }
                    else
                    {
                        RevertTransaction();
                        return Ok(new { Message = "RollBack" });
                    }
                }
                else
                {
                    RevertTransaction();
                    return Ok(new { Message = "RollBack" });
                }


                return Ok();
            }

            return null;
        }
        [HttpPost]
        public IEnumerable<Ar_Invoice> LoadMaster([FromBody] Ar_Invoice Instance)
        {
            if (Instance != null)
            {
                List<Ar_Invoice> vm = new List<Ar_Invoice>();
                DataTable ReturnData = new DataTable();
                DataTable DetailData = new DataTable();

                SqlCommand master = new SqlCommand("SP_SaleInvoice_GetAr_Invoice");
                master.Parameters.AddWithValue("@PeriodID", Instance.PeriodId);
                ReturnData = DBEntity.GetDataTableBySP(master, out msg);

                for (int i = 0; i < ReturnData.Rows.Count; i++)
                {


                    SqlCommand detail = new SqlCommand("SP_SaleInvoice_GetAr_Invoice_Items");
                    detail.Parameters.AddWithValue("@InvoiceId", Convert.ToInt32(ReturnData.Rows[i]["InvoiceId"]));
                    DetailData = DBEntity.GetDataTableBySP(detail, out msg);


                    List<AR_Invoice_Items> list = new List<AR_Invoice_Items>();

                    for (int j = 0; j < DetailData.Rows.Count; j++)
                    {
                        list.Add(new AR_Invoice_Items
                        {
                            InvoiceItemID = Convert.ToInt32(DetailData.Rows[j]["InvoiceItemID"]),
                            InvoiceId = Convert.ToInt32(DetailData.Rows[j]["InvoiceId"]),
                            ItemCode = DetailData.Rows[j]["ItemCode"].ToString(),
                            Description = DetailData.Rows[j]["Description"].ToString(),
                            Stock = Convert.ToDecimal(DetailData.Rows[j]["Stock"]),
                            Qty = Convert.ToDecimal(DetailData.Rows[j]["Qty"]),
                            RateEnt = Convert.ToDecimal(DetailData.Rows[j]["RateEnt"]),
                            Rate = Convert.ToDecimal(DetailData.Rows[j]["Rate"]),
                            IssueRate = Convert.ToDecimal(DetailData.Rows[j]["IssueRate"]),
                            SaleRate = Convert.ToDecimal(DetailData.Rows[j]["SaleRate"]),
                            Cost_Of_Sales = Convert.ToDecimal(DetailData.Rows[j]["Cost_Of_Sales"]),
                            DiscountPercentage = Convert.ToDecimal(DetailData.Rows[j]["DiscountPercentage"]),
                            DiscountAmount = Convert.ToDecimal(DetailData.Rows[j]["DiscountAmount"]),
                            SalesTaxPercentage = Convert.ToDecimal(DetailData.Rows[j]["SalesTaxPercentage"]),
                            SalesTaxAmount = Convert.ToDecimal(DetailData.Rows[j]["SalesTaxAmount"]),
                            ExiseTaxPercentage = Convert.ToDecimal(DetailData.Rows[j]["ExiseTaxPercentage"]),
                            ExiseTaxAmount = Convert.ToDecimal(DetailData.Rows[j]["ExiseTaxAmount"]),
                            Sales_Order_Item_ID = Convert.ToDecimal(DetailData.Rows[j]["Sales_Order_Item_ID"]),
                            Value = Convert.ToDecimal(DetailData.Rows[j]["Value"]),
                            Net_Value = Convert.ToDecimal(DetailData.Rows[j]["Net_Value"]),
                        });
                    }

                    vm.Add(new Ar_Invoice
                    {
                        InvoiceId = Convert.ToInt32(ReturnData.Rows[i]["InvoiceId"]),
                        InvoiceNo = Convert.ToInt32(ReturnData.Rows[i]["InvoiceNo"]),
                        InvoiceDate = Convert.ToDateTime(ReturnData.Rows[i]["InvoiceDate"]),
                        TransactionType = ReturnData.Rows[i]["TransactionType"].ToString(),
                        WareHouse = Convert.ToInt32(ReturnData.Rows[i]["WareHouse"]),
                        Saleman = Convert.ToInt32(ReturnData.Rows[i]["Saleman"]),
                        CustomerId = Convert.ToInt32(ReturnData.Rows[i]["CustomerId"]),
                        Ar_CustomerName = ReturnData.Rows[i]["Ar_CustomerName"].ToString(),
                        CustomerAddress = ReturnData.Rows[i]["CustomerAddress"].ToString(),
                        PeriodId = Convert.ToInt32(ReturnData.Rows[i]["PeriodId"]),
                        InvoiceAmount = Convert.ToDecimal(ReturnData.Rows[i]["InvoiceAmount"]),
                        Freight_Amount = Convert.ToDecimal(ReturnData.Rows[i]["Freight_Amount"]),
                        RefrenceNo = ReturnData.Rows[i]["RefrenceNo"].ToString(),
                        Remarks = ReturnData.Rows[i]["Remarks"].ToString(),
                        CustomerName = ReturnData.Rows[i]["CustomerName"].ToString(),
                        CustomerCell = ReturnData.Rows[i]["CustomerCell"].ToString(),
                        Email_Address = ReturnData.Rows[i]["Email_Address"].ToString(),
                        Sales_Tax_No = ReturnData.Rows[i]["STN_No"].ToString(),
                        NTN_No = ReturnData.Rows[i]["NTN_No"].ToString(),
                        Posted = Convert.ToBoolean(ReturnData.Rows[i]["Posted"]),
                        SalesTaxPercentage = Convert.ToDecimal(ReturnData.Rows[i]["SalesTaxPercentage"]),
                        SalesTaxAmount = Convert.ToDecimal(ReturnData.Rows[i]["SalesTaxAmount"]),
                        DiscountPercentage = Convert.ToDecimal(ReturnData.Rows[i]["DiscountPercentage"]),
                        DiscountAmount = Convert.ToDecimal(ReturnData.Rows[i]["DiscountAmount"]),
                        InvoiceType = Convert.ToInt32(ReturnData.Rows[i]["InvoiceType"]),

                        InvoiceItems = list
                    }); ;

                }

                return vm;
            }
            else
            {
                return null;
            }
        }
        [HttpPost]

        public IActionResult DeleteMaster([FromBody] Ar_Invoice Instance)
        {
            bool Result=Convert.ToBoolean(DBEntity.IsDeleted("Ar_Invoice", "Invoice_ID", Convert.ToInt32(Instance.InvoiceId)));
            if(Result==true)
            {
                return Ok(new { Message = "Sale Invoice Deleted Successfully" });
            }
            else
            {
                return Ok(new { Message = "RollBack" }); 
            }
            
        }

        [HttpPost]
        public IActionResult PostMaster([FromBody] Ar_Invoice Instance)
        {
            SqlCommand com = new SqlCommand("spPostSalesInvoice");
            com.Parameters.AddWithValue("@InvoiceId", Convert.ToInt32(Instance.InvoiceId));
            var DetailData = DBEntity.GetDataTableBySP(com, out msg);

            if (msg == string.Empty)
            {
               
                return Ok(new { Message = "Sale Invoice Posted Successfully" });
            }
            else
            {
                RevertTransaction();
                return Ok(new { Message = "Sale Invoice Posted Not Successfully" });
            }

           
        }

        [HttpPost]
        public IActionResult UnPostMaster([FromBody] Ar_Invoice Instance)
        {
            SqlCommand com = new SqlCommand("spUnPostSalesInvoice");
            com.Parameters.AddWithValue("@Invoice_ID", Convert.ToInt32(Instance.InvoiceId));
            var DetailData = DBEntity.GetDataTableBySP(com, out msg);

            if (msg == string.Empty)
            {

                return Ok(new { Message = "Sale Invoice UnPosted Successfully" });
            }
            else
            {
                RevertTransaction();
                return Ok(new { Message = "Sale Invoice UnPosted Not Successfully" });
            }


        }

        private void RevertTransaction()
         {
            sSQL = string.Empty;
            sSQL = "ROLLBACK TRANSACTION;";
            DBEntity.GetSqlData(sSQL, out msg);

         }

    }
}
