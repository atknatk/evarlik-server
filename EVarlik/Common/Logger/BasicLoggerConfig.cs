using System.Configuration;
using System.Data;
using System.Web;
using log4net.Appender;
using log4net.Core;
using Npgsql;

namespace EVarlik.Common.Logger
{
    public class BasicLoggerConfig : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            using (NpgsqlConnection conn =
                new NpgsqlConnection(ConfigurationManager.ConnectionStrings["VarlikContext"]
                    .ConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand command =
                    new NpgsqlCommand(
                        "INSERT INTO varlik.\"VarlikLog\" (\"LogDate\",\"Thread\",\"LogLevel\",\"Logger\",\"Message\",\"StackTrace\",\"Ip\") VALUES (:log_date, :thread, :log_level, :logger, :message, :exception,:ip)",
                        conn))
                {
                    var logDate = command.CreateParameter();
                    logDate.Direction = ParameterDirection.Input;
                    logDate.DbType = DbType.DateTime;
                    logDate.ParameterName = ":log_date";
                    logDate.Value = loggingEvent.TimeStamp;
                    command.Parameters.Add(logDate);

                    var thread = command.CreateParameter();
                    thread.Direction = ParameterDirection.Input;
                    thread.DbType = DbType.String;
                    thread.ParameterName = ":thread";
                    thread.Value = loggingEvent.ThreadName;
                    command.Parameters.Add(thread);

                    var logLevel = command.CreateParameter();
                    logLevel.Direction = ParameterDirection.Input;
                    logLevel.DbType = DbType.String;
                    logLevel.ParameterName = ":log_level";
                    logLevel.Value = loggingEvent.Level;
                    command.Parameters.Add(logLevel);

                    var logger = command.CreateParameter();
                    logger.Direction = ParameterDirection.Input;
                    logger.DbType = DbType.String;
                    logger.ParameterName = ":logger";
                    logger.Value = loggingEvent.LoggerName;
                    command.Parameters.Add(logger);

                    var message = command.CreateParameter();
                    message.Direction = ParameterDirection.Input;
                    message.DbType = DbType.String;
                    message.ParameterName = ":message";
                    message.Value = loggingEvent.RenderedMessage;
                    command.Parameters.Add(message);

                    var exception = command.CreateParameter();
                    exception.Direction = ParameterDirection.Input;
                    exception.DbType = DbType.String;
                    exception.ParameterName = ":exception";
                    exception.Value = loggingEvent.GetExceptionString();
                    command.Parameters.Add(exception);

                    var ip = command.CreateParameter();
                    ip.Direction = ParameterDirection.Input;
                    ip.DbType = DbType.String;
                    ip.ParameterName = ":ip";
                    ip.Value = HttpContext.Current.Request.UserHostAddress;
                    command.Parameters.Add(ip);

                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
    }
}