# Sistema de Emissão de Notas Fiscais - Arquitetura de Microsserviços

## 📋 Visão Geral

Solução Full-Stack desenvolvida com arquitetura de **Microsserviços** para emissão de Notas Fiscais. O sistema é composto por dois microsserviços principais:

- **Microsserviço de Estoque**: Gerencia o catálogo de produtos e a redução automática de inventário
- **Microsserviço de Faturamento**: Responsável pela emissão, gestão e fechamento de Notas Fiscais

Esta arquitetura garante separação clara de responsabilidades, escalabilidade independente e resiliência entre os serviços.

---

## 🎯 Detalhamento Técnico

Conforme solicitado no escopo do teste, abaixo estão as definições arquiteturais e técnicas aplicadas:

### 1. Frontend (Angular 17+)
*   **Ciclos de Vida Utilizados:** O hook `OnInit` foi utilizado nos componentes principais (como lista de notas e produtos) através do método `ngOnInit()`, sendo responsável por disparar as chamadas HTTP e carregar o estado inicial da tela no momento em que o componente é renderizado.
*   **Uso do RxJS e Signals:** O RxJS foi o pilar da comunicação assíncrona na camada de Services. Operadores como `switchMap` (para evitar race conditions), `catchError` (para tratamento de falhas) e `map` (para projeção de DTOs) foram amplamente utilizados. Além disso, utilizamos **Angular Signals** para reatividade de última geração e atualização automática da interface.
*   **Bibliotecas Visuais:** Para demonstrar domínio em CSS/SCSS e HTML5 semântico, **nenhuma biblioteca de componentes visuais externa** (como Material ou Bootstrap) foi utilizada. A estilização e responsividade foram construídas de forma pura e nativa.

### 2. Backend (C# .NET & PostgreSQL)
*   **Frameworks e Arquitetura:** Backend totalmente desenvolvido em C# (ASP.NET Core), utilizando Entity Framework Core como ORM e PostgreSQL. O projeto segue os princípios da **Clean Architecture** (Domain, Application, Infrastructure, Presentation).
*   **Tratamento de Erros e Exceções:** Implementado um `GlobalExceptionMiddleware`. Em vez de espalhar blocos `try/catch`, este interceptador global captura falhas na esteira HTTP, loga o erro e devolve um JSON padronizado ao frontend (impedindo o vazamento de StackTrace).
*   **Uso de LINQ:** O LINQ foi utilizado para otimização de consultas e manipulação de memória. Exemplo prático: projeção de coleções usando `.Select()` para transformar entidades complexas do domínio em DTOs leves antes do tráfego de rede entre microsserviços.

### 3. Resiliência e Tratamento de Falhas (Polly)
Caso o Serviço de Estoque falhe durante a emissão de uma nota, o sistema foi projetado para se recuperar. Utilizamos a biblioteca **Polly** no Microsserviço de Faturamento:
*   **WaitAndRetryAsync (Backoff Exponencial):** O sistema tenta refazer a requisição automaticamente em caso de falha de rede.
*   Se o erro persistir, a transação é interrompida (garantindo que a nota continue "Aberta"), o Middleware captura a falha de integração e o frontend exibe um feedback visual claro para o usuário tentar a impressão novamente.

### 4. Requisito Opcional: Tratamento de Concorrência
Para garantir consistência em cenários onde múltiplas notas tentam baixar o saldo de um mesmo produto simultaneamente, foi implementado um **Bloqueio Transacional (Lock)** no Serviço de Estoque. A transação só realiza o *commit* se a atualização de saldo e a geração do histórico ocorrerem com sucesso (prevenindo o fenômeno *Lost Update*).

---

## ✨ Funcionalidades Principais

### CRUD de Estoque
- ✅ **Criar**: Adicionar novos produtos ao catálogo
- ✅ **Listar**: Visualizar produtos com paginação e filtros
- ✅ **Atualizar**: Modificar informações de produtos (nome, preço, quantidade)
- ✅ **Deletar**: Remover produtos do catálogo
- ✅ **Consultar**: Busca inteligente com múltiplos critérios

### Fluxo de Emissão de Nota Fiscal

#### 1. Abertura da Nota
- Seleção de produtos disponíveis
- Cálculo automático de valores (subtotal, impostos, total)
- Status inicial: **Aberta**

