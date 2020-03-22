using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.StoreAccessors
{
    internal interface IConsumerWrapper
    {
        Task ForceUpdate();

        bool IsAlive();

        bool IsRendered();
    }
}
