using System;
using Microsoft.Data.Sqlite;

namespace openPERRepositories.Repositories
{
    internal static class RepositoryHelpers
    {
        internal static void RunSqlAllRows(this SqliteConnection connection, string sql, Action<SqliteDataReader> rowHandler, params object[] parameters)
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.AddWithValue("$p" + (i + 1).ToString(), parameters[i]);
                }
            }

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                rowHandler(reader);
            }
        }
        internal static void RunSqlFirstRowOnly(this SqliteConnection connection, string sql, Action<SqliteDataReader> rowHandler, params object[] parameters)
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.AddWithValue("$p" + (i + 1).ToString(), parameters[i]);
                }
            }

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                rowHandler(reader);
            }
        }

    }
}
