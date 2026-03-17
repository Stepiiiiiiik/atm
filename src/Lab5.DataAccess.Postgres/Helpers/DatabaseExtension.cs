using DbUp;
using DbUp.Engine;
using System.Reflection;

namespace Lab5.DataAccess.Postgres.Helpers;

public static class DatabaseExtension
{
    public static bool MigrateDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        UpgradeEngine? upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        DatabaseUpgradeResult? result = upgrader.PerformUpgrade();

        return result.Successful;
    }
}