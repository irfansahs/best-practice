using Serilog.Sinks.MSSqlServer;

namespace Infrastructure.Logging;

public static class SqlServerLogSchema
{
    public static ColumnOptions CreateColumnOptions()
    {
        var columns = new ColumnOptions();
        columns.Store.Remove(StandardColumn.Properties);
        columns.Store.Remove(StandardColumn.MessageTemplate);
        columns.TimeStamp.NonClusteredIndex = true;
        columns.Level.NonClusteredIndex = true;

        columns.AdditionalColumns =
        [
            new SqlColumn("TraceId", System.Data.SqlDbType.NVarChar, dataLength: 64),
            new SqlColumn("SpanId", System.Data.SqlDbType.NVarChar, dataLength: 64),
            new SqlColumn("CorrelationId", System.Data.SqlDbType.NVarChar, dataLength: 64),
            new SqlColumn("UserId", System.Data.SqlDbType.NVarChar, dataLength: 64),
            new SqlColumn("Culture", System.Data.SqlDbType.NVarChar, dataLength: 16),
            new SqlColumn("RequestPath", System.Data.SqlDbType.NVarChar, dataLength: 512),
            new SqlColumn("RequestMethod", System.Data.SqlDbType.NVarChar, dataLength: 16),
            new SqlColumn("StatusCode", System.Data.SqlDbType.Int),
            new SqlColumn("ElapsedMs", System.Data.SqlDbType.BigInt),
            new SqlColumn("ClientIp", System.Data.SqlDbType.NVarChar, dataLength: 64),
            new SqlColumn("SourceContext", System.Data.SqlDbType.NVarChar, dataLength: 256),
            new SqlColumn("Environment", System.Data.SqlDbType.NVarChar, dataLength: 64),
            new SqlColumn("MachineName", System.Data.SqlDbType.NVarChar, dataLength: 64)
        ];

        return columns;
    }
}
