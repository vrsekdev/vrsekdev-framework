using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Options
{
    public abstract class BCFOptionsBuilderBase : IOptionsBuilder<BCFOptions>
    {
        private Type methodBinidngSerializerType = null;
        private Type typeBindingSerializerType = null;

        public void UseMethodBindingSerializer<T>() where T : IContractMethodBindingSerializer
        {
            methodBinidngSerializerType = typeof(T);
        }

        public void UseTypeBindingSerializer<T>() where T : IContractTypeBindingSerializer
        {
            typeBindingSerializerType = typeof(T);
        }

        protected void Build(BCFOptions options)
        {
            options.TypeBindingSerializerType = typeBindingSerializerType ?? options.TypeBindingSerializerType;
            options.MethodBindingSerializerType = methodBinidngSerializerType ?? options.MethodBindingSerializerType;
        }

        public abstract BCFOptions Build();
    }
}
