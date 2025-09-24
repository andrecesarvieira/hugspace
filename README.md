# 🚀 SynQcore - API de Rede Social Corporativa

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)
[![Blazor](https://img.shields.io/badge/Blazor-Híbrido-green)](https://blazor.net/)
[![Status Build](https://img.shields.io/badge/Build-Aprovado-brightgreen)](https://github.com/andrecesarvieira/synqcore)
[![Fase](https://img.shields.io/badge/Fase-2.2%20Completa-success)](ROADMAP.md)
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
## 🚀 Início Rápido

```bash
      # Clone do repositório
   git clone https://github.com/andrecesarvieira/synqcore
   cd synqcore

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
   # API disponível em: http://localhost:5005
   ```

5. **Execute a Aplicação Blazor** *(Fase 2)*
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

> **🎯 Fase 1 CONCLUÍDA!** *(23/09/2025)* - Modelo de dados corporativo completo implementado

Consulte nosso [ROADMAP.md](ROADMAP.md) para acompanhar o progresso detalhado.

| Fase | Status | Descrição | Timeline |
|------|--------|-----------|----------|
| **Fase 1** | ✅ **CONCLUÍDO** | Modelo Corporativo + Database Schema | Set/2025 |
| **Fase 2** | 🚀 **PRÓXIMO** | API Core + JWT Auth + CQRS | Out/2025 |
| **Fase 3** | ⏳ Planejado | Funcionalidades Sociais + Feeds | Nov/2025 |
| **Fase 4** | ⏳ Planejado | Chat + Notificações + Mídia | Dez/2025 |
| **Fase 5** | ⏳ Planejado | Interface Blazor + PWA | Jan/2026 |

### 🎊 Conquistas da Fase 1:
```
✅ Docker Compose (PostgreSQL 16 + Redis 7 + pgAdmin)
✅ Clean Architecture (9 projetos estruturados)
✅ Entity Framework Core 9 (12 entidades corporativas + migration)
✅ Database Schema (13 tabelas implementadas no PostgreSQL)
✅ Configurações EF (Organizadas por domínio + relacionamentos complexos)
✅ GlobalUsings (Centralizados para melhor organização)
✅ Build limpo (0 warnings críticos)
✅ GitHub integrado + documentação atualizada
✅ Base sólida para rede social corporativa escalável
```

## 🛠️ Stack Tecnológica

| Categoria | Tecnologia | Status |
|-----------|------------|---------|
| **Backend** | .NET 9, ASP.NET Core, EF Core 9 | ✅ Configurado |
| **Frontend** | Blazor Híbrido (Server + WebAssembly) | 🚧 Fase 2 |
| **Banco** | PostgreSQL 16 + Npgsql 9.0.4 | ✅ Schema Completo |
| **Cache** | Redis 7 Alpine | ✅ Configurado |
| **Tempo Real** | SignalR | 🚧 Fase 4 |
| **Arquitetura** | Clean Architecture + CQRS | ✅ Base + 🚧 CQRS |
| **DevOps** | Docker Compose + GitHub | ✅ Funcionando |

### 🏗️ **Arquitetura Implementada:**
```
📁 Clean Architecture (9 Projetos):
├── 🎯 SynQcore.Domain        - Entidades + Regras de Negócio
├── 📋 SynQcore.Application   - Casos de Uso (CQRS Preparado)  
├── 🔧 SynQcore.Infrastructure - EF Core + Redis + Externos
├── 🌐 SynQcore.Api           - Web API + Controllers
├── 💻 SynQcore.BlazorApp     - Frontend Híbrido
├── 📚 SynQcore.Shared        - DTOs Compartilhados
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
dotnet ef database update -p src/SynQcore.Infrastructure -s src/SynQcore.Api

# Criar nova migração
dotnet ef migrations add <NomeMigracao> -p src/SynQcore.Infrastructure -s src/SynQcore.Api

# Executar API (porta 5005)
dotnet run --project src/SynQcore.Api
```

## 🌐 Acesso Local

| Serviço | URL | Status | Credenciais |
|---------|-----|--------|-------------|
| **API** | http://localhost:5005 | ✅ Funcionando | - |
| **Aplicação Blazor** | http://localhost:5001 | 🚧 Fase 2 | - |
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