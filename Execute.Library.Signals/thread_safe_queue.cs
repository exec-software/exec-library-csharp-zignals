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
    // - Optimized for scenario where signal burst is enqueued followed by silence.
    // - Background thread start is performed from the background to ensure
    //   servicing queue is a single thread.
    //
    // Dispose keeps its BCL spelling: it is IDisposable's method, not ours to rename.

    private Thread processing_thread_;
    private Queue<Action> pending_signals_ = new Queue<Action>();
    private ManualResetEvent on_new_record_ = new ManualResetEvent(false);
    private ManualResetEvent on_terminate_ = new ManualResetEvent(false);
    private volatile ManualResetEvent on_waiting_ = new ManualResetEvent(false);

    protected abstract void think(signal sig);

    public thread_safe_queue() {
        processing_thread_ = new Thread(new ThreadStart(think_loop));
        processing_thread_.IsBackground = true;
        processing_thread_.Start();
    }

    public void enqueue(signal sig) {
        lock (pending_signals_) {
            pending_signals_.Enqueue(() => think(sig));
        }
        on_new_record_.Set();
    }

    public void flush() => on_waiting_.WaitOne();

    public void Dispose() {
        on_terminate_.Set();
        processing_thread_.Join();
    }

    public async Task<bool> awaiting_processing(Action signal_callback) => pending_signals_.Contains(signal_callback);

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
