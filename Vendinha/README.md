# Vendinha - Sistema de Gerenciamento de Clientes e Dívidas

## Sobre o projeto

A Vendinha é uma aplicação Backend desenvolvida em C# com .NET 8, criada para realizar o cadastro e gerenciamento de clientes e suas respectivas dívidas.

O sistema permite cadastrar clientes, consultar informações, atualizar dados, gerenciar dívidas e registrar pagamentos parciais ou totais.

## Tecnologias utilizadas

- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- Visual Studio

## Banco de dados

A aplicação utiliza SQLite para armazenar os dados.

A conexão está configurada no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ConexaoVendinha": "Data Source=vendinha.db"
  }
}
```

O banco de dados é gerenciado pelo Entity Framework Core, utilizando migrations.

O projeto também fornece o arquivo `banco.sql`, contendo o script de criação do schema do banco.

## Funcionalidades

### Gerenciamento de clientes

- Cadastrar clientes.
- Consultar todos os clientes.
- Consultar cliente pelo ID.
- Pesquisar clientes pelo nome.
- Atualizar dados dos clientes.
- Excluir clientes sem histórico de dívidas.
- Validar CPF e impedir cadastros duplicados.
- Consultar idade calculada a partir da data de nascimento.

### Gerenciamento de dívidas

- Cadastrar dívidas para clientes.
- Consultar dívidas por cliente.
- Impedir o cadastro de outra dívida enquanto existir uma dívida em aberto.
- Registrar pagamentos parciais.
- Calcular automaticamente o valor restante.
- Quitar uma dívida integralmente.
- Impedir pagamentos acima do valor restante.

## Endpoints da API

### Clientes

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Cliente` | Lista clientes com paginação e pesquisa |
| GET | `/api/Cliente/{id}` | Consulta cliente pelo ID |
| POST | `/api/Cliente` | Cadastra um cliente |
| PUT | `/api/Cliente/{id}` | Atualiza um cliente |
| DELETE | `/api/Cliente/{id}` | Exclui um cliente, respeitando as regras de proteção |

### Dívidas

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Divida/cliente/{clienteId}` | Lista dívidas de um cliente |
| POST | `/api/Divida/cliente/{clienteId}` | Cadastra uma dívida |
| PUT | `/api/Divida/{id}/pagamento` | Registra pagamento parcial |
| PUT | `/api/Divida/{id}/pagar` | Quita a dívida integralmente |

## Exemplos de requisições

### Cadastrar cliente

POST `/api/Cliente`

```json
{
  "nomeCompleto": "João da Silva",
  "cpf": "52998224725",
  "dataNascimento": "2000-05-15T00:00:00",
  "email": "joao@gmail.com"
}
```

### Cadastrar dívida

POST `/api/Divida/cliente/1`

```json
{
  "valor": 100
}
```

### Registrar pagamento parcial

PUT `/api/Divida/1/pagamento`

```json
{
  "valor": 30
}
```

O sistema atualiza o valor pago e calcula automaticamente o saldo restante.

### Quitar dívida

PUT `/api/Divida/1/pagar`

Esse endpoint registra a quitação do valor restante da dívida.

## Como executar o projeto

### Pré-requisitos

- Windows, Linux ou macOS.
- .NET 8 SDK.
- Visual Studio ou editor de código compatível.

### 1. Clonar o repositório

```bash
git clone URL_DO_REPOSITORIO
```

### 2. Acessar a pasta do projeto

Entre na pasta que contém o arquivo `Vendinha.csproj`.

### 3. Restaurar as dependências

```bash
dotnet restore
```

### 4. Atualizar o banco de dados

```bash
dotnet ef database update
```

Esse comando aplica as migrations e cria ou atualiza o banco SQLite.

### 5. Executar a aplicação

```bash
dotnet run
```

### 6. Acessar o Swagger

Abra no navegador o endereço HTTPS apresentado no terminal, acrescentando `/swagger`.

Exemplo:

`https://localhost:7075/swagger`

O Swagger permite visualizar e testar os endpoints da API.

## Estrutura do projeto

```text
Vendinha/
│
├── Contexts/
│   └── VendinhaDbContext.cs
│
├── Controllers/
│   ├── ClienteController.cs
│   └── DividaController.cs
│
├── Models/
│   ├── Cliente.cs
│   └── Divida.cs
│
├── Migrations/
│
├── Services/
│   └── CpfService.cs
│
├── Properties/
│
├── Program.cs
├── appsettings.json
├── banco.sql
└── README.md
```

## Objetivo acadêmico

Este projeto foi desenvolvido com o objetivo de aplicar conhecimentos de programação em C#, desenvolvimento de aplicações Web API, Programação Orientada a Objetos, persistência de dados e integração com banco de dados relacional.

## Autor

Desenvolvido como projeto acadêmico.
