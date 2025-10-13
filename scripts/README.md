# 🚀 SynQcore - Script Unificado de Gerenciamento

Este diretório contém o script consolidado para facilitar o desenvolvimento e execução do SynQcore.

## ⭐ Script Principal

### 🎯 synqcore.py - Script Unificado

**ÚNICO SCRIPT NECESSÁRIO**: Substitui todos os scripts anteriores! Todas as funcionalidades em um só lugar.

#### Python (Todas as Plataformas)

```bash
# 🚀 Iniciar aplicação completa (API + Blazor) - PADRÃO
python scripts/synqcore.py

# ou especificamente:
python scripts/synqcore.py start

# 🔗 Iniciar apenas API na porta 5000
python scripts/synqcore.py api

# 🌐 Iniciar apenas Blazor na porta 5226
python scripts/synqcore.py blazor

# 🧹 Limpeza completa do projeto
python scripts/synqcore.py clean

# 🐳 Gerenciar infraestrutura Docker
python scripts/synqcore.py docker-up
python scripts/synqcore.py docker-down

# ❓ Ajuda
python scripts/synqcore.py help
```

### 🎯 Acesso Rápido por Plataforma

#### Linux/Mac (Link Simbólico)

```bash
# Acesso direto da raiz do projeto
./synqcore start          # Aplicação completa
./synqcore api            # Apenas API
./synqcore blazor         # Apenas Blazor
./synqcore clean          # Limpeza
./synqcore docker-up      # Infraestrutura
./synqcore help           # Ajuda
```

#### Windows PowerShell

```powershell
# Acesso via wrapper PowerShell
.\synqcore.ps1 start      # Aplicação completa
.\synqcore.ps1 api        # Apenas API
.\synqcore.ps1 blazor     # Apenas Blazor
.\synqcore.ps1 clean      # Limpeza
.\synqcore.ps1 docker-up  # Infraestrutura
.\synqcore.ps1 help       # Ajuda
```

#### Windows Command Prompt

```cmd
REM Acesso via wrapper CMD
synqcore.cmd start        # Aplicação completa
synqcore.cmd api          # Apenas API
synqcore.cmd blazor       # Apenas Blazor
synqcore.cmd clean        # Limpeza
synqcore.cmd docker-up    # Infraestrutura
synqcore.cmd help         # Ajuda
```

## � Funcionalidades Integradas

### ✨ O que o Script Faz

- **🔍 Verificação Automática**: Valida estrutura do projeto e dependências
- **🔌 Gerenciamento de Portas**: Detecta e libera portas ocupadas automaticamente
- **🏗️ Build Inteligente**: Compilação otimizada evitando erros CLR
- **⚡ Inicialização Rápida**: API na porta 5000, Blazor na porta 5226
- **🌐 Browser Automático**: Abre interface automaticamente
- **📊 Monitoramento**: Health checks e logs coloridos por serviço
- **🧹 Limpeza Completa**: Remove bins, obj, cache NuGet, arquivos temporários
- **🐳 Docker Integration**: Gerencia infraestrutura (PostgreSQL, Redis, pgAdmin)

### 🎨 Recursos Visuais

- **Logs Coloridos**: Output organizado por serviço (API=Magenta, Blazor=Cyan, Docker=Azul)
- **Emojis Informativos**: Feedback visual claro para cada operação
- **Progress Indicators**: Acompanhamento em tempo real do status
- **Error Handling**: Tratamento robusto de erros com mensagens claras

### 🔧 Solução de Problemas Comuns

- **CLR Errors**: Build single-thread para evitar erros internos do CLR
- **Port Conflicts**: Mata processos conflitantes automaticamente
- **MSBuild Issues**: Shutdown automático de servidores de build
- **Memory Cleanup**: Limpeza de cache e arquivos temporários

## 📍 URLs Configuradas

Após executar qualquer comando de inicialização:

| Serviço           | URL                                 | Descrição                 |
| ----------------- | ----------------------------------- | ------------------------- |
| **API**           | http://localhost:5000               | API RESTful completa      |
| **Swagger**       | http://localhost:5000/swagger       | Documentação interativa   |
| **Health Check**  | http://localhost:5000/health        | Monitoramento de saúde    |
| **Blazor App**    | http://localhost:5226               | Interface moderna         |
| **Design System** | http://localhost:5226/design-system | Biblioteca de componentes |
| **pgAdmin**       | http://localhost:8080               | Administração do banco    |

## 🚀 Credenciais de Teste

**SEMPRE use estas credenciais para desenvolvimento:**

- **Email**: `admin@synqcore.com`
- **Senha**: `SynQcore@Admin123!`
- **Username**: `admin`

## �️ Configurações Aplicadas

### launchSettings.json

```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5000"
    }
  }
}
```

### Program.cs

