using System.ComponentModel.DataAnnotations;

namespace Exo.WebApi.Dtos;

/// <summary>
/// Body accepted by the create and update endpoints.
/// Keeping it separate from the entity means the client can never set the Id.
/// </summary>
public sealed class ProjetoRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string NomeDoProjeto { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Area { get; set; } = string.Empty;

    public bool Status { get; set; }
}
