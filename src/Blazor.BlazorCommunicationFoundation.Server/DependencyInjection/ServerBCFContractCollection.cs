using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection
{
    internal class ServerBCFContractCollection : IServerContractCollection
    {
        private readonly IServiceCollection services;

        public HashSet<Type> ContractsTypes { get; } = new HashSet<Type>();

        public int Count => services.Count;

        public bool IsReadOnly => services.IsReadOnly;

        public ServiceDescriptor this[int index] { get => services[index]; set => services[index] = value; }

        public ServerBCFContractCollection(IServiceCollection services)
        {
            this.services = services;
        }

        public int IndexOf(ServiceDescriptor item)
        {
            return services.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            ContractsTypes.Add(item.ServiceType);
            services.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            services.RemoveAt(index);
        }

        public void Add(ServiceDescriptor item)
        {
            ContractsTypes.Add(item.ServiceType);
            services.Add(item);
        }

        public void Clear()
        {
            ContractsTypes.Clear();
            services.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return services.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            services.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            ContractsTypes.Remove(item.ServiceType);
            return services.Remove(item);
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return services.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)services).GetEnumerator();
        }
    }
}
