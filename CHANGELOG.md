# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Não Lançado]

## [4.1.0] - 2025-09-26

### 🎉 **MARCO: Fase 4.1 - Corporate Real-Time Communication SignalR COMPLETA**

#### ✨ **SignalR Hubs Corporativos**
- **CorporateCollaborationHub**: Hub principal para colaboração em tempo real
  - `JoinTeamChannel` / `LeaveTeamChannel` - Canais de equipe com grupos automáticos
  - `JoinProjectChannel` / `LeaveProjectChannel` - Canais de projeto colaborativos  
  - `SendTeamMessage` / `SendProjectMessage` - Mensagens em tempo real
  - `UpdatePresenceStatus` - Sistema de presença (online/offline/busy)
  - Eventos: `UserOnline`, `UserOffline`, `ReceiveTeamMessage`, `ReceiveProjectMessage`, `UserPresenceChanged`

- **ExecutiveCommunicationHub**: Hub para comunicações executivas e anúncios
  - `SendCompanyAnnouncement` - Anúncios para toda empresa (Manager/HR/Admin)
  - `SendExecutiveCommunication` - Comunicações executivas confidenciais
  - `SendPolicyUpdate` - Atualizações de políticas corporativas (HR/Admin)
  - `JoinDepartmentCommunications` / `SendDepartmentCommunication` - Comunicação departamental
  - Eventos: `ReceiveCompanyAnnouncement`, `ReceiveExecutiveCommunication`, `ReceivePolicyUpdate`, `ReceiveDepartmentCommunication`

#### 🔐 **Segurança e Performance SignalR**
- **JWT Authentication**: Autenticação via query string para WebSocket connections
- **Role-Based Groups**: Grupos automáticos por função (Role_Admin, Role_Manager, etc.)
- **LoggerMessage Delegates**: 18 delegates performance-optimized (Event IDs 4001-4109)
- **Connection Management**: OnConnectedAsync/OnDisconnectedAsync com logging completo

#### 📚 **SignalR Documentation System**
- **SignalRDocumentationController**: 4 endpoints para documentação completa
  - `GET /api/SignalRDocumentation/hubs` - Informações dos Hubs disponíveis
  - `GET /api/SignalRDocumentation/examples/javascript` - Exemplos JavaScript prontos
  - `GET /api/SignalRDocumentation/status` - Status das conexões ativas (Admin)
  - `GET /api/SignalRDocumentation/implementation-guide` - Guia de implementação

#### 🛠️ **Configuração Técnica**
- **Program.cs**: AddSignalR() configurado com JWT support
- **Endpoint Mapping**: `/hubs/corporate-collaboration` e `/hubs/executive-communication`
- **Authentication Flow**: JWT Bearer token via access_token query parameter
- **Error Handling**: Reconexão automática e tratamento de desconexões

---

## [4.2.0] - 2025-09-26

### 🎉 **MARCO: Fase 4.2 - Corporate Notification System COMPLETA**

#### ✨ **Sistema de Notificações Corporativas**
- **3 Novas Entidades**: CorporateNotification, NotificationDelivery, NotificationTemplate
- **5 Enums Corporativos**: NotificationType, NotificationPriority, NotificationStatus, NotificationChannels (flags), NotificationChannel, DeliveryStatus
- **Multi-Channel Delivery**: 7 canais (InApp, Email, Push, SMS, Webhook, Teams, Slack)
- **Workflow Corporativo**: 10 status (Draft → Scheduled → PendingApproval → Approved → Sent)
- **Sistema de Templates**: Reutilizáveis com placeholders dinâmicos
- **Targeting Avançado**: Company-wide, Department-specific, Individual

#### 🏗️ **CQRS Notification System**
- **6 Commands**: CreateNotification, ApproveNotification, SendNotification, MarkAsRead, UpdateNotification, CancelNotification
- **6 Queries**: GetEmployeeNotifications, GetCorporateNotifications, GetNotificationDetails, GetNotificationStats, GetNotificationTemplates, GetPendingApprovals
- **8 Handlers**: Implementação completa com LoggerMessage delegates
- **6 Validators**: FluentValidation para todos os commands

#### 🌐 **REST API Notification Controller**
- **10 Endpoints**: CRUD completo + ações específicas (approve, send, mark-read, cancel)
- **Autorização Role-Based**: Admin/HR/Manager para criação, Employee para leitura
- **Paginação e Filtros**: Sistema completo de busca e paginação

#### 🔧 **Infraestrutura**
- **Database Migration**: AddCorporateNotificationSystem aplicada com sucesso
- **EF Core Configurations**: Relacionamentos otimizados com índices de performance
- **Manual Mapping**: Extensões ToCorporateNotificationDto + ToNotificationDeliveryDto
- **Interface Integration**: ISynQcoreDbContext atualizada com novos DbSets

#### 📊 **Analytics e Tracking**
- **9 Status de Entrega**: Pending → Processing → Delivered → Read → Acknowledged → Failed
- **Timestamps Detalhados**: CreatedAt, DeliveredAt, ReadAt para auditoria completa
- **Sistema de Agendamento**: ScheduledFor + ExpiresAt com validações de negócio

