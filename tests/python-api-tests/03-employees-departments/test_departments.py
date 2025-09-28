#!/usr/bin/env python3
"""
Testes de Departamentos - SynQcore API

Este script testa todos os endpoints de departamentos:
- GET /api/departments
- POST /api/departments
- GET /api/departments/{id}
- PUT /api/departments/{id}
- DELETE /api/departments/{id}
- GET /api/departments/hierarchy

Execução: python test_departments.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_department_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de departamentos"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("🏢 Iniciando testes de departamentos")

    # Dados de exemplo
    sample_data = create_sample_data()

    # 1. Teste de listagem de departamentos
    client.log_info("1️⃣  Testando listagem de departamentos")
    result = client.make_request('GET', '/api/departments')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de hierarquia de departamentos
    client.log_info("2️⃣  Testando hierarquia de departamentos")
    result = client.make_request('GET', '/api/departments/hierarchy')
    client.print_test_result(result)
    wait_between_tests()

    # 3. Teste de criação de departamento principal
    client.log_info("3️⃣  Testando criação de departamento principal")
    timestamp = int(time.time())
    department_data = sample_data["department"].copy()
    department_data["name"] = f"Dept Test {timestamp}"

    result = client.make_request('POST', '/api/departments', data=department_data)
    client.print_test_result(result)

    # Armazenar ID do departamento criado
    created_department_id = None
    if result.success and result.response_data:
        created_department_id = result.response_data.get('id')
        client.log_success(f"Departamento criado com ID: {created_department_id}")

    wait_between_tests()

    # 4. Teste de criação de subdepartamento
    if created_department_id:
        client.log_info("4️⃣  Testando criação de subdepartamento")
        subdepartment_data = {
            "name": f"Subdept Test {timestamp}",
            "description": "Subdepartamento de teste",
            "parentDepartmentId": created_department_id
        }
        result = client.make_request('POST', '/api/departments', data=subdepartment_data)
        client.print_test_result(result)

        # Armazenar ID do subdepartamento
        created_subdepartment_id = None
        if result.success and result.response_data:
            created_subdepartment_id = result.response_data.get('id')

        wait_between_tests()

    # 5. Teste de obtenção de departamento por ID
    if created_department_id:
        client.log_info("5️⃣  Testando obtenção por ID")
        result = client.make_request('GET', f'/api/departments/{created_department_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 6. Teste de atualização de departamento
    if created_department_id:
        client.log_info("6️⃣  Testando atualização de departamento")
        update_data = {
            "name": f"Dept Atualizado {timestamp}",
            "description": "Descrição atualizada do departamento"
        }
        result = client.make_request('PUT', f'/api/departments/{created_department_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 7. Teste de listagem com paginação
    client.log_info("7️⃣  Testando listagem com paginação")
    result = client.make_request('GET', '/api/departments', params={'page': 1, 'pageSize': 5})
    client.print_test_result(result)
    wait_between_tests()

    # 8. Teste de busca por nome
    client.log_info("8️⃣  Testando busca por nome")
    result = client.make_request('GET', '/api/departments', params={'searchTerm': 'test'})
    client.print_test_result(result)
    wait_between_tests()

    # 9. Teste de criação com dados inválidos
    client.log_info("9️⃣  Testando criação com dados inválidos")
    invalid_data = {
        "name": "",  # Nome vazio
        "description": "",  # Descrição vazia
        "parentDepartmentId": "00000000-0000-0000-0000-000000000000"  # ID inválido
    }
    result = client.make_request('POST', '/api/departments', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 10. Teste de referência circular (departamento como pai de si mesmo)
    if created_department_id:
        client.log_info("🔟 Testando referência circular")
        circular_data = {
            "name": "Dept Circular",
            "description": "Teste de referência circular",
            "parentDepartmentId": created_department_id
        }
        # Tentar fazer o departamento ser pai de si mesmo na atualização
        update_circular = {
            "parentDepartmentId": created_department_id
        }
        result = client.make_request('PUT', f'/api/departments/{created_department_id}', data=update_circular)
        client.print_test_result(result)
        if not result.success:
            client.log_success("Referência circular foi corretamente rejeitada")
        wait_between_tests()

    # 11. Teste de obtenção com ID inválido
    client.log_info("1️⃣1️⃣ Testando obtenção com ID inválido")
    result = client.make_request('GET', '/api/departments/00000000-0000-0000-0000-000000000000')
    client.print_test_result(result)
    wait_between_tests()

    # 12. Teste de exclusão de subdepartamento primeiro
    if 'created_subdepartment_id' in locals() and created_subdepartment_id:
        client.log_info("1️⃣2️⃣ Testando exclusão de subdepartamento")
        result = client.make_request('DELETE', f'/api/departments/{created_subdepartment_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 13. Teste de exclusão de departamento principal
    if created_department_id:
        client.log_info("1️⃣3️⃣ Testando exclusão de departamento principal")
        result = client.make_request('DELETE', f'/api/departments/{created_department_id}')
        client.print_test_result(result)
        wait_between_tests()

        # Verificar se foi excluído
        client.log_info("1️⃣4️⃣ Verificando se departamento foi excluído")
        result = client.make_request('GET', f'/api/departments/{created_department_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Departamento foi excluído com sucesso")

    return client, created_department_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Departamentos")
    print("=" * 50)

    try:
        client, created_department_id = test_department_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("departments_test_report.json")

        client.log_success("Testes de departamentos concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
