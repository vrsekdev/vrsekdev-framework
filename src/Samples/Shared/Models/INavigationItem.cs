using Havit.Blazor.StateManagement.Mobx.Observables.Default.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Samples.Models
{
    [ObservableArrayElement]
    public interface INavigationItem
    {
        string Title { get; set; }

        string Link { get; set; }

        string Glyph { get; set; }
    }
}
