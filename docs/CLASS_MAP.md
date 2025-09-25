# 🗺️ SynQcore - Mapa de Classes e Responsabilidades

## 📋 Visão Geral

Este documento apresenta um mapeamento detalhado de todas as classes, interfaces e componentes do SynQcore, organizados por responsabilidade e camada arquitetural.

## 🏛️ Domain Layer - Core do Negócio

### 📊 Entidades Principais

#### 🏢 Organization Domain
```
Employee.cs
├── 🎯 Responsabilidade: Funcionário da empresa
├── 📍 Localização: src/SynQcore.Domain/Entities/Organization/
├── 🔗 Relacionamentos:
│   ├── Manager (Employee?) - Gerente direto
│   ├── Subordinates (List<Employee>) - Subordinados
│   ├── EmployeeDepartments (List<EmployeeDepartment>)
│   └── TeamMemberships (List<TeamMembership>)
└── 📋 Propriedades Calculadas:
    ├── FullName → FirstName + LastName
    ├── DisplayName → Email se nome vazio
    └── YearsOfService → Anos desde contratação

Department.cs
├── 🎯 Responsabilidade: Departamento organizacional
├── 📍 Localização: src/SynQcore.Domain/Entities/Organization/
├── 🔗 Relacionamentos:
│   └── EmployeeDepartments (List<EmployeeDepartment>)
└── 📋 Funcionalidades:
    ├── Gerenciar funcionários do departamento
    ├── Controle de ativação/desativação
    └── Metadados organizacionais

Team.cs
├── 🎯 Responsabilidade: Equipe de trabalho/projeto
├── 📍 Localização: src/SynQcore.Domain/Entities/Organization/
├── 🔗 Relacionamentos:
│   └── TeamMemberships (List<TeamMembership>)
└── 📋 Funcionalidades:
    ├── Gerenciar membros da equipe
    ├── Diferentes papéis (TeamRole enum)
    └── Status de atividade

Position.cs
├── 🎯 Responsabilidade: Cargo/posição organizacional
├── 📍 Localização: src/SynQcore.Domain/Entities/Organization/
└── 📋 Funcionalidades:
    ├── Definir hierarquia de cargos
    ├── Descrições e responsabilidades
    └── Status de atividade
```

#### 💬 Communication Domain
```
Post.cs
├── 🎯 Responsabilidade: Publicação na rede social
├── 📍 Localização: src/SynQcore.Domain/Entities/Communication/
├── 🔗 Relacionamentos:
│   ├── Author (Employee) - Autor da publicação
│   ├── Comments (List<Comment>) - Comentários
│   └── PostLikes (List<PostLike>) - Curtidas/reações
└── 📋 Enum: PostVisibility
    ├── Public - Visível para todos
    ├── Department - Apenas departamento
    ├── Team - Apenas equipe
    └── Private - Apenas autor

Comment.cs
├── 🎯 Responsabilidade: Comentário em publicação
├── 📍 Localização: src/SynQcore.Domain/Entities/Communication/
├── 🔗 Relacionamentos:
│   ├── Post (Post) - Publicação comentada
│   ├── Author (Employee) - Autor do comentário
│   └── CommentLikes (List<CommentLike>) - Curtidas
└── 📋 Funcionalidades:
    ├── Thread de comentários
    ├── Sistema de moderação
    └── Auditoria completa

Notification.cs
├── 🎯 Responsabilidade: Notificações do sistema
├── 📍 Localização: src/SynQcore.Domain/Entities/Communication/
├── 🔗 Relacionamentos:
│   └── User (Employee) - Destinatário
└── 📋 Enum: NotificationType
    ├── PostLike - Curtida em post
    ├── Comment - Novo comentário
    ├── Mention - Menção em texto
    └── System - Notificação do sistema
```

