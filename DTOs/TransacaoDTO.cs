using DesafioTecnicoC.Models;
namespace DesafioTecnicoC.DTOs;
public record InserirTransacaoDTO(
  string Descricao,
  double Valor,
  TipoTransacao Tipo,
  int Id_Pessoa
);
public record AtualizarTransacaoDTO(
    int Id_Transacao,
    string Descricao,
    double Valor,
    TipoTransacao Tipo,
    int Id_Pessoa
);
public record TransacaoRespostaDTO(
    int Id,
    string Descricao,
    double Valor,
    TipoTransacao Tipo,
    int Id_Pessoa
);
