using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;

using DesafioTecnicoC.Models;

namespace DesafioTecnicoC.Repositories;
public class RelatorioRepository{

    private readonly string _connectionString;
    public RelatorioRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    /// <summary>
    /// Relatório de todas as receitas e despesas, e saldo de cada pessoa.
    /// </summary>
    public async Task<IEnumerable<RelatorioIndividual?>> ListarRelatoriosPessoalAsync(){
        using var connection = new SqliteConnection(_connectionString);
        string sql = """
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
          """;
        return await connection.QueryAsync<RelatorioIndividual>(sql);
    }
    
    /// <summary>
    /// Relatório geral de todos os totais, de receita, despesa e saldo.
    /// </summary>
    public async Task<RelatorioGeral?> ListarRelatorioGeralAsync(){
      using var connection = new SqliteConnection(_connectionString);
        string sql = """
          SELECT 
              COALESCE(SUM(CASE WHEN tipo = 'CREDITO' THEN valor END), 0) AS total_geral_receitas,
              COALESCE(SUM(CASE WHEN tipo = 'DEBITO' THEN valor END), 0) AS total_geral_despesas,
              COALESCE(SUM(CASE WHEN tipo = 'CREDITO' THEN valor END), 0) - 
              COALESCE(SUM(CASE WHEN tipo = 'DEBITO' THEN valor END), 0) AS saldo_liquido_geral
          FROM transacoes;
          """;
        return await connection.QueryFirstOrDefaultAsync<RelatorioGeral>(sql);
    }
}
