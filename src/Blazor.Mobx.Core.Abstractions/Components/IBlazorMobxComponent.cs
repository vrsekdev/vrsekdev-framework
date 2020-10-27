using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Abstractions.Components
{
    public interface IBlazorMobxComponent
    {
        Task ForceUpdate();
    }
}
