﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public interface IInvocationSerializer
    {
        void Serialize<T>(Stream stream, T instance);

        Task SerializeAsync(Stream stream, Type type, object instance);

        Task<T> DeserializeAsync<T>(Stream stream);
    }
}