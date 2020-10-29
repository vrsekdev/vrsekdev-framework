using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Templates.Frest.Components
{
    public partial class FrestTooltip
    {
        private ElementReference tooltipRef;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("vrsekdev.frest.initTooltip", tooltipRef);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
