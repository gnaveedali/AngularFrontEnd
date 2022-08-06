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
using DBEntity = Data_Access_Layer.DBEntity;
using static Standard_Trading_API.Model.MenusDataType;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportsController : Controller
    {
        public string sSQL, msg;
        public DataTable dt;
       
        [HttpPost]
        public IActionResult GetReports([FromBody] Menus instance)
        {


            sSQL = String.Empty;
            msg = String.Empty;
            dt = new DataTable();

            sSQL = "select m.Menu_ID as menuId , Menu_Name as menuName ,Child as child,Icon as icon from Menus m inner join Group_Menus gm  on gm.Menu_ID = m.Menu_ID where Parent_ID =" + instance.parentId+ " and gm.Group_ID = " + instance.userGroupId + " and gm.Enable = 1";

            dt = DBEntity.fillComboBox(sSQL);


            return Ok(dt);
        }
    }
}
