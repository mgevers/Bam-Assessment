﻿using Ardalis.Result;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stargate.Core.Contracts;
using Stargate.Persistence.Sql;

namespace Stargate.Persistence;

public class EFRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly StargateDbContext dbContext;

    public EFRepository(StargateDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Add(TEntity entity)
    {
        dbContext.Add(entity);
    }

    public void AddMany(IReadOnlyCollection<TEntity> entities)
    {
        dbContext.AddRange(entities);
    }

    public void Remove(TEntity entity)
    {
        dbContext.Remove(entity);
    }

    public void RemoveMany(IReadOnlyCollection<TEntity> entities)
    {
        dbContext.RemoveRange(entities);

    }

    public void Update(TEntity entity)
    {
        dbContext.Update(entity);
    }

    public void UpdateMany(IReadOnlyCollection<TEntity> entities)
    {
        dbContext.UpdateRange(entities);
    }

    public async Task<Result> CommitTransaction(CancellationToken cancellation = default)
    {
        try
        {
            var count = await this.dbContext.SaveChangesAsync(cancellation);
            return Result.Success();
        }
        catch (DbUpdateException ex)
        {
            var message = ex.Message.Replace("See the inner exception for details.", string.Empty);
            return Result.Conflict($"conflict - {message} {ex.InnerException?.Message}");
        }
        catch (SqlException ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }
}
