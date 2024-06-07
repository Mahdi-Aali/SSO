using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Authentication;

public record LoginDto
{
    [Required(ErrorMessage = "Username can't be null or empty.")]
    [MaxLength(256, ErrorMessage = "Username length can't be bigger than 256 character.")]
    [MinLength(4, ErrorMessage = "Username length can't be less than 4 character.")]
    public string Username { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password can't be null or empty.")]
    [MaxLength(256, ErrorMessage = "Password length can't be bigger than 256 character.")]
    [MinLength(8, ErrorMessage = "Password length can't be less than 8 character.")]
    [DataType(DataType.Password)]
    public string Password { get; init; } = string.Empty;


    public bool RememberMe { get; init; } = false;
}
