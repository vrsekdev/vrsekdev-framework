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
                Parameters.MouseEnterEvent += HandleChange;
                Parameters.MouseLeaveEvent += HandleChange;

                await JSRuntime.InvokeVoidAsync("$.app.menu.init", IsCollapsed);
            }

            base.OnAfterRender(firstRender);
        }

        protected async Task<string> GetMenuClassAsync()
        {
            await JSRuntime.InvokeVoidAsync("console.log", $"{Parameters.IsCollapsed} + {Parameters.IsExpanded}");

            switch ((Parameters.IsCollapsed, Parameters.IsExpanded))
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

        private async void HandleChange(object sender, EventArgs e)
        {
            cssClass = await GetMenuClassAsync();
            await JSRuntime.InvokeVoidAsync("console.log", cssClass);

            await InvokeAsync(StateHasChanged);
        }
    }
}
