using Havit.Blazor.Mobx.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Blazor.Mobx.Proxies.RuntimeProxy.Tests.Fakes
{
    public class FakeProperyAccessedObserver : IObserver<PropertyAccessedArgs>
    {
        private Dictionary<string, int> propertyAccessedInfo = new Dictionary<string, int>();

        public void Verify(string propertyName, int times)
        {
            if (!propertyAccessedInfo.ContainsKey(propertyName) && times != 0)
            {
                throw new Exception($"Property {propertyName} was not accessed. Expected: {times}");
            }

            int actual = propertyAccessedInfo[propertyName];
            if (actual != times)
            {
                throw new Exception($"Expected property to be accessed {times}, but actual value is {actual}.");
            }
        }

        public void OnNext(PropertyAccessedArgs value)
        {
            if (!propertyAccessedInfo.ContainsKey(value.PropertyName))
            {
                propertyAccessedInfo.Add(value.PropertyName, 1);
                return;
            }

            int currentValue = propertyAccessedInfo[value.PropertyName];
            propertyAccessedInfo[value.PropertyName] = currentValue + 1;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
