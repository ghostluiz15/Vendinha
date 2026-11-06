
-- ============================================
-- VENDINHA - SCRIPT DE CRIACAO DO BANCO
-- Banco de dados: SQLite
-- ============================================

PRAGMA foreign_keys = ON;

-- ============================================
-- TABELA DE CLIENTES
-- ============================================

CREATE TABLE IF NOT EXISTS Clientes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    NomeCompleto TEXT NOT NULL,
    CPF TEXT NOT NULL,
    DataNascimento TEXT NOT NULL,
    Email TEXT NULL
);

-- CPF UNICO: NAO PERMITE CADASTROS DUPLICADOS

CREATE UNIQUE INDEX IF NOT EXISTS IX_Clientes_CPF
ON Clientes (CPF);


-- ============================================
-- TABELA DE DIVIDAS
-- ============================================

CREATE TABLE IF NOT EXISTS Dividas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Valor DECIMAL(10,2) NOT NULL,
    ValorPago DECIMAL(10,2) NOT NULL DEFAULT 0,
    Paga INTEGER NOT NULL,
    DataCriacao TEXT NOT NULL,
    DataPagamento TEXT NULL,
    ClienteId INTEGER NOT NULL,

    CONSTRAINT FK_Dividas_Clientes_ClienteId
    FOREIGN KEY (ClienteId)
    REFERENCES Clientes(Id)
    ON DELETE CASCADE
);


-- ============================================
-- INDICE DE CLIENTE NAS DIVIDAS
-- ============================================

CREATE INDEX IF NOT EXISTS IX_Dividas_ClienteId
ON Dividas (ClienteId);


-- ============================================
-- FIM DO SCRIPT SQL
-- ============================================
