using Aspose.Cells;

namespace ProyectoERP.Helpers
{
    public class HelperExcelToPdf
    {
        public string ConvertToPdf(string excelFilePath, string pdfFilePath)
        {
            // Creo instancia del objeto Workbook con el archivo de Excel
            Workbook workbook = new Workbook(excelFilePath);
            // Guardo documento en formato PDF
            workbook.Save(pdfFilePath, SaveFormat.Pdf);
            return pdfFilePath;
        }
    }
}
