#!/bin/bash

# Script para remover AutoMapper rapidamente de todos os arquivos

echo "🔧 Corrigindo imports do AutoMapper..."

# 1. Trocar imports
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -type f -exec sed -i 's/using AutoMapper;/using SynQcore.Application.Common.Extensions;/g' {} \;

echo "🗑️  Removendo declarações IMapper..."

# 2. Remover declarações de IMapper
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -type f -exec sed -i '/private readonly IMapper _mapper;/d' {} \;

echo "🔄 Corrigindo construtores..."

# 3. Corrigir construtores (remover IMapper mapper dos parâmetros)
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -type f -exec sed -i 's/, IMapper mapper//g' {} \;
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -type f -exec sed -i 's/IMapper mapper, //g' {} \;

echo "🗑️  Removendo atribuições _mapper = mapper..."

# 4. Remover atribuições _mapper = mapper;
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -type f -exec sed -i '/_mapper = mapper;/d' {} \;

echo "🔄 Corrigindo chamadas de mapeamento..."

# 5. Corrigir chamadas específicas de mapeamento
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -type f -exec sed -i 's/_mapper\.Map<EndorsementDto>(\([^)]*\))/\1.ToEndorsementDto()/g' {} \;
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -type f -exec sed -i 's/_mapper\.Map<EmployeeDto>(\([^)]*\))/\1.ToEmployeeDto()/g' {} \;
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -type f -exec sed -i 's/_mapper\.Map<List<EmployeeDto>>(\([^)]*\))/\1.ToEmployeeDtos()/g' {} \;
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -type f -exec sed -i 's/_mapper\.Map<List<EndorsementDto>>(\([^)]*\))/\1.ToEndorsementDtos()/g' {} \;

echo "✅ AutoMapper removido com sucesso!"

# 6. Compilar para verificar erros
echo "🔨 Testando compilação..."
cd /mnt/Dados/Projetos/SynQcore
dotnet build --no-restore

echo "📊 Verificando arquivos restantes com AutoMapper..."
find /mnt/Dados/Projetos/SynQcore/src -name "*.cs" -exec grep -l "AutoMapper\|IMapper\|_mapper" {} \; | wc -l