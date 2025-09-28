"""
SynQcore API Test Utilities

Funções utilitárias para testes de API do SynQcore.
Inclui autenticação, logging, formatação e helpers comuns.
"""

import os
import json
import time
import logging
from datetime import datetime
from typing import Dict, Any, Optional, List
from dataclasses import dataclass
import requests
from colorama import Fore, Style, init
from tabulate import tabulate
from dotenv import load_dotenv

# Inicializar colorama e carregar variáveis de ambiente
init(autoreset=True)
load_dotenv()

@dataclass
class TestResult:
    """Resultado de um teste de API"""
    test_name: str
    endpoint: str
    method: str
    status_code: int
    response_time: float
    success: bool
    error_message: Optional[str] = None
    response_data: Optional[Dict] = None

class APITestClient:
    """Cliente de teste para API do SynQcore"""

    def __init__(self):
        self.base_url = os.getenv('API_BASE_URL', 'http://localhost:5000')
        self.timeout = int(os.getenv('REQUEST_TIMEOUT', '30'))
        self.token = None
        self.session = requests.Session()
        self.results: List[TestResult] = []

        # Configurar logging
        logging.basicConfig(
            level=getattr(logging, os.getenv('LOG_LEVEL', 'INFO')),
            format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
            handlers=[
                logging.FileHandler(os.getenv('LOG_FILE', 'test_results.log')),
                logging.StreamHandler()
            ]
        )
        self.logger = logging.getLogger('SynQcore-API-Tests')

    def authenticate(self) -> bool:
        """Autentica com credenciais padrão e obtém token JWT"""
        try:
            auth_data = {
                "email": os.getenv('TEST_EMAIL', 'admin@synqcore.com'),
                "password": os.getenv('TEST_PASSWORD', 'SynQcore@Admin123!')
            }

            response = self.session.post(
                f"{self.base_url}/api/auth/login",
                json=auth_data,
                timeout=self.timeout
            )

            if response.status_code == 200:
                data = response.json()
                self.token = data.get('token')
                if self.token:
                    self.session.headers.update({
                        'Authorization': f'Bearer {self.token}'
                    })
                    self.log_success("Autenticação realizada com sucesso")
                    return True

            self.log_error(f"Falha na autenticação: {response.status_code} - {response.text}")
            return False

        except Exception as e:
            self.log_error(f"Erro na autenticação: {str(e)}")
            return False

    def make_request(self, method: str, endpoint: str, data: Optional[Dict] = None,
                    files: Optional[Dict] = None, params: Optional[Dict] = None) -> TestResult:
        """Faz uma requisição HTTP e retorna o resultado formatado"""

        url = f"{self.base_url}{endpoint}"
        start_time = time.time()

        try:
            if method.upper() == 'GET':
                response = self.session.get(url, params=params, timeout=self.timeout)
            elif method.upper() == 'POST':
                if files:
                    response = self.session.post(url, data=data, files=files, timeout=self.timeout)
                else:
                    response = self.session.post(url, json=data, timeout=self.timeout)
            elif method.upper() == 'PUT':
                response = self.session.put(url, json=data, timeout=self.timeout)
            elif method.upper() == 'DELETE':
                response = self.session.delete(url, timeout=self.timeout)
            else:
                raise ValueError(f"Método HTTP não suportado: {method}")

            response_time = round((time.time() - start_time) * 1000, 2)  # em ms

            # Tentar parsear JSON da resposta
            try:
                response_data = response.json() if response.text else None
            except:
                response_data = {"raw_response": response.text[:500]}  # Primeiros 500 chars

            success = 200 <= response.status_code < 300
            error_message = None if success else f"HTTP {response.status_code}: {response.text[:200]}"

            result = TestResult(
                test_name=f"{method.upper()} {endpoint}",
                endpoint=endpoint,
                method=method.upper(),
                status_code=response.status_code,
                response_time=response_time,
                success=success,
                error_message=error_message,
                response_data=response_data
            )

            self.results.append(result)
            return result

        except Exception as e:
            response_time = round((time.time() - start_time) * 1000, 2)
            result = TestResult(
                test_name=f"{method.upper()} {endpoint}",
                endpoint=endpoint,
                method=method.upper(),
                status_code=0,
                response_time=response_time,
                success=False,
                error_message=str(e)
            )
            self.results.append(result)
            return result

    def log_success(self, message: str):
        """Log de sucesso com cor verde"""
        print(f"{Fore.GREEN}✅ {message}{Style.RESET_ALL}")
        self.logger.info(f"SUCCESS: {message}")

    def log_error(self, message: str):
        """Log de erro com cor vermelha"""
        print(f"{Fore.RED}❌ {message}{Style.RESET_ALL}")
        self.logger.error(f"ERROR: {message}")

    def log_warning(self, message: str):
        """Log de aviso com cor amarela"""
        print(f"{Fore.YELLOW}⚠️  {message}{Style.RESET_ALL}")
        self.logger.warning(f"WARNING: {message}")

    def log_info(self, message: str):
        """Log de informação com cor azul"""
        print(f"{Fore.BLUE}ℹ️  {message}{Style.RESET_ALL}")
        self.logger.info(f"INFO: {message}")

    def print_test_result(self, result: TestResult):
        """Imprime resultado de teste formatado"""
        status_color = Fore.GREEN if result.success else Fore.RED
        status_icon = "✅" if result.success else "❌"

        print(f"{status_color}{status_icon} {result.test_name}{Style.RESET_ALL}")
        print(f"   Status: {result.status_code} | Tempo: {result.response_time}ms")

        if result.error_message:
            print(f"   {Fore.RED}Erro: {result.error_message}{Style.RESET_ALL}")

        if result.response_data and isinstance(result.response_data, dict):
            if len(str(result.response_data)) < 200:
                print(f"   Resposta: {json.dumps(result.response_data, indent=2)[:200]}...")

        print()  # Linha em branco

    def generate_summary_report(self) -> Dict[str, Any]:
        """Gera relatório resumido dos testes"""
        if not self.results:
            return {"message": "Nenhum teste executado"}

        total_tests = len(self.results)
        successful_tests = sum(1 for r in self.results if r.success)
        failed_tests = total_tests - successful_tests
        avg_response_time = sum(r.response_time for r in self.results) / total_tests

        # Agrupar por código de status
        status_codes = {}
        for result in self.results:
            code = result.status_code
            if code in status_codes:
                status_codes[code] += 1
            else:
                status_codes[code] = 1

        return {
            "total_tests": total_tests,
            "successful_tests": successful_tests,
            "failed_tests": failed_tests,
            "success_rate": round((successful_tests / total_tests) * 100, 2),
            "average_response_time": round(avg_response_time, 2),
            "status_codes": status_codes,
            "timestamp": datetime.now().isoformat()
        }

    def print_summary_report(self):
        """Imprime relatório resumido formatado"""
        summary = self.generate_summary_report()

        print(f"\n{Fore.CYAN}{'='*60}")
        print(f"📊 RELATÓRIO RESUMIDO DOS TESTES")
        print(f"{'='*60}{Style.RESET_ALL}")

        print(f"📈 Total de Testes: {summary['total_tests']}")
        print(f"✅ Sucessos: {Fore.GREEN}{summary['successful_tests']}{Style.RESET_ALL}")
        print(f"❌ Falhas: {Fore.RED}{summary['failed_tests']}{Style.RESET_ALL}")
        print(f"📊 Taxa de Sucesso: {Fore.CYAN}{summary['success_rate']}%{Style.RESET_ALL}")
        print(f"⏱️  Tempo Médio: {summary['average_response_time']}ms")

        print(f"\n📋 Códigos de Status:")
        for code, count in summary['status_codes'].items():
            color = Fore.GREEN if 200 <= code < 300 else Fore.RED if code >= 400 else Fore.YELLOW
            print(f"   {color}{code}: {count} requisições{Style.RESET_ALL}")

        print(f"\n⏰ Executado em: {summary['timestamp']}")
        print(f"{Fore.CYAN}{'='*60}{Style.RESET_ALL}\n")

    def save_detailed_report(self, filename: str = None):
        """Salva relatório detalhado em arquivo JSON"""
        if not filename:
            timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
            filename = f"test_report_{timestamp}.json"

        report_data = {
            "summary": self.generate_summary_report(),
            "detailed_results": [
                {
                    "test_name": r.test_name,
                    "endpoint": r.endpoint,
                    "method": r.method,
                    "status_code": r.status_code,
                    "response_time": r.response_time,
                    "success": r.success,
                    "error_message": r.error_message,
                    "response_preview": str(r.response_data)[:500] if r.response_data else None
                }
                for r in self.results
            ]
        }

        with open(filename, 'w', encoding='utf-8') as f:
            json.dump(report_data, f, indent=2, ensure_ascii=False)

        self.log_success(f"Relatório detalhado salvo em: {filename}")
        return filename

