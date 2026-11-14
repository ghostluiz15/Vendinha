# 🛒 Vendinha

Sistema de gerenciamento de clientes e dívidas desenvolvido em C#.

A versão principal do projeto utiliza **Windows Forms**, permitindo gerenciar clientes, cadastrar dívidas e registrar pagamentos por meio de uma interface gráfica para Windows.

## 🖥️ Versão principal — Windows Forms

**Tecnologias utilizadas:**
- C#
- .NET 8
- Windows Forms
- Entity Framework Core
- SQLite

### Funcionalidades

- Cadastro e gerenciamento de clientes.
- Validação de CPF.
- Consulta de clientes.
- Cadastro e consulta de dívidas.
- Registro de pagamentos parciais.
- Quitação total de dívidas.
- Cálculo do valor restante.
- Armazenamento persistente utilizando SQLite.

### Como executar

1. Instale o .NET 8 SDK.
2. Instale o Visual Studio 2022 com a carga de trabalho Desenvolvimento para desktop com .NET.
3. Abra `Vendinha.WinForms/Vendinha.WinForms.sln`.
4. Aguarde a restauração dos pacotes NuGet.
5. Defina `Vendinha.WinForms` como projeto de inicialização.
6. Pressione F5.

### Banco de dados

A aplicação utiliza SQLite.

O banco é criado automaticamente na primeira execução e fica armazenado em:

`%LOCALAPPDATA%\Vendinha\vendinha.db`

As principais tabelas são:

- `Clientes`
- `Dividas`

O projeto também possui o arquivo `banco.sql` com a estrutura do banco de dados.

---

## 🌐 Versão Web API

O repositório também contém uma versão anterior desenvolvida com ASP.NET Core Web API.

Ela utiliza:
- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger

O código dessa versão está disponível na pasta `Vendinha/`.

---

## 📁 Estrutura do repositório

```text
Vendinha/
│
├── Vendinha.WinForms/
│   ├── Data/
│   ├── Forms/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   ├── banco.sql
│   └── Vendinha.WinForms.sln
│
├── Vendinha/
│   ├── Contexts/
│   ├── Controllers/
│   ├── Migrations/
│   ├── Models/
│   └── Services/
│
├── .gitignore
└── README.md
```

## 👨‍💻 Objetivo acadêmico

O projeto foi desenvolvido para aplicar conhecimentos de programação em C#, Programação Orientada a Objetos, desenvolvimento de interfaces gráficas, manipulação de banco de dados e desenvolvimento de sistemas.

**Repositório:** https://github.com/ghostluiz15/Vendinha
