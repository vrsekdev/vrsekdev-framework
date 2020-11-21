using System;
using System.Collections.Generic;
using VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public interface IClientContractCollection : IContractCollection
    {
        void AddContract<TContract>() where TContract : class;

        void AddContract(Type contractType);
    }
}
