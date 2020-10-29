using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.Templates.Frest.Models;

namespace VrsekDev.Blazor.Templates.Frest.Components
{
    public partial class FrestMainMenu
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("$.app.menu.init", Store.IsCollapsed);
            }

            base.OnAfterRender(firstRender);
        }

        protected async override Task OnParametersSetAsync()
        {
            cssClass = await GetMenuClassAsync();
        }

        protected async Task<string> GetMenuClassAsync()
        {
            await JSRuntime.InvokeVoidAsync("console.log", $"{Store.IsCollapsed} + {Store.IsExpanded}");

            switch ((Store.IsCollapsed, Store.IsExpanded))
            {
                case var t when t.IsCollapsed && t.IsExpanded:
                    await JSRuntime.InvokeVoidAsync("vrsekdev.frest.mainMenuExpand");
                    return "menu-collapsed-open";
                case var t when t.IsCollapsed && !t.IsExpanded:
                    await JSRuntime.InvokeVoidAsync("vrsekdev.frest.mainMenuCollapse");
                    return "menu-collapsed";
                default:
                    return "open";
            }
        }
    }
}
