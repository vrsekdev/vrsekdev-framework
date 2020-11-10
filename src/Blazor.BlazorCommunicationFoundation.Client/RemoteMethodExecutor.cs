using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class RemoteMethodExecutor
    {
        private readonly HttpClient httpClient;
        private readonly IMethodBinder methodBinder;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IInvocationRequestArgumentSerializer argumentSerializer;

        public RemoteMethodExecutor(HttpClient httpClient,
            IMethodBinder methodBinder,
            IInvocationSerializer invocationSerializer,
            IInvocationRequestArgumentSerializer argumentSerializer)
        {
            this.httpClient = httpClient;
            this.methodBinder = methodBinder;
            this.invocationSerializer = invocationSerializer;
            this.argumentSerializer = argumentSerializer;
        }

        public bool TryInvokeRemoteMethod<TContract>(string methodName, object[] args, out object result)
            where TContract : class
        {
            MethodInfo method = methodBinder.BindMethod(typeof(TContract), methodName, args);

            MemoryStream requestStream = new MemoryStream();
            invocationSerializer.Serialize(requestStream, new InvocationRequest
            {
                BindingInfo = new RequestBindingInfo
                {
                    TypeName = typeof(TContract).AssemblyQualifiedName,
                    MethodName = method.Name,
                },
                Arguments = argumentSerializer.SerializeArguments(method.GetParameters(), args)
            });
            requestStream.Position = 0;

            StreamContent requestContent = new StreamContent(requestStream);

            Type returnType;
            if (method.ReturnType == typeof(Task))
            {
                result = GetResultNoReturnTypeAsync(typeof(TContract).Name, method.Name, requestContent);
            }
            else
            {
                returnType = method.ReturnType.GetGenericArguments()[0];
                result = GetType().GetMethod(nameof(GetResultAsync), BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(returnType)
                            .Invoke(this, new object[] { typeof(TContract).Name, method.Name, requestContent });
            }

            return true;
        }

        private async Task<T> GetResultAsync<T>(string contractName, string methodName, StreamContent requestContent)
        {
            var response = await httpClient.PostAsync($"/bcf/invoke?contract={contractName}&method={methodName}", requestContent);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    return await invocationSerializer.DeserializeAsync<T>(responseStream);
                case HttpStatusCode.NoContent:
                    return default;
                default:
                    throw new Exception("Invalid response from server. Status code: " + response.StatusCode);
            }
        }

        private async Task GetResultNoReturnTypeAsync(string contractName, string methodName, StreamContent requestContent)
        {
            using var response = await httpClient.PostAsync($"/bcf/invoke?contract={contractName}&method={methodName}", requestContent);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NoContent:
                    return;
                default:
                    throw new Exception("Invalid response from server. Status code: " + response.StatusCode);
            }
        }
    }
}
