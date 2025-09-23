#!/bin/bash

# 🚀 Script de Renomeação: EnterpriseHub → SynQcore
# Autor: GitHub Copilot
# Data: $(date)

set -e  # Para no primeiro erro

echo "🚀 Iniciando transformação EnterpriseHub → SynQcore..."

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para log colorido
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

# Verificar se estamos no diretório correto
if [[ ! -f "EnterpriseHub.sln" ]]; then
    log_error "Este script deve ser executado no diretório raiz do EnterpriseHub!"
    exit 1
fi

log_info "Verificando estrutura atual..."

# 1. Fazer commit das mudanças pendentes (se houver)
log_info "Fazendo commit das mudanças pendentes..."
git add .
git commit -m "💾 Backup antes da renomeação para SynQcore" || log_warning "Nenhuma mudança para commit"

# 2. Renomear arquivos .csproj e diretórios
log_info "Renomeando arquivos de projeto..."

# Renomear diretórios src/
if [ -d "src/EnterpriseHub.Domain" ]; then
    mv "src/EnterpriseHub.Domain" "src/SynQcore.Domain"
    log_success "Renomeado: src/EnterpriseHub.Domain → src/SynQcore.Domain"
fi

if [ -d "src/EnterpriseHub.Application" ]; then
    mv "src/EnterpriseHub.Application" "src/SynQcore.Application"
    log_success "Renomeado: src/EnterpriseHub.Application → src/SynQcore.Application"
fi

if [ -d "src/EnterpriseHub.Infrastructure" ]; then
    mv "src/EnterpriseHub.Infrastructure" "src/SynQcore.Infrastructure"
    log_success "Renomeado: src/EnterpriseHub.Infrastructure → src/SynQcore.Infrastructure"
fi

if [ -d "src/EnterpriseHub.Api" ]; then
    mv "src/EnterpriseHub.Api" "src/SynQcore.Api"
    log_success "Renomeado: src/EnterpriseHub.Api → src/SynQcore.Api"
fi

if [ -d "src/EnterpriseHub.Shared" ]; then
    mv "src/EnterpriseHub.Shared" "src/SynQcore.Shared"
    log_success "Renomeado: src/EnterpriseHub.Shared → src/SynQcore.Shared"
fi

# Renomear BlazorApp
if [ -d "src/EnterpriseHub.BlazorApp" ]; then
    if [ -d "src/EnterpriseHub.BlazorApp/EnterpriseHub.BlazorApp" ]; then
        mv "src/EnterpriseHub.BlazorApp/EnterpriseHub.BlazorApp" "src/EnterpriseHub.BlazorApp/SynQcore.BlazorApp"
    fi
    if [ -d "src/EnterpriseHub.BlazorApp/EnterpriseHub.BlazorApp.Client" ]; then
        mv "src/EnterpriseHub.BlazorApp/EnterpriseHub.BlazorApp.Client" "src/EnterpriseHub.BlazorApp/SynQcore.BlazorApp.Client"
    fi
    mv "src/EnterpriseHub.BlazorApp" "src/SynQcore.BlazorApp"
    log_success "Renomeado: BlazorApp estrutura"
fi

# Renomear diretórios tests/
if [ -d "tests/EnterpriseHub.UnitTests" ]; then
    mv "tests/EnterpriseHub.UnitTests" "tests/SynQcore.UnitTests"
    log_success "Renomeado: tests/EnterpriseHub.UnitTests → tests/SynQcore.UnitTests"
fi

if [ -d "tests/EnterpriseHub.IntegrationTests" ]; then
    mv "tests/EnterpriseHub.IntegrationTests" "tests/SynQcore.IntegrationTests"
    log_success "Renomeado: tests/EnterpriseHub.IntegrationTests → tests/SynQcore.IntegrationTests"
fi

# 3. Renomear arquivos .csproj
log_info "Renomeando arquivos .csproj..."

find . -name "EnterpriseHub*.csproj" | while read file; do
    newfile=$(echo "$file" | sed 's/EnterpriseHub/SynQcore/g')
    mv "$file" "$newfile"
    log_success "Renomeado: $file → $newfile"
done

# 4. Renomear solution file
if [ -f "EnterpriseHub.sln" ]; then
    mv "EnterpriseHub.sln" "SynQcore.sln"
    log_success "Renomeado: EnterpriseHub.sln → SynQcore.sln"
fi

# 5. Renomear outros arquivos específicos
if [ -f "src/SynQcore.BlazorApp/EnterpriseHub.BlazorApp.sln" ]; then
    mv "src/SynQcore.BlazorApp/EnterpriseHub.BlazorApp.sln" "src/SynQcore.BlazorApp/SynQcore.BlazorApp.sln"
    log_success "Renomeado: BlazorApp.sln"
fi

log_success "✅ Fase 1 completa: Estrutura de arquivos e pastas renomeada!"

echo ""
echo "🔄 Continuando com atualização de conteúdo dos arquivos..."
echo "   Execute: ./update-content.sh para continuar"
echo ""

log_success "🎉 Estrutura física renomeada com sucesso!"
log_info "Próximo passo: Executar update-content.sh para atualizar conteúdo dos arquivos"