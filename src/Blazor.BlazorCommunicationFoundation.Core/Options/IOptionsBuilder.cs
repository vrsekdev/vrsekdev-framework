using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Options
{
    public interface IOptionsBuilder<TOptions>
    {
        void UseSerializer<T>() where T : IInvocationSerializer => UseSerializer(typeof(T));

        void UseSerializer(Type type);

        TOptions Build();
    }
}
