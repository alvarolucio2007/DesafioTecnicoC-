namespace DesafioTecnicoC.Models;

//Criação de record pessoa, para armazenar os dados de forma organizada.
public record Pessoa
{
    public string Nome { get; init; } = string.Empty;
    public int Idade { get; init; }
}
