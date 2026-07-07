namespace DesafioTecnicoC.Controllers;

using Microsoft.AspNetCore.Mvc;
using DesafioTecnicoC.DTOs;
using DesafioTecnicoC.Models;
using DesafioTecnicoC.Repositories;

[ApiController]
[Route("api/[controller]")]
public class PessoaController : ControllerBase
{
    private readonly PessoaRepository _repository;

    // O construtor recebe o seu repositório automaticamente via Injeção de Dependência
    public PessoaController(PessoaRepository repository)
    {
        _repository = repository;
    }
    [HttpPost]
    public async Task<IActionResult> CriarPessoa([FromBody] CriarPessoaDto dto)
    {
        if (dto.Idade < 0) 
        {
          return BadRequest("A idade não pode ser menor que zero.");
        }
        if (dto.Nome.Length<1)
        {
          return BadRequest("O nome precisa ter no mínimo um caractere");
        }
        var pessoa = new Pessoa 
        { 
            Nome = dto.Nome, 
            Idade = dto.Idade 
        };

        try{
        await _repository.InserirAsync(pessoa);
        }
        catch (Exception ex)
        {
        return StatusCode(500,$"Erro interno ao criar usuário: {ex.Message}");
        }
        return Ok();
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PessoaRespostaDto>>> ListarTodasPessoas(){
      try{
        var pessoas = await _repository.ListarTodasPessoaAsync();
        var respostaDto=pessoas.Select(p=>new PessoaRespostaDto(p.Id,p.Nome,p.Idade));
        return Ok(respostaDto);
      }
      catch (Exception ex){
        return StatusCode(500,$"Erro interno ao listar pessoas: {ex.Message}");
      }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<PessoaRespostaDto>> BuscarPorId(int id){
      if (id<0) return BadRequest("Id precisa ser maior ou igual a 1.");
      try{
      var pessoa = await _repository.BuscarPorIdAsync(id);
      if (pessoa==null) return NotFound("Pessoa não encontrada.");
      var resposta=new PessoaRespostaDto(pessoa.Id,pessoa.Nome,pessoa.Idade);
      return Ok(resposta);
      }
      catch (Exception ex)
      {
        return StatusCode(500,$"Erro interno ao atualizar banco: {ex.Message}");
      }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarPessoa(int id, [FromBody] AtualizarPessoaDto dto){
      if (id<0) return BadRequest("Id precisa ser maior ou igual a 1.");
      if (dto.Idade<0) return BadRequest("Idade não pode ser negativa.");
      try{

        var pessoaAtualizada = new Pessoa {Nome=dto.Nome,Idade=dto.Idade};
        int linhasAfetadas = await _repository.AtualizarPessoaAsync(id,pessoaAtualizada);
        if (linhasAfetadas==0) return NotFound("Pessoa não encontrada.");

        return Ok();
      }catch (Exception ex){

        return StatusCode(500,$"Erro interno ao atualizar no banco de dados: {ex.Message}");

      }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletarPessoa(int id)
    {
      if (id < 1) return BadRequest("Id precisa ser maior ou igual a 1.");
      try{
        int linhasAfetadas = await _repository.DeletarPessoaAsync(id);
        if (linhasAfetadas==0) return NotFound("Pessoa não encontrada.");
        return Ok();
      }
      catch (Exception ex){
        return StatusCode(500,$"Erro interno ao deletar usuário do banco de dados: {ex.Message}");
      }

    }
}
