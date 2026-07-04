using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;

using DesafioTecnicoC.Models;

namespace DesafioTecnicoC.Repositories;
public class TotalRepository{

    private readonly string _connectionString;
    public TotalRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    /// <summary>
    /// Soma total dos creditos e debitos de cada pessoa.
    /// </summary>
    public async Task<IEnumerable<RelatorioDespesas>> ListarTodosRelatorioAsync(){
    
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
        return await connection.QueryAsync<RelatorioDespesas>(sql);
    }
}
