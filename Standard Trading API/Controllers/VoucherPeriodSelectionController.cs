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
using Data_Access_Layer;
using DBEntity = Data_Access_Layer.DBEntity;
using static Standard_Trading_API.Model.VoucherDataType;
using Utility = Data_Access_Layer.Utility;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VoucherPeriodSelectionController : Controller
    {
        string msg = string.Empty;
        string sSQL = string.Empty;
        public DataTable dt;

        VoucherPeriodSelectDB voucherPeriodSelectMgr = new VoucherPeriodSelectDB();
      

        [HttpPost]
        public IActionResult GetVoucherType([FromBody] GLVoucherType Instance)
        {
            if (Instance != null)
            {
                dt = new DataTable();

                if(Instance.voucherType == "INV")
                {
                    dt = DBEntity.fillComboBox("Select DataValue_ID as dataValueId,DataValue_Name as dataValueName ,Description from DataValues where ValueSet_ID=(select ValueSet_ID from ValueSets where ValueSet_Name='Invoice Type')");
                      return Ok(dt);
                }
               

               

            }
            return Ok(null);

        }
        [HttpPost]
        public IActionResult GetPeriod([FromBody] GLVoucherType Instance)
        {
            string msg = string.Empty;
            string sSQL = string.Empty;
            dt = new DataTable();
            VoucherData voucherData = new VoucherData();

            sSQL =   "select * from gl_periods where company_id=" + Utility.CompanyID +
                " and '" + Instance.FrontDate.ToString(Utility.dateFormat) + "' between period_start_date and period_end_date and period_closed='False'";
            dt = DBEntity.fillComboBox(sSQL);

            if(dt.Rows.Count == 0)
            {
               return Ok(new {Message = "Period is not define or has been closed for entry"});
            }
            else
            {

                
                voucherData.sPeriod = DBEntity.getField("select period_id from gl_periods where company_id=" + Utility.CompanyID + " and '" + Instance.FrontDate.ToString("dd-MMM-yy") + "' between period_start_date and period_end_date and period_closed='False'").ToString();

                voucherData.periodStDate = (DateTime)DBEntity.getField("select period_start_date from gl_periods where company_id=" + Utility.CompanyID + " and period_id='" + voucherData.sPeriod + "'");

                voucherData.periodEnDate = Convert.ToDateTime(Convert.ToDateTime(DBEntity.getField("select period_end_date from gl_periods where company_id=" + Utility.CompanyID + " and period_id='" + voucherData.sPeriod + "'")).ToString(Utility.dateFormat) + " 11:59:59");


                return Ok(voucherData);


            }
            
            return Ok(null);
        }

    }
}
