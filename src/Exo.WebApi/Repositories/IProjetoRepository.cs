using Exo.WebApi.Models;

namespace Exo.WebApi.Repositories;

/// <summary>
/// Data access contract for projects. The controller depends on this
/// interface, not on EF Core, which makes it easier to test and replace.
/// </summary>
public interface IProjetoRepository
{
    Task<IReadOnlyList<Projeto>> ListarAsync(CancellationToken cancellationToken = default);

    Task<Projeto?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Projeto> CadastrarAsync(Projeto projeto, CancellationToken cancellationToken = default);

    /// <returns>False when no project has the given id.</returns>
    Task<bool> AtualizarAsync(int id, Projeto dados, CancellationToken cancellationToken = default);

    /// <returns>False when no project has the given id.</returns>
    Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default);
}
