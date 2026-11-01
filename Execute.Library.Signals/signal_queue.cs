/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

public sealed class signal_queue : thread_safe_queue {
    public IDictionary<string, signal_watcher> watchers { get; private set; }

    public signal_queue(IDictionary<string, signal_watcher> queue_watchers) {
        watchers = queue_watchers;
    }

    protected override void think(signal_record sig) {
        if (watchers is not null) {
            foreach (KeyValuePair<string, signal_watcher> pair in watchers) {
                pair.Value.receive(sig);
            }
        }
    }
}