#### 🔗 Relationships Domain
```
EmployeeDepartment.cs
├── 🎯 Responsabilidade: Relacionamento Funcionário-Departamento
├── 📍 Localização: src/SynQcore.Domain/Entities/Relationships/
├── 🔗 Relacionamentos:
│   ├── Employee (Employee) - Funcionário
│   └── Department (Department) - Departamento
├── 📋 Metadados Temporais:
│   ├── StartDate - Início no departamento
│   ├── EndDate? - Fim no departamento (nullable)
│   └── IsActive - Status atual
├── 🎯 Funcionalidades Especiais:
│   ├── IsPrimary - Departamento principal
│   ├── RoleInDepartment - Papel específico
│   └── IsCurrentAssignment - Propriedade calculada

TeamMembership.cs
├── 🎯 Responsabilidade: Participação em equipes
├── 📍 Localização: src/SynQcore.Domain/Entities/Relationships/
├── 🔗 Relacionamentos:
│   ├── Employee (Employee) - Membro
│   └── Team (Team) - Equipe
├── 📋 Metadados Temporais:
│   ├── JoinedAt - Data de entrada
│   ├── LeftAt? - Data de saída (nullable)
│   └── IsActive - Status atual
└── 📋 Enum: TeamRole
    ├── Member - Membro regular
    ├── Lead - Líder da equipe
    └── Admin - Administrador

ReportingRelationship.cs
├── 🎯 Responsabilidade: Hierarquia organizacional
├── 📍 Localização: src/SynQcore.Domain/Entities/Relationships/
├── 🔗 Relacionamentos:
│   ├── Manager (Employee) - Gerente
│   └── Subordinate (Employee) - Subordinado
└── 📋 Metadados Temporais:
    ├── StartDate - Início da relação
    ├── EndDate? - Fim da relação
    └── IsActive - Status atual
```

### 🧱 Base Classes e Value Objects

```
BaseEntity.cs
├── 🎯 Responsabilidade: Classe base para todas as entidades
├── 📍 Localização: src/SynQcore.Domain/Common/
├── 📋 Propriedades Auditoria:
│   ├── Id (Guid) - Identificador único
│   ├── CreatedAt (DateTime) - Data de criação
│   ├── UpdatedAt (DateTime?) - Última atualização
│   ├── IsDeleted (bool) - Soft delete flag
│   └── DeletedAt (DateTime?) - Data de exclusão
└── 📋 Métodos:
    ├── MarkAsDeleted() - Soft delete
    └── UpdateTimestamp() - Atualizar timestamp

ApplicationUserEntity.cs
├── 🎯 Responsabilidade: Usuário do sistema (Identity)
├── 📍 Localização: src/SynQcore.Infrastructure/Identity/
├── 🔗 Herança: IdentityUser<Guid>
├── 🔗 Relacionamentos:
│   └── Employee (Employee?) - Perfil do funcionário
└── 📋 Propriedades Corporativas:
    ├── CorporateRole - Função corporativa
    ├── IsActive - Status ativo
    └── LastLoginAt - Último login
```

## ⚙️ Application Layer - Lógica de Negócio

### 🎯 CQRS Commands (Escrita)

#### Employee Management Commands
```
CreateEmployeeCommand.cs
├── 🎯 Responsabilidade: Comando para criar funcionário
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Commands/
├── 🔗 Request: CreateEmployeeRequest
├── 🔗 Response: EmployeeDto
└── 📋 Dados:
    ├── Informações pessoais
    ├── Departamentos (List<Guid>)
    └── Equipes (List<Guid>)

UpdateEmployeeCommand.cs
├── 🎯 Responsabilidade: Comando para atualizar funcionário
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Commands/
├── 🔗 Request: UpdateEmployeeRequest
├── 🔗 Response: EmployeeDto
└── 📋 Funcionalidades:
    ├── Atualização de dados pessoais
    ├── Gestão de departamentos
    └── Gestão de equipes

DeleteEmployeeCommand.cs
├── 🎯 Responsabilidade: Comando para deletar funcionário (soft delete)
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Commands/
├── 🔗 Response: void
└── 📋 Validações:
    ├── Verificar subordinados
    ├── Transferir responsabilidades
    └── Manter integridade referencial

UploadEmployeeAvatarCommand.cs
├── 🎯 Responsabilidade: Upload de avatar do funcionário
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Commands/
├── 🔗 Response: string (URL do avatar)
└── 📋 Validações:
    ├── Tamanho máximo: 5MB
    ├── Tipos permitidos: JPEG, PNG, GIF
    └── Nomeação única com GUID
```

