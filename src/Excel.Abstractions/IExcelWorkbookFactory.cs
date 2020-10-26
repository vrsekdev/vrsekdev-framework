using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Excel.Abstractions
{
    public interface IExcelWorkbookFactory
    {
        IExcelWorkbookManager Create();
    }
}
