using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

internal class TestDataManager(RecruitDataContext dataContext, IFixture fixture, string schemaName)
{
    private readonly string _schemaPrefix = $"[{schemaName}].[dbo]";
    
    public async Task<T> Create<T>(Action<T>? configure = null) where T : class
    {
        var entity = fixture.Create<T>();
        configure?.Invoke(entity);
        dataContext.Add(entity);
        await dataContext.SaveChangesAsync();
        dataContext.Entry(entity).State = EntityState.Detached;
        return entity;
    }
    
    public async Task<List<T>> CreateMany<T>(int count, Action<List<T>>? configure = null) where T : class
    {
        var entities = fixture.CreateMany<T>(count).ToList();
        configure?.Invoke(entities);
        dataContext.AddRange(entities);
        await dataContext.SaveChangesAsync();
        foreach (var entity in entities)
        {
            dataContext.Entry(entity).State = EntityState.Detached;
        }
        return entities;
    }

    public async Task<T?> Get<T>(object id) where T : class
    {
        return await dataContext.Set<T>().FindAsync(id);
    }

    public async Task WipeDataAsync()
    {
        await dataContext.Database.EnsureCreatedAsync();
        
        var tableNames = dataContext.Model
            .GetEntityTypes()
            .Select(t => t.GetTableName())
            .OfType<string>()
            .Distinct()
            .ToList();
        
        foreach (var sql in tableNames.Select(tableName => $"ALTER TABLE {_schemaPrefix}.[{tableName}] NOCHECK CONSTRAINT ALL"))
        {
            await dataContext.Database.ExecuteSqlRawAsync(sql);
        }
            
        foreach (var sql in tableNames.Select(tableName => $"DELETE FROM {_schemaPrefix}.[{tableName}]"))
        {
            await dataContext.Database.ExecuteSqlRawAsync(sql);
        }
        
        foreach (var sql in tableNames.Select(tableName => $"ALTER TABLE {_schemaPrefix}.[{tableName}] CHECK CONSTRAINT ALL"))
        {
            await dataContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
}