#### 🚀 **Performance e Logging**
- **32 LoggerMessage Delegates**: High-performance logging com Event IDs organizados (5001-5030)
- **Zero Warnings Build**: Compilação limpa seguindo premissas corporativas
- **Mapeamento Manual**: Performance otimizada sem AutoMapper

### 🔧 Melhorado
- **Rate Limiting**: Valores atualizados para padrões modernos (Employee: 500/min, Manager: 1000/min, HR: 1500/min, Admin: 2000/min)
- **Clean Architecture**: Handlers exclusivamente na Application layer
- **XML Documentation**: Seletiva para endpoints e métodos críticos
- **Copilot Instructions**: Práticas atualizadas com feedback de desenvolvimento

### 📈 **Estatísticas do Release**
- **Progresso Geral**: 65% (5.2 de 8 fases concluídas)
- **Arquivos Alterados**: 15+ novos arquivos (entidades, handlers, controllers, validators)
- **Linhas de Código**: 2000+ linhas de código corporativo implementadas
- **Zero Dependências Comerciais**: 100% open-source mantido

### 🎯 **Próximos Passos**
- **Fase 4.3**: Corporate Media e Document Management
- **SignalR Integration**: Real-time notifications via WebSocket
- **Delivery Services**: Implementação de email, SMS e webhooks

## [3.4.0] - 2025-09-26

### ✨ Adicionado - Corporate Feed e Discovery System

#### 🏗️ Novas Entidades
- **FeedEntry**: Entradas no timeline personalizado de cada usuário
- **UserInterest**: Sistema de rastreamento de interesses para personalização
- **FeedPriority** enum: Low, Normal, High, Urgent, Executive
- **FeedReason** enum: Following, SameDepartment, SameTeam, TagInterest, Official, etc.
- **InterestType** enum: Tag, Category, Department, Author, PostType, Skill
- **InterestSource** enum: UserDefined, ViewHistory, LikeHistory, CommentHistory, etc.

#### 🎯 Algoritmo de Relevância Corporativa
- Sistema de scoring baseado em contexto departamental (25%)
- Interesses em tags com decay temporal (35%)
- Importância organizacional para posts oficiais (25%)
- Engajamento social com likes e comentários (15%)
- Decay temporal de 2% por dia para manter conteúdo fresco

#### 📊 Sistema de Feed Personalizado
- Feed corporativo com priorização inteligente
- Filtros avançados por tipo, data, departamento, tags, categorias
- Paginação otimizada com metadata completo
- Ordenação flexível (relevância, data, popularidade)
- Sistema de bookmarks para conteúdo importante
- Ocultação de itens com razões definidas pelo usuário
- Marcação como lido com timestamps de auditoria

#### 💡 Sistema de Interesses Automático
- Tracking automático de interações (view, like, comment, share, bookmark)
- Scoring dinâmico com recalculação baseada em comportamento
- Múltiplos tipos de interesse rastreados
- Limpeza automática de interesses com score baixo
- Fonte de interesse rastreada para transparência

#### 🔌 API REST Completa (12 Endpoints)
- `GET /api/feed` - Obter feed personalizado com filtros
- `PUT /api/feed/{id}/read` - Marcar item como lido
- `PUT /api/feed/{id}/bookmark` - Toggle bookmark
- `PUT /api/feed/{id}/hide` - Ocultar item do feed
- `POST /api/feed/regenerate` - Regenerar feed completo
- `POST /api/feed/interests/update` - Atualizar interesses por interação
- `POST /api/feed/bulk-update` - Processamento em lote (Admin/Manager)
- `GET /api/feed/stats` - Estatísticas do feed do usuário
- `GET /api/feed/interests` - Interesses atuais do usuário

#### 🚀 Handlers CQRS
- **GetCorporateFeedHandler**: Handler principal com algoritmo completo
- **GetFeedStatsHandler**: Estatísticas e analytics
- **GetUserInterestsHandler**: Interesses do usuário
- **MarkFeedItemAsReadHandler**: Marcação de leitura
- **ToggleFeedBookmarkHandler**: Sistema de bookmarks
- **HideFeedItemHandler**: Ocultação de itens
- **RegenerateFeedHandler**: Regeneração inteligente
- **UpdateUserInterestsHandler**: Atualização de interesses
- **ProcessBulkFeedUpdateHandler**: Processamento em lote

#### 📈 Analytics e Estatísticas
- Estatísticas completas do feed (total, não lidos, bookmarkados, ocultos)
- Top posts engajados com scores de relevância
- Interações recentes do usuário com timestamps
- Breakdown por prioridade para análise de conteúdo
- Métricas de interação por tipo de interesse

#### 🔧 Configurações EF Core
- **FeedEntryConfiguration**: Índices otimizados para queries de feed
- **UserInterestConfiguration**: Configurações de precisão e relacionamentos
- Migração `AddCorporateFeedSystem` aplicada com sucesso

