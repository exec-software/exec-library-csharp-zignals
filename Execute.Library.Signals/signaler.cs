/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

using System.Reflection;

public sealed class signaler {
    public IDictionary<string, signal_watcher> watchers { get; private set; } = new Dictionary<string, signal_watcher>();
    public signaler_configuration configuration { get; private set; }
    private signal_queue queue_ { get; set; }

    public signaler(signaler_configuration config) {
        configuration = config;

        // Register attributed classes.
        Assembly assembly = Assembly.GetEntryAssembly();
        IList<object> attributed_records =
            reflector.find_attributed<signal_watcher_attribute>(assembly)
                     .ToList();
        foreach (signal_watcher attributed_record in attributed_records) {
            signal_watcher watcher = reflector.instantiate<signal_watcher>(attributed_record.GetType()) as signal_watcher;
            watch(attributed_record.uuid, watcher);
        }

        queue_ = new signal_queue(watchers);
    }

    public void watch(string class_name, signal_watcher watcher) {
        if (!string.IsNullOrWhiteSpace(class_name)) {
            if (watcher is not null) {
                watchers[class_name] = watcher;
            }
        }
    }

    public bool watched(string class_name) {
        if (!string.IsNullOrWhiteSpace(class_name)) {
            return watchers.ContainsKey(class_name);
        }
        return false;
    }

    public void forget(string class_name) {
        if (!string.IsNullOrWhiteSpace(class_name)) {
            watchers.Remove(class_name);
        }
    }

    public void emit(signal sig) {
        if (sig is not null) {
            queue_.enqueue(sig);
        }
    }

    public void kill() {
        if (queue_ is not null) {
            queue_.flush();
            queue_.Dispose();
        }
    }
}
