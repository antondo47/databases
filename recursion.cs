using System;

class Program
{
    static void Main(string[] args)
    {
        long bytes = input();
        string result = ConvertBytesToHumanReadable(bytes);
        Console.WriteLine(result);
    }

    static string ConvertBytesToHumanReadable(long bytes)
    {
        if (bytes < 0) throw new ArgumentOutOfRangeException(nameof(bytes), "Bytes cannot be negative.");

        if (bytes == 0) return "0 B";

        string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        int i = 0;

        while (bytes >= 1024 && i < suffixes.Length - 1)
        {
            bytes /= 1024;
            i++;
        }

        return $"{bytes} {suffixes[i]}";
    }
}