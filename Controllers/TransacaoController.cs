namespace DesafioTecnicoC.Controllers;
using Microsoft.AspNetCore.Mvc;
using DesafioTecnicoC.DTOs;
using DesafioTecnicoC.Models;
using DesafioTecnicoC.Repositories;
[ApiController]
[Route("api/[controller]")]
public class TransacaoController: ControllerBase
{
  private readonly TransacaoRepository _repository;

  public TransacaoController(TransacaoRepository repository)
  {
    _repository=repository;
  }
  [HttpPost]
  public async Task<IActionResult> CriarTransacao([FromBody] CriarTransacaoDTO dto)
  {
    if (dto.Valor<0)
    {
      return BadRequest("O valor não pode ser menor que 0.");
    }
    if (dto.Id_Pessoa<1)
    {
      return BadRequest("O id da pessoa atrelada não pode ser menor que 1.");
    }
    var transacao = new Transacao { 
      Descricao = dto.Descricao,
      Valor = dto.Valor,
      Tipo = dto.Tipo,
      Id_Pessoa = dto.Id_Pessoa
    };
    try{
      await _repository.InserirTransacaoAsync(transacao);
    } catch (Exception ex){
        return StatusCode(500,$"Erro interno ao criar usuário: {ex.Message}");
    }
    return Ok();
  }
  [HttpGet]
  public async Task<ActionResult<IEnumerable<TransacaoRespostaDTO>>> ListarTodasTransacao(){
    try{
      var transacoes = await _repository.ListarTodosTransacaoAsync();
      var respostaDTO=transacoes.Select(p=>new TransacaoRespostaDTO(p.Id,p.Descricao,p.Valor,p.Tipo,p.Id_Pessoa));
      return Ok(respostaDTO);
    } catch (Exception ex){
      return StatusCode(500,$"Erro interno ao listar pessoas: {ex.Message}");
    }
  }
  [HttpGet("{id}")]
  public async Task<ActionResult<IEnumerable<TransacaoRespostaDTO>>> BuscarPorID(int id){
    if (id<0) return BadRequest("Id precisa ser maior ou igual a 1.");
    try{
      var transacoes = await _repository.ListarTransacaoPessoaAsync(id);
      var respostaDTO=transacoes.Select(p=>new TransacaoRespostaDTO(p.Id,p.Descricao,p.Valor,p.Tipo,p.Id_Pessoa));
      return Ok(respostaDTO);
    } catch (Exception ex){
      return StatusCode(500,$"Erro interno ao listar pessoas: {ex.Message}");
    }
  }
  [HttpPut("{id}")]
  public async Task<ActionResult> AtualizarTransacao(int id, [FromBody] AtualizarTransacaoDTO dto){
      if (id<0) return BadRequest("Id precisa ser maior ou igual a 1.");
      if (dto.Id_Transacao <1 || dto.Id_Pessoa <1) return BadRequest("Id da transação e Id da pessoa precisam ambos serem maiores ou iguais a 1.");
  }
}
