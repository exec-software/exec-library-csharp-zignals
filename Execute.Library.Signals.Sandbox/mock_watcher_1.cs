/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Sandbox;

using System.Text;
using Signals;

[signal_watcher_attribute]
public sealed class mock_watcher_1 : signal_watcher {
    private signaler? signals_ { get; set; }

    public mock_watcher_1() {}

    public string uuid => "mock_watcher_1";

    public mock_watcher_1(signaler sig) {
        signals_ = sig;
        signals_tk.verify_registration(sig, uuid, this);
    }

    public void receive(signal sig) {
        if (zignals.@is(sig).of_type(mock_signals.one)) {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendLine($"[{uuid}]");
            buffer.AppendLine($"\tTag (\"{sig.type}\")");
            buffer.AppendLine($"\tOrigin (\"{sig.origin}\")");
            buffer.AppendLine($"\tTimestamp (\"{sig.timestamp}\")");
            buffer.AppendLine($"\tRecord(s)");
            foreach (KeyValuePair<string, object> record in sig.records) {
                buffer.AppendLine($"\t\t\"{record.Key}\" -> \"{record.Value}\"");
            }
            Console.WriteLine($"mock_watcher_1::received >>>\n{buffer} ");
        }
    }
}
