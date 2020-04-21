using VrsekDev.Excel.Abstractions;
using VrsekDev.Excel.Abstractions.Exceptions;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VrsekDev.Excel.EPPlus
{
    public class EPPlusExcelReader : IExcelReader
    {
        private readonly ExcelPackage excelPackage;

        public EPPlusExcelReader(ExcelPackage excelPackage)
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

        public void Dispose()
        {
            excelPackage.Dispose();
        }
    }
}
