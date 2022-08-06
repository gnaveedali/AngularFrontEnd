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
using static Standard_Trading_API.Model.ItemDataType;
using DBEntity = Data_Access_Layer.DBEntity;
using Utility = Data_Access_Layer.Utility;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemsDefinitionController : Controller
    {
        string msg = string.Empty;
        string sSQL = string.Empty;
        public string ImagePath;
        public DataTable dt;

        [HttpGet]
        // [Authorize]
        public ActionResult GetItems()
        {
            dt = new DataTable();

            SqlCommand com = new SqlCommand("SP_GetItemsDefinition");
            dt = DBEntity.GetDataTableBySP(com, out msg);
            if (dt.Rows.Count == 0)
            {

                return Ok(new { Message = "NULL" });
            }
            return Ok(dt);
        }

        [HttpGet]
        public ActionResult GetItemsCode()
        {
            string Code = DBEntity.getMax("Item_Code", "Items", " where Book_id=" + Data_Access_Layer.Utility.Book_ID);
            string itemCode = Code == "" ? "" : Convert.ToDouble(Code).ToString("000000");

            return Ok(new { ItemCode = itemCode });
        }

        [HttpPost]
        public IActionResult SaveMaster([FromBody] ItemsDefinition Instance)
        {
            if (Instance != null)
            {
                try
                {

                    ImagePath = Instance.photoPath;

                    int Purchaseable = 1, Salesable = 1, Active = 1, AttachBarcode = 0, ExpiryDate = 0, Pack = 0;

                    if (Instance.purchaseable == false)
                        Purchaseable = 0;
                    if (Instance.salesable == false)
                        Salesable = 0;
                    if (Instance.active == false)
                        Active = 0;
                    if (Instance.attachBarcode == true)
                        Active = 1;
                    if (Instance.expiryDate == true)
                        Active = 1;
                    if (Instance.pack == true)
                        Pack = 1;


                    msg = string.Empty;
                    sSQL = string.Empty;

                    sSQL = " BEGIN TRANSACTION; ";
                    DBEntity.GetSqlData(sSQL, out msg);

                    SqlCommand com = new SqlCommand("SP_AddorEditItems");
                    com.Parameters.AddWithValue("@ItemCode", Instance.itemCode);
                    com.Parameters.AddWithValue("@ItemDescription", Instance.itemDescription);
                    com.Parameters.AddWithValue("@SalesRate", Instance.salesRate);
                    com.Parameters.AddWithValue("@MrpRate", Instance.mrpRate);
                    com.Parameters.AddWithValue("@PurchaseRate", Instance.purchaseRate);
                    com.Parameters.AddWithValue("@Uom", Instance.uomName);
                    com.Parameters.AddWithValue("@Pack", Pack);
                    com.Parameters.AddWithValue("@PackQty", Instance.packQty);
                    com.Parameters.AddWithValue("@PackUnit", Instance.packUnitName);
                    com.Parameters.AddWithValue("@ItemMainType", Instance.mainTypeName);
                    com.Parameters.AddWithValue("@ItemSubType", Instance.subTypeName);
                    com.Parameters.AddWithValue("@ManufactureBy", Instance.manufactureByName);
                    com.Parameters.AddWithValue("@Purchaseable", Purchaseable);
                    com.Parameters.AddWithValue("@Salesable", Salesable);
                    com.Parameters.AddWithValue("@MinStockLevel", Instance.minStockLevel);
                    com.Parameters.AddWithValue("@MaxStockLevel", Instance.maxStockLevel);
                    com.Parameters.AddWithValue("@ReorderStockLevel", Instance.reorderStockLevel);
                    com.Parameters.AddWithValue("@ItemAccountName", Instance.accountName);
                    com.Parameters.AddWithValue("@Active", Active);
                    com.Parameters.AddWithValue("@BookId", Utility.Book_ID);
                    com.Parameters.AddWithValue("@AttachBarcode", AttachBarcode);
                    com.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
                    com.Parameters.AddWithValue("@Item_Expiry_Date", Instance.itemExpiryDate.ToString(Utility.dateFormat));
                    com.Parameters.AddWithValue("@PhotoPath", Instance.photoPath);
                    com.Parameters.AddWithValue("@addorEdit", Instance.addorEdit);
                    var CustomerData = DBEntity.GetDataTableBySP(com, out msg);

                    if (msg == string.Empty)
                    {
                        sSQL = string.Empty;
                        sSQL = "COMMIT TRANSACTION;";
                        DBEntity.GetSqlData(sSQL, out msg);

                        return Ok(new { Message = "Success" });
                    }
                    else
                    {
                        RevertTransaction("Add");
                        return Ok(new { Message = "Error" });
                    }

                }

                catch (Exception ex)
                {
                    RevertTransaction("Add");
                    return Ok(new { Message = "Error" });
                }
               
            }
            return Ok();

         
        }

        [HttpPost]
        public IActionResult DeleteMaster([FromBody] ItemsDefinition Instance)
        {
            if (Instance != null)
            {
                try
                {
                    msg = string.Empty;
                    sSQL = string.Empty;

                    sSQL = " BEGIN TRANSACTION; ";
                    DBEntity.GetSqlData(sSQL, out msg);
                    msg = string.Empty;
                    sSQL = string.Empty;

                    sSQL = "Delete from Items where Item_Code =" + Instance.itemCode + " and Book_ID =" + Utility.Book_ID + " ";
                    DBEntity.ExecuteNonQuery(sSQL, out msg);

                    if (msg == string.Empty)
                    {
                        sSQL = string.Empty;
                        sSQL = "COMMIT TRANSACTION;";
                        DBEntity.GetSqlData(sSQL, out msg);

                        return Ok(new { Message = "Success" });
                    }
                    else
                    {
                        RevertTransaction("Add");
                        return Ok(new { Message = "Error" });
                    }

                }
                catch (Exception ex)
                {
                    RevertTransaction("Add");
                    return Ok(new { Message = "Error" });
                }
               
            }

          return Ok();
        }

        private void RevertTransaction(string TransactionType)
        {
            if (TransactionType == "Add" || TransactionType == "Edit")
            {
                if ((System.IO.File.Exists(ImagePath)))
                {
                    System.IO.File.Delete(ImagePath);
                }
                sSQL = string.Empty;
                sSQL = "ROLLBACK TRANSACTION;";
                DBEntity.GetSqlData(sSQL, out msg);

            }
            else
            {
                sSQL = string.Empty;
                sSQL = "ROLLBACK TRANSACTION;";
                DBEntity.GetSqlData(sSQL, out msg);
            }

        }

    }
}
