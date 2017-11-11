#load "packages/simple-targets-csx.6.0.0/contentFiles/csx/any/simple-targets.csx"
#load "scripts/cmd.csx"

using System;
using static SimpleTargets;

var vswhere = "packages/vswhere.2.1.4/tools/vswhere.exe";
var nuget = ".nuget/v4.3.0/NuGet.exe";
string msBuild = null;

var demoSolutions = Directory.EnumerateFiles("demos", "*.sln", SearchOption.AllDirectories);
var exerciseSolutions = Directory.EnumerateFiles("exercises", "*.sln", SearchOption.AllDirectories);

var targets = new TargetDictionary();

targets.Add("default", DependsOn("demos", "exercises"));

targets.Add(
    "restore-demos",
    () =>
    {
        foreach (var solution in demoSolutions)
        {
            Cmd(nuget, $"restore {solution}");
        }
    });

targets.Add(
    "restore-exercises",
    () =>
    {
        foreach (var solution in exerciseSolutions)
        {
            Cmd(nuget, $"restore {solution}");
        }
    });

targets.Add(
    "find-msbuild",
    () => msBuild = $"{ReadCmd(vswhere, "-latest -requires Microsoft.Component.MSBuild -property installationPath").Trim()}/MSBuild/15.0/Bin/MSBuild.exe");

targets.Add(
    "demos",
    DependsOn("find-msbuild", "restore-demos"),
    () =>
    {
        foreach (var solution in demoSolutions)
        {
            Cmd(msBuild, $"{solution} /p:Configuration=Debug /nologo /m /v:m /nr:false");
        }
    });

targets.Add(
    "exercises",
    DependsOn("find-msbuild", "restore-exercises"),
    () =>
    {
        foreach (var solution in exerciseSolutions)
        {
            Cmd(msBuild, $"{solution} /p:Configuration=Debug /nologo /m /v:m /nr:false");
        }
    });

Run(Args, targets);
