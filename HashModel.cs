namespace SimpleJwtServerAuthenticationLibrary
{
    public record HashModel(byte[]  PasswordHash, byte[] PasswordSalt);
    
}