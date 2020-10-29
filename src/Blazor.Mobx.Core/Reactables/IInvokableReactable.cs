using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Reactables
{
    public interface IInvokableReactable
    {
        bool RequiresInitialInvoke();

        ValueTask InvokeAsync();
    }
}
