using System.Text.Json.Serialization;
using DesafioTecnicoC.Repositories;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura a Connection String do SQLite
string connectionString = "Data Source=database.db";

// 2. ADICIONE ISSO: Ativa o suporte para Controllers na API
builder.Services.AddControllers();

// 3. ADICIONE ISSO: Registra o seu Repositório informando a Connection String
builder.Services.AddScoped<PessoaRepository>(provider => new PessoaRepository(connectionString));
builder.Services.AddScoped<TransacaoRepository>(provider => new TransacaoRepository(
    connectionString
));
builder.Services.AddScoped<RelatorioRepository>(provider => new RelatorioRepository(
    connectionString
));

// 4. ADICIONE ISSO: Configura o CORS para o seu React (Porta padrão 5173 do Vite ou 3000)
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Dica extra: Se quiser habilitar a interface visual do Swagger para testar:
    // app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseHttpsRedirection();

// 5. ADICIONE ISSO: Ativa a política de CORS que criamos lá em cima
app.UseCors("AllowReact");

app.UseAuthorization();

// 6. ADICIONE ISSO: Mapeia os endpoints dos seus Controllers automaticamente
app.MapControllers();

app.Run();
