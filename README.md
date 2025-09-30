# 🚀 SynQcore - Rede Social Corporativa

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)
[![Blazor](https://img.shields.io/badge/Blazor-Híbrido-green)](https://blazor.net/)
[![Pioneiro Brasil](https://img.shields.io/badge/🇧🇷%20Pioneiro-Brasil-gold)](docs/PESQUISA-MERCADO-REDES-SOCIAIS-CORPORATIVAS.md)
[![Fase](https://img.shields.io/badge/Fase-5%20Completa-success)](docs/ROADMAP.md)
[![Licença](https://img.shields.io/badge/Licença-MIT-yellow.svg)](LICENSE)

> **🇧🇷 PRIMEIRA rede social corporativa open source brasileira em C#/.NET**  
> Plataforma completa para conectar funcionários, facilitar colaboração e preservar conhecimento organizacional.  
> [**Pesquisa de mercado**](docs/PESQUISA-MERCADO-REDES-SOCIAIS-CORPORATIVAS.md) comprovou **zero concorrentes nacionais** - oportunidade única de market leadership.

## ✨ Características Principais

- 🏛️ **Clean Architecture** - Arquitetura empresarial com .NET 9 + CQRS + MediatR
- 🔐 **Autenticação Completa** - JWT + Identity + Sistema de papéis corporativos
- 🌐 **Interface Moderna** - Blazor Híbrido + Design System + PWA Ready
- 📊 **Sistema Corporativo** - 15+ entidades para rede social empresarial completa
- ⚡ **Performance Otimizada** - Mapeamento manual + LoggerMessage + Rate Limiting
- 🔍 **Busca Inteligente** - Full-text search + Expert Finder + AI Recommendations
- 📱 **Tempo Real** - SignalR + Notificações + Presença corporativa
- 🗄️ **Banco Robusto** - PostgreSQL + Redis + Migrações + Schema corporativo
- 🐳 **Docker Ready** - Ambiente completo containerizado
- 🧪 **Testado** - 27 testes (unitários + integração) funcionais
- 🚀 **Open Source** - MIT License + 100% brasileiro

## 🚀 Início Rápido

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://docker.com/) e Docker Compose
- [Python 3](https://python.org/) (para scripts de desenvolvimento)

### Executando Localmente

1. **Clone e configure**
   ```bash
   git clone https://github.com/andrecesarvieira/synqcore
   cd synqcore
   ```

2. **Inicie o ambiente completo**
   ```bash
   # ⭐ RECOMENDADO: Aplicação completa (API + Blazor)
   python3 scripts/start-full.py
   
   # Abre automaticamente:
   # - API + Swagger: http://localhost:5000/swagger
   # - Blazor App: http://localhost:5226
   ```

3. **Credenciais de teste**
   - **Email**: `admin@synqcore.com`
   - **Senha**: `SynQcore@Admin123!`

### Scripts Alternativos

```bash
python3 scripts/start-api-5000.py    # Apenas API
python3 scripts/start-blazor.py      # Apenas Blazor
python3 scripts/start-dev.py         # Apenas Docker (BD + Redis)
```

## 🏗️ Arquitetura

```
src/
├── SynQcore.Domain/        # Entidades e regras de negócio
├── SynQcore.Application/   # Casos de uso (CQRS + MediatR)
├── SynQcore.Infrastructure/# Implementações (EF Core, Redis)
├── SynQcore.Api/          # Web API + Controllers
├── SynQcore.BlazorApp/    # Frontend Blazor Híbrido
└── SynQcore.Common/       # Utilitários compartilhados
```

## 🌐 URLs de Acesso

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **API** | http://localhost:5000 | API RESTful completa |
| **Swagger** | http://localhost:5000/swagger | Documentação interativa |
| **Blazor App** | http://localhost:5226 | Interface moderna |
| **Design System** | http://localhost:5226/design-system | Biblioteca de componentes |
| **pgAdmin** | http://localhost:8080 | Administração do banco |

## 📊 Status do Projeto

**Fase 5 CONCLUÍDA** _(29/09/2025)_ - Interface Blazor + Design System + Scripts Python

| Fase | Status | Descrição |
|------|--------|-----------|
| **Fase 1-2** | ✅ **COMPLETO** | Infraestrutura + API Core + Autenticação |
| **Fase 3** | ✅ **COMPLETO** | Core Corporativo + Knowledge Management |
| **Fase 4** | ✅ **COMPLETO** | SignalR + Notificações + Busca + Media |
| **Fase 5** | ✅ **COMPLETO** | Interface Blazor + Design System |
| **Fase 6-8** | 🎯 **PLANEJADO** | Recursos Avançados + Performance + Deploy |

> 📋 **[Ver ROADMAP completo →](docs/ROADMAP.md)** - Detalhes de todas as fases, funcionalidades implementadas e planejamento futuro.

## 🔌 Principais Endpoints

### Autenticação
```http
POST /api/auth/register    # Registrar funcionário
POST /api/auth/login       # Login JWT
```

### Gestão de Funcionários
```http
GET    /api/employees           # Listar funcionários
POST   /api/employees           # Criar funcionário
GET    /api/employees/{id}      # Detalhes do funcionário
PUT    /api/employees/{id}      # Atualizar funcionário
```

### Busca Corporativa
```http
GET    /api/corporatesearch                    # Busca básica
POST   /api/corporatesearch/advanced           # Busca avançada
GET    /api/corporatesearch/suggestions        # Sugestões
GET    /api/corporatesearch/analytics          # Analytics
```

### Notificações
```http
POST   /api/notifications                      # Criar notificação
GET    /api/notifications/my-notifications     # Minhas notificações
POST   /api/notifications/{id}/approve         # Aprovar notificação
```

> 📚 **[Ver todos os 100+ endpoints no Swagger →](http://localhost:5000/swagger)**

## 🛠️ Stack Tecnológica

| Categoria | Tecnologia | Status |
|-----------|------------|--------|
| **Backend** | .NET 9, ASP.NET Core, EF Core | ✅ Produção |
| **Frontend** | Blazor Server + WebAssembly | ✅ Funcional |
| **Banco** | PostgreSQL 16 + Redis 7 | ✅ Schema Completo |
| **Tempo Real** | SignalR | ✅ Implementado |
| **Arquitetura** | Clean Architecture + CQRS | ✅ Implementado |
| **DevOps** | Docker Compose + Scripts Python | ✅ Automação |

## 📚 Documentação

- 🗺️ **[ROADMAP.md](docs/ROADMAP.md)** - Planejamento detalhado e progresso
- 🏛️ **[ARCHITECTURE.md](docs/ARCHITECTURE.md)** - Arquitetura técnica
- 🎨 **[DIAGRAMS.md](docs/DIAGRAMS.md)** - Diagramas visuais
- 📊 **[PESQUISA-MERCADO](docs/PESQUISA-MERCADO-REDES-SOCIAIS-CORPORATIVAS.md)** - Análise de mercado
- 🧪 **[Testing](docs/testing/)** - Estratégias e relatórios de teste
- 🤝 **[CONTRIBUTING.md](docs/CONTRIBUTING.md)** - Guia de contribuição

## 🏆 Diferenciais Competitivos

### 🇧🇷 **Pioneirismo Nacional**
- **ÚNICA** solução nacional em C#/.NET
- **Zero concorrentes** diretos identificados
- **Market leadership** por pioneirismo comprovado

### 🔧 **Excelência Técnica**
- **Clean Architecture** empresarial
- **Zero dependências comerciais** (AutoMapper, etc.)
- **Performance otimizada** (mapeamento manual + LoggerMessage)
- **100% Open Source** com MIT License

### 🏢 **Foco Corporativo**
- **Compliance LGPD** nativo
- **Hierarquia organizacional** completa
- **Workflow de aprovação** corporativo
- **Multi-channel notifications** integradas

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

> 📝 **[Guia completo de contribuição →](docs/CONTRIBUTING.md)**

## 📄 Licença

Este projeto está licenciado sob a MIT License - veja o arquivo [LICENSE](LICENSE) para detalhes.

---

<p align="center">
  <strong>🏆 PIONEIRO BRASILEIRO</strong><br>
  <em>Primeira rede social corporativa open source nacional em C#/.NET</em><br><br>
  <sub>⭐ <strong>Marque com estrela</strong> se acredita no futuro das soluções corporativas brasileiras!</sub><br>
  <sub>🤝 <strong>Contribuições bem-vindas</strong> - Faça parte da evolução do software corporativo nacional</sub>
</p>
