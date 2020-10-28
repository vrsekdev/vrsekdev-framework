using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Templates.Frest.Shared
{
    public partial class FrestLayout<TNavBar, TLogo, TMainMenu, TFooter>
        where TNavBar : IComponent
        where TLogo : IComponent
        where TMainMenu : IComponent
        where TFooter : IComponent
    {
    }
}
