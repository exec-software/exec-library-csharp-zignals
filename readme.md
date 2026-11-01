# Execute.Library.Signals

A small .NET library for asynchronous processing and routing of generic
signals. Something raises a signal_record; anything watching that kind of signal_record is
handed it on a background worker, and the raiser carries on.

It is deliberately plain: no dependencies beyond the framework, one worker
thread per queue, and no scheduler to configure.

Requires .NET 8 or newer.

## Getting started

```csharp
using Execute.Library.Signals;

// One signal_router per application, usually.
signal_router sig = new signal_router(new signal_router_configuration { debug = false });

// Register a watcher by hand...
sig.watch("inventory", new inventory_watcher());

// ...and raise a signal_record. emit returns immediately; delivery happens on the
// queue's worker thread.
sig.emit(signals_tk.new_signal(signal_priority.high, "ItemAdded", "warehouse",
    true, new KeyValuePair<string, object>("sku", "WIDGET")));

// On shutdown: drains the queue and stops the worker.
sig.kill();
```

A watcher implements `signal_watcher`:

```csharp
[signal_watcher_attribute("inventory")]
public sealed class inventory_watcher : signal_watcher {
    public string uuid => "inventory";

    public void receive(signal_record sig) {
        if (zignals_match.@is(sig).of_type("ItemAdded")) {
            object? sku = sig.unpack("sku");
            Console.WriteLine($"added {sku}");
        }
    }
}
```

Carrying `[signal_watcher_attribute]` means the `signal_router` constructor finds
the class in the entry assembly, constructs it with its parameterless
constructor, and registers it under its `uuid`. A class with the attribute
that does not implement `signal_watcher` is ignored.

## Signals

A `signal_record` is a type string, a timestamp, an origin, a priority, and a bag of
records.

| Member | Meaning |
|---|---|
| `type` | what happened; tagged `SIG<name>` by `signals_tk.tag` unless you opt out |
| `timestamp` | set when the signal_record is constructed |
| `origin` | who raised it; reads `"Unknown"` when unset |
| `priority` | `minor`, `standard`, `high`, `critical` |
| `records` | string-keyed payload |
| `pack(key, value)` | adds a record; packing the same key again replaces it |
| `unpack(key)` | the record, or `null` if the signal_record does not carry it |

`priority` is carried on the signal_record and is available to a watcher. The queue
itself is FIFO and does not reorder by priority.

## Matching

`zignals_match` is a small chaining helper for the common test in `receive`:

```csharp
if (zignals_match.@is(sig).of_type("ItemAdded")) { ... }
if (zignals_match.@is(sig).not_of_type("ItemRemoved")) { ... }
```

`@is` is spelled with the `@` because `is` is a C# keyword and the house style
is lower case. `signals_tk.is_typeof(sig, type)` does the same thing without
the chain.

## Delivery

`signal_router` owns one `signal_queue`, which extends `thread_safe_queue`:

- One background thread services the queue. It is started once and kept
  alive, never spun up per signal_record.
- `emit` takes a lock only long enough to enqueue, then signals the worker.
- The worker copies the pending queue under the lock and drains its copy
  outside it, so a slow watcher does not block producers.
- Every registered watcher receives every signal_record. Filtering is the watcher's
  job, which is what `zignals_match` is for.
- `pending` reports how many signals are enqueued and not yet delivered.
- `flush()` waits for the worker to reach its idle state; `kill()` flushes and
  shuts down.

A watcher runs on the worker thread, so anything it touches needs to be safe
for that. An exception thrown out of `receive` is caught, reported through
`thread_safe_queue.on_error`, and the worker carries on with the next signal_record.
Override `on_error` to route it somewhere better than stderr.

## Building

```sh
dotnet build Execute.Library.Signals.sln
dotnet run --project Execute.Library.Signals.Sandbox
```

The sandbox registers three watchers and raises three signals. Two of the
three match and print; the third watcher deliberately tests for a type the
program never raises, which is how you can tell filtering is doing something.

## Versioning

`VERSION` holds the Execute rolling version, `vN`.
`scripts/bump-version.sh` increments it and carries the matching `N.0.0` into
the project file.

## Known gaps

- There are no tests. The library was written before the project had any, and
  the sandbox is a demonstration rather than an assertion of anything.
- `receive` is called for every signal_record on every watcher; there is no
  subscription by type.
- `on_error` reports to stderr and continues. There is no retry, and no
  dead-letter queue: a signal_record whose watcher threw is gone.

## License

MIT. See [LICENSE](LICENSE).

Copyright (C) 2008 - 2026 Execute Software.  All rights reserved.
