# Instruções para GitHub Copilot - SynQcore

## Sobre o Projeto

O SynQcore é uma plataforma de comunicação corporativa desenvolvida em .NET 9 com Clean Architecture. O ### Credenciais Padrão Estabelecidas
**SEMPRE use estas credenciais para testes e desenvolvimento:**

- **Email**: `admin@synqcore.com`
- **Senha**: `SynQcore@Admin123!`
- **Username**: `admin`
- **Configuração**: Deve estar em TODOS os appsettings (Development, Production, Docker)

### Observações Importantes

- **Idioma**: Todo código, comentários e documentação devem estar em **português brasileiro**
- **Encoding**: Use UTF-8 para todos os arquivos
- **Line Endings**: Use LF (Unix-style) consistentemente
- **Indentação**: Use 4 espaços (não tabs)
- **Cultura**: Configure `pt-BR` como cultura padrão da aplicação visa criar uma rede social corporativa moderna com recursos de colaboração, análise de sentimentos e gestão de conhecimento.

## Arquitetura do Projeto

### Estrutura de Pastas

- `src/SynQcore.Api/` - API RESTful principal
- `src/SynQcore.Application/` - Camada de aplicação (CQRS, Handlers, DTOs)
- `src/SynQcore.Domain/` - Entidades de domínio e regras de negócio
- `src/SynQcore.Infrastructure/` - Acesso a dados, serviços externos
- `src/SynQcore.Common/` - Utilitários e classes compartilhadas
- `src/SynQcore.BlazorApp/` - Frontend Blazor
- `tests/` - Testes unitários e de integração

### Padrões Utilizados

- **Clean Architecture** com separação clara de responsabilidades
- **CQRS (Command Query Responsibility Segregation)** para operações
- **MediatR** para mediação entre camadas
- **Entity Framework Core** para acesso a dados
- **Mapeamento Manual** via extension methods para performance
- **FluentValidation** para validação de dados

## Diretrizes de Código

### Lições Aprendidas Aplicadas (Fase 6.3+ - Outubro 2025)

- **Script Unificado**: OBRIGATÓRIO - Use APENAS `./synqcore [comando]` - NUNCA crie scripts separados
- **Desenvolvimento Incremental**: OBRIGATÓRIO - Uma alteração por vez, valide antes de continuar
- **Organização por Features**: OBRIGATÓRIO - Use estrutura `/Features/{FeatureName}/` com DTOs, Queries, Commands, Handlers separados
- **LoggerMessage Delegates**: OBRIGATÓRIO para performance - implemente em todos os handlers usando `[LoggerMessage]` attribute
- **Registro Manual de Handlers**: SEMPRE registre manualmente no Program.cs - MediatR auto-discovery falha consistentemente
- **Paginação Padrão**: OBRIGATÓRIO - Implemente desde o início para todos os resultados de listagem usando `PagedResult<T>`
- **15+ Endpoints Abrangentes**: Features complexas DEVEM ter endpoints completos (CRUD + Analytics + Support + Suggestions)
- **Compilação Limpa**: NUNCA aceite warnings - resolva todos antes de continuar
- **Validação Contínua**: SEMPRE teste compilação após modificações
- **Context Único**: OBRIGATÓRIO - Use contexto específico no replace_string_in_file para evitar ambiguidade

### Estrutura Obrigatória de Arquivos por Feature

```
src/SynQcore.Application/Features/{FeatureName}/
├── DTOs/{FeatureName}DTOs.cs           # Todos os DTOs da feature
├── Queries/{FeatureName}Queries.cs     # Todas as queries CQRS
├── Commands/{FeatureName}Commands.cs   # Todos os commands CQRS
├── Handlers/{FeatureName}QueryHandler.cs        # Handler principal
├── Handlers/{FeatureName}SupportHandlers.cs     # Handlers de suporte
└── Handlers/{FeatureName}AnalyticsHandlers.cs   # Handlers de analytics
```

### Nomenclatura

