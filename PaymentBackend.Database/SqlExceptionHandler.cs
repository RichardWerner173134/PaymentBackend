using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace PaymentBackend.Database
{

    public interface ISqlExceptionHandler
    {
        T ExecuteOrThrow<T>(Func<T> func);
        void ExecuteOrThrow(Action act);
    }

    public class SqlExceptionHandler : ISqlExceptionHandler
    {
        private readonly ILogger<SqlExceptionHandler> _logger;

        public SqlExceptionHandler(ILogger<SqlExceptionHandler> logger)
        {
            _logger = logger;
        }


        public T ExecuteOrThrow<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public void ExecuteOrThrow(Action act)
        {
            try
            {
                act.Invoke();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
