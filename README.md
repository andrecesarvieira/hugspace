# 🚀 SynQcore - API de Rede Social Corporativa

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)
[![Blazor](https://img.shields.io### 📞 **Corporate Notification System (Fase 4### 💡 **Recursos Corporativos\*\*

- ✅ **Rate Limiting** atualizado (Employee: 500/min, Manager: 1000/min, HR: 1500/min, Admin: 2000/min)
- ✅ **Multi-Channel Delivery** (7 canais: InApp, Email, Push, SMS, Webhook, Teams, Slack)
- ✅ **Workflow de Aprovação** (10 status corporativos)
- ✅ **Sistema de Templates** reutilizáveis com placeholders
- ✅ **Corporate Search** com full-text search em todo conteúdo
- ✅ **Expert Finder** - "Who knows about...?" baseado em skills
- ✅ **Advanced Search** com filtros complexos (título, conteúdo, autor)
- ✅ **Search Analytics** + Trending Topics + Content Statistics
- ✅ **AI-powered Recommendations** baseadas em role/department
- ✅ **Autorização baseada em roles** (HR/Admin para modificações)
- ✅ **Soft Delete** com auditoria completa
- ✅ **Validação corporativa** (FluentValidation + business rules)
- ✅ **Performance Logging** (60+ LoggerMessage delegates)
- ✅ **Swagger UI** completo em http://localhost:5000/swagger
- ✅ **Health Checks** em /health, /health/ready, /health/livetp
  POST /api/notifications - Criar notificação (Admin/HR/Manager)
  GET /api/notifications/my-notifications - Minhas notificações
  GET /api/notifications - Todas as notificações (Admin)
  GET /api/notifications/{id} - Detalhes da notificação
  POST /api/notifications/{id}/approve - Aprovar/rejeitar notificação
  POST /api/notifications/{id}/send - Enviar notificação aprovada
  POST /api/notifications/{id}/mark-read - Marcar como lida
  PUT /api/notifications/{id} - Atualizar rascunho
  POST /api/notifications/{id}/cancel - Cancelar notificação

````

### 🔍 **Corporate Search e Knowledge Discovery (Fase 4.4)**

```http
GET    /api/corporatesearch                     - Busca corporativa básica
POST   /api/corporatesearch/advanced            - Busca avançada (título, conteúdo, autor)
GET    /api/corporatesearch/suggestions         - Sugestões de busca
GET    /api/corporatesearch/category/{category} - Buscar por categoria
GET    /api/corporatesearch/author/{authorId}   - Buscar por autor
GET    /api/corporatesearch/department/{deptId} - Buscar por departamento
POST   /api/corporatesearch/tags               - Buscar por tags
GET    /api/corporatesearch/similar/{contentId} - Conteúdo similar
GET    /api/corporatesearch/recent             - Conteúdo recente
GET    /api/corporatesearch/popular            - Conteúdo popular
GET    /api/corporatesearch/analytics          - Analytics de busca
GET    /api/corporatesearch/trending           - Trending topics
GET    /api/corporatesearch/stats              - Estatísticas de conteúdo
POST   /api/corporatesearch/export             - Exportar resultados
POST   /api/corporatesearch/click              - Registrar clique (analytics)
```-Híbrido-green)](https://blazor.net/)
[![Status Build](https://img.shields.io/badge/Build-Aprovado-brightgreen)](https://github.com/andrecesarvieira/synqcore)
[![Pioneiro Brasil](https://img.shields.io/badge/🇧🇷%20Pioneiro-Brasil-gold)](docs/PESQUISA-MERCADO-REDES-SOCIAIS-CORPORATIVAS.md)
[![Fase](https://img.shields.io/badge/Fase-4.4%20Completa-success)](docs/ROADMAP.md)
[![Licença](https://img.shields.io/badge/Licença-MIT-yellow.svg)](LICENSE)

## 🏆 **PIONEIRO NO BRASIL**

> **🇧🇷 PRIMEIRA rede social corporativa open source 100% brasileira em C#/.NET** > **Pesquisa de mercado comprovada**: [Zero concorrentes nacionais identificados](docs/PESQUISA-MERCADO-REDES-SOCIAIS-CORPORATIVAS.md) > **Oportunidade única**: Market leader por pioneirismo no segmento

---

> **API de Rede Social Corporativa Open Source** para ambientes empresariais
> Plataforma completa para conectar funcionários, facilitar colaboração e preservar conhecimento organizacional.
> Arquitetura empresarial com .NET 9, Clean Architecture e performance otimizada.

## ✨ Características

- 🏛️ **Clean Architecture** - 9 projetos organizados com dependências corretas
- 🔐 **Autenticação JWT** - Identity + ApplicationUserEntity + Database integrado
- 📊 **Modelo Corporativo** - 12 entidades para rede social empresarial completa
- ⚡ **Performance Otimizada** - Sistema de mapeamento manual sem reflection
- 🎯 **Zero Dependências Comerciais** - 100% open-source sem AutoMapper
- 🗄️ **Banco Pronto** - Schema corporativo com 13 tabelas + Identity
- 🎯 **CQRS Preparado** - Commands/Queries/Handlers estruturados
- 📝 **API Corporativa** - Swagger UI + Rate Limiting + Health Checks
- 🌐 **Tempo Real Preparado** - Estrutura preparada para SignalR
- 📱 **PWA Preparado** - Base Blazor Híbrido configurada
- 🔒 **Segurança em Primeiro Lugar** - JWT + Identity + Corporate roles funcionais
- 🌍 **Open Source** - Licença MIT + comunidade colaborativa
- 🚀 **Docker Preparado** - Ambiente completo containerizado e testado
- 🧪 **Testes Implementados** - 27 testes (14 unitários + 13 integração) 100% funcionais

## 🏆 **Descoberta de Mercado: PIONEIRISMO BRASILEIRO**

### 🇧🇷 **PRIMEIRA Rede Social Corporativa Open Source 100% Brasileira**

**Pesquisa de mercado realizada em setembro/2025** comprovou que:

- ✅ **ZERO concorrentes diretos** no Brasil
- ✅ **ZERO soluções nacionais** similares em C#/.NET
- ✅ **Market gap identificado** para soluções corporativas brasileiras
- ✅ **Oportunidade única** de market leadership por pioneirismo

#### 📊 **Dados da Pesquisa**

- **Repositórios analisados**: 26 projetos brasileiros em C#
- **Termos pesquisados**: "rede social corporativa", "corporate social network", "collaboration platform"
- **Resultado**: Apenas utilitários (PIX, CPF/CNPJ) - **nenhum sistema completo**
- **Conclusão**: **SynQcore é ÚNICO no segmento**

#### 🎯 **Posicionamento Estratégico**

- **Pioneiro absoluto** no mercado brasileiro
- **Referência técnica** em Clean Architecture + CQRS para corporações
- **Zero dependências comerciais** - 100% livre para empresas
- **Compliance LGPD** nativo para mercado nacional

> 📈 **Ver análise completa**: [`docs/PESQUISA-MERCADO-REDES-SOCIAIS-CORPORATIVAS.md`](docs/PESQUISA-MERCADO-REDES-SOCIAIS-CORPORATIVAS.md)

---

## 🚀 Início Rápido

### 🎯 **Chat Modes Sempre Ativos**

```bash
# Ver status e instruções de todos os modos sempre ativos
./chatmode.sh
# ou
./cm
````

**Todos os 10 modos** (desenvolvimento, arquitetura, debugging, testing, documentação, deployment, segurança, performance, blazor, api) estão **sempre ativos simultaneamente** para máxima qualidade e consistência.

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
   # Método recomendado - porta 5000 com Swagger automático
   ./start.sh

   # Ou caminho completo
   ./scripts/start-api-5000.sh

   # Ou método tradicional
   dotnet run --project src/SynQcore.Api
   # API disponível em: http://localhost:5000
   ```

5. **Execute a Aplicação Blazor** _(Fase 5)_
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

> **🎯 Fase 4.4 CONCLUÍDA!** _(29/09/2025)_ - Corporate Search e Knowledge Discovery System completo com busca full-text em todo conteúdo corporativo!

📋 **[📈 ROADMAP DETALHADO →](docs/ROADMAP.md)** - Acompanhe todo o progresso e planejamento das 8 fases!

| Fase             | Status           | Descrição                                                                | Timeline |
| ---------------- | ---------------- | ------------------------------------------------------------------------ | -------- |
| **Fase 1**       | ✅ **CONCLUÍDO** | Modelo Corporativo + Database Schema                                     | Set/2025 |
| **Fase 2**       | ✅ **CONCLUÍDO** | API Core + JWT Auth + CQRS + Rate Limiting + Employee + Admin Management | Set/2025 |
| **Fase 3**       | ✅ **CONCLUÍDO** | Core Corporativo + Knowledge Management + Collaboration + Feed           | Set/2025 |
| **Fase 4.1-4.4** | ✅ **CONCLUÍDO** | SignalR + Notifications + Media Management + Corporate Search            | Set/2025 |
| **Fase 5**       | ⏳ Planejado     | Interface Blazor + PWA + Design System                                   | Nov/2025 |

### 🎊 Conquistas das Fases 1-4.4:

```
✅ Fase 1 - Infraestrutura:
   • Docker Compose (PostgreSQL 16 + Redis 7 + pgAdmin)
   • Clean Architecture (9 projetos estruturados)
   • Entity Framework Core 9 (15+ entidades corporativas + migrations)
   • Database Schema (16+ tabelas implementadas no PostgreSQL)

✅ Fase 2 - API Corporativa:
   • JWT Authentication + Identity integrado
   • CQRS com MediatR + FluentValidation
   • Rate Limiting corporativo por função (500-2000 req/min)
   • Employee Management System completo (8 endpoints)
   • Admin User Management com seleção de papéis (3 endpoints)
   • Sistema de Mapeamento Manual (performance otimizada)

✅ Fase 3 - Core Corporativo:
   • Department Management + Hierarquia Organizacional
   • Knowledge Management + Articles + Tags + Workflow
   • Corporate Collaboration + Discussion Threads + Endorsements
   • Corporate Feed + Discovery System + Personalização

✅ Fase 4.1 - Corporate Real-Time Communication:
   • SignalR Hubs (CorporateCollaborationHub + ExecutiveCommunicationHub)
   • Team/Project channels com mensagens em tempo real
   • Sistema de presença corporativo (online/offline/busy)
   • Executive broadcasts e department communications
   • JWT Authentication para WebSocket connections
   • Documentation Controller com exemplos JavaScript

✅ Fase 4.2 - Corporate Notification System:
   • Corporate Notification System completo (3 entidades)
   • Multi-Channel Delivery (7 canais: Email, Push, SMS, etc.)
   • Workflow de Aprovação (10 status corporativos)
   • Templates System + Analytics + Performance Logging

✅ Fase 4.3 - Corporate Media e Document Management:
   • Corporate Documents Controller (12 endpoints + upload/download)
   • Media Assets Controller (15 endpoints + thumbnails/gallery)
   • Document Templates Controller (10 endpoints + versioning)
   • Database Schema (4 tabelas: CorporateDocuments, MediaAssets, etc.)
   • File Management (upload, versioning, access control)
   • Corporate Asset Library (logos, templates, policies)
   • Authorization role-based + Performance logging

✅ Fase 4.4 - Corporate Search e Knowledge Discovery:
   • Corporate Search Controller (15+ endpoints + analytics)
   • Full-text search em todo conteúdo (Posts, Documents, Media, Employees)
   • Advanced Search (busca avançada com filtros complexos)
   • Expert Finder ("Who knows about...?" baseado em skills)
   • Search Analytics + Trending Topics + Content Stats
   • AI-powered recommendations baseadas em role/department
   • Skills-based search e expertise location
   • Performance otimizada (LoggerMessage delegates + manual mapping)
```

## 🛠️ Stack Tecnológica

| Categoria       | Tecnologia                            | Status             |
| --------------- | ------------------------------------- | ------------------ |
| **Backend**     | .NET 9, ASP.NET Core, EF Core 9       | ✅ Configurado     |
| **Frontend**    | Blazor Híbrido (Server + WebAssembly) | 🚧 Fase 5          |
| **Banco**       | PostgreSQL 16 + Npgsql 9.0.4          | ✅ Schema Completo |
| **Cache**       | Redis 7 Alpine                        | ✅ Configurado     |
| **Tempo Real**  | SignalR                               | 🚧 Fase 4          |
| **Arquitetura** | Clean Architecture + CQRS             | ✅ Implementado    |
| **DevOps**      | Docker Compose + GitHub               | ✅ Funcionando     |

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
POST /api/auth/register    - Registrar novo funcionário
POST /api/auth/login      - Login e obtenção de token JWT
GET  /api/auth/test       - Testar token (requer autenticação)
```

#### 👑 **Usuário Administrador Padrão**

O sistema cria automaticamente um usuário administrador no primeiro boot:

- **Email**: `admin@synqcore.com`
- **Senha**: `SynQcore@Admin123!`
- **Papel**: Admin (acesso completo ao sistema)

#### 👤 **Papel Padrão para Novos Usuários**

Quando um usuário se registra via `/auth/register`:

- **Papel Automático**: `Employee` (funcionário padrão)
- **Permissões**: Acesso básico ao sistema corporativo
- **Escalação**: Admin pode alterar papéis via `/admin/users`

> 🔒 **Importante**: Altere a senha do admin em produção!

### 👥 **Employee Management (Fase 2.5)**

```http
POST   /api/employees           - Criar funcionário
GET    /api/employees/{id}      - Obter funcionário por ID
PUT    /api/employees/{id}      - Atualizar funcionário
DELETE /api/employees/{id}      - Desligar funcionário (soft delete + bloqueio de acesso)
GET    /api/employees           - Listar funcionários (paginação + filtros)
GET    /api/employees/search    - Buscar funcionários (nome/email)
GET    /api/employees/{id}/hierarchy - Ver hierarquia organizacional
POST   /api/employees/{id}/avatar   - Upload de avatar (5MB max)
```

### 👑 **Admin User Management (Fase 2.6)**

```http
POST   /api/admin/users    - Criar usuário com papel específico (Admin only)
GET    /api/admin/users    - Listar todos os usuários (paginação + busca)
GET    /api/admin/roles    - Listar papéis disponíveis no sistema
```

### � **Corporate Notification System (Fase 4.2)**

```http
POST   /api/notifications                    - Criar notificação (Admin/HR/Manager)
GET    /api/notifications/my-notifications   - Minhas notificações
GET    /api/notifications                    - Todas as notificações (Admin)
GET    /api/notifications/{id}               - Detalhes da notificação
POST   /api/notifications/{id}/approve       - Aprovar/rejeitar notificação
POST   /api/notifications/{id}/send          - Enviar notificação aprovada
POST   /api/notifications/{id}/mark-read     - Marcar como lida
PUT    /api/notifications/{id}               - Atualizar rascunho
POST   /api/notifications/{id}/cancel        - Cancelar notificação
```

### �💡 **Recursos Corporativos**

- ✅ **Rate Limiting** atualizado (Employee: 500/min, Manager: 1000/min, HR: 1500/min, Admin: 2000/min)
- ✅ **Multi-Channel Delivery** (7 canais: InApp, Email, Push, SMS, Webhook, Teams, Slack)
- ✅ **Workflow de Aprovação** (10 status corporativos)
- ✅ **Sistema de Templates** reutilizáveis com placeholders
- ✅ **Autorização baseada em roles** (HR/Admin para modificações)
- ✅ **Soft Delete** com auditoria completa
- ✅ **Validação corporativa** (FluentValidation + business rules)
- ✅ **Performance Logging** (32 LoggerMessage delegates)
- ✅ **Swagger UI** completo em http://localhost:5000/swagger
- ✅ **Health Checks** em /health, /health/ready, /health/live

## 📝 Comandos Úteis

### 🐳 **Docker & Infraestrutura:**

```bash
# Iniciar todos os serviços
docker compose up -d

# Verificar status dos containers
docker compose ps

# Parar todos os serviços
docker compose down

# Logs dos serviços
docker compose logs -f postgres redis pgadmin
```

### 🔧 **Desenvolvimento:**

**🐍 PADRÃO ESTABELECIDO: Todos os scripts são feitos em Python**

```bash
# Build completo (zero warnings)
dotnet build

# Executar todos os testes (27 testes implementados)
dotnet test

# Executar apenas testes unitários (14 testes)
dotnet test tests/SynQcore.UnitTests/

# Executar apenas testes de integração (13 testes)
dotnet test tests/SynQcore.IntegrationTests/

# Aplicar migrações
dotnet ef database update -p src/SynQcore.Infrastructure -s src/SynQcore.Api

# Criar nova migração
dotnet ef migrations add <NomeMigracao> -p src/SynQcore.Infrastructure -s src/SynQcore.Api
```

### 🐍 **Scripts Python (Padrão do Projeto):**

```bash
# Script consolidador (recomendado)
python3 synqcore help                    # Ver todos os comandos
python3 synqcore clean                   # Limpeza completa (build/cache)
python3 synqcore cleanup                 # Limpeza de arquivos desnecessários
python3 synqcore start-dev               # Ambiente de desenvolvimento
python3 synqcore start-api               # Iniciar API (porta 5000)
python3 synqcore test-collab             # Testes automatizados

# Execução direta dos scripts
python3 scripts/clean-build.py           # Limpeza completa
python3 scripts/cleanup-project.py       # Remove backups/scripts shell
python3 scripts/start-dev.py             # Ambiente Docker
python3 scripts/start-api-5000.py        # API com Swagger
python3 scripts/test-collaboration-features.py  # Testes
```

### 🔧 **Scripts Legado (Shell - Sendo Migrados):**

```bash
# Execução de API (legado)
./start.sh                              # Link para start-api-5000.sh
./scripts/start-api-5000.sh             # Script shell original
```

## 🌐 Acesso Local

| Serviço              | URL                           | Status         | Credenciais                                        |
| -------------------- | ----------------------------- | -------------- | -------------------------------------------------- |
| **API**              | http://localhost:5000         | ✅ Funcionando | **Admin**: admin@synqcore.com / SynQcore@Admin123! |
| **Swagger UI**       | http://localhost:5000/swagger | ✅ Funcionando | Use o admin acima para testar endpoints            |
| **Aplicação Blazor** | http://localhost:5001         | 🚧 Fase 5      | -                                                  |
| **pgAdmin**          | http://localhost:8080         | ✅ Funcionando | admin@synqcore.dev / admin123                      |
| **PostgreSQL**       | localhost:5432                | ✅ Funcionando | synqcore_user / synqcore_dev_password              |
| **Redis**            | localhost:6379                | ✅ Funcionando | -                                                  |

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

- 🗺️ **[ROADMAP.md](docs/ROADMAP.md)** - Planejamento detalhado das 8 fases de desenvolvimento
- 📊 **Status Atual:** Fase 2.6 completa (38% do projeto)

### 📚 **Documentação Técnica**

- 📂 **[docs/](docs/README.md)** - Índice completo da documentação
- 🏛️ **[ARCHITECTURE.md](docs/ARCHITECTURE.md)** - Visão completa da arquitetura Clean Architecture
- 🎨 **[DIAGRAMS.md](docs/DIAGRAMS.md)** - Diagramas visuais (Mermaid) de entidades, fluxos e dependências
- 🗺️ **[CLASS_MAP.md](docs/CLASS_MAP.md)** - Mapa detalhado de todas as classes e responsabilidades
- 📋 **[CHANGELOG.md](docs/CHANGELOG.md)** - Histórico de mudanças e atualizações

### 🧪 **Testes e Validação**

- � **[docs/testing/](docs/testing/README.md)** - Índice completo de testes
- 📋 **[docs/testing/TODOS-OS-TESTES-SWAGGER.md](docs/testing/TODOS-OS-TESTES-SWAGGER.md)** ⭐ - Guia principal
- 🏗️ **[docs/testing/ESTRATEGIA-TESTES.md](docs/testing/ESTRATEGIA-TESTES.md)** - Estratégia estabelecida

### �🤝 **Contribuição e Legal**

- 🤝 **[CONTRIBUTING.md](docs/CONTRIBUTING.md)** - Guia para contribuição
- 👤 **[AUTHOR.md](docs/AUTHOR.md)** - Informações sobre o autor
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
  <strong>🏆 PIONEIRO BRASILEIRO</strong><br>
  <em>Primeira rede social corporativa open source nacional em C#/.NET</em><br><br>

<sub>⭐ **Marque com estrela** se acredita no futuro das soluções corporativas brasileiras!</sub><br>
<sub>🤝 **Contribuições bem-vindas** - Faça parte da evolução do software corporativo nacional</sub>

</p>
