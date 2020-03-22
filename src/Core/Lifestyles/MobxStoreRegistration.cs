using Havit.Blazor.Mobx.Abstractions;
using Havit.Blazor.Mobx.Reactions;
using Havit.Blazor.Mobx.StoreAccessors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Lifestyles
{
    public class MobxStoreRegistration<TStore>
        where TStore : class
    {
        private readonly IServiceCollection services;

        private TStore defaultState;

        private bool storeMetadataRegistered;

        public MobxStoreRegistration(IServiceCollection services)
        {
            this.services = services;
        }

        public MobxStoreRegistration<TStore> WithReactions<TImpl>()
            where TImpl : ReactionRegistrator<TStore>
        {
            if (storeMetadataRegistered)
            {
                throw new InvalidOperationException("Class containing reactions was already registered for this store");
            }

            services.AddTransient(typeof(TImpl));
            services.AddTransient<IStoreMetadata<TStore>, StoreMetadata<TStore, TImpl>>();
            storeMetadataRegistered = true;

            return this;
        }

        public IServiceCollection LifestyleScoped()
        {
            services.AddScoped<IStoreHolder<TStore>>(provider => GetStoreHolder(provider));
            services.AddTransient<IStoreAccessor<TStore>, StoreAccessor<TStore>>();

            RegisterStoreMetadata();

            return services;
        }

        public IServiceCollection LifestyleTransient()
        {
            services.AddTransient<IStoreHolder<TStore>>(provider => GetStoreHolder(provider));
            services.AddTransient<IStoreAccessor<TStore>, StoreAccessor<TStore>>();

            RegisterStoreMetadata();

            return services;
        }

        public IServiceCollection LifestyleCascading()
        {
            services.AddTransient<IStoreHolder<TStore>>(provider => GetStoreHolder(provider));
            services.AddTransient<IStoreAccessor<TStore>, CascadeStoreAccessor<TStore>>();

            RegisterStoreMetadata();

            return services;
        }

        private void RegisterStoreMetadata()
        {
            if (!storeMetadataRegistered)
            {
                services.AddTransient<IStoreMetadata<TStore>, StoreMetadata<TStore>>();
            }
        }

        private IStoreHolder<TStore> GetStoreHolder(IServiceProvider provider)
        {
            IObservableFactoryFactory observableFactoryFactory = provider.GetRequiredService<IObservableFactoryFactory>();
            IStoreMetadata<TStore> storeMetadata = provider.GetRequiredService<IStoreMetadata<TStore>>();
            var storeHolder = new StoreHolder<TStore>(storeMetadata, observableFactoryFactory);
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
