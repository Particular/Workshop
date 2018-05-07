using System;
using System.Diagnostics;

public static void Start(string fileName, string args)
{
    using (var process = new Process())
    {
        process.StartInfo = new ProcessStartInfo {
            FileName = $"\"{fileName}\"",
            Arguments = args,
            UseShellExecute = true,
        };
        
        Console.WriteLine($"Starting '{process.StartInfo.FileName} {process.StartInfo.Arguments}'...");
        process.Start();
    }
}
