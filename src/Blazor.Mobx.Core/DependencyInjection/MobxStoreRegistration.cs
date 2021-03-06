﻿using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.DependencyInjection;
using VrsekDev.Blazor.Mobx.Reactables.Reactions;
using VrsekDev.Blazor.Mobx.StoreAccessors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.DependencyInjection
{
    public class MobxStoreRegistration<TStore>
        where TStore : class
    {
        private readonly IServiceCollection services;

        private TStore defaultState;

        private bool storeMetadataRegistered;
        private bool dependencyRegistered;

        public MobxStoreRegistration(IServiceCollection services)
        {
            this.services = services;
        }

        public MobxStoreRegistration<TStore> WithDependency<TDependency>(bool register = true)
        {
            if (storeMetadataRegistered)
            {
                throw new InvalidOperationException("Class containing reactions was already registered for this store");
            }

            if (register)
            {
                services.AddTransient(typeof(TDependency));
            }
            services.AddTransient<IStoreDependencyInjector<TStore>, StoreDependencyInjector<TStore, TDependency>>();
            dependencyRegistered = true;

            return this;
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

        public IServiceCollection LifestyleSingleton()
        {
            services.AddSingleton<StoreHolder<TStore>>();
            services.AddSingleton<IStoreHolder<TStore>>(provider => GetStoreHolder(provider));
            services.AddTransient<IStoreAccessor<TStore>, StoreAccessor<TStore>>();

            RegisterStoreMetadata();

            return services;
        }

        public IServiceCollection LifestyleScoped()
        {
            services.AddScoped<StoreHolder<TStore>>();
            services.AddScoped<IStoreHolder<TStore>>(provider => GetStoreHolder(provider));
            services.AddTransient<IStoreAccessor<TStore>, StoreAccessor<TStore>>();

            RegisterStoreMetadata();

            return services;
        }

        public IServiceCollection LifestyleTransient()
        {
            services.AddTransient<StoreHolder<TStore>>();
            services.AddTransient<IStoreHolder<TStore>>(provider => GetStoreHolder(provider));
            services.AddTransient<IStoreAccessor<TStore>, StoreAccessor<TStore>>();

            RegisterStoreMetadata();

            return services;
        }

        public IServiceCollection LifestyleCascading()
        {
            services.AddTransient<StoreHolder<TStore>>();
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

            if (!dependencyRegistered)
            {
                services.AddTransient<IStoreDependencyInjector<TStore>, NoActionStoreDependencyInjector<TStore>>();
            }
        }

        private IStoreHolder<TStore> GetStoreHolder(IServiceProvider provider)
        {
            StoreHolder<TStore> storeHolder = provider.GetRequiredService<StoreHolder<TStore>>();
            if (defaultState != null)
            {
                storeHolder.RootObservableProperty.OverwriteFrom(defaultState, false);
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
