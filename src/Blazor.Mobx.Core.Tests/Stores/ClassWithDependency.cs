using Havit.Blazor.Mobx.Stores;
using Havit.Blazor.Mobx.Tests.Fakes;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Tests.Stores
{
    public class ClassWithDependency
    {
        [Inject]
        public FakeDependency Dependency { get; set; }

    }
}
