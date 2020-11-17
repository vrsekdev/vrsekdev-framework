using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public interface IOptionsBuilder<TOptions>
    {
        void UseSerializer<T>() => UseSerializer(typeof(T));
        void UseSerializer(Type type);

        TOptions Build();
    }
}
