/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

using System.Threading;

public abstract class thread_safe_queue : IDisposable {
    // Note(s)
    //
    // - Background thread is kept alive.  Do not spin up/down managed threads.
    // - Single thread to service queue.  Will never allow situation where 100 threads are
    //   servicing the same queue.
    // - Operations are copied to ensure non-blocking.
    // - AutoResetEvent ensures background thread is in the "wait" state.
    // - Disposable; make sure you clean up after yourself... We don't hold hands here...
    // - Optimized for scenario where signal_record burst is enqueued followed by silence.
    // - Background thread start is performed from the background to ensure
    //   servicing queue is a single thread.
    //
    // Dispose keeps its BCL spelling: it is IDisposable's method, not ours to rename.

    private Thread processing_thread_;
    private Queue<Action> pending_signals_ = new Queue<Action>();
    private ManualResetEvent on_new_record_ = new ManualResetEvent(false);
    private ManualResetEvent on_terminate_ = new ManualResetEvent(false);
    private volatile ManualResetEvent on_waiting_ = new ManualResetEvent(false);

    protected abstract void think(signal_record sig);

    public thread_safe_queue() {
        processing_thread_ = new Thread(new ThreadStart(think_loop));
        processing_thread_.IsBackground = true;
        processing_thread_.Start();
    }

    public void enqueue(signal_record sig) {
        lock (pending_signals_) {
            pending_signals_.Enqueue(() => {
                try {
                    think(sig);
                }
                catch (Exception error) {
                    on_error(sig, error);
                }
            });
        }
        on_new_record_.Set();
    }

    // Called when a watcher throws. Reports to stderr by default; override to
    // route it somewhere useful.
    //
    // Nothing caught this before. think() runs on the background worker, and
    // an exception escaping a thread does not merely end that thread in .NET
    // -- it terminates the process:
    //
    //   Unhandled exception. System.InvalidOperationException: from receive
    //      at thread_safe_queue.think_loop()
    //
    // So any watcher that threw once, for any reason, took the entire host
    // application down with it. A library handed arbitrary user code to run
    // cannot treat that as the caller's problem.
    protected virtual void on_error(signal_record sig, Exception error) {
        Console.Error.WriteLine($"[signals] watcher threw while handling '{sig.type}': {error}");
    }

    public void flush() => on_waiting_.WaitOne();

    public void Dispose() {
        on_terminate_.Set();
        processing_thread_.Join();
    }

    // How many signals are enqueued and not yet handed to think().
    //
    // This replaces 'async Task<bool> awaiting_processing(Action)', which no
    // caller could use: the Action it looked for is a closure built inside
    // enqueue and never handed back, so there was no way to obtain the
    // reference it compared against. It was also declared async with nothing
    // to await, so it warned on every build and ran synchronously anyway, and
    // it read the queue without taking the lock that every other access takes.
    //
    // The only caller was a commented-out emit_await on signal_router, which spun
    // on it in a tight loop.
    public int pending {
        get {
            lock (pending_signals_) {
                return pending_signals_.Count;
            }
        }
    }

    private void think_loop() {
        while (true) {
            on_waiting_.Set();
            int wait_flag = ManualResetEvent.WaitAny(
                new WaitHandle[] {
                    on_new_record_,
                    on_terminate_
            });
            if (wait_flag == 1) {
                // Termination requested.
                return;
            }
            on_new_record_.Reset();
            on_waiting_.Reset();
            Queue<Action> queue_snapshot;
            lock (pending_signals_) {
                queue_snapshot = new Queue<Action>(pending_signals_);
                pending_signals_.Clear();
            }
            foreach (Action callback in queue_snapshot) {
                callback?.Invoke();
            }
        }
    }
}
