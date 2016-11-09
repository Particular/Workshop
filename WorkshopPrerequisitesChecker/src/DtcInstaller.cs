namespace PrerequisitesInstallerConsole
{
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using Microsoft.Win32;

    public class DtcInstaller
    {
        Action<string> output;

        public DtcInstaller(Action<string> output)
        {
            this.output = output;
            output("Checking Distributed Transaction Coordinator Configuration");
        }

        public void ReconfigureAndRestartDtcIfNecessary()
        {
            var processUtil = new ProcessUtil();

            if (DoesSecurityConfigurationRequireRestart(true))
            {
                output("Stopping DTC service");
                processUtil.ChangeServiceStatus(Controller, ServiceControllerStatus.Stopped, Controller.Stop);
            }
            output("Starting DTC service");
            processUtil.ChangeServiceStatus(Controller, ServiceControllerStatus.Running, Controller.Start);
        }

        public bool IsDtcWorking()
        {
            if (DoesSecurityConfigurationRequireRestart(false))
            {
                output("DTC requires a restart");
                return false;
            }

            if (Controller.Status != ServiceControllerStatus.Running)
            {
                output("DTC service is not running");
                return false;
            }
            output("DTC configuration OK");
            return true;
        }

        bool DoesSecurityConfigurationRequireRestart(bool doChanges)
        {
            var regview = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Default;
            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regview))
            {
                const string keyName = @"SOFTWARE\Microsoft\MSDTC\Security";
                var requireRestart = false;
                using (var dtcKey = localMachine.OpenSubKey(keyName, true))
                {
                    if (dtcKey == null)
                    {
                        throw new Exception($@"Registry key noy found: HKEY_LOCAL_MACHINE\{keyName}");
                    }
                    foreach (var val in RegValues)
                    {
                        if ((int)dtcKey.GetValue(val, 0) != 0)
                        {
                            continue;
                        }

                        if (doChanges)
                        {
                            output($"Setting value '{val}' to '{1}' in '{keyName}'");
                            dtcKey.SetValue(val, 1, RegistryValueKind.DWord);
                        }
                        requireRestart = true;
                    }
                }
                return requireRestart;
            }
        }

        static ServiceController Controller = new ServiceController { ServiceName = "MSDTC", MachineName = "." };
        static List<string> RegValues = new List<string>(new[] { "NetworkDtcAccess", "NetworkDtcAccessOutbound", "NetworkDtcAccessTransactions", "XaTransactions" });
    }
}
