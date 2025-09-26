#!/bin/bash

# 🧪 SynQcore - Testes Automatizados Fase 3.3
# Corporate Collaboration Features - Sistema de Endorsements e Discussion Threads

set -e  # Exit on error

# Configurações
BASE_URL="http://localhost:5000"
API_BASE="$BASE_URL/api"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Contadores
TESTS_PASSED=0
TESTS_FAILED=0

# Função para imprimir resultado do teste
print_result() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}✅ $2${NC}"
        ((TESTS_PASSED++))
    else
        echo -e "${RED}❌ $2${NC}"
        ((TESTS_FAILED++))
    fi
}

# Função para fazer request HTTP
make_request() {
    local method=$1
    local url=$2
    local token=$3
    local data=$4
    
    if [ -n "$data" ]; then
        curl -s -w "%{http_code}" -o /tmp/response.json \
            -X "$method" \
            -H "Authorization: Bearer $token" \
            -H "Content-Type: application/json" \
            -d "$data" \
            "$url"
    else
        curl -s -w "%{http_code}" -o /tmp/response.json \
            -X "$method" \
            -H "Authorization: Bearer $token" \
            "$url"
    fi
}

# Função para obter token JWT
get_jwt_token() {
    local email=$1
    local password=$2
    
    echo -e "${BLUE}🔑 Obtendo token JWT para $email...${NC}"
    
    local login_data="{
        \"email\": \"$email\",
        \"password\": \"$password\"
    }"
    
    local status_code=$(make_request "POST" "$API_BASE/auth/login" "" "$login_data")
    
    if [ "$status_code" -eq 200 ]; then
        local token=$(cat /tmp/response.json | jq -r '.token // .accessToken // .jwtToken')
        if [ "$token" != "null" ] && [ -n "$token" ]; then
            echo "$token"
            return 0
        fi
    fi
    
    echo -e "${RED}❌ Falha ao obter token JWT (Status: $status_code)${NC}"
    return 1
}

echo -e "${BLUE}🚀 Iniciando Testes da Fase 3.3 - Corporate Collaboration Features${NC}"
echo "=================================================="

# Verificar se API está rodando
echo -e "${BLUE}🏥 Verificando saúde da API...${NC}"
status_code=$(curl -s -w "%{http_code}" -o /tmp/response.json "$BASE_URL/health" || echo "000")

if [ "$status_code" -eq 200 ]; then
    print_result 0 "API está rodando e saudável"
else
    print_result 1 "API não está acessível (Status: $status_code)"
    echo -e "${RED}❌ Certifique-se de que a API está rodando em $BASE_URL${NC}"
    exit 1
fi

# Tentar obter tokens (podem não funcionar se usuários não existirem)
echo -e "\n${BLUE}🔐 Configurando Autenticação...${NC}"

# Tokens padrão (você deve substituir pelos seus)
ADMIN_TOKEN=""
MANAGER_TOKEN=""  
EMPLOYEE_TOKEN=""

# Tentar login automático se usuários existirem
if ADMIN_TOKEN=$(get_jwt_token "admin@dev.synqcore.com" "DevAdmin@123!" 2>/dev/null); then
    echo -e "${GREEN}✅ Token Admin obtido${NC}"
else
    echo -e "${YELLOW}⚠️ Não foi possível obter token Admin automaticamente${NC}"
fi

# Se não conseguiu tokens, usar tokens mock para testar endpoints públicos
if [ -z "$ADMIN_TOKEN" ]; then
    echo -e "${YELLOW}⚠️ Usando tokens mock para testes. Configure usuários reais para testes completos.${NC}"
    ADMIN_TOKEN="mock_admin_token"
    MANAGER_TOKEN="mock_manager_token"
    EMPLOYEE_TOKEN="mock_employee_token"
fi

echo -e "\n${BLUE}🏢 === TESTES DE ENDORSEMENTS CORPORATIVOS ===${NC}"

