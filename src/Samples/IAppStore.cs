using Havit.Blazor.StateManagement.Mobx.Samples.Models;
using Havit.Blazor.StateManagement.Mobx.Models;

namespace Havit.Blazor.StateManagement.Mobx.Samples
{
    public interface IAppStore
    {
        IObservableArray<INavigationItem> NavigationItems { get; set; }
    }
}
