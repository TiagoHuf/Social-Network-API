using SocialNetwork.Models;
using System.Security.Claims;

namespace SocialNetwork.Extensions;

public static class RoleClaimExtension
{
    public static IEnumerable<Claim> GetClaims(this User user)
    {
        var result = new List<Claim>
        {
            new (ClaimTypes.Name, user.Email),
        };

        result.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));

        return result;
    }
}