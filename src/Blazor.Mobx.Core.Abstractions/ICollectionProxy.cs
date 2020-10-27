using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Abstractions
{
    public interface ICollectionProxy : IObservableProxy, IEnumerable
    {
        Type ElementType { get; }

        bool ElementObserved { get; }

        /// <summary>
        /// Add default elements from base class without event invocation
        /// </summary>
        /// <param name="elements">Default elements from base class</param>
        void AddDefaultElements(IEnumerable elements);

        void Recycle();
    }
}
