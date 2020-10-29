using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.Mobx;
using VrsekDev.Blazor.Mobx.Abstractions.Components;
using VrsekDev.Blazor.Templates.Frest.Stores;

namespace VrsekDev.Blazor.Templates.Frest.Components
{
    public partial class FrestMenuItem
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public IStoreAccessor<MainMenuStore> MainMenuStoreAccessor { get; set; }

        protected override void OnInitialized()
        {
            MainMenuStoreAccessor.SetConsumer((IBlazorMobxComponent)this);

            Autorun(async store =>
            {
                bool shouldBeOpen = store.OpenedMenuItemId == menuRef.Id ? true : false;
                await JSRuntime.InvokeVoidAsync("console.log", $"{shouldBeOpen} + {isOpen}");
                if (shouldBeOpen != isOpen)
                {
                    string function = "vrsekdev.frest." + (isOpen ? "menuItemCollapse" : "menuItemExpand");
                    await JSRuntime.InvokeVoidAsync(function, menuRef);
                }
                isOpen = shouldBeOpen;
                await InvokeAsync(StateHasChanged);
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

        protected Task HandleClickAsync()
        {
            if (ChildContent != null)
            {
                Store.OpenedMenuItemId = !isOpen ? menuRef.Id : null;
                isOpen = !isOpen;
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
