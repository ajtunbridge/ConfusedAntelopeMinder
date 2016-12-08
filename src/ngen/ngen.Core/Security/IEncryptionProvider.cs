#region Using directives

using System;
using System.ComponentModel;
using System.Threading.Tasks;

#endregion

namespace ngen.Core.Security
{
    public interface IEncryptionProvider
    {
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        Task EncryptFileAsync(string srcFile, string destFile, string password);

        Task DecryptFileAsync(string srcFile, string destFile, string password);
    }
}