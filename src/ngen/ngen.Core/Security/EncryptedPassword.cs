namespace ngen.Core.Security
{
    /// <summary>
    ///     The result of a password hash calculation. Contains the hash value
    ///     and the salt value used when hashing.
    /// </summary>
    public class EncryptedPassword
    {
        /// <summary>
        ///     The computed hash
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        ///     The salt used when hashing
        /// </summary>
        public string Salt { get; set; }
    }
}