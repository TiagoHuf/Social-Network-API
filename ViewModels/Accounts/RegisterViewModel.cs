using Biopark.CpaSurvey.Domain.Entities.Usuarios;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.NovaPasta.Accounts;

public class RegisterViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "O E-mail é inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Informe uma senha")]
    public string Password { get; set; }

    public Role Role { get; set; }
}