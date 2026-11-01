/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

public sealed class signal {
    public string type { get; set; } = string.Empty;
    public DateTime timestamp { get; set; } = DateTime.Now;
    public IDictionary<string, object> records { get; set; } = new Dictionary<string, object>();
    public signal_priority priority { get; set; }

    private string origin_ = string.Empty;

    public string origin {
        get {
            if (string.IsNullOrEmpty(origin_)) {
                return "Unknown";
            }
            return origin_;
        }
        set => origin_ = value;
    }

    public signal() {
        priority = signal_priority.standard;
    }

    public void pack(string key, object value) {
        if (!string.IsNullOrEmpty(key)) {
            if (value is not null) {
                records.Add(key, value);
            }
        }
    }

    public void pack(KeyValuePair<string, object> pair) {
        if (!string.IsNullOrEmpty(pair.Key)) {
            if (pair.Value is not null) {
                records.Add(pair);
            }
        }
    }

    public object? unpack(string record_key) {
        if (!string.IsNullOrEmpty(record_key)) {
            return records[record_key];
        }
        return null;
    }
}
