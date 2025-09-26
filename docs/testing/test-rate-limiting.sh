#!/bin/bash

# Script completo para testar Rate Limiting - SynQcore
# Testa limites IP, Client ID e recovery

API_URL="http://localhost:5000"
HEALTH_ENDPOINT="$API_URL/health"
TEST_ENDPOINT="$API_URL/api/auth/login"  # Endpoint real para teste de rate limit
AUTH_ENDPOINT="$API_URL/api/auth/register"

echo "🚦 SynQcore - Teste Completo Rate Limiting"
echo "========================================="

# Função para extrair headers de rate limit
extract_rate_headers() {
    local response="$1"
    local headers=$(echo "$response" | grep -i "x-rate-limit")
    if [ -n "$headers" ]; then
        echo "$headers"
    else
        echo "❌ No rate limit headers found"
    fi
}

# Função para fazer request POST e capturar headers
make_request() {
    local url="$1"
    local client_id="$2"
    local method="${3:-POST}"
    local headers=""
    
    if [ -n "$client_id" ]; then
        headers="-H 'X-ClientId: $client_id'"
    fi
    
    if [ "$method" = "GET" ]; then
        curl -s -I $headers "$url" 2>/dev/null
    else
        curl -s -I -X POST $headers -H "Content-Type: application/json" -d '{}' "$url" 2>/dev/null
    fi
}

echo ""
echo "📊 1. TESTE IP RATE LIMITING (Limite: 100/min)"
echo "----------------------------------------"
echo "Fazendo 105 requests rápidas no $TEST_ENDPOINT..."

rate_limit_hit=false
for i in {1..105}; do
    response=$(make_request "$TEST_ENDPOINT" "" "POST")
    status_code=$(echo "$response" | head -n1 | cut -d' ' -f2)
    
    if [ "$status_code" = "429" ]; then
        echo "🛑 Rate limit atingido na request #$i (Status: 429)"
        echo "Headers de rate limit:"
        extract_rate_headers "$response"
        rate_limit_hit=true
        break
    elif [ $((i % 20)) -eq 0 ]; then
        remaining=$(echo "$response" | grep -i "x-rate-limit-remaining" | cut -d' ' -f2 | tr -d '\r')
        limit=$(echo "$response" | grep -i "x-rate-limit-limit" | cut -d' ' -f2 | tr -d '\r')
        echo "✓ Request #$i - Status: $status_code | Limit: ${limit:-'N/A'} | Remaining: ${remaining:-'N/A'}"
    fi
done

if [ "$rate_limit_hit" = false ]; then
    echo "⚠️  Rate limit não foi atingido em 105 requests"
    echo "💡 Isso pode indicar que o limite é maior ou há configuração específica"
fi

echo ""
echo "📱 2. TESTE CLIENT RATE LIMITING"
echo "--------------------------------"

# Testar diferentes client IDs com limites diferentes
declare -A clients
clients["employee-app"]="100"
clients["manager-app"]="300" 
clients["hr-app"]="500"
clients["admin-app"]="1000"

for client in "${!clients[@]}"; do
    limit=${clients[$client]}
    echo ""
    echo "🔍 Testando Client ID: $client (Limite: $limit/min)"
    
    # Fazer 10 requests para cada client
    client_hit=false
    for i in {1..15}; do
        response=$(make_request "$TEST_ENDPOINT" "$client" "POST")
        status_code=$(echo "$response" | head -n1 | cut -d' ' -f2)
        remaining=$(echo "$response" | grep -i "x-rate-limit-remaining" | cut -d' ' -f2 | tr -d '\r')
        
        if [ "$status_code" = "429" ]; then
            echo "🛑 Client $client: Rate limit atingido na request #$i"
            client_hit=true
            break
        fi
    done
    
    if [ "$client_hit" = false ]; then
        echo "✅ Client $client: 15 requests OK (dentro do limite)"
    fi
    
    # Mostrar headers finais
    final_response=$(make_request "$TEST_ENDPOINT" "$client" "POST")
    echo "📋 Headers finais para $client:"
    extract_rate_headers "$final_response" | head -3
done

echo ""
echo "🔄 3. TESTE DE ENDPOINTS DIFERENTES"
echo "-----------------------------------"

