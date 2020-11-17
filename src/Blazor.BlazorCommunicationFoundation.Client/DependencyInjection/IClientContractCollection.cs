using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public interface IClientContractCollection
    {
        HashSet<Type> ContractTypes { get; }

        void AddContract<TContract>() where TContract : class;
    }
}
