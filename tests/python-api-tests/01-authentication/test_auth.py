#!/usr/bin/env python3
"""
Testes de Autenticação - SynQcore API

Este script testa todos os endpoints relacionados à autenticação:
- POST /api/auth/login
- POST /api/auth/register
- GET /api/auth/test

Execução: python test_auth.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests

def test_authentication_endpoints():
    """Testa todos os endpoints de autenticação"""

    client = APITestClient()
    client.log_info("🔐 Iniciando testes de autenticação")

    # 1. Teste de Health Check primeiro (sem autenticação)
    client.log_info("1️⃣  Testando Health Check")
    result = client.make_request('GET', '/health')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de Login com credenciais válidas
    client.log_info("2️⃣  Testando Login com credenciais válidas")
    login_data = {
        "email": os.getenv('TEST_EMAIL', 'admin@synqcore.com'),
        "password": os.getenv('TEST_PASSWORD', 'SynQcore@Admin123!')
    }
    result = client.make_request('POST', '/api/auth/login', data=login_data)
    client.print_test_result(result)

    # Extrair token se o login foi bem-sucedido
    if result.success and result.response_data:
        token = result.response_data.get('token')
        if token:
            client.token = token
            client.session.headers.update({'Authorization': f'Bearer {token}'})
            client.log_success("Token JWT obtido e configurado para próximos testes")
        else:
            client.log_warning("Token não encontrado na resposta de login")

    wait_between_tests()

    # 3. Teste de Login com credenciais inválidas
    client.log_info("3️⃣  Testando Login com credenciais inválidas")
    invalid_login_data = {
        "email": "usuario.inexistente@synqcore.com",
        "password": "senhaerrada123"
    }
    result = client.make_request('POST', '/api/auth/login', data=invalid_login_data)
    client.print_test_result(result)
    wait_between_tests()

    # 4. Teste do endpoint de teste (requer autenticação)
    client.log_info("4️⃣  Testando endpoint de teste autenticado")
    result = client.make_request('GET', '/api/auth/test')
    client.print_test_result(result)
    wait_between_tests()

    # 5. Teste de Register (criar novo usuário)
    client.log_info("5️⃣  Testando registro de novo usuário")
    register_data = {
        "email": f"teste.usuario.{int(time.time())}@synqcore.com",
        "password": "MinhaSenh@123!",
        "firstName": "Usuário",
        "lastName": "Teste"
    }
    result = client.make_request('POST', '/api/auth/register', data=register_data)
    client.print_test_result(result)
    wait_between_tests()

    # 6. Teste de Register com dados inválidos
    client.log_info("6️⃣  Testando registro com dados inválidos")
    invalid_register_data = {
        "email": "email-invalido",
        "password": "123",  # Senha muito simples
        "firstName": "",
        "lastName": ""
    }
    result = client.make_request('POST', '/api/auth/register', data=invalid_register_data)
    client.print_test_result(result)
    wait_between_tests()

    # 7. Teste de acesso sem token (deve falhar)
    client.log_info("7️⃣  Testando acesso sem token de autenticação")
    # Remover temporariamente o token
    original_headers = client.session.headers.copy()
    if 'Authorization' in client.session.headers:
        del client.session.headers['Authorization']

    result = client.make_request('GET', '/api/employees')
    client.print_test_result(result)

    # Restaurar headers
    client.session.headers.update(original_headers)
    wait_between_tests()

    # 8. Teste de acesso com token inválido
    client.log_info("8️⃣  Testando acesso com token inválido")
    client.session.headers.update({'Authorization': 'Bearer token-completamente-invalido'})

    result = client.make_request('GET', '/api/employees')
    client.print_test_result(result)

    # Restaurar token válido
    client.session.headers.update(original_headers)

    return client

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Autenticação")
    print("=" * 50)

    try:
        client = test_authentication_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("auth_test_report.json")

        client.log_success("Testes de autenticação concluídos!")

        # Retornar o cliente para uso em outros scripts
        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
