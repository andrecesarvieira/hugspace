#!/usr/bin/env python3
"""
SynQcore - Script Unificado de Desenvolvimento
===============================================

Script completo para iniciar a aplicação SynQcore, incluindo:
- Detecção de primeira execução
- Preparação de ambiente e dependências
- Inicialização de containers Docker
- Build e execução da aplicação
- Abertura automática do navegador

Autor: André César Vieira
Versão: 1.0.0
Data: 13 de outubro de 2025
"""

import os
import sys
import subprocess
import time
import json
import webbrowser

# Verificar e instalar dependências Python se necessário
def check_and_install_dependencies():
    """Verifica e instala dependências Python automaticamente"""
    try:
        import requests
        return True
    except ImportError:
        print("🔧 Instalando dependências Python necessárias...")
        script_dir = os.path.dirname(os.path.abspath(__file__))
        requirements_file = os.path.join(script_dir, "requirements.txt")
        
        if os.path.exists(requirements_file):
            try:
                subprocess.run([sys.executable, "-m", "pip", "install", "-r", requirements_file], 
                             check=True, capture_output=True)
                print("✅ Dependências instaladas com sucesso!")
                import requests
                return True
            except subprocess.CalledProcessError as e:
                print(f"❌ Erro instalando dependências: {e}")
                return False
        else:
            try:
                subprocess.run([sys.executable, "-m", "pip", "install", "requests", "colorama"], 
                             check=True, capture_output=True)
                print("✅ Dependências instaladas com sucesso!")
                import requests
                return True
            except subprocess.CalledProcessError as e:
                print(f"❌ Erro instalando dependências: {e}")
                return False

# Instalar dependências no início
if not check_and_install_dependencies():
    sys.exit(1)

import requests
from pathlib import Path
from typing import Dict, List, Optional, Tuple
from dataclasses import dataclass
from enum import Enum
import argparse
import signal

# Configuração de cores para terminal
class Colors:
    HEADER = '\033[95m'
    BLUE = '\033[94m'
    CYAN = '\033[96m'
    GREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

class ServiceType(Enum):
    DOCKER = "docker"
    API = "api"
    BLAZOR = "blazor"
    FULL = "full"

@dataclass
class ServiceConfig:
    name: str
    port: int
    url: str
    health_endpoint: str
    process: Optional[subprocess.Popen] = None

