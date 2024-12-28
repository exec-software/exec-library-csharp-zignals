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

        Signal siNewSignal =
            SignalsTk.NewSignal(SignalPriority.Standard,
                "Event.Something.Happened",
                "Program::Main",
                new KeyValuePair<string, object>("Record1", 1 ),
                new KeyValuePair<string, object>("Record2", "Two" ),
                new KeyValuePair<string, object>("Record3", false ),
                new KeyValuePair<string, object>("Record4",  moMockWatcher1));
        siSignaler.Emit(siNewSignal);

        Console.WriteLine("Press any key to continue...");
        while (!Console.KeyAvailable) {
            continue;
        }
    }
}