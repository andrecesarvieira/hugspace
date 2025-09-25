#!/bin/bash

# Script para testar endpoints de autenticação com MediatR
# SynQcore - Corporate Authentication Testing

API_URL="http://localhost:5005/api/auth"

echo "🧪 SynQcore - Testando MediatR Pipeline Authentication"
echo "========================================="

# 1. Testar Register
echo ""
echo "1️⃣  Testando POST /auth/register"
echo "Payload: RegisterCommand via MediatR"

curl -X POST "$API_URL/register" \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "testuser",
    "email": "test@synqcore.com", 
    "password": "TestPass123",
    "confirmPassword": "TestPass123",
    "phoneNumber": "+55119999999"
  }' \
  -w "\n📊 Status: %{http_code} | Tempo: %{time_total}s\n" \
  -s | jq '.' 2>/dev/null || echo "Response (raw): $(curl -X POST "$API_URL/register" -H "Content-Type: application/json" -d '{"userName":"testuser","email":"test@synqcore.com","password":"TestPass123","confirmPassword":"TestPass123","phoneNumber":"+55119999999"}' -s)"

echo ""
echo "========================================="

# 2. Testar Login
echo ""
echo "2️⃣  Testando POST /auth/login"  
echo "Payload: LoginCommand via MediatR"

curl -X POST "$API_URL/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@synqcore.com",
    "password": "TestPass123"
  }' \
  -w "\n📊 Status: %{http_code} | Tempo: %{time_total}s\n" \
  -s | jq '.' 2>/dev/null || echo "Response (raw): $(curl -X POST "$API_URL/login" -H "Content-Type: application/json" -d '{"email":"test@synqcore.com","password":"TestPass123"}' -s)"

echo ""
echo "========================================="

# 3. Testar validação (senha inválida)
echo ""
echo "3️⃣  Testando Validação FluentValidation"
echo "Payload: Senha muito curta (deve falhar)"

curl -X POST "$API_URL/register" \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "invaliduser",
    "email": "invalid@synqcore.com",
    "password": "123",
    "confirmPassword": "456"
  }' \
  -w "\n📊 Status: %{http_code} | Tempo: %{time_total}s\n" \
  -s | jq '.' 2>/dev/null || echo "Response (raw): $(curl -X POST "$API_URL/register" -H "Content-Type: application/json" -d '{"userName":"invaliduser","email":"invalid@synqcore.com","password":"123","confirmPassword":"456"}' -s)"

echo ""
echo "✅ Testes MediatR Pipeline completos!"
echo "📝 Verificar logs da aplicação para auditoria LoggingBehavior"