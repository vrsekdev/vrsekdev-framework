using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx
{
    internal class PropertyAccessedArgs
    {
        public DynamicStateProperty DynamicStateProperty { get; set; }

        public string PropertyName { get; set; }
    }
}
