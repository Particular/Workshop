namespace PrerequisitesInstallerConsole
{
    using System;
    using System.Threading.Tasks;

    public class NServiceBusPrerequisitesInstaller
    {
        public Task Execute(Action<string> logOutput, Action<string> logError)
        {
            if (!MsmqSetupStep(logOutput, logError))
            {
                return Task.FromResult(0);
            }

            //if (!DtcSetupStep(logOutput, logError))
            //{
            //    return Task.FromResult(0);
            //}

            //if (!PerfCounterSetupStep(logOutput, logError))
            //{
            //    return Task.FromResult(0);
            //}

            return Task.FromResult(0);
        }

        bool PerfCounterSetupStep(Action<string> logOutput, Action<string> logError)
        {
            try
            {
                logOutput("Checking NServiceBus Performance Counters");
                var setup = new PerfCountersInstaller();

                var allCountersExist = setup.CheckCounters();
                if (allCountersExist)
                {
                    logOutput("Performance Counters OK");
                    return true;
                }
                logOutput("Adding NServiceBus Performance Counters");
                if (setup.DoesCategoryExist())
                {
                    setup.DeleteCategory();
                }
                setup.SetupCounters();
            }
            catch (Exception ex)
            {
                logError("NServiceBus Performance Counters install failed:");
                logError($"{ex}");
                return false;
            }
            return true;
        }

        bool DtcSetupStep(Action<string> logOutput, Action<string> logError)
        {
            try
            {
                var dtc = new DtcInstaller(logOutput);
                if (!dtc.IsDtcWorking())
                {
                    dtc.ReconfigureAndRestartDtcIfNecessary();
                }
            }
            catch (Exception ex)
            {
                logError("DTC install has failed:");
                logError($"{ex}");
                return false;
            }
            return true;
        }

        bool MsmqSetupStep(Action<string> logOutput, Action<string> logError)
        {
            try
            {
                logOutput("Checking MSMQ configuration...");
                var msmq = new MsmqInstaller(logOutput);

                if (msmq.IsInstalled())
                {
                    if (msmq.IsInstallationGood())
                    {
                        logOutput("MSMQ configuration OK");
                    }
                    else
                    {
                        logError("MSMQ is already installed but has unsupported options enabled.");
                        logError($"To use NServiceBus please disable any of the following MSMQ components:\r\n {string.Join(",\r\n", msmq.UnsupportedComponents())} \r\n");
                        return false;
                    }
                }
                else
                {
                    logOutput("MSMQ is not present, will be installed.");
                    msmq.InstallMsmq();
                }

                if (!msmq.StartMsmqIfNecessary())
                {
                    logError("MSMQ Service did not start");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logError("MSMQ install has failed:");
                logError($"{ex}");
                return false;
            }
            return true;
        }
    }
}