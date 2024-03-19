namespace ExcelExportApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ExportData exportData = new ExportData();
            //Call the method in the class
            exportData.GetData();
        }
    }
}