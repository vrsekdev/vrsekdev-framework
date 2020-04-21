using VrsekDev.Excel.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;

namespace VrsekDev.Excel.EPPlus
{
    public class EPPlusExcelParser : IExcelParser
    {
        public IExcelReader ParseFile(FileInfo fileInfo)
        {
            return new EPPlusExcelReader(new ExcelPackage(fileInfo));
        }

        public IExcelReader ParseStream(Stream stream)
        {
            return new EPPlusExcelReader(new ExcelPackage(stream));
        }
    }
}
