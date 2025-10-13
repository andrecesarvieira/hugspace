# 🔄 Plano de Consolidação de Componentes

## **Situação Atual**

Temos dois componentes de post:
- `TestPostCard.razor` - Funcional, testado, UI moderna ✅
- `SimplePostCard.razor` - Estrutura existente, não testado 🔄

## **Ação Recomendada**

### **Opção 1: Migrar para SimplePostCard (Recomendado)**
```bash
# 1. Copiar melhorias do TestPostCard para SimplePostCard
# 2. Atualizar Feed.razor para usar SimplePostCard
# 3. Remover TestPostCard após validação
```

### **Opção 2: Manter TestPostCard (Mais Rápido)**
```bash
# 1. Renomear TestPostCard → PostCard
# 2. Atualizar imports e referências
# 3. Usar como componente oficial
```

## **Comando para Executar**

Para migrar as melhorias:
```bash
# Atualizar Feed.razor para usar SimplePostCard
# Copiar funcionalidades do TestPostCard
# Testar funcionalidades
```

Qual opção você prefere?