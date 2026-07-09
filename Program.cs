using System.Text.Json.Serialization;
using Dapper;
using DesafioTecnicoC.Repositories;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

string connectionString = "Data Source=database.db";

builder.Services.AddControllers();

builder.Services.AddScoped<PessoaRepository>(provider => new PessoaRepository(connectionString));
builder.Services.AddScoped<TransacaoRepository>(provider => new TransacaoRepository(
    connectionString
));
builder.Services.AddScoped<RelatorioRepository>(provider => new RelatorioRepository(
    connectionString
));

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowReact",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173", "http://localhost:3000") // Portas do seu Front
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();

    var migrationSql = """
        PRAGMA foreign_keys = ON;

        CREATE TABLE IF NOT EXISTS pessoas (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            nome TEXT NOT NULL UNIQUE,
            idade INTEGER NOT NULL
        );

        CREATE TABLE IF NOT EXISTS transacoes (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            descricao TEXT NOT NULL,
            valor DECIMAL(10,2) NOT NULL,
            tipo TEXT NOT NULL CHECK(tipo IN ('CREDITO', 'DEBITO')),
            id_pessoa INTEGER NOT NULL,
            FOREIGN KEY (id_pessoa) REFERENCES pessoas(id) ON DELETE CASCADE
        );
        """;

    await Dapper.SqlMapper.ExecuteAsync(connection, migrationSql);
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowReact");

app.UseAuthorization();

app.MapControllers();

app.Run();
