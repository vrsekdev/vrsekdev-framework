using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Excel.Abstractions
{
    public interface IExcelWorksheetReader
    {
        bool HasValue(int row, int column);
        string ReadString(int row, int column);
    }
}
