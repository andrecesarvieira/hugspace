# SynQcore - Script de Desenvolvimento

Este script Python unificado gerencia toda a execução da aplicação SynQcore, incluindo preparação de ambiente, infraestrutura Docker e execução dos serviços.

## 🚀 Uso Rápido

```bash
# Aplicação completa (recomendado)
./synqcore start

# Apenas API
./synqcore api  

# Apenas Blazor
./synqcore blazor

# Ver ajuda
./synqcore help
```

## 📋 Comandos Disponíveis

| Comando | Descrição | Portas |
|---------|-----------|--------|
| `start` | Aplicação completa (API + Blazor + Docker) | 5000, 5226 |
| `api` | Apenas SynQcore API | 5000 |
| `blazor` | Apenas SynQcore Blazor | 5226 |
| `clean` | Limpeza completa do projeto | - |
| `docker-up` | Infraestrutura Docker completa | 5432, 6379, 8080 |
| `docker-down` | Parar infraestrutura Docker | - |
| `help` | Exibir ajuda completa | - |

## 🌐 URLs Importantes

Após inicialização bem-sucedida:

- **Aplicação Principal**: http://localhost:5226
- **API Documentation**: http://localhost:5000/swagger  
- **pgAdmin**: http://localhost:8080
- **Health Check**: http://localhost:5000/health

## 🔐 Credenciais Padrão

Para testes e desenvolvimento:

- **Email**: `admin@synqcore.com`
- **Senha**: `SynQcore@Admin123!`
- **Username**: `admin`

## 🎯 Recursos Automáticos

### Primeira Execução
- Detecta automaticamente primeira execução
- Instala dependências Python necessárias
- Configura infraestrutura Docker
- Compila solução completa
- Aplica migrações do banco

### Monitoramento Inteligente
- Verifica portas disponíveis
- Monitora saúde dos serviços
- Health checks automáticos
- Abertura automática do navegador
- Logs coloridos e organizados

### Limpeza Inteligente
- Remove artefatos de build (`bin`, `obj`)
- Limpa logs antigos
- Reseta environment quando necessário

## 🔧 Pré-requisitos

O script verifica automaticamente:

- **.NET 9 SDK**: https://dotnet.microsoft.com/download
- **Docker Desktop**: https://www.docker.com/products/docker-desktop
- **Python 3.7+**: Para execução do script

### Dependências Python (instaladas automaticamente)

- `requests>=2.31.0`: Para health checks HTTP
- `colorama>=0.4.6`: Para cores no terminal Windows

## 🐳 Infraestrutura Docker

### Serviços Incluídos

| Serviço | Container | Porta | Credenciais |
|---------|-----------|-------|-------------|
| PostgreSQL | `synqcore-postgres` | 5432 | `postgres / SynQcore@Dev123!` |
| Redis | `synqcore-redis` | 6379 | Sem senha |
| pgAdmin | `synqcore-pgadmin` | 8080 | `admin@synqcore.dev / SynQcore@Dev123!` |

### Volumes Persistentes

- `synqcore_postgres_data`: Dados do PostgreSQL
- `synqcore_redis_data`: Dados do Redis  
- `synqcore_pgadmin_data`: Configurações do pgAdmin

## 🔍 Solução de Problemas

### Portas Ocupadas
```bash
# Verificar processos usando portas
netstat -ano | findstr :5000
netstat -ano | findstr :5226

# Parar todos os serviços
./synqcore docker-down
```

### Problemas de Compilação
```bash
# Limpeza completa
./synqcore clean

# Rebuild completo
dotnet clean SynQcore.sln
dotnet build SynQcore.sln
```

### Banco de Dados
```bash
# Resetar containers
docker-compose down -v
./synqcore docker-up

# Aplicar migrações manualmente
cd src/SynQcore.Api
dotnet ef database update
```

### Cache/Redis
```bash
# Limpar cache Redis
docker exec synqcore-redis redis-cli FLUSHALL
```

## 📝 Desenvolvimento

### Estrutura do Script

```
scripts/
├── synqcore.py          # Script principal unificado
├── requirements.txt     # Dependências Python
└── README.md           # Esta documentação
```

### Classe Principal: `SynQcoreManager`

- `check_prerequisites()`: Verifica .NET, Docker, Python
- `setup_docker_infrastructure()`: Configura PostgreSQL + Redis
- `build_solution()`: Compila com otimizações
- `start_*_service()`: Inicia API ou Blazor
- `wait_for_service_health()`: Health checks
- `monitor_services()`: Monitoramento contínuo

## 🎨 Personalização

### Variáveis de Ambiente

```bash
# Personalizar portas
set ASPNETCORE_URLS=http://localhost:3000  # API
set ASPNETCORE_URLS=http://localhost:3001  # Blazor

# Environment específico
set ASPNETCORE_ENVIRONMENT=Production
```

### Configuração de Logging

Logs salvos em: `src/SynQcore.Api/logs/`

- Formato: `synqcore-corporate-yyyy-mm-dd.log`
- Retenção: 30 dias
- Níveis: Information, Warning, Error

## 🏗️ Arquitetura

O script segue a metodologia **Desenvolvimento Incremental** estabelecida:

1. **Verificação Prévia**: Prerequisites, portas, primeira execução
2. **Preparação**: Limpeza, Docker, build
3. **Inicialização**: API → Health Check → Blazor → Health Check
4. **Monitoramento**: Continuous health monitoring
5. **Finalização**: Graceful shutdown com cleanup

## 📞 Suporte

Para problemas específicos:

1. Execute `./synqcore clean` primeiro
2. Verifique logs em `src/SynQcore.Api/logs/`
3. Consulte documentação em `docs/`
4. Reporte issues no GitHub

---

**SynQcore** - Rede Social Corporativa  
Desenvolvido por André César Vieira  
Licença: MIT