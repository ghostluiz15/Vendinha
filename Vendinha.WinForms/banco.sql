-- Vendinha Windows Forms - SQLite
-- O próprio aplicativo cria este schema automaticamente na primeira execução.

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Clientes (
    Id INTEGER NOT NULL CONSTRAINT PK_Clientes PRIMARY KEY AUTOINCREMENT,
    NomeCompleto TEXT NOT NULL,
    CPF TEXT NOT NULL,
    DataNascimento TEXT NOT NULL,
    Email TEXT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_Clientes_CPF ON Clientes (CPF);

CREATE TABLE IF NOT EXISTS Dividas (
    Id INTEGER NOT NULL CONSTRAINT PK_Dividas PRIMARY KEY AUTOINCREMENT,
    Valor decimal(10,2) NOT NULL,
    ValorPago decimal(10,2) NOT NULL DEFAULT 0,
    Paga INTEGER NOT NULL,
    DataCriacao TEXT NOT NULL,
    DataPagamento TEXT NULL,
    ClienteId INTEGER NOT NULL,
    CONSTRAINT FK_Dividas_Clientes_ClienteId
        FOREIGN KEY (ClienteId) REFERENCES Clientes (Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_Dividas_ClienteId ON Dividas (ClienteId);
