# Contributing to Zignals

Contributions to the signal types, matching, delivery, tests, and
documentation are welcome. For a bug report, include the .NET version, the
signal and watcher shapes involved, and a minimal reproduction. Use small,
self-contained examples with synthetic data.

Execute Software and community contributors are recognized in
[CONTRIBUTORS.md](CONTRIBUTORS.md). You may add your preferred name to the
community section when submitting a contribution.

Open an issue describing the change before altering public API shape or
delivery ordering. Consumers depend on both, and a reordering is as breaking
as a rename even though it compiles.

## Development workflow

The library and its tests target `net8.0`.

```sh
dotnet restore Execute.Library.Signals.sln
dotnet build Execute.Library.Signals.sln --configuration Release --no-restore
dotnet test Execute.Library.Signals.sln --configuration Release --no-build
```

1. Add or update xunit tests in `Execute.Library.Signals.Tests` for any
   behavior change. Delivery and matching are the contract here, so a change
   to either needs a test that would fail without it.
2. `Execute.Library.Signals.Sandbox` is a scratch console project for trying
   things by hand. It is not a test suite; do not encode expectations there.
3. Describe what changed and which checks passed in the pull request. Mention
   API changes and known limitations.

The continuous integration workflow restores, builds and tests the solution on
Linux, Windows and macOS. Keep the code free of platform-specific assumptions
unless a test covers them.

Two documented gaps are deliberate rather than oversights, so please discuss
before changing them: `receive` is called for every signal on every watcher
with no subscription by type, and `on_error` reports to stderr and continues
with no retry and no dead-letter queue, which means a signal whose watcher
threw is gone. Both are design positions recorded in the
[readme](readme.md#known-gaps).

Do not commit credentials, private project notes, local service addresses,
developer-specific paths, or generated build artifacts. The `.gitignore`
covers the usual `bin/` and `obj/` output.

For a local secret scan, install
[Gitleaks 8.30.1](https://github.com/gitleaks/gitleaks/releases/tag/v8.30.1)
and run these commands from the repository root. Choose a new report directory
outside the checkout:

```sh
python3 scripts/test_audit_secrets.py
python3 scripts/audit_secrets.py --scanner /path/to/gitleaks --report-dir ../zignals-audit
```

The helper scans tracked and non-ignored source files, staged changes, and all
local Git refs. It refuses symlinks and submodules, keeps redacted reports
outside the checkout, and ignores inline scanner-suppression comments. The
source-audit workflow installs the same scanner from a checksum-pinned release
and does not upload reports. Never paste a scanner report into a public issue
or pull request: even redacted findings can reveal private surrounding
context.

## Releases

Maintainers increment the rolling `VERSION` with `./scripts/bump-version.sh`
as part of a default-branch release change. That script also writes the
matching assembly version into the library's `.csproj`, so do not edit one of
those fields on its own.

Contributions use the repository's [MIT License](LICENSE).
