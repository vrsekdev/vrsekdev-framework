using VrsekDev.Blazor.Mobx.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Tests.Stores
{
    public class ClassWithAutorun
    {
        public static int AutorunInvokeCount;

        public ClassWithAutorun()
        {
            AutorunInvokeCount = 0;
        }

        public virtual int Value { get; set; }

        public virtual int AnotherValue { get; set; }

        [Autorun]
        public virtual void AutorunMethod()
        {
            Trace.WriteLine(Value);
            AutorunInvokeCount++;
        }
    }
}
