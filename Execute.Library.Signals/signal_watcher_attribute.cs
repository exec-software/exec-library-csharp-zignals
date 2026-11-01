/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

// Referenced as [signal_watcher_attribute]. C# shortens a 'FooAttribute' to
// [Foo], but that sugar needs the literal 'Attribute' suffix, and [signal_watcher]
// would in any case resolve to the interface of that name rather than here.
[AttributeUsage(AttributeTargets.Class)]
public sealed class signal_watcher_attribute : Attribute {
    public signal_watcher_attribute() {
        name = string.Empty;
    }

    public signal_watcher_attribute(string watcher_name) {
        name = watcher_name;
    }

    public string name { get; set; }
}
