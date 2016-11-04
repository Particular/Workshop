namespace PrerequisitesInstallerConsole
{
    using System;
    using System.ComponentModel;
    using System.ServiceProcess;

    public class ProcessUtil
    {
        public void ChangeServiceStatus(ServiceController controller, ServiceControllerStatus status, Action changeStatus)
        {
            if (controller.Status == status)
            {
                return;
            }

            try
            {
                changeStatus();
            }
            catch (Win32Exception exception)
            {
                ThrowUnableToChangeStatus(controller.ServiceName, status, exception);
            }
            catch (InvalidOperationException exception)
            {
                ThrowUnableToChangeStatus(controller.ServiceName, status, exception);
            }

            var timeout = TimeSpan.FromSeconds(10);
            controller.WaitForStatus(status, timeout);
            if (controller.Status == status)
            {
                // WriteVerbose((controller.ServiceName + " status changed successfully."));
            }
            else
            {
                ThrowUnableToChangeStatus(controller.ServiceName, status);
            }
        }

        void ThrowUnableToChangeStatus(string serviceName, ServiceControllerStatus status)
        {
            ThrowUnableToChangeStatus(serviceName, status, null);
        }

        static void ThrowUnableToChangeStatus(string serviceName, ServiceControllerStatus status, Exception exception)
        {
            var message = $"Unable to change {serviceName} status to {Enum.GetName(typeof(ServiceControllerStatus), status)}";

            if (exception == null)
            {
                throw new InvalidOperationException(message);
            }

            throw new InvalidOperationException(message, exception);
        }
    }
}
