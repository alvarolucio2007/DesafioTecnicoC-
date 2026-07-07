namespace DesafioTecnicoC.DTOs;
public record RelatorioIndividualResponseDto(
  int Id,
  string Nome,
  decimal TotalReceitas,
  decimal TotalDespesas,
  decimal Saldo
);
public record RelatorioGeralResponseDto(
  decimal TotalReceitas,
  decimal TotalDespesas,
  decimal SaldoGeral
);
