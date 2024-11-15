using System.Linq.Expressions;
using Employee.Service.Exceptions;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Employee.Service.Helpers;

public static class DbContextHelpers
{
    public static async Task<T> SingleOrThrowAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> func, CancellationToken ct)
        where T : class
        => await dbSet.SingleOrDefaultAsync(func, ct) 
           ?? throw new EmployeeException("Not found", StatusCode.NotFound);
}