# Teste 1: Listar endorsements (público)
echo -e "\n📋 Testando listagem de endorsements..."
status_code=$(make_request "GET" "$API_BASE/endorsements" "$EMPLOYEE_TOKEN")
print_result $([ "$status_code" -eq 200 ] && echo 0 || echo 1) "GET /endorsements (Status: $status_code)"

# Teste 2: Criar endorsement
echo -e "\n📝 Testando criação de endorsement..."
endorsement_data="{
    \"postId\": \"123e4567-e89b-12d3-a456-426614174000\",
    \"type\": \"Skills\",
    \"note\": \"Teste automatizado - excelente conhecimento técnico\",
    \"isPublic\": true,
    \"context\": \"Teste de API\"
}"

status_code=$(make_request "POST" "$API_BASE/endorsements" "$EMPLOYEE_TOKEN" "$endorsement_data")
print_result $([ "$status_code" -eq 201 ] && echo 0 || echo 1) "POST /endorsements (Status: $status_code)"

# Teste 3: Analytics de endorsements
echo -e "\n📊 Testando analytics de endorsements..."
status_code=$(make_request "GET" "$API_BASE/endorsements/analytics/rankings" "$MANAGER_TOKEN")
print_result $([ "$status_code" -eq 200 ] && echo 0 || echo 1) "GET /endorsements/analytics/rankings (Status: $status_code)"

# Teste 4: Rankings departamentais
echo -e "\n🏆 Testando rankings departamentais..."
status_code=$(make_request "GET" "$API_BASE/endorsements/analytics/department-rankings" "$MANAGER_TOKEN")
print_result $([ "$status_code" -eq 200 ] && echo 0 || echo 1) "GET /endorsements/analytics/department-rankings (Status: $status_code)"

echo -e "\n${BLUE}💬 === TESTES DE DISCUSSION THREADS ===${NC}"

# Teste 5: Criar comment em thread
echo -e "\n💬 Testando criação de comment..."
comment_data="{
    \"postId\": \"123e4567-e89b-12d3-a456-426614174000\",
    \"content\": \"Teste automatizado - excelente post! @admin.test\",
    \"type\": \"Discussion\",
    \"visibility\": \"Department\",
    \"priority\": \"Normal\"
}"

status_code=$(make_request "POST" "$API_BASE/discussion-threads" "$EMPLOYEE_TOKEN" "$comment_data")
print_result $([ "$status_code" -eq 201 ] && echo 0 || echo 1) "POST /discussion-threads (Status: $status_code)"

# Teste 6: Listar comments de um post
echo -e "\n📋 Testando listagem de comments..."
status_code=$(make_request "GET" "$API_BASE/discussion-threads/123e4567-e89b-12d3-a456-426614174000" "$EMPLOYEE_TOKEN")
print_result $([ "$status_code" -eq 200 ] && echo 0 || echo 1) "GET /discussion-threads/{postId} (Status: $status_code)"

# Teste 7: Analytics de discussões
echo -e "\n📈 Testando analytics de discussões..."
status_code=$(make_request "GET" "$API_BASE/discussion-threads/analytics/engagement" "$MANAGER_TOKEN")
print_result $([ "$status_code" -eq 200 ] && echo 0 || echo 1) "GET /discussion-analytics/engagement (Status: $status_code)"

# Teste 8: Métricas de moderação
echo -e "\n🛡️ Testando métricas de moderação..."
status_code=$(make_request "GET" "$API_BASE/discussion-threads/analytics/moderation-metrics" "$ADMIN_TOKEN")
print_result $([ "$status_code" -eq 200 ] && echo 0 || echo 1) "GET /discussion-analytics/moderation-metrics (Status: $status_code)"

echo -e "\n${BLUE}🔒 === TESTES DE SEGURANÇA E AUTORIZAÇÃO ===${NC}"

