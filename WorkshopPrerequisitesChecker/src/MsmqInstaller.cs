namespace PrerequisitesInstallerConsole
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.ServiceProcess;
    using Microsoft.Win32;

    public class MsmqInstaller
    {
        Action<string> output;

        public MsmqInstaller(Action<string> output)
        {
            this.output = output;
            output("Checking Microsoft Message Queue Service (MSMQ)");
        }

        public void InstallMsmq()
        {
            var os = GetOperatingSystem();
            switch (os)
            {
                case OperatingSystemEnum.XpOrServer2003:
                    InstallMsmqOnXpOrServer2003();
                    break;

                case OperatingSystemEnum.Vista:
                    RunExe(OcSetup, OcSetupVistaInstallCommand);
                    break;

                case OperatingSystemEnum.Server2008:
                    RunExe(OcSetup, OcSetupInstallCommand);
                    break;

                case OperatingSystemEnum.Windows7:
                    RunExe(dismPath, "/Online /NoRestart /English /Enable-Feature /FeatureName:MSMQ-Container /FeatureName:MSMQ-Server");
                    break;
                case OperatingSystemEnum.Windows8:
                case OperatingSystemEnum.Server2012:
                case OperatingSystemEnum.Windows10:
                    RunExe(dismPath, "/Online /NoRestart /English /Enable-Feature /all /FeatureName:MSMQ-Server");
                    break;
                default:
                    throw new Exception("Unsupported Operating System");
            }
        }

        public bool StartMsmqIfNecessary()
        {
            var processUtil = new ProcessUtil();
            try
            {
                using (var controller = new ServiceController("MSMQ"))
                {
                    if (IsStopped(controller))
                    {
                        output("Starting MSMQ Service");
                        processUtil.ChangeServiceStatus(controller, ServiceControllerStatus.Running, controller.Start);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return true;
        }

        public bool IsInstallationGood()
        {
            const string msmqSetupKeyPath = @"SOFTWARE\Microsoft\MSMQ\Setup";
            var regView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Default;
            using (var localMachineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regView))
            {
                var msmqSetupKey = localMachineKey.OpenSubKey(msmqSetupKeyPath);
                return msmqSetupKey != null && HasOnlyNeededComponents(msmqSetupKey.GetValueNames());
            }
        }

        static bool IsStopped(ServiceController controller)
        {
            return controller.Status == ServiceControllerStatus.Stopped || controller.Status == ServiceControllerStatus.StopPending;
        }

        public bool IsInstalled()
        {
            var dll = LoadLibraryW("Mqrt.dll");
            return dll != IntPtr.Zero;
        }

        void RunExe(string filename, string args)
        {
            output($"Running {filename} {args}");
            var startInfo = new ProcessStartInfo(filename, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Path.GetTempPath()
            };

            var ptr = new IntPtr();
            var fileSystemRedirectionDisabled = false;

            if (Environment.Is64BitOperatingSystem)
            {
                fileSystemRedirectionDisabled = Wow64DisableWow64FsRedirection(ref ptr);
            }

            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
            }
            finally
            {
                if (fileSystemRedirectionDisabled)
                {
                    Wow64RevertWow64FsRedirection(ptr);
                }
            }
        }

        void InstallMsmqOnXpOrServer2003()
        {
            var p = Path.GetTempFileName();
            using (var sw = File.CreateText(p))
            {
                sw.WriteLine("[Version]");
                sw.WriteLine("Signature = \"$Windows NT$\"");
                sw.WriteLine();
                sw.WriteLine("[Global]");
                sw.WriteLine("FreshMode = Custom");
                sw.WriteLine("MaintenanceMode = RemoveAll");
                sw.WriteLine("UpgradeMode = UpgradeOnly");
                sw.WriteLine();
                sw.WriteLine("[Components]");

                foreach (var s in RequiredMsmqComponentsXp)
                    sw.WriteLine(s + " = ON");

                foreach (var s in UndesirableMsmqComponentsXp)
                    sw.WriteLine(s + " = OFF");

                sw.Flush();
            }
            RunExe("sysocmgr", "/i:sysoc.inf /x /q /w /u:%temp%\\" + Path.GetFileName(p));
        }

        // Based on http://msdn.microsoft.com/en-us/library/windows/desktop/ms724833.aspx
        static OperatingSystemEnum GetOperatingSystem()
        {
            var osVersionInfoEx = new OSVersionInfoEx
            {
                OSVersionInfoSize = (uint)Marshal.SizeOf(typeof(OSVersionInfoEx))
            };

            GetVersionEx(osVersionInfoEx);

            switch (Environment.OSVersion.Version.Major)
            {
                case 10:
                    return OperatingSystemEnum.Windows10;

                case 6:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0:
                            if (osVersionInfoEx.ProductType == VER_NT_WORKSTATION)
                            {
                                return OperatingSystemEnum.Vista;
                            }
                            return OperatingSystemEnum.Server2008;

                        case 1:
                            if (osVersionInfoEx.ProductType == VER_NT_WORKSTATION)
                            {
                                return OperatingSystemEnum.Windows7;
                            }

                            return OperatingSystemEnum.Server2008;

                        case 2:
                        case 3:
                            if (osVersionInfoEx.ProductType == VER_NT_WORKSTATION)
                            {
                                return OperatingSystemEnum.Windows8;
                            }
                            return OperatingSystemEnum.Server2012;
                    }
                    break;

                case 5:
                    return OperatingSystemEnum.XpOrServer2003;
            }

            return OperatingSystemEnum.DontCare;
        }

        bool HasOnlyNeededComponents(IEnumerable<string> installedComponents)
        {
            var needed = new List<string>(RequiredMsmqComponentsXp);

            foreach (var i in installedComponents)
            {
                if (UndesirableMsmqComponentsXp.Contains(i))
                {
                    return false;
                }

                if (UndesirableMsmqComponentsV4.Contains(i))
                {
                    return false;
                }
                needed.Remove(i);
            }

            if (needed.Count == 0)
                return true;

            return false;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW")]
        static extern IntPtr LoadLibraryW([In] [MarshalAs(UnmanagedType.LPWStr)] string lpLibFileName);

        [DllImport("Kernel32", CharSet = CharSet.Auto)]
        static extern bool GetVersionEx([Out] [In] OSVersionInfo versionInformation);


        // ReSharper disable UnusedField.Compiler
        // ReSharper disable NotAccessedField.Local
        // ReSharper disable UnassignedField.Compiler
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        class OSVersionInfoEx : OSVersionInfo
        {
            public ushort ServicePackMajor;
            public ushort ServicePackMinor;
            public ushort SuiteMask;
            public byte ProductType;
            public byte Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        class OSVersionInfo
        {
            // ReSharper disable once NotAccessedField.Global
            public uint OSVersionInfoSize =
                (uint)Marshal.SizeOf(typeof(OSVersionInfo));

            public uint MajorVersion = 0;
            public uint MinorVersion = 0;
            public uint BuildNumber = 0;
            public uint PlatformId = 0;
            // Attribute used to indicate marshalling for String field
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string CSDVersion = null;
        }

        // ReSharper restore UnusedField.Compiler
        // ReSharper restore NotAccessedField.Local
        // ReSharper restore UnassignedField.Compiler
        const byte VER_NT_WORKSTATION = 1;

        static List<string> RequiredMsmqComponentsXp = new List<string>(new[]
        {
            "msmq_Core",
            "msmq_LocalStorage"
        });

        static List<string> UndesirableMsmqComponentsXp = new List<string>(new[]
        {
            "msmq_ADIntegrated",
            "msmq_TriggersService",
            "msmq_HTTPSupport",
            "msmq_RoutingSupport",
            "msmq_MQDSService"
        });

        static List<string> UndesirableMsmqComponentsV4 = new List<string>(new[]
        {
            "msmq_DCOMProxy",
            "msmq_MQDSServiceInstalled",
            "msmq_MulticastInstalled",
            "msmq_RoutingInstalled",
            "msmq_TriggersInstalled"
        });

        enum OperatingSystemEnum
        {
            DontCare,
            XpOrServer2003,
            Vista,
            Server2008,
            Windows7,
            Windows8,
            Server2012,
            Windows10
        }

        const string OcSetup = "OCSETUP";
        static string dismPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "dism.exe");
        const string OcSetupInstallCommand = "MSMQ-Server /passive";
        const string OcSetupVistaInstallCommand = "MSMQ-Container;MSMQ-Server /passive";

        public string[] UnsupportedComponents()
        {
            var os = GetOperatingSystem();
            switch (os)
            {
                case OperatingSystemEnum.XpOrServer2003:
                case OperatingSystemEnum.Vista:
                case OperatingSystemEnum.Server2008:
                    return UndesirableMsmqComponentsXp.ToArray();
                default:
                    return UndesirableMsmqComponentsV4.ToArray();
            }
        }
    }
}