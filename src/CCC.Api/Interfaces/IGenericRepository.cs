namespace CCC.Api.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetOneBy(Dictionary<string, object> parameters);
        Task<List<TEntity>?> GetByParams(Dictionary<string, object> parameters);
    }
}
