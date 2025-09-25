# 🎨 SynQcore - Diagramas Visuais da Arquitetura

## 📊 Diagrama de Entidades (Entity Relationship)

```mermaid
erDiagram
    %% Organization Domain
    Employee {
        guid Id PK
        string FirstName
        string LastName
        string Email UK
        string Phone
        datetime HireDate
        bool IsActive
        guid ManagerId FK
        string AvatarUrl
    }
    
    Department {
        guid Id PK
        string Name
        string Description
        bool IsActive
    }
    
    Team {
        guid Id PK
        string Name
        string Description
        bool IsActive
    }
    
    Position {
        guid Id PK
        string Title
        string Description
        bool IsActive
    }
    
    %% Communication Domain
    Post {
        guid Id PK
        string Content
        guid AuthorId FK
        int Visibility
        datetime CreatedAt
    }
    
    Comment {
        guid Id PK
        string Content
        guid PostId FK
        guid AuthorId FK
        datetime CreatedAt
    }
    
    PostLike {
        guid Id PK
        guid PostId FK
        guid UserId FK
        int ReactionType
    }
    
    CommentLike {
        guid Id PK
        guid CommentId FK
        guid UserId FK
        int ReactionType
    }
    
    Notification {
        guid Id PK
        guid UserId FK
        string Content
        int Type
        bool IsRead
    }
    
    %% Relationships Domain
    EmployeeDepartment {
        guid Id PK
        guid EmployeeId FK
        guid DepartmentId FK
        datetime StartDate
        datetime EndDate
        bool IsActive
        bool IsPrimary
        string RoleInDepartment
    }
    
    TeamMembership {
        guid Id PK
        guid EmployeeId FK
        guid TeamId FK
        datetime JoinedAt
        datetime LeftAt
        int Role
        bool IsActive
    }
    
    ReportingRelationship {
        guid Id PK
        guid ManagerId FK
        guid SubordinateId FK
        datetime StartDate
        datetime EndDate
        bool IsActive
    }
    
    %% Relacionamentos
    Employee ||--o{ Employee : "manages"
    Employee ||--o{ Post : "authors"
    Employee ||--o{ Comment : "writes"
    Employee ||--o{ PostLike : "likes"
    Employee ||--o{ CommentLike : "likes"
    Employee ||--o{ Notification : "receives"
    Employee ||--o{ EmployeeDepartment : "belongs to"
    Employee ||--o{ TeamMembership : "member of"
    Employee ||--o{ ReportingRelationship : "reports to"
    
    Department ||--o{ EmployeeDepartment : "contains"
    Team ||--o{ TeamMembership : "has members"
    
    Post ||--o{ Comment : "has"
    Post ||--o{ PostLike : "receives"
    Comment ||--o{ CommentLike : "receives"
```

## 🏗️ Diagrama de Arquitetura em Camadas

