#!/bin/bash
# Script para limpeza completa do projeto SynQcore
# Executa: ./scripts/clean-build.sh

echo "🧹 Limpando projeto SynQcore..."

# Limpa builds anteriores
echo "🗑️  Removendo builds anteriores..."
dotnet clean --verbosity quiet

# Remove pastas bin e obj
echo "📁 Removendo pastas bin/ e obj/..."
find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null
find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null

# Remove arquivos temporários
echo "🗂️  Removendo arquivos temporários..."
find . -name "*.tmp" -delete
find . -name "*.log" -delete
find . -name "TestResults" -type d -exec rm -rf {} + 2>/dev/null

# Limpa cache do NuGet
echo "📦 Limpando cache do NuGet..."
dotnet nuget locals all --clear --verbosity quiet

echo "✅ Projeto limpo com sucesso!"
echo ""
echo "💡 Para reconstruir: dotnet build"