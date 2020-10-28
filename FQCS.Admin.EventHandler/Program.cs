using Confluent.Kafka;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Kafka;
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
            Handle(settings);
        }

        static void Handle(Settings settings)
        {
            var cancelSource = new CancellationTokenSource();
            using var handler = new Handler(settings);
            handler.SubscribeTopic(Kafka.Constants.KafkaTopic.TOPIC_QC_EVENT);
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
