# 🚀 SynQcore - Rede Social Corporativa

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)
[![Blazor](https://img.shields.io/badge/Blazor-Híbrido-green)](https://blazor.net/)
[![Pioneiro Brasil](https://img.shields.io/badge/🇧🇷%20Pioneiro-Brasil-gold)](docs/PESQUISA-MERCADO-REDES-SOCIAIS-CORPORATIVAS.md)
[![Fase](<https://img.shields.io/badge/Fase-6.4%20(96%25)-success>)](docs/ROADMAP.md)
[![Licença](https://img.shields.io/badge/Licença-MIT-yellow.svg)](LICENSE)

> **🇧🇷 PRIMEIRA rede social corporativa open source brasileira em C#/.NET**
> Plataforma completa para conectar funcionários, facilitar colaboração e preservar conhecimento organizacional.
> [**Pesquisa de mercado**](docs/PESQUISA-MERCADO-REDES-SOCIAIS-CORPORATIVAS.md) comprovou **zero concorrentes nacionais** - oportunidade única de market leadership.

## ✨ Características Principais

- 🏛️ **Clean Architecture** - Arquitetura empresarial com .NET 9 + CQRS + MediatR
- 🔐 **Autenticação Completa** - JWT + Identity + Sistema de papéis corporativos
- 🛡️ **Privacy/LGPD Compliant** - Sistema completo de conformidade com LGPD/GDPR
- 🌐 **Interface Moderna** - Blazor Híbrido + Design System + Feed Layout Profissional
- 📊 **Sistema Corporativo** - 15+ entidades para rede social empresarial completa
- ⚡ **Performance Otimizada** - Mapeamento manual + LoggerMessage + Rate Limiting + CSS Cache
- 🔍 **Busca Inteligente** - Full-text search + Expert Finder + AI Recommendations
- 📱 **Tempo Real** - SignalR + Notificações + Presença corporativa
- 🗄️ **Banco Robusto** - PostgreSQL + Redis + Migrações + Schema corporativo
- 🐳 **Docker Ready** - Ambiente completo containerizado
- 🧪 **Testado** - 27 testes (unitários + integração) funcionais
- 🚀 **Open Source** - MIT License + 100% brasileiro
- 🎨 **UX/UI Profissional** - Feed 3 colunas + Barra pesquisa 1800px + Alinhamento perfeito

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
   # ⭐ NOVO: Script unificado - substitui todos os outros
   
   # Linux/Mac:
   ./synqcore start
   
   # Windows (PowerShell):
   .\synqcore.ps1 start
   
   # Windows (Command Prompt):
   synqcore.cmd start
   
   # ou usando Python diretamente (todas as plataformas):
   python scripts/synqcore.py start

   # Abre automaticamente:
   # - API + Swagger: http://localhost:5000/swagger
   # - Blazor App: http://localhost:5226
   ```

3. **Credenciais de teste**
   - **Email**: `admin@synqcore.com`
   - **Senha**: `SynQcore@Admin123!`

### Scripts por Plataforma

#### Linux/Mac
```bash
./synqcore help                       # Ajuda completa
./synqcore start                      # Aplicação completa (padrão)
./synqcore api                        # Apenas API na porta 5000
./synqcore blazor                     # Apenas Blazor na porta 5226
./synqcore clean                      # Limpeza completa do projeto
./synqcore docker-up                  # Infraestrutura Docker
./synqcore docker-down                # Parar Docker
```

#### Windows PowerShell
```powershell
.\synqcore.ps1 help                   # Ajuda completa
.\synqcore.ps1 start                  # Aplicação completa (padrão)
.\synqcore.ps1 api                    # Apenas API na porta 5000
.\synqcore.ps1 blazor                 # Apenas Blazor na porta 5226
.\synqcore.ps1 clean                  # Limpeza completa do projeto
.\synqcore.ps1 docker-up              # Infraestrutura Docker
.\synqcore.ps1 docker-down            # Parar Docker
```

#### Windows Command Prompt
```cmd
synqcore.cmd help                     # Ajuda completa
synqcore.cmd start                    # Aplicação completa (padrão)
synqcore.cmd api                      # Apenas API na porta 5000
synqcore.cmd blazor                   # Apenas Blazor na porta 5226
synqcore.cmd clean                    # Limpeza completa do projeto
synqcore.cmd docker-up                # Infraestrutura Docker
synqcore.cmd docker-down              # Parar Docker
```

#### Python (Todas as Plataformas)
```bash
python scripts/synqcore.py help       # Ajuda completa
python scripts/synqcore.py start      # Aplicação completa (padrão)
python scripts/synqcore.py api        # Apenas API na porta 5000
python scripts/synqcore.py blazor     # Apenas Blazor na porta 5226
python scripts/synqcore.py clean      # Limpeza completa do projeto
python scripts/synqcore.py docker-up  # Infraestrutura Docker
python scripts/synqcore.py docker-down # Parar Docker
```

### Scripts Alternativos

```bash
# ⭐ NOVO: Script unificado - todas as funcionalidades
./synqcore help                       # Ajuda completa
./synqcore start                      # Aplicação completa (padrão)
./synqcore api                        # Apenas API na porta 5000
./synqcore blazor                     # Apenas Blazor na porta 5226
./synqcore clean                      # Limpeza completa do projeto
./synqcore docker-up                  # Infraestrutura Docker
./synqcore docker-down                # Parar Docker

# Scripts legados (mantidos para compatibilidade)
python3 scripts/start-api-5000.py    # Apenas API
python3 scripts/start-blazor.py      # Apenas Blazor
python3 scripts/start-dev.py         # Apenas Docker (BD + Redis)
```

## 🛠️ Script Unificado

O **synqcore** é um script Python que consolida todas as funcionalidades necessárias para desenvolvimento:

- **Gerenciamento completo**: API, Blazor, Docker, limpeza
- **Detecção automática de problemas**: Portas ocupadas, processos conflitantes
- **Build otimizado**: Evita erros CLR com compilação single-thread
- **Monitoramento**: Health checks automáticos
- **Browser automático**: Abre interface automaticamente
- **Logs coloridos**: Output organizado por serviço

**Todas as funcionalidades em um único comando! 🎯**

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

| Serviço           | URL                                 | Descrição                 |
| ----------------- | ----------------------------------- | ------------------------- |
| **API**           | http://localhost:5000               | API RESTful completa      |
| **Swagger**       | http://localhost:5000/swagger       | Documentação interativa   |
| **Blazor App**    | http://localhost:5226               | Interface moderna         |
| **Design System** | http://localhost:5226/design-system | Biblioteca de componentes |
| **pgAdmin**       | http://localhost:8080               | Administração do banco    |

## 📊 Status do Projeto

**Fase 6.4 96% CONCLUÍDA** _(01/10/2025)_ - UX/UI Avançado + Feed Layout Profissional + Projeto Optimizado

| Fase         | Status               | Descrição                                    |
| ------------ | -------------------- | -------------------------------------------- |
| **Fase 1-2** | ✅ **COMPLETO**      | Infraestrutura + API Core + Autenticação     |
| **Fase 3**   | ✅ **COMPLETO**      | Core Corporativo + Knowledge Management      |
| **Fase 4**   | ✅ **COMPLETO**      | SignalR + Notificações + Busca + Media       |
| **Fase 5**   | ✅ **COMPLETO**      | Interface Blazor + Design System             |
| **Fase 6.1** | ✅ **COMPLETO**      | Sistema Privacy/LGPD Compliance              |
| **Fase 6.2** | ✅ **COMPLETO**      | Segurança Avançada Corporativa               |
| **Fase 6.3** | ✅ **COMPLETO**      | Sistema de Moderação + Blazor Corrigida      |
| **Fase 6.4** | 🔄 **96% CONCLUÍDO** | UX/UI Avançado + Feed Layout + CSS Otimizado |
| **Fase 7-8** | 🎯 **PLANEJADO**     | Performance + Dashboard + Deploy v1.0        |

### 🎉 **Conquistas Recentes (01/10/2025):**

- ✅ **Feed Layout Profissional** - Grid 3 colunas (280px | 1fr | 320px) alinhado
- ✅ **Barra de Pesquisa Expandida** - 1800px de largura + header fixo independente
- ✅ **CSS Estruturado** - Inline styles + cache versioning + performance otimizada
- ✅ **Estabilidade Blazor** - TaskCanceledException corrigida + navegação funcional
- ✅ **Projeto Limpo** - 15+ arquivos backup/test removidos + build otimizado

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

### Privacy/LGPD Compliance

```http
GET    /api/privacy/data-categories            # Categorias de dados pessoais
POST   /api/privacy/data-categories            # Criar categoria
GET    /api/privacy/processing-activities      # Atividades de processamento
POST   /api/privacy/consent-records            # Registros de consentimento
```

> 📚 **[Ver todos os 100+ endpoints no Swagger →](http://localhost:5000/swagger)**

## 🛠️ Stack Tecnológica

| Categoria       | Tecnologia                      | Status             |
| --------------- | ------------------------------- | ------------------ |
| **Backend**     | .NET 9, ASP.NET Core, EF Core   | ✅ Produção        |
| **Frontend**    | Blazor Server + WebAssembly     | ✅ Funcional       |
| **Banco**       | PostgreSQL 16 + Redis 7         | ✅ Schema Completo |
| **Tempo Real**  | SignalR                         | ✅ Implementado    |
| **Arquitetura** | Clean Architecture + CQRS       | ✅ Implementado    |
| **DevOps**      | Docker Compose + Scripts Python | ✅ Automação       |

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

- **Compliance LGPD** nativo com sistema completo
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
