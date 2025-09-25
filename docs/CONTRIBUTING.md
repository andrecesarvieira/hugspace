# Contribuindo para o SynQcore

Antes de tudo, obrigado por considerar contribuir para o SynQcore! 🎉

**SynQcore** é criado e mantido por **[André César Vieira](https://github.com/andrecesarvieira)**, e damos boas-vindas às contribuições da comunidade para tornar esta plataforma de rede social corporativa ainda melhor.

## 👨‍💻 Sobre o Criador do Projeto

**André César Vieira** é um arquiteto de software empresarial com ampla experiência em:
- Ecossistema .NET e Arquitetura Limpa
- Otimização PostgreSQL e design de banco de dados
- Desenvolvimento de aplicações corporativas
- Engenharia de performance e escalabilidade

## 🤝 Como Contribuir

### 1. 🐛 Relatando Bugs

Antes de criar relatórios de bugs, verifique as [issues existentes](https://github.com/andrecesarvieira/synqcore/issues) para evitar duplicatas.

Ao criar um relatório de bug, inclua:
- Descrição clara do problema
- Passos para reproduzir o problema
- Comportamento esperado vs. real
- Detalhes do ambiente (SO, versão .NET, etc.)
- Logs ou screenshots relevantes

### 2. 💡 Sugerindo Funcionalidades

Damos boas-vindas a sugestões de funcionalidades! Por favor:
- Verifique primeiro as solicitações de funcionalidades existentes
- Forneça caso de uso claro e valor de negócio
- Considere como se encaixa nos objetivos da rede social corporativa
- Inclua mockups ou exemplos se úteis

### 3. 🔧 Contribuições de Código

#### Primeiros Passos

1. **Faça fork** do repositório
2. **Clone** seu fork localmente
3. **Crie** uma branch de funcionalidade a partir da `master`
4. **Configure** o ambiente de desenvolvimento:
   ```bash
   # Iniciar infraestrutura
   ./scripts/start-dev.sh
   
   # Aplicar migrações
   dotnet ef database update -p src/SynQcore.Infrastructure -s src/SynQcore.Api
   
   # Executar testes
   dotnet test
   ```

#### Diretrizes de Desenvolvimento

**Princípios de Arquitetura:**
- Siga padrões de **Arquitetura Limpa**
- Mantenha **separação de responsabilidades**
- Use padrão **CQRS** para operações complexas
- Implemente **tratamento adequado de erros**
- Escreva **testes abrangentes**

**Padrões de Código:**
- Use recursos da linguagem **C# 12** apropriadamente
- Siga **convenções de codificação C# da Microsoft**
- Adicione **documentação XML** para APIs públicas
- Mantenha **formatação consistente** (EditorConfig)
- Tenha **performance** em mente (use LoggerMessage, etc.)

**Diretrizes de Banco de Dados:**
- Use migrações do **Entity Framework Core**
- Siga **melhores práticas do PostgreSQL**
- Inclua **índices adequados** para performance
- Escreva **consultas eficientes**

#### Processo de Pull Request

1. **Atualize** a documentação se necessário
2. **Adicione testes** para novas funcionalidades
3. **Garanta** que todos os testes passem
4. **Siga** convenções de mensagens de commit:
   ```
   feat: adicionar endpoint de transferência de departamento do funcionário
   fix: resolver problema de bypass do rate limiting
   docs: atualizar documentação da API para autenticação
   ```
5. **Crie** pull request com:
   - Descrição clara das mudanças
   - Referência a issues relacionadas
   - Screenshots se houver mudanças na UI
   - Notas de impacto na performance se relevante

## 🧪 Diretrizes de Testes

### Executando Testes
```bash
# Executar todos os testes
dotnet test

# Executar projeto de teste específico
dotnet test tests/SynQcore.UnitTests
dotnet test tests/SynQcore.IntegrationTests

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Escrevendo Testes
- **Testes unitários** para lógica de negócio
- **Testes de integração** para endpoints de API
- **Testes de performance** para caminhos críticos
- **Mock de dependências externas** apropriadamente

## 📝 Documentação

### Documentação da API
- Mantenha definições **Swagger/OpenAPI** atualizadas
- Inclua **exemplos de request/response**
- Documente **cenários de erro**
- Explique **rate limiting** e **autenticação**

### Documentação de Código
- **Comentários XML** para APIs públicas
- **Atualizações de README** para novas funcionalidades
- **Registros de decisões de arquitetura** para mudanças significativas

## 🎯 Áreas Procurando Contribuições

### Alta Prioridade
- **Sistema de autenticação** (JWT, papéis, permissões)
- **Funcionalidades em tempo real** (implementação SignalR)
- **Manipulação de upload/mídia** de arquivos
- **Capacidades de busca avançada**
- **Otimizações de performance**

### Documentação e Exemplos
- **Conteúdo de tutorial** para cenários comuns
- **Guias de arquitetura** e melhores práticas
- **Guias de deployment** para diferentes ambientes
- **Exemplos de uso da API** em diferentes linguagens

### Testes e Qualidade
- **Aumentar cobertura de testes** (meta: >80%)
- **Benchmarks de performance** e monitoramento
- **Testes de segurança** e hardening
- **Melhorias de acessibilidade**

## 🏆 Reconhecimento

Contribuidores serão:
- **Listados** no CONTRIBUTORS.md
- **Mencionados** nas notas de release
- **Reconhecidos** na documentação do projeto
- **Convidados** para se juntar à equipe de contribuidores principais (para contribuições significativas)

## 📞 Obtendo Ajuda

- **GitHub Issues** - Para bugs e solicitações de funcionalidades
- **GitHub Discussions** - Para perguntas e discussão geral
- **Email André** - [andrecesarvieira@hotmail.com](mailto:andrecesarvieira@hotmail.com) para comunicação direta

## 📋 Código de Conduta

### Nosso Compromisso
Estamos comprometidos em fornecer uma comunidade acolhedora e inspiradora para todos.

### Comportamento Esperado
- **Seja respeitoso** e inclusivo
- **Dê boas-vindas aos novatos** e os ajude a começar
- **Foque em feedback construtivo**
- **Reconheça contribuições** de outros
- **Priorize objetivos do projeto** sobre preferências pessoais

### Comportamento Inaceitável
- Assédio, discriminação ou comportamento tóxico
- Spam, auto-promoção não relacionada ao projeto
- Publicar informações privadas de outros
- Qualquer conduta inadequada em um ambiente profissional

## 🎉 Obrigado!

Toda contribuição importa, desde corrigir erros de digitação até implementar funcionalidades importantes. Obrigado por ajudar a tornar o SynQcore a melhor plataforma de rede social corporativa de código aberto!

---

**"Construindo o futuro da colaboração corporativa, juntos."**  
*- André César Vieira & a Comunidade SynQcore*