namespace DesafioTecnicoC.Models;

public record RelatorioGeral
{
    public decimal TotalGeralReceitas { get; init; }
    public decimal TotalGeralDespesas { get; init; }
    public decimal SaldoLiquidoGeral { get; init; }
}
