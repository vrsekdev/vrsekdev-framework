using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.Samples.Models;

namespace Havit.Blazor.StateManagement.Mobx.Samples
{
    public interface IAppStore
    {
        IObservableCollection<INavigationItem> NavigationItems { get; set; }
    }
}
