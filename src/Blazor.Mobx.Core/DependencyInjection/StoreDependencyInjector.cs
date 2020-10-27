using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.Mobx.DependencyInjection
{
    internal interface IStoreDependencyInjector<TStore>
        where TStore : class
    {
        void InjectDependency(TStore store);
    }

    internal class NoActionStoreDependencyInjector<TStore> : IStoreDependencyInjector<TStore>
        where TStore : class
    {
        public void InjectDependency(TStore store)
        {
            // NOOP
        }
    }

    internal class StoreDependencyInjector<TStore, TDependency> : IStoreDependencyInjector<TStore>
        where TStore : class
    {
        private static Lazy<Action<TStore, TDependency>> InjectDependencyAction;

        static StoreDependencyInjector()
        {
            var dependencyProperty = typeof(TStore).GetProperties()
                    .FirstOrDefault(x => x.PropertyType == typeof(TDependency) && x.GetCustomAttribute<InjectAttribute>() != null);

            if (dependencyProperty == null)
            {
                InjectDependencyAction = new Lazy<Action<TStore, TDependency>>((store, dep) => { }); // NOOP
            }

            InjectDependencyAction = new Lazy<Action<TStore, TDependency>>(() =>
            {
                return (Action<TStore, TDependency>)Delegate.CreateDelegate(typeof(Action<TStore, TDependency>), dependencyProperty.SetMethod);
            });
        }

        private readonly TDependency dependency;

        public StoreDependencyInjector(TDependency dependency)
        {
            this.dependency = dependency;
        }

        public void InjectDependency(TStore store)
        {
            InjectDependencyAction.Value(store, dependency);
        }
    }
}
