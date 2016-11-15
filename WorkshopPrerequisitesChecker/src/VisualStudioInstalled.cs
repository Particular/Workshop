namespace PrerequisitesInstallerConsole
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal class VisualStudioInstalled
    {
        public Task Execute(Action<string> logOutput, Action<string> logError)
        {
            if (!CheckVisualStudioInstallation(logOutput, logError))
            {
                return Task.FromResult(0);
            }

            return Task.FromResult(0);
        }

        bool CheckVisualStudioInstallation(Action<string> logOutput, Action<string> logError)
        {

            logOutput("Checking to see if you have Visual Studio 2015 installed");

            if (Directory.Exists("C:\\Program Files (x86)\\Microsoft Visual Studio 14.0")
                || Directory.Exists("C:\\Program Files (x86)\\Microsoft Visual Studio 15.0"))
            {
                logOutput($"Visual Studio 2015+ installed OK");
                return true;
            }

            logError("Visual Studio 2015 is not installed(at least not in the default location). You need at least Visual Studio 2015 to do the workshop exercises.");
            return false;
        }
    }
}
