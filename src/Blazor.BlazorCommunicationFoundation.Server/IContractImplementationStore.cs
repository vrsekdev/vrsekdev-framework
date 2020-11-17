using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server
{
    public interface IContractImplementationStore
    {
        Type GetContractType(string contractIdentifier);
    }
}
