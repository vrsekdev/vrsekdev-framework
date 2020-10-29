using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Templates.Frest.Components
{
    public partial class FrestNavBookmarkSearch
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected async Task ToggleVisibleAsync()
        {
            isVisible = !isVisible;
            await InvokeAsync(StateHasChanged);
            if (isVisible)
            {
                await JSRuntime.InvokeVoidAsync("vrsekdev.frest.focusElement", inputReference);
            }
        }
    }
}
