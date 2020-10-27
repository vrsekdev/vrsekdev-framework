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

        public object this[int hash]
        {
            get
            {
                return hashDictionary[hash].GetInterceptorTarget();
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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is MethodInterceptions interceptions)) return false;

            return GetHashCode() == interceptions.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hash = 9109;

            for (int i = 0; i < Interceptions.Length; i++)
            {
                hash ^= Interceptions[i].GetHashCode();
            }

            return hash;
        }
    }
}
