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
            var producer = KafkaHelper.GetPlainProducer(settings.KafkaServer,
                settings.KafkaUsername, settings.KafkaPassword);
            using var handler = new Handler(settings);
            handler.SubscribeTopic(Constants.KafkaTopic.TOPIC_QC_EVENT);
            var task = handler.StartConsuming(cancelSource.Token, settings.RetryAfterSecs);
            Console.WriteLine("Press C to exit");
            while (!cancelSource.IsCancellationRequested)
            {
                var line = Console.ReadLine();
                if (line == "C")
                    cancelSource.Cancel();
                // test only
                else
                    SendTest(settings, producer);
            }
        }

        static void SendTest(Settings settings, IProducer<Null, string> producer)
        {
            var mess = new Message<Null, string>();
            mess.Value = JsonConvert.SerializeObject(new QCEventMessage
            {
                CreatedTime = DateTime.UtcNow,
                QCDefectCode = DateTime.UtcNow.Second.ToString(),
                Identifier = "TestProducer"
            });
            producer.Produce(Constants.KafkaTopic.TOPIC_QC_EVENT, mess);
        }
    }
}
