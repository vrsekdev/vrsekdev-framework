using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class RuntimeProxy<TContract>
        where TContract : class
    {
        private readonly RemoteMethodExecutor remoteMethodExecutor;

        public RuntimeProxy(RemoteMethodExecutor remoteMethodExecutor)
        {
            this.remoteMethodExecutor = remoteMethodExecutor;
        }

        public object InvokeRemoteMethod(string methodName, object[] args)
        {
            if (!remoteMethodExecutor.TryInvokeRemoteMethod<TContract>(methodName, args, out object result))
            {
                throw new Exception();
            }

            return result;
        }
    }
}
