namespace ngen.Core.Security
{
    public class BCryptPasswordProvider : IPasswordProvider
    {
        // how expensive the calculation is. Values can be 1 to 31.
        private const int WorkFactor = 15;

        public string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
        }

        public bool Verify(string plainPassword, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
        }
    }
}