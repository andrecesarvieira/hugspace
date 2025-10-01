# 🚀 SynQcore - Guia de Desenvolvimento Otimizado

## 📋 Configuração Final (Outubro 2025)

### ✅ **Ambiente Híbrido Otimizado**

Sua configuração atual combina o melhor dos dois mundos:

- **Infraestrutura robusta em containers** (PostgreSQL + Redis + pgAdmin)
- **Desenvolvimento ágil local** (API + Blazor com hot reload)

---

## 🎯 **Scripts de Desenvolvimento**

### **Script Principal (Recomendado)**

```bash
# Aplicação completa - uso diário
./synqcore start

# Configuração inicial automática (primeira vez)
./synqcore setup

# Verificar status do ambiente
./synqcore status
```

### **Scripts Específicos**

```bash
# Infraestrutura apenas
./synqcore docker-up    # Iniciar containers
./synqcore docker-down  # Parar containers

# Aplicação separada
./synqcore api          # Apenas API (porta 5000)
./synqcore blazor       # Apenas Blazor (porta 5226)

# Utilitários
./synqcore clean        # Limpeza completa
./synqcore help         # Ajuda completa
```

---

## 🌐 **URLs de Desenvolvimento**

| Serviço              | URL                                 | Descrição              |
| -------------------- | ----------------------------------- | ---------------------- |
| **API Principal**    | http://localhost:5000               | Backend .NET 9         |
| **Swagger Docs**     | http://localhost:5000/swagger       | Documentação da API    |
| **Health Check**     | http://localhost:5000/health        | Status da aplicação    |
| **Blazor App**       | http://localhost:5226               | Frontend principal     |
| **Feed Corporativo** | http://localhost:5226/feed          | Rede social            |
| **Design System**    | http://localhost:5226/design-system | Componentes UI         |
| **pgAdmin**          | http://localhost:8080               | Administração do banco |

---

## 🐳 **Containers Ativos**

### **Infraestrutura (Sempre Ativa)**

```bash
# PostgreSQL 16 - Banco principal
Container: synqcore-postgres
Porta: 5432
Imagem: docker-postgres (personalizada)
Features: extensões, schemas organizacionais

# Redis 7 - Cache e sessões
Container: synqcore-redis
Porta: 6379
Imagem: docker-redis (personalizada)
Features: configuração otimizada para rede social

# pgAdmin 4 - Interface web
Container: synqcore-pgadmin
Porta: 8080
Credenciais: admin@synqcore.dev / SynQcore@Dev123!
```

---

## 🔐 **Credenciais Padrão**

### **Aplicação**

- **Email**: admin@synqcore.com
- **Senha**: SynQcore@Admin123!
- **Username**: admin

### **Banco de dados**

- **Host**: localhost:5432
- **Database**: synqcore_db
- **User**: postgres
- **Password**: SynQcore@Dev123!

### **pgAdmin**

- **URL**: http://localhost:8080
- **Email**: admin@synqcore.dev
- **Password**: SynQcore@Dev123!

---

## 📦 **Estrutura de Containers**

### **Imagens Personalizadas Criadas:**

```dockerfile
# docker-postgres
- Base: postgres:16-alpine
- Scripts: extensões pg_trgm, unaccent, uuid-ossp, pgcrypto
- Schemas: users, social, content, notifications

# docker-redis
- Base: redis:7-alpine
- Config: 512MB maxmemory, LRU policy, persistência
- Otimizado: cache de feeds, sessões, dados real-time
```

---

## 🔄 **Fluxo de Desenvolvimento Diário**

### **1. Manhã - Início do Trabalho**

```bash
# Verificar infraestrutura
docker ps

# Se containers não estiverem rodando
./synqcore docker-up

# Iniciar desenvolvimento
./synqcore start
```

### **2. Durante o Dia**

```bash
# Hot reload automático para mudanças em:
# - Controllers, Services, Models (API)
# - Pages, Components (Blazor)

# Para restart rápido se necessário
Ctrl+C
./synqcore start
```

### **3. Administração do Banco**

```bash
# Via pgAdmin (interface web)
http://localhost:8080

# Via linha de comando
docker exec -it synqcore-postgres psql -U postgres -d synqcore_db
```

### **4. Testes e Debug**

```bash
# Health checks
curl http://localhost:5000/health

# API endpoints
curl http://localhost:5000/api/auth/test

# Logs da aplicação
# Disponíveis no terminal onde rodou ./synqcore start
```

---

## 🛠️ **Troubleshooting**

### **Containers com Problemas**

```bash
# Restart completo da infraestrutura
./synqcore docker-down
./synqcore docker-up

# Logs específicos
docker logs synqcore-postgres
docker logs synqcore-redis
docker logs synqcore-pgadmin
```

### **Problemas de Compilação**

```bash
# Limpeza completa
./synqcore clean

# Build manual
dotnet clean
dotnet build
```

### **Problemas de Banco**

```bash
# Recriar migrations (se necessário)
rm -rf src/SynQcore.Infrastructure/Migrations/*
dotnet ef migrations add NewMigration --project src/SynQcore.Infrastructure --startup-project src/SynQcore.Api
dotnet ef database update --project src/SynQcore.Infrastructure --startup-project src/SynQcore.Api
```

---

## 📈 **Vantagens da Configuração Atual**

### ✅ **Performance**

- Hot reload para desenvolvimento rápido
- Containers isolados e otimizados
- Build incremental inteligente

### ✅ **Facilidade de Uso**

- Script unificado para tudo
- URLs padronizadas e documentadas
- Credenciais consistentes

### ✅ **Robustez**

- Dados persistentes em volumes
- Health checks automáticos
- Configurações otimizadas

### ✅ **Flexibilidade**

- Desenvolvimento local ágil
- Infraestrutura containerizada
- Fácil alternância entre modos

---

## 🎯 **Próximos Passos Sugeridos**

1. **Testes Automatizados**: Expandir cobertura de testes
2. **CI/CD Pipeline**: Automação de deploy
3. **Monitoramento**: Métricas e observabilidade
4. **Backup**: Estratégia de backup automatizado
5. **Performance**: Profiling e otimizações

---

**✨ Ambiente otimizado e pronto para desenvolvimento produtivo!**
