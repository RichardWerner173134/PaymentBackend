using System.Data;
using System.Data.SqlClient;

namespace Database
{
    public static class DatabaseExtensions
    {
        // https://stackoverflow.com/questions/18001129/how-to-get-raw-output-from-sql-command-in-c-sharp
        public static void PrintResult(this SqlDataReader reader)
        {
            var valueArray = new object[reader.FieldCount];
            reader.GetValues(valueArray);

            foreach (object o in valueArray)
            {
                Console.WriteLine(o);
            }
        }

        // https://stackoverflow.com/questions/1772025/sql-data-reader-handling-null-column-values
        public static string SafeGetString(this SqlDataReader reader, string colName)
        {
            if (!reader.IsDBNull(colName))
            {
                return reader.GetString(colName);
            }

            return string.Empty;
        }

        public static long? SafeGetInt64(this SqlDataReader reader, string colName)
        {
            if (!reader.IsDBNull(colName))
            {
                return reader.GetInt64(colName);
            }

            return null;
        }

        public static int? SafeGetInt32(this SqlDataReader reader, string colName)
        {
            if (!reader.IsDBNull(colName))
            {
                return reader.GetInt32(colName);
            }

            return null;
        }

        public static short? SafeGetInt16(this SqlDataReader reader, string colName)
        {
            if (!reader.IsDBNull(colName))
            {
                return reader.GetInt16(colName);
            }

            return null;
        }

        public static DateTime? SafeGetDateTime(this SqlDataReader reader, string colName)
        {
            if (!reader.IsDBNull(colName))
            {
                return reader.GetDateTime(colName);
            }

            return null;
        }

        public static decimal? SafeGetDecimal(this SqlDataReader reader, string colName)
        {
            if (!reader.IsDBNull(colName))
            {
                return reader.GetDecimal(colName);
            }

            return null;
        }

        // https://stackoverflow.com/questions/170186/set-a-database-value-to-null-with-a-sqlcommand-parameters
        public static SqlParameter AddWithNullable<T>(this SqlParameterCollection parms, string parameterName, T? nullable) where T : struct
        {
            if (nullable.HasValue)
            {
                return parms.AddWithValue(parameterName, nullable.Value);
            }
            else
            {
                return parms.AddWithValue(parameterName, DBNull.Value);
            }
        }

        public static SqlParameter AddWithNullable(this SqlParameterCollection parms, string parameterName, string? s)
        {
            if (s != null)
            {
                return parms.AddWithValue(parameterName, s);
            }
            else
            {
                return parms.AddWithValue(parameterName, DBNull.Value);
            }
        }
    }
}
