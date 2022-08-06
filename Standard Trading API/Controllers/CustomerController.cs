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
using static Standard_Trading_API.Model.CustomerDataType;
using static Standard_Trading_API.Model.SegmentManagementDataType;
using DBEntity = Data_Access_Layer.DBEntity;
using Utility = Data_Access_Layer.Utility;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : Controller
    {
        string msg = string.Empty;
        string sSQL = string.Empty;
        public string ImagePath;
        public DataTable dt;
        public string ControlCode = "", GlAccountCode = "", AccountId = String.Empty;
        public int incrementAcCode = 0;
       


        [HttpGet]
        // [Authorize]
        public ActionResult GetCustomers()
        {
            dt = new DataTable();

            SqlCommand com = new SqlCommand("SP_GetCustommer");
            dt = DBEntity.GetDataTableBySP(com, out msg);
            if(dt.Rows.Count == 0)
            {
                
             return Ok(new { Message = "NULL" });
            }
            return Ok(dt);
        }
        [HttpPost]
        public IActionResult SaveMaster([FromBody] Ar_Customer Instance)
        {
            try
            {
                ImagePath = Instance.photoPath;


                msg = string.Empty;
                sSQL = string.Empty;

                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);

                if (Instance.addorEdit == "Add")
                {
                    #region "Add Record";
                    int companyId = Data_Access_Layer.Utility.CompanyID;

                    if (Instance.addManually == false)
                    {
                        ControlCode = DBEntity.getField("select Parameter_Value from sys_Parms where Parameter_Name = 'Customer Control Code'").ToString();

                        var ParentCode = DBEntity.getField("Select Isnull(Max(Account_ID),0)  from GL_Accounts where Account_Code ='"+ ControlCode + "'");

                        string AccountCode = DBEntity.getField("Select Max(Account_Code) from GL_Accounts where Parent_Code ='"+ ParentCode + "'").ToString();

                        if (AccountCode == "")
                        {
                            GlAccountCode = ControlCode + "00001";
                        }
                        else
                        {
                            incrementAcCode = Convert.ToInt32(AccountCode) + 1;
                            GlAccountCode = incrementAcCode.ToString();

                        }

                        SqlCommand com = new SqlCommand("sp_AddCustomerAndSupplierGl_Account");
                        com.Parameters.AddWithValue("@AccountCode", GlAccountCode);
                        com.Parameters.AddWithValue("@AccountName", Instance.customerName+" , "+ Instance.cityName);
                        com.Parameters.AddWithValue("@Levels",4);
                        com.Parameters.AddWithValue("@ParentCode", ParentCode);
                        com.Parameters.AddWithValue("@BookID", Utility.Book_ID);
                        com.Parameters.AddWithValue("@Active",1);
                        com.Parameters.AddWithValue("@AccountType",null);
                        com.Parameters.AddWithValue("@expanded",0);
                        com.Parameters.AddWithValue("@icon", ImagePath);
                        var GlAccountData = DBEntity.GetDataTableBySP(com, out msg);

                        if (msg == string.Empty)
                        {
                            AccountId = DBEntity.getField("Select top 1 Account_Id from GL_Accounts where Levels = 4 and Active = 1 order by Account_Id desc").ToString();
                         }
                        else
                        {
                            RevertTransaction("Add");
                            return Ok(new { Message = "Error" });
                        }
                    }
                    else // Gl Account Part
                    {
                        AccountId = DBEntity.getField("Select Max(Account_Id) from GL_Accounts where Account_Code =" + Instance.AccountCode + " ").ToString();
                    }
                }
                else // Edit
                {
                    AccountId = DBEntity.getField("Select Max(Account_Id) from GL_Accounts where Account_Code =" + Instance.AccountCode + " ").ToString();
                }
                    SqlCommand com1 = new SqlCommand("sp_AddCustomer");
                    com1.Parameters.AddWithValue("@CustomerId", Instance.customerId);
                    com1.Parameters.AddWithValue("@CustomerName", Instance.customerName);
                    com1.Parameters.AddWithValue("@Address", Instance.address);
                    com1.Parameters.AddWithValue("@City", Instance.cityName);
                    com1.Parameters.AddWithValue("@MobileNo", Instance.mobileNo);
                    com1.Parameters.AddWithValue("@PhoneNo", Instance.phoneNo);
                    com1.Parameters.AddWithValue("@FaxNo", Instance.faxNo);
                    com1.Parameters.AddWithValue("@EmailAddress", Instance.emailAddress);
                    com1.Parameters.AddWithValue("@SalesTaxNo", Instance.salesTaxNo);
                    com1.Parameters.AddWithValue("@NtnNo", Instance.ntnNo);
                    com1.Parameters.AddWithValue("@AccountId", AccountId);
                    com1.Parameters.AddWithValue("@PhotoPath", Instance.photoPath);
                    com1.Parameters.AddWithValue("@Country", Instance.countryName);
                    com1.Parameters.AddWithValue("@CompanyId", Utility.CompanyID);
                    com1.Parameters.AddWithValue("@AddorEdit", Instance.addorEdit);
                    var CustomerData = DBEntity.GetDataTableBySP(com1, out msg);

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
                

                #endregion

          
            }
            catch(Exception ex)
            {
                RevertTransaction("Add");
                return Ok(new { message = "Error" });
            }
            return Ok();
        }

        [HttpPost]
        public ActionResult GetMaxID ([FromBody] Ar_Customer Instance)
        {
            var CustomerID = DBEntity.getField("select isnull(max(Customer_ID),0)+1 from AR_Customers oder");
            return Ok(CustomerID);
        }

       
        [HttpPost]
        public ActionResult DeleteCustomer([FromBody] Ar_Customer Instance)
        {
            msg = string.Empty;
            sSQL = string.Empty;

            try
            {

                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);

                #region "Customer Remove";
               
                string customer = DBEntity.getField("Select PhotoPath from Ar_Customers where Customer_ID =" + Instance.customerId + " ").ToString();


                    #endregion

                    #region "User Photo Remove";
                    var path = "";
                    path = customer;

                if (path != "/assets/img/Blank.png")
                {
                    if ((System.IO.File.Exists(path)))
                    {
                     System.IO.File.Delete(path);

                    }
                }
                    DBEntity.ExecuteNonQuery("Delete from Ar_Customers where Customer_ID =" + Instance.customerId + " ", out msg);

                    if (msg == string.Empty)
                    {
                        AccountId = DBEntity.getField("Select Max(Account_Id) from GL_Accounts where Account_Code =" + Instance.AccountCode + " ").ToString();

                        DBEntity.ExecuteNonQuery("Delete from GL_Accounts where Account_Id =" + AccountId + "", out msg);

                        if (msg == string.Empty)
                        {
                            sSQL = string.Empty;
                            sSQL = "COMMIT TRANSACTION;";
                            DBEntity.GetSqlData(sSQL, out msg);

                            return Ok(new { Message = "Success" });
                        }
                        else
                        {
                        RevertTransaction("Delete");
                        return Ok(new { Message = "Error" });
                        }
                    }
                    else
                    {
                    RevertTransaction("Delete");
                    return Ok(new { Message = "Error" });
                    }

                    #endregion

                    return Ok(new { Message = "Success" });

            }
            catch (Exception)
            {
                RevertTransaction("Add");
                return Ok(new { Message = "Error" });
            }
           
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
