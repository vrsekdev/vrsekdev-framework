﻿using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Binding
{
    public class AssemblyQualifiedNameContractTypeSerializer : IContractTypeBindingSerializer
    {
        public string GenerateIdentifier(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}