```mermaid
graph TB
    %% External Layer
    subgraph "🌐 External Layer"
        Browser[Web Browser]
        Mobile[Mobile App]
        API_Client[API Clients]
    end
    
    %% Presentation Layer
    subgraph "📱 Presentation Layer"
        subgraph "API"
            Controllers[Controllers]
            Middleware[Middleware Stack]
            Swagger[Swagger UI]
        end
        
        subgraph "Blazor"
            Pages[Razor Pages]
            Components[Components]
            Services[Client Services]
        end
    end
    
    %% Application Layer
    subgraph "⚙️ Application Layer"
        subgraph "CQRS"
            Commands[Commands]
            Queries[Queries]
            Handlers[Handlers]
        end
        
        subgraph "Cross-Cutting"
            DTOs[DTOs]
            Validators[Validators]
            Mappers[AutoMapper]
            Behaviors[Pipeline Behaviors]
        end
    end
    
    %% Domain Layer
    subgraph "🏛️ Domain Layer"
        subgraph "Core"
            Entities[Entities]
            ValueObjects[Value Objects]
            DomainServices[Domain Services]
        end
        
        subgraph "Abstractions"
            Interfaces[Interfaces]
            Enums[Enums]
            Exceptions[Domain Exceptions]
        end
    end
    
    %% Infrastructure Layer
    subgraph "🔧 Infrastructure Layer"
        subgraph "Data"
            DbContext[EF Core DbContext]
            Repositories[Repositories]
            Configurations[Entity Configurations]
        end
        
        subgraph "External Services"
            Cache[Redis Cache]
            FileStorage[File Storage]
            EmailService[Email Service]
        end
    end
    
    %% Database Layer
    subgraph "💾 Database Layer"
        PostgreSQL[(PostgreSQL 16)]
        Redis[(Redis 7)]
    end
    
    %% Connections
    Browser --> API
    Mobile --> API
    API_Client --> API
    
    Controllers --> Commands
    Controllers --> Queries
    Pages --> Commands
    Pages --> Queries
    
    Commands --> Handlers
    Queries --> Handlers
    Handlers --> Entities
    Handlers --> Repositories
    
    Repositories --> DbContext
    DbContext --> PostgreSQL
    Cache --> Redis
    
    Entities -.-> Interfaces
    Handlers -.-> DTOs
    Handlers -.-> Validators
    Handlers -.-> Mappers
```

## 🔄 Diagrama de Fluxo CQRS

```mermaid
sequenceDiagram
    participant C as Controller
    participant M as MediatR
    participant V as Validator
    participant H as Handler
    participant R as Repository
    participant DB as Database
    participant AM as AutoMapper
    
    %% Command Flow (Write)
    Note over C,AM: Command Flow (Create Employee)
    C->>M: Send CreateEmployeeCommand
    M->>V: Validate Command
    V-->>M: Validation Result
    alt Validation Success
        M->>H: Route to Handler
        H->>R: Save Entity
        R->>DB: Insert Record
        DB-->>R: Success
        R-->>H: Entity Saved
        H->>AM: Map to DTO
        AM-->>H: EmployeeDto
        H-->>M: Return Result
        M-->>C: EmployeeDto
    else Validation Failed
        V-->>M: ValidationException
        M-->>C: BadRequest
    end
    
    %% Query Flow (Read)
    Note over C,AM: Query Flow (Get Employee)
    C->>M: Send GetEmployeeQuery
    M->>H: Route to Handler
    H->>R: Find Entity
    R->>DB: Select Query
    DB-->>R: Entity Data
    R-->>H: Employee Entity
    H->>AM: Map to DTO
    AM-->>H: EmployeeDto
    H-->>M: Return Result
    M-->>C: EmployeeDto
```

## 🔐 Diagrama de Segurança

```mermaid
graph LR
    %% Authentication Flow
    subgraph "🔑 Authentication"
        Login[Login Request]
        JWT[JWT Token]
        Identity[ASP.NET Identity]
    end
    
    %% Authorization Flow
    subgraph "🛡️ Authorization"
        RoleAuth[Role-based Auth]
        PolicyAuth[Policy-based Auth]
        Claims[Claims Principal]
    end
    
    %% Rate Limiting
    subgraph "⏱️ Rate Limiting"
        RL_Middleware[Rate Limit Middleware]
        Redis_RL[Redis Counter]
        Corporate_Rules[Corporate Rules]
    end
    
    %% Security Pipeline
    Client[Client Request] --> RL_Middleware
    RL_Middleware --> JWT
    JWT --> Identity
    Identity --> RoleAuth
    RoleAuth --> Claims
    Claims --> API[Protected API]
    
    %% Rate Limiting Details
    RL_Middleware --> Redis_RL
    RL_Middleware --> Corporate_Rules
    
    %% Role Hierarchy
    Corporate_Rules --> Employee_100[Employee: 100/min]
    Corporate_Rules --> Manager_200[Manager: 200/min]
    Corporate_Rules --> HR_500[HR: 500/min]
    Corporate_Rules --> Admin_500[Admin: 500/min]
```

