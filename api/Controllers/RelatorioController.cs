namespace DesafioTecnicoC.Controllers;

using DesafioTecnicoC.DTOs;
using DesafioTecnicoC.Models;
using DesafioTecnicoC.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RelatorioController : ControllerBase
{
    private readonly RelatorioRepository _repository;

    public RelatorioController(RelatorioRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("especifico")]
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

    [HttpGet("geral")]
    public async Task<ActionResult<RelatorioGeralResponseDto>> ListarRelatorioGeral()
    {
        try
        {
            var relatorio = await _repository.ListarRelatorioGeralAsync();
            if (relatorio == null)
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
            return StatusCode(500, $"Erro interno ao listar o relatório individual: {ex.Message}");
        }
    }
}
