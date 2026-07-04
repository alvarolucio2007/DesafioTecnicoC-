SELECT 
    COALESCE(SUM(CASE WHEN tipo = 'CREDITO' THEN valor END), 0) AS total_geral_receitas,
    COALESCE(SUM(CASE WHEN tipo = 'DEBITO' THEN valor END), 0) AS total_geral_despesas,
    COALESCE(SUM(CASE WHEN tipo = 'CREDITO' THEN valor END), 0) - 
    COALESCE(SUM(CASE WHEN tipo = 'DEBITO' THEN valor END), 0) AS saldo_liquido_geral
FROM transacoes;
