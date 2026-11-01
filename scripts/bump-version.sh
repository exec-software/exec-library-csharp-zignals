#!/usr/bin/env bash
# Execute rolling scheme: mechanical v<N> -> v<N+1>, committed with the change.
# Repository-local equivalent of /Execute/Pipeline/Tools/bump-version.sh.
# Also writes the matching assembly version into the library .csproj.
set -euo pipefail

if [[ "${1:-}" == '--help' || "${1:-}" == '-h' ]]; then
  printf 'Usage: bump-version.sh [VERSION_FILE]\nIncrement an Execute rolling VERSION (v<N>) by exactly one.\n'
  exit 0
fi
if (($# > 1)); then
  printf 'error: expected at most one VERSION file\n' >&2
  exit 1
fi
script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
version_file="${1:-$script_dir/../VERSION}"
if [[ ! -f "$version_file" || -L "$version_file" ]]; then
  printf 'error: VERSION must be a regular file: %s\n' "$version_file" >&2
  exit 1
fi
raw="$(< "$version_file")"
if [[ ! "$raw" =~ ^v([1-9][0-9]*)$ ]]; then
  printf 'error: VERSION must contain exactly v followed by a positive integer\n' >&2
  exit 1
fi
current="${BASH_REMATCH[1]}"
# Stay within JavaScript's exact integer range as well as shell arithmetic's range.
if ((${#current} > 16)) || { ((${#current} == 16)) && [[ "$current" > '9007199254740990' ]]; }; then
  printf 'error: VERSION cannot be incremented safely\n' >&2
  exit 1
fi
next=$((current + 1))
printf 'v%s\n' "$next" > "$version_file"

# Keep the assembly version in step. The rolling Execute version is a single
# integer; .NET wants three parts, so vN becomes N.0.0.
project_file="$script_dir/../Execute.Library.Signals/Execute.Library.Signals.csproj"
if [[ -f "$project_file" ]]; then
  sed -i "s|<Version>[0-9][0-9]*\.0\.0</Version>|<Version>${next}.0.0</Version>|" "$project_file"
  sed -i "s|<AssemblyVersion>[0-9][0-9]*\.0\.0\.0</AssemblyVersion>|<AssemblyVersion>${next}.0.0.0</AssemblyVersion>|" "$project_file"
  sed -i "s|<FileVersion>[0-9][0-9]*\.0\.0\.0</FileVersion>|<FileVersion>${next}.0.0.0</FileVersion>|" "$project_file"
fi

printf 'v%s -> v%s\n' "$current" "$next"
