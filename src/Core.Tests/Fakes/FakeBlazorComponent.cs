using Havit.Blazor.Mobx.Abstractions.Components;
using Havit.Blazor.Mobx.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Tests.Fakes
{
    public class FakeBlazorComponent : IBlazorMobxComponent
    {
        public Task ForceUpdate()
        {
            return Task.CompletedTask;
        }
    }
}