class SynQcoreManager:
    """Gerenciador principal do ambiente SynQcore"""
    
    def __init__(self):
        self.root_dir = Path(__file__).parent.parent.absolute()
        self.docker_dir = self.root_dir / "docker"
        self.api_dir = self.root_dir / "src" / "SynQcore.Api"
        self.blazor_dir = self.root_dir / "src" / "SynQcore.BlazorApp" / "SynQcore.BlazorApp"
        
        # Configuração dos serviços
        self.services = {
            "api": ServiceConfig(
                name="SynQcore API",
                port=5005,  # API em container usa porta 5005
                url="http://localhost:5005",
                health_endpoint="/health"
            ),
            "blazor": ServiceConfig(
                name="SynQcore Blazor",
                port=5226,
                url="http://localhost:5226",
                health_endpoint="/"  # Blazor usa a raiz como health check
            )
        }
        
        # Processos em execução
        self.running_processes: List[subprocess.Popen] = []
        
        # Configurar tratamento de sinais
        signal.signal(signal.SIGINT, self.cleanup_handler)
        signal.signal(signal.SIGTERM, self.cleanup_handler)

    def print_header(self):
        """Exibe o cabeçalho do script"""
        print(f"{Colors.CYAN}{Colors.BOLD}")
        print("=" * 80)
        print("🚀 SynQcore - Rede Social Corporativa")
        print("=" * 80)
        print(f"{Colors.ENDC}")
        print(f"{Colors.BLUE}Autor: André César Vieira")
        print(f"Versão: 1.0.0 | Data: 13 de outubro de 2025")
        print(f"Diretório: {self.root_dir}{Colors.ENDC}")
        print()

    def print_step(self, step: str, description: str):
        """Exibe um passo do processo"""
        print(f"{Colors.GREEN}📋 {step}:{Colors.ENDC} {description}")

    def print_warning(self, message: str):
        """Exibe um aviso"""
        print(f"{Colors.WARNING}⚠️  {message}{Colors.ENDC}")

    def print_error(self, message: str):
        """Exibe um erro"""
        print(f"{Colors.FAIL}❌ {message}{Colors.ENDC}")

    def print_success(self, message: str):
        """Exibe uma mensagem de sucesso"""
        print(f"{Colors.GREEN}✅ {message}{Colors.ENDC}")

    def check_prerequisites(self, require_docker: bool = True) -> bool:
        """Verifica se todas as dependências estão instaladas"""
        self.print_step("VERIFICAÇÃO", "Checando pré-requisitos do sistema")
        
        # Verificar .NET (sempre obrigatório)
        dotnet_available = False
        try:
            result = subprocess.run(["dotnet", "--version"], 
                                  capture_output=True, text=True, timeout=10)
            if result.returncode == 0:
                version = result.stdout.strip()
                print(f"  ✅ dotnet: {version}")
                dotnet_available = True
            else:
                print(f"  ❌ dotnet: erro ao verificar versão")
        except (subprocess.TimeoutExpired, FileNotFoundError):
            print(f"  ❌ dotnet: não encontrado")
        
        # Verificar Docker (condicional)
        docker_available = False
        docker_compose_available = False
        
        try:
            # Verificar se Docker Desktop está rodando
            result = subprocess.run(["docker", "info"], 
                                  capture_output=True, text=True, timeout=15)
            if result.returncode == 0:
                docker_available = True
                print(f"  ✅ docker: disponível e rodando")
            else:
                print(f"  ⚠️  docker: instalado mas não está rodando")
                print(f"      Inicie o Docker Desktop e tente novamente")
        except (subprocess.TimeoutExpired, FileNotFoundError):
            print(f"  ❌ docker: não encontrado ou não instalado")
        
        # Verificar docker-compose
        try:
            result = subprocess.run(["docker-compose", "--version"], 
                                  capture_output=True, text=True, timeout=10)
            if result.returncode == 0:
                version = result.stdout.strip()
                print(f"  ✅ docker-compose: {version}")
                docker_compose_available = True
            else:
                print(f"  ❌ docker-compose: erro ao verificar")
        except (subprocess.TimeoutExpired, FileNotFoundError):
            print(f"  ❌ docker-compose: não encontrado")
        
        # Verificar se requirements mínimos estão atendidos
        if not dotnet_available:
            self.print_error(".NET 9 SDK é obrigatório para todos os comandos")
            print("   📥 Download: https://dotnet.microsoft.com/download/dotnet/9.0")
            return False
        
        if require_docker and (not docker_available or not docker_compose_available):
            self.print_warning("Docker não está disponível - algumas funcionalidades limitadas")
            print("   📥 Docker Desktop: https://www.docker.com/products/docker-desktop")
            print("   ⚠️  Você pode executar apenas 'blazor' sem Docker")
            return False
        
        return True

    def is_first_run(self) -> bool:
        """Verifica se é a primeira execução do projeto"""
        indicators = [
            self.root_dir / "bin",
            self.root_dir / "obj", 
            self.api_dir / "bin",
            self.blazor_dir / "bin"
        ]
        
        return not any(path.exists() for path in indicators)

    def check_ports_available(self, ports: List[int]) -> Dict[int, bool]:
        """Verifica se as portas estão disponíveis"""
        import socket
        
        results = {}
        for port in ports:
            with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
                sock.settimeout(1)
                result = sock.connect_ex(('localhost', port))
                results[port] = result != 0  # True se disponível
        
        return results
    
    def check_port_responding(self, port: int) -> bool:
        """Verifica se uma porta está respondendo (sem HTTP)"""
        import socket
        
        try:
            with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
                sock.settimeout(3)
                result = sock.connect_ex(('localhost', port))
                return result == 0  # True se conectou
        except Exception:
            return False

    def clean_environment(self):
        """Limpa o ambiente de desenvolvimento"""
        self.print_step("LIMPEZA", "Removendo artefatos de build anteriores")
        
        # Usar dotnet clean para limpeza padrão
        try:
            result = subprocess.run(
                ["dotnet", "clean", "SynQcore.sln"],
                cwd=self.root_dir, 
                capture_output=True, 
                text=True
            )
            if result.returncode == 0:
                self.print_success("Limpeza dotnet concluída")
            else:
                self.print_warning("Aviso na limpeza dotnet - continuando...")
        except Exception:
            self.print_warning("dotnet clean falhou - usando limpeza manual")
        
        # Limpeza manual adicional
        import shutil
        
        clean_dirs = []
        
        # Encontrar diretórios bin e obj recursivamente
        for root, dirs, files in os.walk(self.root_dir):
            for dir_name in dirs:
                if dir_name in ["bin", "obj", "logs"]:
                    full_path = os.path.join(root, dir_name)
                    clean_dirs.append(full_path)
        
        # Remover diretórios encontrados
        if clean_dirs:
            print(f"  🧹 Limpando {len(clean_dirs)} diretórios", end="", flush=True)
            
        for dir_path in clean_dirs:
            try:
                if os.path.exists(dir_path):
                    shutil.rmtree(dir_path)
                    print(".", end="", flush=True)
            except Exception as e:
                self.print_warning(f"Não foi possível remover {dir_path}: {e}")
        
        if clean_dirs:
            print(" ✅")
        
        # Remover arquivos de log específicos
        log_patterns = ["*.log", "*.tmp"]
        for pattern in log_patterns:
            try:
                import glob
                for file_path in glob.glob(os.path.join(self.root_dir, "**", pattern), recursive=True):
                    try:
                        os.remove(file_path)
                        print(f"  🗑️  Removido: {os.path.relpath(file_path, self.root_dir)}")
                    except Exception:
                        pass
            except Exception:
                pass

    def setup_docker_infrastructure(self) -> bool:
        """Configura e inicia a infraestrutura Docker"""
        self.print_step("DOCKER", "Configurando infraestrutura (PostgreSQL + Redis)")
        
        try:
            # Verificar se Docker está rodando antes de tentar usar
            docker_check = subprocess.run(["docker", "info"], 
                                        capture_output=True, text=True, timeout=10)
            
            if docker_check.returncode != 0:
                self.print_error("Docker não está rodando. Inicie o Docker Desktop primeiro.")
                print("  🐳 Aguarde o Docker Desktop inicializar completamente")
                print("  🔄 Após inicializar, execute o comando novamente")
                return False
            
            # Verificar se docker-compose.yml existe
            compose_file = self.docker_dir / "docker-compose.yml"
            if not compose_file.exists():
                self.print_error(f"Arquivo docker-compose.yml não encontrado em {self.docker_dir}")
                return False
            
            # Verificar quais containers já estão rodando
            containers_status = {}
            try:
                ps_result = subprocess.run(["docker", "ps", "--format", "{{.Names}}:{{.Status}}"], 
                                         capture_output=True, text=True, timeout=10)
                if ps_result.returncode == 0:
                    for line in ps_result.stdout.strip().split('\n'):
                        if line and ':' in line:
                            name, status = line.split(':', 1)
                            containers_status[name] = 'Up' in status
            except Exception:
                pass
            
            # Parar apenas o container da API (pode ter sido recompilado)
            api_running = containers_status.get('synqcore-api', False)
            if api_running:
                self.print_step("PARANDO", "Container da API (recompilação detectada)")
                subprocess.run(["docker", "stop", "synqcore-api"], capture_output=True)
                subprocess.run(["docker", "rm", "synqcore-api"], capture_output=True)
            
            # Determinar quais containers precisam ser iniciados
            services_to_start = []
            
            # Verificar PostgreSQL
            if not containers_status.get('synqcore-postgres', False):
                services_to_start.append('postgres')
                
            # Verificar Redis  
            if not containers_status.get('synqcore-redis', False):
                services_to_start.append('redis')
                
            # Verificar pgAdmin
            if not containers_status.get('synqcore-pgadmin', False):
                services_to_start.append('pgadmin')
                
            # Sempre iniciar API (pode ter sido recompilada)
            services_to_start.append('api')
            
            if services_to_start:
                services_text = ', '.join(services_to_start).replace('postgres', 'PostgreSQL').replace('redis', 'Redis').replace('pgadmin', 'pgAdmin').replace('api', 'API')
                self.print_step("INICIANDO", f"{services_text}")
                result = subprocess.run(["docker-compose", "up", "-d"] + services_to_start, 
                                      cwd=self.docker_dir, capture_output=True, text=True)
            else:
                self.print_step("VERIFICANDO", "Todos os containers já estão rodando")
                result = subprocess.CompletedProcess(args=[], returncode=0, stdout="", stderr="")
            
            if result.returncode != 0:
                self.print_error(f"Erro iniciando containers: {result.stderr}")
                # Tentar diagnóstico
                self.print_step("DIAGNÓSTICO", "Verificando problemas...")
                
                # Verificar se as imagens existem
                images_result = subprocess.run(["docker", "images"], 
                                             capture_output=True, text=True)
                print(f"  📊 Imagens disponíveis: {len(images_result.stdout.splitlines())} encontradas")
                
                return False
            
            # Aguardar containers ficarem prontos
            self.print_step("AGUARDANDO", "Containers inicializando...")
            
            max_attempts = 30
            postgres_ready = False
            redis_ready = False
            pgadmin_ready = False
            api_ready = False
            
            for attempt in range(max_attempts):
                # Verificar PostgreSQL
                if not postgres_ready:
                    pg_result = subprocess.run(
                        ["docker", "exec", "synqcore-postgres", "pg_isready", "-U", "postgres"],
                        capture_output=True, text=True
                    )
                    postgres_ready = pg_result.returncode == 0
                
                # Verificar Redis
                if not redis_ready:
                    redis_result = subprocess.run(
                        ["docker", "exec", "synqcore-redis", "redis-cli", "ping"],
                        capture_output=True, text=True
                    )
                    redis_ready = redis_result.returncode == 0 and "PONG" in redis_result.stdout
                
                # Verificar pgAdmin (apenas se container está rodando)
                if not pgadmin_ready:
                    pgadmin_check = subprocess.run(
                        ["docker", "ps", "--filter", "name=synqcore-pgadmin", "--format", "{{.Status}}"],
                        capture_output=True, text=True
                    )
                    pgadmin_ready = "Up" in pgadmin_check.stdout
                
                # Verificar API (container)
                if not api_ready:
                    api_check = subprocess.run(
                        ["docker", "ps", "--filter", "name=synqcore-api", "--format", "{{.Status}}"],
                        capture_output=True, text=True
                    )
                    api_ready = "Up" in api_check.stdout
                
                if postgres_ready and redis_ready and pgadmin_ready and api_ready:
                    self.print_success("Infraestrutura Docker pronta!")
                    print("  ✅ PostgreSQL: Pronto")
                    print("  ✅ Redis: Pronto") 
                    print("  ✅ pgAdmin: Pronto")
                    print("  ✅ API: Pronto (container)")
                    return True
                
                status_msg = []
                if not postgres_ready:
                    status_msg.append("PostgreSQL")
                if not redis_ready:
                    status_msg.append("Redis")
                if not pgadmin_ready:
                    status_msg.append("pgAdmin")
                if not api_ready:
                    status_msg.append("API")
                
                print(f"  ⏳ Aguardando {', '.join(status_msg)}... ({attempt + 1}/{max_attempts})")
                time.sleep(2)
            
            self.print_warning("Timeout aguardando containers, mas continuando...")
            return True
            
        except subprocess.TimeoutExpired:
            self.print_error("Timeout executando comandos Docker")
            return False
        except subprocess.CalledProcessError as e:
            self.print_error(f"Erro configurando Docker: {e}")
            return False
        except Exception as e:
            self.print_error(f"Erro inesperado: {e}")
            return False

    def build_solution(self) -> bool:
        """Compila a solução completa"""
        self.print_step("BUILD", "Compilando solução SynQcore")
        
        try:
            # Restaurar pacotes
            print("  🔄 Restaurando pacotes", end="", flush=True)
            result = subprocess.run(
                ["dotnet", "restore", "SynQcore.sln"],
                cwd=self.root_dir, capture_output=True, text=True
            )
            print(" ✅")
            
            if result.returncode != 0:
                self.print_error(f"Erro no restore: {result.stderr}")
                return False
            
            # Build com CPU única para evitar problemas de CLR
            print("  🔨 Compilando projeto", end="", flush=True)
            result = subprocess.run(
                ["dotnet", "build", "SynQcore.sln", "--no-restore", 
                 "--maxcpucount:1", "--verbosity", "minimal"],
                cwd=self.root_dir, capture_output=True, text=True
            )
            print(" ✅")
            
            if result.returncode != 0:
                self.print_error(f"Erro no build: {result.stderr}")
                return False
            
            self.print_success("Build concluído com sucesso!")
            return True
            
        except Exception as e:
            self.print_error(f"Erro durante o build: {e}")
            return False

    def check_migrations_needed(self) -> bool:
        """Verifica se há migrações pendentes baseado em arquivo marker"""
        try:
            # Arquivo marker que indica quando migrações foram aplicadas pela última vez
            migrations_marker = self.root_dir / ".migrations_applied"
            
            # Se não existe o arquivo marker, assumir que precisa
            if not migrations_marker.exists():
                return True
                
            # Verificar se há arquivos de migração mais novos que o marker
            migrations_dir = self.api_dir / "Migrations"
            if migrations_dir.exists():
                marker_time = migrations_marker.stat().st_mtime
                for migration_file in migrations_dir.glob("*.cs"):
                    if migration_file.stat().st_mtime > marker_time:
                        return True
                        
            return False
            
        except Exception:
            # Em caso de erro, aplicar migrações por segurança
            return True

    def apply_database_migrations(self, force: bool = False) -> bool:
        """Aplica migrações do banco de dados apenas quando necessário"""
        
        # Verificar se migrações são necessárias (exceto quando forçado)
        if not force and not self.check_migrations_needed():
            self.print_step("MIGRATIONS", "Banco de dados já está atualizado")
            return True
            
        self.print_step("MIGRATIONS", "Aplicando migrações do banco de dados")
        print("  💾 Atualizando banco", end="", flush=True)
        
        try:
            result = subprocess.run(
                ["dotnet", "ef", "database", "update"],
                cwd=self.api_dir, capture_output=True, text=True
            )
            print(" ✅")
            
            if result.returncode != 0:
                self.print_warning("EF Core tools não encontrados ou erro nas migrações")
                self.print_warning("Execute manualmente: dotnet ef database update")
                return True  # Não falhar por causa das migrações
            
            self.print_success("Migrações aplicadas!")
            
            # Criar arquivo marker indicando que migrações foram aplicadas
            try:
                migrations_marker = self.root_dir / ".migrations_applied"
                migrations_marker.touch()
            except Exception:
                pass  # Não falhar se não conseguir criar o marker
                
            return True
            
        except Exception as e:
            self.print_warning(f"Aviso nas migrações: {e}")
            return True  # Não falhar por causa das migrações

    def start_api_service(self) -> subprocess.Popen:
        """Inicia o serviço da API"""
        self.print_step("API", "Iniciando SynQcore API na porta 5000")
        print("  🔧 Preparando API", end="", flush=True)
        
        env = os.environ.copy()
        env["ASPNETCORE_ENVIRONMENT"] = "Development"
        env["ASPNETCORE_URLS"] = "http://localhost:5000"
        
        process = subprocess.Popen(
            ["dotnet", "run", "--project", str(self.api_dir)],
            env=env,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
            bufsize=1,
            universal_newlines=True
        )
        print(" ✅")
        
        self.running_processes.append(process)
        return process

    def start_blazor_service(self) -> subprocess.Popen | None:
        """Inicia o serviço Blazor"""
        self.print_step("BLAZOR", "Iniciando SynQcore Blazor na porta 5226")
        
        # Verificar se o diretório existe
        if not self.blazor_dir.exists():
            self.print_error(f"Diretório Blazor não encontrado: {self.blazor_dir}")
            return None
        
        print("  🚀 Preparando Blazor", end="", flush=True)
        
        env = os.environ.copy()
        env["ASPNETCORE_ENVIRONMENT"] = "Development"
        env["ASPNETCORE_URLS"] = "http://localhost:5226"
        
        print(f"    📁 Diretório: {self.blazor_dir}")
        print(f"    🌐 URL: {env['ASPNETCORE_URLS']}")
        
        try:
            # Usar o mesmo método que funciona manualmente
            process = subprocess.Popen(
                ["dotnet", "run", "--urls", "http://localhost:5226"],
                cwd=str(self.blazor_dir),  # Definir working directory
                env=env,
                # Não capturar output para evitar deadlock
                stdout=None,
                stderr=None
            )
            print(" ✅")
            
            # Aguardar um pouco e verificar se o processo está vivo
            time.sleep(3)  # Dar mais tempo para inicialização
            if process.poll() is not None:
                self.print_error("Processo Blazor falhou ao iniciar")
                return None
            
            self.running_processes.append(process)
            print(f"    ✅ Processo iniciado (PID: {process.pid})")
            return process
            
        except Exception as e:
            self.print_error(f"Erro ao iniciar Blazor: {e}")
            return None
        return process

    def wait_for_service_health(self, service_name: str, timeout: int = 60) -> bool:
        """Aguarda um serviço ficar saudável"""
        service = self.services.get(service_name)
        if not service:
            return False
        
        self.print_step("AGUARDANDO", f"{service.name} inicializar...")
        
        for attempt in range(timeout):
            try:
                response = requests.get(f"{service.url}{service.health_endpoint}", timeout=5)
                
                # Para API, aceitar apenas status 200
                if service_name == "api" and response.status_code == 200:
                    self.print_success(f"{service.name} pronto em {service.url}")
                    return True
                
                # Para Blazor, aceitar qualquer resposta HTTP válida ou conteúdo HTML
                elif service_name == "blazor" and (
                    response.status_code in [200, 302, 404] or 
                    'html' in response.headers.get('content-type', '').lower() or
                    'text/html' in response.headers.get('content-type', '') or
                    b'<!DOCTYPE html>' in response.content[:100].lower()
                ):
                    self.print_success(f"{service.name} pronto em {service.url}")
                    return True
                    
            except requests.RequestException as e:
                # Para Blazor, tentar verificação de porta como fallback
                if service_name == "blazor":
                    if self.check_port_responding(service.port):
                        self.print_success(f"{service.name} pronto em {service.url} (verificação de porta)")
                        return True
                
                # Para debug, mostrar erro a cada 10 tentativas
                if attempt % 10 == 0 and attempt > 0:
                    print(f"    💭 Aguardando [{service_name}]: {service.url}{service.health_endpoint}")
            
            if attempt < timeout - 1:
                # Indicador de progresso simples com pontinhos
                dots = "." * (attempt % 4)
                if attempt == 0:
                    print(f"  ⏳ Conectando{dots}", end="", flush=True)
                else:
                    print(".", end="", flush=True)
                
                # A cada 15 segundos, mostrar progresso
                if attempt > 0 and attempt % 15 == 0:
                    print(f" ({attempt}s)", end="", flush=True)
                
                time.sleep(1)
        
        print()  # Nova linha após tentativas
        
        return False

    def open_browser_urls(self, urls: List[str]):
        """Abre URLs no navegador padrão"""
        self.print_step("NAVEGADOR", "Abrindo aplicação no navegador")
        
        # Aguardar um pouco para garantir que os serviços estão prontos
        time.sleep(3)
        
        for url in urls:
            try:
                webbrowser.open(url)
                print(f"  🌐 Aberto: {url}")
            except Exception as e:
                self.print_warning(f"Não foi possível abrir {url}: {e}")

    def monitor_services(self):
        """Monitora os serviços em execução"""
        print(f"\n{Colors.CYAN}{Colors.BOLD}🎯 MONITORAMENTO DE SERVIÇOS{Colors.ENDC}")
        print("=" * 60)
        print("📊 URLs disponíveis:")
        print("   • Blazor App: http://localhost:5226 (Interface Principal)")
        print("   • API: http://localhost:5005 (Container - Swagger: /swagger)")
        print("   • pgAdmin: http://localhost:8080 (admin@synqcore.dev / SynQcore@Dev123!)")
        print("\n🔐 Credenciais padrão:")
        print("   • Email: admin@synqcore.com")
        print("   • Senha: SynQcore@Admin123!")
        print(f"\n{Colors.WARNING}Pressione Ctrl+C para parar todos os serviços{Colors.ENDC}")
        print("=" * 60)
        
        try:
            while True:
                # Verificar se processos ainda estão rodando
                active_processes = []
                for process in self.running_processes:
                    if process.poll() is None:
                        active_processes.append(process)
                    else:
                        self.print_warning(f"Processo finalizou com código: {process.returncode}")
                
                if not active_processes:
                    self.print_error("Todos os processos finalizaram!")
                    break
                
                time.sleep(5)
                
        except KeyboardInterrupt:
            print(f"\n{Colors.CYAN}Recebido sinal de interrupção. Finalizando serviços...{Colors.ENDC}")

    def cleanup_handler(self, signum, frame):
        """Handler para limpeza quando recebe sinal de interrupção"""
        self.cleanup_processes(keep_docker=True)
        sys.exit(0)

    def cleanup_processes(self, keep_docker: bool = True):
        """Finaliza todos os processos em execução"""
        if self.running_processes:
            self.print_step("FINALIZANDO", "Parando aplicações (.NET)")
            
            for process in self.running_processes:
                if process.poll() is None:
                    try:
                        process.terminate()
                        # Aguardar término gracioso
                        process.wait(timeout=10)
                    except subprocess.TimeoutExpired:
                        process.kill()
                    except Exception:
                        pass
            
            self.running_processes.clear()
            
            if keep_docker:
                self.print_success("Aplicações .NET finalizadas - Docker mantido em execução")
                print("  🐳 Containers Docker continuam rodando")
                print("  📊 Use 'docker-down' para parar a infraestrutura")
                self.show_docker_status()
            else:
                self.print_success("Todos os serviços foram finalizados")

    def show_docker_status(self):
        """Mostra o status atual dos containers Docker com detalhes melhorados"""
        try:
            result = subprocess.run(
                ["docker", "ps", "--format", "table {{.Names}}\t{{.Status}}\t{{.Ports}}"],
                cwd=self.docker_dir, 
                capture_output=True, 
                text=True,
                timeout=10
            )
            
            if result.returncode == 0 and result.stdout.strip():
                print(f"\n  📊 Status dos containers:")
                lines = result.stdout.strip().split('\n')
                synqcore_containers = []
                
                for line in lines:
                    if 'synqcore' in line.lower():
                        # Adicionar ícones de status baseado no status do container
                        if 'healthy' in line.lower() or '(healthy)' in line.lower():
                            status_icon = "🟢"
                        elif 'unhealthy' in line.lower():
                            status_icon = "🟡"
                        elif 'up' in line.lower():
                            status_icon = "🔵"
                        elif 'exited' in line.lower():
                            status_icon = "🔴"
                        else:
                            status_icon = "⚪"
                        
                        print(f"     {status_icon} {line}")
                        synqcore_containers.append(line)
                        
                if synqcore_containers:
                    print(f"  🌐 Serviços disponíveis:")
                    print(f"     • PostgreSQL: localhost:5432")
                    print(f"     • Redis: localhost:6379") 
                    print(f"     • pgAdmin: http://localhost:8080 (admin@synqcore.dev)")
                    
                    # Verificar se API está rodando também
                    if any('synqcore-api' in container for container in synqcore_containers):
                        print(f"     • API Swagger: http://localhost:5005/swagger")
                else:
                    print(f"     ⚪ Nenhum container SynQcore encontrado")
                
        except Exception:
            pass  # Falha silenciosa se não conseguir verificar

    def check_system_integrity(self):
        """Verifica a integridade completa do sistema SynQcore"""
        print(f"\n{Colors.CYAN}🔍 VERIFICANDO INTEGRIDADE DO SISTEMA SYNQCORE{Colors.ENDC}")
        print("=" * 60)
        
        all_checks_passed = True
        
        # 1. Verificar pré-requisitos do sistema
        print(f"\n{Colors.WARNING}📋 Verificando pré-requisitos:{Colors.ENDC}")
        
        # .NET SDK
        try:
            result = subprocess.run(["dotnet", "--version"], capture_output=True, text=True, timeout=10)
            if result.returncode == 0:
                version = result.stdout.strip()
                print(f"  ✅ .NET SDK: {version}")
            else:
                print(f"  ❌ .NET SDK: Não encontrado")
                all_checks_passed = False
        except Exception:
            print(f"  ❌ .NET SDK: Erro na verificação")
            all_checks_passed = False
            
        # Docker
        try:
            result = subprocess.run(["docker", "--version"], capture_output=True, text=True, timeout=10)
            if result.returncode == 0:
                version = result.stdout.strip()
                print(f"  ✅ Docker: {version}")
            else:
                print(f"  ❌ Docker: Não encontrado")
                all_checks_passed = False
        except Exception:
            print(f"  ❌ Docker: Erro na verificação")
            all_checks_passed = False
            
        # Python
        print(f"  ✅ Python: {sys.version.split()[0]}")
        
        # 2. Verificar estrutura de arquivos
        print(f"\n{Colors.WARNING}📁 Verificando estrutura de arquivos:{Colors.ENDC}")
        
        critical_files = [
            ("SynQcore.sln", "Solução principal"),
            ("src/SynQcore.Api/SynQcore.Api.csproj", "Projeto API"),
            ("src/SynQcore.BlazorApp/SynQcore.BlazorApp/SynQcore.BlazorApp.csproj", "Projeto Blazor"),
            ("docker/docker-compose.yml", "Configuração Docker"),
            ("src/SynQcore.Infrastructure/Data/SynQcoreDbContext.cs", "Context do banco"),
        ]
        
        for file_path, description in critical_files:
            full_path = self.root_dir / file_path
            if full_path.exists():
                print(f"  ✅ {description}: {file_path}")
            else:
                print(f"  ❌ {description}: {file_path} (não encontrado)")
                all_checks_passed = False
        
        # 3. Verificar containers Docker
        print(f"\n{Colors.WARNING}🐳 Verificando containers Docker:{Colors.ENDC}")
        try:
            result = subprocess.run(
                ["docker", "ps", "-a", "--format", "{{.Names}}\t{{.Status}}"],
                capture_output=True, text=True, timeout=10
            )
            
            if result.returncode == 0:
                containers = result.stdout.strip().split('\n')
                required_containers = ['synqcore-postgres', 'synqcore-redis', 'synqcore-pgadmin']
                
                for container_name in required_containers:
                    found = any(container_name in line for line in containers if line.strip())
                    if found:
                        status_line = next((line for line in containers if container_name in line), "")
                        if "Up" in status_line:
                            print(f"  🟢 {container_name}: Rodando")
                        else:
                            print(f"  🟡 {container_name}: Parado")
                    else:
                        print(f"  ❌ {container_name}: Não encontrado")
                        
            else:
                print(f"  ⚠️  Não foi possível verificar containers")
                
        except Exception as e:
            print(f"  ❌ Erro ao verificar Docker: {e}")
            
        # 4. Verificar conectividade de rede
        print(f"\n{Colors.WARNING}🌐 Verificando conectividade:{Colors.ENDC}")
        
        endpoints = [
            ("http://localhost:5005/health", "API Health"),
            ("http://localhost:5226", "Blazor App"),
            ("http://localhost:8080", "pgAdmin"),
        ]
        
        print("  🌐 Testando conexões", end="", flush=True)
        for url, name in endpoints:
            try:
                response = requests.get(url, timeout=5)
                if response.status_code < 400:
                    print(".", end="", flush=True)
                else:
                    print("!", end="", flush=True)
            except requests.exceptions.ConnectionError:
                print("x", end="", flush=True)
            except Exception as e:
                print("?", end="", flush=True)
        print("")
        
        # Mostrar resultados detalhados
        for url, name in endpoints:
            try:
                response = requests.get(url, timeout=5)
                if response.status_code < 400:
                    print(f"  🟢 {name}: Acessível ({response.status_code})")
                else:
                    print(f"  🟡 {name}: Resposta {response.status_code}")
            except requests.exceptions.ConnectionError:
                print(f"  🔴 {name}: Não acessível")
            except Exception as e:
                print(f"  ⚠️  {name}: Erro na verificação")
        
        # 5. Resumo final
        print(f"\n{Colors.WARNING}📊 RESUMO DA VERIFICAÇÃO:{Colors.ENDC}")
        if all_checks_passed:
            print(f"  🎉 {Colors.GREEN}Sistema íntegro - Todos os componentes OK{Colors.ENDC}")
            print(f"  💡 Execute: {Colors.CYAN}./synqcore start{Colors.ENDC} para iniciar")
        else:
            print(f"  ⚠️  {Colors.WARNING}Problemas encontrados - Verifique os itens acima{Colors.ENDC}")
            print(f"  💡 Execute: {Colors.CYAN}./synqcore docker-up{Colors.ENDC} para configurar infraestrutura")
            
        print("=" * 60)

    def restart_api_container(self) -> bool:
        """Reinicia apenas o container da API (para recompilações)"""
        try:
            self.print_step("REINICIANDO", "Container da API")
            
            # Parar e remover container da API
            subprocess.run(["docker", "stop", "synqcore-api"], capture_output=True)
            subprocess.run(["docker", "rm", "synqcore-api"], capture_output=True)
            
            # Reiniciar apenas a API
            result = subprocess.run(["docker-compose", "up", "-d", "api"], 
                                  cwd=self.docker_dir, capture_output=True, text=True)
            
            if result.returncode != 0:
                self.print_error(f"Erro reiniciando API: {result.stderr}")
                return False
                
            # Aguardar API ficar pronta
            if self.wait_for_service_health("api", timeout=30):
                return True
                
            self.print_warning("API reiniciada mas health check falhou")
            return True
            
        except Exception as e:
            self.print_error(f"Erro reiniciando API: {e}")
            return False

    def start_full_application(self):
        """Inicia a aplicação completa"""
        try:
            # 1. Verificar pré-requisitos (Docker obrigatório para aplicação completa)
            if not self.check_prerequisites(require_docker=True):
                return False
            
            # 2. Verificar se é primeira execução
            first_run = self.is_first_run()
            if first_run:
                self.print_step("PRIMEIRA EXECUÇÃO", "Detectada primeira execução - preparando ambiente")
            
            # 3. Verificar portas disponíveis
            ports_status = self.check_ports_available([5000, 5226, 5432, 6379])
            occupied_ports = [port for port, available in ports_status.items() if not available]
            
            if occupied_ports:
                self.print_warning(f"Portas ocupadas: {occupied_ports}")
                print("Verifique se outros processos estão usando essas portas.")
            
            # 4. Limpeza se necessário
            if first_run:
                self.clean_environment()
            
            # 5. Build apenas na primeira execução
            if first_run:
                if not self.build_solution():
                    return False
            
            # 6. Configurar Docker
            if not self.setup_docker_infrastructure():
                return False
            
            # 7. Aplicar migrações apenas na primeira execução ou quando necessário
            if first_run:
                self.apply_database_migrations(force=True)  # Forçar na primeira execução
            else:
                self.apply_database_migrations(force=False)  # Verificar se necessário
            
            # 8. Aguardar API container ficar pronto
            if not self.wait_for_service_health("api", 90):
                self.print_warning("API container demorou mais que esperado")
                # Continuar mesmo assim pois API pode estar inicializando
            
            # 9. Iniciar Blazor
            blazor_process = self.start_blazor_service()
            
            # 10. Aguardar Blazor ficar pronto
            if not self.wait_for_service_health("blazor", 60):
                # Verificar se o processo ainda está rodando
                if blazor_process is not None:
                    blazor_running = blazor_process.poll() is None
                    if blazor_running:
                        self.print_warning("Blazor não respondeu ao health check, mas processo está rodando")
                        self.print_warning("Continuando - Blazor pode estar inicializando ainda...")
                    else:
                        self.print_error("Blazor não ficou pronto e processo finalizou")
                        return False
                else:
                    self.print_error("Blazor não foi iniciado corretamente")
                    return False
            
            # 11. Abrir navegador - APENAS Blazor para start completo
            self.open_browser_urls([
                "http://localhost:5226"  # Blazor como página principal
            ])
            
            # 13. Monitorar serviços
            self.monitor_services()
            
            return True
            
        except KeyboardInterrupt:
            print(f"\n{Colors.CYAN}Interrompido pelo usuário{Colors.ENDC}")
            return False
        except Exception as e:
            self.print_error(f"Erro inesperado: {e}")
            return False
        finally:
            self.cleanup_processes(keep_docker=True)

    def start_api_only(self):
        """Inicia apenas a API em container"""
        try:
            # API precisa de Docker para PostgreSQL
            if not self.check_prerequisites(require_docker=True):
                return False
            
            if not self.setup_docker_infrastructure():
                return False
            
            # Build é feito pelo Docker
            # Aplicar migrações apenas se necessário
            self.apply_database_migrations(force=False)
            
            if not self.wait_for_service_health("api", 90):
                self.print_warning("API container demorou mais que esperado, mas pode estar funcionando")
            
            self.open_browser_urls(["http://localhost:5005/swagger"])
            
            print(f"\n{Colors.GREEN}🎯 API executando em: http://localhost:5005 (container){Colors.ENDC}")
            print(f"{Colors.CYAN}Pressione Ctrl+C para parar{Colors.ENDC}")
            
            # Monitorar container da API
            try:
                while True:
                    result = subprocess.run(
                        ["docker", "ps", "--filter", "name=synqcore-api", "--format", "{{.Status}}"],
                        capture_output=True, text=True
                    )
                    if "Up" not in result.stdout:
                        self.print_error("Container da API parou de funcionar!")
                        break
                    time.sleep(5)
            except KeyboardInterrupt:
                pass
            
            return True
            
        finally:
            # API está em container, não precisa cleanup de processos
            pass

    def start_blazor_only(self):
        """Inicia apenas o Blazor"""
        try:
            # Blazor pode rodar sem Docker (modo standalone)
            if not self.check_prerequisites(require_docker=False):
                return False
            
            # Build apenas na primeira execução
            first_run = self.is_first_run()
            if first_run:
                if not self.build_solution():
                    return False
            
            blazor_process = self.start_blazor_service()
            
            if not self.wait_for_service_health("blazor", 60):
                self.print_error("Blazor não ficou pronto no tempo esperado")
                return False
            
            self.open_browser_urls(["http://localhost:5226"])
            
            print(f"\n{Colors.GREEN}🎯 Blazor executando em: http://localhost:5226{Colors.ENDC}")
            print(f"{Colors.CYAN}Pressione Ctrl+C para parar{Colors.ENDC}")
            
            # Monitorar apenas o Blazor
            try:
                if blazor_process is not None:
                    blazor_process.wait()
                else:
                    print("\n❌ Nenhum processo Blazor para monitorar")
            except KeyboardInterrupt:
                pass
            
            return True
            
        finally:
            self.cleanup_processes(keep_docker=False)  # Blazor standalone não precisa manter Docker

    def docker_up(self):
        """Sobe apenas a infraestrutura Docker"""
        try:
            self.print_step("DOCKER UP", "Iniciando infraestrutura completa")
            print("  🐳 Subindo containers", end="", flush=True)
            
            result = subprocess.run(
                ["docker-compose", "up", "-d"],
                cwd=self.docker_dir,
                capture_output=True,
                text=True
            )
            print(" ✅")
            
            if result.returncode != 0:
                self.print_error(f"Erro iniciando Docker: {result.stderr}")
                return False
            
            self.print_success("Infraestrutura Docker iniciada!")
            print("📊 Serviços disponíveis:")
            print("   • PostgreSQL: localhost:5432")
            print("   • Redis: localhost:6379") 
            print("   • pgAdmin: http://localhost:8080")
            
            return True
            
        except Exception as e:
            self.print_error(f"Erro: {e}")
            return False

    def docker_down(self):
        """Para a infraestrutura Docker"""
        try:
            self.print_step("DOCKER DOWN", "Parando infraestrutura Docker")
            
            result = subprocess.run(
                ["docker-compose", "down"],
                cwd=self.docker_dir,
                capture_output=True,
                text=True
            )
            
            if result.returncode != 0:
                self.print_warning(f"Aviso parando Docker: {result.stderr}")
            
            self.print_success("Infraestrutura Docker parada!")
            return True
            
        except Exception as e:
            self.print_error(f"Erro: {e}")
            return False

    def check_docker_status(self):
        """Verifica status detalhado do Docker"""
        print(f"\n{Colors.CYAN}{Colors.BOLD}🐳 DIAGNÓSTICO DO DOCKER{Colors.ENDC}")
        print("=" * 50)
        
        # Verificar se Docker está instalado
        try:
            result = subprocess.run(["docker", "--version"], 
                                  capture_output=True, text=True, timeout=10)
            if result.returncode == 0:
                print(f"  ✅ Docker instalado: {result.stdout.strip()}")
            else:
                print(f"  ❌ Docker não responde adequadamente")
                return
        except (subprocess.TimeoutExpired, FileNotFoundError):
            print(f"  ❌ Docker não está instalado")
            print(f"  📥 Baixe em: https://www.docker.com/products/docker-desktop")
            return
        
        # Verificar se Docker daemon está rodando
        try:
            result = subprocess.run(["docker", "info"], 
                                  capture_output=True, text=True, timeout=15)
            if result.returncode == 0:
                print(f"  ✅ Docker Desktop está rodando")
                
                # Mostrar informações básicas
                lines = result.stdout.split('\n')
                for line in lines[:10]:  # Primeiras 10 linhas
                    if 'Server Version' in line or 'Total Memory' in line or 'CPUs' in line:
                        print(f"  📊 {line.strip()}")
            else:
                print(f"  ❌ Docker Desktop não está rodando")
                print(f"  🔄 Inicie o Docker Desktop e aguarde completar")
        except subprocess.TimeoutExpired:
            print(f"  ⚠️  Docker Desktop pode estar inicializando...")
            print(f"  ⏳ Aguarde alguns minutos e tente novamente")
        
        # Verificar docker-compose
        try:
            result = subprocess.run(["docker-compose", "--version"], 
                                  capture_output=True, text=True, timeout=10)
            if result.returncode == 0:
                print(f"  ✅ Docker Compose: {result.stdout.strip()}")
            else:
                print(f"  ❌ Docker Compose com problemas")
        except (subprocess.TimeoutExpired, FileNotFoundError):
            print(f"  ❌ Docker Compose não encontrado")
    
    def show_help(self):
        """Exibe a ajuda do script"""
        print(f"{Colors.CYAN}{Colors.BOLD}SynQcore - Script Unificado{Colors.ENDC}")
        print("=" * 50)
        print("\n📋 Comandos disponíveis:")
        print(f"  {Colors.GREEN}start{Colors.ENDC}       - Aplicação completa (API + Blazor + Docker)")
        print(f"  {Colors.GREEN}api{Colors.ENDC}         - Apenas API na porta 5000")
        print(f"  {Colors.GREEN}blazor{Colors.ENDC}      - Apenas Blazor na porta 5226")
        print(f"  {Colors.GREEN}build{Colors.ENDC}       - Compilar solução completa")
        print(f"  {Colors.GREEN}migrate{Colors.ENDC}     - Aplicar migrações do banco")
        print(f"  {Colors.GREEN}clean{Colors.ENDC}       - Limpeza completa do projeto")
        print(f"  {Colors.GREEN}docker-up{Colors.ENDC}   - Infraestrutura Docker")
        print(f"  {Colors.GREEN}docker-down{Colors.ENDC} - Parar Docker")
        print(f"  {Colors.GREEN}docker-status{Colors.ENDC} - Verificar status do Docker")
        print(f"  {Colors.GREEN}check{Colors.ENDC}       - Verificar integridade do sistema")
        print(f"  {Colors.GREEN}help{Colors.ENDC}        - Esta ajuda")
        
        print(f"\n🔧 Uso:")
        print(f"  python scripts/synqcore.py [comando]")
        print(f"  ./synqcore [comando]  (se configurado)")
        
        print(f"\n🌐 URLs após inicialização:")
        print("  • Blazor: http://localhost:5226")
        print("  • API: http://localhost:5000/swagger")
        print("  • pgAdmin: http://localhost:8080")
        
        print(f"\n🔐 Credenciais padrão:")
        print("  • Email: admin@synqcore.com")
        print("  • Senha: SynQcore@Admin123!")
        
        print(f"\n💡 Dicas:")
        print("  • Use 'blazor' se não tiver Docker instalado") 
        print("  • Use 'docker-status' para diagnosticar problemas")
        print("  • Use 'clean' se houver problemas de compilação")
        print("  • Containers Docker são mantidos entre execuções")
        print("  • Use 'docker-down' apenas quando necessário")

