#!/usr/bin/env bash
set -euxo pipefail

BASE="/app/browser/assets/config.base.json"
OUT="/app/browser/assets/config.json"

node /opt/apply-config-env.js "$BASE" "$OUT"

exec "$@"
