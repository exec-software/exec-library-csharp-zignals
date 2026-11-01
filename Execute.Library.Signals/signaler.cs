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
        //
        // This iterated an IList<object> as 'foreach (signal_watcher x in ...)',
        // which casts each element, so a class carrying the attribute without
        // implementing signal_watcher took the constructor down with an
        // InvalidCastException -- the attribute is the only thing asking to be
        // registered, and nothing made it imply the interface. It then built a
        // SECOND instance of each watcher through reflector.instantiate and
        // registered that, discarding the one find_attributed had already
        // constructed, so every watcher was created twice.
        //
        // GetEntryAssembly also returns null when there is no managed entry
        // point, such as under a test host or when hosted natively, which the
        // old code passed straight on.
        Assembly? assembly = Assembly.GetEntryAssembly();
        if (assembly is not null) {
            foreach (object attributed in reflector.find_attributed<signal_watcher_attribute>(assembly)) {
                if (attributed is signal_watcher watcher) {
                    watch(watcher.uuid, watcher);
                }
            }
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
