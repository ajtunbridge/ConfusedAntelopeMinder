namespace ngen.Core.Security
{
    public interface IPasswordProvider
    {
        string HashPassword(string plainPassword);
        
        bool Verify(string plainPassword, string hash);
    }
}