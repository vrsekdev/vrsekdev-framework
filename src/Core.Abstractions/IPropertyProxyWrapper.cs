using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public interface IPropertyProxyWrapper
    {
        T WrapPropertyObservable<T>(IPropertyProxy propertyProxy)
            where T : class;

        IPropertyProxy UnwrapPropertyObservable<T>(T propertyProxy)
            where T : class;
    }
}
