#!/usr/bin/env bash
#!/usr/bin/env bash
set -euo pipefail

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

command_exists() {
    command -v "$@" > /dev/null 2>&1
}

echo "Running postStartCommand..."

echo "Restore dependencies..."
./build.sh restore
echo "Dependencies restored successfully."

echo "Ran postStartCommand successfully."

# Ensure squad CLI is installed or updated
if command_exists squad; then
    log_info "Updating squad CLI to latest version..."
    npm install --ignore-scripts -g @bradygaster/squad-cli@latest > /dev/null 2>&1
    log_success "squad CLI updated to latest version."
else
    log_info "Installing squad CLI..."
    npm install --ignore-scripts -g @bradygaster/squad-cli > /dev/null 2>&1
    log_success "squad CLI installed successfully."
fi

# Check if Squad is already initialized in the workspace
if [ -d ".squad" ]; then
    log_success "Squad is already initialized in this workspace."
else
    log_info "Squad is not yet initialized."
    log_info ""
    log_info "To set up your Squad team, run:"
    log_info "  squad init"
fi

# Check GitHub authentication status
log_info ""
log_info "Checking GitHub authentication status..."
if gh auth status > /dev/null 2>&1; then
    log_success "You are authenticated with GitHub."
    GH_USERNAME=$(gh api user -q '.login' 2>/dev/null || echo "unknown")
    log_info "Logged in as: $GH_USERNAME"
else
    log_warning "You are not authenticated with GitHub."
    log_info "To authenticate, run: gh auth login"
fi