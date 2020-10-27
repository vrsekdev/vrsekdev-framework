using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Attributes;
using VrsekDev.Blazor.Mobx.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Samples.Shared.Stores
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
