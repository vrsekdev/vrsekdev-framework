using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Excel.Abstractions.Exceptions
{
    public class ExcelException : Exception
    {
        public ExcelException(string message) : base(message)
        {

        }
    }
}
