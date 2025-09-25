# 🚀 SynQcore - API de Rede Social Corporativa

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)
[![Blazor](https://img.shields.io/badge/Blazor-Híbrido-green)](https://blazor.net/)
[![Status Build](https://img.shields.io/badge/Build-Aprovado-brightgreen)](https://github.com/andrecesarvieira/synqcore)
[![Fase](https://img.shields.io/badge/Fase-2.5%20Completa-success)](ROADMAP.md)
[![Licença](https://img.shields.io/badge/Licença-MIT-yellow.svg)](LICENSE)
[![Autor](https://img.shields.io/badge/Autor-André%20César%20Vieira-blue)](https://github.com/andrecesarvieira)

> **API de Rede Social Corporativa Open Source** criada por **[André César Vieira](https://github.com/andrecesarvieira)**  
> Plataforma completa para conectar funcionários, facilitar colaboração e preservar conhecimento organizacional.  
> Arquitetura empresarial com .NET 9, Clean Architecture e performance otimizada.

## 👨‍💻 Sobre o Criador

**André César Vieira** é um desenvolvedor senior especializado em arquitetura .NET e sistemas corporativos escaláveis.  

- 🌐 **GitHub**: [@andrecesarvieira](https://github.com/andrecesarvieira)  
- 📧 **Email**: [andrecesarvieira@hotmail.com](mailto:andrecesarvieira@hotmail.com)  
- 🏗️ **Especialidades**: Clean Architecture, .NET Enterprise, PostgreSQL, Otimização de Performance  

**SynQcore** representa anos de experiência em desenvolvimento corporativo, aplicando as melhores práticas da indústria em um projeto open source completo.

## ✨ Características

- 🏛️ **Clean Architecture** - 9 projetos organizados com dependências corretas
- 🔐 **Autenticação JWT** - Identity + ApplicationUserEntity + Database integrado
- 📊 **Modelo Corporativo** - 12 entidades para rede social empresarial completa
- ⚡ **Performance** - PostgreSQL 16 + Redis 7 + .NET 9 otimizado
- 🗄️ **Banco Pronto** - Schema corporativo com 13 tabelas + Identity
- 🎯 **CQRS Preparado** - Commands/Queries/Handlers estruturados
- 📝 **API Corporativa** - Swagger UI + Rate Limiting + Health Checks
- 🌐 **Tempo Real Preparado** - Estrutura preparada para SignalR
- 📱 **PWA Preparado** - Base Blazor Híbrido configurada
- 🔒 **Segurança em Primeiro Lugar** - JWT + Identity + Corporate roles funcionais
- 🌍 **Open Source** - Licença MIT + comunidade colaborativa
- 🚀 **Docker Preparado** - Ambiente completo containerizado e testado
- 🧪 **Testes Preparados** - Estrutura para testes unitários e integração

## 🚀 Início Rápido

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://docker.com/) e Docker Compose
- [Git](https://git-scm.com/)

### Executando Localmente

1. **Clone o repositório**
   ```bash
   git clone https://github.com/andrecesarvieira/synqcore
   cd synqcore
   ```

2. **Inicie a infraestrutura**
   ```bash
   ./scripts/start-dev.sh
   ```

3. **Aplicar migrações do banco**
   ```bash
   dotnet ef database update -p src/SynQcore.Infrastructure -s src/SynQcore.Api
   ```

4. **Execute a API**
   ```bash
   dotnet run --project src/SynQcore.Api
   # API disponível em: http://localhost:5006
   ```

5. **Execute a Aplicação Blazor** *(Fase 5)*
   ```bash
   dotnet run --project src/SynQcore.BlazorApp/SynQcore.BlazorApp
   ```

## 🏗️ Arquitetura

```
src/
├── SynQcore.Domain/        # Entidades e regras de negócio
├── SynQcore.Application/   # Casos de uso (CQRS + MediatR)
├── SynQcore.Infrastructure/# Implementações (EF Core, Redis)
├── SynQcore.Api/          # Web API
├── SynQcore.BlazorApp/    # Frontend Blazor Híbrido
└── SynQcore.Shared/       # DTOs compartilhados
```

## 📊 Status do Desenvolvimento

> **🎯 Fase 2.5 CONCLUÍDA!** *(25/09/2025)* - Employee Management System + CRUD + Hierarquia + Upload completos!

📋 **[📈 ROADMAP DETALHADO →](ROADMAP.md)** - Acompanhe todo o progresso e planejamento das 8 fases!

| Fase | Status | Descrição | Timeline |
|------|--------|-----------|----------|
| **Fase 1** | ✅ **CONCLUÍDO** | Modelo Corporativo + Database Schema | Set/2025 |
| **Fase 2** | ✅ **CONCLUÍDO** | API Core + JWT Auth + CQRS + Rate Limiting + Employee Management | Set/2025 |
| **Fase 3** | 🚀 **PRÓXIMO** | Core Corporativo + Estrutura Organizacional | Out/2025 |
| **Fase 4** | ⏳ Planejado | Chat + Notificações + Mídia | Nov/2025 |
| **Fase 5** | ⏳ Planejado | Interface Blazor + PWA | Dez/2025 |

### 🎊 Conquistas das Fases 1 + 2:
```
✅ Fase 1 - Infraestrutura:
   • Docker Compose (PostgreSQL 16 + Redis 7 + pgAdmin)
   • Clean Architecture (9 projetos estruturados)
   • Entity Framework Core 9 (12 entidades corporativas + migration)
   • Database Schema (13 tabelas implementadas no PostgreSQL)

✅ Fase 2 - API Corporativa:
   • JWT Authentication + Identity integrado
   • CQRS com MediatR + FluentValidation
   • Rate Limiting corporativo por função (Employee/Manager/HR/Admin)
   • Employee Management System completo (8 endpoints)
   • Swagger UI + Health Checks + Audit Logging
   • GlobalExceptionHandler + estrutura de testes
```

## 🛠️ Stack Tecnológica

| Categoria | Tecnologia | Status |
|-----------|------------|---------|
| **Backend** | .NET 9, ASP.NET Core, EF Core 9 | ✅ Configurado |
| **Frontend** | Blazor Híbrido (Server + WebAssembly) | 🚧 Fase 5 |
| **Banco** | PostgreSQL 16 + Npgsql 9.0.4 | ✅ Schema Completo |
| **Cache** | Redis 7 Alpine | ✅ Configurado |
| **Tempo Real** | SignalR | 🚧 Fase 4 |
| **Arquitetura** | Clean Architecture + CQRS | ✅ Implementado |
| **DevOps** | Docker Compose + GitHub | ✅ Funcionando |

### 🏗️ **Arquitetura Implementada:**
```
📁 Clean Architecture (9 Projetos):
├── 🎯 SynQcore.Domain        - Entidades + Regras de Negócio
├── 📋 SynQcore.Application   - Casos de Uso (CQRS + MediatR)  
├── 🔧 SynQcore.Infrastructure - EF Core + Redis + Externos
├── 🌐 SynQcore.Api           - Web API + Controllers
├── 💻 SynQcore.BlazorApp     - Frontend Híbrido
├── 📚 SynQcore.Shared        - DTOs Compartilhados
├── 🏢 SynQcore.Common        - Utilitários Compartilhados
└── 🧪 Tests (Unit + Integration) - Cobertura Preparada
```

## 🔌 API Endpoints Implementados

### 🔐 **Autenticação (Fase 2.2)**
```http
POST /api/v1/auth/register    - Registrar novo funcionário
POST /api/v1/auth/login      - Login e obtenção de token JWT
GET  /api/v1/auth/test       - Testar token (requer autenticação)
```

### 👥 **Employee Management (Fase 2.5)**
```http
POST   /api/v1/employees           - Criar funcionário
GET    /api/v1/employees/{id}      - Obter funcionário por ID  
PUT    /api/v1/employees/{id}      - Atualizar funcionário
DELETE /api/v1/employees/{id}      - Deletar funcionário (soft delete)
GET    /api/v1/employees           - Listar funcionários (paginação + filtros)
GET    /api/v1/employees/search    - Buscar funcionários (nome/email)
GET    /api/v1/employees/{id}/hierarchy - Ver hierarquia organizacional
POST   /api/v1/employees/{id}/avatar   - Upload de avatar (5MB max)
```

### 💡 **Recursos Corporativos**
- ✅ **Rate Limiting** por função (Employee: 100/min, Manager: 200/min, HR/Admin: 500/min)
- ✅ **Autorização baseada em roles** (HR/Admin para modificações)
- ✅ **Soft Delete** com auditoria completa
- ✅ **Validação corporativa** (FluentValidation + business rules)
- ✅ **Swagger UI** completo em http://localhost:5006/swagger
- ✅ **Health Checks** em /health, /health/ready, /health/live

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
dotnet ef database update -p src/SynQcore.Infrastructure -s src/SynQcore.Api

# Criar nova migração
dotnet ef migrations add <NomeMigracao> -p src/SynQcore.Infrastructure -s src/SynQcore.Api

# Executar API (porta 5006)
dotnet run --project src/SynQcore.Api
```

## 🌐 Acesso Local

| Serviço | URL | Status | Credenciais |
|---------|-----|--------|-------------|
| **API** | http://localhost:5006 | ✅ Funcionando | - |
| **Swagger UI** | http://localhost:5006/swagger | ✅ Funcionando | - |
| **Aplicação Blazor** | http://localhost:5001 | 🚧 Fase 5 | - |
| **pgAdmin** | http://localhost:8080 | ✅ Funcionando | admin@synqcore.dev / admin123 |
| **PostgreSQL** | localhost:5432 | ✅ Funcionando | synqcore_user / synqcore_dev_password |
| **Redis** | localhost:6379 | ✅ Funcionando | - |

### 🗄️ **Banco de Dados Atual:**
```
📊 13 Tabelas Criadas (Schema Corporativo Completo):

🏢 Organization:
   Employees              - Perfis de funcionários
   Departments           - Departamentos da empresa
   Teams                 - Times de trabalho
   Positions             - Cargos e posições

💬 Communication:
   Posts                 - Publicações da rede social
   Comments              - Comentários em posts
   PostLikes            - Curtidas em posts (com tipos de reação)
   CommentLikes         - Curtidas em comentários
   Notifications        - Sistema de notificações

🔗 Relationships:
   EmployeeDepartments   - Relacionamentos funcionário-departamento
   TeamMemberships      - Participação em times
   ReportingRelationships - Hierarquia organizacional (Manager/Subordinate)

📋 Sistema:
   __EFMigrationsHistory - Histórico de migrações (EF Core)
```

## 📚 Documentação Completa

### 📋 **Planejamento e Progresso**
- 🗺️ **[ROADMAP.md](ROADMAP.md)** - Planejamento detalhado das 8 fases de desenvolvimento
- 📊 **Status Atual:** Fase 2.5 completa (35% do projeto)

### 📚 **Documentação Técnica**
- 📂 **[docs/](docs/README.md)** - Índice completo da documentação
- 🏛️ **[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)** - Visão completa da arquitetura Clean Architecture
- 🎨 **[docs/DIAGRAMS.md](docs/DIAGRAMS.md)** - Diagramas visuais (Mermaid) de entidades, fluxos e dependências
- 🗺️ **[docs/CLASS_MAP.md](docs/CLASS_MAP.md)** - Mapa detalhado de todas as classes e responsabilidades

### 🧪 **Testes e Validação**
- � **[docs/testing/](docs/testing/README.md)** - Índice completo de testes
- 📋 **[docs/testing/TODOS-OS-TESTES-SWAGGER.md](docs/testing/TODOS-OS-TESTES-SWAGGER.md)** ⭐ - Guia principal
- 🏗️ **[docs/testing/ESTRATEGIA-TESTES.md](docs/testing/ESTRATEGIA-TESTES.md)** - Estratégia estabelecida

### �🤝 **Contribuição e Legal**
- 🤝 **[docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)** - Guia para contribuição
- 📄 **[LICENSE](LICENSE)** - Licença MIT

## 🤝 Contribuindo

Adoramos contribuições! Consulte nosso **[docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)** para começar.

### Desenvolvimento

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está licenciado sob a MIT License - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 💬 Comunidade

- 🐛 **Issues**: [GitHub Issues](https://github.com/andrecesarvieira/synqcore/issues)
- 💡 **Discussões**: [GitHub Discussions](https://github.com/andrecesarvieira/synqcore/discussions)
- 📚 **Wiki**: [Documentação](https://github.com/andrecesarvieira/synqcore/wiki)

---

<p align="center">
  <strong>Created with ❤️ by <a href="https://github.com/andrecesarvieira">André César Vieira</a></strong><br>
  <em>Enterprise Software Architect | .NET Specialist | Open Source Enthusiast</em><br><br>
  
  <a href="https://github.com/andrecesarvieira">
    <img src="https://img.shields.io/badge/Follow-André%20César%20Vieira-blue?style=social&logo=github" alt="Follow André César Vieira">
  </a>
  <a href="mailto:andrecesarvieira@hotmail.com">
    <img src="https://img.shields.io/badge/Contact-Email-red?style=social&logo=gmail" alt="Email André">
  </a>
</p>

<p align="center">
  <sub>⭐ **Marque este repositório com estrela se o SynQcore ajudou você a construir aplicações corporativas melhores!**</sub><br>
  <sub>🤝 **Contribuições são bem-vindas** - Junte-se à revolução do desenvolvimento corporativo</sub>
</p>