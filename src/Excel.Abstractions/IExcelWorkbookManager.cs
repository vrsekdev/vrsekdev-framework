using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VrsekDev.Excel.Abstractions
{
    public interface IExcelWorkbookManager : IDisposable
    {
        IExcelWorksheetReader OpenWorksheet(string name);
        IExcelWorksheetReader OpenWorksheet(int index);

        IExcelWorksheetWriter CreateWorksheet(string name);

        Stream SaveWorkbook();
        void SaveWorkbook(Stream stream);
    }
}
