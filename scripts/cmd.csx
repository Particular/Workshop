using System;
using System.Diagnostics;

public static void Cmd(string fileName, string args)
{
    using (var process = new Process())
    {
        process.StartInfo = new ProcessStartInfo {
            FileName = $"\"{fileName}\"",
            Arguments = args,
            UseShellExecute = false,
        };
        
        Console.WriteLine($"Running '{process.StartInfo.FileName} {process.StartInfo.Arguments}'...");
        process.Start();

        process.WaitForExit();
        
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"The command exited with code {process.ExitCode}.");
        }
    }
}