def wait_between_tests():
    """Aguarda entre testes para não sobrecarregar a API"""
    delay = float(os.getenv('DELAY_BETWEEN_TESTS', '0.5'))
    if delay > 0:
        time.sleep(delay)

def format_json_response(data: Any, max_length: int = 300) -> str:
    """Formata resposta JSON para exibição"""
    if not data:
        return "Sem dados"

    try:
        formatted = json.dumps(data, indent=2, ensure_ascii=False)
        if len(formatted) > max_length:
            return formatted[:max_length] + "..."
        return formatted
    except:
        return str(data)[:max_length]

def create_sample_data():
    """Retorna dados de exemplo para testes"""
    return {
        "employee": {
            "firstName": "João",
            "lastName": "Silva",
            "email": "joao.silva@synqcore.com",
            "phoneNumber": "+55 11 99999-9999",
            "departmentId": None,  # Será preenchido dinamicamente
            "positionTitle": "Desenvolvedor Senior",
            "skills": "Python, .NET, React",
            "bio": "Desenvolvedor experiente em soluções corporativas"
        },
        "department": {
            "name": "Tecnologia da Informação",
            "description": "Departamento responsável por tecnologia e inovação",
            "parentDepartmentId": None
        },
        "knowledge_post": {
            "title": "Guia de Boas Práticas - API REST",
            "content": "Este guia apresenta as melhores práticas para desenvolvimento de APIs REST...",
            "categoryId": None,  # Será preenchido dinamicamente
            "visibility": "Company",
            "postType": "Article",
            "requiresApproval": False
        },
        "document_template": {
            "name": "Template Relatório Mensal",
            "description": "Template padrão para relatórios mensais da empresa",
            "category": "Report",
            "content": "# Relatório Mensal\\n\\n## Resumo Executivo\\n[Conteúdo do template]",
            "isActive": True
        }
    }

if __name__ == "__main__":
    # Teste básico do utilitário
    client = APITestClient()
    print("🔧 SynQcore API Test Utilities - Carregado com sucesso!")
    print(f"📡 Base URL: {client.base_url}")
    print(f"⏱️  Timeout: {client.timeout}s")
