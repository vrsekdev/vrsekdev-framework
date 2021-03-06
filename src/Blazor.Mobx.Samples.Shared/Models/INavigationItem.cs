﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.Mobx.Samples.Models
{
    public interface INavigationItem
    {
        string Title { get; set; }

        string Link { get; set; }

        string Glyph { get; set; }
    }
}
