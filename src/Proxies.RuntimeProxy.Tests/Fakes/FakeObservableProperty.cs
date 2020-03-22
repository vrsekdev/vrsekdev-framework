using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Fakes
{
    internal class FakeObservableProperty : IObservableProperty
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public Type ObservedType => throw new NotImplementedException();

        public Dictionary<string, IObservableCollection> GetObservedCollections()
        {
            return new Dictionary<string, IObservableCollection>();
        }

        public Dictionary<string, IObservableProperty> GetObservedProperties()
        {
            return new Dictionary<string, IObservableProperty>();
        }

        public bool TryGetMember(string name, out object result)
        {
            return values.TryGetValue(name, out result);
        }

        public bool TrySetMember(string name, object value)
        {
            values[name] = value;

            return true;
        }

        public IObservableFactory CreateFactory()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void OverwriteFrom(object source)
        {
            throw new NotImplementedException();
        }

        public void ResetValues()
        {
            throw new NotImplementedException();
        }
    }
}
