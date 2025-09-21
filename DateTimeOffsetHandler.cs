using Dapper;
using System.Data;

namespace simple_note_app_api
{
    public class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
    {
        public override DateTimeOffset Parse(object value)
        {
            if (value is DateTime dateTime)
            {
                // Tell Dapper that database DateTime is UTC
                var utcDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                return new DateTimeOffset(utcDateTime);
            }
            return DateTimeOffset.Parse(value.ToString()!);
        }

        public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
        {
            // When saving, convert DateTimeOffset back to UTC DateTime
            parameter.Value = value.UtcDateTime;
        }
    }
}
