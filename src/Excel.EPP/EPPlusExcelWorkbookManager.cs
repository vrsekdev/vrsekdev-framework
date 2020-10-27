using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VrsekDev.Excel.Abstractions;
using VrsekDev.Excel.Abstractions.Exceptions;

namespace VrsekDev.Excel.EPPlus
{
    internal class EPPlusExcelWorkbookManager : IExcelWorkbookManager
    {
        private readonly ExcelPackage excelPackage;

        public EPPlusExcelWorkbookManager(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
        }

        public IExcelWorksheetReader OpenWorksheet(int index)
        {
            var worksheet = excelPackage.Workbook.Worksheets.ElementAtOrDefault(index);
            if (worksheet == null)
            {
                throw new ExcelException("Could not find worksheet.");
            }

            return new EPPlusExcelWorksheetReader(worksheet);
        }

        public IExcelWorksheetReader OpenWorksheet(string name)
        {
            var worksheet = excelPackage.Workbook.Worksheets[name];
            if (worksheet == null)
            {
                throw new ExcelException("Could not find worksheet.");
            }

            return new EPPlusExcelWorksheetReader(worksheet);
        }

        public IExcelWorksheetWriter CreateWorksheet(string name)
        {
            var worksheet = excelPackage.Workbook.Worksheets[name];
            if (worksheet != null)
            {
                throw new ExcelException($"Worksheet `{name}` already exists.");
            }

            worksheet = excelPackage.Workbook.Worksheets.Add(name);
            return new EPPlusExcelWorksheetWriter(worksheet);
        }

        public Stream SaveWorkbook()
        {
            MemoryStream ms = new MemoryStream();
            excelPackage.SaveAs(ms);

            return ms;
        }

        public void SaveWorkbook(Stream stream)
        {
            excelPackage.SaveAs(stream);
        }

        public void Dispose()
        {
            excelPackage.Dispose();
        }
    }
}
