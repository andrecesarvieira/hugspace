#!/bin/bash

# 🔄 Script de Atualização de Conteúdo: EnterpriseHub → SynQcore
# Fase 2: Atualizar conteúdo dos arquivos

set -e

# Cores para output
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

echo "🔄 Fase 2: Atualizando conteúdo dos arquivos..."

# Função para substituir em arquivo
replace_in_file() {
    local file="$1"
    if [ -f "$file" ]; then
        # Substituir EnterpriseHub por SynQcore
        sed -i 's/EnterpriseHub/SynQcore/g' "$file"
        # Substituir enterprisehub por synqcore (lowercase)
        sed -i 's/enterprisehub/synqcore/g' "$file"
        log_success "Atualizado: $file"
    fi
}

# 1. Atualizar arquivo .sln
log_info "Atualizando SynQcore.sln..."
replace_in_file "SynQcore.sln"

# 2. Atualizar todos os .csproj
log_info "Atualizando arquivos .csproj..."
find . -name "*.csproj" -exec bash -c 'replace_in_file() { local file="$1"; sed -i "s/EnterpriseHub/SynQcore/g" "$file"; sed -i "s/enterprisehub/synqcore/g" "$file"; echo "✅ Atualizado: $file"; }; replace_in_file "$0"' {} \;

# 3. Atualizar todos os arquivos .cs
log_info "Atualizando namespaces em arquivos .cs..."
find . -name "*.cs" -exec bash -c 'replace_in_file() { local file="$1"; sed -i "s/EnterpriseHub/SynQcore/g" "$file"; echo "✅ Namespace atualizado: $file"; }; replace_in_file "$0"' {} \;

# 4. Atualizar arquivos de configuração
log_info "Atualizando arquivos de configuração..."

# appsettings.json files
find . -name "appsettings*.json" -exec bash -c 'replace_in_file() { local file="$1"; sed -i "s/EnterpriseHub/SynQcore/g" "$file"; sed -i "s/enterprisehub/synqcore/g" "$file"; echo "✅ Config atualizado: $file"; }; replace_in_file "$0"' {} \;

# launchSettings.json
find . -name "launchSettings.json" -exec bash -c 'replace_in_file() { local file="$1"; sed -i "s/EnterpriseHub/SynQcore/g" "$file"; sed -i "s/enterprisehub/synqcore/g" "$file"; echo "✅ Launch settings atualizado: $file"; }; replace_in_file "$0"' {} \;

# 5. Atualizar README.md
log_info "Atualizando README.md..."
if [ -f "README.md" ]; then
    # Substituir título principal
    sed -i 's/# 🏢 EnterpriseHub/# 🏢 SynQcore/g' README.md
    # Substituir todas as ocorrências
    sed -i 's/EnterpriseHub/SynQcore/g' README.md
    sed -i 's/enterprisehub/synqcore/g' README.md
    # Atualizar descrição
    sed -i 's/Corporate Social Network/Corporate Collaboration Platform/g' README.md
    log_success "README.md atualizado!"
fi

# 6. Atualizar docker-compose.yml
log_info "Atualizando docker-compose.yml..."
if [ -f "docker/docker-compose.yml" ]; then
    sed -i 's/enterprisehub/synqcore/g' docker/docker-compose.yml
    sed -i 's/EnterpriseHub/SynQcore/g' docker/docker-compose.yml
    log_success "Docker-compose atualizado!"
fi

# 7. Atualizar scripts
log_info "Atualizando scripts..."
find scripts/ -name "*.sh" 2>/dev/null | while read script; do
    if [ -f "$script" ]; then
        sed -i 's/EnterpriseHub/SynQcore/g' "$script"
        sed -i 's/enterprisehub/synqcore/g' "$script"
        log_success "Script atualizado: $script"
    fi
done

# 8. Atualizar arquivos .http
log_info "Atualizando arquivos .http..."
find . -name "*.http" -exec bash -c 'replace_in_file() { local file="$1"; sed -i "s/EnterpriseHub/SynQcore/g" "$file"; sed -i "s/enterprisehub/synqcore/g" "$file"; echo "✅ HTTP file atualizado: $file"; }; replace_in_file "$0"' {} \;

# 9. Atualizar ROADMAP.md se existir
if [ -f "ROADMAP.md" ]; then
    sed -i 's/EnterpriseHub/SynQcore/g' ROADMAP.md
    sed -i 's/enterprisehub/synqcore/g' ROADMAP.md
    log_success "ROADMAP.md atualizado!"
fi

# 10. Atualizar Directory.Build.props se existir
if [ -f "Directory.Build.props" ]; then
    sed -i 's/EnterpriseHub/SynQcore/g' Directory.Build.props
    sed -i 's/enterprisehub/synqcore/g' Directory.Build.props
    log_success "Directory.Build.props atualizado!"
fi

log_success "✅ Fase 2 completa: Conteúdo dos arquivos atualizado!"

echo ""
log_info "🧪 Testando compilação..."
dotnet clean > /dev/null 2>&1
if dotnet build --verbosity quiet > /dev/null 2>&1; then
    log_success "🎉 Compilação bem-sucedida!"
else
    log_warning "⚠️  Possíveis erros de compilação - verificar manualmente"
fi

echo ""
log_success "🎉 Transformação SynQcore completa!"
echo ""
echo "📋 Próximos passos manuais:"
echo "   1. Renomear pasta raiz: mv /mnt/Dados/Projetos/EnterpriseHub /mnt/Dados/Projetos/SynQcore"
echo "   2. Criar novo repositório GitHub: synqcore"
echo "   3. Atualizar README.md com nova descrição"
echo "   4. Commit: git add . && git commit -m '🚀 Rebrand: EnterpriseHub → SynQcore'"
echo ""