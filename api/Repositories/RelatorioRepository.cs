using System.Data;
using Dapper;
using DesafioTecnicoC.Models;
using Microsoft.Data.Sqlite;

namespace DesafioTecnicoC.Repositories;

public class RelatorioRepository
{
    private readonly string _connectionString;

    public RelatorioRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Relatório de todas as receitas e despesas, e saldo de cada pessoa.
    /// </summary>
    public async Task<IEnumerable<RelatorioIndividual>> ListarRelatoriosPessoalAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql = """
            SELECT 
                p.id,
                p.nome,
                COALESCE(SUM(CASE WHEN t.tipo = 'CREDITO' THEN t.valor END), 0) AS TotalReceitas,
                COALESCE(SUM(CASE WHEN t.tipo = 'DEBITO' THEN t.valor END), 0) AS TotalDespesas,
                COALESCE(SUM(CASE WHEN t.tipo = 'CREDITO' THEN t.valor END), 0) - 
                COALESCE(SUM(CASE WHEN t.tipo = 'DEBITO' THEN t.valor END), 0) AS Saldo
            FROM pessoas p
            LEFT JOIN transacoes t ON p.id = t.id_pessoa
            GROUP BY p.id, p.nome;
            """;
        return await connection.QueryAsync<RelatorioIndividual>(sql);
    }

    /// <summary>
    /// Relatório geral de todos os totais, de receita, despesa e saldo.
    /// </summary>
    public async Task<RelatorioGeral?> ListarRelatorioGeralAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql = """
            SELECT 
                COALESCE(SUM(CASE WHEN tipo = 'CREDITO' THEN valor END), 0) AS TotalGeralReceitas,
                COALESCE(SUM(CASE WHEN tipo = 'DEBITO' THEN valor END), 0) AS TotalGeralDespesas,
                COALESCE(SUM(CASE WHEN tipo = 'CREDITO' THEN valor END), 0) - 
                COALESCE(SUM(CASE WHEN tipo = 'DEBITO' THEN valor END), 0) AS SaldoLiquidoGeral
            FROM transacoes;
            """;
        return await connection.QueryFirstOrDefaultAsync<RelatorioGeral>(sql);
    }
}