# Teste 9: Acesso não autorizado
echo -e "\n🚫 Testando acesso sem token..."
status_code=$(curl -s -w "%{http_code}" -o /tmp/response.json "$API_BASE/endorsements/analytics/rankings")
print_result $([ "$status_code" -eq 401 ] && echo 0 || echo 1) "Acesso negado sem token (Status: $status_code)"

# Teste 10: Rate limiting (simulado)
echo -e "\n⏱️ Testando rate limiting..."
# Fazer algumas requests rápidas para testar
for i in {1..5}; do
    status_code=$(make_request "GET" "$API_BASE/endorsements" "$EMPLOYEE_TOKEN")
    if [ "$status_code" -eq 429 ]; then
        print_result 0 "Rate limiting funcionando (Status: 429)"
        break
    fi
done

echo -e "\n${BLUE}📊 === TESTES DE PERFORMANCE ===${NC}"

# Teste 11: Performance de listagem
echo -e "\n⚡ Testando performance de listagem..."
start_time=$(date +%s%N)
status_code=$(make_request "GET" "$API_BASE/endorsements?page=1&pageSize=50" "$EMPLOYEE_TOKEN")
end_time=$(date +%s%N)
duration=$(( (end_time - start_time) / 1000000 )) # Convert to milliseconds

if [ "$status_code" -eq 200 ] && [ "$duration" -lt 1000 ]; then
    print_result 0 "Performance listagem OK (${duration}ms)"
else
    print_result 1 "Performance listagem lenta (${duration}ms, Status: $status_code)"
fi

# Teste 12: Health checks específicos
echo -e "\n🏥 Testando health checks específicos..."
status_code=$(curl -s -w "%{http_code}" -o /tmp/response.json "$BASE_URL/health/ready")
print_result $([ "$status_code" -eq 200 ] && echo 0 || echo 1) "Health check /ready (Status: $status_code)"

echo -e "\n${BLUE}🧪 === TESTES DE DADOS E VALIDAÇÃO ===${NC}"

# Teste 13: Validação de dados inválidos
echo -e "\n❌ Testando validação de dados inválidos..."
invalid_data="{
    \"type\": \"InvalidType\",
    \"note\": \"\"
}"

status_code=$(make_request "POST" "$API_BASE/endorsements" "$EMPLOYEE_TOKEN" "$invalid_data")
print_result $([ "$status_code" -eq 400 ] && echo 0 || echo 1) "Validação de dados inválidos (Status: $status_code)"

# Teste 14: Teste com GUID inválido
echo -e "\n🔢 Testando GUID inválido..."
status_code=$(make_request "GET" "$API_BASE/endorsements/invalid-guid" "$EMPLOYEE_TOKEN")
print_result $([ "$status_code" -eq 400 ] && echo 0 || echo 1) "Validação GUID inválido (Status: $status_code)"

# Resumo final
echo -e "\n${BLUE}📋 === RESUMO DOS TESTES ===${NC}"
echo "=================================================="
echo -e "${GREEN}✅ Testes Passaram: $TESTS_PASSED${NC}"
echo -e "${RED}❌ Testes Falharam: $TESTS_FAILED${NC}"

total_tests=$((TESTS_PASSED + TESTS_FAILED))
if [ $total_tests -gt 0 ]; then
    success_rate=$((TESTS_PASSED * 100 / total_tests))
    echo -e "${BLUE}📊 Taxa de Sucesso: $success_rate%${NC}"
fi

if [ $TESTS_FAILED -eq 0 ]; then
    echo -e "\n${GREEN}🎉 TODOS OS TESTES PASSARAM! Fase 3.3 Corporate Collaboration Features está funcionando! 🎉${NC}"
    exit 0
else
    echo -e "\n${YELLOW}⚠️ Alguns testes falharam. Verifique a configuração da API e autenticação.${NC}"
    echo -e "${BLUE}💡 Dicas:${NC}"
    echo "   • Certifique-se de que a API está rodando em $BASE_URL"
    echo "   • Configure usuários de teste com os papéis corretos"
    echo "   • Verifique se o banco de dados tem dados de teste"
    exit 1
fi