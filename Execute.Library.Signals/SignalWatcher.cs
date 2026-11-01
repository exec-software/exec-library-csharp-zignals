/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

public interface SignalWatcher {
    public string UUID { get; }
    
    public void Receive(Signal siSignal);
}