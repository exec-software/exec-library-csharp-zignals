namespace Execute.Library.Signals;

// Copyright (C) 2008 - 2025 Execute Software, LLC.  All rights reserved.

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