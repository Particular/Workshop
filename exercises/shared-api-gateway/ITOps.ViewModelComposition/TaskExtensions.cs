namespace ITOps.ViewModelComposition
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    static class TaskExtensions
    {
        public static async Task WithLogging(this Task task, Func<ILogger> createLogger)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                createLogger().LogError(ex.ToString());
                throw;
            }
        }
    }
}
