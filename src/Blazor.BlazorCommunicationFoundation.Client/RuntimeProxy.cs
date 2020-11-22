using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;

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

        public object InvokeRemoteMethod(RuntimeMethodHandle methodHandle, KeyValuePair<string, object>[] args)
        {
            MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(methodHandle);
            if (!remoteMethodExecutor.TryInvokeRemoteMethod<TContract>(methodInfo, args, out object result))
            {
                throw new Exception();
            }

            return result;
        }
    }
}
