using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Core.Extensions
{
    public static class ComponentExtension
    {
        public static RenderFragment Render<TComponent>(this TComponent component)
            where TComponent : IComponent
        {
            return builder =>
            {
                builder.OpenComponent<TComponent>(0);
                builder.CloseComponent();
            };
        }
    }
}
