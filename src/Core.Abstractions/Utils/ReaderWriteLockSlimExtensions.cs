using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Abstractions.Utils
{
    public static class ReaderWriteLockSlimExtensions
    {
        public static bool TryExecuteWithWriteLock(this ReaderWriterLockSlim readerWriterLock, Action action)
        {
            if (readerWriterLock.IsWriteLockHeld || !readerWriterLock.TryEnterWriteLock(0))
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
                readerWriterLock.ExitWriteLock();
            }
        }

        public static bool TryExecuteWithWriteLock<TValue>(this ReaderWriterLockSlim readerWriterLock, Func<TValue> func, out TValue value)
        {
            if (readerWriterLock.IsWriteLockHeld || !readerWriterLock.TryEnterWriteLock(0))
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
                readerWriterLock.ExitWriteLock();
            }
        }

        public static async Task TryExecuteWithWriteLockAsync(this ReaderWriterLockSlim readerWriterLock, Func<Task> func)
        {
            if (readerWriterLock.IsWriteLockHeld || !readerWriterLock.TryEnterWriteLock(0))
            {
                // Already being invoked. All changes are going to be rendered.
                // Possibly this call is from an invoked reaction
                return;
            }
            try
            {
                await func();
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
    }
}
