using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Reactables
{
    public interface IInvokableReactable
    {
        bool ShouldInvoke();

        void Invoke();
    }
}
