/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Sandbox;

using System;
using System.Text;
using Signals;

[SignalWatcher]
public sealed class MockWatcher3 : SignalWatcher {
    private Signaler Signals { get; set; }
    
    public string UUID => "MockWatcher3";
    
    public MockWatcher3() {}
    
    public MockWatcher3(Signaler siSignaler) {
        Signals = siSignaler;
    }
    
    public void Receive(Signal siSignal) {
        if (Zignals.Is(siSignal).OfType(MockSignals.DaFuq)) {
            StringBuilder sbBuffer = new StringBuilder();
            sbBuffer.AppendLine($"[{UUID}]");
            sbBuffer.AppendLine($"\tTag (\"{siSignal.Type}\")");
            sbBuffer.AppendLine($"\tOrigin (\"{siSignal.Origin}\")");
            sbBuffer.AppendLine($"\tTimestamp (\"{siSignal.Timestamp}\")");
            sbBuffer.AppendLine($"\tRecord(s)");
            foreach (KeyValuePair<string, object> kvRecord in siSignal.Records) {
                sbBuffer.AppendLine($"\t\t\"{kvRecord.Key}\" -> \"{kvRecord.Value}\"");
            }
            Console.WriteLine($"MockedRecord3::Received >>>\n{sbBuffer} ");
        }
    }
}