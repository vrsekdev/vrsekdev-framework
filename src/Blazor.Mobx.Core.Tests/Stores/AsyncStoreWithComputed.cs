using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.Abstractions.Attributes;
using VrsekDev.Blazor.Mobx.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Tests.Stores
{
    public class AsyncStoreWithComputed : AsyncMobxStore
    {
        [ComputedValue]
        public virtual AsyncResult<bool> GetValueAsync()
        {
            return CompleteAsync(Task.Run(async () =>
            {
                await Task.Delay(200);
                return true;
            }));
        }

        [Observable]
        public virtual PagingInfo PagingInfo { get; set; } = new PagingInfo { PageSize = 30, PageIndex = 1 };

        [ComputedValue]
		public virtual AsyncResult<bool> Questions => GetQuestionsAsync();

		private AsyncResult<bool> GetQuestionsAsync()
		{
			return CompleteAsync(Task.Run(async () =>
            {
                await Task.Delay(200);

                return true;
            }));
		}
    }
}
