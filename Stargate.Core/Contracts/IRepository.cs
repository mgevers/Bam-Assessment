using Ardalis.Result;

namespace Stargate.Core.Contracts;

public interface IRepository<TEntity>
    where TEntity : class
{
    void Add(TEntity entity);
    void AddMany(IReadOnlyCollection<TEntity> entities);
    void Update(TEntity entity);
    void UpdateMany(IReadOnlyCollection<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveMany(IReadOnlyCollection<TEntity> entities);

    Task<Result> CommitTransaction(CancellationToken cancellation = default);
}
