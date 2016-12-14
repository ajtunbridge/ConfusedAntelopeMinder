using ngen.Data;
using ngen.Data.Model;

namespace ngen.Domain.IO
{
    public class AzureDocumentStore : DocumentStoreBase
    {
        public AzureDocumentStore(Employee currentEmployee) : base(currentEmployee)
        {
        }

        public AzureDocumentStore(ngenDbContext dataContext, Employee currentEmployee)
            : base(dataContext, currentEmployee)
        {
        }
    }
}