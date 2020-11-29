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
using VrsekDev.Blazor.BlazorCommunicationFoundation.Metadata;

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

            Type returnType;
            if (contractMethod.ReturnType == typeof(Task))
            {
                result = GetResultNoReturnTypeAsync(typeof(TContract), contractMethod, requestContent);
            }
            else
            {
                returnType = contractMethod.ReturnType.GetGenericArguments()[0];
                result = GetType().GetMethod(nameof(GetResultAsync), BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(returnType)
                            .Invoke(this, new object[] { typeof(TContract), contractMethod, requestContent });
            }

            return true;
        }

        private async Task<T> GetResultAsync<T>(Type contractType, MethodInfo contactMethod, StreamContent requestContent)
        {
            HttpClient httpClient = httpClientResolver.GetHttpClient(contractType);
            var response = await httpClient.PostAsync(CreateRequestPath(contractType, contactMethod), requestContent);

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

        private async Task GetResultNoReturnTypeAsync(Type contractType, MethodInfo contractMethod, StreamContent requestContent)
        {
            HttpClient httpClient = httpClientResolver.GetHttpClient(contractType);
            using var response = await httpClient.PostAsync(CreateRequestPath(contractType, contractMethod), requestContent);

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

            return $"/{typeIdentifier}/{methodIdentifier}";
        }
    }
}
