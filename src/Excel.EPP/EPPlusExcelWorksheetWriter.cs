using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Excel.Abstractions;

namespace VrsekDev.Excel.EPPlus
{
    internal class EPPlusExcelWorksheetWriter : IExcelWorksheetWriter
    {
        private readonly ExcelWorksheet worksheet;

        public EPPlusExcelWorksheetWriter(ExcelWorksheet worksheet)
        {
            this.worksheet = worksheet;
        }

        public void WriteString(int row, int column, string value)
        {
            worksheet.Cells[row, column].Value = value;
        }

        public void WriteInt(int row, int column, int value)
        {
            worksheet.Cells[row, column].Value = value;
        }
    }
}
