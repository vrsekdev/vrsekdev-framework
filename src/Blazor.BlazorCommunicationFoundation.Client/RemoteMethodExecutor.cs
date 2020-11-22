using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class RemoteMethodExecutor
    {
        private readonly IHttpClientResolver httpClientResolver;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IContractBindingSerializer contractBindingSerializer;

        private readonly Dictionary<MethodInfo, string> bindingIdentifierCache = new Dictionary<MethodInfo, string>();

        public RemoteMethodExecutor(IHttpClientResolver httpClientResolver,
            IInvocationSerializer invocationSerializer,
            IContractBindingSerializer contractBindingSerializer)
        {
            this.httpClientResolver = httpClientResolver;
            this.invocationSerializer = invocationSerializer;
            this.contractBindingSerializer = contractBindingSerializer;
        }

        public bool TryInvokeRemoteMethod<TContract>(MethodInfo contractMethod, object[] args, out object result)
            where TContract : class
        {
            if (!bindingIdentifierCache.TryGetValue(contractMethod, out string bindingIdentifier))
            {
                bindingIdentifier = contractBindingSerializer.GenerateIdentifier(typeof(TContract), contractMethod);
                bindingIdentifierCache.Add(contractMethod, bindingIdentifier);
            }

            MemoryStream requestStream = new MemoryStream();
            invocationSerializer.Serialize(requestStream, new InvocationRequest
            {
                BindingInfo = new RequestBindingInfo
                {
                    BindingIdentifier = bindingIdentifier,
                },
                Arguments = args
            });
            requestStream.Position = 0;

            StreamContent requestContent = new StreamContent(requestStream);

            Type returnType;
            if (contractMethod.ReturnType == typeof(Task))
            {
                result = GetResultNoReturnTypeAsync(typeof(TContract), contractMethod.Name, requestContent);
            }
            else
            {
                returnType = contractMethod.ReturnType.GetGenericArguments()[0];
                result = GetType().GetMethod(nameof(GetResultAsync), BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(returnType)
                            .Invoke(this, new object[] { typeof(TContract), contractMethod.Name, requestContent });
            }

            return true;
        }

        private async Task<T> GetResultAsync<T>(Type contractType, string methodName, StreamContent requestContent)
        {
            HttpClient httpClient = httpClientResolver.GetHttpClient(contractType);
            var response = await httpClient.PostAsync($"/bcf/invoke?contract={contractType.Name}&method={methodName}", requestContent);

            Stream responseStream;
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    responseStream = await response.Content.ReadAsStreamAsync();
                    return await invocationSerializer.DeserializeAsync<T>(responseStream);
                case HttpStatusCode.NoContent:
                    return default;
                case (HttpStatusCode)455: // Custom http status code
                    throw new Exception($"Contract `{contractType.Name}` is not registered.");
                case HttpStatusCode.InternalServerError:
                    string exceptionDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception("Internal server error occured." + (!String.IsNullOrEmpty(exceptionDetails) ? " Details: " + exceptionDetails : ""));
                default:
                    throw new Exception("Invalid response from server. Status code: " + response.StatusCode);
            }
        }

        private async Task GetResultNoReturnTypeAsync(Type contractType, string methodName, StreamContent requestContent)
        {
            HttpClient httpClient = httpClientResolver.GetHttpClient(contractType);
            using var response = await httpClient.PostAsync($"/bcf/invoke?contract={contractType.Name}&method={methodName}", requestContent);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NoContent:
                    return;
                case (HttpStatusCode)455: // Custom http status code
                    throw new Exception($"Contract `{contractType.Name}` is not registered.");
                case HttpStatusCode.InternalServerError:
                    string exceptionDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception("Internal server error occured." + (!String.IsNullOrEmpty(exceptionDetails) ? " Details: " + exceptionDetails : ""));
                default:
                    throw new Exception("Invalid response from server. Status code: " + response.StatusCode);
            }
        }
    }
}
