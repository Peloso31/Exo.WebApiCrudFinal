using Exo.WebApi.Contexts;
using Exo.WebApi.Models;
using Exo.WebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Exo.WebApi.Tests;

/// <summary>
/// Unit tests for the repository, using EF Core's in-memory provider.
/// Each test gets its own database name, so tests never share state.
/// </summary>
public class ProjetoRepositoryTests
{
    private static ExoContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ExoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ExoContext(options);
    }

    private static Projeto NovoProjeto(string nome = "Portal do Cliente") => new()
    {
        NomeDoProjeto = nome,
        Area = "Backend",
        Status = true
    };

    [Fact]
    public async Task CadastrarAsync_SavesProjectAndGeneratesId()
    {
        await using var context = CreateContext();
        var repository = new ProjetoRepository(context);

        var criado = await repository.CadastrarAsync(NovoProjeto());

        Assert.True(criado.Id > 0);
        Assert.Single(await repository.ListarAsync());
    }

    [Fact]
    public async Task ListarAsync_ReturnsProjectsOrderedById()
    {
        await using var context = CreateContext();
        var repository = new ProjetoRepository(context);
        await repository.CadastrarAsync(NovoProjeto("Primeiro"));
        await repository.CadastrarAsync(NovoProjeto("Segundo"));

        var projetos = await repository.ListarAsync();

        Assert.Equal(new[] { "Primeiro", "Segundo" }, projetos.Select(p => p.NomeDoProjeto));
    }

    [Fact]
    public async Task BuscarPorIdAsync_ReturnsNull_WhenIdDoesNotExist()
    {
        await using var context = CreateContext();
        var repository = new ProjetoRepository(context);

        Assert.Null(await repository.BuscarPorIdAsync(999));
    }

    [Fact]
    public async Task AtualizarAsync_ChangesAllFields_WhenProjectExists()
    {
        await using var context = CreateContext();
        var repository = new ProjetoRepository(context);
        var criado = await repository.CadastrarAsync(NovoProjeto());

        var atualizado = await repository.AtualizarAsync(criado.Id, new Projeto
        {
            NomeDoProjeto = "Novo nome",
            Area = "Dados",
            Status = false
        });

        var buscado = await repository.BuscarPorIdAsync(criado.Id);
        Assert.True(atualizado);
        Assert.NotNull(buscado);
        Assert.Equal("Novo nome", buscado.NomeDoProjeto);
        Assert.Equal("Dados", buscado.Area);
        Assert.False(buscado.Status);
    }

    [Fact]
    public async Task AtualizarAsync_ReturnsFalse_WhenProjectDoesNotExist()
    {
        await using var context = CreateContext();
        var repository = new ProjetoRepository(context);

        Assert.False(await repository.AtualizarAsync(999, NovoProjeto()));
    }

    [Fact]
    public async Task DeletarAsync_RemovesProject_WhenProjectExists()
    {
        await using var context = CreateContext();
        var repository = new ProjetoRepository(context);
        var criado = await repository.CadastrarAsync(NovoProjeto());

        var deletado = await repository.DeletarAsync(criado.Id);

        Assert.True(deletado);
        Assert.Empty(await repository.ListarAsync());
    }

    [Fact]
    public async Task DeletarAsync_ReturnsFalse_WhenProjectDoesNotExist()
    {
        await using var context = CreateContext();
        var repository = new ProjetoRepository(context);

        Assert.False(await repository.DeletarAsync(999));
    }
}
