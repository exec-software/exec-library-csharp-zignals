/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Sandbox;

using System;
using System.Text;
using Signals;

[SignalWatcher]
public sealed class MockWatcher1 : SignalWatcher {
    private Signaler Signals { get; set; }

    public MockWatcher1() {}

    public string UUID => "MockWatcher1";
    
    public MockWatcher1(Signaler siSignaler) {
        Signals = siSignaler;
        SignalsTk.VerifyRegistration(siSignaler, UUID, this);
    }
    
    public void Receive(Signal siSignal) {
        if (Zignals.Is(siSignal).OfType(MockSignals.One)) {
            StringBuilder sbBuffer = new StringBuilder();
            sbBuffer.AppendLine($"[{UUID}]");
            sbBuffer.AppendLine($"\tTag (\"{siSignal.Type}\")");
            sbBuffer.AppendLine($"\tOrigin (\"{siSignal.Origin}\")");
            sbBuffer.AppendLine($"\tTimestamp (\"{siSignal.Timestamp}\")");
            sbBuffer.AppendLine($"\tRecord(s)");
            foreach (KeyValuePair<string, object> kvRecord in siSignal.Records) {
                sbBuffer.AppendLine($"\t\t\"{kvRecord.Key}\" -> \"{kvRecord.Value}\"");
            }
            Console.WriteLine($"MockedRecord1::Received >>>\n{sbBuffer} ");
        }
    }
}