using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Attributes;
using Havit.Blazor.Mobx.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Samples.Shared.Stores
{
    public class AsyncCounterStore : AsyncMobxStore
    {
        [ComputedValue]
        public virtual AsyncResult<int> ComputeAsync()
        {
            return CompleteAsync(Task.Run(async () =>
            {
                await Task.Delay(2000);
                return 50;
            }));
        }
    }
}
