# 🎯 EXECUÇÃO COMPLETA - TODOS OS TESTES VIA SWAGGER UI

## 🚀 **API RODANDO**: http://localhost:5006/swagger

### 📋 **SEQUÊNCIA COMPLETA DE TESTES**

## **PARTE 1: KNOWLEDGE CATEGORIES** 🗂️

### **1️⃣ Verificar Estado Inicial**
- **Endpoint**: `GET /api/KnowledgeCategories`
- **Clique em**: "Try it out" → "Execute"
- **Resultado esperado**: `[]` (array vazio)

### **2️⃣ Criar Categoria Raiz - TI**
- **Endpoint**: `POST /api/KnowledgeCategories`
- **Clique em**: "Try it out"
- **Cole o JSON**:
```json
{
  "name": "Conhecimento de TI",
  "description": "Base de conhecimento para área de Tecnologia da Informação",
  "color": "#007ACC",
  "icon": "💻",
  "isActive": true
}
```
- **Execute** e **COPIE O ID** retornado (ex: `a1b2c3d4-e5f6-...`)

### **3️⃣ Criar Subcategoria - Programação**
- **Endpoint**: `POST /api/KnowledgeCategories`
- **Cole o JSON** (substitua `SEU_ID_TI_AQUI`):
```json
{
  "name": "Programação",
  "description": "Conhecimentos sobre linguagens de programação e desenvolvimento",
  "color": "#28A745",
  "icon": "⌨️",
  "isActive": true,
  "parentCategoryId": "SEU_ID_TI_AQUI"
}
```

### **4️⃣ Criar Outra Categoria Raiz - RH**
- **Endpoint**: `POST /api/KnowledgeCategories`
- **Cole o JSON**:
```json
{
  "name": "Recursos Humanos",
  "description": "Políticas, procedimentos e conhecimentos de RH",
  "color": "#FFC107",
  "icon": "👥",
  "isActive": true
}
```

### **5️⃣ Ver Hierarquia Completa**
- **Endpoint**: `GET /api/KnowledgeCategories`
- **Parâmetros**:
  - `includeInactive`: `false`
  - `includeHierarchy`: `true`
- **Resultado esperado**: Categorias com subcategorias aninhadas

### **6️⃣ Buscar Categoria Específica**
- **Endpoint**: `GET /api/KnowledgeCategories/{id}`
- **Substitua {id}** pelo ID da categoria TI
- **Resultado**: Detalhes da categoria com contagem de posts

### **7️⃣ Atualizar Categoria**
- **Endpoint**: `PUT /api/KnowledgeCategories/{id}`
- **Substitua {id}** pelo ID da categoria TI
- **Cole o JSON**:
```json
{
  "description": "Base de conhecimento ATUALIZADA para área de TI",
  "color": "#0066CC"
}
```

## **PARTE 2: TAGS** 🏷️

### **8️⃣ Verificar Tags Vazias**
- **Endpoint**: `GET /api/Tags`
- **Resultado esperado**: `[]` (array vazio)

### **9️⃣ Criar Tags de Tecnologia**
- **Endpoint**: `POST /api/Tags`
- **Tag 1 - C#**:
```json
{
  "name": "C#",
  "description": "Linguagem de programação C#",
  "type": 2,
  "color": "#239120"
}
```
- **Tag 2 - .NET**:
```json
{
  "name": ".NET",
  "description": "Framework .NET da Microsoft",
  "type": 2,
  "color": "#512BD4"
}
```
- **Tag 3 - Entity Framework**:
```json
{
  "name": "Entity Framework",
  "description": "ORM para .NET",
  "type": 2,
  "color": "#FF6B35"
}
```

### **🔟 Criar Tags de Habilidades**
- **Endpoint**: `POST /api/Tags`
- **Tag 1 - Backend Development**:
```json
{
  "name": "Backend Development",
  "description": "Desenvolvimento de sistemas backend",
  "type": 1,
  "color": "#DC3545"
}
```
- **Tag 2 - Database Design**:
```json
{
  "name": "Database Design",
  "description": "Design e modelagem de banco de dados",
  "type": 1,
  "color": "#17A2B8"
}
```

