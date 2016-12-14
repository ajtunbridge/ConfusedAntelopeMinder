#region Using directives

using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace ngen.Core.IO
{
    public class FileCompressor
    {
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        
        public async Task CompressAsync(string source)
        {
            await CompressAsync(source, source, true);
        }

        public async Task CompressAsync(string source, string destination, bool deleteSource = false)
        {
            await ProcessAsync(source, destination, false, deleteSource);
        }

        public async Task DecompressAsync(string source)
        {
            await DecompressAsync(source, source, true);
        }

        public async Task DecompressAsync(string source, string destination, bool deleteSource = false)
        {
            await ProcessAsync(source, destination, true, deleteSource);
        }

        private async Task ProcessAsync(string source, string destination, bool sourceIsCompressed, bool deleteSource = false)
        {
            var tmpDestination = $"{destination}.tmp";
            
            try
            {
                using (var outputStream = new FileStream(tmpDestination, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (var inputStream = new FileStream(source, FileMode.Open, FileAccess.Read))
                    {
                        using (var progressStream = new ProgressStream(inputStream))
                        {
                            progressStream.ProgressChanged += ProgressStream_ProgressChanged;

                            GZipStream gzip;

                            if (sourceIsCompressed)
                            {
                                gzip = new GZipStream(progressStream, CompressionMode.Decompress);
                                await gzip.CopyToAsync(outputStream);
                            }
                            else
                            {
                                gzip = new GZipStream(outputStream, CompressionMode.Compress);
                                await progressStream.CopyToAsync(gzip);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.Delete(tmpDestination);
                throw;
            }

            if (File.Exists(tmpDestination))
            {
                if (deleteSource)
                {
                    File.Delete(source);
                }

                File.Move(tmpDestination, destination);
            }
        }
        
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

        private void ProgressStream_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnProgressChanged(e);
        }

        private void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(null, e);
        }
    }
}