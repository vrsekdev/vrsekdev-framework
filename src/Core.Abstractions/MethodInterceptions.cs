using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Blazor.Mobx.Abstractions
{
    public class MethodInterceptions : IEnumerable<KeyValuePair<int, MethodInterception>>
    {
        private readonly Dictionary<int, MethodInterception> hashDictionary = new Dictionary<int, MethodInterception>();

        public Delegate this[int hash]
        {
            get
            {
                return hashDictionary[hash].Interceptor;
            }
        }

        private MethodInterception[] interceptions;
        public MethodInterception[] Interceptions
        {
            get
            {
                return interceptions;
            }
            set
            {
                hashDictionary.Clear();
                foreach(var interception in value)
                {
                    hashDictionary.Add(interception.GetHashCode(), interception);
                }

                interceptions = value;
            }
        }

        IEnumerator<KeyValuePair<int, MethodInterception>> IEnumerable<KeyValuePair<int, MethodInterception>>.GetEnumerator()
        {
            return hashDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hashDictionary.GetEnumerator();
        }
    }
}
