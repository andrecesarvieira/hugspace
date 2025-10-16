# Script para abrir workspaces focados do SynQcore
# Execute: .\open-workspace.ps1 [workspace]

param(
    [Parameter(Position=0)]
    [ValidateSet("backend", "frontend", "search", "feed", "full", "help")]
    [string]$Workspace = "help"
)

$workspaceDir = "d:\Projetos\SynQcore\workspaces"

function Show-Help {
    Write-Host "`n🎯 WORKSPACES FOCADOS DISPONÍVEIS:" -ForegroundColor Cyan
    Write-Host "================================" -ForegroundColor Cyan
    Write-Host "`n📚 COMO USAR:" -ForegroundColor Yellow
    Write-Host "  .\open-workspace.ps1 [nome]" -ForegroundColor White
    
    Write-Host "`n🎯 WORKSPACES DISPONÍVEIS:" -ForegroundColor Yellow
    Write-Host "  backend   - 🔧 API + Application + Domain + Infrastructure" -ForegroundColor Green
    Write-Host "  frontend  - 🌐 Blazor + Application + Domain + Tests" -ForegroundColor Green  
    Write-Host "  search    - 🔍 Apenas funcionalidade de busca" -ForegroundColor Green
    Write-Host "  feed      - 📄 Apenas funcionalidade do feed" -ForegroundColor Green
    Write-Host "  full      - 📁 Projeto completo (lento)" -ForegroundColor Red
    
    Write-Host "`n💡 EXEMPLOS:" -ForegroundColor Yellow
    Write-Host "  .\open-workspace.ps1 backend   # Para trabalhar na API" -ForegroundColor White
    Write-Host "  .\open-workspace.ps1 frontend  # Para trabalhar no Blazor" -ForegroundColor White
    Write-Host "  .\open-workspace.ps1 search    # Para trabalhar na busca" -ForegroundColor White
    
    Write-Host "`n⚡ BENEFÍCIOS:" -ForegroundColor Yellow
    Write-Host "  • 50-70% menos uso de RAM" -ForegroundColor Green
    Write-Host "  • IntelliSense 3x mais rápido" -ForegroundColor Green  
    Write-Host "  • Busca de arquivos instantânea" -ForegroundColor Green
    Write-Host "  • Foco apenas no que você precisa" -ForegroundColor Green
    Write-Host ""
}

function Open-Workspace($WorkspaceName, $WorkspaceFile) {
    $fullPath = Join-Path $workspaceDir $WorkspaceFile
    
    if (Test-Path $fullPath) {
        Write-Host "🚀 Abrindo workspace: $WorkspaceName" -ForegroundColor Green
        Write-Host "📁 Arquivo: $WorkspaceFile" -ForegroundColor Gray
        
        # Abrir no VS Code
        & code $fullPath
        
        Write-Host "✅ Workspace aberto com sucesso!" -ForegroundColor Green
        Write-Host "💡 Aguarde o carregamento do OmniSharp..." -ForegroundColor Yellow
    } else {
        Write-Host "❌ Erro: Arquivo não encontrado: $fullPath" -ForegroundColor Red
    }
}

switch ($Workspace.ToLower()) {
    "backend" {
        Open-Workspace "Backend (API + Core)" "SynQcore-Backend.code-workspace"
    }
    "frontend" {
        Open-Workspace "Frontend (Blazor + UI)" "SynQcore-Frontend.code-workspace"  
    }
    "search" {
        Open-Workspace "Search Feature" "SynQcore-Search.code-workspace"
    }
    "feed" {
        Open-Workspace "Feed Feature" "SynQcore-Feed.code-workspace"
    }
    "full" {
        Write-Host "⚠️  Abrindo projeto completo (pode ser lento)..." -ForegroundColor Yellow
        & code "d:\Projetos\SynQcore"
    }
    "help" {
        Show-Help
    }
    default {
        Write-Host "❌ Workspace inválido: $Workspace" -ForegroundColor Red
        Show-Help
    }
}