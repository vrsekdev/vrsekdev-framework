using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Abstractions.Components
{
    public interface IBlazorMobxComponent
    {
        bool IsRendered();

        Task ForceUpdate();
    }
}
