using MiniExcelLibs;
using System.Configuration;
using System.Data.SqlClient;

namespace ExcelExportApp
{
    public class ExportData
    {
        public void GetData()
        {
            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string LogFolder = @"D:\ExcelExport\Logs";
            SqlConnection sqlCon; 
            try
            {                
                int maxLoopCounter = 0;
                string TableName = "dummy_table";
                //Initialize sheets variable to store the data sheet wise
                var sheets = new Dictionary<string, object>();

                string excelFilePath = @"D:\ExcelExport\DataFile.xlsx";

                //Get connection string from App.config
                string connectionString = ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
                SqlCommand sqlcmd;

                //Create sqlConnection
                sqlCon = new SqlConnection(connectionString);
                sqlCon.Open();
                sqlcmd = new SqlCommand("GenerateTempTable", sqlCon);
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
                //Setting CommandTimeout = 0 so that the connection won't close while executing the stored procedure
                sqlcmd.CommandTimeout = 0;
                //Store value from stored procedure into maxLoopCounter
                maxLoopCounter = Convert.ToInt32(sqlcmd.ExecuteScalar());

                //Fetch the record depending on the number of part value     
                for (int i = 1; i <= maxLoopCounter; i++)
                {
                    string sheetName = "Sheet" + i.ToString();
                    //Establish Connection
                    sqlCon = new SqlConnection(connectionString);
                    sqlCon.Open();
                    //Sql Command to extract the data based on part variable. First 1 million records will be fetched and stored in the first sheet and so on.
                    sqlcmd = new SqlCommand(@"select a.* from " + TableName + " a inner join temp_" + TableName + " b on a.id = b.id where b.part = " + i + "", sqlCon);
                    sheets.Add(sheetName, sqlcmd.ExecuteReader());
                }
                //Finally, Saving the excel file with all the sheets.
                MiniExcel.SaveAs(excelFilePath, sheets);
            }
            catch (Exception exception)
            {
                //Any exception while executing the code, will be logged to the ErrorLog file
                using (StreamWriter sw = File.CreateText(LogFolder + "\\" + "ErrorLog " + currentDate + ".log"))
                {
                    sw.WriteLine(exception.ToString());
                }
            }
        }
    }
}
