#r "packages/Bullseye.1.0.0-rc.5/lib/netstandard2.0/Bullseye.dll"
#r "packages/SimpleExec.2.2.0/lib/netstandard2.0/SimpleExec.dll"

using System;
using static Bullseye.Targets;
using static SimpleExec.Command;

var vswhere = "packages/vswhere.2.1.4/tools/vswhere.exe";
var nuget = ".nuget/v4.3.0/NuGet.exe";
string msBuild = null;

var demoSolutions = Directory.EnumerateFiles("demos", "*.sln", SearchOption.AllDirectories);
var exerciseSolutions = Directory.EnumerateFiles("exercises", "*.sln", SearchOption.AllDirectories);

Add("default", DependsOn("demos", "exercises"));

Add(
    "restore-demos",
    () =>
    {
        foreach (var solution in demoSolutions)
        {
            Run(nuget, $"restore {solution}");
        }
    });

Add(
    "restore-exercises",
    () =>
    {
        foreach (var solution in exerciseSolutions)
        {
            Run(nuget, $"restore {solution}");
        }
    });

Add(
    "find-msbuild",
    () => msBuild = $"{Read(vswhere, "-latest -requires Microsoft.Component.MSBuild -property installationPath").Trim()}/MSBuild/15.0/Bin/MSBuild.exe");

Add(
    "demos",
    DependsOn("find-msbuild", "restore-demos"),
    () =>
    {
        foreach (var solution in demoSolutions)
        {
            Run(msBuild, $"{solution} /p:Configuration=Debug /nologo /m /v:m /nr:false");
        }
    });

Add(
    "exercises",
    DependsOn("find-msbuild", "restore-exercises"),
    () =>
    {
        foreach (var solution in exerciseSolutions)
        {
            Run(msBuild, $"{solution} /p:Configuration=Debug /nologo /m /v:m /nr:false");
        }
    });


Run(Args);
