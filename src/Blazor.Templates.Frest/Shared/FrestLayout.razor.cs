using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.Mobx;
using VrsekDev.Blazor.Templates.Frest.Models;
using VrsekDev.Blazor.Templates.Frest.Stores;

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

        [Inject]
        public IStoreAccessor<MainMenuStore> MainMenuAccessor { get; set; }

        public MainMenuStore Store => MainMenuAccessor.Store;

        protected override void OnInitialized()
        {
            MainMenuAccessor.SetConsumer(this);
        }

        private async Task HandleMainMenuOnMouseleave()
        {
            Store.IsExpanded = false;
            await JSRuntime.InvokeVoidAsync("console.log", $"leaving {Store.IsCollapsed} + {Store.IsExpanded}");
        }

        private async Task HandleMainMenuOnMouseenter()
        {
            Store.IsExpanded = true;
            await JSRuntime.InvokeVoidAsync("console.log", $"entering {Store.IsCollapsed} + {Store.IsExpanded}");
        }

        private async Task HandleMainMenuCollapseClick()
        {
            Store.IsCollapsed = !Store.IsCollapsed;

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
