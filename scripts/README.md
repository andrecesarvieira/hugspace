# Scripts de Inicialização - SynQcore

Este diretório contém scripts para facilitar o desenvolvimento e execução do SynQcore.

## Scripts Disponíveis

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