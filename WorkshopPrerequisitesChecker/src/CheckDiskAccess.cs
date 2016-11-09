namespace PrerequisitesInstallerConsole
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal class CheckDiskAccess
    {
        string path = @"c:\temp";

        public Task Execute(Action<string> logOutput, Action<string> logError)
        {
            if (!CheckFolderAccess(logOutput, logError))
            {
                return Task.FromResult(0);
            }

            return Task.FromResult(0);
        }

        bool CheckFolderAccess(Action<string> logOutput, Action<string> logError)
        {

            logOutput($@"Checking access to {path}");

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var randomFile = Path.Combine(path, Path.GetRandomFileName());
                // This will raise an exception if there is no access to create a file
                using (File.Create(randomFile, 1, FileOptions.DeleteOnClose)){}

                logOutput($"Access to {path} OK");

                return true;
            }
            catch (Exception ex)
            {
                logError("Access to c:\\temp was denied. The SQLite database used in the workshop needs write access to c:\\temp to work.");
                logError($"{ex}");
                return false;
            }
        }
    }
}
