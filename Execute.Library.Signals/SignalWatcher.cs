namespace Execute.Library.Signals;

// Copyright (C) 2008 - 2025 Execute Software, LLC.  All rights reserved.

public interface SignalWatcher {
    public string UUID { get; }
    
    public void Receive(Signal siSignal);
}