#!/usr/bin/env python3
"""
Testes de Categorias de Conhecimento - SynQcore API

Este script testa todos os endpoints de categorias de conhecimento:
- GET /api/knowledge-categories
- POST /api/knowledge-categories
- GET /api/knowledge-categories/{id}
- PUT /api/knowledge-categories/{id}
- DELETE /api/knowledge-categories/{id}
- GET /api/knowledge-categories/{id}/posts

Execução: python test_knowledge_categories.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_knowledge_categories_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de categorias de conhecimento"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("📂 Iniciando testes de categorias de conhecimento")

    # Dados de exemplo
    timestamp = int(time.time())

    # 1. Teste de listagem de categorias
    client.log_info("1️⃣  Testando listagem de categorias")
    result = client.make_request('GET', '/api/knowledge-categories')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de criação de categoria
    client.log_info("2️⃣  Testando criação de categoria")
    category_data = {
        "name": f"Categoria Teste {timestamp}",
        "description": "Categoria de teste para conhecimento corporativo",
        "icon": "📚",
        "color": "#007ACC",
        "isActive": True
    }

    result = client.make_request('POST', '/api/knowledge-categories', data=category_data)
    client.print_test_result(result)

    # Armazenar ID da categoria criada
    created_category_id = None
    if result.success and result.response_data:
        created_category_id = result.response_data.get('id')
        client.log_success(f"Categoria criada com ID: {created_category_id}")

    wait_between_tests()

    # 3. Teste de obtenção de categoria por ID
    if created_category_id:
        client.log_info("3️⃣  Testando obtenção por ID")
        result = client.make_request('GET', f'/api/knowledge-categories/{created_category_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 4. Teste de atualização de categoria
    if created_category_id:
        client.log_info("4️⃣  Testando atualização de categoria")
        update_data = {
            "name": f"Categoria Atualizada {timestamp}",
            "description": "Descrição atualizada da categoria",
            "icon": "📖",
            "color": "#28A745",
            "isActive": True
        }
        result = client.make_request('PUT', f'/api/knowledge-categories/{created_category_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 5. Teste de obtenção de posts por categoria
    if created_category_id:
        client.log_info("5️⃣  Testando posts por categoria")
        result = client.make_request('GET', f'/api/knowledge-categories/{created_category_id}/posts')
        client.print_test_result(result)
        wait_between_tests()

    # 6. Teste de listagem com paginação
    client.log_info("6️⃣  Testando listagem com paginação")
    result = client.make_request('GET', '/api/knowledge-categories', params={'page': 1, 'pageSize': 10})
    client.print_test_result(result)
    wait_between_tests()

    # 7. Teste de busca por nome
    client.log_info("7️⃣  Testando busca por nome")
    result = client.make_request('GET', '/api/knowledge-categories', params={'searchTerm': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 8. Teste de criação com dados inválidos
    client.log_info("8️⃣  Testando criação com dados inválidos")
    invalid_data = {
        "name": "",  # Nome vazio
        "description": "",
        "icon": "",
        "color": "cor-inválida",  # Cor inválida
        "isActive": True
    }
    result = client.make_request('POST', '/api/knowledge-categories', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 9. Teste de criação de categoria duplicada
    if created_category_id:
        client.log_info("9️⃣  Testando criação de categoria duplicada")
        duplicate_data = {
            "name": f"Categoria Atualizada {timestamp}",  # Mesmo nome da categoria atualizada
            "description": "Tentativa de categoria duplicada",
            "icon": "🔄",
            "color": "#FFC107",
            "isActive": True
        }
        result = client.make_request('POST', '/api/knowledge-categories', data=duplicate_data)
        client.print_test_result(result)
        wait_between_tests()

    # 10. Teste de desativação de categoria
    if created_category_id:
        client.log_info("🔟 Testando desativação de categoria")
        deactivate_data = {
            "name": f"Categoria Atualizada {timestamp}",
            "description": "Descrição atualizada da categoria",
            "icon": "📖",
            "color": "#28A745",
            "isActive": False  # Desativando
        }
        result = client.make_request('PUT', f'/api/knowledge-categories/{created_category_id}', data=deactivate_data)
        client.print_test_result(result)
        wait_between_tests()

    # 11. Teste de obtenção com ID inválido
    client.log_info("1️⃣1️⃣ Testando obtenção com ID inválido")
    result = client.make_request('GET', '/api/knowledge-categories/00000000-0000-0000-0000-000000000000')
    client.print_test_result(result)
    wait_between_tests()

    # 12. Teste de exclusão de categoria
    if created_category_id:
        client.log_info("1️⃣2️⃣ Testando exclusão de categoria")
        result = client.make_request('DELETE', f'/api/knowledge-categories/{created_category_id}')
        client.print_test_result(result)
        wait_between_tests()

        # Verificar se foi excluída
        client.log_info("1️⃣3️⃣ Verificando se categoria foi excluída")
        result = client.make_request('GET', f'/api/knowledge-categories/{created_category_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Categoria foi excluída com sucesso")

    return client, created_category_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Categorias de Conhecimento")
    print("=" * 50)

    try:
        client, created_category_id = test_knowledge_categories_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("knowledge_categories_test_report.json")

        client.log_success("Testes de categorias de conhecimento concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
