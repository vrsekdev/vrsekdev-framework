using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VrsekDev.Excel.Abstractions
{
    public interface IExcelParser
    {
        IExcelWorkbookManager ParseFile(FileInfo fileInfo);

        IExcelWorkbookManager ParseStream(Stream stream);
    }
}
