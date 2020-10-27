using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Blazor.Mobx.Samples.Models
{
    public interface IPerson
    {
        string FirstName { get; set; }

        string LastName { get; set; }
    }
}
