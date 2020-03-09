using Havit.Blazor.StateManagement.Mobx.Models;
using Havit.Blazor.StateManagement.Mobx.Extensions;
using Havit.Blazor.StateManagement.Mobx.Samples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Samples
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
                    Title = "Fetch data",
                    Link = "fetchdata",
                    Glyph = "oi oi-list-rich"
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
            }.ToObservableArray();
        }

        public IObservableArray<INavigationItem> NavigationItems { get; set; }
    }
}
