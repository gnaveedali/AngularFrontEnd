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
using static Standard_Trading_API.Model.MenusDataType;
using DBEntity = Data_Access_Layer.DBEntity;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserGroupsController : Controller
    {
        string sSQL, msg;
        DataTable dt;

        [HttpPost]
        public DataTable GetAllMenusUGropById([FromBody] Menus instance)
        {
            SqlCommand com = new SqlCommand("sp_UserGroups_GetAllMenusUGropById");
            com.Parameters.AddWithValue("@UserGroupId", instance.userGroupId);
            var menuData = DBEntity.GetDataTableBySP(com, out msg);

            return menuData;
        }

        [HttpGet]
        public DataTable GetAllUserGroups()
        {
            DataTable dt = new DataTable();
            dt = DBEntity.GetSqlData("select User_Group_Id as userGroupId,User_Group_Name as userGroupName from User_Groups", out msg);

            return dt;
        }


        [HttpPost]
        public ActionResult GetAllMenus([FromBody] Menus instance)
        {
            sSQL = String.Empty;
            msg = String.Empty;
            dt = new DataTable();

            sSQL = "select m.Menu_ID as menuId , Menu_Name as menuName ,Child as child,Icon as icon ,Enable as enable from Menus m inner join Group_Menus gm  on gm.Menu_ID = m.Menu_ID where Parent_ID =" + instance.parentId + " and gm.Group_ID = " + instance.userGroupId + " ";

            dt = DBEntity.GetSqlData(sSQL, out msg);

            return Ok(dt);
        }

        [HttpPut]
        public ActionResult UpdateMenus([FromBody]  Menus instance)
        {

            msg = string.Empty;
            sSQL = string.Empty;
            int Enable = 0;

            try
            {
                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);

                if (instance.enable == true)
                    Enable = 1;
                else
                    Enable = 0;



                var UserGroupId = DBEntity.getField("select max(User_Group_ID) from User_Groups where User_Group_Name = '"+ instance.UserGroupName+ "'").ToString();

                var Data = DBEntity.ExecuteNonQuery("Update Group_Menus set Enable =" + Enable + " where Menu_ID =" + instance.menuId + " and Group_ID =" + UserGroupId + "", out msg);



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

                }


            }
            catch (Exception)
            {
                RevertTransaction();

            }


            return Ok(new { Message = "Rollback" });
        }

        private void RevertTransaction()
        {
            sSQL = string.Empty;
            sSQL = "ROLLBACK TRANSACTION;";
            DBEntity.GetSqlData(sSQL, out msg);
        }
    }

}
