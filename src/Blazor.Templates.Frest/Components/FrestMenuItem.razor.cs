using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Templates.Frest.Components
{
    public partial class FrestMenuItem
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected override void OnParametersSet()
        {
            if (ChildContent != null)
            {
                hasSubClass = "has-sub";
            }

            base.OnParametersSet();
        }

        protected async Task HandleClickAsync()
        {
            if (ChildContent != null)
            {
                isOpen = !isOpen;
                openClass = isOpen ? "open" : "";

                string function = "vrsekdev.frest." + (isOpen ? "menuItemCollapse" : "menuItemExpand");
                await JSRuntime.InvokeVoidAsync(function, menuRef);
            }
        }

        protected Task HandleMouseEnterAsync()
        {
            hoverClass = "hover";

            return iconRef?.HandleMouseEnterEventAsync() ?? Task.CompletedTask;
        }

        protected Task HandleMouseLeaveAsync()
        {
            hoverClass = "";

            return iconRef?.HandleMouseLeaveEventAsync() ?? Task.CompletedTask;
        }
    }
}
