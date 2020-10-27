using VrsekDev.Blazor.Mobx.Abstractions.Components;
using VrsekDev.Blazor.Mobx.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Tests.Fakes
{
    public class FakeBlazorComponent : IBlazorMobxComponent
    {
        internal int invokeCount = 0;

        public Task ForceUpdate()
        {
            invokeCount++;
            return Task.CompletedTask;
        }
    }
}