### 🔍 CQRS Queries (Leitura)

#### Employee Management Queries
```
GetEmployeeByIdQuery.cs
├── 🎯 Responsabilidade: Buscar funcionário por ID
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Queries/
├── 🔗 Response: EmployeeDto
└── 📋 Includes:
    ├── Manager navigation
    ├── Departments com relacionamentos
    └── Teams com relacionamentos

GetEmployeesQuery.cs
├── 🎯 Responsabilidade: Listar funcionários com paginação
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Queries/
├── 🔗 Request: EmployeeSearchRequest
├── 🔗 Response: PagedResult<EmployeeDto>
└── 📋 Filtros:
    ├── SearchTerm (nome, sobrenome, email)
    ├── DepartmentId
    ├── TeamId
    └── IsActive

SearchEmployeesQuery.cs
├── 🎯 Responsabilidade: Busca rápida de funcionários
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Queries/
├── 🔗 Response: List<EmployeeDto>
└── 📋 Características:
    ├── Busca em tempo real
    ├── Limit de 20 resultados
    └── Ordenação alfabética

GetEmployeeHierarchyQuery.cs
├── 🎯 Responsabilidade: Visualizar hierarquia organizacional
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Queries/
├── 🔗 Response: EmployeeHierarchyDto
└── 📋 Estrutura:
    ├── Manager chain (superiores)
    ├── Employee details
    └── Subordinates tree
```

### 🔧 Command/Query Handlers

```
CreateEmployeeHandler.cs
├── 🎯 Responsabilidade: Processar criação de funcionário
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Handlers/
├── 🔗 Dependencies: ISynQcoreDbContext, IMapper, ILogger
└── 📋 Fluxo:
    ├── 1. Validar dados de entrada
    ├── 2. Criar entidade Employee
    ├── 3. Criar relacionamentos (departments, teams)
    ├── 4. Salvar no contexto
    ├── 5. Log da operação
    └── 6. Retornar DTO mapeado

UpdateEmployeeHandler.cs
├── 🎯 Responsabilidade: Processar atualização de funcionário
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Handlers/
├── 🔗 Dependencies: ISynQcoreDbContext, IMapper, ILogger
└── 📋 Fluxo:
    ├── 1. Buscar funcionário existente
    ├── 2. Atualizar propriedades
    ├── 3. Gerenciar relacionamentos (add/remove)
    ├── 4. Timestamp de atualização
    └── 5. Salvar e mapear resultado

DeleteEmployeeHandler.cs
├── 🎯 Responsabilidade: Processar exclusão (soft delete)
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Handlers/
└── 📋 Validações de Negócio:
    ├── Verificar se tem subordinados
    ├── Desativar relacionamentos
    ├── Manter integridade histórica
    └── Log de auditoria

GetEmployeesHandler.cs
├── 🎯 Responsabilidade: Processar listagem com filtros
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Handlers/
└── 📋 Otimizações:
    ├── Query otimizada com includes
    ├── Filtros no banco de dados
    ├── Paginação eficiente
    └── Projeção para DTOs
```

### 📋 DTOs (Data Transfer Objects)

