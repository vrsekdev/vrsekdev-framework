using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.Templates.Frest.Models;

namespace VrsekDev.Blazor.Templates.Frest.Components
{
    public partial class Livicon
    {
        private ElementReference iconRef;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("vrsekdev.frest.addLiviconEvo", iconRef, new LiviconConfiguration
                {
                    Name = Icon,
                    Style = Style,
                    Duration = 0.85,
                    StrokeWidth = "1.3px",
                    EventOn = "none",
                    StrokeColor = "#8494A7",
                    SolidColor = "#8494A7",
                    FillColor = "#D4EBF9",
                    StrokeColorAlt = "#E67E22"
                });
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task HandleMouseEnterEventAsync()
        {
            await JSRuntime.InvokeVoidAsync("vrsekdev.frest.stopLiviconEvo", iconRef);
            await JSRuntime.InvokeVoidAsync("vrsekdev.frest.playLiviconEvo", iconRef);
        }
    }
}
