#!/usr/bin/env bash
set -euo pipefail

echo "Running postStartCommand..."

DEPS_RESTORED_MARKER=".devcontainer/.deps_restored"

if [ ! -f "$DEPS_RESTORED_MARKER" ]; then
  echo "Restore dependencies..."
  ./build.sh restore
  echo "Dependencies restored successfully."
  mkdir -p "$(dirname "$DEPS_RESTORED_MARKER")"
  touch "$DEPS_RESTORED_MARKER"
else
  echo "Dependencies already restored, skipping restore step."
fi
echo "Ran postStartCommand successfully."
