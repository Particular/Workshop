using System.Diagnostics;
using System.Reflection;

namespace WcfService.Handlers
{
    public class TvShowLoggerHandler :
        IHandleMessages<TvShowOfTheYearMessage>,
        IHandleMessages<TvShowReviewMessage>
    {
        public void Handle(TvShowOfTheYearMessage message)
        {
            LogMessage(message);
        }

        public void Handle(TvShowReviewMessage message)
        {
            LogMessage(message);
        }

        private static void LogMessage(IMessage message)
        {
            var typeName = message.GetType();

            Debug.WriteLine($"Typename {typeName} received");
            LogProperties(message);
        }

        private static void LogProperties(IMessage message)
        {
            PropertyInfo[] _PropertyInfos = message.GetType().GetProperties();

            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(message, null) ?? "(null)";
                Debug.WriteLine($"\t{info.Name} : {value.ToString()}");
            }

        }
    }
}