using VrsekDev.Excel.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;

namespace VrsekDev.Excel.EPPlus
{
    internal class EPPlusExcelParser : IExcelParser
    {
        public IExcelWorkbookManager ParseFile(FileInfo fileInfo)
        {
            return new EPPlusExcelWorkbookManager(new ExcelPackage(fileInfo));
        }

        public IExcelWorkbookManager ParseStream(Stream stream)
        {
            return new EPPlusExcelWorkbookManager(new ExcelPackage(stream));
        }
    }
}
