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

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class DynamicProxy<TInterface> : DynamicObject
        where TInterface : class
    {
        private static readonly HashSet<string> methods = new HashSet<string>(typeof(TInterface).GetMethods().Select(x => x.Name));

        private readonly HttpClient httpClient;
        private readonly IInvocationSerializer invocationSerializer;

        public DynamicProxy(HttpClient httpClient,
            IInvocationSerializer invocationSerializer)
        {
            this.httpClient = httpClient;
            this.invocationSerializer = invocationSerializer;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (!methods.Contains(binder.Name))
            {
                result = null;
                return false;
            }

            using MemoryStream requestStream = new MemoryStream();
            invocationSerializer.Serialize(requestStream, new InvocationWrapper
            {
                BindingInfo = new BindingInfo
                {
                    TypeName = typeof(TInterface).AssemblyQualifiedName,
                    MethodName = binder.Name,
                },
                Arguments = args
            });
            requestStream.Position = 0;

            Type returnType = typeof(TInterface).GetMethod(binder.Name).ReturnType.GetGenericArguments()[0];

            StreamContent requestContent = new StreamContent(requestStream);

            //Type returnType = typeof(Task<>).MakeGenericType(binder.ReturnType.GetGenericArguments()[0]);
            result = GetType().GetMethod(nameof(GetResultAsync), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(returnType)
                .Invoke(this, new object[] { requestContent });
            return true;
        }

        private async Task<T> GetResultAsync<T>(StreamContent requestContent)
        {
            var response = await httpClient.PostAsync("/bcf/invoke", requestContent);
            var responseStream = await response.Content.ReadAsStreamAsync();

            return await invocationSerializer.DeserializeAsync<T>(responseStream);
        }
    }
}
