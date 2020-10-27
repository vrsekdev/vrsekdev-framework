using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Extensions;
using VrsekDev.Blazor.Mobx.Samples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Samples
{
    public class DefaultAppStore : IAppStore
    {
        public DefaultAppStore()
        {
            NavigationItems = new List<INavigationItem>
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

        public IObservableCollection<INavigationItem> NavigationItems { get; set; }
    }
}