#### 2. Baixa Automática de Estoque
- Integração entre Microsserviço de Faturamento e Estoque
- Redução automática da quantidade disponível ao confirmar a nota
- Validação de disponibilidade antes da emissão
- Tratamento resiliente com retry automático via Polly

#### 3. Fechamento da Nota
- Finalização com status **Fechada**
- Impossibilidade de modificações posteriores
- Geração de documentos e logs de auditoria
- Sincronização com sistema de estoque

---

## 🚀 Como Rodar o Projeto

### Pré-requisitos
- .NET 10
- Node.js 18+ e npm/yarn
- PostgreSQL 14+
- Angular CLI (`npm install -g @angular/cli`)

### Backend - Visual Studio

#### 1. Configurar Banco de Dados
```bash
# No Visual Studio, abra o Package Manager Console
# Navegue até o projeto de Infrastructure

# Execute as migrations
Update-Database
```

#### 2. Restaurar Dependências
- Abra a Solution no Visual Studio
- A restauração de pacotes NuGet ocorre automaticamente

#### 3. Rodar os Microsserviços

**Microsserviço de Estoque:**
- Defina como projeto de inicialização: `EstoqueAPI` ou conforme nomeado
- Pressione `F5` ou clique em "Start Debugging"
- A API estará disponível em `https://localhost:7208`

**Microsserviço de Faturamento:**
- Altere o projeto de inicialização para `FaturamentoAPI` ou conforme nomeado
- Pressione `F5`
- A API estará disponível em `https://localhost:7099`

#### 4. Verificar Saúde das APIs
```bash
# Teste os endpoints de health check
curl https://localhost:7208/health
curl https://localhost:7099/health
```

### Frontend - Angular

#### 1. Instalar Dependências
```bash
cd frontend
npm install
```

#### 2. Configurar Variáveis de Ambiente
- Verifique o arquivo `src/environments/environment.ts`
- Certifique-se de que as URLs das APIs estão corretas:
  ```typescript
  export const environment = {
    production: false,
    apiEstoqueUrl: 'https://localhost:7208',
    apiFaturamentoUrl: 'https://localhost:7099'
  };
  ```

#### 3. Rodar o Servidor de Desenvolvimento
```bash
ng serve
# ou
npm start
```

#### 4. Acessar a Aplicação
- Abra o navegador e acesse: `http://localhost:4200`
- A aplicação recarregará automaticamente a cada alteração de código (Hot Reload)

### Estrutura Geral de Execução

```
┌─────────────────────────────────────────┐
│         Frontend (Angular 17+)          │
│      http://localhost:4200              │
└────────────────┬────────────────────────┘
                 │
        ┌────────┴────────┐
        │                 │
┌───────▼────────┐  ┌────▼──────────┐
│  Estoque API   │  │ Faturamento   │
│ :7208 (Clean   │  │    API :7099  │
│   Architecture)│  │  (Com Polly)  │
└────────┬───────┘  └────┬──────────┘
         │               │
         └───────┬───────┘
                 │
         ┌───────▼────────┐
         │  PostgreSQL    │
         │    Database    │
         └────────────────┘
```

---

## 📝 Notas Importantes

### Comunicação Entre Microsserviços
- As requisições entre microsserviços são protegidas por Polly
- Em caso de falha temporária, há retry automático com backoff exponencial
- Circuit Breaker protege contra falhas em cascata

### Reatividade em Tempo Real
- O frontend utiliza Signals para atualização instantânea da interface
- Operadores RxJS garantem gerenciamento eficiente de requisições assíncronas
- Cancelamento automático de requisições obsoletas via `switchMap`

### Tratamento de Erros
- Erro de rede: Tratado com `catchError` e notificação ao usuário
- Falha de estoque: Validação prévia antes de emitir nota
- Banco de dados indisponível: Retry automático via Polly

---

## 📞 Suporte

Para dúvidas ou problemas durante a execução:
1. Verifique as portas (7208, 7099, 4200) estão disponíveis
2. Certifique-se que PostgreSQL está em execução
3. Valide as strings de conexão no `appsettings.json`
4. Consulte os logs no Visual Studio e console do Angular

---

**Versão**: 1.0.0  
**Data**: 2026  
**Autor**: Diego Dias Inocencio
