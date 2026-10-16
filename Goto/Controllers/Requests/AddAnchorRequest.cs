using System.ComponentModel.DataAnnotations;

namespace Goto.Controllers.Requests;

public sealed class AddAnchorRequest
{
    [MaxLength(255)]
    public required string Prefix { get; init; }
    public required Dictionary<string, string> Description { get; init; }
}
