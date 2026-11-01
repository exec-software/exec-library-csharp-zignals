/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Tests;

using Xunit;
using Signals;

public sealed class signals_tk_tests {
    [Fact]
    public void new_signal_tags_the_type_by_default() {
        signal_record sig = signals_tk.new_signal(signal_priority.high, "Tick");
        Assert.Equal("SIGTick", sig.type);
        Assert.Equal(signal_priority.high, sig.priority);
    }

    [Fact]
    public void new_signal_can_skip_tagging() {
        signal_record sig = signals_tk.new_signal(signal_priority.minor, "Raw", "", false);
        Assert.Equal("Raw", sig.type);
    }

    [Fact]
    public void new_signal_accepts_records_sharing_a_key() {
        // Regression: this threw while building the signal, because the params
        // pairs were forwarded into a pack that used Add.
        signal_record sig = signals_tk.new_signal(signal_priority.standard, "Tick", "origin", true,
            new KeyValuePair<string, object>("a", 1),
            new KeyValuePair<string, object>("a", 2));
        Assert.Equal(2, sig.unpack("a"));
        Assert.Single(sig.records);
    }

    [Fact]
    public void is_typeof_compares_the_tagged_type() {
        signal_record sig = signals_tk.new_signal(signal_priority.standard, "Tick");
        Assert.True(signals_tk.is_typeof(sig, "SIGTick"));
        Assert.False(signals_tk.is_typeof(sig, "SIGTock"));
    }

    [Fact]
    public void tag_applies_the_prefix_and_an_optional_separator() {
        Assert.Equal("SIGTick", signals_tk.tag("Tick"));
        Assert.Equal("SIG.Tick", signals_tk.tag("Tick", "."));
    }
}
