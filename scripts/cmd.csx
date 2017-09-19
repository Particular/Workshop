using System;
using System.Diagnostics;

public static string ReadCmd(string fileName, string args)
{
    var output = new StringBuilder();
    using (var process = new Process())
    {
        process.StartInfo = new ProcessStartInfo {
            FileName = $"\"{fileName}\"",
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        process.OutputDataReceived += (sender, e) => output.AppendLine(e.Data);
        process.ErrorDataReceived += (sender, e) => output.AppendLine(e.Data);
        
        Console.WriteLine($"Running '{process.StartInfo.FileName} {process.StartInfo.Arguments}'...");
        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();
        
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"The command exited with code {process.ExitCode}. {output.ToString()}");
        }
    }

    return output.ToString();
}

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
