using Havit.Blazor.StateManagement.Mobx.Abstractions;
using Havit.Blazor.StateManagement.Mobx.StoreAccessors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Lifestyles
{
    public class MobxStoreRegistration<TStore>
        where TStore : class
    {
        private readonly IServiceCollection services;

        private TStore defaultState;

        public MobxStoreRegistration(IServiceCollection services)
        {
            this.services = services;
        }

        public IServiceCollection AsSingleton()
        {
            services.AddSingleton<IStoreHolder<TStore>>(provider => GetStoreHolder(provider));
            services.AddTransient<IStoreAccessor<TStore>, InjectedStoreAccessor<TStore>>();

            return services;
        }

        public IServiceCollection AsTransient()
        {
            services.AddTransient<IStoreHolder<TStore>>(provider => GetStoreHolder(provider));
            services.AddTransient<IStoreAccessor<TStore>, InjectedStoreAccessor<TStore>>();

            return services;
        }

        public IServiceCollection Cascading()
        {
            services.AddTransient<IStoreHolder<TStore>>(provider => GetStoreHolder(provider));
            services.AddTransient<IStoreAccessor<TStore>, CascadeStoreAccessor<TStore>>();

            return services;
        }

        private IStoreHolder<TStore> GetStoreHolder(IServiceProvider provider)
        {
            IObservableFactoryFactory observableFactoryFactory = provider.GetRequiredService<IObservableFactoryFactory>();
            var storeHolder = new StoreHolder<TStore>(observableFactoryFactory);
            if (defaultState != null)
            {
                storeHolder.RootObservableProperty.OverwriteFrom(defaultState);
            }

            return storeHolder;
        }

        public MobxStoreRegistration<TStore> WithDefaultState<TStoreImpl>(TStoreImpl defaultState)
            where TStoreImpl : class, TStore
        {
            this.defaultState = defaultState;

            return this;
        }
    }
}
