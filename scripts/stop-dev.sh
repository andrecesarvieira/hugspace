#!/bin/bash
# Script para parar ambiente de desenvolvimento do EnterpriseHub
# Executa: ./scripts/stop-dev.sh

echo "🛑 Parando ambiente EnterpriseHub..."

# Navega para o diretório docker
cd "$(dirname "$0")/../docker"

# Para os containers
docker compose down

echo "✅ Ambiente EnterpriseHub parado com sucesso!"
echo ""
echo "💡 Para remover volumes (CUIDADO - perde dados): docker compose down -v"