- Use **PascalCase** para classes, métodos e propriedades
- Use **camelCase** para parâmetros e variáveis locais
- Use **kebab-case** para rotas de API
- Prefixe interfaces com `I` (ex: `IRepository`)
- Use sufixos descritivos: `Controller`, `Service`, `Repository`, `Handler`, `Validator`
- **Padrão de arquivos**: `{FeatureName}DTOs.cs`, `{FeatureName}Queries.cs`, `{FeatureName}Handlers.cs`

### Estrutura de Classes Otimizada (Padrão Fase 4.4+)

```csharp
// Template de Handler com LoggerMessage para Performance
public class {Feature}QueryHandler : IRequestHandler<{Feature}Query, PagedResult<{Feature}Dto>>
{
    private readonly SynQcoreDbContext _context;
    private readonly ILogger<{Feature}QueryHandler> _logger;

    // OBRIGATÓRIO: LoggerMessage delegates para alta performance
    [LoggerMessage(LogLevel.Information, "Processando {feature} - Parâmetros: {parameters}")]
    private static partial void LogProcessingStarted(ILogger logger, string feature, object parameters, Exception? exception);

    public {Feature}QueryHandler(SynQcoreDbContext context, ILogger<{Feature}QueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<{Feature}Dto>> Handle({Feature}Query request, CancellationToken cancellationToken)
    {
        LogProcessingStarted(_logger, "{Feature}", request, null);

        var query = _context.{Entity}
            .Where(/* filtros */)
            .OrderBy(/* ordenação */)
            .AsQueryable();

        return await query.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
```

### Controllers (Padrão Estabelecido)

- **OBRIGATÓRIO**: Use `ApiController` attribute e `[Authorize]`
- **OBRIGATÓRIO**: Implemente 15+ endpoints para features complexas
- **OBRIGATÓRIO**: Retorne tipos específicos (`ActionResult<T>`)
- **OBRIGATÓRIO**: Use roteamento por atributos com padrão `/api/[controller]`
- **OBRIGATÓRIO**: Documente TODOS endpoints com XML comments para Swagger
- **OBRIGATÓRIO**: Inclua `ProducesResponseType` para todos retornos
- **PADRÃO**: Endpoints básicos + analytics + suggestions + support

### Checklist Obrigatório por Feature

Antes de considerar uma feature concluída, VERIFICAR:

- [ ] **Script Único**: Usou APENAS `./synqcore` para testes - NÃO criou scripts separados
- [ ] **Desenvolvimento Incremental**: Aplicou mudanças uma por vez com validação contínua
- [ ] **DTOs**: Criados em arquivo dedicado `{Feature}DTOs.cs`
- [ ] **Queries**: Criadas em arquivo dedicado `{Feature}Queries.cs`
- [ ] **Commands**: Criados em arquivo dedicado `{Feature}Commands.cs`
- [ ] **Handlers**: LoggerMessage delegates implementados em TODOS
- [ ] **Context Único**: Todos os `replace_string_in_file` usaram contexto específico e único
- [ ] **Paginação**: PagedResult<T> usado em listagens
- [ ] **Registro**: Handlers registrados manualmente no Program.cs
- [ ] **Controller**: 15+ endpoints documentados com XML
- [ ] **Compilação**: Zero warnings, zero errors - validado incrementalmente
- [ ] **Autenticação**: JWT funcionando com credenciais padrão
- [ ] **Testes**: Endpoints testados com `./synqcore start` + dados reais
- [ ] **Sintaxe**: Cada alteração validada antes da próxima

### Handlers CQRS (Template Obrigatório)

- **OBRIGATÓRIO**: Separar Commands (escrita) de Queries (leitura)
- **OBRIGATÓRIO**: Um handler por operação
- **OBRIGATÓRIO**: Use `CancellationToken` em operações assíncronas
- **OBRIGATÓRIO**: Implemente LoggerMessage delegates para performance
- **OBRIGATÓRIO**: Use paginação para resultados de listagem
- **OBRIGATÓRIO**: Implemente validação com FluentValidation

### Template Obrigatório de Handler

