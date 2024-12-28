namespace Execute.Library.Signals.Sandbox;

// Copyright (C) 2008 - 2025 Execute Software, LLC.  All rights reserved.

using System;
using System.Text;
using Signals;

[SignalWatcher]
public sealed class MockWatcher2 : SignalWatcher {
    private Signaler Signals { get; set; }
    
    public string UUID => "MockWatcher2";
    
    public MockWatcher2() {}
    
    public MockWatcher2(Signaler siSignaler) {
        Signals = siSignaler;
    }
    public void Receive(Signal siSignal) {
        if (siSignal is not null) {
            StringBuilder sbBuffer = new StringBuilder();
            sbBuffer.AppendLine($"[{UUID}]");
            sbBuffer.AppendLine($"\tTag (\"{siSignal.Type}\")");
            sbBuffer.AppendLine($"\tOrigin (\"{siSignal.Origin}\")");
            sbBuffer.AppendLine($"\tTimestamp (\"{siSignal.Timestamp}\")");
            sbBuffer.AppendLine($"\tRecord(s)");
            foreach (KeyValuePair<string, object> kvRecord in siSignal.Records) {
                sbBuffer.AppendLine($"\t\t\"{kvRecord.Key}\" -> \"{kvRecord.Value}\"");
            }
            Console.WriteLine($"MockedRecord::Received >>>\n{sbBuffer} ");
        }
    }
}