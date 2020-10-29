using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.Templates.Frest.Models;

namespace VrsekDev.Blazor.Templates.Frest.Shared
{
    public partial class FrestLayout<TNavBar, TLogo, TMainMenu, TFooter>
        where TNavBar : IComponent
        where TLogo : IComponent
        where TMainMenu : IComponent
        where TFooter : IComponent
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                mainMenuParameters.MouseEnterEvent += HandleChange;
                mainMenuParameters.MouseLeaveEvent += HandleChange;
            }

            base.OnAfterRender(firstRender);
        }

        private string GetMainMenuExpandedClass()
        {
            return mainMenuParameters.IsCollapsed && mainMenuParameters.IsExpanded ? "expanded" : "";
        }

        private async void HandleChange(object sender, EventArgs e)
        {
            mainMenuExpandedClass = GetMainMenuExpandedClass();
            await InvokeAsync(StateHasChanged);
        }

        private async Task HandleMainMenuCollapseClick()
        {
            mainMenuParameters.IsCollapsed = !mainMenuParameters.IsCollapsed;

            await JSRuntime.InvokeVoidAsync("vrsekdev.frest.mainMenuToggleCollapse");
        }

        private RenderFragment NavBar() => builder =>
        {
            builder.OpenComponent<TNavBar>(0);
            builder.CloseComponent();
        };

        private RenderFragment Logo() => builder =>
        {
            builder.OpenComponent<TLogo>(0);
            builder.CloseComponent();
        };

        private RenderFragment MainMenu() => builder =>
        {
            builder.OpenComponent<TMainMenu>(0);
            builder.CloseComponent();
        };

        private RenderFragment Footer() => builder =>
        {
            builder.OpenComponent<TFooter>(0);
            builder.CloseComponent();
        };
    }
}
