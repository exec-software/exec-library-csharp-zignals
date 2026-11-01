/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

using System.Reflection;

public static class reflector {
    public static object instantiate<Tx>(string name, Tx class_instance) where Tx : class {
        Type target_type =
            Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => x.IsSubclassOf(typeof(Tx))
                                && x.GetCustomAttribute<signal_watcher_attribute>()?.name.Equals(name) == true)
                    .FirstOrDefault();
        if (target_type is not null) {
            return Activator.CreateInstance(target_type);
        }
        return null;
    }

    public static object instantiate<Tx>(Type target_type) where Tx : class {
        if (target_type is not null) {
            return Activator.CreateInstance(target_type);
        }
        return null;
    }

    public static IEnumerable<object> find_attributed<Tx>(Assembly assembly) where Tx : class {
        foreach (Type candidate in assembly.GetTypes()) {
            if (candidate.GetCustomAttributes(typeof(Tx), false).Length > 0) {
                yield return instantiate<Tx>(candidate);
            }
        }
    }
}