```
EmployeeDto.cs
├── 🎯 Responsabilidade: Representação do funcionário para API
├── 📍 Localização: src/SynQcore.Application/Features/Employees/DTOs/
├── 📋 Propriedades:
│   ├── Dados pessoais (Id, FirstName, LastName, Email, etc.)
│   ├── Manager (EmployeeDto?) - Gerente
│   ├── Departments (List<DepartmentDto>)
│   ├── Teams (List<TeamDto>)
│   └── Propriedades calculadas (FullName, YearsOfService)
└── 📋 Uso: Resposta de APIs, binding de UI

CreateEmployeeRequest.cs
├── 🎯 Responsabilidade: Dados para criação de funcionário
├── 📍 Localização: src/SynQcore.Application/Features/Employees/DTOs/
├── 📋 Validações:
│   ├── [Required] FirstName, LastName, Email
│   ├── [EmailAddress] Email
│   ├── [MaxLength] para strings
│   └── Lists não nulas para IDs
└── 📋 Campos:
    ├── Informações pessoais obrigatórias
    ├── DepartmentIds (List<Guid>)
    └── TeamIds (List<Guid>)

UpdateEmployeeRequest.cs
├── 🎯 Responsabilidade: Dados para atualização de funcionário
├── 📍 Localização: src/SynQcore.Application/Features/Employees/DTOs/
└── 📋 Diferenças do Create:
    ├── Alguns campos podem ser opcionais
    ├── ManagerId pode ser atualizado
    └── Listas podem ser vazias (remove all)

PagedResult<T>.cs
├── 🎯 Responsabilidade: Container para resultados paginados
├── 📍 Localização: src/SynQcore.Application/Common/
├── 📋 Propriedades:
│   ├── Items (List<T>) - Dados da página
│   ├── TotalCount (int) - Total de registros
│   ├── PageNumber (int) - Página atual
│   ├── PageSize (int) - Tamanho da página
│   └── TotalPages (int) - Total de páginas
└── 📋 Métodos Calculados:
    ├── HasPreviousPage
    ├── HasNextPage
    └── IsFirstPage, IsLastPage
```

### ✅ Validators (FluentValidation)

```
CreateEmployeeValidator.cs
├── 🎯 Responsabilidade: Validação para criação de funcionário
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Validators/
├── 📋 Regras de Negócio:
│   ├── FirstName: Required, MaxLength(100)
│   ├── LastName: Required, MaxLength(100)
│   ├── Email: Required, EmailAddress, MaxLength(255)
│   ├── Phone: Optional, formato válido
│   ├── HireDate: Required, não futuro
│   ├── DepartmentIds: Must have at least one
│   └── TeamIds: Can be empty
└── 📋 Validações Customizadas:
    ├── Email único no sistema
    ├── Departamentos existem e estão ativos
    └── Teams existem e estão ativos

UpdateEmployeeValidator.cs
├── 🎯 Responsabilidade: Validação para atualização
├── 📍 Localização: src/SynQcore.Application/Features/Employees/Validators/
└── 📋 Validações Especiais:
    ├── ManagerId não pode ser o próprio funcionário
    ├── Não criar ciclos na hierarquia
    └── Email único (exceto o próprio)
```

### 🗺️ Mappings (AutoMapper)

```
EmployeeProfile.cs
├── 🎯 Responsabilidade: Mapeamento Employee <-> EmployeeDto
├── 📍 Localização: src/SynQcore.Application/Common/Mappings/
├── 📋 Mapeamentos:
│   ├── Employee -> EmployeeDto (includes Manager, Departments, Teams)
│   ├── CreateEmployeeRequest -> Employee
│   ├── UpdateEmployeeRequest -> Employee (merge)
│   └── TeamRole enum -> string (custom converter)
└── 📋 Configurações Especiais:
    ├── Ignore navigation properties em requests
    ├── Map calculated properties
    └── Handle nullable relationships
```

## 🔧 Infrastructure Layer - Implementações

### 💾 Data Access

```
SynQcoreDbContext.cs
├── 🎯 Responsabilidade: Context principal do EF Core
├── 📍 Localização: src/SynQcore.Infrastructure/Data/
├── 🔗 Herança: DbContext, ISynQcoreDbContext
├── 📋 DbSets:
│   ├── Employees (Employee)
│   ├── Departments (Department)
│   ├── Teams (Team)
│   ├── Posts (Post)
│   ├── Comments (Comment)
│   ├── EmployeeDepartments (EmployeeDepartment)
│   ├── TeamMemberships (TeamMembership)
│   └── ReportingRelationships (ReportingRelationship)
└── 📋 Configurações:
    ├── Global query filters (IsDeleted)
    ├── Soft delete interceptor
    ├── Audit interceptor
    └── Configurações de entidades por domínio

ISynQcoreDbContext.cs
├── 🎯 Responsabilidade: Interface do contexto de dados
├── 📍 Localização: src/SynQcore.Application/Common/Interfaces/
├── 📋 Operações:
│   ├── DbSets de todas as entidades
│   ├── SaveChangesAsync()
│   └── ChangeTracker access
└── 📋 Benefícios:
    ├── Testabilidade (mockable)
    ├── Inversão de dependência
    └── Clean Architecture compliance
```

