/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

using System;
using System.Threading;
using System.Collections.Generic;

public abstract class ThreadSafeQueue : IDisposable {
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

    private Thread thProcessingThread;
    private Queue<Action> quPendingSignals = new Queue<Action>();
    private ManualResetEvent mreOnNewRecord = new ManualResetEvent(false);
    private ManualResetEvent mreOnTerminate = new ManualResetEvent(false);
    private volatile ManualResetEvent mreOnWaiting = new ManualResetEvent(false);

    protected abstract void Think(Signal siSignal);
    
    public ThreadSafeQueue() {
        thProcessingThread = new Thread(new ThreadStart(ThinkLoop));
        thProcessingThread.IsBackground = true;
        thProcessingThread.Start();
    }

    public void Enqueue(Signal siSignal) {
        lock (quPendingSignals) {
            quPendingSignals.Enqueue(() => Think(siSignal));
        }
        mreOnNewRecord.Set();
    }

    public void Flush() => mreOnWaiting.WaitOne();
    public void Dispose() {
        mreOnTerminate.Set();
        thProcessingThread.Join();
    }

    public async Task<bool> AwaitingProcessing(Action aSignalCallback) => quPendingSignals.Contains(aSignalCallback);

    private void ThinkLoop() {
        while (true) {
            mreOnWaiting.Set();
            int iThreadWaitFlag = ManualResetEvent.WaitAny(
                new WaitHandle[] {
                    mreOnNewRecord,
                    mreOnTerminate 
            });
            if (iThreadWaitFlag == 1) {
                // Termination requested.
                return;
            }
            mreOnNewRecord.Reset();
            mreOnWaiting.Reset();
            Queue<Action> quQueueSnapshot;
            lock (quPendingSignals) {
                quQueueSnapshot = new Queue<Action>(quPendingSignals);
                quPendingSignals.Clear();
            }
            foreach (Action acCallback in quQueueSnapshot) {
                acCallback?.Invoke();
            }
        }
    }
}