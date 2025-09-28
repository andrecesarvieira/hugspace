#!/usr/bin/env python3
"""
Testes de Threads de Discussão - SynQcore API

Este script testa todos os endpoints de threads de discussão:
- GET /api/discussion-threads
- POST /api/discussion-threads
- GET /api/discussion-threads/{id}
- PUT /api/discussion-threads/{id}
- DELETE /api/discussion-threads/{id}
- POST /api/discussion-threads/{id}/replies
- GET /api/discussion-threads/{id}/replies
- POST /api/discussion-threads/{id}/like
- POST /api/discussion-threads/{id}/follow
- GET /api/discussion-threads/trending

Execução: python test_discussion_threads.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_discussion_threads_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de threads de discussão"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("💬 Iniciando testes de threads de discussão")

    # Dados de exemplo
    timestamp = int(time.time())

    # 1. Teste de listagem de threads
    client.log_info("1️⃣  Testando listagem de threads")
    result = client.make_request('GET', '/api/discussion-threads')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de criação de thread
    client.log_info("2️⃣  Testando criação de thread")
    thread_data = {
        "title": f"Thread de Discussão {timestamp}",
        "content": "Esta é uma thread de discussão para teste. Vamos discutir tópicos importantes da empresa.\n\n**Pontos principais:**\n- Ponto 1\n- Ponto 2\n- Ponto 3",
        "tags": ["discussao", "teste", "corporativo"],
        "categoryId": None,  # Será definido se categoria existir
        "isPublic": True,
        "allowReplies": True,
        "isPinned": False
    }

    result = client.make_request('POST', '/api/discussion-threads', data=thread_data)
    client.print_test_result(result)

    # Armazenar ID da thread criada
    created_thread_id = None
    if result.success and result.response_data:
        created_thread_id = result.response_data.get('id')
        client.log_success(f"Thread criada com ID: {created_thread_id}")

    wait_between_tests()

    # 3. Teste de obtenção de thread por ID
    if created_thread_id:
        client.log_info("3️⃣  Testando obtenção por ID")
        result = client.make_request('GET', f'/api/discussion-threads/{created_thread_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 4. Teste de criação de resposta
    if created_thread_id:
        client.log_info("4️⃣  Testando criação de resposta")
        reply_data = {
            "content": "Esta é uma resposta de teste à thread de discussão. Concordo com os pontos levantados.",
            "isInternal": True
        }
        result = client.make_request('POST', f'/api/discussion-threads/{created_thread_id}/replies', data=reply_data)
        client.print_test_result(result)

        # Armazenar ID da resposta criada
        created_reply_id = None
        if result.success and result.response_data:
            created_reply_id = result.response_data.get('id')

        wait_between_tests()

    # 5. Teste de obtenção de respostas
    if created_thread_id:
        client.log_info("5️⃣  Testando obtenção de respostas")
        result = client.make_request('GET', f'/api/discussion-threads/{created_thread_id}/replies')
        client.print_test_result(result)
        wait_between_tests()

    # 6. Teste de curtir thread
    if created_thread_id:
        client.log_info("6️⃣  Testando curtir thread")
        result = client.make_request('POST', f'/api/discussion-threads/{created_thread_id}/like')
        client.print_test_result(result)
        wait_between_tests()

    # 7. Teste de seguir thread
    if created_thread_id:
        client.log_info("7️⃣  Testando seguir thread")
        result = client.make_request('POST', f'/api/discussion-threads/{created_thread_id}/follow')
        client.print_test_result(result)
        wait_between_tests()

    # 8. Teste de threads em trending
    client.log_info("8️⃣  Testando threads em trending")
    result = client.make_request('GET', '/api/discussion-threads/trending')
    client.print_test_result(result)
    wait_between_tests()

    # 9. Teste de atualização de thread
    if created_thread_id:
        client.log_info("9️⃣  Testando atualização de thread")
        update_data = {
            "title": f"Thread Atualizada {timestamp}",
            "content": "Conteúdo atualizado da thread de discussão com novas informações.",
            "tags": ["discussao", "teste", "atualizado"],
            "isPublic": True,
            "allowReplies": True,
            "isPinned": True  # Fixando a thread
        }
        result = client.make_request('PUT', f'/api/discussion-threads/{created_thread_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 10. Teste de criação de múltiplas respostas
    if created_thread_id:
        client.log_info("🔟 Testando múltiplas respostas")
        replies = [
            {"content": "Primeira resposta adicional", "isInternal": True},
            {"content": "Segunda resposta adicional", "isInternal": False},
            {"content": "Terceira resposta adicional", "isInternal": True}
        ]

        for i, reply_data in enumerate(replies):
            client.log_info(f"🔟.{i+1} Criando resposta {i+1}")
            result = client.make_request('POST', f'/api/discussion-threads/{created_thread_id}/replies', data=reply_data)
            client.print_test_result(result)
            wait_between_tests()

    # 11. Teste de listagem com filtros
    client.log_info("1️⃣1️⃣ Testando listagem com filtros")
    result = client.make_request('GET', '/api/discussion-threads', params={'tags': 'teste', 'isPublic': True})
    client.print_test_result(result)
    wait_between_tests()

    # 12. Teste de listagem com paginação
    client.log_info("1️⃣2️⃣ Testando listagem com paginação")
    result = client.make_request('GET', '/api/discussion-threads', params={'page': 1, 'pageSize': 5})
    client.print_test_result(result)
    wait_between_tests()

    # 13. Teste de busca por título
    client.log_info("1️⃣3️⃣ Testando busca por título")
    result = client.make_request('GET', '/api/discussion-threads', params={'searchTerm': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 14. Teste de threads fixadas
    client.log_info("1️⃣4️⃣ Testando threads fixadas")
    result = client.make_request('GET', '/api/discussion-threads', params={'isPinned': True})
    client.print_test_result(result)
    wait_between_tests()

    # 15. Teste de curtir novamente (descurtir)
    if created_thread_id:
        client.log_info("1️⃣5️⃣ Testando descurtir thread")
        result = client.make_request('POST', f'/api/discussion-threads/{created_thread_id}/like')
        client.print_test_result(result)
        wait_between_tests()

    # 16. Teste de parar de seguir thread
    if created_thread_id:
        client.log_info("1️⃣6️⃣ Testando parar de seguir thread")
        result = client.make_request('POST', f'/api/discussion-threads/{created_thread_id}/follow')
        client.print_test_result(result)
        wait_between_tests()

    # 17. Teste de criação com dados inválidos
    client.log_info("1️⃣7️⃣ Testando criação com dados inválidos")
    invalid_data = {
        "title": "",  # Título vazio
        "content": "",  # Conteúdo vazio
        "tags": [],  # Sem tags
        "isPublic": True,
        "allowReplies": True
    }
    result = client.make_request('POST', '/api/discussion-threads', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 18. Teste de obtenção com ID inválido
    client.log_info("1️⃣8️⃣ Testando obtenção com ID inválido")
    result = client.make_request('GET', '/api/discussion-threads/00000000-0000-0000-0000-000000000000')
    client.print_test_result(result)
    wait_between_tests()

    # 19. Teste de exclusão de thread
    if created_thread_id:
        client.log_info("1️⃣9️⃣ Testando exclusão de thread")
        result = client.make_request('DELETE', f'/api/discussion-threads/{created_thread_id}')
        client.print_test_result(result)
        wait_between_tests()

        # Verificar se foi excluída
        client.log_info("2️⃣0️⃣ Verificando se thread foi excluída")
        result = client.make_request('GET', f'/api/discussion-threads/{created_thread_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Thread foi excluída com sucesso")

    return client, created_thread_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Threads de Discussão")
    print("=" * 50)

    try:
        client, created_thread_id = test_discussion_threads_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("discussion_threads_test_report.json")

        client.log_success("Testes de threads de discussão concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
