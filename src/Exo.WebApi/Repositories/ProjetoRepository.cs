using Exo.WebApi.Contexts;
using Exo.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Exo.WebApi.Repositories;

public class ProjetoRepository : IProjetoRepository
{
    private readonly ExoContext _context;

    public ProjetoRepository(ExoContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Projeto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        // AsNoTracking: read-only query, so EF does not need to track changes.
        return await _context.Projetos
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Projeto?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Projetos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Projeto> CadastrarAsync(Projeto projeto, CancellationToken cancellationToken = default)
    {
        _context.Projetos.Add(projeto);
        await _context.SaveChangesAsync(cancellationToken);
        return projeto;
    }

    public async Task<bool> AtualizarAsync(int id, Projeto dados, CancellationToken cancellationToken = default)
    {
        var projeto = await _context.Projetos.FindAsync(new object[] { id }, cancellationToken);
        if (projeto is null)
        {
            return false;
        }

        projeto.NomeDoProjeto = dados.NomeDoProjeto;
        projeto.Area = dados.Area;
        projeto.Status = dados.Status;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeletarAsync(int id, CancellationToken cancellationToken = default)
    {
        var projeto = await _context.Projetos.FindAsync(new object[] { id }, cancellationToken);
        if (projeto is null)
        {
            return false;
        }

        _context.Projetos.Remove(projeto);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
