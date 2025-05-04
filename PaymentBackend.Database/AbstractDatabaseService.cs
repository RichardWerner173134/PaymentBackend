using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using PaymentBackend.Common.Exceptions;
using PaymentBackend.Settings;

namespace PaymentBackend.Database
{
    public abstract class AbstractDatabaseService
    {
        protected readonly IFunctionSettingsResolver _functionSettingsResolver;
        protected readonly ILogger _logger;
        protected readonly ISqlExceptionHandler _exceptionHandler;

        protected AbstractDatabaseService(ISqlExceptionHandler exceptionHandler, IFunctionSettingsResolver functionSettingsResolver, ILogger logger)
        {
            _exceptionHandler = exceptionHandler;
            _functionSettingsResolver = functionSettingsResolver;
            _logger = logger;
        }

        protected string GetConnectionString()
        {
            try
            {
                var cb = new SqlConnectionStringBuilder
                {
                    DataSource = _functionSettingsResolver.GetStringValue(FunctionSettings.DATABASE_HOST),
                    UserID = _functionSettingsResolver.GetStringValue(FunctionSettings.DATABASE_USER),
                    Password = _functionSettingsResolver.GetStringValue(FunctionSettings.DATABASE_PASSWORD),
                    InitialCatalog = _functionSettingsResolver.GetStringValue(FunctionSettings.DATABASE_NAME)
                };

                return cb.ConnectionString;
            }
            catch (SqlException e)
            {
                _logger.LogError(e.ToString());
                throw new InvalidValueException("Cannot build database connection string!");
            }
        }
    }
}
