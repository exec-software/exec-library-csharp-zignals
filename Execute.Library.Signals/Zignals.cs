/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

public sealed class ZignalsImpl {
    // Optional LINQ chain interface for signal processing.
    private bool IsOk { get; set; }
    private Signal Reference { get; set; }

    public ZignalsImpl Is(Signal siSignal) {
        if (siSignal is not null) {
            IsOk = true;
            Reference = siSignal;
        }
        return this;
    }

    public bool OfType(string sSignalType, bool bRequiresTagging = true) {
        if (bRequiresTagging) {
            sSignalType = SignalsTk.Tag(sSignalType);
        }
        if (!string.IsNullOrEmpty(sSignalType)) {
            if (IsOk && Reference is not null) {
                return SignalsTk.IsTypeof(Reference, sSignalType);
            }
        }
        return false;
    }
    
    public bool NotOfType(string sSignalType) => !OfType(sSignalType);
}

public static class Zignals {
    public static ZignalsImpl Is(Signal siSignal) => new ZignalsImpl().Is(siSignal);
}