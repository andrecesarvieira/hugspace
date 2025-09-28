#!/usr/bin/env python3
"""
Testes de Comunicação Corporativa - SynQcore API

Este script testa todos os endpoints de comunicação corporativa:
- GET /api/corporate-communication
- POST /api/corporate-communication
- GET /api/corporate-communication/{id}
- PUT /api/corporate-communication/{id}
- DELETE /api/corporate-communication/{id}
- POST /api/corporate-communication/{id}/publish
- POST /api/corporate-communication/{id}/draft
- GET /api/corporate-communication/published
- GET /api/corporate-communication/drafts
- GET /api/corporate-communication/analytics

Execução: python test_corporate_communication.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_corporate_communication_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de comunicação corporativa"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("📢 Iniciando testes de comunicação corporativa")

    # Dados de exemplo
    timestamp = int(time.time())

    # 1. Teste de listagem de comunicações
    client.log_info("1️⃣  Testando listagem de comunicações")
    result = client.make_request('GET', '/api/corporate-communication')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de criação de comunicação corporativa
    client.log_info("2️⃣  Testando criação de comunicação")
    communication_data = {
        "title": f"Comunicado Corporativo {timestamp}",
        "content": f"# Comunicado Importante\n\nEste é um comunicado corporativo de teste criado em {timestamp}.\n\n## Principais Pontos:\n\n- **Ponto 1**: Informação relevante para todos os funcionários\n- **Ponto 2**: Novas diretrizes da empresa\n- **Ponto 3**: Próximos passos e ações\n\n---\n\n*Atenciosamente,*\n*Diretoria*",
        "summary": "Resumo do comunicado corporativo para teste",
        "type": "Announcement",  # Announcement, Policy, News, Alert, Training
        "priority": "High",  # Low, Medium, High, Critical
        "targetAudience": "AllEmployees",  # AllEmployees, Management, Department, Specific
        "departmentIds": [],
        "employeeIds": [],
        "tags": ["comunicado", "teste", "corporativo"],
        "scheduledPublishDate": None,
        "expirationDate": None,
        "isPublished": False,  # Inicialmente como rascunho
        "allowComments": True,
        "allowReactions": True,
        "requireAcknowledgment": False
    }

    result = client.make_request('POST', '/api/corporate-communication', data=communication_data)
    client.print_test_result(result)

    # Armazenar ID da comunicação criada
    created_communication_id = None
    if result.success and result.response_data:
        created_communication_id = result.response_data.get('id')
        client.log_success(f"Comunicação criada com ID: {created_communication_id}")

    wait_between_tests()

    # 3. Teste de obtenção por ID
    if created_communication_id:
        client.log_info("3️⃣  Testando obtenção por ID")
        result = client.make_request('GET', f'/api/corporate-communication/{created_communication_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 4. Teste de listagem de rascunhos
    client.log_info("4️⃣  Testando listagem de rascunhos")
    result = client.make_request('GET', '/api/corporate-communication/drafts')
    client.print_test_result(result)
    wait_between_tests()

    # 5. Teste de publicação de comunicação
    if created_communication_id:
        client.log_info("5️⃣  Testando publicação de comunicação")
        publish_data = {
            "publishNow": True,
            "notifyEmployees": True
        }
        result = client.make_request('POST', f'/api/corporate-communication/{created_communication_id}/publish', data=publish_data)
        client.print_test_result(result)
        wait_between_tests()

    # 6. Teste de listagem de publicações
    client.log_info("6️⃣  Testando listagem de publicações")
    result = client.make_request('GET', '/api/corporate-communication/published')
    client.print_test_result(result)
    wait_between_tests()

    # 7. Teste de atualização de comunicação
    if created_communication_id:
        client.log_info("7️⃣  Testando atualização de comunicação")
        update_data = {
            "title": f"Comunicado Atualizado {timestamp}",
            "content": f"# Comunicado Atualizado\n\nEste comunicado foi atualizado em {timestamp}.\n\n## Atualizações:\n\n- **Nova informação**: Dados atualizados\n- **Correção**: Informação corrigida\n\n*Última atualização: {timestamp}*",
            "summary": "Resumo atualizado do comunicado",
            "type": "Policy",
            "priority": "Medium",
            "targetAudience": "AllEmployees",
            "tags": ["comunicado", "atualizado", "politica"],
            "allowComments": True,
            "allowReactions": True,
            "requireAcknowledgment": True  # Agora requer confirmação
        }
        result = client.make_request('PUT', f'/api/corporate-communication/{created_communication_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 8. Teste de criação de comunicação crítica
    client.log_info("8️⃣  Testando criação de comunicação crítica")
    critical_data = {
        "title": f"ALERTA CRÍTICO {timestamp}",
        "content": f"# 🚨 ALERTA CRÍTICO\n\nEste é um alerta crítico de teste criado em {timestamp}.\n\n**AÇÃO IMEDIATA NECESSÁRIA**\n\n⚠️ Todos os funcionários devem estar cientes desta informação.\n\n## Próximos Passos:\n1. Ler comunicado completo\n2. Confirmar recebimento\n3. Seguir instruções\n\n*Este é apenas um teste*",
        "summary": "Alerta crítico de teste",
        "type": "Alert",
        "priority": "Critical",
        "targetAudience": "AllEmployees",
        "tags": ["alerta", "critico", "teste"],
        "isPublished": True,  # Publicado imediatamente
        "allowComments": False,
        "allowReactions": True,
        "requireAcknowledgment": True
    }

    result = client.make_request('POST', '/api/corporate-communication', data=critical_data)
    client.print_test_result(result)

    created_critical_id = None
    if result.success and result.response_data:
        created_critical_id = result.response_data.get('id')

    wait_between_tests()

    # 9. Teste de comunicação para departamento específico
    client.log_info("9️⃣  Testando comunicação para departamento")
    department_data = {
        "title": f"Comunicado Departamental {timestamp}",
        "content": "Comunicado específico para o departamento de teste.",
        "summary": "Comunicado departamental",
        "type": "News",
        "priority": "Low",
        "targetAudience": "Department",
        "departmentIds": [],  # IDs seriam fornecidos se departamentos existissem
        "tags": ["departamento", "teste"],
        "isPublished": True,
        "allowComments": True,
        "allowReactions": True
    }

    result = client.make_request('POST', '/api/corporate-communication', data=department_data)
    client.print_test_result(result)

    created_department_id = None
    if result.success and result.response_data:
        created_department_id = result.response_data.get('id')

    wait_between_tests()

    # 10. Teste de analytics
    client.log_info("🔟 Testando analytics de comunicação")
    result = client.make_request('GET', '/api/corporate-communication/analytics')
    client.print_test_result(result)
    wait_between_tests()

    # 11. Teste de listagem com filtros
    client.log_info("1️⃣1️⃣ Testando listagem com filtros")
    result = client.make_request('GET', '/api/corporate-communication', params={'type': 'Alert', 'priority': 'Critical'})
    client.print_test_result(result)
    wait_between_tests()

    # 12. Teste de listagem com paginação
    client.log_info("1️⃣2️⃣ Testando listagem com paginação")
    result = client.make_request('GET', '/api/corporate-communication', params={'page': 1, 'pageSize': 5})
    client.print_test_result(result)
    wait_between_tests()

    # 13. Teste de busca
    client.log_info("1️⃣3️⃣ Testando busca")
    result = client.make_request('GET', '/api/corporate-communication', params={'searchTerm': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 14. Teste de retornar para rascunho
    if created_communication_id:
        client.log_info("1️⃣4️⃣ Testando retornar para rascunho")
        result = client.make_request('POST', f'/api/corporate-communication/{created_communication_id}/draft')
        client.print_test_result(result)
        wait_between_tests()

    # 15. Teste de criação com dados inválidos
    client.log_info("1️⃣5️⃣ Testando criação com dados inválidos")
    invalid_data = {
        "title": "",  # Título vazio
        "content": "",  # Conteúdo vazio
        "type": "InvalidType",  # Tipo inválido
        "priority": "InvalidPriority",  # Prioridade inválida
        "targetAudience": "InvalidAudience",  # Audiência inválida
        "tags": []
    }
    result = client.make_request('POST', '/api/corporate-communication', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 16. Teste de obtenção com ID inválido
    client.log_info("1️⃣6️⃣ Testando obtenção com ID inválido")
    result = client.make_request('GET', '/api/corporate-communication/00000000-0000-0000-0000-000000000000')
    client.print_test_result(result)
    wait_between_tests()

    # 17. Limpeza - Exclusão das comunicações criadas
    communication_ids = [created_communication_id, created_critical_id, created_department_id]

    for i, comm_id in enumerate(communication_ids):
        if comm_id:
            client.log_info(f"1️⃣7️⃣.{i+1} Testando exclusão de comunicação")
            result = client.make_request('DELETE', f'/api/corporate-communication/{comm_id}')
            client.print_test_result(result)
            wait_between_tests()

    # 18. Verificação final
    if created_communication_id:
        client.log_info("1️⃣8️⃣ Verificando se comunicação foi excluída")
        result = client.make_request('GET', f'/api/corporate-communication/{created_communication_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Comunicação foi excluída com sucesso")

    return client, created_communication_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Comunicação Corporativa")
    print("=" * 50)

    try:
        client, created_communication_id = test_corporate_communication_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("corporate_communication_test_report.json")

        client.log_success("Testes de comunicação corporativa concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
