using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Linq;
using MessagePack;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;
using System.Net;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class DynamicProxy<TInterface> : DynamicObject
        where TInterface : class
    {
        private static readonly HashSet<string> methods = new HashSet<string>(typeof(TInterface).GetMethods().Select(x => x.Name));

        private readonly HttpClient httpClient;
        private readonly IMethodBinder methodBinder;
        private readonly IInvocationSerializer invocationSerializer;
        private readonly IInvocationRequestArgumentSerializer argumentSerializer;

        public DynamicProxy(HttpClient httpClient,
            IMethodBinder methodBinder, 
            IInvocationSerializer invocationSerializer,
            IInvocationRequestArgumentSerializer argumentSerializer)
        {
            this.httpClient = httpClient;
            this.methodBinder = methodBinder;
            this.invocationSerializer = invocationSerializer;
            this.argumentSerializer = argumentSerializer;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (!methods.Contains(binder.Name))
            {
                result = null;
                return false;
            }

            MethodInfo method = methodBinder.BindMethod(typeof(TInterface), binder.Name, args);

            using MemoryStream requestStream = new MemoryStream();
            invocationSerializer.Serialize(requestStream, new InvocationRequest
            {
                BindingInfo = new RequestBindingInfo
                {
                    TypeName = typeof(TInterface).AssemblyQualifiedName,
                    MethodName = method.Name,
                },
                Arguments = argumentSerializer.SerializeArguments(method.GetParameters(), args)
            });
            requestStream.Position = 0;

            StreamContent requestContent = new StreamContent(requestStream);

            Type returnType;
            if (method.ReturnType == typeof(Task))
            {
                result = GetResultNoReturnTypeAsync(requestContent);
            }
            else
            {
                returnType = method.ReturnType.GetGenericArguments()[0];
                result = GetType().GetMethod(nameof(GetResultAsync), BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(returnType)
                            .Invoke(this, new object[] { requestContent });
            }
            
            return true;
        }

        private async Task<T> GetResultAsync<T>(StreamContent requestContent)
        {
            var response = await httpClient.PostAsync("/bcf/invoke", requestContent);

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

        private async Task GetResultNoReturnTypeAsync(StreamContent requestContent)
        {
            var response = await httpClient.PostAsync("/bcf/invoke", requestContent);

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
