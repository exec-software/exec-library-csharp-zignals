namespace Execute.Library.Signals;

// Copyright (C) 2008 - 2025 Execute Software, LLC.  All rights reserved.

public static class SignalsTk {
    public static Signal NewSignal(SignalPriority spSignalPriority, string sSignalType, string sSignalOrigin = "",
        bool bRequiresTagging = true, params KeyValuePair<string, object>[] kvaRecords) {
        if (bRequiresTagging) {
            sSignalType = Tag(sSignalType);
        }
        Signal siNewSignal = new Signal {
            Priority = spSignalPriority,
            Type = sSignalType,
            Origin = sSignalOrigin
        };
        foreach (KeyValuePair<string, object> kvPair in kvaRecords) {
            siNewSignal.Pack(kvPair);
        }
        return siNewSignal;
    }

    public static void VerifyRegistration(Signaler siSignaler, string sClassName, SignalWatcher swSignalWatcher) {
        if (siSignaler is not null) {
            if (!string.IsNullOrEmpty(sClassName)) {
                if (!siSignaler.Watched(sClassName)) {
                    siSignaler.Watch(sClassName, swSignalWatcher);
                }
            }
        }
    }

    public static bool IsTypeof(Signal siSignal, string sSignalType) {
        if (siSignal is not null) {
            if (!string.IsNullOrEmpty(sSignalType)) {
                return siSignal.Type == sSignalType;
            }
        }
        return false;
    }
    
    public static string Tag(string siSignalName, string sSeperator = "") => $"SIG{sSeperator}{siSignalName}";
}