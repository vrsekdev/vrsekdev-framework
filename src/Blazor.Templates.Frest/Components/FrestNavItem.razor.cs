using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Templates.Frest.Components
{
    public partial class FrestNavItem
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected override void OnInitialized()
        {
            Autorun(async store =>
            {
                if (ChildContent != null)
                {
                    bool shouldBeOpen = store.OpenedNavigationId == menuRef.Id ? true : false;
                    if (shouldBeOpen != isOpen)
                    {
                        string function = "vrsekdev.frest." + (isOpen ? "menuItemCollapse" : "menuItemExpand");
                        await JSRuntime.InvokeVoidAsync(function, menuRef);
                    }
                    isOpen = shouldBeOpen;
                    await InvokeAsync(StateHasChanged);
                }
            });
        }

        protected override void OnParametersSet()
        {
            if (ChildContent != null)
            {
                hasSubClass = "has-sub";
            }

            base.OnParametersSet();
        }

        protected Task ToggleOpenAsync()
        {
            if (ChildContent != null)
            {
                Store.OpenedNavigationId = !isOpen ? menuRef.Id : null;
            }

            return Task.CompletedTask;
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
