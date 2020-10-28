using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Templates.Frest.Components
{
    public partial class NavItem
    {
        private Livicon iconRef;

        protected Task HandleMouseEnterEventAsync()
        {
            return iconRef?.HandleMouseEnterEventAsync() ?? Task.CompletedTask;
        }
    }
}
