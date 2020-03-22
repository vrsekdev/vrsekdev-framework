using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Emit
{
    internal class DelegateWrapperTypeContainer
    {
        public TypeBuilder TypeBuilder { get; set; }
        public ConstructorBuilder Constructor { get; set; }
        public MethodBuilder CreateDelegateMethod { get; set; }
        public FieldBuilder DelegateTypeField { get; set; }
        public FieldBuilder TargetField { get; set; }
        public FieldBuilder BaseMethodInfoField { get; set; }
        public FieldBuilder DelegateField { get; set; }
    }
}
