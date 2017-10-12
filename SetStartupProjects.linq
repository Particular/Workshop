<Query Kind="Program">
  <NuGetReference>SetStartupProjects</NuGetReference>
  <Namespace>SetStartupProjects</Namespace>
</Query>

void Main()
{
	var currentDiretory = Path.GetDirectoryName(Util.CurrentQueryPath);
	SetStartupProjects(currentDiretory);
}

public static void SetStartupProjects(string codeDirectory)
{
	foreach (var suo in Directory.EnumerateDirectories(codeDirectory, ".vs", SearchOption.AllDirectories))
	{
		Directory.Delete(suo, true);
	}
	foreach (var suo in Directory.EnumerateFiles(codeDirectory, "*.suo", SearchOption.AllDirectories))
	{
		File.Delete(suo);
	}
	var startProjectSuoCreator = new StartProjectSuoCreator();
	foreach (var solutionFile in Directory.EnumerateFiles(codeDirectory, "*.sln", SearchOption.AllDirectories))
	{
		var startProjects = GetStartupProjects(solutionFile).ToList();
		if (startProjects.Any())
		{
			startProjectSuoCreator.CreateForSolutionFile(solutionFile, startProjects, VisualStudioVersions.Vs2017);
		}
	}
}

public static IEnumerable<string> GetStartupProjects(string solutionFile)
{
	var solutionDirectory = Path.Combine(Path.GetDirectoryName(solutionFile));
	var defaultProjectsTextFile = Path.Combine(solutionDirectory, "DefaultStartupProjects.txt");
	if (!File.Exists(defaultProjectsTextFile))
	{
		var startProjectFinder = new StartProjectFinder();
		foreach (var startProject in startProjectFinder.GetStartProjects(solutionFile))
		{
			yield return startProject;
		}
		yield break;
	}
	var allPossibleProjects = SolutionProjectExtractor.GetAllProjectFiles(solutionFile).ToList();
	var defaultProjects = File.ReadAllLines(defaultProjectsTextFile)
		.Where(x => !string.IsNullOrWhiteSpace(x));
	foreach (var startupProject in defaultProjects)
	{
		var project = allPossibleProjects.FirstOrDefault(x => x.RelativePath == startupProject);
		if (project != null)
		{
			yield return project.Guid;
		}
	}
}
