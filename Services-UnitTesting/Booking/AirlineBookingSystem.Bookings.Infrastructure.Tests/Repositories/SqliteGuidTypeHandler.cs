using System.Data;
using Dapper;

namespace AirlineBookingSystem.Bookings.Infrastructure.Tests.Repositories;

internal sealed class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
    }

    public override Guid Parse(object value)
    {
        return value switch
        {
            Guid guid => guid,
            string text => Guid.Parse(text),
            byte[] bytes => new Guid(bytes),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType()} to Guid")
        };
    }
}
