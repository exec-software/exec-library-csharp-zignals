/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Tests;

using System.Reflection;
using Xunit;
using Signals;

// Carries the attribute and implements the interface: should register.
[signal_watcher_attribute("test_watcher")]
public sealed class test_watcher : signal_watcher {
    public static int constructed;
    public readonly List<string> seen = new List<string>();
    public test_watcher() { Interlocked.Increment(ref constructed); }
    public string uuid => "test_watcher";
    public void receive(signal_record sig) {
        lock (seen) { seen.Add(sig.type); }
    }
}

// Carries the attribute but is not a watcher at all.
[signal_watcher_attribute("impostor_type")]
public sealed class impostor_type { }

public sealed class signal_router_tests {
    [Fact]
    public void watch_and_forget_track_a_watcher() {
        signal_router router = new signal_router(new signal_router_configuration());
        test_watcher watcher = new test_watcher();
        router.watch("manual", watcher);
        Assert.True(router.watched("manual"));
        router.forget("manual");
        Assert.False(router.watched("manual"));
        router.kill();
    }

    [Fact]
    public void an_emitted_signal_reaches_a_registered_watcher() {
        signal_router router = new signal_router(new signal_router_configuration());
        test_watcher watcher = new test_watcher();
        router.watch("manual", watcher);
        router.emit(signals_tk.new_signal(signal_priority.standard, "Tick"));
        router.kill();
        Assert.Contains("SIGTick", watcher.seen);
    }

    [Fact]
    public void an_attributed_class_that_is_not_a_watcher_does_not_break_construction() {
        // Regression: the registration loop was 'foreach (signal_watcher x in
        // IList<object>)', so the impostor_type above threw InvalidCastException
        // out of the constructor.
        signal_router router = new signal_router(new signal_router_configuration());
        Assert.NotNull(router.watchers);
        router.kill();
    }

    [Fact]
    public void watch_ignores_a_blank_name() {
        signal_router router = new signal_router(new signal_router_configuration());
        int before = router.watchers.Count;
        router.watch("   ", new test_watcher());
        Assert.Equal(before, router.watchers.Count);
        router.kill();
    }

    [Fact]
    public void reflector_finds_an_attributed_type_in_the_assembly_it_is_given() {
        // Regression: this searched Assembly.GetExecutingAssembly(), the
        // library itself, and tested IsSubclassOf, which is false for an
        // interface. It returned null for every possible input.
        object? made = signal_reflector.instantiate<signal_watcher>(
            Assembly.GetExecutingAssembly(), "test_watcher");
        Assert.NotNull(made);
        Assert.IsType<test_watcher>(made);
    }

    [Fact]
    public void reflector_returns_null_for_an_unknown_name() {
        object? made = signal_reflector.instantiate<signal_watcher>(
            Assembly.GetExecutingAssembly(), "no_such_watcher");
        Assert.Null(made);
    }
}
