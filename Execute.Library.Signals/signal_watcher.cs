/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

public interface signal_watcher {
    public string uuid { get; }

    public void receive(signal_record sig);
}
