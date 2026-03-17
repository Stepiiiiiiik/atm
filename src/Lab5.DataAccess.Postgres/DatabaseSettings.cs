namespace Lab5.DataAccess.Postgres;

public class DatabaseSettings
{
    public DatabaseSettings() { }

    public DatabaseSettings(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; set; } = string.Empty;
}