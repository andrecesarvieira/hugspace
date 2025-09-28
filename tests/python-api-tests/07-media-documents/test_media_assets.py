#!/usr/bin/env python3
"""
Testes de Media Assets - SynQcore API

Este script testa todos os endpoints de assets de mídia:
- GET /api/media-assets
- POST /api/media-assets/upload
- GET /api/media-assets/{id}
- PUT /api/media-assets/{id}
- DELETE /api/media-assets/{id}
- GET /api/media-assets/{id}/download
- POST /api/media-assets/{id}/share
- GET /api/media-assets/search
- GET /api/media-assets/analytics

Execução: python test_media_assets.py
"""

import sys
import os
import time
import tempfile
import base64
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests, create_sample_data

def create_test_file(filename, content="Conteúdo de teste", extension="txt"):
    """Cria um arquivo de teste temporário"""
    temp_file = tempfile.NamedTemporaryFile(mode='w', suffix=f'.{extension}', delete=False)
    temp_file.write(content)
    temp_file.close()
    return temp_file.name

def test_media_assets_endpoints(client: APITestClient = None):
    """Testa todos os endpoints de media assets"""

    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação. Teste abortado.")
            return client

    client.log_info("📁 Iniciando testes de media assets")

    # Dados de exemplo
    timestamp = int(time.time())

    # 1. Teste de listagem de assets
    client.log_info("1️⃣  Testando listagem de assets")
    result = client.make_request('GET', '/api/media-assets')
    client.print_test_result(result)
    wait_between_tests()

    # 2. Teste de upload de arquivo texto
    client.log_info("2️⃣  Testando upload de arquivo texto")

    # Simulando upload com dados base64 (método alternativo para testes)
    file_content = f"Este é um documento de teste criado em {timestamp}.\n\nConteúdo do arquivo:\n- Linha 1\n- Linha 2\n- Linha 3\n\nFim do arquivo."
    file_base64 = base64.b64encode(file_content.encode()).decode()

    upload_data = {
        "fileName": f"documento-teste-{timestamp}.txt",
        "description": "Documento de teste para upload",
        "category": "Document",  # Document, Image, Video, Audio, Other
        "tags": ["teste", "documento", "upload"],
        "isPublic": True,
        "fileContent": file_base64,  # Conteúdo em base64
        "contentType": "text/plain"
    }

    result = client.make_request('POST', '/api/media-assets/upload', data=upload_data)
    client.print_test_result(result)

    # Armazenar ID do asset criado
    created_asset_id = None
    if result.success and result.response_data:
        created_asset_id = result.response_data.get('id')
        client.log_success(f"Asset criado com ID: {created_asset_id}")

    wait_between_tests()

    # 3. Teste de obtenção de asset por ID
    if created_asset_id:
        client.log_info("3️⃣  Testando obtenção por ID")
        result = client.make_request('GET', f'/api/media-assets/{created_asset_id}')
        client.print_test_result(result)
        wait_between_tests()

    # 4. Teste de atualização de asset
    if created_asset_id:
        client.log_info("4️⃣  Testando atualização de asset")
        update_data = {
            "fileName": f"documento-atualizado-{timestamp}.txt",
            "description": "Documento de teste atualizado",
            "category": "Document",
            "tags": ["teste", "documento", "atualizado"],
            "isPublic": False  # Tornando privado
        }
        result = client.make_request('PUT', f'/api/media-assets/{created_asset_id}', data=update_data)
        client.print_test_result(result)
        wait_between_tests()

    # 5. Teste de download de arquivo
    if created_asset_id:
        client.log_info("5️⃣  Testando download de arquivo")
        result = client.make_request('GET', f'/api/media-assets/{created_asset_id}/download')
        client.print_test_result(result)
        wait_between_tests()

    # 6. Teste de compartilhamento
    if created_asset_id:
        client.log_info("6️⃣  Testando compartilhamento")
        share_data = {
            "shareWithEmployeeIds": [],  # IDs seriam fornecidos se funcionários existissem
            "shareWithDepartmentIds": [],  # IDs seriam fornecidos se departamentos existissem
            "shareWithAll": True,
            "permissions": "ReadOnly",  # ReadOnly, ReadWrite, FullAccess
            "expirationDate": None,
            "message": "Compartilhando documento de teste"
        }
        result = client.make_request('POST', f'/api/media-assets/{created_asset_id}/share', data=share_data)
        client.print_test_result(result)
        wait_between_tests()

    # 7. Teste de upload de diferentes tipos de arquivos
    client.log_info("7️⃣  Testando upload de diferentes tipos")

    file_types = [
        {
            "content": '{"name": "test", "value": 123}',
            "fileName": f"config-{timestamp}.json",
            "contentType": "application/json",
            "category": "Document"
        },
        {
            "content": "Nome,Idade,Cargo\nJoão,30,Desenvolvedor\nMaria,25,Designer",
            "fileName": f"dados-{timestamp}.csv",
            "contentType": "text/csv",
            "category": "Document"
        },
        {
            "content": f"# Documentação {timestamp}\n\n## Seção 1\n\nConteúdo em markdown.\n\n- Item 1\n- Item 2",
            "fileName": f"readme-{timestamp}.md",
            "contentType": "text/markdown",
            "category": "Document"
        }
    ]

    created_additional_assets = []
    for i, file_data in enumerate(file_types):
        client.log_info(f"7️⃣.{i+1} Fazendo upload: {file_data['fileName']}")

        file_base64 = base64.b64encode(file_data['content'].encode()).decode()
        upload_data = {
            "fileName": file_data['fileName'],
            "description": f"Arquivo de teste {file_data['fileName']}",
            "category": file_data['category'],
            "tags": ["teste", "upload", "multiplo"],
            "isPublic": True,
            "fileContent": file_base64,
            "contentType": file_data['contentType']
        }

        result = client.make_request('POST', '/api/media-assets/upload', data=upload_data)
        client.print_test_result(result)

        if result.success and result.response_data:
            created_additional_assets.append(result.response_data.get('id'))

        wait_between_tests()

    # 8. Teste de busca de assets
    client.log_info("8️⃣  Testando busca de assets")
    result = client.make_request('GET', '/api/media-assets/search', params={'query': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 9. Teste de listagem com filtros
    client.log_info("9️⃣  Testando listagem com filtros")
    result = client.make_request('GET', '/api/media-assets', params={'category': 'Document', 'isPublic': True})
    client.print_test_result(result)
    wait_between_tests()

    # 10. Teste de listagem com paginação
    client.log_info("🔟 Testando listagem com paginação")
    result = client.make_request('GET', '/api/media-assets', params={'page': 1, 'pageSize': 5})
    client.print_test_result(result)
    wait_between_tests()

    # 11. Teste de analytics
    client.log_info("1️⃣1️⃣ Testando analytics de assets")
    result = client.make_request('GET', '/api/media-assets/analytics')
    client.print_test_result(result)
    wait_between_tests()

    # 12. Teste de busca por tags
    client.log_info("1️⃣2️⃣ Testando busca por tags")
    result = client.make_request('GET', '/api/media-assets', params={'tags': 'teste'})
    client.print_test_result(result)
    wait_between_tests()

    # 13. Teste de upload com dados inválidos
    client.log_info("1️⃣3️⃣ Testando upload com dados inválidos")
    invalid_data = {
        "fileName": "",  # Nome vazio
        "description": "",
        "category": "InvalidCategory",  # Categoria inválida
        "tags": [],
        "isPublic": True,
        "fileContent": "conteudo-invalido",  # Base64 inválido
        "contentType": ""  # Content type vazio
    }
    result = client.make_request('POST', '/api/media-assets/upload', data=invalid_data)
    client.print_test_result(result)
    wait_between_tests()

    # 14. Teste de obtenção com ID inválido
    client.log_info("1️⃣4️⃣ Testando obtenção com ID inválido")
    result = client.make_request('GET', '/api/media-assets/00000000-0000-0000-0000-000000000000')
    client.print_test_result(result)
    wait_between_tests()

    # 15. Teste de download com ID inválido
    client.log_info("1️⃣5️⃣ Testando download com ID inválido")
    result = client.make_request('GET', '/api/media-assets/00000000-0000-0000-0000-000000000000/download')
    client.print_test_result(result)
    wait_between_tests()

    # 16. Teste de busca sem resultados
    client.log_info("1️⃣6️⃣ Testando busca sem resultados")
    result = client.make_request('GET', '/api/media-assets/search', params={'query': 'termoinexistentequenadadeveencontrar'})
    client.print_test_result(result)
    wait_between_tests()

    # 17. Limpeza - Exclusão dos assets criados
    all_asset_ids = [created_asset_id] + created_additional_assets if created_asset_id else created_additional_assets

    for i, asset_id in enumerate(all_asset_ids):
        if asset_id:
            client.log_info(f"1️⃣7️⃣.{i+1} Testando exclusão de asset")
            result = client.make_request('DELETE', f'/api/media-assets/{asset_id}')
            client.print_test_result(result)
            wait_between_tests()

    # 18. Verificação final
    if created_asset_id:
        client.log_info("1️⃣8️⃣ Verificando se asset foi excluído")
        result = client.make_request('GET', f'/api/media-assets/{created_asset_id}')
        client.print_test_result(result)
        if not result.success:
            client.log_success("Asset foi excluído com sucesso")

    return client, created_asset_id

def main():
    """Função principal de execução dos testes"""
    print("🚀 SynQcore API - Testes de Media Assets")
    print("=" * 50)

    try:
        client, created_asset_id = test_media_assets_endpoints()

        # Gerar relatório final
        client.print_summary_report()

        # Salvar relatório detalhado
        report_file = client.save_detailed_report("media_assets_test_report.json")

        client.log_success("Testes de media assets concluídos!")

        return client

    except KeyboardInterrupt:
        print(f"\n{APITestClient().log_warning('Testes interrompidos pelo usuário')}")
    except Exception as e:
        print(f"\n{APITestClient().log_error(f'Erro durante os testes: {str(e)}')}")
        raise

if __name__ == "__main__":
    main()
