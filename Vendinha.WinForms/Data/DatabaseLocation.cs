namespace Vendinha.WinForms.Data;

internal static class DatabaseLocation
{
    public static string GetDatabasePath()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Vendinha");

        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "vendinha.db");
    }
}
