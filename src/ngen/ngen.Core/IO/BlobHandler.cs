#region Using directives

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace ngen.Core.IO
{
    /// <summary>
    ///     Provides methods to compress, decompress, encrypt and decrypt serializable objects to and from byte arrays
    /// </summary>
    public static class BlobHandler
    {
        /// <summary>
        ///     Serializes and compresses the provided object into a byte array
        /// </summary>
        /// <param name="obj">The serializable object to process</param>
        /// <returns></returns>
        public static byte[] Compress(object obj)
        {
            byte[] bytes;

            var formatter = new BinaryFormatter();

            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, obj);
                bytes = memoryStream.ToArray();
            }

            return FileCompressor.Compress(bytes);
        }

        /// <summary>
        ///     Decompress and deserialize an object from a byte array
        /// </summary>
        /// <typeparam name="T">The type of the serialized object</typeparam>
        /// <param name="blob">The byte array containing the compressed object</param>
        /// <returns></returns>
        public static T Decompress<T>(byte[] blob)
        {
            T obj;

            var decompressedBytes = FileCompressor.Decompress(blob);

            var formatter = new BinaryFormatter();

            using (var memoryStream = new MemoryStream(decompressedBytes))
            {
                obj = (T) formatter.Deserialize(memoryStream);
            }

            return obj;
        }
    }
}