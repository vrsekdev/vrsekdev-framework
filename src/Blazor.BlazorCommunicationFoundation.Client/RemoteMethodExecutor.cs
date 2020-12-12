using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class RemoteMethodExecutor
    {
        private readonly IHttpClientResolver httpClientResolver;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IContractTypeBindingSerializer contractTypeBindingSerializer;
        private readonly IContractMethodBindingSerializer contractMethodBindingSerializer;

        public RemoteMethodExecutor(IHttpClientResolver httpClientResolver,
            IInvocationSerializer invocationSerializer,
            IContractTypeBindingSerializer contractTypeBindingSerializer,
            IContractMethodBindingSerializer contractMethodBindingSerializer)
        {
            this.httpClientResolver = httpClientResolver;
            this.invocationSerializer = invocationSerializer;
            this.contractTypeBindingSerializer = contractTypeBindingSerializer;
            this.contractMethodBindingSerializer = contractMethodBindingSerializer;
        }

        public bool TryInvokeRemoteMethod<TContract>(MethodInfo contractMethod, KeyValuePair<string, object>[] args, out object result)
            where TContract : class
        {
            MemoryStream requestStream = new MemoryStream();

            ArgumentDictionary arguments = new ArgumentDictionary(args.ToDictionary(x => x.Key, x => x.Value));
            invocationSerializer.Serialize(requestStream, arguments);
            requestStream.Position = 0;

            StreamContent requestContent = new StreamContent(requestStream);
            requestContent.Headers.ContentType = new MediaTypeHeaderValue(invocationSerializer.MediaType);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, CreateRequestPath(typeof(TContract), contractMethod))
            {
                Content = requestContent
            };
            requestMessage.Headers.Accept.Clear();
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(invocationSerializer.MediaType));

            Type returnType;
            if (contractMethod.ReturnType == typeof(Task))
            {
                result = GetResultNoReturnTypeAsync(typeof(TContract), requestMessage);
            }
            else
            {
                returnType = contractMethod.ReturnType.GetGenericArguments()[0];
                result = GetType().GetMethod(nameof(GetResultAsync), BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(returnType)
                            .Invoke(this, new object[] { typeof(TContract), requestMessage });
            }

            return true;
        }

        private async Task<T> GetResultAsync<T>(Type contractType, HttpRequestMessage requestMessage)
        {
            HttpClient httpClient = httpClientResolver.GetHttpClient(contractType);
            using var response = await httpClient.SendAsync(requestMessage);

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

        private async Task GetResultNoReturnTypeAsync(Type contractType, HttpRequestMessage requestMessage)
        {
            HttpClient httpClient = httpClientResolver.GetHttpClient(contractType);
            using var response = await httpClient.SendAsync(requestMessage);

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

        private string CreateRequestPath(Type contractType, MethodInfo contractMethod)
        {
            string typeIdentifier = contractTypeBindingSerializer.GenerateIdentifier(contractType);
            string methodIdentifier = contractMethodBindingSerializer.GenerateIdentifier(contractMethod);

            return BindingHelper.CreateRequestPath(typeIdentifier, methodIdentifier);
        }
    }
}
