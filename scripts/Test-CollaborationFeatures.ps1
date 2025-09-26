# 🧪 SynQcore - Testes Automatizados Fase 3.3 (PowerShell)
# Corporate Collaboration Features - Sistema de Endorsements e Discussion Threads

param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$AdminEmail = "admin@dev.synqcore.com",
    [string]$AdminPassword = "DevAdmin@123!"
)

# Configurações
$ApiBase = "$BaseUrl/api"
$TestsPassed = 0
$TestsFailed = 0

# Função para imprimir resultado do teste
function Write-TestResult {
    param([bool]$Success, [string]$Message)
    
    if ($Success) {
        Write-Host "✅ $Message" -ForegroundColor Green
        $script:TestsPassed++
    } else {
        Write-Host "❌ $Message" -ForegroundColor Red
        $script:TestsFailed++
    }
}

# Função para fazer request HTTP
function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Url,
        [string]$Token = "",
        [hashtable]$Body = $null
    )
    
    try {
        $headers = @{}
        if ($Token) {
            $headers["Authorization"] = "Bearer $Token"
        }
        
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $headers
            ContentType = "application/json"
        }
        
        if ($Body) {
            $params.Body = $Body | ConvertTo-Json -Depth 10
        }
        
        $response = Invoke-RestMethod @params
        return @{ Success = $true; Data = $response; StatusCode = 200 }
    }
    catch {
        $statusCode = 0
        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode
        }
        return @{ Success = $false; Error = $_.Exception.Message; StatusCode = $statusCode }
    }
}

# Função para obter token JWT
function Get-JwtToken {
    param([string]$Email, [string]$Password)
    
    Write-Host "🔑 Obtendo token JWT para $Email..." -ForegroundColor Blue
    
    $loginData = @{
        email = $Email
        password = $Password
    }
    
    $result = Invoke-ApiRequest -Method "POST" -Url "$ApiBase/auth/login" -Body $loginData
    
    if ($result.Success) {
        $token = $result.Data.token ?? $result.Data.accessToken ?? $result.Data.jwtToken
        if ($token) {
            return $token
        }
    }
    
    Write-Host "❌ Falha ao obter token JWT (Status: $($result.StatusCode))" -ForegroundColor Red
    return $null
}

Write-Host "🚀 Iniciando Testes da Fase 3.3 - Corporate Collaboration Features" -ForegroundColor Blue
Write-Host "=" * 50

# Verificar se API está rodando
Write-Host "🏥 Verificando saúde da API..." -ForegroundColor Blue
$healthResult = Invoke-ApiRequest -Method "GET" -Url "$BaseUrl/health"

if ($healthResult.Success) {
    Write-TestResult -Success $true -Message "API está rodando e saudável"
} else {
    Write-TestResult -Success $false -Message "API não está acessível (Status: $($healthResult.StatusCode))"
    Write-Host "❌ Certifique-se de que a API está rodando em $BaseUrl" -ForegroundColor Red
    exit 1
}

# Configurar autenticação
Write-Host "`n🔐 Configurando Autenticação..." -ForegroundColor Blue

$AdminToken = Get-JwtToken -Email $AdminEmail -Password $AdminPassword
$EmployeeToken = "mock_employee_token"  # Fallback para testes básicos

if (-not $AdminToken) {
    Write-Host "⚠️ Usando tokens mock para testes. Configure usuários reais para testes completos." -ForegroundColor Yellow
    $AdminToken = "mock_admin_token"
}

Write-Host "`n🏢 === TESTES DE ENDORSEMENTS CORPORATIVOS ===" -ForegroundColor Blue

# Teste 1: Listar endorsements
Write-Host "`n📋 Testando listagem de endorsements..."
$result = Invoke-ApiRequest -Method "GET" -Url "$ApiBase/endorsements" -Token $EmployeeToken
Write-TestResult -Success $result.Success -Message "GET /endorsements (Status: $($result.StatusCode))"

# Teste 2: Criar endorsement
Write-Host "`n📝 Testando criação de endorsement..."
$endorsementData = @{
    postId = "123e4567-e89b-12d3-a456-426614174000"
    type = "Skills"
    note = "Teste automatizado PowerShell - excelente conhecimento técnico"
    isPublic = $true
    context = "Teste de API PowerShell"
}

$result = Invoke-ApiRequest -Method "POST" -Url "$ApiBase/endorsements" -Token $EmployeeToken -Body $endorsementData
Write-TestResult -Success ($result.StatusCode -eq 201) -Message "POST /endorsements (Status: $($result.StatusCode))"

# Teste 3: Analytics de endorsements
Write-Host "`n📊 Testando analytics de endorsements..."
$result = Invoke-ApiRequest -Method "GET" -Url "$ApiBase/endorsements/analytics/rankings" -Token $AdminToken
Write-TestResult -Success $result.Success -Message "GET /endorsements/analytics/rankings (Status: $($result.StatusCode))"

# Teste 4: Rankings departamentais
Write-Host "`n🏆 Testando rankings departamentais..."
$result = Invoke-ApiRequest -Method "GET" -Url "$ApiBase/endorsements/analytics/department-rankings" -Token $AdminToken
Write-TestResult -Success $result.Success -Message "GET /endorsements/analytics/department-rankings (Status: $($result.StatusCode))"

