using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Database
{

    public interface ISqlExceptionHandler
    {
        T ExecuteOrThrow<T>(Func<T> func);
        T ExecuteOrReturn<T>(Func<T> func, T onErrorReturn);
        void Execute(Action act, bool logOnly = false);
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

        public T ExecuteOrReturn<T>(Func<T> func, T onErrorReturn)
        {
            try
            {
                return func.Invoke();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex.ToString());
                return onErrorReturn;
            }
        }

        public void Execute(Action act, bool logOnly = false)
        {
            try
            {
                act.Invoke();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex.ToString());
                if (!logOnly)
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
