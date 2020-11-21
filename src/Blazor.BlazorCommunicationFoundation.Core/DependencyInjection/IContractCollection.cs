using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.DependencyInjection
{
    public interface IContractCollection
    {
        IEnumerable<Type> ContractTypes { get; }
    }
}
