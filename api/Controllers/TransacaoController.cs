namespace DesafioTecnicoC.Controllers;

using DesafioTecnicoC.DTOs;
using DesafioTecnicoC.Models;
using DesafioTecnicoC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller responsável pelo ciclo de vida e gerenciamento de transações financeiras (Débitos e Créditos).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacaoController : ControllerBase
{
    private readonly TransacaoRepository _repository;
    private readonly PessoaRepository _pessoaRepository;

    public TransacaoController(TransacaoRepository repository, PessoaRepository pessoaRepository)
    {
        _repository = repository;
        _pessoaRepository = pessoaRepository;
    }

    /// <summary>
    /// Regista uma nova transação financeira no sistema, validando regras de negócio de saldo e idade.
    /// </summary>
    /// <param name="dto">Dados necessários para a criação da transação.</param>
    /// <returns>Retorna HTTP 200 em caso de sucesso.</returns>
    /// <response code="200">Transação registada com sucesso.</response>
    /// <response code="400">Dados inválidos (valor negativo, ID inválido ou menor de idade a tentar registar crédito).</response>
    /// <response code="404">A pessoa vinculada à transação não foi encontrada no sistema.</response>
    /// <response code="500">Erro interno do servidor ao processar a inserção no banco de dados.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarTransacao([FromBody] CriarTransacaoDTO dto)
    {
        if (dto.Valor < 0)
        {
            return BadRequest("O valor não pode ser menor que 0.");
        }
        if (dto.Id_Pessoa < 1)
        {
            return BadRequest("O id da pessoa atrelada não pode ser menor que 1.");
        }

        var p = await _pessoaRepository.BuscarPorIdAsync(dto.Id_Pessoa);
        if (p == null)
        {
            return NotFound("Pessoa não encontrada.");
        }

        // Regra de Negócio: Impedir receitas para menores de 18 anos
        if (p.Idade < 18 && dto.Tipo.ToString().ToUpper() == "CREDITO")
        {
            return BadRequest("Menores de 18 anos só podem cadastrar despesas (DÉBITOS).");
        }

        var transacao = new TransacaoModel
        {
            Descricao = dto.Descricao,
            Valor = dto.Valor,
            Tipo = dto.Tipo,
            Id_Pessoa = dto.Id_Pessoa,
        };

        try
        {
            await _repository.InserirTransacaoAsync(transacao);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao criar transação: {ex.Message}");
        }
    }

    /// <summary>
    /// Lista todas as transações financeiras registadas no sistema.
    /// </summary>
    /// <returns>Uma coleção de DTOs contendo os detalhes das transações.</returns>
    /// <response code="200">Retorna a lista de transações geradas com sucesso.</response>
    /// <response code="500">Erro interno do servidor ao ler a tabela do banco de dados.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TransacaoRespostaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TransacaoRespostaDTO>>> ListarTodasTransacao()
    {
        try
        {
            var transacoes = await _repository.ListarTodosTransacaoAsync();
            var respostaDTO = transacoes.Select(p => new TransacaoRespostaDTO(
                p.Id,
                p.Descricao,
                p.Valor,
                p.Tipo,
                p.Id_Pessoa
            ));
            return Ok(respostaDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao listar transações: {ex.Message}");
        }
    }

    /// <summary>
    /// Lista todas as transações financeiras que pertencem a uma pessoa específica.
    /// </summary>
    /// <param name="id">O ID da pessoa cuja lista de transações será consultada.</param>
    /// <returns>Uma coleção de DTOs filtrada pelo ID informado.</returns>
    /// <response code="200">Retorna a lista de transações vinculadas à pessoa informada.</response>
    /// <response code="400">ID informado é inválido (menor que 1).</response>
    /// <response code="500">Erro interno do servidor ao processar o filtro no banco de dados.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IEnumerable<TransacaoRespostaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TransacaoRespostaDTO>>> BuscarPorID(int id)
    {
        if (id < 1)
            return BadRequest("Id precisa ser maior ou igual a 1.");

        try
        {
            var transacoes = await _repository.ListarTransacaoPessoaAsync(id);
            var respostaDTO = transacoes.Select(p => new TransacaoRespostaDTO(
                p.Id,
                p.Descricao,
                p.Valor,
                p.Tipo,
                p.Id_Pessoa
            ));
            return Ok(respostaDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao buscar transações da pessoa: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza os dados de uma transação existente com base em seu identificador único.
    /// </summary>
    /// <param name="id">Identificador único da transação a ser alterada.</param>
    /// <param name="dto">Novos dados para substituição da transação.</param>
    /// <returns>Retorna HTTP 200 em caso de sucesso.</returns>
    /// <response code="200">Transação atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos ou inconsistência de ID (menor que 1).</response>
    /// <response code="404">A transação ou a nova pessoa informada não foram localizadas.</response>
    /// <response code="500">Erro interno do servidor ao persistir a alteração no banco de dados.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AtualizarTransacao(int id, [FromBody] AtualizarTransacaoDTO dto)
    {
        if (id < 1) // Corrigido de < 0 para < 1
            return BadRequest("Id precisa ser maior ou igual a 1.");
        if (dto.Id_Pessoa < 1)
            return BadRequest(
                "Id da transação e Id da pessoa precisam ambos serem maiores ou iguais a 1."
            );

        var p = await _pessoaRepository.BuscarPorIdAsync(dto.Id_Pessoa);
        if (p is null)
        {
            return NotFound("Pessoa não encontrada.");
        }

        // Regra de Negócio: Impedir alteração para receita se menor de idade
        if (p.Idade < 18 && dto.Tipo.ToString().ToUpper() == "CREDITO")
        {
            return BadRequest("Menores de 18 anos só podem cadastrar despesas (DÉBITOS).");
        }

        try
        {
            var transacaoAtualizada = new TransacaoModel
            {
                Descricao = dto.Descricao,
                Valor = dto.Valor,
                Tipo = dto.Tipo,
                Id_Pessoa = dto.Id_Pessoa,
            };

            int linhasAfetadas = await _repository.AtualizarTransacaoAsync(id, transacaoAtualizada);
            if (linhasAfetadas == 0)
                return NotFound("Transação não encontrada.");

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao atualizar transação: {ex.Message}");
        }
    }

    /// <summary>
    /// Remove permanentemente uma transação do sistema a partir de seu ID.
    /// </summary>
    /// <param name="id">Identificador único da transação a ser excluída.</param>
    /// <returns>Retorna HTTP 200 em caso de sucesso.</returns>
    /// <response code="200">Transação removida com sucesso.</response>
    /// <response code="400">ID informado inválido (menor que 1).</response>
    /// <response code="404">O usuário com o ID informado não existe.</response>
    /// <response code="500">Erro interno do servidor ao tentar processar a remoção do registro.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeletarTransacao(int id)
    {
        if (id < 1)
            return BadRequest("Id precisa ser maior ou igual a 1.");

        try
        {
            int linhasAfetadas = await _repository.DeletarTransacaoAsync(id);
            if (linhasAfetadas == 0)
                return NotFound("Transação não encontrada.");

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao deletar transação: {ex.Message}");
        }
    }
}
