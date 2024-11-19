using Google.Protobuf.WellKnownTypes;
using Utis.Minex.WrokerIntegration;

namespace Employee.Client.Dtos;

public class EmployeeDto
{
    public long? Id { get; set; }
    public string LastName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public DateOnly BirthDay { get; set; }
    public Sex Sex { get; set; }
    public bool HaveChildren { get; set; }

    public EmployeeDto()
    {
    }

    public EmployeeDto(EmployeeDto anotherDto)
    {
        Id = anotherDto.Id;
        FirstName = anotherDto.FirstName;
        MiddleName = anotherDto.MiddleName;
        LastName = anotherDto.LastName;
        Sex = anotherDto.Sex;
        BirthDay = anotherDto.BirthDay;
        HaveChildren = anotherDto.HaveChildren;
    }

    public EmployeeDto(WorkerMessage message)
    {
        Id = message.Id.Value;
        LastName = message.LastName;
        FirstName = message.FirstName;
        MiddleName = message.MiddleName;
        BirthDay = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(message.Birthday).DateTime);
        Sex = message.Sex;
    }
    
    public WorkerMessage GetWorkerMessage()
    {
        return new WorkerMessage()
        {
            LastName = LastName,
            FirstName = FirstName,
            MiddleName = MiddleName,
            Birthday = (long)(BirthDay.ToDateTime(TimeOnly.MinValue) - new DateTime(1970, 1, 1)).TotalSeconds,
            Sex = Sex,
            HaveChildren = HaveChildren,
            Id = Id.HasValue ? new Int64Value { Value = Id.Value } : new Int64Value()
        };
    }
}
