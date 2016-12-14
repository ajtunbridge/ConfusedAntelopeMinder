namespace ngen.Domain.IO
{
    public enum DocumentTransferStatus
    {
        ComputingHash,
        Compressing,
        Decompressing,
        Encrypting,
        Decrypting,
        Complete
    }
}
