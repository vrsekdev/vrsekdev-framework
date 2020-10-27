using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Excel.Abstractions
{
    public interface IExcelWorksheetWriter
    {
        void WriteString(int row, int column, string value);

        void WriteInt(int row, int column, int value);
    }
}