### Entity Configurations (EF Core)
```
EmployeeConfiguration.cs
├── 🎯 Responsabilidade: Configuração da entidade Employee
├── 📍 Localização: src/SynQcore.Infrastructure/Data/Configurations/
├── 📋 Configurações:
│   ├── Table name e schema
│   ├── Primary key (Id)
│   ├── Properties (lengths, required, etc.)
│   ├── Índices (Email unique, Manager, etc.)
│   ├── Relationships (Manager/Subordinates)
│   └── Global query filter (IsDeleted)

EmployeeDepartmentConfiguration.cs
├── 🎯 Responsabilidade: Configuração many-to-many Employee-Department
├── 📍 Localização: src/SynQcore.Infrastructure/Data/Configurations/
├── 📋 Características:
│   ├── Composite business key
│   ├── Foreign key constraints
│   ├── Check constraints (EndDate > StartDate)
│   └── Índices para performance
```

## 📱 Presentation Layer - APIs e UI

### 🌐 API Controllers

```
EmployeesController.cs
├── 🎯 Responsabilidade: Endpoints REST para funcionários
├── 📍 Localização: src/SynQcore.Api/Controllers/
├── 🔗 Dependencies: IMediator
├── 📋 Endpoints:
│   ├── POST /api/v1/employees - Criar
│   ├── GET /api/v1/employees/{id} - Buscar por ID
│   ├── PUT /api/v1/employees/{id} - Atualizar
│   ├── DELETE /api/v1/employees/{id} - Deletar
│   ├── GET /api/v1/employees - Listar (paginado)
│   ├── GET /api/v1/employees/search - Buscar
│   ├── GET /api/v1/employees/{id}/hierarchy - Hierarquia
│   └── POST /api/v1/employees/{id}/avatar - Upload avatar
└── 📋 Características:
    ├── [ApiVersion("1.0")]
    ├── [Authorize] nos métodos de modificação
    ├── Swagger documentation completa
    └── HTTP status codes apropriados

AuthController.cs
├── 🎯 Responsabilidade: Autenticação e autorização
├── 📍 Localização: src/SynQcore.Api/Controllers/
├── 📋 Endpoints:
│   ├── POST /api/v1/auth/register - Registrar usuário
│   ├── POST /api/v1/auth/login - Login
│   └── GET /api/v1/auth/test - Testar autenticação
└── 📋 Funcionalidades:
    ├── JWT token generation
    ├── Role-based authentication
    └── Corporate user validation
```

### 🔧 Middleware Stack

```
Program.cs
├── 🎯 Responsabilidade: Configuração da aplicação
├── 📍 Localização: src/SynQcore.Api/
├── 📋 Middleware Pipeline:
│   ├── 1. Exception handling
│   ├── 2. HTTPS redirection
│   ├── 3. Rate limiting
│   ├── 4. CORS
│   ├── 5. Authentication
│   ├── 6. Authorization
│   ├── 7. Swagger (development)
│   └── 8. Controllers mapping
└── 📋 Services Registration:
    ├── DbContext (PostgreSQL)
    ├── Identity (ApplicationUserEntity)
    ├── JWT Authentication
    ├── MediatR
    ├── AutoMapper
    ├── FluentValidation
    ├── Rate Limiting (AspNetCoreRateLimit)
    ├── Health Checks
    └── Serilog Logging

GlobalExceptionHandler.cs
├── 🎯 Responsabilidade: Tratamento global de exceções
├── 📍 Localização: src/SynQcore.Api/Handlers/
├── 📋 Exceções Tratadas:
│   ├── ValidationException -> BadRequest (400)
│   ├── NotFoundException -> NotFound (404)
│   ├── ConflictException -> Conflict (409)
│   ├── UnauthorizedException -> Unauthorized (401)
│   └── Exception -> InternalServerError (500)
└── 📋 Features:
    ├── Structured logging
    ├── ProblemDetails response
    └── Correlation ID tracking

CorporateRateLimitMiddleware.cs
├── 🎯 Responsabilidade: Rate limiting baseado em roles
├── 📍 Localização: src/SynQcore.Api/Middleware/
├── 📋 Políticas:
│   ├── Employee: 100 requests/minute
│   ├── Manager: 200 requests/minute
│   ├── HR: 500 requests/minute
│   └── Admin: 500 requests/minute
└── 📋 Características:
    ├── Redis-based storage
    ├── Corporate role detection
    ├── Bypass para health checks
    └── Custom headers
```

