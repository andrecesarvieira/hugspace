#!/bin/bash
# Script para iniciar ambiente de desenvolvimento do SynQcore
# Executa: ./scripts/start-dev.sh

echo "🚀 Iniciando ambiente SynQcore..."

# Verifica se Docker está rodando
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker não está rodando. Inicie o Docker primeiro."
    exit 1
fi

# Navega para o diretório docker
cd "$(dirname "$0")/../docker"

# Para containers existentes (se houver)
echo "🛑 Parando containers existentes..."
docker compose down

# Remove volumes órfãos (opcional - descomente se necessário)
# docker compose down -v

# Builda e inicia os serviços
echo "🐳 Iniciando containers..."
docker compose up -d

# Aguarda os serviços ficarem prontos
echo "⏳ Aguardando serviços iniciarem..."
sleep 10

# Verifica status dos serviços
echo "📊 Status dos serviços:"
docker compose ps

echo ""
echo "✅ Ambiente SynQcore iniciado com sucesso!"
echo ""
echo "📋 Acesso aos serviços:"
echo "   🐘 PostgreSQL: localhost:5432"
echo "      Database: synqcore_db"
echo "      User: synqcore_user"  
echo "      Password: synqcore_dev_password"
echo ""
echo "   🚀 Redis: localhost:6379"
echo ""
echo "   🌐 pgAdmin: http://localhost:8080"
echo "      Email: admin@synqcore.dev"
echo "      Password: admin123"
echo ""
echo "� Para iniciar a API na porta 5000:"
echo "   ./scripts/start-api-5000.sh"
echo "   ou simplesmente: ./start5000.sh"
echo ""
echo "�💡 Para parar: docker compose down"
echo "🔄 Para logs: docker compose logs -f"