def main():
    """Função principal do script"""
    parser = argparse.ArgumentParser(
        description="SynQcore - Script unificado de desenvolvimento",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Exemplos:
  python synqcore.py start         # Aplicação completa
  python synqcore.py api           # Apenas API
  python synqcore.py blazor        # Apenas Blazor
  python synqcore.py build         # Compilar solução  
  python synqcore.py migrate       # Aplicar migrações
  python synqcore.py clean         # Limpeza completa
        """
    )
    
    parser.add_argument(
        'command',
        nargs='?',
        default='start',
        choices=['start', 'api', 'blazor', 'build', 'migrate', 'clean', 'docker-up', 'docker-down', 'docker-status', 'check', 'help'],
        help='Comando a executar'
    )
    
    args = parser.parse_args()
    
    manager = SynQcoreManager()
    manager.print_header()
    
    success = False
    
    if args.command == 'help':
        manager.show_help()
        success = True
    elif args.command == 'start':
        success = manager.start_full_application()
    elif args.command == 'api':
        success = manager.start_api_only()
    elif args.command == 'blazor':
        success = manager.start_blazor_only()
    elif args.command == 'build':
        success = manager.build_solution()
    elif args.command == 'migrate':
        success = manager.apply_database_migrations(force=True)
    elif args.command == 'clean':
        manager.clean_environment()
        success = True
    elif args.command == 'docker-up':
        success = manager.docker_up()
    elif args.command == 'docker-down':
        success = manager.docker_down()
    elif args.command == 'docker-status':
        manager.check_docker_status()
        success = True
    elif args.command == 'check':
        manager.check_system_integrity()
        success = True
    
    if success:
        print(f"\n{Colors.GREEN}🎉 Operação concluída com sucesso!{Colors.ENDC}")
    else:
        print(f"\n{Colors.FAIL}❌ Operação falhou. Verifique os logs acima.{Colors.ENDC}")
        sys.exit(1)

if __name__ == "__main__":
    main()