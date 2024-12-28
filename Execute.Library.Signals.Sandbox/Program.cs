namespace Execute.Library.Signals.Sandbox;

// Copyright (C) 2008 - 2025 Execute Software, LLC.  All rights reserved.

using System;
using System.IO;
using System.Diagnostics;
using Signals;

public static class Program {
    public static async Task Main(string[] saArguments) {
        Signaler siSignaler = new Signaler(new SignalerConfiguration {
            Debug = true
        });
        MockWatcher1 moMockWatcher1 = new MockWatcher1(siSignaler);
        MockWatcher2 moMockWatcher2 = new MockWatcher2(siSignaler);
        MockWatcher3 moMockWatcher3 = new MockWatcher3(siSignaler);
        
        siSignaler.Emit(SignalsTk.NewSignal(SignalPriority.Standard, "Event.Signal.One"));
        siSignaler.Emit(SignalsTk.NewSignal(SignalPriority.Standard, "Event.Signal.Three"));
        siSignaler.Emit(SignalsTk.NewSignal(SignalPriority.Standard, "SomeBadShit"));

        Console.WriteLine("Press any key to continue...");
        while (!Console.KeyAvailable) {
            continue;
        }
    }
}