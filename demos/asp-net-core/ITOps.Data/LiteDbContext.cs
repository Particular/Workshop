using LiteDB;

namespace ITOps.Data;

public class LiteDbContext : ILiteDbContext
{
    public LiteDatabase Database { get; }

    public LiteDbContext(LiteDbOptions options)
    {
        var storagePath = options.DatabaseLocation;
        if (string.IsNullOrEmpty(storagePath))
        {
            storagePath = FindStoragePath();
        }
        Directory.CreateDirectory(storagePath);

        var databaseLocation = Path.Combine(storagePath, options.DatabaseName.ToLower() + ".db");
        var connectionString = $"Filename={databaseLocation}; Connection=shared;";

        Database = new LiteDatabase(connectionString);

        options.DatabaseInitializer(Database);
    }

    static string FindStoragePath()
    {
        var directory = AppDomain.CurrentDomain.BaseDirectory;

        while (true)
        {
            // Finding a solution file takes precedence
            if (Directory.EnumerateFiles(directory).Any(file => file.EndsWith(".sln")))
            {
                return Path.Combine(directory, DefaultDatabaseDirectory);
            }

            // When no solution file was found try to find a database directory
            var databaseDirectory = Path.Combine(directory, DefaultDatabaseDirectory);
            if (Directory.Exists(databaseDirectory))
            {
                return databaseDirectory;
            }

            var parent = Directory.GetParent(directory);

            if (parent == null)
            {
                // throw for now. if we discover there is an edge then we can fix it in a patch.
                throw new Exception(
                    $"Unable to determine the storage directory path for the database due to the absence of a solution file. Please create a '{DefaultDatabaseDirectory}' directory in one of this project’s parent directories.");
            }

            directory = parent.FullName;
        }
    }

    const string DefaultDatabaseDirectory = ".db";
}

public interface ILiteDbContext
{
    LiteDatabase Database { get; }
}

public class LiteDbOptions
{
    public string DatabaseLocation { get; set; }
    public string DatabaseName { get; set; }
    public Action<LiteDatabase> DatabaseInitializer { get; set; }
}