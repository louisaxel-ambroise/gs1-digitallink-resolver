using System.ComponentModel.DataAnnotations;

namespace Goto.Controllers.Requests;

public sealed class UpdateAnchorRequest
{
    public required Dictionary<string, string> Description { get; init; }
}
