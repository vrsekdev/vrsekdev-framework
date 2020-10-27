using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Excel.Abstractions;

namespace VrsekDev.Excel.EPPlus
{
    internal class EPPlusExcelWorkbookFactory : IExcelWorkbookFactory
    {
        public IExcelWorkbookManager Create()
        {
            return new EPPlusExcelWorkbookManager(new ExcelPackage());
        }
    }
}
