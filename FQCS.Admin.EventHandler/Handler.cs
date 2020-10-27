using Confluent.Kafka;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;
using FQCS.Admin.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FQCS.Admin.EventHandler
{
    public class Handler : IDisposable
    {

        protected readonly IConsumer<Null, string> consumer;

        public Handler(Settings settings)
        {
            consumer = KafkaHelper.GetPlainConsumer(settings.KafkaServer,
                settings.GroupId,
                settings.KafkaUsername,
                settings.KafkaPassword);
        }

        public void SubscribeTopic(string topic)
        {
            consumer.Subscribe(topic);
        }

        protected async Task HandleMessage(ConsumeResult<Null, string> result)
        {
            var mess = result.Message;
            switch (result.Topic)
            {
                case Constants.KafkaTopic.TOPIC_QC_EVENT:
                    {
                        var model = JsonConvert.DeserializeObject<QCEventMessage>(mess.Value);
                        Console.WriteLine(mess.Value);
                    }
                    break;
            }
        }

        public Task StartConsuming(CancellationToken cancellation, int retryAfterSecs = 10)
        {
            return Task.Run(async () =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    var result = consumer.Consume(cancellation);
                    try
                    {
                        var reqId = Guid.NewGuid().ToString();
                        Console.WriteLine($"{reqId} | Receive message at {DateTime.UtcNow}");
                        await HandleMessage(result);
                        Console.WriteLine($"{reqId} | Finish handle message");
                        consumer.Commit(result);
                    }
                    catch (Exception e)
                    {
                        consumer.Unassign();
                        Console.WriteLine($"Error: {e}");
                        Thread.Sleep(retryAfterSecs * 1000);
                    }
                }
            }, cancellation);
        }

        public void Dispose()
        {
            consumer.Close();
            consumer.Dispose();
        }
    }
}
