#!/usr/bin/env python3
"""
Testes de Funcionários - SynQcore API

Este script testa todos os endpoints de funcionários:
- GET /api/employees
- POST /api/employees
- GET /api/employees/{id}
- PUT /api/employees/{id}
- DELETE /api/employees/{id}
- GET /api/employees/hierarchy

Execução: python test_employees.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_employee_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de funcionários"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("👥 Iniciando testes de funcionários")

    # Dados de exemplo
    sample_data = create_sample_data()

    # 1. Teste de listagem de funcionários
    client.log_info("1️⃣  Testando listagem de funcionários")
    result = client.make_request('GET', '/api/employees')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de listagem com paginação
    client.log_info("2️⃣  Testando listagem com paginação")
    result = client.make_request('GET', '/api/employees', params={'page': 1, 'pageSize': 5})
    client.print_test_result(result)
    wait_between_tests()

    # 3. Teste de busca por nome
    client.log_info("3️⃣  Testando busca por nome")
    result = client.make_request('GET', '/api/employees', params={'searchTerm': 'admin'})
    client.print_test_result(result)
    wait_between_tests()

    # 4. Teste de criação de funcionário
    client.log_info("4️⃣  Testando criação de funcionário")
    timestamp = int(time.time())
    employee_data = sample_data["employee"].copy()
    employee_data["email"] = f"funcionario.{timestamp}@synqcore.com"

    result = client.make_request('POST', '/api/employees', data=employee_data)
    client.print_test_result(result)

    # Armazenar ID do funcionário criado
    created_employee_id = None
    if result.success and result.response_data:
        created_employee_id = result.response_data.get('id')
        client.log_success(f"Funcionário criado com ID: {created_employee_id}")

    wait_between_tests()

    # 5. Teste de obtenção de funcionário por ID
    if created_employee_id:
        client.log_info("5️⃣  Testando obtenção por ID")
        result = client.make_request('GET', f'/api/employees/{created_employee_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 6. Teste de atualização de funcionário
    if created_employee_id:
        client.log_info("6️⃣  Testando atualização de funcionário")
        update_data = {
            "firstName": "João Carlos",
            "lastName": "Silva Santos",
            "positionTitle": "Desenvolvedor Pleno",
            "skills": "Python, .NET, React, Docker"
        }
        result = client.make_request('PUT', f'/api/employees/{created_employee_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 7. Teste de hierarquia organizacional
    client.log_info("7️⃣  Testando hierarquia organizacional")
    result = client.make_request('GET', '/api/employees/hierarchy')
    client.print_test_result(result)
    wait_between_tests()

    # 8. Teste de busca avançada
    client.log_info("8️⃣  Testando busca avançada")
    result = client.make_request('GET', '/api/employees', params={
        'searchTerm': 'desenvolvedor',
        'sortBy': 'firstName',
        'sortOrder': 'asc',
        'page': 1,
        'pageSize': 10
    })
    client.print_test_result(result)
    wait_between_tests()

    # 9. Teste de criação com dados inválidos
    client.log_info("9️⃣  Testando criação com dados inválidos")
    invalid_data = {
        "firstName": "",  # Nome vazio
        "lastName": "",   # Sobrenome vazio
        "email": "email-invalido",  # Email inválido
        "phoneNumber": "123"  # Telefone muito curto
    }
    result = client.make_request('POST', '/api/employees', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 10. Teste de obtenção com ID inválido
    client.log_info("🔟 Testando obtenção com ID inválido")
    result = client.make_request('GET', '/api/employees/00000000-0000-0000-0000-000000000000')
    client.print_test_result(result)
    wait_between_tests()

    # 11. Teste de exclusão (soft delete)
    if created_employee_id:
        client.log_info("1️⃣1️⃣ Testando exclusão de funcionário")
        result = client.make_request('DELETE', f'/api/employees/{created_employee_id}')
        client.print_test_result(result)
        wait_between_tests()

        # Verificar se foi excluído (soft delete)
        client.log_info("1️⃣2️⃣ Verificando se funcionário foi excluído")
        result = client.make_request('GET', f'/api/employees/{created_employee_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Funcionário foi excluído com sucesso (soft delete)")

    return client, created_employee_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Funcionários")
    print("=" * 50)

    try:
        client, created_employee_id = test_employee_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("employees_test_report.json")

        client.log_success("Testes de funcionários concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
