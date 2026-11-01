/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Tests;

using Xunit;
using Signals;

public sealed class signal_record_tests {
    [Fact]
    public void packing_the_same_key_twice_replaces_it() {
        // Regression: pack used IDictionary.Add, which threw
        // ArgumentException on the second call.
        signal_record sig = new signal_record();
        sig.pack("k", 1);
        sig.pack("k", 2);
        Assert.Equal(2, sig.unpack("k"));
        Assert.Single(sig.records);
    }

    [Fact]
    public void packing_a_pair_twice_replaces_it() {
        signal_record sig = new signal_record();
        sig.pack(new KeyValuePair<string, object>("p", 1));
        sig.pack(new KeyValuePair<string, object>("p", 9));
        Assert.Equal(9, sig.unpack("p"));
    }

    [Fact]
    public void unpacking_a_key_that_was_never_packed_returns_null() {
        // Regression: unpack indexed the dictionary and threw
        // KeyNotFoundException, despite returning object?.
        signal_record sig = new signal_record();
        Assert.Null(sig.unpack("absent"));
    }

    [Fact]
    public void unpacking_an_empty_key_returns_null() {
        Assert.Null(new signal_record().unpack(""));
    }

    [Fact]
    public void packing_an_empty_key_or_null_value_is_ignored() {
        signal_record sig = new signal_record();
        sig.pack("", 1);
        sig.pack("k", null!);
        Assert.Empty(sig.records);
    }

    [Fact]
    public void origin_reads_unknown_until_it_is_set() {
        signal_record sig = new signal_record();
        Assert.Equal("Unknown", sig.origin);
        sig.origin = "warehouse";
        Assert.Equal("warehouse", sig.origin);
    }

    [Fact]
    public void priority_defaults_to_standard() {
        Assert.Equal(signal_priority.standard, new signal_record().priority);
    }
}
