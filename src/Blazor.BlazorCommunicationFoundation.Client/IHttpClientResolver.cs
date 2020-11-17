using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client
{
    public interface IHttpClientResolver
    {
        HttpClient GetHttpClient();
    }
}
