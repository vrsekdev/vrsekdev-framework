using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection
{
    public interface IClientContractCollection
    {
        void AddContract<TContract>() where TContract : class;
    }
}
