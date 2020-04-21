using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Excel.Abstractions
{
    public interface IExcelReader : IDisposable
    {
        IExcelWorksheetReader OpenWorksheet(int index);
        IExcelWorksheetReader OpenWorksheet(string name);
    }
}
