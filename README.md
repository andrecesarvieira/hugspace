# 🏢 EnterpriseHub - Corporate Social Network (Open Source)

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)
[![Blazor](https://img.shields.io/badge/Blazor-Hybrid-green)](https://blazor.net/)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)](https://github.com/andrecesarvieira/hugspace)
[![Phase](https://img.shields.io/badge/Phase-1%20Complete-success)](ROADMAP.md)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> **Open Source Corporate Social Network** para conectar funcionários, facilitar colaboração e preservar conhecimento organizacional. Construída com .NET 9, Blazor e PostgreSQL.

## ✨ Características

- 🏛️ **Clean Architecture** - 9 projetos organizados com dependências corretas
- ⚡ **Performance** - PostgreSQL 16 + Redis 7 + .NET 9 otimizado
- 🌐 **Real-time Ready** - Estrutura preparada para SignalR
- 📱 **PWA Ready** - Base Blazor Hybrid configurada
- 🔒 **Security First** - Preparado para JWT + Identity + roles
- 🌍 **Open Source** - MIT License + comunidade colaborativa
- 🚀 **Docker Ready** - Ambiente completo containerizado
- 🧪 **Test Ready** - Estrutura para testes unitários e integração

## 🚀 Quick Start

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://docker.com/) e Docker Compose
- [Git](https://git-scm.com/)

### Executando Localmente

1. **Clone o repositório**
   ```bash
   git clone https://github.com/andrecesarvieira/hugspace
   cd hugspace
   ```

2. **Inicie a infraestrutura**
   ```bash
   ./scripts/start-dev.sh
   ```

3. **Aplicar migrações do banco**
   ```bash
   dotnet ef database update -p src/HugSpace.Infrastructure -s src/HugSpace.Api
   ```

4. **Execute a API**
   ```bash
   dotnet run --project src/HugSpace.Api
   # API disponível em: http://localhost:5005
   ```

5. **Execute o Blazor App** *(Fase 2)*
   ```bash
   dotnet run --project src/HugSpace.BlazorApp/HugSpace.BlazorApp
   ```

## 🏗️ Arquitetura

```
src/
├── HugSpace.Domain/        # Entidades e regras de negócio
├── HugSpace.Application/   # Casos de uso (CQRS + MediatR)
├── HugSpace.Infrastructure/# Implementações (EF Core, Redis)
├── HugSpace.Api/          # Web API
├── HugSpace.BlazorApp/    # Frontend Blazor Hybrid
└── HugSpace.Shared/       # DTOs compartilhados
```

## 📊 Status do Desenvolvimento

> **🎯 Fase 1 CONCLUÍDA!** *(21/09/2025)* - Infraestrutura sólida implementada

Consulte nosso [ROADMAP.md](ROADMAP.md) para acompanhar o progresso detalhado.

| Fase | Status | Descrição | Timeline |
|------|--------|-----------|----------|
| **Fase 1** | ✅ **CONCLUÍDO** | Infraestrutura + Clean Architecture | Set/2025 |
| **Fase 2** | 🚀 **PRÓXIMO** | API Core + JWT Auth + CQRS | Out/2025 |
| **Fase 3** | ⏳ Planejado | Funcionalidades Sociais + Feeds | Nov/2025 |
| **Fase 4** | ⏳ Planejado | Chat + Notificações + Mídia | Dez/2025 |
| **Fase 5** | ⏳ Planejado | Interface Blazor + PWA | Jan/2026 |

### 🎊 Conquistas da Fase 1:
```
✅ Docker Compose (PostgreSQL 16 + Redis 7 + pgAdmin)
✅ Clean Architecture (9 projetos estruturados)
✅ Entity Framework Core 9 (5 entidades + migrations)
✅ Build limpo (0 warnings críticos)
✅ GitHub integrado + documentação
✅ Base sólida para desenvolvimento escalável
```

## 🛠️ Stack Tecnológica

| Categoria | Tecnologia | Status |
|-----------|------------|---------|
| **Backend** | .NET 9, ASP.NET Core, EF Core 9 | ✅ Configurado |
| **Frontend** | Blazor Hybrid (Server + WebAssembly) | 🚧 Fase 2 |
| **Banco** | PostgreSQL 16 + Npgsql 9.0.4 | ✅ Funcionando |
| **Cache** | Redis 7 Alpine | ✅ Configurado |
| **Real-time** | SignalR | 🚧 Fase 4 |
| **Arquitetura** | Clean Architecture + CQRS | ✅ Base + 🚧 CQRS |
| **DevOps** | Docker Compose + GitHub | ✅ Funcionando |

### 🏗️ **Arquitetura Implementada:**
```
📁 Clean Architecture (9 Projetos):
├── 🎯 HugSpace.Domain        - Entidades + Business Rules
├── 📋 HugSpace.Application   - Use Cases (CQRS Ready)  
├── 🔧 HugSpace.Infrastructure - EF Core + Redis + External
├── 🌐 HugSpace.Api           - Web API + Controllers
├── 💻 HugSpace.BlazorApp     - Frontend Hybrid
├── 📚 HugSpace.Shared        - DTOs Compartilhados
└── 🧪 Tests (Unit + Integration) - Cobertura Preparada
```

## 📝 Comandos Úteis

### 🐳 **Docker & Infraestrutura:**
```bash
# Iniciar todos os serviços
docker-compose up -d

# Verificar status dos containers
docker-compose ps

# Parar todos os serviços  
docker-compose down

# Logs dos serviços
docker-compose logs -f postgres redis pgadmin
```

### 🔧 **Desenvolvimento:**
```bash
# Build completo (zero warnings)
dotnet build

# Executar testes (quando implementados)
dotnet test

# Aplicar migrações
dotnet ef database update -p src/HugSpace.Infrastructure -s src/HugSpace.Api

# Criar nova migration
dotnet ef migrations add <NomeMigration> -p src/HugSpace.Infrastructure -s src/HugSpace.Api

# Executar API (porta 5005)
dotnet run --project src/HugSpace.Api
```

## 🌐 Acesso Local

| Serviço | URL | Status | Credenciais |
|---------|-----|--------|-------------|
| **API** | http://localhost:5005 | ✅ Funcionando | - |
| **Blazor App** | http://localhost:5001 | 🚧 Fase 2 | - |
| **pgAdmin** | http://localhost:8080 | ✅ Funcionando | admin@hugspace.dev / admin123 |
| **PostgreSQL** | localhost:5432 | ✅ Funcionando | hugspace_user / hugspace_dev_password |
| **Redis** | localhost:6379 | ✅ Funcionando | - |

### 🗄️ **Banco de Dados Atual:**
```
📊 5 Tabelas Criadas:
   Users      - Perfis de usuários
   Posts      - Conteúdo da rede social  
   Follows    - Relacionamentos sociais
   Likes      - Curtidas em posts (PostLike)
   Comments   - Comentários em posts
```

## 🤝 Contribuindo

Adoramos contribuições! Consulte nosso [CONTRIBUTING.md](CONTRIBUTING.md) para começar.

### Desenvolvimento

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está licenciado sob a MIT License - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 💬 Comunidade

- 🐛 **Issues**: [GitHub Issues](https://github.com/andrecesarvieira/hugspace/issues)
- 💡 **Discussões**: [GitHub Discussions](https://github.com/andrecesarvieira/hugspace/discussions)
- 📚 **Wiki**: [Documentação](https://github.com/andrecesarvieira/hugspace/wiki)

---

<p align="center">
  Feito com ❤️ pela comunidade HugSpace
</p>