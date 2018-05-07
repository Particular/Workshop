#r "packages/SetStartupProjects.1.4.0/lib/net452/SetStartupProjects.dll"

#load "packages/simple-targets-csx.6.0.0/contentFiles/csx/any/simple-targets.csx"
#load "scripts/cmd.csx"
#load "scripts/start.csx"

using System;
using static SimpleTargets;
using SetStartupProjects;

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

targets.Add(
    "delete-vs-folders",
    () =>
    {
        foreach (var suo in Directory.EnumerateDirectories(".", ".vs", SearchOption.AllDirectories))
        {
            Directory.Delete(suo, true);
        }
    }
);

targets.Add(
    "set-startup-projects",
    DependsOn("delete-vs-folders"),
    () =>
    {
        var suoCreator = new StartProjectSuoCreator();
        foreach (var sln in Directory.EnumerateFiles(".", "*.sln", SearchOption.AllDirectories))
        {
            var startProjects = new StartProjectFinder().GetStartProjects(sln).ToList();
            if (startProjects.Any())
            {
                suoCreator.CreateForSolutionFile(sln, startProjects, VisualStudioVersions.Vs2017);
            }
        }
    }
);

targets.Add(
    "run-gateway-demo",
    DependsOn("demos"),
    () =>
    {
        Start(@".\demos\asp-net-core\Divergent.Sales.API.Host\bin\debug\Divergent.Sales.API.Host.exe", "");
        Start(@".\demos\asp-net-core\Divergent.Shipping.API.Host\bin\debug\Divergent.Shipping.API.Host.exe", "");
        Start(@"dotnet", @"run --project demos/asp-net-core/Divergent.CompositionGateway/Divergent.CompositionGateway.csproj");
    });

targets.Add(
    "run-website-demo",
    DependsOn("demos"),
    () =>
    {
        Start(@".\demos\asp-net-core\Divergent.Sales.API.Host\bin\debug\Divergent.Sales.API.Host.exe", "");
        Start(@".\demos\asp-net-core\Divergent.Shipping.API.Host\bin\debug\Divergent.Shipping.API.Host.exe", "");
        Start(@"dotnet", @"run --project demos/asp-net-core/Divergent.Website/Divergent.Website.csproj");
        Start("http://localhost:11493", "");
    });

Run(Args, targets);
