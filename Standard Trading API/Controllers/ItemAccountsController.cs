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
using static Standard_Trading_API.Model.GlAccountsDataType;
using static Standard_Trading_API.Model.ItemAccountDataType;
using DBEntity = Data_Access_Layer.DBEntity;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemAccountsController : Controller
    {
        public string sSQL, msg;
        public DataTable dt;


        [HttpGet]
        public ActionResult getItemAccounts()
        {
            SqlCommand com = new SqlCommand("SP_GetItemAccount");
            dt = DBEntity.GetDataTableBySP(com, out msg);
            return Ok(dt);
        }

        [HttpGet]
        public ActionResult GL_AccountsLOV()
        {
            sSQL = String.Empty;
            msg = String.Empty;
            dt = new DataTable();

            sSQL = "select Account_Code as  accountCode,Account_Name as accountName from GL_Accounts where Levels = 4";

            dt = DBEntity.fillComboBox(sSQL);
            return Ok(dt);
        }

        [HttpPost]

        public ActionResult SaveMaster([FromBody] ItemAccount instance)
        {

            msg = string.Empty;
            sSQL = string.Empty;

            try
            {

                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);


                int GlSalesAccount = Convert.ToInt32(GetAccountID(instance.GlSalesAccount.ToString()));
                int GlStockAccount = Convert.ToInt32(GetAccountID(instance.GlStockAccount.ToString()));
                int GlCostAccount = Convert.ToInt32(GetAccountID(instance.GlCostAccount.ToString()));
                int GlConsumptionAccount = Convert.ToInt32(GetAccountID(instance.GlConsumptionAccount.ToString()));
                int GlSalePromotionAccount = Convert.ToInt32(GetAccountID(instance.GlSalePromotionAccount.ToString()));

                SqlCommand com = new SqlCommand("SP_SaveItemAccount");
                com.Parameters.AddWithValue("@ItemAccountName", instance.ItemAccountName);
                com.Parameters.AddWithValue("@GlSalesAccount", GlSalesAccount);
                com.Parameters.AddWithValue("@GlStockAccount", GlStockAccount);
                com.Parameters.AddWithValue("@GlCostAccount", GlCostAccount);
                com.Parameters.AddWithValue("@GlConsumptionAccount", GlConsumptionAccount);
                com.Parameters.AddWithValue("@GlSalePromotionAccount", GlSalePromotionAccount);
                com.Parameters.AddWithValue("@TransactionType", instance.transactionType);
                com.Parameters.AddWithValue("@ItemAccountId", instance.ItemAccountId);
                


                com.Parameters.AddWithValue("@BookId", 1);

                DBEntity.GetDataTableBySP(com, out msg);
                if (msg == string.Empty)
                {
                    sSQL = string.Empty;
                    sSQL = "COMMIT TRANSACTION;";
                    DBEntity.GetSqlData(sSQL, out msg);
                    return Ok(new { Message = "Success" });
                }
                else
                {
                    RevertTransaction();
                    return Ok(new { Message = "Rollback" });
                }
            }
            catch (Exception)
            {
                RevertTransaction();
                return Ok(new { Message = "Rollback" });
            }
            RevertTransaction();
            return Ok(new { Message = "Rollback" });
        }
        [HttpPost]
        public ActionResult DeleteMaster([FromBody] ItemAccount instance)
        {
            dt = new DataTable();
            msg = string.Empty;
            sSQL = string.Empty;

            try
                {
                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);
                msg = string.Empty;
                sSQL = string.Empty;
                sSQL = "delete from Item_Accounts where Item_Account_ID =" + instance.ItemAccountId + "";
                DBEntity.ExecuteNonQuery(sSQL,out msg);
                if (msg == string.Empty)
                {
                    sSQL = string.Empty;
                    sSQL = "COMMIT TRANSACTION;";
                    DBEntity.GetSqlData(sSQL, out msg);
                    return Ok(new { Message = "Success" });
                }
                else
                {
                    RevertTransaction();
                    return Ok(new { Message = "Item Account Deleted not successfully" });
                }

            }
                catch (Exception)
                {
                RevertTransaction();
                return Ok(new { Message = "Item Account Deleted not successfully" });
            }


            RevertTransaction();
            return Ok(new { Message = "Item Account Deleted not successfully" });
        }

        private void RevertTransaction()
        {
            sSQL = string.Empty;
            sSQL = "ROLLBACK TRANSACTION;";
            DBEntity.GetSqlData(sSQL, out msg);
        }
        [HttpPost]
        public string GetAccountID(string PCode)
        {
            sSQL = string.Empty;
            sSQL = "Select Isnull(Max(Account_ID),0) from GL_Accounts where Account_Code =" + PCode + "";
            var AccountId = DBEntity.getField(sSQL);
            return (AccountId.ToString());
        }

    }

   
}
