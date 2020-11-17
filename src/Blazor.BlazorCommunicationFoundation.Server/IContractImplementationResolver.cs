using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    public interface IContractImplementationResolver
    {
        object Resolve(string contractIdentifier);
    }
}
