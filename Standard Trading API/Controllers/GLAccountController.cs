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
using static Standard_Trading_API.Model.SegmentManagementDataType;
using DBEntity = Data_Access_Layer.DBEntity;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GLAccountController : Controller
    {
        public string sSQL, msg;
        public DataTable dt;
        bool ReturnFolderLimt = false;
        bool ExistAccount = false;

        [HttpGet]
        // [Authorize]
        public ActionResult GetGLAccounts()
        {
            dt = new DataTable();

            SqlCommand com = new SqlCommand("sp_getGlAccounts");
            dt = DBEntity.GetDataTableBySP(com, out msg);
            return Ok(dt);
        }

        [HttpPut]
        public IActionResult Modify([FromBody]  GlAccounts instance)
        {
            
                try
                {

                    if (instance.accountId != null)
                    {
                         dt = new DataTable();
                         msg = string.Empty;
                         sSQL = string.Empty;

                         sSQL = " BEGIN TRANSACTION; ";
                         DBEntity.GetSqlData(sSQL, out msg);

                         SqlCommand com = new SqlCommand("sp_GlAccountModify");
                         com.Parameters.AddWithValue("@AccountID", instance.accountId);
                         com.Parameters.AddWithValue("@AccountName", instance.accountName);
                         DBEntity.GetDataTableBySP(com, out msg);

                         if (msg == string.Empty)
                         {
                             sSQL = string.Empty;
                             sSQL = "COMMIT TRANSACTION;";
                             DBEntity.GetSqlData(sSQL, out msg);
                             
                             SqlCommand getcom = new SqlCommand("sp_getGlAccounts");
                             dt = DBEntity.GetDataTableBySP(getcom, out msg);
                              return Ok(dt);
                         }
                         else
                         {
                           RevertTransaction();
                           return Ok(new { message = "Error" });
                         }
                    }
                  
                }
                catch (Exception)
                {
                RevertTransaction();
                return Ok(new { message = "Error" });
                }
                return Ok();

            
        }

       
        [HttpPost]
        public IActionResult GetMaxAccountCode([FromBody] GlAccounts instance)
        {
            if(instance.parentCode != null)
            {
                sSQL = string.Empty;
                sSQL = "select max(Account_Code) from GL_Accounts where Parent_Code =" + instance.parentCode + "";
                string AccountCode = DBEntity.getField(sSQL).ToString();

              
                return Ok(AccountCode);
            }
            return Ok();
        }

        private void ResetExpanded()
        {
            DataTable resetdt = new DataTable();
            resetdt = DBEntity.fillComboBox("select Account_ID from GL_Accounts where expanded = 1");
           
            for(int i = 0; i < resetdt.Rows.Count; i++)
            {
                DBEntity.ExecuteNonQuery("Update GL_Accounts set expanded= 0 where Account_ID ="+ resetdt.Rows[i]["Account_ID"] + "", out msg);
            }
           
        }

        private void FolderLimit(int AccountID)
        {
            sSQL = string.Empty;
            sSQL = "select Max(Account_Code) from GL_Accounts where Parent_Code ="+ AccountID + "";
            string FolderLimit = DBEntity.getField(sSQL).ToString();

            if (FolderLimit == "19" || FolderLimit == "29" || FolderLimit == "39" || FolderLimit == "49" || FolderLimit == "59" || FolderLimit == "69" || FolderLimit == "79" || FolderLimit == "89")
            {
                ReturnFolderLimt = true;

            }
            else
            {
                ReturnFolderLimt = false;
            }


        }

        private void ExistAccountCode(string PCode)
        {
            DataTable dataTable = new DataTable();

            dataTable = DBEntity.fillComboBox("select Account_Code from GL_Accounts where Account_Code like '%" + PCode + "%' ");

            if (dataTable.Rows.Count > 0)
            {
                ExistAccount = true;
            }
            else
            {
                ExistAccount = false;
            }

        }

        [HttpPost]
        public IActionResult AddChild([FromBody] GlAccounts instance)
        {
            
                try
                {
                    int AccountCode1 = 0;
                    string MaxAccountCode;

                      msg = string.Empty;
                      sSQL = string.Empty;

                      sSQL = " BEGIN TRANSACTION; ";
                      DBEntity.GetSqlData(sSQL, out msg);


                int Account_ID = Convert.ToInt32(instance.accountId);
                    FolderLimit(Account_ID);

                    if (ReturnFolderLimt == true)
                    {
                        return Ok(new { Message = "Full Folder Limit" });
                    }

                    else
                    {

                        ResetExpanded();


                        if (instance.levels == 1)
                        {
                        
                        var AccountId= DBEntity.getField("select Max(Account_ID) from GL_Accounts where Account_Code =" + Account_ID + "").ToString();

                        MaxAccountCode = Convert.ToInt32(Account_ID) + "1";

                            ExistAccountCode(MaxAccountCode);


                            if (ExistAccount == true)
                            {
                                int PCode = Convert.ToInt32(instance.accountCode);
                                
                                var AccountCode = DBEntity.getField("select Max(Account_Code) from      GL_Accounts where Parent_Code =" + PCode + "").ToString();

                            AccountCode1 = Convert.ToInt32(AccountCode) + 1;
                          

                            SqlCommand com0 = new SqlCommand("sp_GlAccountsAddChild");
                            com0.Parameters.AddWithValue("@AccountCode", AccountCode1);
                            com0.Parameters.AddWithValue("@AccountName", instance.accountName);
                            com0.Parameters.AddWithValue("@Levels", 2);
                            com0.Parameters.AddWithValue("@ParentCode", instance.accountId);
                            com0.Parameters.AddWithValue("@BookId", instance.BookId);
                            com0.Parameters.AddWithValue("@Active", instance.Active);
                            com0.Parameters.AddWithValue("@AccountType", instance.AccountType);
                            com0.Parameters.AddWithValue("@Expanded", instance.Expanded);
                            com0.Parameters.AddWithValue("@Icon", instance.icon);

                            var UserData = DBEntity.GetDataTableBySP(com0, out msg);
                            

                            }
                            else
                            {
                            SqlCommand com1 = new SqlCommand("sp_GlAccountsAddChild");
                            com1.Parameters.AddWithValue("@AccountCode", MaxAccountCode);
                            com1.Parameters.AddWithValue("@AccountName", instance.accountName);
                            com1.Parameters.AddWithValue("@Levels", 2);
                            com1.Parameters.AddWithValue("@ParentCode", instance.accountId);
                            com1.Parameters.AddWithValue("@BookId", instance.BookId);
                            com1.Parameters.AddWithValue("@Active", instance.Active);
                            com1.Parameters.AddWithValue("@AccountType", instance.AccountType);
                            com1.Parameters.AddWithValue("@Expanded", instance.Expanded);
                            com1.Parameters.AddWithValue("@Icon", instance.icon);

                            var UserData = DBEntity.GetDataTableBySP(com1, out msg);
                            }

                       


                    }

                        else if (instance.levels == 2)
                        {
                            int Acode = 0;
                           
                        var AccountCode = DBEntity.getField("select Max(Account_Code) from      GL_Accounts where Parent_Code =" + Account_ID + "").ToString();


                        if (AccountCode == "")
                            {
                            MaxAccountCode = Convert.ToInt32(instance.accountCode) + "01";
                           
                            SqlCommand com2 = new SqlCommand("sp_GlAccountsAddChild");
                            com2.Parameters.AddWithValue("@AccountCode", MaxAccountCode);
                            com2.Parameters.AddWithValue("@AccountName", instance.accountName);
                            com2.Parameters.AddWithValue("@Levels", 3);
                            com2.Parameters.AddWithValue("@ParentCode", instance.accountId);
                            com2.Parameters.AddWithValue("@BookId", instance.BookId);
                            com2.Parameters.AddWithValue("@Active", instance.Active);
                            com2.Parameters.AddWithValue("@AccountType", instance.AccountType);
                            com2.Parameters.AddWithValue("@Expanded", instance.Expanded);
                            com2.Parameters.AddWithValue("@Icon", instance.icon);

                            var UserData = DBEntity.GetDataTableBySP(com2, out msg);
                             }
                            else
                            {
                                Acode = Convert.ToInt32(AccountCode) + 1;

                                ExistAccountCode(Acode.ToString());

                                if (ExistAccount == true)
                                {
                                 MaxAccountCode = Convert.ToInt32(instance.accountCode) + "01";
                                
                                SqlCommand com3 = new SqlCommand("sp_GlAccountsAddChild");
                                com3.Parameters.AddWithValue("@AccountCode", MaxAccountCode);
                                com3.Parameters.AddWithValue("@AccountName", instance.accountName);
                                com3.Parameters.AddWithValue("@Levels", 3);
                                com3.Parameters.AddWithValue("@ParentCode", instance.accountId);
                                com3.Parameters.AddWithValue("@BookId", instance.BookId);
                                com3.Parameters.AddWithValue("@Active", instance.Active);
                                com3.Parameters.AddWithValue("@AccountType", instance.AccountType);
                                com3.Parameters.AddWithValue("@Expanded", instance.Expanded);
                                com3.Parameters.AddWithValue("@Icon", instance.icon);

                                var UserData = DBEntity.GetDataTableBySP(com3, out msg);
                                }
                                else
                                {
                                SqlCommand com4 = new SqlCommand("sp_GlAccountsAddChild");
                                com4.Parameters.AddWithValue("@AccountCode", Acode);
                                com4.Parameters.AddWithValue("@AccountName", instance.accountName);
                                com4.Parameters.AddWithValue("@Levels", 3);
                                com4.Parameters.AddWithValue("@ParentCode", instance.accountId);
                                com4.Parameters.AddWithValue("@BookId", instance.BookId);
                                com4.Parameters.AddWithValue("@Active", instance.Active);
                                com4.Parameters.AddWithValue("@AccountType", instance.AccountType);
                                com4.Parameters.AddWithValue("@Expanded", instance.Expanded);
                                com4.Parameters.AddWithValue("@Icon", instance.icon);

                                var UserData = DBEntity.GetDataTableBySP(com4, out msg);
                            }
                            }

                        }

                        else if (instance.levels == 3)
                        {
                            int Acode = 0;
                           
                          var AccountCode = DBEntity.getField("select Max(Account_Code) from      GL_Accounts where Parent_Code =" + Account_ID + "").ToString();


                        if (AccountCode == "")
                            {
                             MaxAccountCode = Convert.ToInt32(instance.accountCode) + "00001";
                               
                            SqlCommand com5 = new SqlCommand("sp_GlAccountsAddChild");
                            com5.Parameters.AddWithValue("@AccountCode", MaxAccountCode);
                            com5.Parameters.AddWithValue("@AccountName", instance.accountName);
                            com5.Parameters.AddWithValue("@Levels", 4);
                            com5.Parameters.AddWithValue("@ParentCode", instance.accountId);
                            com5.Parameters.AddWithValue("@BookId", instance.BookId);
                            com5.Parameters.AddWithValue("@Active", instance.Active);
                            com5.Parameters.AddWithValue("@AccountType", instance.AccountType);
                            com5.Parameters.AddWithValue("@Expanded", instance.Expanded);
                            com5.Parameters.AddWithValue("@Icon", instance.icon);

                            var UserData = DBEntity.GetDataTableBySP(com5, out msg);
                        }
                            else
                            {
                                Acode = Convert.ToInt32(AccountCode) + 1;

                                ExistAccountCode(Acode.ToString());

                            SqlCommand com6 = new SqlCommand("sp_GlAccountsAddChild");
                            com6.Parameters.AddWithValue("@AccountCode", Acode);
                            com6.Parameters.AddWithValue("@AccountName", instance.accountName);
                            com6.Parameters.AddWithValue("@Levels", 4);
                            com6.Parameters.AddWithValue("@ParentCode", instance.accountId);
                            com6.Parameters.AddWithValue("@BookId", instance.BookId);
                            com6.Parameters.AddWithValue("@Active", instance.Active);
                            com6.Parameters.AddWithValue("@AccountType", instance.AccountType);
                            com6.Parameters.AddWithValue("@Expanded", instance.Expanded);
                            com6.Parameters.AddWithValue("@Icon", instance.icon);

                            var UserData = DBEntity.GetDataTableBySP(com6, out msg);
                        }
                        }
                    }

                if (msg == string.Empty)
                {
                    sSQL = string.Empty;
                    sSQL = "COMMIT TRANSACTION;";
                    DBEntity.GetSqlData(sSQL, out msg);

                    SqlCommand getcom = new SqlCommand("sp_getGlAccounts");
                    dt = DBEntity.GetDataTableBySP(getcom, out msg);
                    return Ok(dt);
                }
                else
                {
                    RevertTransaction();
                    return Ok(new { message = "Error" });
                }
               
                }
                catch (Exception)
                {
                   
                }
           

            return Ok(new { Message = "Rollback" });
        }

       
        [HttpPost]

        public ActionResult AddSibling([FromBody]  GlAccounts instance)
        {
            
                try
                {
                    int Splitpcode = 0;
                    int AcCode = 0;
                    msg = string.Empty;
                    sSQL = string.Empty;

                if (!ModelState.IsValid)
                     return BadRequest("Invalid data.");
                    int Account_ID = Convert.ToInt32(instance.accountCode)/10;
               
                    FolderLimit(Account_ID);

                if (ReturnFolderLimt == true)
                {
                    return Ok(new { Message = "Full Folder Limit" });
                }

                else
                {

                    sSQL = " BEGIN TRANSACTION; ";
                    DBEntity.GetSqlData(sSQL, out msg);
                    ResetExpanded();

                    if (instance.levels == 2)
                    {

                        Splitpcode = Convert.ToInt32(instance.accountCode) / 10;
                        var str = Splitpcode.ToString();

                        //Get AccountID 
                        var AccountId = DBEntity.getField("select Account_ID from GL_Accounts where Account_Code =" + Splitpcode.ToString() + "").ToString();

                        var ParentCode = DBEntity.getField("select Max(Account_Code) from GL_Accounts where Parent_Code =" + Splitpcode.ToString() + "").ToString();


                        AcCode = Convert.ToInt32(ParentCode) + 1;

                        SqlCommand com2 = new SqlCommand("sp_GlAccountsAddChild");
                        com2.Parameters.AddWithValue("@AccountCode", AcCode);
                        com2.Parameters.AddWithValue("@AccountName", instance.accountName);
                        com2.Parameters.AddWithValue("@Levels", instance.levels);
                        com2.Parameters.AddWithValue("@ParentCode", AccountId);
                        com2.Parameters.AddWithValue("@BookId", instance.BookId);
                        com2.Parameters.AddWithValue("@Active", instance.Active);
                        com2.Parameters.AddWithValue("@AccountType", instance.AccountType);
                        com2.Parameters.AddWithValue("@Expanded", instance.Expanded);
                        com2.Parameters.AddWithValue("@Icon", instance.icon);

                        var UserData = DBEntity.GetDataTableBySP(com2, out msg);

                    }
                    else if (instance.levels == 3)
                    {
                        Splitpcode = Convert.ToInt32(instance.accountCode) / 100;

                        //Get AccountID 
                        var AccountId = DBEntity.getField("select Account_ID from GL_Accounts where Account_Code =" + Splitpcode.ToString() + "").ToString();

                        var ParentCode = DBEntity.getField("select Max(Account_Code) from GL_Accounts where Parent_Code =" + Convert.ToInt32(AccountId) + "").ToString();


                        AcCode = Convert.ToInt32(ParentCode) + 1;

                        SqlCommand com3 = new SqlCommand("sp_GlAccountsAddChild");
                        com3.Parameters.AddWithValue("@AccountCode", AcCode);
                        com3.Parameters.AddWithValue("@AccountName", instance.accountName);
                        com3.Parameters.AddWithValue("@Levels", instance.levels);
                        com3.Parameters.AddWithValue("@ParentCode", AccountId);
                        com3.Parameters.AddWithValue("@BookId", instance.BookId);
                        com3.Parameters.AddWithValue("@Active", instance.Active);
                        com3.Parameters.AddWithValue("@AccountType", instance.AccountType);
                        com3.Parameters.AddWithValue("@Expanded", instance.Expanded);
                        com3.Parameters.AddWithValue("@Icon", instance.icon);

                        var UserData = DBEntity.GetDataTableBySP(com3, out msg);

                    }
                    else if (instance.levels == 4)
                    {
                        Splitpcode = Convert.ToInt32(instance.accountCode) / 100000;

                        var AccountId = DBEntity.getField("select Account_ID from GL_Accounts where Account_Code =" + Splitpcode.ToString() + "").ToString();

                        var ParentCode = DBEntity.getField("select Max(Account_Code) from GL_Accounts where Parent_Code =" + Convert.ToInt32(AccountId) + "").ToString();



                        AcCode = Convert.ToInt32(ParentCode) + 1;

                        SqlCommand com4 = new SqlCommand("sp_GlAccountsAddChild");
                        com4.Parameters.AddWithValue("@AccountCode", AcCode);
                        com4.Parameters.AddWithValue("@AccountName", instance.accountName);
                        com4.Parameters.AddWithValue("@Levels", instance.levels);
                        com4.Parameters.AddWithValue("@ParentCode", AccountId);
                        com4.Parameters.AddWithValue("@BookId", instance.BookId);
                        com4.Parameters.AddWithValue("@Active", instance.Active);
                        com4.Parameters.AddWithValue("@AccountType", instance.AccountType);
                        com4.Parameters.AddWithValue("@Expanded", instance.Expanded);
                        com4.Parameters.AddWithValue("@Icon", "favicon.ico");
                        var UserData = DBEntity.GetDataTableBySP(com4, out msg);


                    }

                    if (msg == string.Empty)
                    {
                        sSQL = string.Empty;
                        sSQL = "COMMIT TRANSACTION;";
                        DBEntity.GetSqlData(sSQL, out msg);

                        SqlCommand getcom = new SqlCommand("sp_getGlAccounts");
                        dt = DBEntity.GetDataTableBySP(getcom, out msg);
                        return Ok(dt);
                    }
                    else
                    {
                        RevertTransaction();
                        return Ok(new { message = "Error" });
                    }
                }


                }
                catch (Exception)
                {
                   
                }
           

            return Ok(new { Message = "Rollback" });

        }

      
        [HttpPost]
        public IActionResult Delete([FromBody] GlAccounts instance)
        {
            
                try
                {

                        msg = string.Empty;
                        sSQL = string.Empty;
                        sSQL = " BEGIN TRANSACTION; ";
                        DBEntity.GetSqlData(sSQL, out msg);

               
                var GetAccountEntry = DBEntity.getField("select * from GL_Voucher_Accounts where Account_ID = "+ instance.accountId+ "").ToString();

                if (GetAccountEntry != "")
                    {
                        return Ok(new { Message = "GL Voucher Error" });
                    }
                    else
                    {
                       

                    var RecordDelete = DBEntity.getField("select * from GL_Accounts where Account_ID = " + instance.accountId + "").ToString();

                    if (RecordDelete == "")
                    {
                        return Ok(new { Message = "Account Deleted not successfully" });
                    }

                    else
                    {
                        DBEntity.ExecuteNonQuery("delete from GL_Accounts where Account_ID =" + RecordDelete + "", out msg);
                        if (msg == string.Empty)
                        {
                            sSQL = string.Empty;
                            sSQL = "COMMIT TRANSACTION;";
                            DBEntity.GetSqlData(sSQL, out msg);


                            return Ok(new { Message = "Account Deleted successfully" });
                        }
                        else
                        {
                            RevertTransaction();
                            return Ok(new { Message = "Account Deleted not successfully" });
                        }
                    }
                    }
                }
                catch (Exception)
                {
                RevertTransaction();
                return Ok(new { Message = "Account Deleted not successfully" });
            }
           
            //return Ok(new { Message = "Rollback" });
        }




        private void RevertTransaction()
        {
            sSQL = string.Empty;
            sSQL = "ROLLBACK TRANSACTION;";
            DBEntity.GetSqlData(sSQL, out msg);
           
        }

    }
}
