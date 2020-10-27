using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.StoreAccessors
{
    internal interface IConsumerWrapper
    {
        Task ForceUpdate();

        bool IsAlive();
    }
}