## 📊 Diagrama de Dados

```mermaid
graph TB
    %% Application Layer
    subgraph "Application"
        API[API Layer]
        Blazor[Blazor App]
    end
    
    %% Caching Layer
    subgraph "📦 Caching Layer"
        Redis[Redis 7]
        subgraph "Cache Types"
            RateLimit[Rate Limiting]
            Sessions[User Sessions]
            TempData[Temp Data]
            SearchCache[Search Cache]
        end
    end
    
    %% Database Layer
    subgraph "💾 Database Layer"
        PostgreSQL[PostgreSQL 16]
        subgraph "Schema Organization"
            Identity_Schema[Identity Tables]
            Business_Schema[Business Tables]
            Audit_Schema[Audit Tables]
        end
    end
    
    %% Data Flow
    API --> Redis
    Blazor --> Redis
    API --> PostgreSQL
    Blazor --> PostgreSQL
    
    Redis --> RateLimit
    Redis --> Sessions
    Redis --> TempData
    Redis --> SearchCache
    
    PostgreSQL --> Identity_Schema
    PostgreSQL --> Business_Schema
    PostgreSQL --> Audit_Schema
    
    %% Table Details
    Identity_Schema --> AspNetUsers[AspNetUsers]
    Identity_Schema --> AspNetRoles[AspNetRoles]
    
    Business_Schema --> Employees[Employees]
    Business_Schema --> Departments[Departments]
    Business_Schema --> Teams[Teams]
    Business_Schema --> Posts[Posts]
    Business_Schema --> Comments[Comments]
    
    Audit_Schema --> AuditLogs[Audit Logs]
    Audit_Schema --> ChangeTracking[Change Tracking]
```

## 🎯 Diagrama de Dependências

```mermaid
graph TD
    %% Projects
    API[SynQcore.API<br/>🌐 Web API]
    BlazorApp[SynQcore.BlazorApp<br/>📱 UI]
    Application[SynQcore.Application<br/>⚙️ Business Logic]
    Domain[SynQcore.Domain<br/>🏛️ Core Models]
    Infrastructure[SynQcore.Infrastructure<br/>🔧 Data Access]
    Common[SynQcore.Common<br/>📦 Shared Utils]
    Shared[SynQcore.Shared<br/>🔗 DTOs]
    
    %% Test Projects
    UnitTests[SynQcore.UnitTests<br/>🧪 Unit Tests]
    IntegrationTests[SynQcore.IntegrationTests<br/>🔬 Integration Tests]
    
    %% Dependencies
    API --> Application
    API --> Infrastructure
    API --> Common
    
    BlazorApp --> Application
    BlazorApp --> Shared
    BlazorApp --> Common
    
    Application --> Domain
    Application --> Common
    
    Infrastructure --> Domain
    Infrastructure --> Application
    Infrastructure --> Common
    
    Shared --> Common
    
    %% Test Dependencies
    UnitTests --> Domain
    UnitTests --> Application
    UnitTests --> Common
    
    IntegrationTests --> API
    IntegrationTests --> Infrastructure
    IntegrationTests --> Common
    
    %% External Dependencies
    subgraph "📚 External Packages"
        EF[Entity Framework Core 9]
        MediatR[MediatR]
        AutoMapper[AutoMapper]
        FluentValidation[FluentValidation]
        Serilog[Serilog]
        JWT[JWT Bearer]
        Redis_Client[Redis Client]
        Swagger[Swashbuckle]
    end
    
    Infrastructure --> EF
    Application --> MediatR
    Application --> AutoMapper
    Application --> FluentValidation
    API --> Serilog
    API --> JWT
    API --> Redis_Client
    API --> Swagger
    
    %% Styling
    classDef project fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef test fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef external fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
    
    class API,BlazorApp,Application,Domain,Infrastructure,Common,Shared project
    class UnitTests,IntegrationTests test
    class EF,MediatR,AutoMapper,FluentValidation,Serilog,JWT,Redis_Client,Swagger external
```

