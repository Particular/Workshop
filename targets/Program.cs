using System.IO;
using static Bullseye.Targets;
using static SimpleExec.Command;

internal class Program
{
    public static void Main(string[] args)
    {
        Target("default", DependsOn("demos", "exercises"));

        Target(
            "demos",
            Directory.EnumerateFiles("demos", "*.sln", SearchOption.AllDirectories),
            solution => Run("dotnet", $"build {solution} --configuration Debug"));

        Target(
            "exercises",
            Directory.EnumerateFiles("exercises", "*.sln", SearchOption.AllDirectories),
            solution => Run("dotnet", $"build {solution} --configuration Debug"));

        RunTargets(args);
    }
}