### 🎉 Melhorias
- Sistema de mapeamento manual otimizado para DTOs de feed
- Performance melhorada com queries específicas para feed
- Logging estruturado para todas as operações do feed
- Validações corporativas para integridade dos dados
- Preservação de bookmarks durante regeneração de feed

### 🐛 Correções
- Correções nos LoggerMessage delegates para performance
- Ajustes nos campos das entidades para compatibilidade
- Remoção de referências inexistentes (PostLike, AvatarUrl)
- Correções de cultura para métodos ToLower()

## [3.3.0] - 2025-09-26

### ✨ Adicionado - Corporate Collaboration Features
- Sistema completo de endorsements corporativos
- Discussion threads com moderação corporativa
- Sistema de threading hierárquico para comentários
- Mention system corporativo com notificações
- Analytics de discussão e engajamento
- REST API controllers completos (40+ endpoints)

## [3.2.0] - 2025-09-26

### ✨ Adicionado - Knowledge Management System
- CRUD completo de knowledge articles (20+ endpoints)
- Sistema de tags corporativas e skill tagging
- Visibilidade por departamento (4 níveis)
- Approval workflow para políticas oficiais
- Versioning de documentos e articles
- Templates para diferentes tipos de conteúdo

## [3.1.0] - 2025-09-25

### ✨ Adicionado - Department Management System
- CRUD completo para Department entities (6 endpoints)
- Sistema hierárquico com validações de referência circular
- Visualização de hierarquia organizacional
- Associação de funcionários com controle de relacionamentos
- Validações de negócio para estrutura organizacional
- DTOs, Commands/Queries e Handlers completos

## [2.3.0] - 2025-09-24

### ✨ Adicionado - CQRS Corporativo e Admin Management
- Sistema CQRS completo com MediatR
- Admin User Management com seleção de papéis
- Employee Management CRUD com hierarquia
- Upload de arquivos com validação corporativa
- Sistema de mapeamento manual (substituição do AutoMapper)
- Zero dependências comerciais - 100% open-source

## [2.2.0] - 2025-09-24

### ✨ Adicionado - Autenticação Corporativa
- ASP.NET Identity para funcionários (ApplicationUserEntity)
- Autenticação JWT preparada para SSO
- Endpoints de autenticação (/auth/register, /auth/login, /auth/test)
- Integração preparada para Active Directory/LDAP
- Funções corporativas (Employee, Manager, HR, Admin)
- Schema Identity integrado com tabelas de negócio

## [2.1.0] - 2025-09-24

### ✨ Adicionado - Fundação da API Corporativa
- ASP.NET Core Web API com Swagger/OpenAPI
- Pipeline de middleware corporativo (CORS, auditoria, exceções)
- Versionamento de API (v1) para compatibilidade
- Serilog com trilhas de auditoria e logging estruturado
- Verificações de saúde corporativas (/health)
- Rate limiting por departamento/função

## [1.5.0] - 2025-09-23

### ✨ Adicionado - Build e Deploy
- Build limpo sem warnings críticos
- API base executando na porta 5005
- Docker Compose funcional com volumes persistentes
- Configuração de development environment
- Git repository conectado ao GitHub
- Documentação completa atualizada

## [1.4.0] - 2025-09-23

### ✨ Adicionado - Migrações e Banco de Dados
- Migração InitialCreate com 12 tabelas
- Configurações EF Core organizadas por domínio
- Relacionamentos complexos (Manager/Subordinate, Posts/Comments)
- Soft delete global implementado
- Enums para tipos corporativos
- Índices otimizados para performance
- Schema PostgreSQL funcional e testado

## [1.3.0] - 2025-09-23

### ✨ Adicionado - Modelo de Dados Corporativo
- 12 entidades organizadas em 3 domínios:
  - **Organização**: Employee, Department, Team, Position
  - **Comunicação**: Post, Comment, PostLike, CommentLike, Notification
  - **Relacionamentos**: EmployeeDepartment, TeamMembership, ReportingRelationship
- DbContext com todos os DbSets
- Configurações EF Core organizadas por domínio
- GlobalUsings centralizado no projeto Domain

## [1.2.0] - 2025-09-23

### ✨ Adicionado - Arquitetura Base
- Clean Architecture com dependências corretas
- Entity Framework Core 9 com PostgreSQL
- Dependency injection e configuração base
- Estrutura preparada para Repository pattern
- Estrutura preparada para MediatR/CQRS
- Estrutura base para testes (Unit + Integration)

## [1.1.0] - 2025-09-23

### ✨ Adicionado - Setup de Infraestrutura
- Docker Compose (PostgreSQL + Redis + pgAdmin)
- Solução .NET 9 com Clean Architecture (9 projetos)
- Projetos: Domain, Application, Infrastructure, API, Blazor
- Setup do repositório Git com GitHub integration
- .editorconfig, Directory.Build.props
- Docker services (postgres:16, redis:7, pgadmin)

## [1.0.0] - 2025-09-23

### ✨ Inicial
- Projeto SynQcore iniciado
- Definição da arquitetura corporativa
- Setup inicial do repositório
- Documentação base criada