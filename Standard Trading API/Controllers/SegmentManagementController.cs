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
using static Standard_Trading_API.Model.SegmentManagementDataType;
using DBEntity = Data_Access_Layer.DBEntity;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SegmentManagementController : Controller
    {
        string sSQL, msg;
        DataTable dt ;

        
        [HttpGet]
        //[Authorize]
        public IActionResult GetAllValueSets()
        {
            sSQL = string.Empty;
            msg = string.Empty;
            sSQL = "select ValueSet_ID as valueSetId ,ValueSet_Name as valueSetName from ValueSets";
            dt= DBEntity.fillComboBox(sSQL);
           
            return Ok(dt);
        }
        [HttpPost]
        public DataTable  GetDataValueById([FromBody] ValueSets instance)
        {
            if(instance != null)
            {
                DataTable dtReturn = new DataTable();
                SqlCommand com = new SqlCommand("sp_GetDataValues");
                com.Parameters.AddWithValue("@ValueSetId", instance.ValueSetID);
                dtReturn = DBEntity.GetDataTableBySP(com, out msg);

                return dtReturn;
            }
            return null;
        }

       
        [HttpPost]
        public IActionResult SubmitDataValue([FromBody]  DataValues instance)
        {
            msg = string.Empty;
            sSQL = string.Empty;
            try
            {
             

                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);


                SqlCommand com = new SqlCommand("sp_DataValueAddOrEdit");
                   com.Parameters.AddWithValue("@DataValueId", instance.DataValueID);
                   com.Parameters.AddWithValue("@ValueSetId", instance.ValueSetID);
                   com.Parameters.AddWithValue("@DataValueName", instance.DataValueName);
                   com.Parameters.AddWithValue("@Description", instance.Description);
                   com.Parameters.AddWithValue("@TransactionType", instance.TransactionType);
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
                    
                }
               

            }
            catch (Exception)
            {
                RevertTransaction();

            }
            

            return Ok(new { Message = "Rollback" });

        }
        [HttpPost]
        public IActionResult DeleteDataValue([FromBody] DataValues instance)
        {
            msg = string.Empty;
            sSQL = string.Empty;

            try
            {
                sSQL = " BEGIN TRANSACTION; ";
                DBEntity.GetSqlData(sSQL, out msg);

                msg = string.Empty;
                sSQL = string.Empty;


                sSQL = "Delete from DataValues where DataValue_ID =" + instance.DataValueID + "";
                DBEntity.GetSqlData(sSQL, out msg);
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