## 🧪 Testing Layer - Qualidade

### 🔬 Test Structure (Planejado)
```
SynQcore.UnitTests/
├── 📁 Domain/
│   ├── EmployeeTests.cs - Testes da entidade Employee
│   ├── BaseEntityTests.cs - Testes da classe base
│   └── ValueObjectTests.cs - Testes dos value objects
├── 📁 Application/
│   ├── Commands/
│   │   ├── CreateEmployeeHandlerTests.cs
│   │   ├── UpdateEmployeeHandlerTests.cs
│   │   └── DeleteEmployeeHandlerTests.cs
│   ├── Queries/
│   │   ├── GetEmployeeByIdHandlerTests.cs
│   │   └── GetEmployeesHandlerTests.cs
│   └── Validators/
│       ├── CreateEmployeeValidatorTests.cs
│       └── UpdateEmployeeValidatorTests.cs
└── 📁 Infrastructure/
    ├── DbContextTests.cs - Testes do contexto
    └── RepositoryTests.cs - Testes dos repositórios

SynQcore.IntegrationTests/
├── 📁 API/
│   ├── EmployeesControllerTests.cs
│   ├── AuthControllerTests.cs
│   └── RateLimitingTests.cs
├── 📁 Database/
│   ├── MigrationTests.cs
│   └── PerformanceTests.cs
└── 📁 Infrastructure/
    ├── RedisTests.cs
    └── EmailServiceTests.cs
```

## 📊 Métricas e Responsabilidades por Camada

### Domain Layer (🏛️)
```
📊 Estatísticas:
├── Entidades: 12 classes
├── Value Objects: 3 classes  
├── Enums: 5 types
├── Interfaces: 2 contracts
└── Responsabilidades:
    ├── 🎯 Business logic pura
    ├── 📋 Domain invariants
    ├── 🔗 Entity relationships
    └── 💼 Corporate rules
```

### Application Layer (⚙️)
```
📊 Estatísticas:
├── Commands: 4 classes
├── Queries: 4 classes
├── Handlers: 8 classes
├── DTOs: 5 classes
├── Validators: 2 classes
├── Mappers: 1 profile
└── Responsabilidades:
    ├── 🎯 Use cases orchestration
    ├── 📋 Input validation
    ├── 🔄 Data transformation
    └── 📨 Cross-cutting concerns
```

### Infrastructure Layer (🔧)
```
📊 Estatísticas:
├── DbContext: 1 class
├── Configurations: 12 classes
├── Services: 3 implementations
├── Migrations: 2 files
└── Responsabilidades:
    ├── 💾 Data persistence
    ├── 🔌 External integrations  
    ├── 🗄️ Cache management
    └── 📧 Infrastructure services
```

### Presentation Layer (📱)
```
📊 Estatísticas:
├── Controllers: 2 classes
├── Middleware: 3 classes
├── Handlers: 1 class
├── Configuration: 1 class
└── Responsabilidades:
    ├── 🌐 HTTP endpoints
    ├── 🔐 Authentication/Authorization
    ├── 📝 Request/Response handling
    └── ⚡ Performance optimization
```

---

*Mapa de classes atualizado em: 25 de Setembro de 2025*  
*Total de classes mapeadas: 67*  
*Cobertura: 100% das classes implementadas*