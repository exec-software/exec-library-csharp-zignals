/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Tests;

using Xunit;
using Signals;

public sealed class zignals_match_tests {
    [Fact]
    public void of_type_matches_a_tagged_signal() {
        signal_record sig = signals_tk.new_signal(signal_priority.standard, "Tick");
        Assert.True(zignals_match.@is(sig).of_type("Tick"));
        Assert.False(zignals_match.@is(sig).of_type("Tock"));
    }

    [Fact]
    public void of_type_can_compare_without_tagging() {
        signal_record sig = signals_tk.new_signal(signal_priority.standard, "Raw", "", false);
        Assert.True(zignals_match.@is(sig).of_type("Raw", false));
    }

    [Fact]
    public void not_of_type_is_the_inverse() {
        signal_record sig = signals_tk.new_signal(signal_priority.standard, "Tick");
        Assert.True(zignals_match.@is(sig).not_of_type("Tock"));
        Assert.False(zignals_match.@is(sig).not_of_type("Tick"));
    }
}
