using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;

using DesafioTecnicoC.Models;

namespace DesafioTecnicoC.Repositories;

public class PessoaRepository
{
    private readonly string _connectionString;
    public PessoaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    ///<summary>
    ///Insere uma nova pessoa no banco de dados.
    ///</summary>
    ///<param name="pessoa"> Objeto contendo os dados da pessoa (nome e idade).</param>
    public async Task InserirAsync(Pessoa pessoa)
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql = "INSERT INTO pessoas (nome, idade) VALUES (@Nome, @Idade);";
        await connection.ExecuteAsync(sql, pessoa);
    }
    ///<summary>
    ///Lista todas as pessoas cadastradas no banco de dados.
    ///</summary>
    public async Task<IEnumerable<Pessoa>> ListarTodasPessoaAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql = "SELECT id,nome,idade FROM pessoas;";
        return await connection.QueryAsync<Pessoa>(sql);
    }
    ///<summary>
    ///Busca uma pessoa em específico através do ID
    ///</summary>
    ///<param name="id">ID da pessoa buscada.</param>
    public async Task<Pessoa?> BuscarPorIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql = "SELECT id,nome,idade FROM pessoas WHERE id = @Id;";
        return await connection.QueryFirstOrDefaultAsync<Pessoa>(sql, new { Id = id });
    }
    ///<summary>
    ///Atualiza uma pessoa através do ID dela.
    ///</summary>
    ///<param name="id">ID da pessoa editada.</param>
    ///<param name="pessoa">Novos dados da pessoa editada.</param>
    public async Task<int> AtualizarPessoaAsync(int id, Pessoa pessoa)
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql = "UPDATE pessoas SET nome = @Nome, Idade = @Idade WHERE id=@Id";
        return await connection.ExecuteAsync(sql, new { Id = id, Nome = pessoa.Nome, Idade = pessoa.Idade });
    }
    ///<summary>
    ///Remove uma pessoa através do ID dela.
    ///</summary>
    ///<param name="id">ID da pessoa a ser removida. </param>
    public async Task<int> DeletarPessoaAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        string sql = "DELETE FROM pessoas WHERE id=@Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
