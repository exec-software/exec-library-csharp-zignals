namespace Execute.Library.Signals;

// Copyright (C) 2008 - 2025 Execute Software, LLC.  All rights reserved.

using System;
using System.Collections;

public sealed class SignalQueue : ThreadSafeQueue {
    public IDictionary<string, SignalWatcher> Watchers { get; private set; }
    
    public SignalQueue(IDictionary<string, SignalWatcher> idQueueWatchers) {
        Watchers = idQueueWatchers;
    }
    
    protected override void Think(Signal siSignal) {
        if (Watchers is not null) {
            foreach (KeyValuePair<string, SignalWatcher> kvPair in Watchers) {
                kvPair.Value.Receive(siSignal);
            }
        }
    }
}