using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Extensions;
using VrsekDev.Blazor.Mobx.Samples.Models;

namespace VrsekDev.Blazor.Mobx.Samples.Shared.Stores
{
    public class AppStore
    {
        public AppStore()
        {
            NavigationItems = new List<NavigationItem>
            {
                new NavigationItem
                {
                    Title = "Home",
                    Link = "/",
                    Glyph = "oi oi-home"
                },
                new NavigationItem
                {
                    Title = "Counter",
                    Link = "/counter",
                    Glyph = "oi oi-plus"
                },
                new NavigationItem
                {
                    Title = "Person",
                    Link = "/person",
                    Glyph = "oi oi-plus"
                },
                new NavigationItem
                {
                    Title = "Add NavLink",
                    Link = "/addNavLink",
                    Glyph = "oi oi-plus"
                },
                new NavigationItem
                {
                    Title = "Todos",
                    Link = "/todos",
                    Glyph = "oi oi-plus"
                }
            }.ToObservableCollection();
        }

        public virtual bool IsLoading { get; set; } = true;

        public virtual IObservableCollection<NavigationItem> NavigationItems { get; set; }
    }
}
