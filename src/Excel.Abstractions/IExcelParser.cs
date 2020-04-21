using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VrsekDev.Excel.Abstractions
{
    public interface IExcelParser
    {
        IExcelReader ParseFile(FileInfo fileInfo);

        IExcelReader ParseStream(Stream stream);
    }
}
