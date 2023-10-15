namespace Biopark.CpaSurvey.Domain.Entities.Usuarios;

/// <summary>
/// Tipos de permissão por usuário.
/// </summary>
public enum Role
{
    /// <summary>
    /// Usuário com permissão total.
    /// </summary>
    Admin,

    /// <summary>
    /// Usuário com permissão para postar e navegar.
    /// </summary>
    User,
}