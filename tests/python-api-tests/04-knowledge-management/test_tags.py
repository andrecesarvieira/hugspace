#!/usr/bin/env python3
"""
Testes de Tags - SynQcore API

Este script testa todos os endpoints de tags:
- GET /api/tags
- POST /api/tags
- GET /api/tags/{id}
- PUT /api/tags/{id}
- DELETE /api/tags/{id}
- GET /api/tags/popular
- GET /api/tags/search

Execução: python test_tags.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_tags_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de tags"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("🏷️  Iniciando testes de tags")

    # Dados de exemplo
    timestamp = int(time.time())

    # 1. Teste de listagem de tags
    client.log_info("1️⃣  Testando listagem de tags")
    result = client.make_request('GET', '/api/tags')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de criação de tag
    client.log_info("2️⃣  Testando criação de tag")
    tag_data = {
        "name": f"tag-teste-{timestamp}",
        "description": "Tag de teste para conhecimento corporativo",
        "color": "#007ACC",
        "isActive": True
    }

    result = client.make_request('POST', '/api/tags', data=tag_data)
    client.print_test_result(result)

    # Armazenar ID da tag criada
    created_tag_id = None
    if result.success and result.response_data:
        created_tag_id = result.response_data.get('id')
        client.log_success(f"Tag criada com ID: {created_tag_id}")

    wait_between_tests()

    # 3. Teste de obtenção de tag por ID
    if created_tag_id:
        client.log_info("3️⃣  Testando obtenção por ID")
        result = client.make_request('GET', f'/api/tags/{created_tag_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 4. Teste de atualização de tag
    if created_tag_id:
        client.log_info("4️⃣  Testando atualização de tag")
        update_data = {
            "name": f"tag-atualizada-{timestamp}",
            "description": "Tag atualizada para conhecimento corporativo",
            "color": "#28A745",
            "isActive": True
        }
        result = client.make_request('PUT', f'/api/tags/{created_tag_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 5. Teste de tags populares
    client.log_info("5️⃣  Testando tags populares")
    result = client.make_request('GET', '/api/tags/popular')
    client.print_test_result(result)
    wait_between_tests()

    # 6. Teste de busca de tags
    client.log_info("6️⃣  Testando busca de tags")
    result = client.make_request('GET', '/api/tags/search', params={'query': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 7. Teste de listagem com paginação
    client.log_info("7️⃣  Testando listagem com paginação")
    result = client.make_request('GET', '/api/tags', params={'page': 1, 'pageSize': 10})
    client.print_test_result(result)
    wait_between_tests()

    # 8. Teste de criação de várias tags
    client.log_info("8️⃣  Testando criação de múltiplas tags")
    additional_tags = [
        {
            "name": f"desenvolvimento-{timestamp}",
            "description": "Tag para desenvolvimento",
            "color": "#17A2B8",
            "isActive": True
        },
        {
            "name": f"documentacao-{timestamp}",
            "description": "Tag para documentação",
            "color": "#FFC107",
            "isActive": True
        },
        {
            "name": f"tutorial-{timestamp}",
            "description": "Tag para tutoriais",
            "color": "#DC3545",
            "isActive": True
        }
    ]

    created_tag_ids = []
    for i, tag_data in enumerate(additional_tags):
        client.log_info(f"8️⃣.{i+1} Criando tag: {tag_data['name']}")
        result = client.make_request('POST', '/api/tags', data=tag_data)
        client.print_test_result(result)

        if result.success and result.response_data:
            created_tag_ids.append(result.response_data.get('id'))

        wait_between_tests()

    # 9. Teste de criação com dados inválidos
    client.log_info("9️⃣  Testando criação com dados inválidos")
    invalid_data = {
        "name": "",  # Nome vazio
        "description": "",
        "color": "cor-inválida",  # Cor inválida
        "isActive": True
    }
    result = client.make_request('POST', '/api/tags', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 10. Teste de criação de tag duplicada
    if created_tag_id:
        client.log_info("🔟 Testando criação de tag duplicada")
        duplicate_data = {
            "name": f"tag-atualizada-{timestamp}",  # Mesmo nome da tag atualizada
            "description": "Tentativa de tag duplicada",
            "color": "#6F42C1",
            "isActive": True
        }
        result = client.make_request('POST', '/api/tags', data=duplicate_data)
        client.print_test_result(result)
        wait_between_tests()

    # 11. Teste de desativação de tag
    if created_tag_id:
        client.log_info("1️⃣1️⃣ Testando desativação de tag")
        deactivate_data = {
            "name": f"tag-atualizada-{timestamp}",
            "description": "Tag atualizada para conhecimento corporativo",
            "color": "#28A745",
            "isActive": False  # Desativando
        }
        result = client.make_request('PUT', f'/api/tags/{created_tag_id}', data=deactivate_data)
        client.print_test_result(result)
        wait_between_tests()

    # 12. Teste de obtenção com ID inválido
    client.log_info("1️⃣2️⃣ Testando obtenção com ID inválido")
    result = client.make_request('GET', '/api/tags/00000000-0000-0000-0000-000000000000')
    client.print_test_result(result)
    wait_between_tests()

    # 13. Teste de busca sem resultados
    client.log_info("1️⃣3️⃣ Testando busca sem resultados")
    result = client.make_request('GET', '/api/tags/search', params={'query': 'terminexistentequenadadeveencontrar'})
    client.print_test_result(result)
    wait_between_tests()

    # 14. Limpeza - Exclusão das tags criadas
    all_tag_ids = [created_tag_id] + created_tag_ids if created_tag_id else created_tag_ids

    for i, tag_id in enumerate(all_tag_ids):
        if tag_id:
            client.log_info(f"1️⃣4️⃣.{i+1} Testando exclusão de tag")
            result = client.make_request('DELETE', f'/api/tags/{tag_id}')
            client.print_test_result(result)
            wait_between_tests()

    # 15. Verificação final - confirmar exclusões
    if created_tag_id:
        client.log_info("1️⃣5️⃣ Verificando se tag principal foi excluída")
        result = client.make_request('GET', f'/api/tags/{created_tag_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Tag foi excluída com sucesso")

    return client, created_tag_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Tags")
    print("=" * 50)

    try:
        client, created_tag_id = test_tags_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("tags_test_report.json")

        client.log_success("Testes de tags concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
