using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Excel.Abstractions;

namespace VrsekDev.Excel.EPPlus.DependencyInjection
{
    public static class EPPlusExcelInstaller
    {
        public static void AddEPPlusExcel(this IServiceCollection services)
        {
            services.AddTransient<IExcelParser, EPPlusExcelParser>();
        }
    }
}
