# 🚀 HugSpace - Rede Social Moderna e Amigável

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)
[![Blazor](https://img.shields.io/badge/Blazor-Hybrid-green)](https://blazor.net/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> Uma rede social moderna, segura e centrada na comunidade, construída com .NET 9, Blazor e PostgreSQL.

## ✨ Características

- 🏛️ **Clean Architecture** - Código organizacional e testável
- ⚡ **Performance** - PostgreSQL + Redis + .NET 9
- 🌐 **Real-time** - SignalR para chat e notificações
- 📱 **PWA** - Experiência mobile-first
- 🔒 **Segurança** - Autenticação JWT + moderação AI
- 🌍 **Open Source** - Comunidade colaborativa

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

3. **Execute a API**
   ```bash
   dotnet run --project src/HugSpace.Api
   ```

4. **Execute o Blazor App**
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

Consulte nosso [ROADMAP.md](ROADMAP.md) para acompanhar o progresso detalhado.

- ✅ **Fase 1**: Infraestrutura e Clean Architecture
- 🔧 **Fase 2**: API Core e Autenticação
- ⏳ **Fase 3**: Funcionalidades Sociais
- ⏳ **Fase 4**: Chat e Mídia Real-time
- ⏳ **Fase 5**: Interface Blazor Avançada

## 🛠️ Tecnologias

| Categoria | Tecnologia |
|-----------|------------|
| **Backend** | .NET 9, ASP.NET Core, EF Core |
| **Frontend** | Blazor Hybrid (Server + WebAssembly) |
| **Banco** | PostgreSQL 16, Redis 7 |
| **Real-time** | SignalR |
| **Arquitetura** | Clean Architecture, CQRS, MediatR |
| **DevOps** | Docker, GitHub Actions |

## 📝 Scripts Úteis

```bash
# Iniciar ambiente de desenvolvimento
./scripts/start-dev.sh

# Parar ambiente
./scripts/stop-dev.sh  

# Limpeza de build
./scripts/clean-build.sh

# Executar testes
dotnet test

# Build completo
dotnet build
```

## 🌐 Acesso Local

| Serviço | URL | Credenciais |
|---------|-----|-------------|
| **API** | http://localhost:5000 | - |
| **Blazor App** | http://localhost:5001 | - |
| **pgAdmin** | http://localhost:8080 | admin@hugspace.dev / admin123 |
| **PostgreSQL** | localhost:5432 | hugspace_user / hugspace_dev_password |
| **Redis** | localhost:6379 | - |

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