# Testar endpoint com rate limit  
echo "🎯 Testando endpoint COM rate limit: /api/test"
response=$(curl -s -I "$API_URL/api/test" 2>/dev/null)
status_code=$(echo "$response" | head -n1 | cut -d' ' -f2)
echo "   Status: $status_code"
extract_rate_headers "$response" | head -2 | sed 's/^/   /'

echo ""
echo "🎯 Testando endpoint SEM rate limit (bypass): /health"
response=$(curl -s -I "$HEALTH_ENDPOINT" 2>/dev/null)
status_code=$(echo "$response" | head -n1 | cut -d' ' -f2)
echo "   Status: $status_code"
# Teste preciso para bypass - deve estar vazio
rate_headers=$(echo "$response" | grep -i "x-rate-limit")
if [ -z "$rate_headers" ]; then
    echo "   ✅ Bypass funcionando PERFEITAMENTE - SEM headers"
else
    echo "   ❌ PROBLEMA: Headers encontrados no bypass:"
    echo "$rate_headers" | sed 's/^/   /'
fi

echo ""
echo "⏰ 4. TESTE DE RECOVERY (aguardar reset)"
echo "----------------------------------------"

# Pegar tempo de reset atual
reset_response=$(make_request "$TEST_ENDPOINT" "" "POST")
reset_time=$(echo "$reset_response" | grep -i "x-rate-limit-reset" | cut -d' ' -f2 | tr -d '\r')

if [ -n "$reset_time" ]; then
    echo "🕐 Reset programado para: $reset_time"
    echo "⏳ Aguardando 70 segundos para testar recovery..."
    
    for i in {1..14}; do
        echo -n "."
        sleep 5
    done
    echo ""
    
    echo "🔄 Testando após período de reset..."
    recovery_response=$(make_request "$TEST_ENDPOINT" "" "POST")
    recovery_status=$(echo "$recovery_response" | head -n1 | cut -d' ' -f2)
    
    if [ "$recovery_status" != "429" ]; then
        echo "✅ Recovery funcionando - Status: $recovery_status (não bloqueado)"
        extract_rate_headers "$recovery_response"
    else
        echo "❌ Recovery falhou - Ainda bloqueado: $recovery_status"
    fi
else
    echo "⚠️  Não foi possível determinar tempo de reset"
fi

echo ""
echo "🧪 5. TESTE DE BYPASS (Swagger)"
echo "-------------------------------"

swagger_endpoints=("$API_URL/" "$API_URL/swagger/v1/swagger.json")
for swagger_url in "${swagger_endpoints[@]}"; do
    endpoint_name=$(echo "$swagger_url" | sed "s|$API_URL/||" | sed "s|^$|root|")
    echo "🔍 Testando bypass: $endpoint_name"
    
    # Fazer várias requests para testar se está no bypass
    bypass_working=true
    for i in {1..5}; do
        response=$(make_request "$swagger_url" "" "GET")
        status_code=$(echo "$response" | head -n1 | cut -d' ' -f2)
        
        if [ "$status_code" = "429" ]; then
            echo "❌ Endpoint NÃO está no bypass - Status: 429 na request $i"
            bypass_working=false
            break
        fi
    done
    
    if [ "$bypass_working" = true ]; then
        echo "✅ Bypass funcionando - 5 requests consecutivas OK"
        # Teste preciso para bypass - deve estar vazio
        rate_headers=$(echo "$response" | grep -i "x-rate-limit")
        if [ -z "$rate_headers" ]; then
            echo "   ✅ Sem headers de rate limit (bypass PERFEITO)"
        else
            echo "   ❌ PROBLEMA: Headers presentes no bypass:"
            echo "$rate_headers" | sed 's/^/   /'
        fi
    fi
done

echo ""
echo "📋 6. RESUMO CONFIGURAÇÃO ATUAL"
echo "-------------------------------"

echo "🔧 Rate Limiting configurado:"
echo "   • IP Geral: 100/min, 1000/hora"  
echo "   • Employee App: 100/min, 1000/hora"
echo "   • Manager App: 300/min, 5000/hora"
echo "   • HR App: 500/min, 10000/hora"
echo "   • Admin App: 1000/min, 50000/hora"
echo "   • Whitelist: 127.0.0.1, ::1"
echo "   • Bypass: /health, /swagger"

echo ""
echo "✅ Teste Rate Limiting concluído!"
echo "🔍 Verifique logs da aplicação para detalhes dos middlewares"