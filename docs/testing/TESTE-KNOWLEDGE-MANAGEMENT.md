# 🧪 Guia de Testes - Sistema de Gestão de Conhecimento

## 📋 Pré-requ### 2. **Guia Completo de Testes** 📄  
- Arquivo: `TODOS-OS-TESTES-SWAGGER.md`
- 21 cenários de teste sequenciais
- Inclui casos de erro e validação
- Interface visual intuitiva via Swagger UIs
- API rodando em `http://localhost:5000`
- Swagger UI disponível em `http://localhost:5000/swagger`
- Seguir guia em `TODOS-OS-TESTES-SWAGGER.md` para execução sequencial

## 🎯 Cenários de Teste

### 1. **Teste via Swagger UI** (Padrão Estabelecido)

Acesse: `http://localhost:5000/swagger`

#### **Knowledge Categories**
1. **GET /api/KnowledgeCategories** - Lista categorias (inicialmente vazia)
2. **POST /api/KnowledgeCategories** - Crie a primeira categoria:
   ```json
   {
     "name": "Conhecimento de TI",
     "description": "Base de conhecimento para área de Tecnologia da Informação",
     "color": "#007ACC",
     "icon": "💻",
     "isActive": true
   }
   ```
3. **Copie o ID retornado** para usar nos próximos testes
4. **POST /api/KnowledgeCategories** - Crie uma subcategoria:
   ```json
   {
     "name": "Programação",
     "description": "Conhecimentos sobre linguagens de programação",
     "color": "#28A745", 
     "icon": "⌨️",
     "isActive": true,
     "parentCategoryId": "COLE_O_ID_DA_CATEGORIA_PAI_AQUI"
   }
   ```
5. **GET /api/KnowledgeCategories?includeHierarchy=true** - Veja a hierarquia

#### **Tags**
1. **GET /api/Tags** - Lista tags (inicialmente vazia)
2. **POST /api/Tags** - Crie tags de tecnologia:
   ```json
   {
     "name": "C#",
     "description": "Linguagem de programação C#",
     "type": 2,
     "color": "#239120"
   }
   ```
   ```json
   {
     "name": ".NET",
     "description": "Framework .NET da Microsoft", 
     "type": 2,
     "color": "#512BD4"
   }
   ```
3. **GET /api/Tags?type=2** - Filtre por tipo Technology (2)
4. **GET /api/Tags/popular** - Veja tags mais populares

### 2. **Testes de Validação e Erros**

#### **Duplicatas** (Devem falhar com erro 409)
- Tente criar categoria com nome duplicado
- Tente criar tag com nome duplicado

#### **Não Encontrado** (Devem falhar com erro 404)
- GET categoria com ID inexistente: `/api/KnowledgeCategories/00000000-0000-0000-0000-000000000000`
- GET tag com ID inexistente: `/api/Tags/00000000-0000-0000-0000-000000000000`

### 3. **Testes de Integração**

#### **Verificar Dados Existentes**
1. **GET /api/Departments** - Liste departamentos existentes
2. **GET /api/Employees** - Liste funcionários (futuros autores)

## 🔍 **Pontos de Verificação**

### ✅ **Funcionalidades a Validar**

**Knowledge Categories:**
- [x] Criação de categoria raiz
- [x] Criação de subcategoria 
- [x] Listagem hierárquica
- [x] Busca por ID
- [x] Atualização parcial
- [x] Validação de nomes duplicados
- [x] Validação de categoria pai

**Tags:**
- [x] Criação de tags por tipo
- [x] Listagem com filtros
- [x] Busca por termo
- [x] Ordenação por uso/nome/data
- [x] Tags populares
- [x] Atualização
- [x] Validação de duplicatas

**Integridade:**
- [x] Soft delete funcionando
- [x] Relacionamentos preservados
- [x] Contadores atualizados
- [x] Índices otimizados

## 📊 **Resultados Esperados**

### **Swagger UI**
- 10 endpoints para Knowledge Categories
- 12 endpoints para Tags
- Schemas bem documentados
- Validações funcionando

### **Banco de Dados**
```sql
-- Verificar dados criados
SELECT * FROM Communication.KnowledgeCategories;
SELECT * FROM Communication.Tags;
SELECT * FROM Communication.PostTags; -- Estará vazia inicialmente
```

### **Logs da API**
- Requests sendo processados
- Validações sendo executadas
- Exceções capturadas e retornadas adequadamente

## 🚀 **Próximos Testes (Fase 3.3)**
Após validar categories e tags:
1. Implementar controlador de Posts aprimorado
2. Testar criação de posts com categorias e tags
3. Validar sistema de versionamento
4. Testar workflow de aprovação
5. Verificar métricas e contadores

## 🐛 **Solução de Problemas**

**Erro 500:**
- Verificar se a migração foi aplicada: `dotnet ef database update`
- Verificar logs da aplicação no terminal

**Erro 404 em Controllers:**
- Verificar se os controllers estão registrados
- Rebuild da aplicação: `dotnet build`

**Dados não aparecem:**
- Verificar soft delete: entidades com `IsDeleted = true` não aparecem
- Verificar filtros de query aplicados automaticamente