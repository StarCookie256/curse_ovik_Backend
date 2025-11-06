using System.Data.SQLite;

namespace PerfumeryBackend.DatabaseLayer;

// класс подключения к БД
public class PerfumeData
{
    private static string connString = @"Data Source=D:\Games\My projects c#\PerfumeryBackend\perfumeryDB.sqlite;Version=3;";
    public static SQLiteConnection conn = new SQLiteConnection(connString);

    public static void DBconnect()
    {
        using var conn = new SQLiteConnection(connString);
    }
}
