#!/usr/bin/env python3
"""
Testes de Administração - SynQcore API

Este script testa todos os endpoints administrativos:
- GET /api/admin/users
- POST /api/admin/users
- GET /api/admin/roles

Execução: python test_admin.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests

def test_admin_endpoints(client: APITestClient = None):
    """Testa todos os endpoints administrativos"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("👑 Iniciando testes administrativos")

    # 1. Teste de listagem de papéis disponíveis
    client.log_info("1️⃣  Testando listagem de papéis (roles)")
    result = client.make_request('GET', '/api/admin/roles')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de listagem de usuários (com paginação)
    client.log_info("2️⃣  Testando listagem de usuários")
    result = client.make_request('GET', '/api/admin/users', params={'page': 1, 'pageSize': 10})
    client.print_test_result(result)
    wait_between_tests()

    # 3. Teste de busca de usuários por nome
    client.log_info("3️⃣  Testando busca de usuários por nome")
    result = client.make_request('GET', '/api/admin/users', params={'searchTerm': 'admin'})
    client.print_test_result(result)
    wait_between_tests()

    # 4. Teste de criação de novo usuário administrativo
    client.log_info("4️⃣  Testando criação de novo usuário")
    timestamp = int(time.time())
    new_user_data = {
        "email": f"novo.usuario.{timestamp}@synqcore.com",
        "password": "NovaSenh@123!",
        "firstName": "Novo",
        "lastName": "Usuário",
        "roles": ["Employee"]  # Papel básico
    }
    result = client.make_request('POST', '/api/admin/users', data=new_user_data)
    client.print_test_result(result)

    # Armazenar ID do usuário criado para testes futuros
    created_user_id = None
    if result.success and result.response_data:
        created_user_id = result.response_data.get('id')
        client.log_success(f"Usuário criado com ID: {created_user_id}")

    wait_between_tests()

    # 5. Teste de criação de usuário com papel de Manager
    client.log_info("5️⃣  Testando criação de usuário Manager")
    manager_data = {
        "email": f"manager.{timestamp}@synqcore.com",
        "password": "ManagerSenh@123!",
        "firstName": "Gestor",
        "lastName": "Teste",
        "roles": ["Manager", "Employee"]
    }
    result = client.make_request('POST', '/api/admin/users', data=manager_data)
    client.print_test_result(result)
    wait_between_tests()

    # 6. Teste de criação de usuário com dados inválidos
    client.log_info("6️⃣  Testando criação com dados inválidos")
    invalid_user_data = {
        "email": "email-invalido",  # Email inválido
        "password": "123",  # Senha muito simples
        "firstName": "",  # Nome vazio
        "lastName": "",  # Sobrenome vazio
        "roles": ["RoleInexistente"]  # Papel que não existe
    }
    result = client.make_request('POST', '/api/admin/users', data=invalid_user_data)
    client.print_test_result(result)
    wait_between_tests()

    # 7. Teste de criação de usuário com email duplicado
    client.log_info("7️⃣  Testando criação com email duplicado")
    duplicate_email_data = {
        "email": "admin@synqcore.com",  # Email que já existe
        "password": "OutraSenh@123!",
        "firstName": "Admin",
        "lastName": "Duplicado",
        "roles": ["Employee"]
    }
    result = client.make_request('POST', '/api/admin/users', data=duplicate_email_data)
    client.print_test_result(result)
    wait_between_tests()

    # 8. Teste de listagem com filtros avançados
    client.log_info("8️⃣  Testando listagem com filtros avançados")
    result = client.make_request('GET', '/api/admin/users', params={
        'page': 1,
        'pageSize': 5,
        'searchTerm': 'teste',
        'sortBy': 'email',
        'sortOrder': 'asc'
    })
    client.print_test_result(result)
    wait_between_tests()

    # 9. Teste de acesso negado (remover temporariamente permissões de admin)
    client.log_info("9️⃣  Testando acesso com usuário não-admin")
    # Este teste simula um cenário onde um usuário não-admin tenta acessar endpoints admin
    # Em um ambiente real, seria necessário autenticar com um usuário diferente
    client.log_info("   (Simulação: em produção, este endpoint deveria retornar 403 Forbidden)")
    wait_between_tests()

    return client, created_user_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes Administrativos")
    print("=" * 50)

    try:
        client, created_user_id = test_admin_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("admin_test_report.json")

        client.log_success("Testes administrativos concluídos!")

        if created_user_id:
            client.log_info(f"Usuário de teste criado com ID: {created_user_id}")
            client.log_info("Este usuário pode ser usado para testes adicionais")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
