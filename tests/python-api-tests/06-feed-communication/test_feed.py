#!/usr/bin/env python3
"""
Testes de Feed - SynQcore API

Este script testa todos os endpoints de feed:
- GET /api/feed
- GET /api/feed/personal
- GET /api/feed/company
- GET /api/feed/trending
- POST /api/feed/posts
- GET /api/feed/posts/{id}
- PUT /api/feed/posts/{id}
- DELETE /api/feed/posts/{id}
- POST /api/feed/posts/{id}/like
- POST /api/feed/posts/{id}/comment
- GET /api/feed/analytics

Execução: python test_feed.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_feed_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de feed"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("📰 Iniciando testes de feed")

    # Dados de exemplo
    timestamp = int(time.time())

    # 1. Teste de feed geral
    client.log_info("1️⃣  Testando feed geral")
    result = client.make_request('GET', '/api/feed')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de feed pessoal
    client.log_info("2️⃣  Testando feed pessoal")
    result = client.make_request('GET', '/api/feed/personal')
    client.print_test_result(result)
    wait_between_tests()

    # 3. Teste de feed da empresa
    client.log_info("3️⃣  Testando feed da empresa")
    result = client.make_request('GET', '/api/feed/company')
    client.print_test_result(result)
    wait_between_tests()

    # 4. Teste de trending
    client.log_info("4️⃣  Testando trending")
    result = client.make_request('GET', '/api/feed/trending')
    client.print_test_result(result)
    wait_between_tests()

    # 5. Teste de criação de post no feed
    client.log_info("5️⃣  Testando criação de post")
    post_data = {
        "content": f"Este é um post de teste no feed da empresa {timestamp}. 🚀\n\n**Destaques:**\n- Ponto importante 1\n- Ponto importante 2\n\n#teste #feed #corporativo",
        "type": "General",  # General, Announcement, Achievement, Question
        "visibility": "Public",  # Public, Department, Private
        "tags": ["teste", "feed", "corporativo"],
        "attachments": [],
        "isPinned": False,
        "allowComments": True,
        "allowReactions": True
    }

    result = client.make_request('POST', '/api/feed/posts', data=post_data)
    client.print_test_result(result)

    # Armazenar ID do post criado
    created_post_id = None
    if result.success and result.response_data:
        created_post_id = result.response_data.get('id')
        client.log_success(f"Post criado com ID: {created_post_id}")

    wait_between_tests()

    # 6. Teste de obtenção de post por ID
    if created_post_id:
        client.log_info("6️⃣  Testando obtenção de post por ID")
        result = client.make_request('GET', f'/api/feed/posts/{created_post_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 7. Teste de curtir post
    if created_post_id:
        client.log_info("7️⃣  Testando curtir post")
        result = client.make_request('POST', f'/api/feed/posts/{created_post_id}/like')
        client.print_test_result(result)
        wait_between_tests()

    # 8. Teste de comentário em post
    if created_post_id:
        client.log_info("8️⃣  Testando comentário em post")
        comment_data = {
            "content": "Este é um comentário de teste no post do feed! 👍",
            "isInternal": True
        }
        result = client.make_request('POST', f'/api/feed/posts/{created_post_id}/comment', data=comment_data)
        client.print_test_result(result)
        wait_between_tests()

    # 9. Teste de atualização de post
    if created_post_id:
        client.log_info("9️⃣  Testando atualização de post")
        update_data = {
            "content": f"Post atualizado no feed {timestamp}! ✨\n\n**Atualização:**\n- Conteúdo modificado\n- Novas informações\n\n#atualizado #teste",
            "type": "Announcement",
            "visibility": "Public",
            "tags": ["atualizado", "teste", "feed"],
            "isPinned": True,  # Fixando o post
            "allowComments": True,
            "allowReactions": True
        }
        result = client.make_request('PUT', f'/api/feed/posts/{created_post_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 10. Teste de criação de diferentes tipos de posts
    client.log_info("🔟 Testando diferentes tipos de posts")
    post_types = [
        {
            "content": f"Pergunta importante para o time {timestamp}: Como podemos melhorar nossos processos?",
            "type": "Question",
            "visibility": "Department",
            "tags": ["pergunta", "melhoria"]
        },
        {
            "content": f"Conquista alcançada! 🎉 Projeto concluído com sucesso em {timestamp}",
            "type": "Achievement",
            "visibility": "Public",
            "tags": ["conquista", "sucesso"]
        }
    ]

    created_additional_posts = []
    for i, post_data in enumerate(post_types):
        client.log_info(f"🔟.{i+1} Criando post do tipo: {post_data['type']}")
        result = client.make_request('POST', '/api/feed/posts', data=post_data)
        client.print_test_result(result)

        if result.success and result.response_data:
            created_additional_posts.append(result.response_data.get('id'))

        wait_between_tests()

    # 11. Teste de feed com paginação
    client.log_info("1️⃣1️⃣ Testando feed com paginação")
    result = client.make_request('GET', '/api/feed', params={'page': 1, 'pageSize': 5})
    client.print_test_result(result)
    wait_between_tests()

    # 12. Teste de feed com filtros
    client.log_info("1️⃣2️⃣ Testando feed com filtros")
    result = client.make_request('GET', '/api/feed', params={'type': 'Announcement', 'visibility': 'Public'})
    client.print_test_result(result)
    wait_between_tests()

    # 13. Teste de analytics do feed
    client.log_info("1️⃣3️⃣ Testando analytics do feed")
    result = client.make_request('GET', '/api/feed/analytics')
    client.print_test_result(result)
    wait_between_tests()

    # 14. Teste de busca no feed
    client.log_info("1️⃣4️⃣ Testando busca no feed")
    result = client.make_request('GET', '/api/feed', params={'searchTerm': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 15. Teste de posts fixados
    client.log_info("1️⃣5️⃣ Testando posts fixados")
    result = client.make_request('GET', '/api/feed', params={'isPinned': True})
    client.print_test_result(result)
    wait_between_tests()

    # 16. Teste de curtir novamente (descurtir)
    if created_post_id:
        client.log_info("1️⃣6️⃣ Testando descurtir post")
        result = client.make_request('POST', f'/api/feed/posts/{created_post_id}/like')
        client.print_test_result(result)
        wait_between_tests()

    # 17. Teste de criação com dados inválidos
    client.log_info("1️⃣7️⃣ Testando criação com dados inválidos")
    invalid_data = {
        "content": "",  # Conteúdo vazio
        "type": "InvalidType",  # Tipo inválido
        "visibility": "InvalidVisibility",  # Visibilidade inválida
        "tags": [],
        "allowComments": True
    }
    result = client.make_request('POST', '/api/feed/posts', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 18. Teste de obtenção com ID inválido
    client.log_info("1️⃣8️⃣ Testando obtenção com ID inválido")
    result = client.make_request('GET', '/api/feed/posts/00000000-0000-0000-0000-000000000000')
    client.print_test_result(result)
    wait_between_tests()

    # 19. Limpeza - Exclusão dos posts criados
    all_post_ids = [created_post_id] + created_additional_posts if created_post_id else created_additional_posts

    for i, post_id in enumerate(all_post_ids):
        if post_id:
            client.log_info(f"1️⃣9️⃣.{i+1} Testando exclusão de post")
            result = client.make_request('DELETE', f'/api/feed/posts/{post_id}')
            client.print_test_result(result)
            wait_between_tests()

    # 20. Verificação final
    if created_post_id:
        client.log_info("2️⃣0️⃣ Verificando se post principal foi excluído")
        result = client.make_request('GET', f'/api/feed/posts/{created_post_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Post foi excluído com sucesso")

    return client, created_post_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Feed")
    print("=" * 50)

    try:
        client, created_post_id = test_feed_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("feed_test_report.json")

        client.log_success("Testes de feed concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
