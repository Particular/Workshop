#r "packages/Bullseye.1.1.0-rc.2/lib/netstandard2.0/Bullseye.dll"
#load "scripts/cmd.csx"

using System;
using static Bullseye.Targets;

Target("default", DependsOn("demos", "exercises"));

Target(
    "demos",
    Directory.EnumerateFiles("demos", "*.sln", SearchOption.AllDirectories),
    solution => Cmd("dotnet", $"build {solution} --configuration Debug"));

Target(
    "exercises",
    Directory.EnumerateFiles("exercises", "*.sln", SearchOption.AllDirectories),
    solution => Cmd("dotnet", $"build {solution} --configuration Debug"));

RunTargets(Args);
