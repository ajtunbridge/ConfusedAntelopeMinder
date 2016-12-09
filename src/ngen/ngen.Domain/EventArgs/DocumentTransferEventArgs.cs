using ngen.Data.Model;
using ngen.Domain.Enum;

namespace ngen.Domain.EventArgs
{
    public class DocumentTransferEventArgs : System.EventArgs
    {
        public DocumentTransferEventArgs(string fileName, DocumentTransferStep currentStep, int percentComplete)
        {
            FileName = fileName;
            CurrentStep = currentStep;
            PercentComplete = percentComplete;
        }

        public string FileName { get; private set; }

        public DocumentTransferStep CurrentStep { get; private set; }

        public int PercentComplete { get; private set; }
    }
}