using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Divergent.Finance.Config;

namespace Divergent.Finance
{
    internal class ProgramService : ServiceBase
    {
        private const string runAsServiceArg = "--run-as-service";

        private IEndpointInstance endpoint;

        private static ILog logger;

        static ProgramService()
        {
            logger = LogManager.GetLogger<ProgramService>();
        }

        public static void Main(string[] args)
        {
            using (var service = new ProgramService())
            {
                // pass argument at command line to run as a windows service
                if (args.Contains(runAsServiceArg))
                {
                    Run(service);
                    return;
                }

                Console.Title = "Divergent.Finance";
                Console.CancelKeyPress += (sender, e) => { service.OnStop(); };
                service.OnStart(null);
                Console.WriteLine("\r\nPress enter key to stop program\r\n");
                Console.Read();
                service.OnStop();
            }
        }

        protected override void OnStart(string[] args)
        {
            AsyncOnStart().GetAwaiter().GetResult();
        }

        async Task AsyncOnStart()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration("Divergent.Finance");

                logger.Info("Customize...");

                EndpointConfig.Customize(endpointConfiguration);

                endpoint = await Endpoint.Start(endpointConfiguration)
                    .ConfigureAwait(false);

                PerformStartupOperations();
            }
            catch (Exception exception)
            {
                Exit("Failed to start", exception);
            }
        }

        void Exit(string failedToStart, Exception exception)
        {
            logger.Fatal(failedToStart, exception);
            Environment.FailFast(failedToStart, exception);
        }

        void PerformStartupOperations()
        {
            //TODO: perform any startup operations
        }

        Task OnCriticalError(ICriticalErrorContext context)
        {
            var fatalMessage = $"The following critical error was encountered:\n{context.Error}\nProcess is shutting down.";
            Exit(fatalMessage, context.Exception);
            return Task.FromResult(0);
        }

        protected override void OnStop()
        {
            endpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}
