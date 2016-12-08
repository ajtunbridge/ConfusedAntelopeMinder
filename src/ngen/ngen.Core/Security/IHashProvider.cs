#region Using directives

using System.Threading.Tasks;

#endregion

namespace ngen.Core.Security
{
    public interface IHashProvider
    {
        byte[] ComputeHash(byte[] bytes);

        Task<byte[]> ComputeHashAsync(string fileName);
    }
}