## 🚀 Diagrama de Pipeline de CI/CD (Planejado)

```mermaid
graph LR
    %% Source Control
    subgraph "📝 Source"
        Git[Git Repository]
        PR[Pull Request]
    end
    
    %% Build Pipeline
    subgraph "🔨 Build Pipeline"
        Checkout[Checkout Code]
        Restore[Restore Packages]
        Build[Build Solution]
        Test[Run Tests]
        Pack[Package App]
    end
    
    %% Quality Gates
    subgraph "✅ Quality Gates"
        CodeCoverage[Code Coverage > 80%]
        SonarQube[SonarQube Analysis]
        SecurityScan[Security Scan]
    end
    
    %% Deployment Pipeline
    subgraph "🚀 Deployment"
        DockerBuild[Build Docker Images]
        Registry[Push to Registry]
        Deploy_Dev[Deploy to Dev]
        Deploy_Staging[Deploy to Staging]
        Deploy_Prod[Deploy to Production]
    end
    
    %% Flow
    Git --> Checkout
    PR --> Checkout
    Checkout --> Restore
    Restore --> Build
    Build --> Test
    Test --> Pack
    
    Pack --> CodeCoverage
    CodeCoverage --> SonarQube
    SonarQube --> SecurityScan
    
    SecurityScan --> DockerBuild
    DockerBuild --> Registry
    Registry --> Deploy_Dev
    Deploy_Dev --> Deploy_Staging
    Deploy_Staging --> Deploy_Prod
```

## 📈 Diagrama de Performance e Monitoramento

```mermaid
graph TB
    %% Application Tier
    subgraph "🖥️ Application Tier"
        LoadBalancer[Load Balancer]
        API_Instance1[API Instance 1]
        API_Instance2[API Instance 2]
        API_Instance3[API Instance 3]
    end
    
    %% Monitoring
    subgraph "📊 Monitoring Stack"
        Prometheus[Prometheus]
        Grafana[Grafana]
        AlertManager[Alert Manager]
    end
    
    %% Logging
    subgraph "📝 Logging Stack"
        Serilog_App[Serilog]
        ElasticSearch[ElasticSearch]
        Kibana[Kibana]
    end
    
    %% Caching Tier
    subgraph "⚡ Caching Tier"
        Redis_Primary[Redis Primary]
        Redis_Replica[Redis Replica]
    end
    
    %% Database Tier
    subgraph "💾 Database Tier"
        PostgreSQL_Primary[PostgreSQL Primary]
        PostgreSQL_Replica[PostgreSQL Replica]
    end
    
    %% Connections
    LoadBalancer --> API_Instance1
    LoadBalancer --> API_Instance2
    LoadBalancer --> API_Instance3
    
    API_Instance1 --> Redis_Primary
    API_Instance2 --> Redis_Primary
    API_Instance3 --> Redis_Primary
    Redis_Primary --> Redis_Replica
    
    API_Instance1 --> PostgreSQL_Primary
    API_Instance2 --> PostgreSQL_Primary
    API_Instance3 --> PostgreSQL_Primary
    PostgreSQL_Primary --> PostgreSQL_Replica
    
    %% Monitoring Connections
    API_Instance1 --> Prometheus
    API_Instance2 --> Prometheus
    API_Instance3 --> Prometheus
    Prometheus --> Grafana
    Prometheus --> AlertManager
    
    %% Logging Connections
    API_Instance1 --> Serilog_App
    API_Instance2 --> Serilog_App
    API_Instance3 --> Serilog_App
    Serilog_App --> ElasticSearch
    ElasticSearch --> Kibana
```

---

*Diagramas criados em: 25 de Setembro de 2025*  
*Ferramentas: Mermaid.js*  
*Versão: 1.0*