```csharp
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger"; // /swagger path
});
```

### Variáveis de Ambiente

```bash
export ASPNETCORE_URLS="http://localhost:5000"
export ASPNETCORE_ENVIRONMENT=Development
```

## 📈 Uso Recomendado

### 🥇 Desenvolvimento Diário

```bash
./synqcore start    # Aplicação completa em um comando
```

### 🔧 Desenvolvimento Específico

```bash
./synqcore api      # Apenas back-end
./synqcore blazor   # Apenas front-end
./synqcore clean    # Limpeza antes de push
```

### 🐳 Infraestrutura

```bash
./synqcore docker-up    # PostgreSQL + Redis + pgAdmin
./synqcore docker-down  # Parar infraestrutura
```

## ⚠️ Troubleshooting

### Porta Ocupada

O script resolve automaticamente, mas pode verificar manualmente:

```bash
lsof -i :5000
pkill -f "dotnet.*SynQcore"
```

### Build Fails

```bash
./synqcore clean
./synqcore start
```

### CLR Internal Error

```bash
# O script já trata isso automaticamente com:
dotnet build-server shutdown
rm -rf src/*/bin src/*/obj
dotnet build --maxcpucount:1
```

## 🏆 Vantagens do Script Unificado

- ✅ **Simplicidade**: Um script, todas as funcionalidades
- ✅ **Robustez**: Tratamento automático de problemas comuns
- ✅ **Performance**: Build otimizado e gerenciamento de recursos
- ✅ **Produtividade**: Menos comandos para memorizar
- ✅ **Consistência**: Comportamento padronizado em todos os ambientes
- ✅ **Manutenibilidade**: Código centralizado e organizado

**Resultado: Desenvolvimento mais ágil e confiável! 🎯**

#### Limpeza de Arquivos Desnecessários

```bash
# Limpeza completa de arquivos desnecessários
python3 scripts/cleanup-project.py

# Remove: backups, scripts shell, logs antigos, arquivos temporários
```

#### Correção de Warnings

```bash
# Remover comentários XML (exceto controllers)
python3 scripts/remove-xml-comments.py

# Corrigir warnings CA1861 (arrays constantes)
python3 scripts/fix-ca1861-warnings-v2.py
python3 scripts/fix-ca1861-complete.py
python3 scripts/fix-descending-arrays.py
```

### 🔧 Scripts Shell (Legado - Sendo Migrados)

### 🚀 Inicialização da API na Porta 5000

#### Script Completo

```bash
./scripts/start-api-5000.sh
```

- Script completo com verificações e cleanup
- Verifica se a porta está livre
- Mata processos conflitantes automaticamente
- Exibe URLs formatadas
- Cleanup automático ao encerrar (Ctrl+C)

#### Acesso Rápido

```bash
./start.sh
```

- Link simbólico na raiz para o script completo
- Todas as funcionalidades do script completo
- Acesso conveniente

### 🐳 Ambiente Docker

#### Iniciar Infraestrutura

```bash
./scripts/start-dev.sh
```

- Inicia PostgreSQL, Redis e pgAdmin
- Exibe informações de acesso
- Inclui instruções para iniciar a API

#### Parar Ambiente

```bash
./scripts/stop-dev.sh
```

#### Limpeza Completa

```bash
./scripts/clean-build.sh
```

### 🔧 Scripts de Migração

#### Migração AutoMapper (Concluída)

```bash
./scripts/fix_automapper.sh
```

- Script de migração do AutoMapper para mapeamento manual
- **Status**: ✅ Concluída - Aplicação 100% livre de dependências comerciais

## URLs Configuradas

Após executar qualquer script de inicialização da API:

- **API Base**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

## Configurações Alteradas

### launchSettings.json

```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5000"
    }
  }
}
```

### Program.cs

```csharp
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger"; // /swagger path
});
```

### Variáveis de Ambiente

```bash
export ASPNETCORE_URLS="http://localhost:5000"
export ASPNETCORE_ENVIRONMENT=Development
```

## Uso Recomendado

1. **Primeira execução**:

   ```bash
   ./scripts/start-dev.sh    # Inicia infraestrutura
   ./scripts/start-api-5000.sh  # Inicia API
   ```

2. **Desenvolvimento diário**:

   ```bash
   ./start.sh  # Start completo e rápido da API
   ```

3. **Teste da API**:
   - Abra http://localhost:5000/swagger
   - Todos os endpoints documentados
   - Autenticação JWT configurada

## Troubleshooting

### Porta 5000 ocupada

O script `start-api-5000.sh` resolve automaticamente, mas pode verificar manualmente:

```bash
lsof -i :5000
pkill -f "dotnet.*SynQcore"
```

### Build fails

```bash
dotnet clean
dotnet restore
dotnet build
```