Write-Host "`n💬 === TESTES DE DISCUSSION THREADS ===" -ForegroundColor Blue

# Teste 5: Criar comment em thread
Write-Host "`n💬 Testando criação de comment..."
$commentData = @{
    postId = "123e4567-e89b-12d3-a456-426614174000"
    content = "Teste automatizado PowerShell - excelente post! @admin.test"
    type = "Discussion"
    visibility = "Department"
    priority = "Normal"
}

$result = Invoke-ApiRequest -Method "POST" -Url "$ApiBase/discussion-threads" -Token $EmployeeToken -Body $commentData
Write-TestResult -Success ($result.StatusCode -eq 201) -Message "POST /discussion-threads (Status: $($result.StatusCode))"

# Teste 6: Listar comments de um post
Write-Host "`n📋 Testando listagem de comments..."
$result = Invoke-ApiRequest -Method "GET" -Url "$ApiBase/discussion-threads/123e4567-e89b-12d3-a456-426614174000" -Token $EmployeeToken
Write-TestResult -Success $result.Success -Message "GET /discussion-threads/{postId} (Status: $($result.StatusCode))"

# Teste 7: Analytics de discussões
Write-Host "`n📈 Testando analytics de discussões..."
$result = Invoke-ApiRequest -Method "GET" -Url "$ApiBase/discussion-threads/analytics/engagement" -Token $AdminToken
Write-TestResult -Success $result.Success -Message "GET /discussion-analytics/engagement (Status: $($result.StatusCode))"

Write-Host "`n🔒 === TESTES DE SEGURANÇA E AUTORIZAÇÃO ===" -ForegroundColor Blue

# Teste 8: Acesso não autorizado
Write-Host "`n🚫 Testando acesso sem token..."
$result = Invoke-ApiRequest -Method "GET" -Url "$ApiBase/endorsements/analytics/rankings"
Write-TestResult -Success ($result.StatusCode -eq 401) -Message "Acesso negado sem token (Status: $($result.StatusCode))"

# Teste 9: Performance de listagem
Write-Host "`n⚡ Testando performance de listagem..."
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$result = Invoke-ApiRequest -Method "GET" -Url "$ApiBase/endorsements?page=1&pageSize=50" -Token $EmployeeToken
$stopwatch.Stop()

$isGoodPerformance = $result.Success -and $stopwatch.ElapsedMilliseconds -lt 1000
Write-TestResult -Success $isGoodPerformance -Message "Performance listagem ($($stopwatch.ElapsedMilliseconds)ms, Status: $($result.StatusCode))"

# Teste 10: Health checks específicos
Write-Host "`n🏥 Testando health checks específicos..."
$result = Invoke-ApiRequest -Method "GET" -Url "$BaseUrl/health/ready"
Write-TestResult -Success $result.Success -Message "Health check /ready (Status: $($result.StatusCode))"

# Teste 11: Validação de dados inválidos
Write-Host "`n❌ Testando validação de dados inválidos..."
$invalidData = @{
    type = "InvalidType"
    note = ""
}

$result = Invoke-ApiRequest -Method "POST" -Url "$ApiBase/endorsements" -Token $EmployeeToken -Body $invalidData
Write-TestResult -Success ($result.StatusCode -eq 400) -Message "Validação de dados inválidos (Status: $($result.StatusCode))"

# Teste 12: Teste com GUID inválido
Write-Host "`n🔢 Testando GUID inválido..."
$result = Invoke-ApiRequest -Method "GET" -Url "$ApiBase/endorsements/invalid-guid" -Token $EmployeeToken
Write-TestResult -Success ($result.StatusCode -eq 400) -Message "Validação GUID inválido (Status: $($result.StatusCode))"

# Resumo final
Write-Host "`n📋 === RESUMO DOS TESTES ===" -ForegroundColor Blue
Write-Host "=" * 50
Write-Host "✅ Testes Passaram: $TestsPassed" -ForegroundColor Green
Write-Host "❌ Testes Falharam: $TestsFailed" -ForegroundColor Red

$totalTests = $TestsPassed + $TestsFailed
if ($totalTests -gt 0) {
    $successRate = [math]::Round(($TestsPassed * 100) / $totalTests, 1)
    Write-Host "📊 Taxa de Sucesso: $successRate%" -ForegroundColor Blue
}

if ($TestsFailed -eq 0) {
    Write-Host "`n🎉 TODOS OS TESTES PASSARAM! Fase 3.3 Corporate Collaboration Features está funcionando! 🎉" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`n⚠️ Alguns testes falharam. Verifique a configuração da API e autenticação." -ForegroundColor Yellow
    Write-Host "💡 Dicas:" -ForegroundColor Blue
    Write-Host "   • Certifique-se de que a API está rodando em $BaseUrl"
    Write-Host "   • Configure usuários de teste com os papéis corretos"
    Write-Host "   • Verifique se o banco de dados tem dados de teste"
    exit 1
}