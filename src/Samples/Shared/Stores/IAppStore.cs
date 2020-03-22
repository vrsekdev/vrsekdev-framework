using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Samples.Models;

namespace Havit.Blazor.Mobx.Samples
{
    public interface IAppStore
    {
        IObservableCollection<INavigationItem> NavigationItems { get; set; }
    }
}
