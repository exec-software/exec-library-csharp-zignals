/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

public enum signal_priority : byte {
    minor = 1,
    standard = 2,
    high = 3,
    critical = 4
}
