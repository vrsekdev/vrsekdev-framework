using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Templates.MajesticAdmin.Shared
{
    public partial class MajesticAdminLayout<TNavBar, TSideBar, TFooter>
        where TNavBar : IComponent
        where TSideBar : IComponent
        where TFooter : IComponent
    {
    }
}
