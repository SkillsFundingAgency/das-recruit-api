﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginatedList<T>> GetPagedAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending = true, 
        int totalRecords =  0,
        CancellationToken token = default)
    {
        if (!string.IsNullOrWhiteSpace(sortColumn))
        {
            var param = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(param, sortColumn);
            var lambda = Expression.Lambda(property, param);

            string methodName = isAscending ? "OrderBy" : "OrderByDescending";
            var orderByExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                [typeof(T), property.Type],
                query.Expression,
                Expression.Quote(lambda)
            );

            query = query.Provider.CreateQuery<T>(orderByExpression);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

        return new PaginatedList<T>(items, totalRecords, pageNumber, pageSize);
    }
}