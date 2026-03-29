#!/usr/bin/env bash
set -euo pipefail

echo "Running onCreateCommand..."

# Install xdg-utils for opening the browser from the container
sudo apt-get update
sudo apt-get install -y xdg-utils

echo "Runned onCreateCommand successfully."
