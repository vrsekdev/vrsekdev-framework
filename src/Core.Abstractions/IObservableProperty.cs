using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public interface IObservableProperty : IDisposable
    {
        Type ObservedType { get; }

        bool TrySetDefaultValue(string name, object value);
        bool TryGetMember(string name, out object result);
        bool TrySetMember(string name, object value);
        void OverwriteFrom(object source, bool notify);
        void ResetValues();

        IObservableFactory CreateFactory();
        Dictionary<string, IObservableProperty> GetObservedProperties();
        Dictionary<string, IObservableCollection> GetObservedCollections();
    }
}
