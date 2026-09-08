using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sims.BuildingManagement.Repositories.Csv;

// Pomocna klasa za citanje i pisanje CSV datoteka
// Prvi red datoteke je zaglavlje
// Polja koja sadrze zarez ili navodnike se navode pod navodnicima, 
// uz odvajanje unutrasnjih navodnika

internal static class CsvFile
{
    public static List<string[]> ReadRows(string path)
    {
        List<string[]> rows = new List<string[]>();
        if (!File.Exists(path))
            return rows;

        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Length == 0)
                continue;
            rows.Add(ParseLine(lines[i]));
        }

        return rows;
    }

    public static void WriteRows(string path, string[] header, IEnumerable<string[]> rows)
    {
        string directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        List<string> lines = new List<string> { FormatLine(header) };
        foreach (string[] row in rows)
            lines.Add(FormatLine(row));

        File.WriteAllLines(path, lines);
    }

    private static string[] ParseLine(string line)
    {
        List<string> fields = new List<string>();
        StringBuilder field = new StringBuilder();
        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char current = line[i];
            if (insideQuotes)
            {
                bool isEscapedQuote = current == '"' && i + 1 < line.Length && line[i + 1] == '"';
                if (isEscapedQuote)
                {
                    field.Append('"');
                    i++;
                }
                else if (current == '"')
                {
                    insideQuotes = false;
                }
                else
                {
                    field.Append(current);
                }
            }
            else if (current == '"')
            {
                insideQuotes = true;
            }
            else if (current == ',')
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(current);
            }
        }

        fields.Add(field.ToString());
        return fields.ToArray();
    }

    private static string FormatLine(string[] fields)
    {
        string[] escaped = new string[fields.Length];
        for (int i = 0; i < fields.Length; i++)
            escaped[i] = EscapeField(fields[i]);

        return string.Join(",", escaped);
    }

    private static string EscapeField(string value)
    {
        value ??= string.Empty;
        bool needsQuotes = value.Contains(',') || value.Contains('"') || value.Contains('\n');
        if (!needsQuotes)
            return value;

        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
