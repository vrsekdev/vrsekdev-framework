﻿using VrsekDev.Blazor.Mobx.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VrsekDev.Blazor.Mobx.Tests.Stores
{
    public class ClassStoreWithAction
    {
        public static int AutorunInvokeCount;

        public ClassStoreWithAction()
        {
            AutorunInvokeCount = 0;
        }

        [Observable]
        public virtual PagingInfo PagingInfo { get; set; }

        public virtual int Value { get; set; }

        public virtual int AnotherValue { get; set; }

        [Action]
        public virtual void ActionMethod(string param)
        {
            Value = 50;
            Value = 60;
            AnotherValue = 88;
        }

        [Autorun]
        public virtual void AutorunMethod()
        {
            _ = Value;
            AutorunInvokeCount++;
        }
    }
}