```csharp
public class {Feature}QueryHandler : IRequestHandler<{Feature}Query, PagedResult<{Feature}Dto>>
{
    private readonly SynQcoreDbContext _context;
    private readonly ILogger<{Feature}QueryHandler> _logger;

    // OBRIGATÓRIO: LoggerMessage delegates
    [LoggerMessage(LogLevel.Information, "Processando {feature} - Parâmetros: {parameters}")]
    private static partial void LogProcessingStarted(ILogger logger, string feature, object parameters, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Concluído {feature} - Resultados: {count}")]
    private static partial void LogProcessingCompleted(ILogger logger, string feature, int count, Exception? exception);

    public async Task<PagedResult<{Feature}Dto>> Handle({Feature}Query request, CancellationToken cancellationToken)
    {
        LogProcessingStarted(_logger, "{Feature}", request, null);

        var query = _context.{Entity}
            .Where(/* filtros */)
            .OrderBy(/* ordenação */)
            .AsQueryable();

        var result = await query.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);

        LogProcessingCompleted(_logger, "{Feature}", result.Items.Count, null);
        return result;
    }
}
```

### DTOs e Mapeamento

- Crie DTOs específicos para Request/Response
- Use AutoMapper para conversões
- Valide DTOs com DataAnnotations ou FluentValidation

### Tratamento de Erros

- Use classes Result personalizadas
- Implemente middleware global de tratamento de exceções
- Log adequado com diferentes níveis (Information, Warning, Error)

### Testes

- Teste unitário para lógica de domínio
- Teste de integração para APIs
- Use nomenclatura descritiva: `MetodoSendoTestado_Cenario_ResultadoEsperado`
- Mock dependências externas

## Funcionalidades Principais

### Módulos do Sistema

1. **Autenticação e Autorização** - JWT, roles, permissions
2. **Gestão de Funcionários** - CRUD, perfis, departamentos
3. **Threads de Discussão** - Postagens, comentários, reações
4. **Sistema de Endorsements** - Validação de conhecimento entre pares
5. **Feed de Notícias** - Timeline personalizado
6. **Gestão de Conhecimento** - Base de conhecimento corporativo
7. **Notificações** - Sistema em tempo real
8. **Analytics** - Métricas e relatórios
9. **Comunicação Corporativa** - Anúncios oficiais

### Tecnologias Integradas

- **SignalR** para comunicação em tempo real
- **Redis** para cache e sessões
- **PostgreSQL** como banco principal
- **Docker** para containerização
- **Serilog** para logging estruturado

## Boas Práticas

### Performance

- Use `async/await` para operações I/O
- Implemente cache onde apropriado
- Use paginação em listagens grandes
- Otimize queries do Entity Framework

### Segurança

- Valide sempre entrada do usuário
- Use HTTPS em produção
- Implemente rate limiting
- Sanitize dados antes de armazenar

### Manutenibilidade

- Mantenha métodos pequenos e focados
- Use injeção de dependência
- Documente código complexo
- Siga princípios SOLID

## Script de Desenvolvimento (ATUALIZADO - Outubro 2025)

### 🚀 Script Unificado - ÚNICO MÉTODO AUTORIZADO

**NUNCA mais crie scripts separados!** Use EXCLUSIVAMENTE o script unificado:

```bash
# MÉTODO OFICIAL: Script unificado synqcore.py
./synqcore start                      # Aplicação completa (API + Blazor)
./synqcore api                        # Apenas API na porta 5000
./synqcore blazor                     # Apenas Blazor na porta 5226
./synqcore clean                      # Limpeza completa do projeto
./synqcore docker-up                  # Infraestrutura Docker
./synqcore docker-down                # Parar Docker
./synqcore help                       # Ajuda completa

# Alternativa via Python:
python3 scripts/synqcore.py [comando]
```

### ❌ PROIBIDO - Não Crie Mais Scripts

- **NÃO** crie scripts separados tipo `start-api.py`, `start-blazor.py`, etc.
- **NÃO** sugira comandos `dotnet run` manuais
- **NÃO** crie shell scripts (.sh)
- **SEMPRE** use o script unificado existente

### ✅ Recursos do Script Unificado

- **Detecção automática**: Portas ocupadas, processos conflitantes
- **Build otimizado**: Evita erros CLR com single-thread
- **Logs coloridos**: Output organizado por serviço
- **Health checks**: Monitora API e Blazor automaticamente
- **Browser automático**: Abre interface quando pronto
- **Cleanup inteligente**: Limpeza completa de artefatos

