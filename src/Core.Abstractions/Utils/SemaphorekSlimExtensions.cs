using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Abstractions.Utils
{
    public static class SemaphorekSlimExtensions
    {
        public static bool TryExecuteWithWriteLock(this SemaphoreSlim semaphore, Action action)
        {
            if (!semaphore.Wait(0))
            {
                // Already being invoked. All changes are going to be rendered.
                // Possibly this call is from an invoked reaction
                return false;
            }
            try
            {
                action();
                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static bool TryExecuteWithWriteLock<TValue>(this SemaphoreSlim semaphore, Func<TValue> func, out TValue value)
        {
            if (!semaphore.Wait(0))
            {
                // Already being invoked. All changes are going to be rendered.
                // Possibly this call is from an invoked reaction
                value = default;
                return false;
            }
            try
            {
                value = func();
                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static async Task<bool> TryExecuteWithWriteLockAsync(this SemaphoreSlim semaphore, Func<Task> func)
        {
            if (!semaphore.Wait(0))
            {
                // Already being invoked. All changes are going to be rendered.
                // Possibly this call is from an invoked reaction
                return false;
            }
            try
            {
                await func();
                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
