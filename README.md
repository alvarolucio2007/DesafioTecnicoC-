# DesafioTecnicoC#React

Uma aplicação **Fullstack** completa para o gerenciamento e consolidação de transações financeiras individuais e globais. O ecossistema é composto por uma API REST estruturada em .NET 9 no back-end e uma aplicação cliente integrada no front-end.

## Tecnologias Utilizadas

### Back-end
* **Runtime:** .NET 9 (C#) / ASP.NET Core Web API
* **Persistência de Dados:** Dapper (Micro-ORM) & Microsoft.Data.Sqlite
* **Documentação:** Comentários XML nativos

###  Front-end

* **Ambiente de Desenvolvimento:** Node.js (configurado por padrão na porta `5173`).
* **Framework:** React com Vite (garantindo hot-reload ultrarrápido e build otimizado).
* **Linguagem:** TypeScript (configurado com tipagem estrita para garantir consistência e segurança na compilação).
* **Comunicação com a API:** Axios (centralizando as requisições HTTP para os endpoints do back-end em C#).
* **Estilização:** CSS-in-JS / Estilos Inline (adotado para garantir portabilidade absoluta dos componentes, sem dependências externas de pré-processadores ou bibliotecas de terceiros).

## Arquitetura do Back-end

O projeto adota uma separação clara de responsabilidades:
* `DTOs/`: Objetos de Transferência de Dados imutáveis (`records`) totalmente documentados que definem os contratos de entrada e saída da API.
* `Models/`: Modelos de domínio que representam as entidades do banco de dados.
* `Repositories/`: Camada de persistência isolada contendo as queries SQL nativas.
* `Controllers/`: Endpoints expostos para operações transacionais.

---

## Configuração e Execução

### Pré-requisitos
* [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
* [Node.js](https://nodejs.org/)

### Passo a Passo

1. **Clonar o repositório:**
   ```bash
   git clone https://github.com/alvarolucio2007/DesafioTecnicoFluxoDeCaixa.git
   cd DesafioTecnicoFluxoDeCaixa
    ```
   Executar o Back-end:
    Navegue até a pasta `api` e execute os comandos:
    ```bash

    dotnet restore
    dotnet build
    dotnet run
    ```

    API Endpoints: `http://localhost:5262`

    Executar o Front-end:
    Navegue até a pasta `frontend-react` e execute os comandos:
    ```bash

    npm install
    npm run dev
    ```

   URL do Front-end: `http://localhost:5173`
   
---
 Considerações sobre o Banco de Dados (SQLite)

O banco de dados roda localmente através do arquivo database.db localizado na raiz do projeto back-end.

Este arquivo está incluído no .gitignore para evitar que dados locais de testes sejam versionados.

A string de conexão foi configurada de forma relativa para garantir portabilidade imediata em qualquer ambiente de desenvolvimento.


