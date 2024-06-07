using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Authentication;

public record TFADto
{
    [Required(ErrorMessage = "Invalid user.")]
    public Guid UserId { get; init; }

    [Required(ErrorMessage = "Two factor authentication code is required.")]
    [Length(6, 6, ErrorMessage = "TFA token can't be more or less than 6 character.")]
    public string TFAToken { get; init; } = string.Empty;

    public bool RememberMe { get; init; }
}
