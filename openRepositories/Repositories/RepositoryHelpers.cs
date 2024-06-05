using System;
using System.Diagnostics;
using MySqlConnector;

namespace openPERRepositories.Repositories
{
    internal static class RepositoryHelpers
    {
        internal static void RunSqlAllRows(this MySqlConnection connection, string sql, Action<MySqlDataReader> rowHandler, params object[] parameters)
        {
//            var stopWatch = new Stopwatch();
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.AddWithValue("@p" + (i + 1).ToString(), parameters[i]);
                }
            }


//            stopWatch.Start();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                rowHandler(reader);
            }
//            stopWatch.Stop();
//            Console.WriteLine($"{sql} {stopWatch.ElapsedMilliseconds}");
        }
        internal static void RunSqlFirstRowOnly(this MySqlConnection connection, string sql, Action<MySqlDataReader> rowHandler, params object[] parameters)
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.AddWithValue("@p" + (i + 1).ToString(), parameters[i]);
                }
            }

//            var stopWatch = new Stopwatch();
//            stopWatch.Start();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                rowHandler(reader);
            }
//            stopWatch.Stop();
//            Console.WriteLine($"{sql} {stopWatch.ElapsedMilliseconds}");
        }

    }
}
