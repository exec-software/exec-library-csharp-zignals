namespace Execute.Library.Signals;

// Copyright (C) 2008 - 2025 Execute Software, LLC.  All rights reserved.

using System;


public sealed class Signal {
    public string Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public IDictionary<string, object> Records { get; set; } = new Dictionary<string, object>();
    public SignalPriority Priority { get; set; }
    
    private string siOrigin = string.Empty;
    
    public string Origin {
        get {
            if (string.IsNullOrEmpty(siOrigin)) {
                return "Unknown";
            }
            return siOrigin;
        }
        set => siOrigin = value;
    }

    public Signal() {
        Priority = SignalPriority.Standard;
    }

    public void Link(string sKey, object oValue) {
        if (!string.IsNullOrEmpty(sKey)) {
            if (oValue is not null) {
                Records.Add(sKey, oValue);
            }
        }
    }
    
    public void Link(KeyValuePair<string, object> kvPair) {
        if (!string.IsNullOrEmpty(kvPair.Key)) {
            if (kvPair.Value is not null) {
                Records.Add(kvPair);
            }
        }
    }
}