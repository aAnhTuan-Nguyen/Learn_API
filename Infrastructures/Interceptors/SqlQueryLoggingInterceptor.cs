using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Diagnostics;

namespace TodoWeb.Infrastructures.Interceptors
{
    // log lại những câu query có thời gian thực thi lớn hơn 2ms
    // 
    public class SqlQueryLoggingInterceptor : DbCommandInterceptor
    {
        private Stopwatch stopwatch = new Stopwatch();
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            stopwatch.Start();
            

            return base.ReaderExecuting(command, eventData, result);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            // để ghi log trong file thì bên này có
            // using ở đây là dùng xong depoist luôn append: true là ghi ở cuối của file còn false là ghi đè
            using StreamWriter writer = new StreamWriter("D:\\CSharpAH\\TodoWeb\\sqllog.txt", append: true);
            stopwatch.Stop();
            var miliseconds = stopwatch.ElapsedMilliseconds;
            if (miliseconds > 2)
            {
                writer.WriteLine(command.CommandText);// đây là câu lệnh sql
            }

            return base.ReaderExecuted(command, eventData, result);
        }

    }
}
