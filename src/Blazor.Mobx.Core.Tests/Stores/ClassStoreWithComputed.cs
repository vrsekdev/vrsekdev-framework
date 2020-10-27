using VrsekDev.Blazor.Mobx.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Tests.Stores
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
        public virtual int ComputedProperty
        {
            get
            {
                Console.WriteLine(Value);

                return ++InvokeCount;
            }
        }

        [ComputedValue]
        public virtual int ComputedMethodIncerceptingValue()
        {
            return ++InvokeCount;
        }
    }
}
