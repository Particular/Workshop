namespace PrerequisitesInstallerConsole
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    class Program
    {
        static List<string> _errors = new List<string>();

        static StringBuilder _installerOutput = new StringBuilder();

        static void Main(string[] args)
        {
            Console.Title = "Workshop Prerequisites Checker";

            ExecuteInstallers();

            ExecuteCheckDisk();

            ExecuteVisualStudioInstalled();

            Console.WriteLine(_installerOutput);

            if (_errors.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Errors: {0}", string.Join(System.Environment.NewLine, _errors));
                Console.ResetColor();
            }
            else {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Your machine is ready for the workshop. Have fun!");
                Console.ResetColor();
            }

            Console.Read();
        }

        internal static async void ExecuteInstallers()
        {
            NServiceBusPrerequisitesInstaller installer = new NServiceBusPrerequisitesInstaller();

            await installer.Execute(AddOutput, AddError).ConfigureAwait(false);
        }

        internal static async void ExecuteCheckDisk()
        {
            CheckDiskAccess installer = new CheckDiskAccess();

            await installer.Execute(AddOutput, AddError).ConfigureAwait(false);
        }

        internal static async void ExecuteVisualStudioInstalled()
        {
            VisualStudioInstalled installer = new VisualStudioInstalled();

            await installer.Execute(AddOutput, AddError).ConfigureAwait(false);
        }

        static void AddOutput(string output)
        {
            _installerOutput.AppendLine(output);
        }

        static void AddError(string error)
        {
            _errors.Add(error);
        }
    }
}
