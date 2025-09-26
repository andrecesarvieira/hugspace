# 🧪 Testes Completos - Fase 3.3 Corporate Collaboration Features

## 📋 Visão Geral dos Testes

Esta fase implementou **Corporate Collaboration Features** incluindo Sistema de Endorsements e Discussion Threads. Os testes devem validar funcionalidades corporativas, performance, segurança e compliance.

---

## 🏢 1. TESTES DE ENDORSEMENTS CORPORATIVOS

### 1.1 Testes de API - EndorsementsController

#### **Cenário: Criar Endorsement Corporativo**
```bash
# POST /api/endorsements
curl -X POST "http://localhost:5000/api/endorsements" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "postId": "123e4567-e89b-12d3-a456-426614174000",
    "type": "Skills",
    "note": "Excelente conhecimento técnico demonstrado",
    "isPublic": true,
    "context": "Projeto de migração cloud"
  }'

# Validações Esperadas:
# - Status: 201 Created
# - Response inclui: ID, Type, EndorsedAt, EndorserName
# - Audit log registrado com LoggerMessage
```

#### **Cenário: Analytics de Endorsements por Departamento**  
```bash
# GET /api/endorsements/analytics/department-rankings
curl -X GET "http://localhost:5000/api/endorsements/analytics/department-rankings?departmentId=456&startDate=2025-09-01&topCount=10" \
  -H "Authorization: Bearer MANAGER_JWT_TOKEN"

# Validações Esperadas:
# - Rankings por engagement score
# - Breakdown por tipo de endorsement
# - Métricas de crescimento mensal
```

### 1.2 Testes de Negócio - Regras Corporativas

#### **Teste: Endorsement Duplicado**
```bash
# Tentar endossar o mesmo post/comment duas vezes
curl -X POST "http://localhost:5000/api/endorsements" \
  -H "Authorization: Bearer EMPLOYEE_JWT_TOKEN" \
  -d '{"postId": "SAME_POST_ID", "type": "Skills"}'

# Expectativa: 409 Conflict - "Já existe endorsement deste usuário"
```

#### **Teste: Auto-Endorsement Bloqueado**
```bash
# Tentar endossar próprio conteúdo
curl -X POST "http://localhost:5000/api/endorsements" \
  -H "Authorization: Bearer AUTHOR_TOKEN" \
  -d '{"postId": "OWN_POST_ID", "type": "Helpful"}'

# Expectativa: 400 BadRequest - "Não é possível endossar próprio conteúdo"
```

### 1.3 Testes de Performance

#### **Load Test: Múltiplos Endorsements**
```bash
# Script para 100 endorsements simultâneos
for i in {1..100}; do
  curl -X POST "http://localhost:5000/api/endorsements" \
    -H "Authorization: Bearer TOKEN_$i" \
    -d '{"postId":"test-post","type":"Skills"}' &
done
wait

# Validações:
# - Rate limiting funcionando (100 req/min por Employee)
# - Database sem deadlocks
# - Response time < 200ms
```

---

## 💬 2. TESTES DE DISCUSSION THREADS

### 2.1 Testes de Threading Corporativo

#### **Cenário: Criar Thread de Discussão**
```bash
# POST /api/discussion-threads
curl -X POST "http://localhost:5000/api/discussion-threads" \
  -H "Authorization: Bearer EMPLOYEE_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "postId": "123e4567-e89b-12d3-a456-426614174000",
    "content": "Excelente iniciativa! @joao.silva que tal implementarmos isso no nosso departamento?",
    "type": "Discussion",
    "visibility": "Department",
    "priority": "Normal"
  }'

# Validações:
# - ThreadPath gerado automaticamente (001)
# - ThreadLevel = 1 (root comment)
# - Mention @joao.silva processada e notificação enviada
# - Status inicial: Pending (se departamento requer moderação)
```

#### **Cenário: Reply Hierárquico**
```bash
# POST /api/discussion-threads/reply
curl -X POST "http://localhost:5000/api/discussion-threads/reply" \
  -H "Authorization: Bearer MANAGER_JWT_TOKEN" \
  -d '{
    "parentCommentId": "COMMENT_ID_FROM_ABOVE",
    "content": "Ótima ideia! Vamos agendar uma reunião para discutir.",
    "type": "Answer"
  }'

# Validações:
# - ThreadPath = 001.001 (subthread)
# - ThreadLevel = 2
# - ParentCommentId preenchido
# - Hierarquia mantida no response
```

### 2.2 Testes de Moderação Corporativa

#### **Cenário: Moderação por HR**
```bash
# PUT /api/discussion-threads/moderate
curl -X PUT "http://localhost:5000/api/discussion-threads/{commentId}/moderate" \
  -H "Authorization: Bearer HR_JWT_TOKEN" \
  -d '{
    "status": "Approved",
    "moderatorNotes": "Conteúdo apropriado para comunicação corporativa"
  }'

# Validações:
# - Status atualizado para Approved
# - ModeratedAt timestamp
# - ModeratedBy = HR user ID
# - Audit trail registrado
```

