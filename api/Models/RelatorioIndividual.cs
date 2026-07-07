namespace DesafioTecnicoC.Models;

public record RelatorioIndividual
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public decimal TotalReceitas { get; init; }
    public decimal TotalDespesas { get; init; }
    public decimal Saldo { get; init; }
}
