#region Using directives

using System.IO;
using System.IO.Compression;

#endregion

namespace ngen.Core.IO
{
    public static class GZip
    {
        public static byte[] Decompress(byte[] zippedData)
        {
            byte[] decompressedData;
            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(zippedData))
                {
                    using (var zip = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zip.CopyTo(outputStream);
                    }
                }
                decompressedData = outputStream.ToArray();
            }

            return decompressedData;
        }

        public static byte[] Compress(byte[] plainData)
        {
            byte[] compressedData;

            using (var outputStream = new MemoryStream())
            {
                using (var zip = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    zip.Write(plainData, 0, plainData.Length);
                }
                // Dont get the MemoryStream data before the GZipStream is disposed since it doesn’t yet contain complete compressed data.
                // GZipStream writes additional data including footer information when its been disposed
                compressedData = outputStream.ToArray();
            }

            return compressedData;
        }
    }
}