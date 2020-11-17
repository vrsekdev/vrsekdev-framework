using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public class NamedHttpClientNameContainer
    {
        public string HttpClientName { get; }

        public NamedHttpClientNameContainer(string httpClientName)
        {
            HttpClientName = httpClientName;
        }
    }
}
