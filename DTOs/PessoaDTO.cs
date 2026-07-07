namespace DesafioTecnicoC.DTOs;
public record CriarPessoaDto(
  string Nome,
  int Idade
);
public record PessoaRespostaDto(
    int Id,
    string Nome,
    int Idade
);
public record AtualizarPessoaDto(
    int Id,
    string Nome,
    int Idade
);
