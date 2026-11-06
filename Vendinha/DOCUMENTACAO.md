# DOCUMENTAÇÃO — VENDINHA

## 1. Apresentação do projeto

A Vendinha é uma aplicação Backend desenvolvida em C# com .NET 8, com o objetivo de realizar o gerenciamento de clientes e suas respectivas dívidas.

O sistema permite cadastrar clientes, consultar informações, atualizar dados, registrar dívidas e controlar pagamentos parciais ou totais.

## 2. Tecnologias utilizadas

- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- Visual Studio

## 3. Arquitetura do projeto

O projeto está organizado em pastas, separando as responsabilidades de cada parte da aplicação.

### Controllers

Responsáveis por receber as requisições HTTP, executar as operações solicitadas e retornar as respostas da API.

- `ClienteController.cs`: gerencia as operações relacionadas aos clientes.
- `DividaController.cs`: gerencia as operações relacionadas às dívidas e pagamentos.

### Models

Contém as classes que representam os dados utilizados pela aplicação.

- `Cliente.cs`: representa os clientes.
- `Divida.cs`: representa as dívidas dos clientes.

### Contexts

Contém a configuração de acesso ao banco de dados.

- `VendinhaDbContext.cs`: realiza a comunicação entre a aplicação e o banco SQLite por meio do Entity Framework Core.

### Services

Contém serviços auxiliares utilizados pela aplicação.

- `CpfService.cs`: contém a lógica de validação de CPF.

### Migrations

Armazena as migrações utilizadas para criar e atualizar a estrutura do banco de dados.

## 4. Banco de dados

A aplicação utiliza SQLite como sistema de gerenciamento de banco de dados.

A conexão está configurada no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ConexaoVendinha": "Data Source=vendinha.db"
  }
}
```

O banco possui duas tabelas principais:

### Tabela Clientes

Armazena as informações dos clientes.

| Campo | Descrição |
|---|---|
| Id | Identificador do cliente |
| NomeCompleto | Nome completo |
| CPF | CPF do cliente |
| DataNascimento | Data de nascimento |
| Email | Endereço de e-mail |

### Tabela Dividas

Armazena as informações das dívidas.

| Campo | Descrição |
|---|---|
| Id | Identificador da dívida |
| Valor | Valor original da dívida |
| ValorPago | Valor acumulado dos pagamentos |
| Paga | Indica se a dívida foi quitada |
| DataCriacao | Data de criação da dívida |
| DataPagamento | Data da quitação |
| ClienteId | Identificador do cliente relacionado |

O campo `ValorRestante` é calculado automaticamente pela aplicação, subtraindo o valor pago do valor original.

O relacionamento entre as tabelas permite que um cliente possua várias dívidas.

O arquivo `banco.sql` contém o script de criação do schema do banco de dados.

## 5. Funcionalidades do sistema

### Gerenciamento de clientes

- Cadastrar clientes.
- Consultar todos os clientes.
- Consultar cliente por identificador.
- Pesquisar clientes pelo nome.
- Atualizar informações dos clientes.
- Excluir clientes sem histórico de dívidas.
- Impedir CPF duplicado.
- Validar o formato do CPF.
- Calcular automaticamente a idade.

### Gerenciamento de dívidas

- Cadastrar dívidas para clientes.
- Consultar dívidas por cliente.
- Impedir o cadastro de uma nova dívida enquanto existir outra em aberto para o mesmo cliente.
- Registrar pagamentos parciais.
- Calcular automaticamente o valor restante.
- Quitar uma dívida integralmente.
- Impedir pagamentos maiores que o saldo restante.
- Atualizar a situação da dívida após a quitação.

## 6. Endpoints da API

### Clientes

| Método HTTP | Rota | Funcionalidade |
|---|---|---|
| GET | `/api/Cliente` | Lista clientes, com pesquisa e paginação |
| GET | `/api/Cliente/{id}` | Consulta cliente por ID |
| POST | `/api/Cliente` | Cadastra um cliente |
| PUT | `/api/Cliente/{id}` | Atualiza um cliente |
| DELETE | `/api/Cliente/{id}` | Exclui um cliente, respeitando as regras de proteção |

O endpoint de listagem aceita os parâmetros opcionais `busca` e `pagina`.

Exemplo:

`GET /api/Cliente?busca=João&pagina=1`

### Dívidas

| Método HTTP | Rota | Funcionalidade |
|---|---|---|
| GET | `/api/Divida/cliente/{clienteId}` | Lista as dívidas de um cliente |
| POST | `/api/Divida/cliente/{clienteId}` | Cadastra uma dívida |
| PUT | `/api/Divida/{id}/pagamento` | Registra um pagamento parcial |
| PUT | `/api/Divida/{id}/pagar` | Quita uma dívida integralmente |

## 7. Exemplos de utilização

### Cadastro de cliente

Método: POST

Rota: `/api/Cliente`

Exemplo de corpo da requisição:

```json
{
  "nomeCompleto": "João da Silva",
  "cpf": "52998224725",
  "dataNascimento": "2000-05-15T00:00:00",
  "email": "joao@gmail.com"
}
```

### Cadastro de dívida

Método: POST

Rota: `/api/Divida/cliente/1`

Exemplo de corpo da requisição:

```json
{
  "valor": 100
}
```

### Pagamento parcial

Método: PUT

Rota: `/api/Divida/1/pagamento`

Exemplo de corpo da requisição:

```json
{
  "valor": 30
}
```

Nesse exemplo, considerando uma dívida de R$ 100,00, o sistema registra R$ 30,00 pagos e calcula R$ 70,00 restantes.

### Quitação total

Método: PUT

Rota: `/api/Divida/1/pagar`

O sistema registra o pagamento do saldo restante e atualiza a dívida para o estado de quitada.

## 8. Como executar a aplicação

### Pré-requisitos

- .NET 8 SDK instalado.
- Visual Studio ou ambiente compatível com .NET.
- Pacotes necessários do Entity Framework Core.

### Executando pelo Visual Studio

1. Abra a solução `Vendinha.sln`.
2. Defina o projeto `Vendinha` como projeto de inicialização.
3. Restaure os pacotes NuGet.
4. Verifique a configuração da conexão SQLite.
5. Abra o Console do Gerenciador de Pacotes NuGet.
6. Execute `Update-Database` para aplicar as migrações.
7. Pressione F5 para iniciar a aplicação.

### Acessando a documentação Swagger

Com a aplicação em execução, acesse:

`https://localhost:7075/swagger`

O Swagger permite visualizar, enviar e testar as requisições disponíveis na API.

## 9. Considerações finais

A Vendinha foi desenvolvida como projeto acadêmico para aplicar conhecimentos de desenvolvimento Backend com C#, .NET 8 e Programação Orientada a Objetos.

O projeto utiliza separação de responsabilidades, persistência em banco de dados relacional, operações CRUD e regras de negócio para o gerenciamento de clientes e dívidas.