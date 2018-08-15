#r "packages/Bullseye.1.1.0-rc.2/lib/netstandard2.0/Bullseye.dll"
#load "scripts/cmd.csx"

using System;
using static Bullseye.Targets;

var vswhere = "packages/vswhere.2.1.4/tools/vswhere.exe";
var nuget = ".nuget/v4.3.0/NuGet.exe";
string msBuild = null;

var demoSolutions = Directory.EnumerateFiles("demos", "*.sln", SearchOption.AllDirectories);
var exerciseSolutions = Directory.EnumerateFiles("exercises", "*.sln", SearchOption.AllDirectories);

Target("default", DependsOn("demos", "exercises"));

Target(
    "restore-demos",
    demoSolutions,
    solution => Cmd(nuget, $"restore {solution}"));

Target(
    "restore-exercises",
    exerciseSolutions,
    solution => Cmd(nuget, $"restore {solution}"));

Target(
    "find-msbuild",
    () => msBuild = $"{ReadCmd(vswhere, "-latest -requires Microsoft.Component.MSBuild -property installationPath").Trim()}/MSBuild/15.0/Bin/MSBuild.exe");

Target(
    "demos",
    DependsOn("find-msbuild", "restore-demos"),
    demoSolutions,
    solution => Cmd(msBuild, $"{solution} /p:Configuration=Debug /nologo /m /v:m /nr:false"));

Target(
    "exercises",
    DependsOn("find-msbuild", "restore-exercises"),
    exerciseSolutions,
    solution => Cmd(msBuild, $"{solution} /p:Configuration=Debug /nologo /m /v:m /nr:false"));


RunTargets(Args);
