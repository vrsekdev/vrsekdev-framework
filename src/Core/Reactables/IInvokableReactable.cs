using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables
{
    public interface IInvokableReactable
    {
        bool RequiresInitialInvoke();

        void Invoke();
    }
}
