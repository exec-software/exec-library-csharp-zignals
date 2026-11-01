/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Sandbox;

using Signals;

public static class program {
    // Main keeps its BCL spelling: the runtime looks for it by name.
    public static void Main(string[] arguments) {
        signaler sig = new signaler(new signaler_configuration {
            debug = true
        });
        mock_watcher_1 watcher_1 = new mock_watcher_1(sig);
        mock_watcher_2 watcher_2 = new mock_watcher_2(sig);
        mock_watcher_3 watcher_3 = new mock_watcher_3(sig);

        sig.emit(signals_tk.new_signal(signal_priority.standard, mock_signals.one));
        sig.emit(signals_tk.new_signal(signal_priority.standard, mock_signals.two));
        sig.emit(signals_tk.new_signal(signal_priority.standard, mock_signals.da_fuq));

        Console.WriteLine("Press any key to continue...");
        while (!Console.KeyAvailable) {
            continue;
        }
    }
}
