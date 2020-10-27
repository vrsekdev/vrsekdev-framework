using VrsekDev.Blazor.Mobx.Stores;
using VrsekDev.Blazor.Mobx.Tests.Fakes;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Tests.Stores
{
    public class ClassWithDependency
    {
        [Inject]
        public FakeDependency Dependency { get; set; }

    }
}
