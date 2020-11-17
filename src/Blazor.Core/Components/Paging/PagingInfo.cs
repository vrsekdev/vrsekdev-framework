using VrsekDev.Blazor.Mobx.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Components.Paging
{
    public class PagingInfo
    {
        public virtual int CurrentPage { get; set; }

        public virtual int ItemsPerPage { get; set; }

        public virtual int ItemsCount { get; set; }

        [ComputedValue]
        public virtual int PagesCount => (int)Math.Ceiling(ItemsCount / (decimal)ItemsPerPage);

        [ComputedValue]
        public virtual bool IsPreviousPageEnabled => CurrentPage > 1;

        [ComputedValue]
        public virtual bool IsNextPageEnabled => CurrentPage < PagesCount;
    }
}
