using System.Net;
using System.Net.Http.Json;
using Exo.WebApi.Models;

namespace Exo.WebApi.Tests;

/// <summary>
/// Integration tests: real HTTP requests against the API running in memory.
/// Each test creates the data it needs, so the order of tests does not matter.
/// </summary>
public class ProjetosApiTests : IClassFixture<ApiFactory>
{
    private const string BaseUrl = "/api/projetos";
    private readonly HttpClient _client;

    public ProjetosApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Projeto> CriarProjetoAsync(string nome = "Portal do Cliente")
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, new { nomeDoProjeto = nome, area = "Backend", status = true });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Projeto>())!;
    }

    [Fact]
    public async Task Post_ReturnsCreatedWithLocationHeader()
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, new { nomeDoProjeto = "API de Pedidos", area = "Backend", status = true });

        var criado = await response.Content.ReadFromJsonAsync<Projeto>();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.NotNull(criado);
        Assert.True(criado.Id > 0);
        Assert.Equal("API de Pedidos", criado.NomeDoProjeto);
    }

    [Theory]
    [InlineData("", "Backend")]
    [InlineData("AB", "Backend")]
    [InlineData("Nome valido", "")]
    public async Task Post_ReturnsBadRequest_WhenBodyIsInvalid(string nome, string area)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, new { nomeDoProjeto = nome, area, status = true });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_IgnoresIdSentByClient()
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, new { id = 12345, nomeDoProjeto = "Projeto X", area = "Dados", status = true });

        var criado = await response.Content.ReadFromJsonAsync<Projeto>();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(12345, criado!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsProject_WhenItExists()
    {
        var criado = await CriarProjetoAsync("Relatorios");

        var buscado = await _client.GetFromJsonAsync<Projeto>($"{BaseUrl}/{criado.Id}");

        Assert.NotNull(buscado);
        Assert.Equal("Relatorios", buscado.NomeDoProjeto);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenIdDoesNotExist()
    {
        var response = await _client.GetAsync($"{BaseUrl}/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ContainsCreatedProject()
    {
        var criado = await CriarProjetoAsync("Listagem");

        var projetos = await _client.GetFromJsonAsync<List<Projeto>>(BaseUrl);

        Assert.NotNull(projetos);
        Assert.Contains(projetos, p => p.Id == criado.Id);
    }

    [Fact]
    public async Task Put_UpdatesProject_WhenItExists()
    {
        var criado = await CriarProjetoAsync();

        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{criado.Id}", new { nomeDoProjeto = "Nome atualizado", area = "Dados", status = false });
        var buscado = await _client.GetFromJsonAsync<Projeto>($"{BaseUrl}/{criado.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("Nome atualizado", buscado!.NomeDoProjeto);
        Assert.False(buscado.Status);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
    {
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/999999", new { nomeDoProjeto = "Qualquer", area = "Dados", status = true });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesProject_WhenItExists()
    {
        var criado = await CriarProjetoAsync();

        var response = await _client.DeleteAsync($"{BaseUrl}/{criado.Id}");
        var depois = await _client.GetAsync($"{BaseUrl}/{criado.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, depois.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenIdDoesNotExist()
    {
        var response = await _client.DeleteAsync($"{BaseUrl}/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
