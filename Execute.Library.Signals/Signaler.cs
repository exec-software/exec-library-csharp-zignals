namespace Execute.Library.Signals;

// Copyright (C) 2008 - 2025 Execute Software, LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;

public sealed class Signaler {
    public IDictionary<string, SignalWatcher> Watchers { get; private set; } = new Dictionary<string, SignalWatcher>();
    public SignalerConfiguration Configuration { get; private set; }
    private SignalQueue Queue { get; set; }

    public Signaler(SignalerConfiguration scConfiguration) {
        Configuration = scConfiguration;
        
        // Register attributed classes.
        Assembly asAssembly = Assembly.GetEntryAssembly();
        IList<object> ieAttributedRecords = 
            Reflector.FindAttributed<SignalWatcherAttribute>(asAssembly)
                     .ToList();
        foreach (SignalWatcher siAttributedRecord in ieAttributedRecords) {
            SignalWatcher swSignalWatcher = Reflector.Instantiate<SignalWatcher>(siAttributedRecord.GetType()) as SignalWatcher;
            Watch(siAttributedRecord.UUID, swSignalWatcher);
        }
        
        Queue = new SignalQueue(Watchers);
    }
    
    public void Watch(string sClassName, SignalWatcher siSignalWatcher) {
        if (!string.IsNullOrWhiteSpace(sClassName)) {
            if (siSignalWatcher is not null) {
                Watchers[sClassName] = siSignalWatcher;
            }
        }
    }

    public bool Watched(string sClassName) {
        if (!string.IsNullOrWhiteSpace(sClassName)) {
            return Watchers.ContainsKey(sClassName);
        }
        return false;
    }

    public void Forget(string sClassName) {
        if (!string.IsNullOrWhiteSpace(sClassName)) {
            Watchers.Remove(sClassName);
        }
    }
    
    public void Emit(Signal siSignal) {
        if (siSignal is not null) {
            Queue.Enqueue(siSignal); 
        }
    }
    
    // public async Task EmitAwait(Signal siSignal) {
    //     if (siSignal is not null) {
    //         Queue.Enqueue(siSignal);
    //         while (Queue.AwaitingProcessing(siSignal)) {
    //             continue;
    //         }
    //     }
    // }

    public void Kill() {
        if (Queue is not null) {
            Queue.Flush();
            Queue.Dispose();
        }
    }
}