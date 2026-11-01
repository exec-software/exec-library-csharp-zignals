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

    // Assigns rather than Add. Add throws ArgumentException when the key is
    // already present, so packing the same key twice killed the caller:
    //
    //   sig.pack("k", 1); sig.pack("k", 2);
    //   -> ArgumentException: An item with the same key has already been added.
    //
    // A signal is a bag of records being assembled, so the last value for a
    // key wins and packing is safe to repeat.
    public void pack(string key, object value) {
        if (!string.IsNullOrEmpty(key)) {
            if (value is not null) {
                records[key] = value;
            }
        }
    }

    public void pack(KeyValuePair<string, object> pair) {
        if (!string.IsNullOrEmpty(pair.Key)) {
            if (pair.Value is not null) {
                records[pair.Key] = pair.Value;
            }
        }
    }

    // Returns null for a key that is not present, which the object? return
    // type always promised. It indexed the dictionary directly, so asking for
    // a record a signal does not carry threw instead:
    //
    //   sig.unpack("absent")
    //   -> KeyNotFoundException: The given key 'absent' was not present
    //
    // A receiver cannot know which records a sender packed without being able
    // to ask, so throwing is the wrong answer to a reasonable question.
    public object? unpack(string record_key) {
        if (!string.IsNullOrEmpty(record_key)) {
            if (records.TryGetValue(record_key, out object? value)) {
                return value;
            }
        }
        return null;
    }
}
