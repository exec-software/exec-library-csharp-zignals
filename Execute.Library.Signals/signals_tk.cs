/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

public static class signals_tk {
    public static signal_record new_signal(signal_priority priority, string signal_type, string signal_origin = "",
        bool requires_tagging = true, params KeyValuePair<string, object>[] records) {
        if (requires_tagging) {
            signal_type = tag(signal_type);
        }
        signal_record new_sig = new signal_record {
            priority = priority,
            type = signal_type,
            origin = signal_origin
        };
        foreach (KeyValuePair<string, object> pair in records) {
            new_sig.pack(pair);
        }
        return new_sig;
    }

    public static void verify_registration(signal_router sig, string class_name, signal_watcher watcher) {
        if (sig is not null) {
            if (!string.IsNullOrEmpty(class_name)) {
                if (!sig.watched(class_name)) {
                    sig.watch(class_name, watcher);
                }
            }
        }
    }

    public static bool is_typeof(signal_record sig, string signal_type) {
        if (sig is not null) {
            if (!string.IsNullOrEmpty(signal_type)) {
                return sig.type == signal_type;
            }
        }
        return false;
    }

    public static string tag(string signal_name, string separator = "") => $"SIG{separator}{signal_name}";
}
