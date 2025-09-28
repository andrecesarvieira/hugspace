#!/usr/bin/env python3
"""
Suite de Testes Completa - SynQcore API

Este script executa todos os testes da API de forma organizada e sequencial,
gerando relatórios consolidados de todos os endpoints testados.

Estrutura dos Testes:
1. 🔐 Autenticação
2. 👑 Administração
3. 👥 Funcionários e Departamentos
4. 📚 Gestão de Conhecimento
5. 🤝 Colaboração
6. 📰 Feed e Comunicação Corporativa
7. 📁 Media e Documentos
8. 🔍 Busca e Analytics

Execução: python run_all_tests.py
"""

import sys
import os
import time
import json
from datetime import datetime

# Adicionar o diretório utils ao path
sys.path.append(os.path.join(os.path.dirname(__file__), 'utils'))

from utils.api_test_utils import APITestClient

def run_test_category(category_name, category_path, test_files, global_client=None):
    """Executa uma categoria completa de testes"""
    print(f"\n{'='*60}")
    print(f"🚀 CATEGORIA: {category_name}")
    print(f"{'='*60}")

    category_results = {
        'category': category_name,
        'path': category_path,
        'tests': [],
        'total_tests': 0,
        'passed_tests': 0,
        'failed_tests': 0,
        'start_time': datetime.now().isoformat(),
        'end_time': None,
        'duration': None
    }

    start_time = time.time()

    # Usar cliente global ou criar um novo
    client = global_client if global_client else APITestClient()

    for test_file in test_files:
        test_module_path = f"{category_path}.{test_file}"
        print(f"\n🔹 Executando: {test_file}")
        print("-" * 50)

        try:
            # Importar e executar o módulo de teste
            module = __import__(test_module_path, fromlist=[test_file])

            # Executar a função main do módulo se existir
            if hasattr(module, 'main'):
                test_client = module.main()

                # Coletar resultados do teste
                if hasattr(test_client, 'test_results'):
                    test_result = {
                        'test_name': test_file,
                        'total_requests': len(test_client.test_results),
                        'successful_requests': sum(1 for r in test_client.test_results if r.success),
                        'failed_requests': sum(1 for r in test_client.test_results if not r.success),
                        'results': [
                            {
                                'method': r.method,
                                'endpoint': r.endpoint,
                                'status_code': r.status_code,
                                'success': r.success,
                                'duration': r.duration
                            } for r in test_client.test_results
                        ]
                    }

                    category_results['tests'].append(test_result)
                    category_results['total_tests'] += test_result['total_requests']
                    category_results['passed_tests'] += test_result['successful_requests']
                    category_results['failed_tests'] += test_result['failed_requests']

                print(f"✅ {test_file} concluído com sucesso!")

            else:
                print(f"⚠️  Módulo {test_file} não possui função main()")

        except ImportError as e:
            print(f"❌ Erro ao importar {test_file}: {e}")
        except Exception as e:
            print(f"❌ Erro ao executar {test_file}: {e}")

        # Pausa entre testes
        time.sleep(2)

    end_time = time.time()
    category_results['end_time'] = datetime.now().isoformat()
    category_results['duration'] = round(end_time - start_time, 2)

    # Resumo da categoria
    print(f"\n📊 RESUMO - {category_name}:")
    print(f"   Total de testes: {category_results['total_tests']}")
    print(f"   ✅ Sucessos: {category_results['passed_tests']}")
    print(f"   ❌ Falhas: {category_results['failed_tests']}")
    print(f"   ⏱️  Duração: {category_results['duration']:.2f}s")

    return category_results, client

