namespace DesafioTecnicoC.Controllers;

using DesafioTecnicoC.DTOs;
using DesafioTecnicoC.Models;
using DesafioTecnicoC.Repositories;
using Microsoft.AspNetCore.Mvc;

///<summary>
///Controller responsável pela geração e consolidação de relatórios financeiros.
///</summary>
[ApiController]
[Route("api/[controller]")]
public class RelatorioController : ControllerBase
{
    private readonly RelatorioRepository _repository;

    public RelatorioController(RelatorioRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtém o relatório financeiro consolidado de cada pessoa cadastrada. Método: GET
    /// </summary>
    /// <returns>Uma lista de DTOs contendo totais de receita, despesa e saldo individual.</returns>
    /// <response code="200">Retorna a lista de relatórios gerada com sucesso.</response>
    /// <response code="500">Erro interno do servidor ao processar a consulta SQL.</response>
    [HttpGet("pessoal")]
    [ProducesResponseType(
        typeof(IEnumerable<RelatorioIndividualResponseDto>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<
        ActionResult<IEnumerable<RelatorioIndividualResponseDto>>
    > ListarRelatoriosIndividual()
    {
        try
        {
            var relatorios = await _repository.ListarRelatoriosPessoalAsync();
            var respostaDto = relatorios.Select(p => new RelatorioIndividualResponseDto(
                p.Id,
                p.Nome,
                p.TotalReceitas,
                p.TotalDespesas,
                p.Saldo
            ));
            return Ok(respostaDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao listar o relatório individual: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém o relatório financeiro resumido de todas as pessoas. Método: GET
    /// </summary>
    /// <returns>Uma lista de DTOs contendo totais de receita, despesa e saldo geral.</returns>
    /// <response code="200">Retorna a lista de relatórios gerada com sucesso.</response>
    /// <response code="404">Caso não existam dados para gerar o relatório geral.</response>
    /// <response code="500">Erro interno do servidor ao processar a consulta SQL.</response>
    [HttpGet("geral")]
    [ProducesResponseType(typeof(RelatorioGeralResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RelatorioGeralResponseDto>> ListarRelatorioGeral()
    {
        try
        {
            var relatorio = await _repository.ListarRelatorioGeralAsync();
            if (relatorio is null)
                return NotFound("Relatório geral não encontrado.");
            var respostaDto = new RelatorioGeralResponseDto(
                relatorio.TotalGeralReceitas,
                relatorio.TotalGeralDespesas,
                relatorio.SaldoLiquidoGeral
            );
            return Ok(respostaDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao listar o relatório geral: {ex.Message}");
        }
    }
}
