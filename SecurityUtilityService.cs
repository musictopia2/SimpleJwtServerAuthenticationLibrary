using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace SimpleJwtServerAuthenticationLibrary
{
    public class SecurityUtilityService : ISecurityUtilityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServerJwtKey _jwtKey;
        public SecurityUtilityService(IHttpContextAccessor httpContextAccessor, IServerJwtKey jwtKey)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtKey = jwtKey;
        }
        public HashModel CreatePasswordHash(string password)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var passwordSalt = hmac.Key;
            var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return new HashModel(passwordHash, passwordSalt);
        }

        public string CreateToken(ClaimUserModel user, TimeSpan expire)
        {
            //decided to just use standard list so i don't have to use my custom functions just for that part.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.NameIdentifier),
                new Claim(ClaimTypes.Name, user.Name)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey.GetKey()));
            DateTime tempExpire = DateTime.Now.Add(expire);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: tempExpire,
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public string GetUserString()
        {
            string output = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return output;
        }

        public bool VerifyPasswordHash(string password, HashModel hash)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(hash.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            if (computedHash.Length != hash.PasswordHash.Length)
            {
                return false; //to prevent errors where the lengths are different but would get index out of range error.
            }
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != hash.PasswordHash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}