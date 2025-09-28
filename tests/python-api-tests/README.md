# 🧪 SynQcore API - Suite de Testes Python

Esta é uma suite completa de testes para a API do SynQcore, organizada por funcionalidades e desenvolvida para validar todos os endpoints implementados.

## 📁 Estrutura dos Testes

```
tests/python-api-tests/
├── 📋 requirements.txt                    # Dependências
├── ⚙️  .env                              # Configurações
├── 🔧 utils/
│   └── api_test_utils.py                 # Utilitários de teste
├── 🔐 01-authentication/
│   └── test_auth.py                      # Testes de autenticação
├── 👑 02-administration/
│   └── test_admin.py                     # Testes administrativos
├── 👥 03-employees-departments/
│   ├── test_employees.py                 # Testes de funcionários
│   └── test_departments.py               # Testes de departamentos
├── 📚 04-knowledge-management/
│   ├── test_knowledge_posts.py           # Posts de conhecimento
│   ├── test_knowledge_categories.py      # Categorias
│   └── test_tags.py                      # Tags
├── 🤝 05-collaboration/
│   ├── test_endorsements.py              # Endorsements
│   └── test_discussion_threads.py        # Threads de discussão
├── 📰 06-feed-communication/
│   ├── test_feed.py                      # Feed corporativo
│   └── test_corporate_communication.py   # Comunicação oficial
├── 📁 07-media-documents/
│   └── test_media_assets.py              # Assets de mídia
├── 🔍 08-search-analytics/
│   └── test_corporate_search.py          # Busca corporativa
└── 🚀 run_all_tests.py                   # Execução de todos os testes
```

## 🛠️ Configuração do Ambiente

### 1. Instalação das Dependências

```bash
# Navegar para o diretório de testes
cd tests/python-api-tests/

# Instalar dependências
pip install -r requirements.txt
```

### 2. Configuração do Ambiente (.env)

O arquivo `.env` já está configurado com os valores padrão:

```env
# API Configuration
API_BASE_URL=http://localhost:5000
API_TIMEOUT=30

# Authentication
DEFAULT_EMAIL=admin@synqcore.com
DEFAULT_PASSWORD=SynQcore@Admin123!
DEFAULT_USERNAME=admin

# Test Configuration
VERBOSE_OUTPUT=true
SAVE_DETAILED_REPORTS=true
DELAY_BETWEEN_TESTS=1
```

### 3. Pré-requisitos

- **Python 3.8+**
- **SynQcore API** rodando em `http://localhost:5000`
- **Banco de dados** configurado e acessível
- **Credenciais de admin** configuradas no sistema

## 🚀 Como Executar os Testes

### Execução Completa (Recomendado)

Execute todos os testes de forma sequencial e organizada:

```bash
python run_all_tests.py
```

Este comando irá:

- ✅ Executar todas as 8 categorias de teste
- 📊 Gerar relatórios consolidados
- 💾 Salvar logs detalhados
- 🎯 Fornecer estatísticas completas

### Execução Individual por Categoria

Você pode executar categorias específicas:

```bash
# Autenticação
python 01-authentication/test_auth.py

# Administração
python 02-administration/test_admin.py

# Funcionários
python 03-employees-departments/test_employees.py

# Departamentos
python 03-employees-departments/test_departments.py

# Posts de Conhecimento
python 04-knowledge-management/test_knowledge_posts.py

# Categorias de Conhecimento
python 04-knowledge-management/test_knowledge_categories.py

# Tags
python 04-knowledge-management/test_tags.py

# Endorsements
python 05-collaboration/test_endorsements.py

# Threads de Discussão
python 05-collaboration/test_discussion_threads.py

# Feed
python 06-feed-communication/test_feed.py

# Comunicação Corporativa
python 06-feed-communication/test_corporate_communication.py

# Media Assets
python 07-media-documents/test_media_assets.py

# Busca Corporativa
python 08-search-analytics/test_corporate_search.py
```

## 📊 Funcionalidades Testadas

### 🔐 1. Autenticação

- Login/Logout
- Registro de usuários
- Validação de tokens JWT
- Testes de acesso não autorizado

### 👑 2. Administração

- Gestão de usuários
- Atribuição de roles
- Operações administrativas
- Validações de permissão

### 👥 3. Funcionários e Departamentos

- **Funcionários**: CRUD, busca, hierarquia, soft delete
- **Departamentos**: CRUD, hierarquia, validação de referência circular

### 📚 4. Gestão de Conhecimento

- **Posts**: Criação, edição, curtidas, comentários, analytics
- **Categorias**: Organização, ativação/desativação
- **Tags**: Sistema de marcação, popularidade, busca

### 🤝 5. Colaboração

- **Endorsements**: Validação de conhecimento, aprovação/rejeição
- **Discussões**: Threads, replies, curtidas, seguir

### 📰 6. Feed e Comunicação

- **Feed**: Posts corporativos, tipos diferentes, trending
- **Comunicação**: Comunicados oficiais, prioridades, publicação

### 📁 7. Media e Documentos

- Upload de arquivos múltiplos
- Download e compartilhamento
- Organização por categorias
- Analytics de uso

### 🔍 8. Busca e Analytics

- Busca geral e avançada
- Sugestões automáticas
- Consultas salvas
- Analytics de busca

## 🎯 Cobertura de Testes

Cada módulo testa:

