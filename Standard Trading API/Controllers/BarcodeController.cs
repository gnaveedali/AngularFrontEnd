using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Standard_Trading_API.Model;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static Standard_Trading_API.Model.BarcodeDataType;
using DBEntity = Data_Access_Layer.DBEntity;
using Utility = Data_Access_Layer.Utility;
using ZebraPrinter = Data_Access_Layer.ZebraPrinter;

namespace Standard_Trading_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BarcodeController : Controller
    {
        string msg = string.Empty;
        string sSQL = string.Empty;
        public DataTable dt;

        [HttpGet]
        public ActionResult  GetMultiBarcode()
        {
            dt = new DataTable();
            sSQL = "Select Barcode_ID as BarcodeId,Item_Code as ItemCode,Barcode as Barcode from Multi_Barcode";

            dt = DBEntity.fillComboBox(sSQL);

            return Ok(dt);
        }
        
        [HttpPost]
        public ActionResult PrintBarcode([FromBody] MultiBarcode Instance)
        {
           
            string printerName = "ZDesigner TLP 2844";
            string barcode = "Engr M Ali";

            var sb = new StringBuilder();

            sb.AppendLine();

            sb.AppendLine("N");// clear Image Buffer
            sb.AppendLine("q200"); // Set Form Width
            sb.AppendLine("Q280,32-7");// set form length

            //sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A0,10,0,3,1,0,N,\"" + CompanyName + "\""));
            //sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A0,50,0,2,1,1,N,\"" + title + "\""));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "B10,20,0,1,2,4,90,B,\"" + barcode + "\""));
            //sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A0,190,0,3,1,1,N,\"Rate : " + Rate + "\""));

            sb.AppendLine("P1,2");
            ZebraPrinter.SendStringToPrinter(printerName, sb.ToString());



            //catch (Exception ex)
            //{

            //}


            return Ok();
        }

    }
}
