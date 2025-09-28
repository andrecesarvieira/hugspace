#!/usr/bin/env python3
"""
Testes de Posts de Conhecimento - SynQcore API

Este script testa todos os endpoints de posts de conhecimento:
- GET /api/knowledge-posts
- POST /api/knowledge-posts
- GET /api/knowledge-posts/{id}
- PUT /api/knowledge-posts/{id}
- DELETE /api/knowledge-posts/{id}
- POST /api/knowledge-posts/{id}/like
- POST /api/knowledge-posts/{id}/comment
- GET /api/knowledge-posts/search
- GET /api/knowledge-posts/analytics

Execução: python test_knowledge_posts.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_knowledge_posts_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de posts de conhecimento"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("📚 Iniciando testes de posts de conhecimento")

    # Dados de exemplo
    sample_data = create_sample_data()
    timestamp = int(time.time())

    # 1. Teste de listagem de posts
    client.log_info("1️⃣  Testando listagem de posts de conhecimento")
    result = client.make_request('GET', '/api/knowledge-posts')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de criação de post de conhecimento
    client.log_info("2️⃣  Testando criação de post de conhecimento")
    post_data = {
        "title": f"Artigo de Conhecimento {timestamp}",
        "content": "# Título Principal\n\nEste é um artigo de conhecimento corporativo com conteúdo relevante.\n\n## Seção 1\n\nConteúdo da seção 1 com informações importantes.\n\n## Seção 2\n\n- Item 1\n- Item 2\n- Item 3",
        "summary": "Resumo do artigo de conhecimento para teste",
        "tags": ["teste", "conhecimento", "documentacao"],
        "categoryId": None,  # Será definido se categoria existir
        "isPublished": True,
        "allowComments": True
    }

    result = client.make_request('POST', '/api/knowledge-posts', data=post_data)
    client.print_test_result(result)

    # Armazenar ID do post criado
    created_post_id = None
    if result.success and result.response_data:
        created_post_id = result.response_data.get('id')
        client.log_success(f"Post criado com ID: {created_post_id}")

    wait_between_tests()

    # 3. Teste de obtenção de post por ID
    if created_post_id:
        client.log_info("3️⃣  Testando obtenção por ID")
        result = client.make_request('GET', f'/api/knowledge-posts/{created_post_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 4. Teste de atualização de post
    if created_post_id:
        client.log_info("4️⃣  Testando atualização de post")
        update_data = {
            "title": f"Artigo Atualizado {timestamp}",
            "content": "# Conteúdo Atualizado\n\nEste artigo foi atualizado com novas informações.",
            "summary": "Resumo atualizado do artigo",
            "tags": ["teste", "conhecimento", "atualizado"],
            "isPublished": True
        }
        result = client.make_request('PUT', f'/api/knowledge-posts/{created_post_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 5. Teste de curtir post
    if created_post_id:
        client.log_info("5️⃣  Testando curtir post")
        result = client.make_request('POST', f'/api/knowledge-posts/{created_post_id}/like')
        client.print_test_result(result)
        wait_between_tests()

    # 6. Teste de comentário em post
    if created_post_id:
        client.log_info("6️⃣  Testando comentário em post")
        comment_data = {
            "content": "Este é um comentário de teste no artigo de conhecimento.",
            "isInternal": True
        }
        result = client.make_request('POST', f'/api/knowledge-posts/{created_post_id}/comment', data=comment_data)
        client.print_test_result(result)
        wait_between_tests()

    # 7. Teste de busca de posts
    client.log_info("7️⃣  Testando busca de posts")
    result = client.make_request('GET', '/api/knowledge-posts/search', params={'query': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 8. Teste de busca por tag
    client.log_info("8️⃣  Testando busca por tag")
    result = client.make_request('GET', '/api/knowledge-posts', params={'tags': 'conhecimento'})
    client.print_test_result(result)
    wait_between_tests()

    # 9. Teste de listagem com paginação
    client.log_info("9️⃣  Testando listagem com paginação")
    result = client.make_request('GET', '/api/knowledge-posts', params={'page': 1, 'pageSize': 5})
    client.print_test_result(result)
    wait_between_tests()

    # 10. Teste de analytics de posts
    client.log_info("🔟 Testando analytics de posts")
    result = client.make_request('GET', '/api/knowledge-posts/analytics')
    client.print_test_result(result)
    wait_between_tests()

    # 11. Teste de criação com dados inválidos
    client.log_info("1️⃣1️⃣ Testando criação com dados inválidos")
    invalid_data = {
        "title": "",  # Título vazio
        "content": "",  # Conteúdo vazio
        "tags": [],  # Sem tags
        "isPublished": True
    }
    result = client.make_request('POST', '/api/knowledge-posts', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 12. Teste de busca sem resultados
    client.log_info("1️⃣2️⃣ Testando busca sem resultados")
    result = client.make_request('GET', '/api/knowledge-posts/search', params={'query': 'termoinexistentequenadadeveencontrar'})
    client.print_test_result(result)
    wait_between_tests()

    # 13. Teste de curtir novamente (descurtir)
    if created_post_id:
        client.log_info("1️⃣3️⃣ Testando descurtir post")
        result = client.make_request('POST', f'/api/knowledge-posts/{created_post_id}/like')
        client.print_test_result(result)
        wait_between_tests()

    # 14. Teste de exclusão suave (soft delete)
    if created_post_id:
        client.log_info("1️⃣4️⃣ Testando exclusão de post")
        result = client.make_request('DELETE', f'/api/knowledge-posts/{created_post_id}')
        client.print_test_result(result)
        wait_between_tests()

        # Verificar se foi excluído
        client.log_info("1️⃣5️⃣ Verificando se post foi excluído")
        result = client.make_request('GET', f'/api/knowledge-posts/{created_post_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Post foi excluído com sucesso")

    return client, created_post_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Posts de Conhecimento")
    print("=" * 50)

    try:
        client, created_post_id = test_knowledge_posts_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("knowledge_posts_test_report.json")

        client.log_success("Testes de posts de conhecimento concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