- ✅ **Operações CRUD** completas
- ✅ **Validação de dados** de entrada
- ✅ **Tratamento de erros** e casos edge
- ✅ **Paginação** e filtros
- ✅ **Autenticação** e autorização
- ✅ **Analytics** e métricas
- ✅ **Limpeza** de dados de teste

## 📈 Relatórios Gerados

### Relatórios Individuais

Cada teste gera um relatório JSON com:

- Detalhes de cada requisição
- Tempos de resposta
- Status codes
- Dados de resposta

### Relatório Consolidado

O script `run_all_tests.py` gera:

- `synqcore_complete_test_report_YYYYMMDD_HHMMSS.json`
- Estatísticas por categoria
- Taxa de sucesso geral
- Tempos de execução

## 🎨 Saída Visual

Os testes utilizam cores e emojis para facilitar a leitura:

- ✅ **Verde**: Sucesso
- ❌ **Vermelho**: Erro
- ⚠️ **Amarelo**: Aviso
- 🔵 **Azul**: Informação
- 📊 **Estatísticas**: Resumos e métricas

## 🛠️ Utilitários Incluídos

### APITestClient

Classe principal com:

- 🔐 Autenticação automática
- 📝 Logging estruturado
- 📊 Coleta de métricas
- 🎨 Saída colorida
- 💾 Geração de relatórios

### Funções Auxiliares

- `wait_between_tests()`: Pausa configúrável entre testes
- `create_sample_data()`: Dados de exemplo para testes
- Validação de respostas JSON
- Formatação de outputs

## 🔧 Configurações Avançadas

### Variáveis de Ambiente

| Variável                | Padrão                  | Descrição                     |
| ----------------------- | ----------------------- | ----------------------------- |
| `API_BASE_URL`          | `http://localhost:5000` | URL base da API               |
| `API_TIMEOUT`           | `30`                    | Timeout em segundos           |
| `DEFAULT_EMAIL`         | `admin@synqcore.com`    | Email para autenticação       |
| `DEFAULT_PASSWORD`      | `SynQcore@Admin123!`    | Senha para autenticação       |
| `VERBOSE_OUTPUT`        | `true`                  | Saída detalhada               |
| `SAVE_DETAILED_REPORTS` | `true`                  | Salvar relatórios JSON        |
| `DELAY_BETWEEN_TESTS`   | `1`                     | Delay entre testes (segundos) |

### Personalização

Para personalizar os testes, edite:

- **Dados de teste**: Função `create_sample_data()` em `utils/api_test_utils.py`
- **Configurações**: Arquivo `.env`
- **Endpoints**: URLs nos arquivos de teste individuais

## 🏃‍♂️ Execução Rápida

Para uma validação rápida de todos os endpoints:

```bash
# Executar apenas testes essenciais (implementar se necessário)
python run_all_tests.py --quick

# Executar com menos verbosidade
VERBOSE_OUTPUT=false python run_all_tests.py

# Executar categoria específica
python run_all_tests.py --category=authentication
```

## 🐛 Troubleshooting

### Problemas Comuns

1. **API não está rodando**

   ```
   ConnectionError: Cannot connect to http://localhost:5000
   ```

   **Solução**: Verifique se a API está ativa na porta 5000

2. **Credenciais inválidas**

   ```
   401 Unauthorized
   ```

   **Solução**: Verifique as credenciais no arquivo `.env`

3. **Banco de dados vazio**

   ```
   404 Not Found em vários endpoints
   ```

   **Solução**: Execute as migrations e seed do banco

4. **Timeout nos testes**
   ```
   ReadTimeout: Request timed out
   ```
   **Solução**: Aumente `API_TIMEOUT` no `.env`

### Logs de Debug

Para debug detalhado, ative:

```bash
VERBOSE_OUTPUT=true python test_name.py
```

## 📝 Contribuindo

Para adicionar novos testes:

1. **Crie um novo arquivo** na categoria apropriada
2. **Use o template** do `APITestClient`
3. **Implemente função `main()`** que retorna o client
4. **Adicione ao `run_all_tests.py`** se necessário

### Template de Teste

```python
#!/usr/bin/env python3
"""
Testes de [FEATURE] - SynQcore API
"""

import sys
import os
sys.path.append(os.path.join(os.path.dirname(__file__), '..'))

from utils.api_test_utils import APITestClient, wait_between_tests

def test_feature_endpoints(client: APITestClient = None):
    if not client:
        client = APITestClient()
        if not client.authenticate():
            client.log_error("Falha na autenticação.")
            return client

    client.log_info("🎯 Iniciando testes de [FEATURE]")

    # Seus testes aqui

    return client

def main():
    print("🚀 SynQcore API - Testes de [FEATURE]")
    try:
        client = test_feature_endpoints()
        client.print_summary_report()
        return client
    except Exception as e:
        print(f"Erro: {e}")
        raise

if __name__ == "__main__":
    main()
```

## 🎉 Conclusão

Esta suite de testes fornece uma cobertura completa da API SynQcore, garantindo que todas as funcionalidades implementadas estejam funcionando corretamente. Use-a regularmente para validar mudanças e garantir a qualidade do sistema.

**Tempo estimado de execução completa**: ~15-20 minutos
**Total de endpoints testados**: 150+ endpoints
**Cobertura de funcionalidades**: 100% das features implementadas

---

_Desenvolvido para o projeto SynQcore - Plataforma de Comunicação Corporativa_ 🚀
