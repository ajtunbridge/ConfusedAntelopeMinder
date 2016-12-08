#region Using directives

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace ngen.Core.Security
{
    /// <summary>
    ///     Provides methods for the encryption/decryption of strings and
    ///     Byte arrays using a symmetric cryptographic algorithm
    /// </summary>
    public sealed class SymmetricCryptoProvider : IDisposable
    {
        #region Private Variables

        /// <summary>
        ///     The symmetric algorithm to use for cryptography
        /// </summary>
        private readonly SymmetricAlgorithm _algorithm;

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Releases all resources used by the symmetric algorithm
        /// </summary>
        public void Dispose()
        {
            _algorithm.Clear();
            _algorithm.Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Process the supplied Byte array using the ICryptoTransform object provided
        /// </summary>
        /// <param name="data">The data to process</param>
        /// <param name="startIndex">The index to start processing from</param>
        /// <param name="count">The number of bytes to process</param>
        /// <param name="cryptor">The ICryptoTransform object to use</param>
        /// <returns></returns>
        private byte[] Process(byte[] data, int startIndex, int count, ICryptoTransform cryptor)
        {
            // The memory stream granularity must match the block size
            // of the current cryptographic operation
            var capacity = count;
            var mod = count%_algorithm.BlockSize;

            if (mod > 0)
            {
                capacity += _algorithm.BlockSize - mod;
            }

            var memoryStream = new MemoryStream(capacity);

            var cryptoStream = new CryptoStream(
                memoryStream,
                cryptor,
                CryptoStreamMode.Write);

            cryptoStream.Write(data, startIndex, count);
            cryptoStream.FlushFinalBlock();

            cryptoStream.Close();
            cryptoStream = null;

            cryptor.Dispose();
            cryptor = null;

            return memoryStream.ToArray();
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructs using the Rijndael algorithm by default. Remember to store the Key and IV somewhere or you ain't gettin'
        ///     yo' data!
        /// </summary>
        public SymmetricCryptoProvider()
            : this(SymmetricCryptoAlgorithm.Rijndael)
        {
        }

        /// <summary>
        ///     Constructs using the specified symmetric algorithm. Remember to store the Key and IV somewhere or you ain't gettin'
        ///     yo' data back!
        /// </summary>
        /// <param name="algorithm">The symmetric algorithm to use</param>
        public SymmetricCryptoProvider(SymmetricCryptoAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case SymmetricCryptoAlgorithm.AES:
                    _algorithm = new AesCryptoServiceProvider();
                    break;

                case SymmetricCryptoAlgorithm.Rijndael:
                    _algorithm = new RijndaelManaged();
                    break;

                case SymmetricCryptoAlgorithm.TripleDES:
                    _algorithm = new TripleDESCryptoServiceProvider();
                    break;

                case SymmetricCryptoAlgorithm.RC2:
                    _algorithm = new RC2CryptoServiceProvider();
                    break;

                default:
                    _algorithm = new RijndaelManaged();
                    break;
            }

            _algorithm.Mode = CipherMode.CBC;

            _algorithm.GenerateIV();
            _algorithm.GenerateKey();

            Key = _algorithm.Key;
            IV = _algorithm.IV;
        }

        /// <summary>
        ///     Constructs using the specified symmetric algorithm, key and initialization vector
        /// </summary>
        /// <param name="algorithm">The symmetric algorithm to use</param>
        /// <param name="key">The hexadecimal encoded key</param>
        /// <param name="iv">The hexadcimal encoded initialization vector</param>
        public SymmetricCryptoProvider(SymmetricCryptoAlgorithm algorithm, string key, string iv)
        {
            switch (algorithm)
            {
                case SymmetricCryptoAlgorithm.AES:
                    _algorithm = new AesCryptoServiceProvider();
                    break;

                case SymmetricCryptoAlgorithm.Rijndael:
                    _algorithm = new RijndaelManaged();
                    break;

                case SymmetricCryptoAlgorithm.TripleDES:
                    _algorithm = new TripleDESCryptoServiceProvider();
                    break;

                case SymmetricCryptoAlgorithm.RC2:
                    _algorithm = new RC2CryptoServiceProvider();
                    break;

                default:
                    _algorithm = new RijndaelManaged();
                    break;
            }

            _algorithm.Mode = CipherMode.CBC;

            _algorithm.IV = iv.ToBytes();
            _algorithm.Key = key.ToBytes();

            Key = _algorithm.Key;
            IV = _algorithm.IV;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the secret key to use
        /// </summary>
        public byte[] Key
        {
            get { return _algorithm.Key; }
            set { _algorithm.Key = value; }
        }

        /// <summary>
        ///     Gets or sets the initialization vector for the symmetric algorithm
        /// </summary>
        public byte[] IV
        {
            get { return _algorithm.IV; }
            set { _algorithm.IV = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Encrypts the supplied array of bytes
        /// </summary>
        /// <param name="plainBytes">The array of bytes to encrypt</param>
        /// <returns></returns>
        public byte[] EncryptBytes(byte[] plainBytes)
        {
            byte[] output;

            using (var cryptoTransform = _algorithm.CreateEncryptor())
            {
                output = Process(plainBytes, 0, plainBytes.Length, cryptoTransform);
            }

            return output;
        }

        /// <summary>
        ///     Descrypts the supplied array of bytes
        /// </summary>
        /// <param name="cipherBytes">The array of bytes to decrypt</param>
        /// <returns></returns>
        public byte[] DecryptBytes(byte[] cipherBytes)
        {
            byte[] output;

            using (var cryptoTransform = _algorithm.CreateDecryptor())
            {
                output = Process(cipherBytes, 0, cipherBytes.Length, cryptoTransform);
            }

            return output;
        }

        /// <summary>
        ///     Encrypts the supplied string
        /// </summary>
        /// <param name="plainText">The plain text string to encrypt</param>
        /// <returns></returns>
        public string EncryptString(string plainText)
        {
            return Convert.ToBase64String(EncryptBytes(Encoding.UTF8.GetBytes(plainText)));
        }

        /// <summary>
        ///     Decrypts the supplied string
        /// </summary>
        /// <param name="cipherText">The cipher text to decrypt</param>
        /// <returns></returns>
        public string DecryptString(string cipherText)
        {
            return Encoding.UTF8.GetString(DecryptBytes(Convert.FromBase64String(cipherText)));
        }

        /// <summary>
        ///     Generates a random secret key and initialization vector
        /// </summary>
        public void RandomizeKeyAndIV()
        {
            _algorithm.GenerateKey();
            _algorithm.GenerateIV();
        }

        #endregion
    }
}