# 🚀 Guia de Otimização de Performance VS Code - SynQcore

## ✅ IMPLEMENTADO AUTOMATICAMENTE

As seguintes otimizações foram aplicadas ao `.vscode/settings.json`:

### 📁 **Exclusões de Pastas**
- `bin/`, `obj/` - Artefatos de build
- `node_modules/` - Dependências Node.js  
- `.vs/`, `packages/` - Cache do Visual Studio
- `wwwroot/lib/` - Bibliotecas estáticas
- `.git/objects/` - Objetos Git grandes

### 🔍 **Otimizações de Busca**
- Excluir arquivos binários (`.dll`, `.exe`, `.pdb`)
- Limitar monitoramento de arquivos
- Desabilitar busca em pastas grandes

### ⚡ **Performance do Editor**
- Minimap desabilitado
- CodeLens desabilitado  
- Breadcrumbs desabilitados
- Highlighting semântico otimizado

### 🧠 **Limitações de Memória**
- TypeScript: 4GB máximo
- Arquivos grandes: 4GB máximo
- OmniSharp otimizado

## 🛠️ AÇÕES MANUAIS RECOMENDADAS

### 1. **Fechar Abas Desnecessárias**
- Use `Ctrl+Shift+P` → "Close All Editors"
- Mantenha apenas 3-5 arquivos abertos

### 2. **Extensões a Desabilitar Temporariamente**
```
- GitLens (se instalado)
- Live Share
- Docker (se não usando)
- Azure extensions
- Prettier (se não usando)
```

### 3. **Usar Multi-Root Workspace**
Trabalhe apenas com a parte do projeto que precisa:

**Para API apenas:**
```
File → Add Folder to Workspace
→ Adicionar apenas: src/SynQcore.Api
```

**Para Blazor apenas:**
```
File → Add Folder to Workspace  
→ Adicionar apenas: src/SynQcore.BlazorApp
```

### 4. **Comando de Limpeza Rápida**
```powershell
# Execute no terminal do VS Code
cd "d:\Projetos\SynQcore"
.\synqcore.cmd clean
```

### 5. **Restart OmniSharp**
- `Ctrl+Shift+P` → "OmniSharp: Restart OmniSharp"
- Faça isso se o IntelliSense ficar lento

## 🎯 ESTRATÉGIAS AVANÇADAS

### **Workspace Focado por Feature**
Em vez de abrir o projeto inteiro:

1. **Para trabalhar no Feed:**
   - Abra apenas: `Components/Pages/Feed.razor`
   - Abra apenas: `Components/Social/`

2. **Para trabalhar na API:**
   - Abra apenas: `src/SynQcore.Api/`
   - Abra apenas: `src/SynQcore.Application/Features/[Feature]/`

### **Terminal Performance**
```powershell
# Use este comando para verificar uso de memória
Get-Process Code | Select-Object Name, WorkingSet, CPU

# Se VS Code usar > 2GB, restart recomendado
```

### **Hotkeys de Produtividade**
- `Ctrl+P` - Quick Open (mais rápido que explorer)
- `Ctrl+Shift+O` - Go to Symbol (navegação rápida)
- `Ctrl+Shift+F` - Find in Files (busca otimizada)
- `Ctrl+Shift+E` - Toggle Explorer (esconder quando não usar)

## 🔧 CONFIGURAÇÕES DE SISTEMA

### **Windows Performance**
- Desabilitar Windows Defender real-time scan na pasta do projeto
- Adicionar exceção: `d:\Projetos\SynQcore\`
- SSD recomendado para projetos .NET grandes

### **Memória RAM**
- Mínimo: 16GB para projetos .NET 9
- Recomendado: 32GB para desenvolvimento full-stack
- Fechar Chrome/browsers desnecessários

## 🚨 SINAIS DE LENTIDÃO

Se ainda estiver lento, verifique:

1. **Task Manager** - VS Code usando > 2GB RAM
2. **OmniSharp** - Logs de erro no Output
3. **Extensions** - Muitas extensões ativas
4. **Disk Usage** - SSD com pouco espaço livre

## 💡 DICAS FINAIS

- **Reinicie VS Code** a cada 2-3 horas de uso intenso
- **Use workspaces específicos** por feature
- **Mantenha apenas arquivos essenciais abertos**
- **Execute limpeza** antes de sessões longas de coding

---
*Configurações aplicadas automaticamente em: `.vscode/settings.json`*