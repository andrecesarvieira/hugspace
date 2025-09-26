#!/bin/bash
# Script para iniciar SynQcore API na porta 5000 com Swagger
# Executa: ./scripts/start-api-5000.sh
# 
# URLs disponíveis:
# - API: http://localhost:5000
# - Swagger: http://localhost:5000/swagger

set -e # Interrompe execução em caso de erro

echo "🚀 Iniciando SynQcore API na porta 5000..."
echo ""

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para logging
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

# Verifica se a porta 5000 está livre
if lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null 2>&1; then
    log_warning "Porta 5000 está ocupada. Liberando automaticamente..."
    
    # Tenta finalizar processos relacionados ao SynQcore
    pkill -f "dotnet.*SynQcore" 2>/dev/null || true
    pkill -f "SynQcore.Api" 2>/dev/null || true
    
    # Aguarda um pouco mais para garantir a liberação
    sleep 3
    
    # Verifica novamente
    if lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null 2>&1; then
        log_warning "Tentativa adicional de liberação da porta..."
        # Força a finalização dos processos específicos
        PIDs=$(lsof -Pi :5000 -sTCP:LISTEN -t 2>/dev/null)
        if [ -n "$PIDs" ]; then
            kill -9 $PIDs 2>/dev/null || true
            sleep 2
        fi
        
        # Verificação final
        if lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null 2>&1; then
            log_error "Não foi possível liberar a porta 5000. Processos ainda ativos:"
            lsof -Pi :5000 -sTCP:LISTEN
            log_info "Tentando continuar mesmo assim..."
        else
            log_success "Porta 5000 liberada com sucesso!"
        fi
    else
        log_success "Porta 5000 liberada com sucesso!"
    fi
fi

# Detecta a raiz do projeto (onde está o .sln)
SCRIPT_PATH="${BASH_SOURCE[0]}"
# Resolve links simbólicos
while [ -L "$SCRIPT_PATH" ]; do
    SCRIPT_DIR="$(cd -P "$(dirname "$SCRIPT_PATH")" >/dev/null 2>&1 && pwd)"
    SCRIPT_PATH="$(readlink "$SCRIPT_PATH")"
    [[ $SCRIPT_PATH != /* ]] && SCRIPT_PATH="$SCRIPT_DIR/$SCRIPT_PATH"
done
SCRIPT_DIR="$(cd -P "$(dirname "$SCRIPT_PATH")" >/dev/null 2>&1 && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
PROJECT_DIR="$PROJECT_ROOT/src/SynQcore.Api"
cd "$PROJECT_DIR"

log_info "Verificando dependências do projeto..."

# Verifica se o projeto compila
if ! dotnet build --no-restore --verbosity quiet > /dev/null 2>&1; then
    log_warning "Projeto não está compilado. Executando build completo..."
    dotnet build
    if [ $? -ne 0 ]; then
        log_error "Falha no build do projeto"
        exit 1
    fi
fi

log_success "Build verificado com sucesso"

# Define variáveis de ambiente para forçar porta 5000
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS="http://localhost:5000"

log_info "Iniciando API na porta 5000..."
echo ""

# Função para cleanup ao encerrar o script
cleanup() {
    log_info "Encerrando aplicação..."
    
    # Verificar se ainda há processos SynQcore ativos antes de tentar finalizar
    if pgrep -f "dotnet.*SynQcore" >/dev/null 2>&1; then
        pkill -f "dotnet.*SynQcore" >/dev/null 2>&1 || true
        sleep 1
        # Verificação final silenciosa
        pgrep -f "dotnet.*SynQcore" >/dev/null 2>&1 && pkill -9 -f "dotnet.*SynQcore" >/dev/null 2>&1 || true
    fi
    
    exit 0
}

# Configura trap para cleanup
trap cleanup SIGINT SIGTERM

# Inicia a aplicação
log_success "🌐 SynQcore API iniciada com sucesso!"
echo ""
echo "📋 URLs disponíveis:"
echo "   🚀 API Base: http://localhost:5000"
echo "   📚 Swagger: http://localhost:5000/swagger (abrirá automaticamente)"
echo "   🏥 Health: http://localhost:5000/health"
echo ""
echo "💡 Pressione Ctrl+C para parar a aplicação"
echo ""

# Executa a aplicação
dotnet run --no-build --no-restore

# Se chegou aqui, a aplicação foi encerrada normalmente
log_info "Aplicação encerrada"