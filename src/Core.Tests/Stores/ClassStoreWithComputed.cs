using Havit.Blazor.Mobx.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Tests.Stores
{
    public class ClassStoreWithComputed
    {
        public static int InvokeCount;

        public ClassStoreWithComputed()
        {
            InvokeCount = 0;
        }

        public virtual int AnotherValue { get; set; }

        public virtual int Value { get; set; }

        [ComputedValue]
        public virtual int ComputedMethodIncerceptingValue()
        {
            Console.WriteLine(Value);

            return ++InvokeCount;
        }
    }
}