def main():
    """Função principal que executa toda a suite de testes"""
    print("🚀 SynQcore API - Suite Completa de Testes")
    print("=" * 60)
    print(f"Início dos testes: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print("=" * 60)

    # Configuração dos testes por categoria
    test_categories = [
        {
            'name': '🔐 Autenticação',
            'path': '01-authentication',
            'files': ['test_auth']
        },
        {
            'name': '👑 Administração',
            'path': '02-administration',
            'files': ['test_admin']
        },
        {
            'name': '👥 Funcionários e Departamentos',
            'path': '03-employees-departments',
            'files': ['test_employees', 'test_departments']
        },
        {
            'name': '📚 Gestão de Conhecimento',
            'path': '04-knowledge-management',
            'files': ['test_knowledge_posts', 'test_knowledge_categories', 'test_tags']
        },
        {
            'name': '🤝 Colaboração',
            'path': '05-collaboration',
            'files': ['test_endorsements', 'test_discussion_threads']
        },
        {
            'name': '📰 Feed e Comunicação Corporativa',
            'path': '06-feed-communication',
            'files': ['test_feed', 'test_corporate_communication']
        },
        {
            'name': '📁 Media e Documentos',
            'path': '07-media-documents',
            'files': ['test_media_assets']
        },
        {
            'name': '🔍 Busca e Analytics',
            'path': '08-search-analytics',
            'files': ['test_corporate_search']
        }
    ]

    # Resultados consolidados
    consolidated_results = {
        'execution_summary': {
            'start_time': datetime.now().isoformat(),
            'end_time': None,
            'total_duration': None,
            'total_categories': len(test_categories),
            'total_test_files': sum(len(cat['files']) for cat in test_categories)
        },
        'categories': []
    }

    start_time = time.time()
    global_client = None

    try:
        # Executar cada categoria de testes
        for i, category in enumerate(test_categories, 1):
            print(f"\n🎯 CATEGORIA {i}/{len(test_categories)}: {category['name']}")

            category_results, client = run_test_category(
                category['name'],
                category['path'],
                category['files'],
                global_client
            )

            consolidated_results['categories'].append(category_results)

            # Manter o cliente autenticado para próximas categorias
            if not global_client and client:
                global_client = client

            # Pausa entre categorias
            if i < len(test_categories):
                print(f"\n⏳ Pausa de 5 segundos antes da próxima categoria...")
                time.sleep(5)

    except KeyboardInterrupt:
        print(f"\n🛑 Testes interrompidos pelo usuário")
        return
    except Exception as e:
        print(f"\n❌ Erro durante execução dos testes: {e}")
        return

    # Finalização
    end_time = time.time()
    total_duration = end_time - start_time

    consolidated_results['execution_summary']['end_time'] = datetime.now().isoformat()
    consolidated_results['execution_summary']['total_duration'] = round(total_duration, 2)

    # Calcular estatísticas finais
    total_tests = sum(cat.get('total_tests', 0) for cat in consolidated_results['categories'])
    total_passed = sum(cat.get('passed_tests', 0) for cat in consolidated_results['categories'])
    total_failed = sum(cat.get('failed_tests', 0) for cat in consolidated_results['categories'])
    success_rate = (total_passed / total_tests * 100) if total_tests > 0 else 0

    # Relatório final
    print(f"\n{'='*60}")
    print("🎉 RELATÓRIO FINAL - SUITE COMPLETA DE TESTES")
    print(f"{'='*60}")
    print(f"📅 Data/Hora: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print(f"⏱️  Duração total: {total_duration:.2f} segundos ({total_duration/60:.1f} minutos)")
    print(f"📁 Categorias testadas: {len(test_categories)}")
    print(f"🧪 Total de testes: {total_tests}")
    print(f"✅ Sucessos: {total_passed}")
    print(f"❌ Falhas: {total_failed}")
    print(f"📊 Taxa de sucesso: {success_rate:.1f}%")
    print(f"{'='*60}")

    # Detalhamento por categoria
    print("\n📋 DETALHAMENTO POR CATEGORIA:")
    for category in consolidated_results['categories']:
        category_success_rate = (category['passed_tests'] / category['total_tests'] * 100) if category['total_tests'] > 0 else 0
        print(f"  {category['category']}: {category['passed_tests']}/{category['total_tests']} ({category_success_rate:.1f}%) - {category['duration']:.1f}s")

    # Salvar relatório consolidado
    timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
    report_filename = f"synqcore_complete_test_report_{timestamp}.json"

    try:
        with open(report_filename, 'w', encoding='utf-8') as f:
            json.dump(consolidated_results, f, indent=2, ensure_ascii=False)
        print(f"\n💾 Relatório detalhado salvo em: {report_filename}")
    except Exception as e:
        print(f"\n⚠️  Erro ao salvar relatório: {e}")

    # Mensagem final
    if total_failed == 0:
        print(f"\n🎉 PARABÉNS! Todos os {total_tests} testes foram executados com sucesso!")
    else:
        print(f"\n⚠️  {total_failed} teste(s) falharam. Verifique os logs para mais detalhes.")

    print("\n✨ Suite de testes concluída!")

    return consolidated_results

if __name__ == "__main__":
    main()
