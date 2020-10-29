using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Templates.Frest.Stores
{
    public class MainMenuStore
    {
        public virtual string OpenedNavigationId { get; set; }

        public virtual bool IsCollapsed { get; set; }

        public virtual bool IsExpanded { get; set; }

        public virtual string IconStyle { get; set; } = "lines";

    }
}
