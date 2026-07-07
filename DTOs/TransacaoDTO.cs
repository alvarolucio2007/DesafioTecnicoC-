using DesafioTecnicoC.Models;
namespace DesafioTecnicoC.DTOs;
public record CriarTransacaoDTO(
  string Descricao,
  decimal Valor,
  TipoTransacao Tipo,
  int Id_Pessoa
);
public record AtualizarTransacaoDTO(
    int Id_Transacao,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    int Id_Pessoa
);
public record TransacaoRespostaDTO(
    int Id,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    int Id_Pessoa
);
