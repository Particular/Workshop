using System;
using System.Diagnostics;
using System.IO;
using static Bullseye.Targets;

internal class Program
{
    public static void Main(string[] args)
    {
        Target("default", DependsOn("demos", "exercises"));

        Target(
            "demos",
            Directory.EnumerateFiles("demos", "*.sln", SearchOption.AllDirectories),
            solution => Cmd("dotnet", $"build {solution} --configuration Debug"));

        Target(
            "exercises",
            Directory.EnumerateFiles("exercises", "*.sln", SearchOption.AllDirectories),
            solution => Cmd("dotnet", $"build {solution} --configuration Debug"));

        RunTargets(args);
    }

    private static void Cmd(string fileName, string args)
    {
        using (var process = new Process())
        {
            process.StartInfo = new ProcessStartInfo
            {
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
}
