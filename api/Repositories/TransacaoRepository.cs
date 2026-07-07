using System.Data;
using Dapper;
using DesafioTecnicoC.Models;
using Microsoft.Data.Sqlite;

namespace DesafioTecnicoC.Repositories;

public class TransacaoRepository
{
    private readonly string _connectionString;

    public TransacaoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Insere uma nova transação associada a uma pessoa via FK
    /// </summary>
    /// <param name="transacao">Dados da transação</param>
    public async Task InserirTransacaoAsync(Transacao transacao)
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql =
            "INSERT INTO transacoes (descricao, valor,tipo,id_pessoa) VALUES (@Descricao, @Valor,@Tipo,@Id_Pessoa);";
        await connection.ExecuteAsync(
            sql,
            new
            {
                transacao.Descricao,
                transacao.Valor,
                Tipo = transacao.Tipo.ToString().ToUpper(),
                transacao.Id_Pessoa,
            }
        );
    }

    ///<summary>
    ///Listar todas as transações no banco de dados.
    ///</summary>
    public async Task<IEnumerable<Transacao>> ListarTodosTransacaoAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql = "SELECT id,descricao,valor,tipo,id_pessoa FROM transacoes;";
        return await connection.QueryAsync<Transacao>(sql);
    }

    ///<summary>
    ///Lista todas as transações associadas a uma pessoa específica através do ID dela.
    ///</summary>
    ///<param name="id">ID da pessoa para a pesquisa.</param>
    public async Task<IEnumerable<Transacao>> ListarTransacaoPessoaAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql =
            "SELECT id,descricao,valor,tipo,id_pessoa FROM transacoes WHERE id_pessoa = @Id_Pessoa;";
        return await connection.QueryAsync<Transacao>(sql, new { Id_Pessoa = id });
    }

    ///<summary>
    ///Atualiza uma transação baseada no ID dela.
    ///</summary>
    ///<param name="id">ID da transacao para a mudança.</param>
    ///<param name="transacao">Novos dados da transação</param>
    public async Task<int> AtualizarTransacaoAsync(int id, Transacao transacao)
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql =
            "UPDATE transacoes SET descricao = @Descricao, valor = @Valor,tipo = @Tipo, id_pessoa=@Id_Pessoa WHERE id=@Id";
        return await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                Descricao = transacao.Descricao,
                Valor = transacao.Valor,
                Tipo = transacao.Tipo,
                Id_Pessoa = transacao.Id_Pessoa,
            }
        );
    }

    ///<summary>
    ///Remove uma transação através do ID dela.
    ///</summary>
    ///<param name="id">ID da transação a ser removida. </param>
    public async Task<int> DeletarTransacaoAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql = "DELETE FROM transacoes WHERE id=@Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
