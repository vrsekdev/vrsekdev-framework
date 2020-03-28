using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Abstractions.Attributes;
using Havit.Blazor.Mobx.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Tests.Stores
{
    public class AsyncStoreWithComputed : AsyncMobxStore
    {
        [ComputedValue]
        public virtual AsyncResult<bool> GetValueAsync()
        {
            return CompleteAsync(Task.Run(async () =>
            {
                await Task.Delay(500);
                return true;
            }));
        }

        [ComputedValue]
		public virtual bool PagingInfo => Questions.Value;

		[ComputedValue]
		public virtual AsyncResult<bool> Questions => GetQuestionsAsync();

		private AsyncResult<bool> GetQuestionsAsync()
		{
			return CompleteAsync(Task.Run(async () =>
            {
                await Task.Delay(2000);

                return true;
            }));
		}
    }
}
