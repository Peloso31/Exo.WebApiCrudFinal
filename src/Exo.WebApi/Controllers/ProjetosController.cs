using Exo.WebApi.Dtos;
using Exo.WebApi.Models;
using Exo.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Exo.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjetosController : ControllerBase
{
    private readonly IProjetoRepository _repository;

    public ProjetosController(IProjetoRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Lists all projects.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Projeto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        return Ok(await _repository.ListarAsync(cancellationToken));
    }

    /// <summary>Gets one project by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Projeto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarPorId(int id, CancellationToken cancellationToken)
    {
        var projeto = await _repository.BuscarPorIdAsync(id, cancellationToken);
        return projeto is null ? NotFound() : Ok(projeto);
    }

    /// <summary>Creates a project.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Projeto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cadastrar(ProjetoRequest request, CancellationToken cancellationToken)
    {
        var projeto = await _repository.CadastrarAsync(ToEntity(request), cancellationToken);

        // 201 + Location header pointing to the new resource.
        return CreatedAtAction(nameof(BuscarPorId), new { id = projeto.Id }, projeto);
    }

    /// <summary>Updates a project.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, ProjetoRequest request, CancellationToken cancellationToken)
    {
        var atualizado = await _repository.AtualizarAsync(id, ToEntity(request), cancellationToken);
        return atualizado ? NoContent() : NotFound();
    }

    /// <summary>Deletes a project.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(int id, CancellationToken cancellationToken)
    {
        var deletado = await _repository.DeletarAsync(id, cancellationToken);
        return deletado ? NoContent() : NotFound();
    }

    private static Projeto ToEntity(ProjetoRequest request) => new()
    {
        NomeDoProjeto = request.NomeDoProjeto.Trim(),
        Area = request.Area.Trim(),
        Status = request.Status
    };
}