### **1️⃣1️⃣ Filtrar Tags por Tipo**
- **Endpoint**: `GET /api/Tags`
- **Parâmetro `type`**: `2` (Technology)
- **Resultado**: Apenas C#, .NET e Entity Framework

### **1️⃣2️⃣ Buscar Tags por Termo**
- **Endpoint**: `GET /api/Tags`
- **Parâmetro `searchTerm`**: `.NET`
- **Resultado**: Apenas a tag .NET

### **1️⃣3️⃣ Ver Tags Populares**
- **Endpoint**: `GET /api/Tags/popular`
- **Parâmetro `count`**: `10`
- **Resultado**: Todas as tags ordenadas por uso

### **1️⃣4️⃣ Ordenar por Uso**
- **Endpoint**: `GET /api/Tags`
- **Parâmetros**:
  - `sortBy`: `usagecount`
  - `sortDescending`: `true`

## **PARTE 3: TESTES DE VALIDAÇÃO** ⚠️

### **1️⃣5️⃣ Erro 409 - Nome Duplicado**
- **Endpoint**: `POST /api/KnowledgeCategories`
- **Tente criar categoria com nome existente**:
```json
{
  "name": "Conhecimento de TI",
  "description": "Teste de duplicata",
  "color": "#FF0000",
  "icon": "❌"
}
```
- **Resultado esperado**: Erro 409 Conflict

### **1️⃣6️⃣ Erro 404 - Não Encontrado**
- **Endpoint**: `GET /api/KnowledgeCategories/{id}`
- **Use ID inválido**: `00000000-0000-0000-0000-000000000000`
- **Resultado esperado**: Erro 404 Not Found

### **1️⃣7️⃣ Tag Duplicada**
- **Endpoint**: `POST /api/Tags`
- **Tente criar tag existente**:
```json
{
  "name": "C#",
  "description": "Teste duplicata",
  "type": 2,
  "color": "#FF0000"
}
```
- **Resultado esperado**: Erro 409 Conflict

## **PARTE 4: INTEGRAÇÃO COM DADOS EXISTENTES** 🔗

### **1️⃣8️⃣ Verificar Departamentos**
- **Endpoint**: `GET /api/Departments`
- **Resultado**: Lista de departamentos existentes

### **1️⃣9️⃣ Verificar Funcionários**
- **Endpoint**: `GET /api/Employees`
- **Resultado**: Lista de funcionários (futuros autores)

## **PARTE 5: LIMPEZA (OPCIONAL)** 🧹

### **2️⃣0️⃣ Tentar Excluir Tag**
- **Endpoint**: `DELETE /api/Tags/{id}`
- **Use ID de uma tag**
- **Resultado**: Sucesso (não há posts usando ainda)

### **2️⃣1️⃣ Tentar Excluir Categoria com Subcategoria**
- **Endpoint**: `DELETE /api/KnowledgeCategories/{id}`
- **Use ID da categoria TI (que tem subcategoria)**
- **Resultado esperado**: Erro 400 - Cannot delete category with subcategories

## 🎉 **RESULTADOS ESPERADOS**

✅ **20+ testes executados com sucesso**  
✅ **Sistema de categorias hierárquicas funcional**  
✅ **Sistema de tags tipificadas funcional**  
✅ **Validações de negócio funcionando**  
✅ **Tratamento de erros adequado**  
✅ **Integração com dados existentes**  

## 📊 **MÉTRICAS DE SUCESSO**

- **Categories criadas**: 3 (TI, Programação, RH)
- **Tags criadas**: 5 (C#, .NET, EF, Backend, Database)
- **Hierarquia testada**: ✅
- **Filtros testados**: ✅  
- **Validações testadas**: ✅
- **Errors handled**: ✅

🚀 **Sistema de Gestão de Conhecimento 100% Funcional!**