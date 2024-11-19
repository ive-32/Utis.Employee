namespace Employee.RpcService.Helpers;

public static class DateTimeHelpers
{
    public static DateOnly ToDateOnly(this long value)
        => DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(value).DateTime);

    public static long ToUnixEpoch(this DateOnly value)
        => (long)(value.ToDateTime(TimeOnly.MinValue) - new DateTime(1970, 1, 1)).TotalSeconds;
}