using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Tests.Stores
{
	public class PagingInfo
	{
		public virtual int TotalItems { get; set; }
		public virtual int PageIndex { get; set; }
		public virtual int PageSize { get; set; }

		public int PageNumber => this.PageIndex + 1;
		public int TotalPages => (int)Math.Ceiling(this.TotalItems / (float)this.PageSize);
		public int PreviousPageNumber => this.PageNumber - 1;
		public int NextPageNumber => (this.PageNumber == this.TotalPages) ? 0 : this.PageNumber + 1;
	}
}
