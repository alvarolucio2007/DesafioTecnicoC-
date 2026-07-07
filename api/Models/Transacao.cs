namespace DesafioTecnicoC.Models;

public record Transacao
{
    public int Id {get;init;}
    public string Descricao { get; init; } = string.Empty;
    public decimal Valor { get; init; }
    public TipoTransacao Tipo { get; init; }
    public int Id_Pessoa { get; init; }
}
public enum TipoTransacao
{
    CREDITO,
    DEBITO
}
