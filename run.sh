#!/usr/bin/env bash
# Starts the ABC Pharmacy API and React client together (macOS / Linux).
# Usage: ./run.sh    Press Ctrl+C to stop both.
set -euo pipefail
cd "$(dirname "$0")"

command -v dotnet >/dev/null || { echo "Please install the .NET 10 SDK first."; exit 1; }
command -v npm >/dev/null || { echo "Please install Node.js first."; exit 1; }

# On exit, stop everything started here, including the API process 'dotnet run' spawns.
kill_tree() { for child in $(pgrep -P "$1"); do kill_tree "$child"; done; kill "$1" 2>/dev/null || true; }
trap 'trap - EXIT; for p in $(pgrep -P $$); do kill_tree "$p"; done' INT TERM EXIT

dotnet run --project PharmacyApi --launch-profile http &
echo "Waiting for the API to start..."
until curl -s -o /dev/null http://localhost:5270/api/medicines; do sleep 1; done

cd pharmacy-client
[ -d node_modules ] || npm install
npm run dev -- --open &
wait $!
