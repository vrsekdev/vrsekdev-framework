using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Havit.Blazor.StateManagement.Mobx.Components
{
    public class MobxStoreHolder
    {
        public const string CascadingParameterName = "__StoreHolder";
    }

    public class MobxStoreHolder<TStore> : ComponentBase
        where TStore : class
    {
        private readonly IStoreHolder<TStore> storeHolder;

        private IStoreAccessor<TStore> StoreAccessor => new DynamicStoreAccessor<TStore>(storeHolder);

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public MobxStoreHolder()
        {
            storeHolder = new StoreHolder<TStore>();
        }

        protected override void BuildRenderTree(RenderTreeBuilder __builder)
        {
            //__builder.AddMarkupContent(0, "<h3>MobxStore</h3>\r\n\r\n");
            TypeInference.CreateCascadingValue_0(__builder, 1, 2, StoreAccessor, 3, 4, (__builder2) =>
            {
                __builder2.AddMarkupContent(5, "\r\n    ");
                __builder2.AddContent(6,ChildContent);
                __builder2.AddMarkupContent(7, "\r\n");
            });
        }

        private static class TypeInference
        {
            public static void CreateCascadingValue_0<TValue>(RenderTreeBuilder __builder, int seq, int __seq0, TValue __arg0, int __seq1, int __seq2, RenderFragment __arg2)
            {
                __builder.OpenComponent<CascadingValue<TValue>>(seq);
                __builder.AddAttribute(__seq0, "Value", __arg0);
                __builder.AddAttribute(__seq1, "Name", MobxStoreHolder.CascadingParameterName);
                __builder.AddAttribute(__seq2, "ChildContent", __arg2);
                __builder.CloseComponent();
            }
        }
    }
}
