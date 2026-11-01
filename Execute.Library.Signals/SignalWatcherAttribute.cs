/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SignalWatcherAttribute : Attribute {
    public SignalWatcherAttribute() {
    }
    
    public SignalWatcherAttribute(string sName) {
        Name = sName;
    }
    
    public string Name { get; set; }
}