#### **Teste: Escalation para Admin**
```bash
# PUT /api/discussion-threads/{commentId}/moderate
curl -X PUT "http://localhost:5000/api/discussion-threads/{commentId}/moderate" \
  -H "Authorization: Bearer HR_JWT_TOKEN" \
  -d '{
    "status": "Escalated",
    "moderatorNotes": "Conteúdo requer revisão administrativa"
  }'

# Expectativa:
# - Notificação automática para Admins
# - Status = Escalated
# - Priority aumentada automaticamente
```

### 2.3 Testes de Analytics de Discussões

#### **Cenário: Métricas de Engagement**
```bash
# GET /api/discussion-threads/analytics/engagement
curl -X GET "http://localhost:5000/api/discussion-threads/analytics/engagement?postId=123&includeMetrics=true" \
  -H "Authorization: Bearer MANAGER_JWT_TOKEN"

# Response Esperado:
{
  "totalComments": 45,
  "totalReplies": 28,
  "avgResponseTime": "2.5h",
  "topContributors": [...],
  "engagementScore": 8.7,
  "threadDepth": 4
}
```

---

## 🔒 3. TESTES DE SEGURANÇA E AUTORIZAÇÃO

### 3.1 Role-Based Access Control

#### **Teste: Employee não pode moderar**
```bash
curl -X PUT "http://localhost:5000/api/discussion-threads/{id}/moderate" \
  -H "Authorization: Bearer EMPLOYEE_JWT_TOKEN"

# Expectativa: 403 Forbidden
```

#### **Teste: Manager pode ver analytics departamentais**
```bash
curl -X GET "http://localhost:5000/api/endorsements/analytics/department-rankings" \
  -H "Authorization: Bearer MANAGER_JWT_TOKEN"

# Expectativa: 200 OK com dados do departamento do manager
```

### 3.2 Testes de Rate Limiting Corporativo

#### **Teste: Rate Limit por Role**
```bash
# Employee (100 req/min)
for i in {1..105}; do
  curl -X GET "http://localhost:5000/api/endorsements" \
    -H "Authorization: Bearer EMPLOYEE_JWT_TOKEN"
done

# Expectativa: 
# - Requests 1-100: 200 OK
# - Request 101+: 429 Too Many Requests
```

---

## 📊 4. TESTES DE PERFORMANCE E DADOS

### 4.1 Testes de Volume

#### **Cenário: 10,000 Comments em Thread**
```sql
-- Script SQL para popular database
INSERT INTO Comments (Id, PostId, AuthorId, Content, ThreadPath, ThreadLevel, CreatedAt)
SELECT 
  gen_random_uuid(),
  'test-post-id',
  'test-author-id',
  'Comment content ' || generate_series,
  lpad(generate_series::text, 3, '0'),
  1,
  NOW()
FROM generate_series(1, 10000);
```

```bash
# Test endpoint performance
curl -X GET "http://localhost:5000/api/discussion-threads/test-post-id?pageSize=50" \
  -H "Authorization: Bearer TOKEN"

# Validações:
# - Response time < 500ms
# - Paginação funcionando
# - Hierarquia preservada
```

### 4.2 Testes de Concorrência

#### **Cenário: Múltiplas Mentions Simultâneas**
```bash
# 20 comments com mention simultâneas
for i in {1..20}; do
  curl -X POST "http://localhost:5000/api/discussion-threads" \
    -H "Authorization: Bearer TOKEN_$i" \
    -d '{
      "content": "Mencionar @admin.test em comment '$i'",
      "type": "Question"
    }' &
done
wait

# Validações:
# - Todas as mentions processadas
# - Sem duplicação de notificações
# - Database consistency mantida
```

---

## 🧪 5. TESTES AUTOMATIZADOS RECOMENDADOS

### 5.1 Unit Tests (xUnit + Moq)

```csharp
// Exemplo de teste unitário para EndorsementHandler
[Fact]
public async Task CreateEndorsement_ValidRequest_ShouldCreateSuccessfully()
{
    // Arrange
    var command = new CreateEndorsementCommand 
    { 
        PostId = Guid.NewGuid(),
        Type = EndorsementType.Skills,
        Note = "Test endorsement"
    };
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
    _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
}
```

### 5.2 Integration Tests

```csharp
[Fact]
public async Task POST_Endorsements_ShouldReturn201WithValidData()
{
    // Arrange
    var client = _factory.CreateClient();
    var token = await GetJwtTokenAsync("employee@test.com");
    client.DefaultRequestHeaders.Authorization = new("Bearer", token);
    
    var request = new CreateEndorsementDto
    {
        PostId = _testPostId,
        Type = EndorsementType.Skills,
        IsPublic = true
    };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/endorsements", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

---

## 📈 6. MONITORAMENTO E MÉTRICAS

### 6.1 Health Checks Específicos

```bash
# Health check para Discussion Threads
curl -X GET "http://localhost:5000/health/discussion-threads"

