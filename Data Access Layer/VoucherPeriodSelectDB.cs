using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Data_Access_Layer
{
    public class VoucherPeriodSelectDB
    {
        public DataTable fetchComboboxdata()
        {
            DataTable dataTable = new DataTable();
            string strSql;
            try
            {
                strSql = "Select Voucher_Type,Description,Ref_Narration from GL_Voucher_Type " +
                         "where Active = 'True';";
                dataTable = DBEntity.fillComboBox(strSql);
            }
            catch (Exception E) { Console.WriteLine("Exception IS:" + E); }

            return dataTable;
        }

        public static bool validatePeriodDate(DateTime dt)
        {
            DataTable dataTable = new DataTable();
            string strSql;
            strSql = "select * from gl_periods where company_id=" + Utility.CompanyID +
                " and '" + dt.ToString(Utility.dateFormat) + "' between period_start_date and period_end_date and period_closed='False'";
            dataTable = DBEntity.fillComboBox(strSql);
            if (dataTable.Rows.Count == 0)
                return false;
            else
                return true;
        }
    }
}
