/*
 * Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
 * SPDX-License-Identifier: MIT
 */

namespace Execute.Library.Signals;

using System.Reflection;
using System.Collections.Generic;

public static class Reflector {
    public static object Instantiate<Tx>(string sName, Tx txClass) where Tx : class {
        Type tyTargetType = 
            Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => x.IsSubclassOf(typeof(Tx)) && x.GetCustomAttribute<SignalWatcherAttribute>()?.Name.Equals(sName) == true)
                    .FirstOrDefault();
        if (tyTargetType is not null) {
            return Activator.CreateInstance(tyTargetType);
        }
        return null;
    }
    
    public static object Instantiate<Tx>(Type tyTargetType) where Tx : class {
        if (tyTargetType is not null) {
            return Activator.CreateInstance(tyTargetType);;
        }
        return null;
    }

    // public static IEnumerable<Type> FindAttributed<Tx>(Assembly asAssembly) where Tx : class, new() {
    //     foreach (Type tyType in asAssembly.GetTypes()) {
    //         if (tyType.GetCustomAttributes(typeof(Tx), true).Length > 0) {
    //             yield return tyType;
    //         }
    //     }
    // }
    
    public static IEnumerable<object> FindAttributed<Tx>(Assembly asAssembly) where Tx : class {
        foreach (Type tyType in asAssembly.GetTypes()) {
            if (tyType.GetCustomAttributes(typeof(Tx), false).Length > 0) {
                yield return Instantiate<Tx>(tyType);
            }
        }
    }
}