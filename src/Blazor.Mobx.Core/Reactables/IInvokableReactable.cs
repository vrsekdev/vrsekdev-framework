using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Reactables
{
    public interface IInvokableReactable
    {
        bool RequiresInitialInvoke();

        void Invoke();
    }
}
