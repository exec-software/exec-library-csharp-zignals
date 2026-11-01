/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals.Tests;

using Xunit;
using Signals;

public sealed class thread_safe_queue_tests {
    private sealed class recording_queue : thread_safe_queue {
        public readonly List<string> seen = new List<string>();
        protected override void think(signal_record sig) {
            lock (seen) { seen.Add(sig.type); }
        }
    }

    private sealed class throwing_queue : thread_safe_queue {
        public int attempts;
        public readonly List<string> errors = new List<string>();
        protected override void think(signal_record sig) {
            Interlocked.Increment(ref attempts);
            throw new InvalidOperationException("from the watcher");
        }
        protected override void on_error(signal_record sig, Exception error) {
            lock (errors) { errors.Add($"{sig.type}:{error.GetType().Name}"); }
        }
    }

    private static void drain(thread_safe_queue queue, Func<bool> done) {
        queue.flush();
        for (int i = 0; i < 100 && !done(); i++) {
            Thread.Sleep(20);
        }
    }

    [Fact]
    public void every_enqueued_signal_reaches_think() {
        using recording_queue queue = new recording_queue();
        for (int i = 0; i < 100; i++) {
            queue.enqueue(signals_tk.new_signal(signal_priority.standard, $"s{i}"));
        }
        drain(queue, () => queue.seen.Count == 100);
        Assert.Equal(100, queue.seen.Count);
        Assert.Equal(0, queue.pending);
    }

    [Fact]
    public void delivery_is_fifo_and_does_not_reorder_by_priority() {
        // The readme says so, so it is worth holding to.
        using recording_queue queue = new recording_queue();
        queue.enqueue(signals_tk.new_signal(signal_priority.minor, "A"));
        queue.enqueue(signals_tk.new_signal(signal_priority.critical, "B"));
        queue.enqueue(signals_tk.new_signal(signal_priority.standard, "C"));
        drain(queue, () => queue.seen.Count == 3);
        Assert.Equal(new[] { "SIGA", "SIGB", "SIGC" }, queue.seen);
    }

    [Fact]
    public void a_watcher_that_throws_does_not_stop_the_queue() {
        // Regression: nothing caught what think() threw, and an exception
        // escaping a background thread terminates the process, so one bad
        // watcher took down the whole host application.
        using throwing_queue queue = new throwing_queue();
        queue.enqueue(signals_tk.new_signal(signal_priority.standard, "one"));
        queue.enqueue(signals_tk.new_signal(signal_priority.standard, "two"));
        queue.enqueue(signals_tk.new_signal(signal_priority.standard, "three"));
        drain(queue, () => queue.attempts == 3);
        Assert.Equal(3, queue.attempts);
        Assert.Equal(3, queue.errors.Count);
        Assert.All(queue.errors, e => Assert.EndsWith(":InvalidOperationException", e));
    }

    [Fact]
    public void pending_reports_zero_when_idle() {
        using recording_queue queue = new recording_queue();
        Assert.Equal(0, queue.pending);
    }
}
