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
using static Standard_Trading_API.Model.UserDetailsDataType;
using Data_Access_Layer;
using DBEntity = Data_Access_Layer.DBEntity;
using UserDetailsDataType = Standard_Trading_API.Model.UserDetailsDataType;

namespace Standard_Trading_API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]

    public class UserDetailsController : ControllerBase
    {
       
        string msg = string.Empty;
        string sSQL = string.Empty;
        public string ImagePath;
        public DataTable dt;
        bool isAuthenticated = false,canAccessUserProfile = false,canAccessDashboard = false;

        [HttpPost]
        public IActionResult Login([FromBody] UserDetails userDetails)
        {
            string Token;
             dt = new DataTable();
            
           
            try
            {
                msg = string.Empty;
                sSQL = string.Empty;


                sSQL = "select Id as UserID, CONCAT(fname, lname) as UserName,email as Email,Position as Role,Description,claimType,claimValue,Group_ID as GroupID from UserDetails ud inner join userClaims uc on uc.UserId = ud.id where email = '" + userDetails.email + "' and password = '" + Data_Access_Layer.Utility.Encrypt(userDetails.password) + "'";

                dt = DBEntity.GetSqlData(sSQL, out msg);


                Token = jwtSecurityToken.jwtsecuritytoken();
                if (Token != "")
                {
                    isAuthenticated = true;

                    
                    Data_Access_Layer.Utility.CompanyID = Convert.ToInt32(DBEntity.getField("select Company_ID from sys_Companys where Company_ID = 1"));

                    Data_Access_Layer.Utility.Book_ID = Convert.ToInt32(DBEntity.getField("select Book_id from sys_Companys where Company_ID = 1"));

                    Data_Access_Layer.Utility.User_ID = Convert.ToInt32(dt.Rows[0]["UserID"]);
                }

                UserDetailsDataType.UserAuthenticationObject jWTVM = new UserDetailsDataType.UserAuthenticationObject();


                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    if (dt.Rows[i]["claimType"].ToString() == "canAccessDashboard")
                    {
                        canAccessDashboard = Convert.ToBoolean(dt.Rows[i]["claimValue"]);
                    }
                    if (dt.Rows[i]["claimType"].ToString() == "canAccessUserProfile")
                    {
                        canAccessUserProfile = Convert.ToBoolean(dt.Rows[i]["claimValue"]);

                    }
                }

                jWTVM.bearerToken = Token;
                jWTVM.email = dt.Rows[0]["Email"].ToString();
                jWTVM.userName = dt.Rows[0]["username"].ToString();
                jWTVM.role = dt.Rows[0]["Role"].ToString();
                jWTVM.isAuthenticated = isAuthenticated;
                jWTVM.canAccessDashboard = canAccessDashboard;
                jWTVM.canAccessUserProfile = canAccessUserProfile;
                jWTVM.groupID = 1;

                return Ok(jWTVM);
            }

            catch
            {
                return BadRequest
                ("An error occurred in generating the token");
            }
            return Unauthorized();
        }
        [HttpPost]
        public IActionResult SaveUserDetail([FromBody] UserDetails userDetails)
        {
            try
            {
                 ImagePath = userDetails.Photo;


                msg = string.Empty;
                sSQL = string.Empty;

                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);

               
                SqlCommand com = new SqlCommand("sp_UserDetailsAddUserDetails");
                com.Parameters.AddWithValue("@fname", userDetails.fname);
                com.Parameters.AddWithValue("@lname", userDetails.lname);
                com.Parameters.AddWithValue("@Position", userDetails.position);
                com.Parameters.AddWithValue("@Address", userDetails.addres);
                com.Parameters.AddWithValue("@Contact", userDetails.contact);
                com.Parameters.AddWithValue("@City", userDetails.city);
                com.Parameters.AddWithValue("@Country", userDetails.country);
                com.Parameters.AddWithValue("@email", userDetails.email);
                com.Parameters.AddWithValue("@canAccessDashboard", userDetails.canAccessDashboard);
                com.Parameters.AddWithValue("@canAccessUserProfile", userDetails.canAccessUserProfile);
                com.Parameters.AddWithValue("@password", Data_Access_Layer.Utility.Encrypt(userDetails.password));
                com.Parameters.AddWithValue("@Photo", userDetails.Photo);

                var UserData = DBEntity.GetDataTableBySP(com, out msg);

                if (msg == string.Empty)
                {
                    sSQL = string.Empty;
                    sSQL = "COMMIT TRANSACTION;";
                    DBEntity.GetSqlData(sSQL, out msg);
                    return Ok(new { message = "Success" });
                }
                else
                {
                    RevertTransaction("Add");
                    return Ok(new { message = "Error" });
                }
            }
            catch
            {
                RevertTransaction("Add");
                return Ok(new { message = "Error" });
            }
            return Ok();

        }


        [HttpPut]
        public IActionResult UpdateUserDetail([FromBody] UserDetails userDetails)
        {
            try 
            { 
            msg = string.Empty;
            sSQL = string.Empty;

            sSQL = " BEGIN TRANSACTION; ";
            DBEntity.GetSqlData(sSQL, out msg);
                SqlCommand com = new SqlCommand("sp_UserDetailsEditUserDetails");
                com.Parameters.AddWithValue("@UserId", userDetails.Id);
                com.Parameters.AddWithValue("@fname", userDetails.fname);
                com.Parameters.AddWithValue("@lname", userDetails.lname);
                com.Parameters.AddWithValue("@Position", userDetails.position);
                com.Parameters.AddWithValue("@Address", userDetails.addres);
                com.Parameters.AddWithValue("@Contact", userDetails.contact);
                com.Parameters.AddWithValue("@City", userDetails.city);
                com.Parameters.AddWithValue("@Country", userDetails.country);
                com.Parameters.AddWithValue("@email", userDetails.email);
                com.Parameters.AddWithValue("@canAccessDashboard", userDetails.canAccessDashboard);
                com.Parameters.AddWithValue("@canAccessUserProfile", userDetails.canAccessUserProfile);
                com.Parameters.AddWithValue("@password", Data_Access_Layer.Utility.Encrypt(userDetails.password));
                com.Parameters.AddWithValue("@Photo", userDetails.Photo);

                var UserData = DBEntity.GetDataTableBySP(com, out msg);

                if (msg == string.Empty)
                {
                    sSQL = string.Empty;
                    sSQL = "COMMIT TRANSACTION;";
                    DBEntity.GetSqlData(sSQL, out msg);
                    return Ok(new { message = "Success" });
                }
                else
                {
                    RevertTransaction("Edit");
                    return Ok(new { message = "Error" });
                }
            }
            catch
            {
                RevertTransaction("Edit");
                return Ok(new { message = "Error" });
            }
          


            return Ok();
        }
        [HttpGet]
        public ActionResult  GetAlluserDetail()
        {
              dt = new DataTable();
             sSQL = string.Empty;
            sSQL = "select * from UserDetails ";

             dt = DBEntity.GetSqlData(sSQL, out msg);

            List<UserDetails> userlist = new List<UserDetails>();
            
            for(int i=0;i<dt.Rows.Count;i++) 
            {
                userlist.Add(new UserDetails
                {
                    Id = Convert.ToInt32(dt.Rows[i]["Id"]),
                    fname = dt.Rows[i]["fname"].ToString() +" "+ dt.Rows[i]["lname"].ToString(),
                    position = dt.Rows[i]["Position"].ToString(),
                    addres = dt.Rows[i]["Addres"].ToString(),
                    email = dt.Rows[i]["email"].ToString(),
                    Photo = dt.Rows[i]["Photo"].ToString(),
                    password = Data_Access_Layer.Utility.Decrypt(dt.Rows[i]["password"].ToString()),

                }); ;
            }


            return Ok(userlist);
        }


        [HttpPost]
        public ActionResult GetuserDetailById([FromBody] UserDetails instance)
        {
            msg = string.Empty;
            sSQL = string.Empty;
            dt = new DataTable();

            if (instance != null)
            {

                sSQL = "SELECT UserDetails.*,userClaims.* FROM UserDetails INNER JOIN userClaims ON UserDetails.Id = userClaims.UserId WHERE UserDetails.Id = " + instance.Id + "";
                dt = DBEntity.GetSqlData(sSQL, out msg);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["Description"].ToString() == "Can Access Dashboard" && Convert.ToBoolean(dt.Rows[i]["claimValue"]) == true)
                        {
                            canAccessDashboard = true;
                        }
                        else if (dt.Rows[i]["Description"].ToString() == "Can Access User Profile" && Convert.ToBoolean(dt.Rows[i]["claimValue"]) == true)
                        {
                            canAccessUserProfile = true;
                        }
                    }

                    UserDetails userDetails = new UserDetails();

                    userDetails.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                    userDetails.fname = dt.Rows[0]["fname"].ToString();
                    userDetails.lname = dt.Rows[0]["lname"].ToString();
                    userDetails.position = dt.Rows[0]["Position"].ToString();
                    userDetails.addres = dt.Rows[0]["Addres"].ToString();
                    userDetails.email = dt.Rows[0]["email"].ToString();
                    userDetails.Photo = dt.Rows[0]["Photo"].ToString();
                    userDetails.city = Convert.ToInt32(dt.Rows[0]["City"]);
                    userDetails.country = Convert.ToInt32(dt.Rows[0]["Country"]);
                    userDetails.password = Data_Access_Layer.Utility.Decrypt(dt.Rows[0]["password"].ToString());
                    userDetails.canAccessDashboard = canAccessDashboard;
                    userDetails.canAccessUserProfile = canAccessUserProfile;

                    return Ok(userDetails);

                }
            }
            return Ok();
        }
        [HttpPost]
        public ActionResult DeleteUserDetail([FromBody] UserDetails instance)
        {
            msg = string.Empty;
            sSQL = string.Empty;

            if (instance.Id != null)
            {
                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);

                SqlCommand com = new SqlCommand("sp_UserDetailsDelete");
                com.Parameters.AddWithValue("@UserId", instance.Id);
                 var UserData = DBEntity.GetDataTableBySP(com, out msg);

                if(msg != null)
                {
                    var path = "";
                    path = instance.Photo;
                    if (path != "/assets/img/Blank.png")
                    {
                        if ((System.IO.File.Exists(path)))
                        {
                            System.IO.File.Delete(path);

                        }
                    }
                    sSQL = string.Empty;
                    sSQL = "COMMIT TRANSACTION;";
                    DBEntity.GetSqlData(sSQL, out msg);
                    return Ok(new { message = "Success" });

                }
                else
                {
                    RevertTransaction("Delete");
                    return Ok(new { message = "Error" });
                }
            }
            return Ok(new { message = "Error" });
        }

        private void RevertTransaction(string TransactionType)
        {
            if(TransactionType == "Add" )
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
