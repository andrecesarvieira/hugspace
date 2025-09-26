# 🧪 DEMONSTRAÇÃO PRÁTICA - SWAGGER UI

## 📋 INSTRUÇÕES

1. **Certifique-se de que a API está rodando**:
   ```bash
   dotnet run --project /mnt/Dados/Projetos/SynQcore/src/SynQcore.Api/SynQcore.Api.csproj
   ```
   
2. **Acesse o Swagger UI**: http://localhost:5000/swagger

3. **Execute os testes na ordem** seguindo o guia em `TODOS-OS-TESTES-SWAGGER.md`

## 🎯 SEQUÊNCIA DE TESTES RECOMENDADA

### 1️⃣ **VERIFICAR ESTADO INICIAL**
```http
GET http://localhost:5000/api/KnowledgeCategories
```
**Resultado esperado**: `[]` (array vazio)

### 2️⃣ **CRIAR PRIMEIRA CATEGORIA**
```http
POST http://localhost:5000/api/KnowledgeCategories
Content-Type: application/json

{
    "name": "Conhecimento de TI",
    "description": "Base de conhecimento para área de Tecnologia da Informação",
    "color": "#007ACC",
    "icon": "💻",
    "isActive": true
}
```
**⚠️ COPIE O ID RETORNADO!** Exemplo: `"id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"`

### 3️⃣ **CRIAR SUBCATEGORIA**
```http
POST http://localhost:5000/api/KnowledgeCategories
Content-Type: application/json

{
    "name": "Programação",
    "description": "Conhecimentos sobre linguagens de programação",
    "color": "#28A745",
    "icon": "⌨️",
    "isActive": true,
    "parentCategoryId": "COLE_O_ID_DA_CATEGORIA_ANTERIOR_AQUI"
}
```

### 4️⃣ **VER HIERARQUIA**
```http
GET http://localhost:5000/api/KnowledgeCategories?includeHierarchy=true
```
**Resultado esperado**: Categoria TI com subcategoria Programação

### 5️⃣ **CRIAR TAGS**
```http
POST http://localhost:5000/api/Tags
Content-Type: application/json

{
    "name": "C#",
    "description": "Linguagem de programação C#",
    "type": 2,
    "color": "#239120"
}
```

```http
POST http://localhost:5000/api/Tags
Content-Type: application/json

{
    "name": ".NET",
    "description": "Framework .NET da Microsoft",
    "type": 2,
    "color": "#512BD4"
}
```

### 6️⃣ **FILTRAR TAGS POR TIPO**
```http
GET http://localhost:5000/api/Tags?type=2
```
**Resultado esperado**: Só as tags de tecnologia (C# e .NET)

## 🔍 TIPOS DE TAG

- `0` = General (Geral)
- `1` = Skill (Habilidade)  
- `2` = Technology (Tecnologia)
- `3` = Department (Departamento)
- `4` = Project (Projeto)

## ✅ TESTES DE VALIDAÇÃO

### **Erro 409 - Conflito**
Tente criar categoria com nome duplicado:
```http
POST http://localhost:5000/api/KnowledgeCategories
Content-Type: application/json

{
    "name": "Conhecimento de TI",
    "description": "Teste de duplicata",
    "color": "#FF0000",
    "icon": "❌"
}
```

### **Erro 404 - Não Encontrado**
```http
GET http://localhost:5000/api/KnowledgeCategories/00000000-0000-0000-0000-000000000000
```

## 🎉 SUCESSO!

Se todos os testes funcionarem, você terá:

✅ **Sistema de categorias hierárquicas** funcionando  
✅ **Sistema de tags tipificadas** funcionando  
✅ **Validações de negócio** funcionando  
✅ **API REST completa** para gestão de conhecimento  

## 🚀 PRÓXIMOS PASSOS

Após validar Categories e Tags, você pode:
1. Implementar Posts com Knowledge Management
2. Testar associação de Posts com Categories e Tags
3. Implementar sistema de busca avançada
4. Criar workflow de aprovação