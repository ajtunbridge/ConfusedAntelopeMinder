using System;
using System.Threading.Tasks;
using ngen.Data.Model;
using ngen.Domain.EventArgs;
using ngen.Domain.Security;

namespace ngen.Domain.IO
{
    public interface IDocumentStore
    {
        /// <summary>
        ///     The employee currently logged in
        /// </summary>
        Employee CurrentEmployee { get; set; }

        event EventHandler<DocumentTransferEventArgs> TransferProgress;

        /// <summary>
        ///     Opens a temporary copy of the document version then deletes it when it has been closed
        /// </summary>
        /// <param name="version">The document version to open</param>
        /// <exception cref="DocumentAccessException">Thrown if employee does not have read access to the document</exception>
        /// <returns></returns>
        Task OpenTempAsync(DocumentVersion version);

        /// <summary>
        ///     Adds the specified source file to storage and attaches it to the specified entity
        /// </summary>
        /// <param name="source">The file to store</param>
        /// <param name="entity">The entity to attach to</param>
        /// <param name="changes">The text to store in the changes column</param>
        /// <exception cref="SystemPermissionException">Throw if employee doesn't have permission to manage documents</exception>
        /// <returns></returns>
        Task AddAsync(string source, object entity, string changes = null);

        /// <summary>
        ///     Deletes the specified document and all it's versions
        /// </summary>
        /// <param name="document">The document to delete</param>
        /// <returns></returns>
        Task DeleteAsync(Document document);

        /// <summary>
        ///     Checks the specified document out of the system for editing
        /// </summary>
        /// <param name="document">The document to check out</param>
        /// <returns></returns>
        Task CheckOutAsync(Document document);

        /// <summary>
        ///     Checks back in the specified document and creates a new version if necessary
        /// </summary>
        /// <param name="document">The document to check back in</param>
        /// <param name="changes">Any specified changes made to the document</param>
        /// <returns></returns>
        Task CheckInAsync(Document document, string changes = null);

        /// <summary>
        ///     Deletes any files that have been saved in the temp directory but not deleted
        /// </summary>
        void PurgeTempFiles();
    }
}