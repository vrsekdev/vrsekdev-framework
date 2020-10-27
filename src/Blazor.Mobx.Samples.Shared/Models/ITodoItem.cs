using VrsekDev.Blazor.Mobx.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Samples.Models
{
    public interface ITodoItem
    {
        int Id { get; set; }

        [Observable]
        IHeader Header { get; set; }

        [Observable]
        IText Text { get; set; }
    }
}
