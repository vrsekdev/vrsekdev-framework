using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.Mobx.Abstractions;
using VrsekDev.Blazor.Mobx.StoreAccessors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace VrsekDev.Blazor.Mobx.Components
{
    public class CascadeStoreHolder
    {
        public const string CascadingParameterName = "__StoreHolder";
    }

    public class CascadeStoreHolder<TStore> : ComponentBase
        where TStore : class
    {
        private IStoreAccessor<TStore> StoreAccessor => CreateStoreAccessor();

        [Inject]
        private IPropertyProxyFactory PropertyProxyFactory { get; set; }

        [Inject]
        private IPropertyProxyWrapper PropertyProxyWrapper { get; set; }

        [Inject]
        private IStoreHolder<TStore> StoreHolder { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder __builder)
        {
            TypeInference.CreateCascadingValue_0(__builder, 1, 2, StoreAccessor, 3, 4, (__builder2) =>
            {
                __builder2.AddMarkupContent(5, "\r\n    ");
                __builder2.AddContent(6,ChildContent);
                __builder2.AddMarkupContent(7, "\r\n");
            });
        }

        private IStoreAccessor<TStore> CreateStoreAccessor()
        {
            return new StoreAccessor<TStore>(
                StoreHolder,
                PropertyProxyFactory,
                PropertyProxyWrapper);
        }

        private static class TypeInference
        {
            public static void CreateCascadingValue_0<TValue>(RenderTreeBuilder __builder, int seq, int __seq0, TValue __arg0, int __seq1, int __seq2, RenderFragment __arg2)
            {
                __builder.OpenComponent<CascadingValue<TValue>>(seq);
                __builder.AddAttribute(__seq0, "Value", __arg0);
                __builder.AddAttribute(__seq1, "Name", CascadeStoreHolder.CascadingParameterName);
                __builder.AddAttribute(__seq2, "ChildContent", __arg2);
                __builder.CloseComponent();
            }
        }
    }
}
