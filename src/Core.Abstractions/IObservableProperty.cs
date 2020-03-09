using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.StateManagement.Mobx.Abstractions
{
    public interface IObservableProperty
    {
        Type ObservedType { get; }

        bool TryGetMember(string name, out object result);

        bool TrySetMember(string name, object value);

        void OverwriteFrom(IObservableProperty source);

        void OverwriteFrom(object source);

        void ResetValues();

        IObservableFactory CreateFactory();

        Dictionary<string, IObservableProperty> GetObservedProperties();

        Dictionary<string, IObservableCollection> GetObservedCollections();
    }
}
