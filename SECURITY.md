# Reporting security issues

Use GitHub's private vulnerability reporting for this repository when
available. If no private reporting channel is available, open a minimal issue
requesting one without including exploit details, credentials, or private
data. Ordinary bugs can be reported publicly.

Include the .NET version, the affected revision, the signal and watcher shapes
involved, and a minimal reproduction in the private report. Do not test
against another person's application or infrastructure.

## Integration boundaries

Zignals is an in-process library. It opens no sockets, reads no configuration,
and performs no authentication or authorization; an application embedding it
owns all of that. Two behaviors are worth knowing before you put untrusted
data through it:

- **Every watcher sees every signal.** There is no subscription by type, so a
  watcher is not a privilege boundary. Do not rely on signal type to keep data
  away from a watcher registered by another part of the application.
- **A failing watcher loses its signal.** `on_error` reports to stderr and
  continues; there is no retry and no dead-letter queue. A signal whose
  watcher threw is gone, so do not use delivery as an audit or durability
  mechanism.

Both are documented in the [readme](readme.md#known-gaps) and are design
positions, not vulnerabilities. Report instead anything that breaks the
library's own guarantees: a watcher able to observe or alter another watcher's
delivery, an exception escaping dispatch and taking down an unrelated caller,
or unbounded growth reachable from ordinary use.

Keep local configuration and secrets outside the source tree. If a credential
is committed, revoke or rotate it and review historical exposure; removing the
current file alone does not remove the credential from Git history.
