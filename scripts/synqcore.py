#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
🚀 SynQcore - Script Unificado de Gerenciamento
================================================

Script consolidado que substitui todos os scripts anteriores.
Oferece todas as funcionalidades necessárias para o desenvolvimento do SynQcore.

Uso: python3 scripts/synqcore.py [comando]

Comandos disponíveis:
  start           - Iniciar aplicação completa (API + Blazor)
  api             - Iniciar apenas API na porta 5000
  blazor          - Iniciar apenas Blazor na porta 5226
  clean           - Limpeza completa do projeto
  docker-up       - Iniciar infraestrutura Docker
  docker-down     - Parar infraestrutura Docker
  help            - Mostrar esta ajuda

Se nenhum comando for especificado, será executado 'start' por padrão.

📍 URLs após inicialização:
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Health: http://localhost:5000/health
- Blazor: http://localhost:5226
- Design System: http://localhost:5226/design-system

Pressione Ctrl+C para parar todas as aplicações.
"""

import os
import signal
import subprocess
import sys
import time
import threading
import webbrowser
import shutil
from pathlib import Path

# Cores para terminal
class Colors:
    RED = '\033[0;31m'
    GREEN = '\033[0;32m'
    YELLOW = '\033[1;33m'
    BLUE = '\033[0;34m'
    CYAN = '\033[0;36m'
    MAGENTA = '\033[0;35m'
    BOLD = '\033[1m'
    NC = '\033[0m'  # No Color

def log_info(message):
    print(f"{Colors.BLUE}ℹ️  {message}{Colors.NC}")

def log_success(message):
    print(f"{Colors.GREEN}✅ {message}{Colors.NC}")

def log_warning(message):
    print(f"{Colors.YELLOW}⚠️  {message}{Colors.NC}")

def log_error(message):
    print(f"{Colors.RED}❌ {message}{Colors.NC}")

def log_api(message):
    print(f"{Colors.MAGENTA}[API] {message}{Colors.NC}")

def log_blazor(message):
    print(f"{Colors.CYAN}[BLAZOR] {message}{Colors.NC}")

def log_docker(message):
    print(f"{Colors.BLUE}[DOCKER] {message}{Colors.NC}")

def print_banner(title):
    """Imprime banner formatado"""
    border = "=" * 70
    print(f"{Colors.GREEN}{border}{Colors.NC}")
    print(f"{Colors.GREEN}  🚀 {title}{Colors.NC}")
    print(f"{Colors.GREEN}{border}{Colors.NC}")
    print()

def run_command(command, capture_output=True, cwd=None):
    """Executa um comando e retorna o resultado"""
    try:
        result = subprocess.run(
            command,
            shell=True,
            capture_output=capture_output,
            text=True,
            cwd=cwd
        )
        return result.returncode == 0, result.stdout, result.stderr
    except Exception as e:
        return False, "", str(e)

def is_port_in_use(port):
    """Verifica se a porta está em uso"""
    success, stdout, stderr = run_command(f"lsof -Pi :{port} -sTCP:LISTEN -t")
    return success and stdout.strip()

def kill_processes_on_port(port):
    """Mata processos que estão usando a porta"""
    if port == 5000:
        # Para API
        run_command("pkill -f 'dotnet.*SynQcore.Api'")
        run_command("pkill -f 'SynQcore.Api'")
    elif port == 5226:
        # Para Blazor
        run_command("pkill -f 'dotnet.*Blazor'")
        run_command("pkill -f 'SynQcore.BlazorApp'")

    # Mata qualquer processo na porta
    run_command("pkill -f 'dotnet.*run'")

    time.sleep(3)

    # Se ainda houver processos na porta, mata-os forçadamente
    success, pids, stderr = run_command(f"lsof -Pi :{port} -sTCP:LISTEN -t")
    if success and pids.strip():
        for pid in pids.strip().split('\n'):
            if pid:
                run_command(f"kill -9 {pid}")
        time.sleep(2)

def check_project_structure():
    """Verifica se a estrutura dos projetos está correta"""
    api_dir = Path.cwd() / "src" / "SynQcore.Api"
    api_file = api_dir / "SynQcore.Api.csproj"

    blazor_dir = Path.cwd() / "src" / "SynQcore.BlazorApp" / "SynQcore.BlazorApp"
    blazor_file = blazor_dir / "SynQcore.BlazorApp.csproj"

    if not api_dir.exists():
        log_error(f"Diretório da API não encontrado: {api_dir}")
        return False, None, None

    if not api_file.exists():
        log_error(f"Arquivo do projeto API não encontrado: {api_file}")
        return False, None, None

    if not blazor_dir.exists():
        log_error(f"Diretório do Blazor não encontrado: {blazor_dir}")
        return False, None, None

    if not blazor_file.exists():
        log_error(f"Arquivo do projeto Blazor não encontrado: {blazor_file}")
        return False, None, None

    return True, api_dir, blazor_dir

def restore_and_build():
    """Restaura e compila a solução usando método seguro"""
    try:
        print(f"\n{Colors.CYAN}📦 Preparando ambiente...{Colors.NC}")

        # Método super seguro para evitar erro CLR
        log_info("Limpando servidores de build...")
        subprocess.run(["dotnet", "build-server", "shutdown"],
                      check=False, capture_output=True)

        log_info("Limpando artefatos de build...")
        subprocess.run(["rm", "-rf", "src/*/bin", "src/*/obj", "tests/*/bin", "tests/*/obj"],
                      shell=True, check=False, capture_output=True)

        # Aguardar um pouco para evitar conflitos
        time.sleep(2)

        log_success("Limpeza concluída!")

        # Usar build direto com dotnet (single thread para evitar CLR error)
        log_info("Executando build seguro...")
        result = subprocess.run([
            "dotnet", "build", "SynQcore.sln",
            "--configuration", "Debug",
            "--maxcpucount:1",  # Force single CPU to avoid CLR errors
            "--verbosity", "minimal"
        ],
                               cwd=os.getcwd(),
                               capture_output=True,
                               text=True)

        if result.returncode != 0:
            log_error("Falha na compilação:")
            print(result.stderr)
            print(result.stdout)
            return False

        log_success("Compilação bem-sucedida!")
        return True

    except Exception as e:
        log_error(f"Erro durante compilação: {e}")
        return False

def run_api(api_dir):
    """Executa a API em thread separada"""
    def api_thread():
        try:
            log_api("Iniciando API na porta 5000...")
            process = subprocess.Popen(
                "dotnet run --no-build --no-restore --urls http://localhost:5000",
                shell=True,
                cwd=api_dir,
                stdout=subprocess.PIPE,
                stderr=subprocess.STDOUT,
                universal_newlines=True,
                bufsize=1
            )

            # Aguardar a API estar pronta
            api_ready = False
            for line in iter(process.stdout.readline, ''):
                line = line.strip()
                if line:
                    print(f"{Colors.MAGENTA}[API] {line}{Colors.NC}")

                    if "Application started" in line or "Now listening on" in line:
                        api_ready = True
                        log_api("API iniciada com sucesso!")
                        break

            if not api_ready:
                log_error("API falhou ao iniciar")
                return

            # Continuar mostrando logs da API
            for line in iter(process.stdout.readline, ''):
                line = line.strip()
                if line:
                    print(f"{Colors.MAGENTA}[API] {line}{Colors.NC}")

        except Exception as e:
            log_error(f"Erro na API: {e}")

    thread = threading.Thread(target=api_thread, daemon=True)
    thread.start()
    return thread

def run_blazor(blazor_dir):
    """Executa o Blazor em thread separada"""
    def blazor_thread():
        try:
            log_blazor("Iniciando Blazor App na porta 5226...")
            process = subprocess.Popen(
                "dotnet run --no-build --no-restore --urls http://localhost:5226",
                shell=True,
                cwd=blazor_dir,
                stdout=subprocess.PIPE,
                stderr=subprocess.STDOUT,
                universal_newlines=True,
                bufsize=1
            )

            # Aguardar o Blazor estar pronto
            blazor_ready = False
            for line in iter(process.stdout.readline, ''):
                line = line.strip()
                if line:
                    print(f"{Colors.CYAN}[BLAZOR] {line}{Colors.NC}")

                    if "Application started" in line or "Now listening on" in line:
                        blazor_ready = True
                        log_blazor("Blazor App iniciado com sucesso!")
                        break

            if not blazor_ready:
                log_error("Blazor App falhou ao iniciar")
                return

            # Continuar mostrando logs do Blazor
            for line in iter(process.stdout.readline, ''):
                line = line.strip()
                if line:
                    print(f"{Colors.CYAN}[BLAZOR] {line}{Colors.NC}")

        except Exception as e:
            log_error(f"Erro no Blazor: {e}")

    thread = threading.Thread(target=blazor_thread, daemon=True)
    thread.start()
    return thread

def wait_for_services():
    """Aguarda os serviços ficarem prontos"""
    log_info("Aguardando serviços iniciarem...")

    max_attempts = 30
    api_ready = False
    blazor_ready = False

    for attempt in range(max_attempts):
        # Verificar API
        if not api_ready:
            success, stdout, stderr = run_command("curl -s -o /dev/null -w '%{http_code}' http://localhost:5000/health")
            if success and "200" in stdout:
                api_ready = True
                log_api("Health check da API respondendo!")

        # Verificar Blazor
        if not blazor_ready:
            success, stdout, stderr = run_command("curl -s -o /dev/null -w '%{http_code}' http://localhost:5226/")
            if success and "200" in stdout:
                blazor_ready = True
                log_blazor("Blazor App respondendo!")

        if api_ready and blazor_ready:
            break

        time.sleep(2)

    return api_ready, blazor_ready

def open_browser_delayed(delay=5):
    """Abre o navegador após um delay"""
    def open_browser():
        time.sleep(delay)
        try:
            log_info("Abrindo navegador automaticamente...")
            webbrowser.open("http://localhost:5226/")

        except Exception as e:
            log_warning(f"Erro ao abrir navegador: {e}")

    thread = threading.Thread(target=open_browser, daemon=True)
    thread.start()

def setup_signal_handlers():
    """Configura handlers para sinais de interrupção"""
    def signal_handler(sig, frame):
        print("\n")
        log_info("Encerrando aplicações...")

        # Matar processos das portas
        kill_processes_on_port(5000)
        kill_processes_on_port(5226)

        log_info("Aplicações encerradas")
        sys.exit(0)

    signal.signal(signal.SIGINT, signal_handler)
    signal.signal(signal.SIGTERM, signal_handler)

def clean_project():
    """Limpeza completa do projeto"""
    print_banner("SynQcore - Limpeza Completa do Projeto")

    log_info("Limpando projeto SynQcore...")
    print()

    # 1. Limpa builds anteriores
    log_info("🗑️  Removendo builds anteriores...")
    run_command("dotnet clean --verbosity quiet", capture_output=True)

    # 2. Remove pastas bin e obj
    log_info("📁 Removendo pastas bin/ e obj/...")
    count = 0
    for root, dirs, files in os.walk('.'):
        dirs_to_remove = [d for d in dirs if d in ['bin', 'obj']]
        for dir_name in dirs_to_remove:
            dir_path = Path(root) / dir_name
            try:
                shutil.rmtree(dir_path)
                count += 1
            except Exception as e:
                pass
    if count > 0:
        log_info(f"Removidos {count} diretórios bin/obj")

    # 3. Remove arquivos temporários
    log_info("🗂️  Removendo arquivos temporários...")
    temp_count = 0
    for root, dirs, files in os.walk('.'):
        files_to_remove = [f for f in files if f.endswith(('.tmp', '.log'))]
        for file_name in files_to_remove:
            file_path = Path(root) / file_name
            try:
                file_path.unlink()
                temp_count += 1
            except Exception as e:
                pass
    if temp_count > 0:
        log_info(f"Removidos {temp_count} arquivos temporários")

    # 4. Remove TestResults
    log_info("🧪 Removendo resultados de testes...")
    test_count = 0
    for root, dirs, files in os.walk('.'):
        dirs_to_remove = [d for d in dirs if d == 'TestResults']
        for dir_name in dirs_to_remove:
            dir_path = Path(root) / dir_name
            try:
                shutil.rmtree(dir_path)
                test_count += 1
            except Exception as e:
                pass
    if test_count > 0:
        log_info(f"Removidos {test_count} diretórios TestResults")

    # 5. Limpa cache do NuGet
    log_info("📦 Limpando cache do NuGet...")
    run_command("dotnet nuget locals all --clear --verbosity quiet", capture_output=True)

    print()
    log_success("Projeto limpo com sucesso!")
    print()
    log_info("💡 Para reconstruir: python3 scripts/synqcore.py start")
    print()

def docker_up():
    """Inicia infraestrutura Docker"""
    print_banner("SynQcore - Iniciando Infraestrutura Docker")

    docker_dir = Path.cwd() / "docker"
    if not docker_dir.exists():
        log_error("Diretório docker/ não encontrado")
        sys.exit(1)

    log_docker("Iniciando containers Docker...")
    success, stdout, stderr = run_command("docker-compose up -d", cwd=docker_dir)

    if success:
        log_success("Infraestrutura Docker iniciada!")
        print()
        print("📍 Serviços disponíveis:")
        print("   🐘 PostgreSQL: localhost:5432")
        print("   🗄️  Redis: localhost:6379")
        print("   🌐 pgAdmin: http://localhost:8080")
        print()
        log_info("Para iniciar a aplicação: python3 scripts/synqcore.py start")
    else:
        log_error("Erro ao iniciar Docker:")
        print(stderr)

def docker_down():
    """Para infraestrutura Docker"""
    print_banner("SynQcore - Parando Infraestrutura Docker")

    docker_dir = Path.cwd() / "docker"
    if not docker_dir.exists():
        log_error("Diretório docker/ não encontrado")
        sys.exit(1)

    log_docker("Parando containers Docker...")
    success, stdout, stderr = run_command("docker-compose down", cwd=docker_dir)

    if success:
        log_success("Infraestrutura Docker parada!")
    else:
        log_error("Erro ao parar Docker:")
        print(stderr)

def start_api_only():
    """Inicia apenas a API"""
    print_banner("SynQcore API - Porta 5000")

    # Verificar se estamos no diretório correto
    if not Path("SynQcore.sln").exists():
        log_error("Execute este script no diretório raiz do projeto (onde está SynQcore.sln)")
        sys.exit(1)

    # Configurar handlers de sinal
    setup_signal_handlers()

    # Verificar e liberar porta
    if is_port_in_use(5000):
        log_warning("Porta 5000 está ocupada. Liberando...")
        kill_processes_on_port(5000)

    # Verificar estrutura do projeto
    is_valid, api_dir, _ = check_project_structure()
    if not is_valid:
        sys.exit(1)

    # Restaurar e compilar
    if not restore_and_build():
        sys.exit(1)

    print()
    log_success("🎯 Configuração concluída! Iniciando API...")
    print()
    print("📍 URLs disponíveis:")
    print("   🌐 API: http://localhost:5000")
    print("   📚 Swagger: http://localhost:5000/swagger")
    print("   🏥 Health: http://localhost:5000/health")
    print()
    print("💡 Pressione Ctrl+C para parar a aplicação")
    print()

    # Executar a aplicação
    try:
        subprocess.run(
            "dotnet run --no-build --no-restore --urls http://localhost:5000",
            shell=True,
            cwd=api_dir
        )
    except KeyboardInterrupt:
        pass

    log_info("API encerrada")

def start_blazor_only():
    """Inicia apenas o Blazor"""
    print_banner("SynQcore Blazor App - Porta 5226")

    # Verificar se estamos no diretório correto
    if not Path("SynQcore.sln").exists():
        log_error("Execute este script no diretório raiz do projeto (onde está SynQcore.sln)")
        sys.exit(1)

    # Configurar handlers de sinal
    setup_signal_handlers()

    # Verificar e liberar porta
    if is_port_in_use(5226):
        log_warning("Porta 5226 está ocupada. Liberando...")
        kill_processes_on_port(5226)

    # Verificar estrutura do projeto
    is_valid, _, blazor_dir = check_project_structure()
    if not is_valid:
        sys.exit(1)

    # Restaurar e compilar
    if not restore_and_build():
        sys.exit(1)

    print()
    log_success("🎯 Configuração concluída! Iniciando Blazor App...")
    print()
    print("📍 URLs disponíveis:")
    print("   🌐 Blazor App: http://localhost:5226")
    print("   📱 Feed Principal: http://localhost:5226/")
    print("   🎨 Design System: http://localhost:5226/design-system")
    print()
    print("🌐 Abrindo navegador automaticamente...")
    print("💡 Pressione Ctrl+C para parar a aplicação")
    print()

    # Abrir navegador após delay
    threading.Thread(target=lambda: (time.sleep(5), webbrowser.open("http://localhost:5226/")), daemon=True).start()

    # Executar a aplicação
    try:
        subprocess.run(
            "dotnet run --no-build --no-restore --urls http://localhost:5226",
            shell=True,
            cwd=blazor_dir
        )
    except KeyboardInterrupt:
        pass

    log_info("Blazor App encerrado")

def start_full_application():
    """Inicia aplicação completa (API + Blazor)"""
    print_banner("SynQcore - Aplicação Completa (API + Blazor)")

    # Verificar se estamos no diretório correto
    if not Path("SynQcore.sln").exists():
        log_error("Execute este script no diretório raiz do projeto (onde está SynQcore.sln)")
        sys.exit(1)

    # Configurar handlers de sinal
    setup_signal_handlers()

    print("🔍 Verificando estrutura do projeto...")
    is_valid, api_dir, blazor_dir = check_project_structure()
    if not is_valid:
        sys.exit(1)

    log_success("Estrutura do projeto validada!")

    # Verificar e liberar portas
    print("\n🔌 Verificando portas...")
    for port, name in [(5000, "API"), (5226, "Blazor")]:
        if is_port_in_use(port):
            log_warning(f"Porta {port} ({name}) está ocupada. Liberando...")
            kill_processes_on_port(port)

            if is_port_in_use(port):
                log_error(f"Não foi possível liberar a porta {port}")
                sys.exit(1)
            else:
                log_success(f"Porta {port} ({name}) liberada!")
        else:
            log_success(f"Porta {port} ({name}) está livre!")

    # Restaurar e compilar
    if not restore_and_build():
        sys.exit(1)

    # Iniciar serviços
    print("\n🚀 Iniciando serviços...")

    api_thread = run_api(api_dir)
    time.sleep(3)  # Dar tempo para a API iniciar primeiro

    blazor_thread = run_blazor(blazor_dir)

    # Aguardar serviços ficarem prontos
    print("\n⏳ Aguardando serviços ficarem prontos...")
    api_ready, blazor_ready = wait_for_services()

    print("\n" + "="*70)

    if api_ready and blazor_ready:
        log_success("🎉 Todas as aplicações iniciadas com sucesso!")

        print("\n📍 URLs disponíveis:")
        print(f"   {Colors.MAGENTA}🔗 API:{Colors.NC} http://localhost:5000")
        print(f"   {Colors.MAGENTA}📚 Swagger:{Colors.NC} http://localhost:5000/swagger")
        print(f"   {Colors.MAGENTA}🏥 Health:{Colors.NC} http://localhost:5000/health")
        print()
        print(f"   {Colors.CYAN}🌐 Blazor App:{Colors.NC} http://localhost:5226")
        print(f"   {Colors.CYAN}📱 Feed Principal:{Colors.NC} http://localhost:5226/")
        print(f"   {Colors.CYAN}🎨 Design System:{Colors.NC} http://localhost:5226/design-system")

        print("\n🌐 Abrindo Feed Principal automaticamente em alguns segundos...")
        open_browser_delayed()

    else:
        log_warning("Algumas aplicações podem não ter iniciado corretamente")
        if not api_ready:
            log_error("API não está respondendo")
        if not blazor_ready:
            log_error("Blazor App não está respondendo")

    print(f"\n💡 Pressione {Colors.YELLOW}Ctrl+C{Colors.NC} para parar todas as aplicações")
    print("="*70)

    try:
        # Manter o script rodando
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        pass

def show_help():
    """Mostra ajuda do comando"""
    print(__doc__)

def main():
    """Função principal"""
    # Determinar comando
    command = sys.argv[1] if len(sys.argv) > 1 else "start"

    # Verificar se estamos no diretório correto
    if not Path("SynQcore.sln").exists():
        log_error("Execute este script no diretório raiz do projeto (onde está SynQcore.sln)")
        sys.exit(1)

    # Executar comando
    if command == "start":
        start_full_application()
    elif command == "api":
        start_api_only()
    elif command == "blazor":
        start_blazor_only()
    elif command == "clean":
        clean_project()
    elif command == "docker-up":
        docker_up()
    elif command == "docker-down":
        docker_down()
    elif command == "help" or command == "--help" or command == "-h":
        show_help()
    else:
        log_error(f"Comando desconhecido: {command}")
        print()
        show_help()
        sys.exit(1)

if __name__ == "__main__":
    main()