# Validações:
# - Database connectivity
# - Recent comment creation time < 1min
# - Moderation queue size < 100
```

### 6.2 Performance Metrics

```bash
# Métricas de performance via endpoint
curl -X GET "http://localhost:5000/api/discussion-threads/analytics/performance" \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN"

# Response esperado:
{
  "avgCommentCreationTime": "45ms",
  "avgModerationTime": "2.3h", 
  "threadDepthDistribution": {...},
  "dailyCommentVolume": 1847
}
```

---

## 🎯 7. SCRIPTS DE TESTE RÁPIDO

### Script Bash para Testes Principais

```bash
#!/bin/bash
# quick-test-collaboration.sh

BASE_URL="http://localhost:5000"
ADMIN_TOKEN="YOUR_ADMIN_TOKEN"
EMPLOYEE_TOKEN="YOUR_EMPLOYEE_TOKEN"

echo "🧪 Testando Endorsements..."
curl -X POST "$BASE_URL/api/endorsements" \
  -H "Authorization: Bearer $EMPLOYEE_TOKEN" \
  -d '{"postId":"test-post","type":"Skills"}' \
  && echo "✅ Endorsement criado" \
  || echo "❌ Falha no endorsement"

echo "🧪 Testando Discussion Thread..."
curl -X POST "$BASE_URL/api/discussion-threads" \
  -H "Authorization: Bearer $EMPLOYEE_TOKEN" \
  -d '{"postId":"test-post","content":"Test comment","type":"Discussion"}' \
  && echo "✅ Comment criado" \
  || echo "❌ Falha no comment"

echo "🧪 Testando Analytics..."
curl -X GET "$BASE_URL/api/endorsements/analytics/rankings" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  && echo "✅ Analytics funcionando" \
  || echo "❌ Falha no analytics"
```

### PowerShell para Windows

```powershell
# quick-test-collaboration.ps1
$BaseUrl = "http://localhost:5000"
$Token = "YOUR_JWT_TOKEN"

$Headers = @{ "Authorization" = "Bearer $Token" }

Write-Host "🧪 Testando Endorsements..." -ForegroundColor Blue
try {
    $Body = @{
        postId = "test-post-id"
        type = "Skills"
        note = "PowerShell test endorsement"
    } | ConvertTo-Json

    Invoke-RestMethod -Uri "$BaseUrl/api/endorsements" -Method POST -Headers $Headers -Body $Body -ContentType "application/json"
    Write-Host "✅ Endorsement criado com sucesso" -ForegroundColor Green
}
catch {
    Write-Host "❌ Falha: $($_.Exception.Message)" -ForegroundColor Red
}
```

---

## 🎯 8. CHECKLIST DE VALIDAÇÃO

### ✅ Funcionalidades Core
- [ ] **Endorsements CRUD** - Criar, listar, atualizar, deletar
- [ ] **8 Tipos de Endorsements** - Skills, Leadership, Communication, etc.
- [ ] **Analytics Dashboard** - Rankings, insights, métricas departamentais  
- [ ] **Discussion Threads** - Comentários hierárquicos com threading
- [ ] **Corporate Moderation** - 6 estados de moderação funccionais
- [ ] **Mention System** - @employee.name com notificações
- [ ] **Visibility Controls** - Public, Department, Team, Private

### ✅ Performance & Quality  
- [ ] **Zero Warnings** - Build limpo conforme premissas
- [ ] **LoggerMessage Performance** - Audit trails otimizados
- [ ] **Rate Limiting** - Por role funcionando (Employee: 100/min, etc.)
- [ ] **Response Times** - < 200ms para endpoints básicos
- [ ] **Database Performance** - Queries otimizadas com índices

### ✅ Segurança & Compliance
- [ ] **JWT Authorization** - Role-based access control
- [ ] **Input Validation** - FluentValidation em todos os commands
- [ ] **Audit Logging** - Todas as ações críticas registradas
- [ ] **CORS & Headers** - Configuração segura
- [ ] **SQL Injection Prevention** - EF Core parameterized queries

### ✅ Integration & API
- [ ] **Swagger Documentation** - Todos os endpoints documentados
- [ ] **Error Handling** - Responses consistentes e informativos
- [ ] **Health Checks** - Status da aplicação monitorável
- [ ] **Database Migration** - PostgreSQL schema atualizado
- [ ] **Docker Compatibility** - Aplicação roda em containers

---

## 📞 Próximos Passos

1. **Execute os testes básicos** usando os scripts acima
2. **Valide métricas de performance** em ambiente local
3. **Teste cenários de volume** com dados de teste
4. **Verifique logs de auditoria** para compliance
5. **Prepare para Fase 3.4** - Corporate Feed e Discovery

---

*Documento de testes criado em: 26 de Setembro de 2025*  
*Para dúvidas técnicas: andre@synqcore.dev*