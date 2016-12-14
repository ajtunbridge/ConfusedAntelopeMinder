using ngen.Data.Model;
using ngen.Domain.IO;

namespace ngen.Domain.EventArgs
{
    public class DocumentTransferEventArgs : System.EventArgs
    {
        public DocumentTransferEventArgs(string fileName, DocumentTransferStatus currentStatus, int percentComplete)
        {
            FileName = fileName;
            CurrentStatus = currentStatus;
            PercentComplete = percentComplete;
        }

        public string FileName { get; private set; }

        public DocumentTransferStatus CurrentStatus { get; private set; }

        public int PercentComplete { get; private set; }
    }
}