using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Templates.MajesticAdmin.Shared
{
    public partial class MaMainLayout<TLogo, TNavBar, TSideBar, TFooter>
        where TLogo : IComponent
        where TNavBar : IComponent
        where TSideBar : IComponent
        where TFooter : IComponent
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private async Task ToggleSideBarAsync()
        {
            await JSRuntime.InvokeVoidAsync("vrsekdev.majesticAdmin.toggleSideBar");
        }
    }
}
