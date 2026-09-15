using System.ComponentModel.DataAnnotations;

namespace Goto.Controllers.Requests;

public sealed class UpdateAnchorRequest
{
    [MaxLength(1024)]
    public required string Description { get; init; }
}
