using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Samples.Models;

namespace VrsekDev.Blazor.Mobx.Samples
{
    public interface IAppStore
    {
        IObservableCollection<INavigationItem> NavigationItems { get; set; }
    }
}
