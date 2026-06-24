using Azure.Data.Tables;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// Repository to operate on azure table storage entities.
    /// </summary>
    /// <typeparam name="TEntity">Table Entity to work on.</typeparam>
    public interface IAzureTableStorageRepository<TEntity>
        where TEntity : ITableEntity
    {
        /// <summary>
        /// Adds or replaces a table entity.
        /// </summary>
        /// <param name="entity">The entity to be added or replaced.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddOrReplace(TEntity entity);
    }
}