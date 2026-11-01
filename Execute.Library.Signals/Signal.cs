/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

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

    public void Pack(string sKey, object oValue) {
        if (!string.IsNullOrEmpty(sKey)) {
            if (oValue is not null) {
                Records.Add(sKey, oValue);
            }
        }
    }
    
    public void Pack(KeyValuePair<string, object> kvPair) {
        if (!string.IsNullOrEmpty(kvPair.Key)) {
            if (kvPair.Value is not null) {
                Records.Add(kvPair);
            }
        }
    }

    public object? Unpack(string sRecordKey) {
        if (!string.IsNullOrEmpty(sRecordKey)) {
            return Records[sRecordKey];
        }
        return null;
    }
}