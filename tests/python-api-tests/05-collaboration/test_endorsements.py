#!/usr/bin/env python3
"""
Testes de Endorsements - SynQcore API

Este script testa todos os endpoints de endorsements (validações de conhecimento):
- GET /api/endorsements
- POST /api/endorsements
- GET /api/endorsements/{id}
- PUT /api/endorsements/{id}
- DELETE /api/endorsements/{id}
- GET /api/endorsements/pending
- GET /api/endorsements/received
- GET /api/endorsements/given
- POST /api/endorsements/{id}/approve
- POST /api/endorsements/{id}/reject

Execução: python test_endorsements.py
"""

import sys
import os
import time
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def test_endorsements_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de endorsements"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("🤝 Iniciando testes de endorsements")

    # Dados de exemplo
    timestamp = int(time.time())

    # 1. Teste de listagem de endorsements
    client.log_info("1️⃣  Testando listagem de endorsements")
    result = client.make_request('GET', '/api/endorsements')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de criação de endorsement
    client.log_info("2️⃣  Testando criação de endorsement")
    endorsement_data = {
        "endorsedEmployeeId": None,  # Será definido dinamicamente se possível
        "skillName": f"Skill Teste {timestamp}",
        "description": "Descrição do endorsement de teste para validação de conhecimento",
        "level": "Intermediate",  # Beginner, Intermediate, Advanced, Expert
        "evidenceUrl": "https://exemplo.com/evidencia",
        "isPublic": True
    }

    result = client.make_request('POST', '/api/endorsements', data=endorsement_data)
    client.print_test_result(result)

    # Armazenar ID do endorsement criado
    created_endorsement_id = None
    if result.success and result.response_data:
        created_endorsement_id = result.response_data.get('id')
        client.log_success(f"Endorsement criado com ID: {created_endorsement_id}")

    wait_between_tests()

    # 3. Teste de obtenção de endorsement por ID
    if created_endorsement_id:
        client.log_info("3️⃣  Testando obtenção por ID")
        result = client.make_request('GET', f'/api/endorsements/{created_endorsement_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 4. Teste de endorsements pendentes
    client.log_info("4️⃣  Testando endorsements pendentes")
    result = client.make_request('GET', '/api/endorsements/pending')
    client.print_test_result(result)
    wait_between_tests()

    # 5. Teste de endorsements recebidos
    client.log_info("5️⃣  Testando endorsements recebidos")
    result = client.make_request('GET', '/api/endorsements/received')
    client.print_test_result(result)
    wait_between_tests()

    # 6. Teste de endorsements dados
    client.log_info("6️⃣  Testando endorsements dados")
    result = client.make_request('GET', '/api/endorsements/given')
    client.print_test_result(result)
    wait_between_tests()

    # 7. Teste de aprovação de endorsement
    if created_endorsement_id:
        client.log_info("7️⃣  Testando aprovação de endorsement")
        approval_data = {
            "comment": "Endorsement aprovado após validação",
            "verificationNotes": "Verificado através de evidências fornecidas"
        }
        result = client.make_request('POST', f'/api/endorsements/{created_endorsement_id}/approve', data=approval_data)
        client.print_test_result(result)
        wait_between_tests()

    # 8. Teste de atualização de endorsement
    if created_endorsement_id:
        client.log_info("8️⃣  Testando atualização de endorsement")
        update_data = {
            "skillName": f"Skill Atualizada {timestamp}",
            "description": "Descrição atualizada do endorsement",
            "level": "Advanced",
            "evidenceUrl": "https://exemplo.com/evidencia-atualizada",
            "isPublic": True
        }
        result = client.make_request('PUT', f'/api/endorsements/{created_endorsement_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 9. Teste de criação de endorsement com nível diferente
    client.log_info("9️⃣  Testando criação com nível Expert")
    expert_endorsement_data = {
        "endorsedEmployeeId": None,
        "skillName": f"Expert Skill {timestamp}",
        "description": "Endorsement de nível expert para teste",
        "level": "Expert",
        "evidenceUrl": "https://exemplo.com/evidencia-expert",
        "isPublic": True
    }

    result = client.make_request('POST', '/api/endorsements', data=expert_endorsement_data)
    client.print_test_result(result)

    created_expert_endorsement_id = None
    if result.success and result.response_data:
        created_expert_endorsement_id = result.response_data.get('id')

    wait_between_tests()

    # 10. Teste de rejeição de endorsement
    if created_expert_endorsement_id:
        client.log_info("🔟 Testando rejeição de endorsement")
        rejection_data = {
            "comment": "Endorsement rejeitado - evidências insuficientes",
            "reason": "Faltam evidências concretas da competência"
        }
        result = client.make_request('POST', f'/api/endorsements/{created_expert_endorsement_id}/reject', data=rejection_data)
        client.print_test_result(result)
        wait_between_tests()

    # 11. Teste de listagem com filtros
    client.log_info("1️⃣1️⃣ Testando listagem com filtros")
    result = client.make_request('GET', '/api/endorsements', params={'level': 'Advanced', 'status': 'Approved'})
    client.print_test_result(result)
    wait_between_tests()

    # 12. Teste de listagem com paginação
    client.log_info("1️⃣2️⃣ Testando listagem com paginação")
    result = client.make_request('GET', '/api/endorsements', params={'page': 1, 'pageSize': 5})
    client.print_test_result(result)
    wait_between_tests()

    # 13. Teste de criação com dados inválidos
    client.log_info("1️⃣3️⃣ Testando criação com dados inválidos")
    invalid_data = {
        "endorsedEmployeeId": "00000000-0000-0000-0000-000000000000",  # ID inválido
        "skillName": "",  # Nome vazio
        "description": "",  # Descrição vazia
        "level": "InvalidLevel",  # Nível inválido
        "evidenceUrl": "url-inválida",  # URL inválida
        "isPublic": True
    }
    result = client.make_request('POST', '/api/endorsements', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 14. Teste de busca por skill
    client.log_info("1️⃣4️⃣ Testando busca por skill")
    result = client.make_request('GET', '/api/endorsements', params={'searchTerm': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 15. Teste de obtenção com ID inválido
    client.log_info("1️⃣5️⃣ Testando obtenção com ID inválido")
    result = client.make_request('GET', '/api/endorsements/00000000-0000-0000-0000-000000000000')
    client.print_test_result(result)
    wait_between_tests()

    # 16. Limpeza - Exclusão dos endorsements criados
    endorsement_ids = [created_endorsement_id, created_expert_endorsement_id]

    for i, endorsement_id in enumerate(endorsement_ids):
        if endorsement_id:
            client.log_info(f"1️⃣6️⃣.{i+1} Testando exclusão de endorsement")
            result = client.make_request('DELETE', f'/api/endorsements/{endorsement_id}')
            client.print_test_result(result)
            wait_between_tests()

    # 17. Verificação final
    if created_endorsement_id:
        client.log_info("1️⃣7️⃣ Verificando se endorsement foi excluído")
        result = client.make_request('GET', f'/api/endorsements/{created_endorsement_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Endorsement foi excluído com sucesso")

    return client, created_endorsement_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Endorsements")
    print("=" * 50)

    try:
        client, created_endorsement_id = test_endorsements_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("endorsements_test_report.json")

        client.log_success("Testes de endorsements concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
