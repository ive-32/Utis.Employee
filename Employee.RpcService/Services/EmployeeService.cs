using System.Data.Common;
using Employee.Data;
using Employee.Data.Entities;
using Employee.Proto;
using Employee.RpcService.Exceptions;
using Employee.RpcService.Helpers;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Utis.Minex.WrokerIntegration;

namespace Employee.RpcService.Services;

public class EmployeeService : IEmployeeService
{
    private readonly EmployeeDbContext _dbContext;

    public EmployeeService(EmployeeDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<long> Create(WorkerMessage request, CancellationToken ct)
    {
        var employee = new EmployeeEntity
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            BirthDay = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(request.Birthday).DateTime),
            Sex = (int)request.Sex,
            HaveChildren = request.HaveChildren
        };

        _dbContext.Add(employee);

        try
        {
            await _dbContext.SaveChangesAsync(ct);

            return employee.Id;
        }
        catch (DbException)
        {
            throw new EmployeeException(ErrorMessages.DbError);
        }
    }
    
    public async Task Update(long id, WorkerMessage request, CancellationToken ct)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var employee = await _dbContext.Employers.SingleOrThrowAsync(e => e.Id == id, ct);

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.MiddleName = request.MiddleName;
            employee.BirthDay =
                DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(request.Birthday).DateTime);
            employee.Sex = (int)request.Sex;
            employee.HaveChildren = request.HaveChildren;

            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw new EmployeeException(ErrorMessages.DbError);
        }
    }

    public async Task Delete(IdModel request, CancellationToken ct)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var employee = await _dbContext.Employers.SingleOrThrowAsync(e => e.Id == request.Id, ct);
            _dbContext.Employers.Remove(employee);
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbException)
        {
            throw new EmployeeException(ErrorMessages.DbError);
        }
    }

    public async Task<IReadOnlyList<WorkerMessage>> GetList(GetListModel request, CancellationToken ct)
    {
        
        var query = _dbContext.Employers.AsQueryable();

        if (request.Ids.Any())
            query = query.Where(e => request.Ids.Contains(e.Id));
        
        if (request.LastNames.Any())
            query = query.Where(e => request.LastNames.Contains(e.LastName));

        if (request.FirstNames.Any())
            query = query.Where(e => request.FirstNames.Contains(e.FirstName));
        
        if (request.MiddleNames.Any())
            query = query.Where(e => request.MiddleNames.Contains(e.MiddleName));

        if (request.MinBirthDay is not null)
            query = query.Where(e => e.BirthDay >= request.MinBirthDay.Value.ToDateOnly());

        if (request.MaxBirthDay is not null)
            query = query.Where(e => e.BirthDay <= request.MaxBirthDay.Value.ToDateOnly());

        if (request.HaveChildren is not null)
            query = query.Where(e => e.HaveChildren == request.HaveChildren.Value);

        if (request.Sexes.Any())
            query = query.Where(e => request.Sexes.Cast<int>().Contains(e.Sex));

        if (!string.IsNullOrEmpty(request.PartOfFirstName?.Value))
        {
            var searchPattern = $"%{request.PartOfFirstName.Value}%";
            query = query.Where(e => EF.Functions.Like(e.FirstName, searchPattern));
        }    
        
        if (!string.IsNullOrEmpty(request.PartOfLastName?.Value))
        {
            var searchPattern = $"%{request.PartOfLastName.Value}%";
            query = query.Where(e => EF.Functions.Like(e.LastName, searchPattern));
        }

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 50 : request.PageSize;
        
        var workerList = await query
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToListAsync(ct);
        
        return workerList.Select(w => new WorkerMessage
        {
            LastName = w.LastName,
            FirstName = w.FirstName,
            MiddleName = w.MiddleName,
            Birthday = w.BirthDay.ToUnixEpoch(),
            Sex = (Sex) w.Sex,
            HaveChildren = w.HaveChildren,
            Id = new Int64Value { Value = w.Id }
        }).ToList();
    }
}