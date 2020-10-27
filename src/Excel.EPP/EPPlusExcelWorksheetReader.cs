using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Excel.Abstractions;

namespace VrsekDev.Excel.EPPlus
{
    internal class EPPlusExcelWorksheetReader : IExcelWorksheetReader
    {
        private readonly ExcelWorksheet worksheet;

        public EPPlusExcelWorksheetReader(ExcelWorksheet worksheet)
        {
            this.worksheet = worksheet;
        }

        public bool HasValue(int row, int column)
        {
            object value = worksheet.Cells[row, column].Value;
            return value != null;
        }

        public string ReadString(int row, int column)
        {
            return worksheet.Cells[row, column].Text;
        }
    }
}
