using System;

namespace SimpleJwtServerAuthenticationLibrary
{
    public interface ISecurityUtilityService
    {
        string GetUserString();
        string CreateToken(ClaimUserModel user, TimeSpan expire);
        HashModel CreatePasswordHash(string password);
        bool VerifyPasswordHash(string password, HashModel hash);
    }
}