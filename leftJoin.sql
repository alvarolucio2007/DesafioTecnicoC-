SELECT 
    p.id,
    p.nome,
    COALESCE(SUM(CASE WHEN t.tipo = 'CREDITO' THEN t.valor END), 0) AS total_receitas,
    COALESCE(SUM(CASE WHEN t.tipo = 'DEBITO' THEN t.valor END), 0) AS total_despesas,
    COALESCE(SUM(CASE WHEN t.tipo = 'CREDITO' THEN t.valor END), 0) - 
    COALESCE(SUM(CASE WHEN t.tipo = 'DEBITO' THEN t.valor END), 0) AS saldo
FROM pessoas p
LEFT JOIN transacoes t ON p.id = t.id_pessoa
GROUP BY p.id, p.nome;
