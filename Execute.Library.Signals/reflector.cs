/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

using System.Reflection;

public static class reflector {
    // Searches the assembly it is handed.
    //
    // This took a name and an unused instance of Tx, and looked in
    // Assembly.GetExecutingAssembly() -- this library. The types it is meant
    // to find live in the consumer's assembly, never in here, so it returned
    // null for every input: verified against a consumer type carrying the
    // attribute it was searching for.
    //
    // It also required Tx to be a base CLASS, via IsSubclassOf, while every
    // watcher implements the signal_watcher INTERFACE, for which IsSubclassOf
    // is always false. Two independent reasons it could not succeed.
    public static object? instantiate<Tx>(Assembly assembly, string name) where Tx : class {
        Type? target_type =
            assembly.GetTypes()
                    .Where(x => typeof(Tx).IsAssignableFrom(x)
                                && !x.IsAbstract
                                && x.GetCustomAttribute<signal_watcher_attribute>()?.name == name)
                    .FirstOrDefault();
        if (target_type is not null) {
            return Activator.CreateInstance(target_type);
        }
        return null;
    }

    public static object? instantiate<Tx>(Type target_type) where Tx : class {
        if (target_type is not null) {
            return Activator.CreateInstance(target_type);
        }
        return null;
    }

    // Skips anything that could not be constructed rather than yielding null
    // into the caller's loop.
    public static IEnumerable<object> find_attributed<Tx>(Assembly assembly) where Tx : class {
        foreach (Type candidate in assembly.GetTypes()) {
            if (candidate.IsAbstract || candidate.IsInterface) {
                continue;
            }
            if (candidate.GetCustomAttributes(typeof(Tx), false).Length > 0) {
                object? created = instantiate<Tx>(candidate);
                if (created is not null) {
                    yield return created;
                }
            }
        }
    }
}
