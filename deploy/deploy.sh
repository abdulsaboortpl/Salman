#!/usr/bin/env bash
# =============================================================================
# VPS deployment script — pull latest images and restart containers
# Usage: ./deploy.sh
# =============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

if [ ! -f .env ]; then
  echo "ERROR: .env file not found. Copy .env.example to .env and configure it."
  exit 1
fi

echo "==> Pulling latest images..."
docker compose pull

echo "==> Starting containers..."
docker compose up -d

echo "==> Waiting for API to apply migrations on SalmanDB..."
sleep 15
docker compose ps
docker compose logs api --tail 20

echo ""
echo "Deployment complete!"
echo "Frontend: http://$(hostname -I | awk '{print $1}'):${HTTP_PORT:-80}"
echo "Database:  ${DB_NAME:-SalmanDB} on VPS host port ${DB_PORT:-1433}"
echo "Login with: admin / Admin@123"