## Metodologia de Desenvolvimento (ATUALIZADA - Outubro 2025)

### 🔄 Desenvolvimento Incremental - PADRÃO OBRIGATÓRIO

**SEMPRE use o método incremental linha por linha para prevenir arquivos corrompidos:**

#### 1. Análise Prévia

- **SEMPRE** leia o arquivo completo antes de editar
- Identifique exatamente onde fazer alterações
- Planeje as modificações em pequenos blocos

#### 2. Edição Incremental

- **UMA alteração por vez** usando `replace_string_in_file`
- **Inclua 3-5 linhas de contexto** antes e depois
- **Valide a sintaxe** após cada modificação
- **Teste a compilação** após blocos lógicos

#### 3. Validação Contínua

- Execute `./synqcore clean` se houver problemas
- Execute `dotnet build` para verificar compilação
- Teste funcionalidades incrementalmente
- **NUNCA** faça múltiplas alterações sem validar

#### 4. Prevenção de Corrupção

- **SEMPRE** use strings literais exatas no `oldString`
- **NUNCA** use marcadores como `/* ... */` ou `// ... existing code ...`
- **VERIFIQUE** se o contexto é único no arquivo
- **PARE** imediatamente se houver erros de compilação

### 📝 Template de Workflow Incremental

```markdown
1. PLANEJAR: Identificar alterações necessárias
2. LER: read_file para entender contexto atual
3. EDITAR: replace_string_in_file com contexto específico
4. VALIDAR: Verificar sintaxe e compilação
5. TESTAR: Executar funcionalidade modificada
6. REPETIR: Próxima alteração apenas se anterior válida
```

### ⚠️ Sinais de Alerta

Se encontrar qualquer um destes, **PARE IMEDIATAMENTE**:

- Warnings de compilação
- Erros de sintaxe
- Arquivos corrompidos
- Funcionalidades quebradas
- Context não único no `replace_string_in_file`

### 🎯 Benefícios do Método Incremental

- ✅ **Previne corrupção**: Alterações pequenas e controladas
- ✅ **Facilita debugging**: Erro localizado imediatamente
- ✅ **Reduz retrabalho**: Validação contínua
- ✅ **Melhora qualidade**: Código sempre funcional
- ✅ **Aumenta confiança**: Progresso visível e seguro

## Comandos Úteis (ATUALIZADOS)

### Desenvolvimento Diário

```bash
# Iniciar desenvolvimento
./synqcore start

# Limpeza antes de trabalhar
./synqcore clean

# Verificar saúde da aplicação
curl http://localhost:5000/health

# Testar autenticação
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@synqcore.com","password":"SynQcore@Admin123!"}'
```

### Build e Testes

```bash
# Build seguro (método do script)
dotnet build SynQcore.sln --maxcpucount:1 --verbosity minimal

# Executar testes
dotnet test SynQcore.sln --verbosity normal

# Formatação de código
dotnet format SynQcore.sln
```

### Docker

```bash
# Subir ambiente completo
docker-compose up -d

# Apenas banco de dados
docker-compose up -d postgres redis
```

## Observações Importantes

- **Idioma**: Todo código, comentários e documentação devem estar em **português brasileiro**
- **Encoding**: Use UTF-8 para todos os arquivos
- **Line Endings**: Use LF (Unix-style) consistentemente
- **Indentação**: Use 4 espaços (não tabs)
- **Cultura**: Configure `pt-BR` como cultura padrão da aplicação

## Recursos de Negócio

### Domínio Principal

- Funcionários podem criar discussões sobre tópicos profissionais
- Sistema de endorsements para validar conhecimentos
- Analytics de engajamento e participação
- Feed personalizado baseado em interesses
- Notificações em tempo real
- Gestão hierárquica de departamentos

### Regras de Negócio Importantes

- Apenas funcionários autenticados podem acessar o sistema
- Endorsements requerem validação por pares
- Conteúdo pode ter diferentes níveis de visibilidade
- Analytics respeitam privacidade dos funcionários
- Notificações podem ser configuradas individualmente

Mantenha essas diretrizes em mente ao gerar código e sugestões para o projeto SynQcore.
