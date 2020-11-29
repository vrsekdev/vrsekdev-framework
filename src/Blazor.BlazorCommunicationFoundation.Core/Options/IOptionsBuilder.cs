using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Options
{
    public interface IOptionsBuilder<TOptions>
    {
        void AddSerializer<T>() where T : IInvocationSerializer => AddSerializer(typeof(T));

        void AddSerializer(Type type);

        TOptions Build();
    }
}
