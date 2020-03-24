using Havit.Blazor.Mobx.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Havit.Blazor.Mobx.Tests.Stores
{
    public class ClassStoreWithAction
    {
        public static int AutorunInvokeCount;

        public ClassStoreWithAction()
        {
            AutorunInvokeCount = 0;
        }

        public virtual int Value { get; set; }

        public virtual int AnotherValue { get; set; }


        [Action]
        public virtual void ActionMethod()
        {
            Value = 50;
            Value = 60;
        }

        [Autorun]
        public virtual void AutorunMethod()
        {
            Trace.WriteLine(Value);
            Trace.WriteLine(AnotherValue);
            AutorunInvokeCount++;
        }
    }
}
