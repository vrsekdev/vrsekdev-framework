using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions
{
    public class ArgumentDictionary : Dictionary<string, object>
    {
        public ArgumentDictionary()
        {

        }

        public ArgumentDictionary(IDictionary<string, object> dictionary) : base(dictionary)
        {

        }
    }
}
