# 🚀 Script de Otimização Rápida VS Code - SynQcore
# Execute este script quando o VS Code estiver lento

Write-Host "🔧 INICIANDO OTIMIZAÇÃO VS CODE - SYNQCORE" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

# 1. Verificar uso atual de memória
Write-Host "`n📊 Verificando uso atual de memória..." -ForegroundColor Yellow
$vscodeProcesses = Get-Process | Where-Object { $_.ProcessName -eq "Code" }
$totalMemoryMB = ($vscodeProcesses | Measure-Object WorkingSet -Sum).Sum / 1MB
Write-Host "💾 VS Code usando: $([math]::Round($totalMemoryMB, 1)) MB total" -ForegroundColor White

if ($totalMemoryMB -gt 2048) {
    Write-Host "⚠️  ALERTA: Uso de memória alto! Recomendado restart." -ForegroundColor Red
}

# 2. Limpar cache do .NET
Write-Host "`n🧹 Limpando cache .NET..." -ForegroundColor Yellow
try {
    dotnet nuget locals all --clear | Out-Null
    Write-Host "✅ Cache .NET limpo" -ForegroundColor Green
} catch {
    Write-Host "❌ Erro ao limpar cache .NET" -ForegroundColor Red
}

# 3. Limpar artefatos de build
Write-Host "`n🗂️  Limpando artefatos de build..." -ForegroundColor Yellow
$buildDirs = @("bin", "obj")
$deletedCount = 0

foreach ($dir in $buildDirs) {
    $paths = Get-ChildItem -Path "." -Recurse -Directory -Name $dir -ErrorAction SilentlyContinue
    foreach ($path in $paths) {
        try {
            Remove-Item -Path $path -Recurse -Force -ErrorAction SilentlyContinue
            $deletedCount++
        } catch {
            # Ignorar erros de acesso
        }
    }
}
Write-Host "✅ $deletedCount diretórios de build removidos" -ForegroundColor Green

# 4. Verificar espaço em disco
Write-Host "`n💽 Verificando espaço em disco..." -ForegroundColor Yellow
$drive = Get-WmiObject -Class Win32_LogicalDisk -Filter "DeviceID='D:'"
if ($drive) {
    $freeSpaceGB = [math]::Round($drive.FreeSpace / 1GB, 1)
    $totalSpaceGB = [math]::Round($drive.Size / 1GB, 1)
    $percentFree = [math]::Round(($drive.FreeSpace / $drive.Size) * 100, 1)
    
    Write-Host "💾 Drive D: $freeSpaceGB GB livres de $totalSpaceGB GB ($percentFree%)" -ForegroundColor White
    
    if ($percentFree -lt 10) {
        Write-Host "⚠️  ALERTA: Pouco espaço livre! Pode afetar performance." -ForegroundColor Red
    }
}

# 5. Listar extensões VS Code pesadas (se código estiver rodando)
Write-Host "`n🔌 Verificando extensões ativas..." -ForegroundColor Yellow
$vscodeExtensionsPath = "$env:USERPROFILE\.vscode\extensions"
if (Test-Path $vscodeExtensionsPath) {
    $extensions = Get-ChildItem $vscodeExtensionsPath | Where-Object { $_.Name -match "gitlens|live-share|docker|azure" }
    if ($extensions.Count -gt 0) {
        Write-Host "⚠️  Extensões que podem causar lentidão:" -ForegroundColor Yellow
        foreach ($ext in $extensions) {
            Write-Host "   - $($ext.Name)" -ForegroundColor Red
        }
        Write-Host "💡 Considere desabilitar temporariamente" -ForegroundColor Cyan
    } else {
        Write-Host "✅ Nenhuma extensão pesada detectada" -ForegroundColor Green
    }
}

# 6. Recomendações finais
Write-Host "`n🎯 RECOMENDACOES:" -ForegroundColor Cyan
Write-Host "1. 📁 Feche abas desnecessárias no VS Code" -ForegroundColor White
Write-Host "2. 🔄 Restart OmniSharp: Ctrl+Shift+P → 'OmniSharp: Restart'" -ForegroundColor White
Write-Host "3. 💾 Se usando >2GB RAM, reinicie VS Code" -ForegroundColor White
Write-Host "4. 🎯 Use workspaces focados por feature" -ForegroundColor White
Write-Host "5. ⚡ Configuracoes ja otimizadas em .vscode/settings.json" -ForegroundColor White

Write-Host "`n✅ OTIMIZACAO CONCLUIDA!" -ForegroundColor Green
Write-Host "📖 Veja mais dicas em: docs/PERFORMANCE_VSCODE.md" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan