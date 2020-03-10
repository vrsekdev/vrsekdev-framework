using Havit.Blazor.StateManagement.Mobx.ObservableProperties.Default.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.StateManagement.Mobx.Samples.Models
{
    [ObservableArrayElement]
    public interface ITodoItem
    {
        int Id { get; set; }

        [Observable]
        IHeader Header { get; set; }

        [Observable]
        IText Text { get; set; }
    }
}
