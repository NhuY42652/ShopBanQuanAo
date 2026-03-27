using System.Text;
using Microsoft.Data.SqlClient;

namespace ShopBanQuanAoOnline.Data;

public static class DbScriptInitializer
{
    public static async Task InitializeFromScriptAsync(
        string connectionString,
        string sqlScriptPath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(sqlScriptPath))
        {
            throw new FileNotFoundException($"Không tìm thấy script SQL: {sqlScriptPath}");
        }

        var scriptContent = await File.ReadAllTextAsync(sqlScriptPath, cancellationToken);
        var commands = SplitSqlCommandsByGo(scriptContent);

        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "master"
        };

        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        foreach (var commandText in commands)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                continue;
            }

            await using var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private static IEnumerable<string> SplitSqlCommandsByGo(string script)
    {
        var sb = new StringBuilder();

        foreach (var line in script.Split('\n'))
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                var cmd = sb.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    yield return cmd;
                }

                sb.Clear();
                continue;
            }

            sb.AppendLine(line);
        }

        var remaining = sb.ToString().Trim();
        if (!string.IsNullOrWhiteSpace(remaining))
        {
            yield return remaining;
        }
    }
}
