#!/bin/bash
# Script para iniciar ambiente de desenvolvimento do EnterpriseHub
# Executa: ./scripts/start-dev.sh

echo "🚀 Iniciando ambiente EnterpriseHub..."

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
echo "✅ Ambiente EnterpriseHub iniciado com sucesso!"
echo ""
echo "📋 Acesso aos serviços:"
echo "   🐘 PostgreSQL: localhost:5432"
echo "      Database: enterprisehub_db"
echo "      User: enterprisehub_user"  
echo "      Password: enterprisehub_dev_password"
echo ""
echo "   🚀 Redis: localhost:6379"
echo ""
echo "   🌐 pgAdmin: http://localhost:8080"
echo "      Email: admin@enterprisehub.dev"
echo "      Password: admin123"
echo ""
echo "💡 Para parar: docker compose down"
echo "🔄 Para logs: docker compose logs -f"