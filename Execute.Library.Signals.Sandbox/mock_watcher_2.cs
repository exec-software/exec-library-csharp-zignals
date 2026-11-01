/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Sandbox;

using System.Text;
using Signals;

[signal_watcher_attribute]
public sealed class mock_watcher_2 : signal_watcher {
    private signal_router? signals_ { get; set; }

    public mock_watcher_2() {}

    public string uuid => "mock_watcher_2";

    public mock_watcher_2(signal_router sig) {
        signals_ = sig;

    }

    public void receive(signal_record sig) {
        if (zignals_match.@is(sig).of_type("Event.Signal.Two")) {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendLine($"[{uuid}]");
            buffer.AppendLine($"\tTag (\"{sig.type}\")");
            buffer.AppendLine($"\tOrigin (\"{sig.origin}\")");
            buffer.AppendLine($"\tTimestamp (\"{sig.timestamp}\")");
            buffer.AppendLine($"\tRecord(s)");
            foreach (KeyValuePair<string, object> record in sig.records) {
                buffer.AppendLine($"\t\t\"{record.Key}\" -> \"{record.Value}\"");
            }
            Console.WriteLine($"mock_watcher_2::received >>>\n{buffer} ");
        }
    }
}
