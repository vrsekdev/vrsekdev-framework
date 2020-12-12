using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Options
{
    public interface IOptionsBuilder<TOptions>
    {
        void UseTypeBindingSerializer<T>() where T : IContractTypeBindingSerializer;

        void UseMethodBindingSerializer<T>() where T : IContractMethodBindingSerializer;

        TOptions Build();
    }
}
