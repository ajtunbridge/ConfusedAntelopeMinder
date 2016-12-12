using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngen.Domain.Enum
{
    public enum DocumentTransferStep
    {
        ComputingHash,
        Encrypting,
        Decrypting,
        Querying,
        Opening,
        Complete
    }
}
