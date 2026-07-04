-- Ativa o suporte a Chaves Estrangeiras no SQLite (Obrigatório rodar sempre)
PRAGMA foreign_keys = ON;

-- 1. Tabela de Pessoas
CREATE TABLE IF NOT EXISTS pessoas (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nome TEXT NOT NULL UNIQUE,
    idade INTEGER NOT NULL
);

-- 2. Tabela de Transações
CREATE TABLE IF NOT EXISTS transacoes (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    descricao TEXT NOT NULL,
    valor REAL NOT NULL,
    tipo TEXT NOT NULL CHECK(tipo IN ('CREDITO', 'DEBITO')),
    id_pessoa INTEGER NOT NULL,
    FOREIGN KEY (id_pessoa) REFERENCES pessoas(id) ON DELETE CASCADE
);

