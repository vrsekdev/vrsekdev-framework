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
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("$.app.menu.init", IsCollapsed);

            base.OnAfterRender(firstRender);
        }
    }
}
