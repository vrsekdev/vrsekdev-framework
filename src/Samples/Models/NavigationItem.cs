using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Samples.Models
{
    public class NavigationItem : INavigationItem
    {
        public string Title { get; set; }
        public string Link { get; set; }

        public string Glyph { get; set; }
    }
}
