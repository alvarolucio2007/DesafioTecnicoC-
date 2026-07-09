namespace DesafioTecnicoC.Controllers;

using DesafioTecnicoC.DTOs;
using DesafioTecnicoC.Models;
using DesafioTecnicoC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller responsável pelo gerenciamento de registros de pessoas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PessoaController : ControllerBase
{
    private readonly PessoaRepository _repository;

    public PessoaController(PessoaRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Cadastra uma nova pessoa no sistema.
    /// </summary>
    /// <param name="dto">Dados para criação da pessoa (Nome :string e Idade :int).</param>
    /// <returns>Retorna HTTP 200 em caso de sucesso.</returns>
    /// <response code="200">Pessoa cadastrada com sucesso.</response>
    /// <response code="400">Dados inválidos (idade fora do escopo 0-120 ou nome vazio).</response>
    /// <response code="500">Erro interno do servidor ao inserir o registro.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarPessoa([FromBody] CriarPessoaDto dto)
    {
        if (dto.Idade < 0)
        {
            return BadRequest("A idade não pode ser menor que zero.");
        }
        if (dto.Idade > 120)
        {
            return BadRequest("A idade não pode ser maior que 120.");
        }
        if (dto.Nome.Length < 1)
        {
            return BadRequest("O nome precisa ter no mínimo um caractere");
        }

        var pessoa = new PessoaModel { Nome = dto.Nome, Idade = dto.Idade };

        try
        {
            await _repository.InserirAsync(pessoa);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao criar pessoa: {ex.Message}");
        }
    }

    /// <summary>
    /// Lista todas as pessoas cadastradas no sistema.
    /// </summary>
    /// <returns>Uma coleção de DTOs contendo dados de pessoas.</returns>
    /// <response code="200">Retorna a lista de DTOs gerada com sucesso.</response>
    /// <response code="500">Erro interno do servidor ao consultar o banco de dados.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PessoaRespostaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PessoaRespostaDto>>> ListarTodasPessoas()
    {
        try
        {
            var pessoas = await _repository.ListarTodasPessoaAsync();
            var respostaDto = pessoas.Select(p => new PessoaRespostaDto(p.Id, p.Nome, p.Idade));
            return Ok(respostaDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao listar pessoas: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém os detalhes de uma pessoa específica através de seu ID.
    /// </summary>
    /// <param name="id">Identificador único da pessoa.</param>
    /// <returns>Os dados da pessoa encontrada.</returns>
    /// <response code="200">Retorna os dados da pessoa solicitada.</response>
    /// <response code="400">ID informado é inválido (menor que 1).</response>
    /// <response code="404">Nenhuma pessoa foi encontrada com o ID fornecido.</response>
    /// <response code="500">Erro interno do servidor ao processar a consulta.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PessoaRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PessoaRespostaDto>> BuscarPorId(int id)
    {
        if (id < 1) // Corrigido para < 1
            return BadRequest("Id precisa ser maior ou igual a 1.");

        try
        {
            var pessoa = await _repository.BuscarPorIdAsync(id);
            if (pessoa == null)
                return NotFound("Pessoa não encontrada.");

            var resposta = new PessoaRespostaDto(pessoa.Id, pessoa.Nome, pessoa.Idade);
            return Ok(resposta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao buscar pessoa: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza os dados de uma pessoa existente.
    /// </summary>
    /// <param name="id">Identificador único da pessoa a ser alterada.</param>
    /// <param name="dto">Novos dados para atualização (Nome e Idade).</param>
    /// <returns>Retorna HTTP 200 em caso de sucesso.</returns>
    /// <response code="200">Pessoa atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos ou ID inconsistente (menor que 1).</response>
    /// <response code="404">Nenhum registro foi encontrado com o ID informado.</response>
    /// <response code="500">Erro interno do servidor ao salvar as alterações.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AtualizarPessoa(int id, [FromBody] AtualizarPessoaDto dto)
    {
        if (id < 1) // Corrigido para < 1
            return BadRequest("Id precisa ser maior ou igual a 1.");
        if (dto.Idade < 0)
            return BadRequest("Idade não pode ser negativa.");
        if (dto.Idade > 120)
            return BadRequest("Idade não pode ser maior que 120.");

        try
        {
            var pessoaAtualizada = new Pessoa { Nome = dto.Nome, Idade = dto.Idade };
            int linhasAfetadas = await _repository.AtualizarPessoaAsync(id, pessoaAtualizada);
            if (linhasAfetadas == 0)
                return NotFound("Pessoa não encontrada.");

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao atualizar pessoa: {ex.Message}");
        }
    }

    /// <summary>
    /// Remove permanentemente uma pessoa do sistema.
    /// </summary>
    /// <param name="id">Identificador único da pessoa a ser excluída.</param>
    /// <returns>Retorna HTTP 200 em caso de sucesso.</returns>
    /// <response code="200">Pessoa deletada com sucesso.</response>
    /// <response code="400">ID informado é inválido (menor que 1).</response>
    /// <response code="404">Nenhum registro foi encontrado com o ID informado.</response>
    /// <response code="500">Erro interno do servidor ao processar a exclusão.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeletarPessoa(int id)
    {
        if (id < 1)
            return BadRequest("Id precisa ser maior ou igual a 1.");

        try
        {
            int linhasAfetadas = await _repository.DeletarPessoaAsync(id);
            if (linhasAfetadas == 0)
                return NotFound("Pessoa não encontrada.");

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao deletar pessoa: {ex.Message}");
        }
    }
}
