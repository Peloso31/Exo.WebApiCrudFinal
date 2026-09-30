namespace Exo.WebApi.Models;

/// <summary>
/// A project stored in the database.
/// </summary>
public class Projeto
{
    public int Id { get; set; }

    public string NomeDoProjeto { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;

    /// <summary>
    /// True when the project is active.
    /// </summary>
    public bool Status { get; set; }
}
