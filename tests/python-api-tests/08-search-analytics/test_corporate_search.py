#!/usr/bin/env python3
"""
Testes de Busca Corporativa - SynQcore API

Este script testa todos os endpoints de busca corporativa:
- GET /api/corporate-search
- POST /api/corporate-search/advanced
- GET /api/corporate-search/suggestions
- GET /api/corporate-search/recent
- GET /api/corporate-search/popular
- GET /api/corporate-search/analytics
- POST /api/corporate-search/save-query
- GET /api/corporate-search/saved-queries
- DELETE /api/corporate-search/saved-queries/{id}

Execução: python test_corporate_search.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_corporate_search_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de busca corporativa"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("🔍 Iniciando testes de busca corporativa")

    # Dados de exemplo
    timestamp = int(time.time())

    # 1. Teste de busca geral
    client.log_info("1️⃣  Testando busca geral")
    result = client.make_request('GET', '/api/corporate-search', params={'query': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de busca avançada
    client.log_info("2️⃣  Testando busca avançada")
    advanced_search_data = {
        "query": "conhecimento corporativo",
        "contentTypes": ["KnowledgePost", "DiscussionThread", "Document"],
        "categories": [],
        "tags": ["conhecimento", "corporativo"],
        "authors": [],
        "dateRange": {
            "startDate": None,
            "endDate": None
        },
        "departments": [],
        "sortBy": "Relevance",  # Relevance, Date, Popularity, Author
        "sortOrder": "Desc",  # Asc, Desc
        "includeArchived": False,
        "page": 1,
        "pageSize": 10
    }

    result = client.make_request('POST', '/api/corporate-search/advanced', data=advanced_search_data)
    client.print_test_result(result)
    wait_between_tests()

    # 3. Teste de sugestões de busca
    client.log_info("3️⃣  Testando sugestões de busca")
    result = client.make_request('GET', '/api/corporate-search/suggestions', params={'partial': 'test'})
    client.print_test_result(result)
    wait_between_tests()

    # 4. Teste de buscas recentes
    client.log_info("4️⃣  Testando buscas recentes")
    result = client.make_request('GET', '/api/corporate-search/recent')
    client.print_test_result(result)
    wait_between_tests()

    # 5. Teste de buscas populares
    client.log_info("5️⃣  Testando buscas populares")
    result = client.make_request('GET', '/api/corporate-search/popular')
    client.print_test_result(result)
    wait_between_tests()

    # 6. Teste de salvamento de consulta
    client.log_info("6️⃣  Testando salvamento de consulta")
    save_query_data = {
        "name": f"Consulta Salva {timestamp}",
        "description": "Consulta de teste salva para uso posterior",
        "query": "desenvolvimento software",
        "filters": {
            "contentTypes": ["KnowledgePost", "DiscussionThread"],
            "tags": ["desenvolvimento", "software"],
            "departments": []
        },
        "isPublic": False,
        "scheduleFrequency": None  # None, Daily, Weekly, Monthly
    }

    result = client.make_request('POST', '/api/corporate-search/save-query', data=save_query_data)
    client.print_test_result(result)

    # Armazenar ID da consulta salva
    saved_query_id = None
    if result.success and result.response_data:
        saved_query_id = result.response_data.get('id')
        client.log_success(f"Consulta salva com ID: {saved_query_id}")

    wait_between_tests()

    # 7. Teste de listagem de consultas salvas
    client.log_info("7️⃣  Testando consultas salvas")
    result = client.make_request('GET', '/api/corporate-search/saved-queries')
    client.print_test_result(result)
    wait_between_tests()

    # 8. Teste de analytics de busca
    client.log_info("8️⃣  Testando analytics de busca")
    result = client.make_request('GET', '/api/corporate-search/analytics')
    client.print_test_result(result)
    wait_between_tests()

    # 9. Teste de busca com filtros específicos
    client.log_info("9️⃣  Testando busca com filtros")
    result = client.make_request('GET', '/api/corporate-search', params={
        'query': 'teste',
        'contentType': 'KnowledgePost',
        'sortBy': 'Date'
    })
    client.print_test_result(result)
    wait_between_tests()

    # 10. Teste de busca avançada com data
    client.log_info("🔟 Testando busca avançada com filtro de data")
    date_search_data = {
        "query": "*",  # Buscar tudo
        "contentTypes": ["Document", "KnowledgePost"],
        "dateRange": {
            "startDate": "2024-01-01T00:00:00Z",
            "endDate": "2024-12-31T23:59:59Z"
        },
        "sortBy": "Date",
        "sortOrder": "Desc",
        "page": 1,
        "pageSize": 5
    }

    result = client.make_request('POST', '/api/corporate-search/advanced', data=date_search_data)
    client.print_test_result(result)
    wait_between_tests()

    # 11. Teste de busca por autor
    client.log_info("1️⃣1️⃣ Testando busca por autor")
    author_search_data = {
        "query": "",
        "contentTypes": ["KnowledgePost", "DiscussionThread"],
        "authors": ["admin"],  # Usando o autor padrão
        "sortBy": "Date",
        "sortOrder": "Desc",
        "page": 1,
        "pageSize": 10
    }

    result = client.make_request('POST', '/api/corporate-search/advanced', data=author_search_data)
    client.print_test_result(result)
    wait_between_tests()

    # 12. Teste de múltiplas buscas para analytics
    client.log_info("1️⃣2️⃣ Testando múltiplas buscas")
    search_terms = ['conhecimento', 'colaboração', 'projeto', 'equipe', 'desenvolvimento']

    for i, term in enumerate(search_terms):
        client.log_info(f"1️⃣2️⃣.{i+1} Buscando: {term}")
        result = client.make_request('GET', '/api/corporate-search', params={'query': term})
        client.print_test_result(result, verbose=False)  # Menos verboso para múltiplas buscas
        wait_between_tests(0.5)  # Intervalo menor

    # 13. Teste de busca com paginação
    client.log_info("1️⃣3️⃣ Testando busca com paginação")
    result = client.make_request('GET', '/api/corporate-search', params={
        'query': 'teste',
        'page': 1,
        'pageSize': 3
    })
    client.print_test_result(result)
    wait_between_tests()

    # 14. Teste de busca vazia
    client.log_info("1️⃣4️⃣ Testando busca vazia")
    result = client.make_request('GET', '/api/corporate-search', params={'query': ''})
    client.print_test_result(result)
    wait_between_tests()

    # 15. Teste de busca com caracteres especiais
    client.log_info("1️⃣5️⃣ Testando busca com caracteres especiais")
    result = client.make_request('GET', '/api/corporate-search', params={'query': 'test* AND "exact phrase" OR (group)'})
    client.print_test_result(result)
    wait_between_tests()

    # 16. Teste de sugestões com termo parcial
    client.log_info("1️⃣6️⃣ Testando sugestões com termo parcial")
    partial_terms = ['con', 'des', 'proj']

    for i, partial in enumerate(partial_terms):
        client.log_info(f"1️⃣6️⃣.{i+1} Sugestões para: {partial}")
        result = client.make_request('GET', '/api/corporate-search/suggestions', params={'partial': partial})
        client.print_test_result(result, verbose=False)
        wait_between_tests(0.5)

    # 17. Teste de busca sem resultados
    client.log_info("1️⃣7️⃣ Testando busca sem resultados")
    result = client.make_request('GET', '/api/corporate-search', params={'query': 'terminexistentequenadadeveencontrar'})
    client.print_test_result(result)
    wait_between_tests()

    # 18. Teste de exclusão de consulta salva
    if saved_query_id:
        client.log_info("1️⃣8️⃣ Testando exclusão de consulta salva")
        result = client.make_request('DELETE', f'/api/corporate-search/saved-queries/{saved_query_id}')
        client.print_test_result(result)
        wait_between_tests()

        # Verificar se foi excluída
        client.log_info("1️⃣9️⃣ Verificando se consulta foi excluída")
        result = client.make_request('GET', '/api/corporate-search/saved-queries')
        client.print_test_result(result)

        # Verificar se a consulta não está mais na lista
        if result.success and result.response_data:
            saved_queries = result.response_data.get('items', [])
            if not any(q.get('id') == saved_query_id for q in saved_queries):
                client.log_success("Consulta salva foi excluída com sucesso")

    # 20. Analytics final após todas as buscas
    client.log_info("2️⃣0️⃣ Analytics final após testes")
    result = client.make_request('GET', '/api/corporate-search/analytics')
    client.print_test_result(result)

    return client, saved_query_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Busca Corporativa")
    print("=" * 50)

    try:
        client, saved_query_id = test_corporate_search_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("corporate_search_test_report.json")

        client.log_success("Testes de busca corporativa concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
