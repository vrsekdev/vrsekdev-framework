using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public class MethodInterceptions : IEnumerable<MethodInterception>
    {
        public object CapturedContext { get; set; }

        public MethodInterception[] Interceptions { get; set; }

        public IEnumerator<MethodInterception> GetEnumerator()
        {
            return ((IEnumerable<MethodInterception>)Interceptions).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Interceptions.GetEnumerator();
        }
    }
}
