using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;

namespace FQCS.Admin.EventHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = File.ReadAllText("appsettings.json");
            var settings = JsonConvert.DeserializeObject<Settings>(json);
            var cancelSource = new CancellationTokenSource();
            using var handler = new Handler(settings);
            handler.SubscribeTopic(Constants.KafkaTopic.TOPIC_QC_EVENT);
            var task = handler.StartConsuming(cancelSource.Token, settings.RetryAfterSecs);
            Console.WriteLine("Press C to exit");
            while (!cancelSource.IsCancellationRequested)
            {
                var line = Console.ReadLine();
                if (line == "C")
                    cancelSource.Cancel();
            }
        }
    }
}
