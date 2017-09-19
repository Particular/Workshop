#load "packages/simple-targets-csx.5.2.1/simple-targets.csx"
#load "scripts/cmd.csx"

using System;
using static SimpleTargets;

var vswhere = "packages/vswhere.2.1.4/tools/vswhere.exe";
var nuget = ".nuget/v4.3.0/NuGet.exe";
string msBuild = null;

var solutions = Directory.EnumerateFiles(".", "*.sln", SearchOption.AllDirectories);

var targets = new TargetDictionary();

targets.Add("default", DependsOn("build"));

targets.Add(
    "restore",
    () =>
    {
        foreach (var solution in solutions)
        {
            Cmd(nuget, $"restore {solution}");
        }
    });

targets.Add(
    "find-msbuild",
    () => msBuild = $"{ReadCmd(vswhere, "-latest -requires Microsoft.Component.MSBuild -property installationPath").Trim()}/MSBuild/15.0/Bin/MSBuild.exe");

targets.Add(
    "build",
    DependsOn("find-msbuild", "restore"),
    () =>
    {
        foreach (var solution in solutions)
        {
            Cmd(msBuild, $"{solution} /p:Configuration=Debug /nologo /m /v:m /nr:false");
        }
    });

Run(Args, targets);
