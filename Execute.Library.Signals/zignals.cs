/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

public sealed class zignals_impl {
    // Optional LINQ chain interface for signal processing.
    private bool is_ok_ { get; set; }
    private signal? reference_ { get; set; }

    public zignals_impl @is(signal sig) {
        if (sig is not null) {
            is_ok_ = true;
            reference_ = sig;
        }
        return this;
    }

    public bool of_type(string signal_type, bool requires_tagging = true) {
        if (requires_tagging) {
            signal_type = signals_tk.tag(signal_type);
        }
        if (!string.IsNullOrEmpty(signal_type)) {
            if (is_ok_ && reference_ is not null) {
                return signals_tk.is_typeof(reference_, signal_type);
            }
        }
        return false;
    }

    public bool not_of_type(string signal_type) => !of_type(signal_type);
}

public static class zignals {
    public static zignals_impl @is(signal sig) => new zignals_impl().@is(sig);
}
