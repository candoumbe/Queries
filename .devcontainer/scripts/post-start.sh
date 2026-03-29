#!/usr/bin/env bash
set -euo pipefail

echo "Running postStartCommand..."

echo "Restore dependencies..."
./build.sh restore
echo "Dependencies restored successfully."

echo "Ran postStartCommand successfully."
