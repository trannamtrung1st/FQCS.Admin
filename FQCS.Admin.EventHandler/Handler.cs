using Confluent.Kafka;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Business.Queries;
using FQCS.Admin.Business.Services;
using FQCS.Admin.Data.Models;
using FQCS.Admin.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FQCS.Admin.EventHandler
{
    public class Handler : IDisposable
    {

        protected readonly IConsumer<Null, string> consumer;
        protected readonly IServiceProvider provider;

        public Handler(Settings settings, IServiceProvider provider)
        {
            this.provider = provider;
            this.consumer = KafkaHelper.GetPlainConsumer(settings.KafkaServer,
                settings.GroupId,
                settings.KafkaUsername,
                settings.KafkaPassword);
        }

        public void SubscribeTopic(string topic)
        {
            consumer.Subscribe(topic);
        }

        protected async Task HandleMessage(ConsumeResult<Null, string> result, string savePath)
        {
            var mess = result.Message;
            switch (result.Topic)
            {
                case Kafka.Constants.KafkaTopic.TOPIC_QC_EVENT:
                    {
                        var model = JsonConvert.DeserializeObject<QCEventMessage>(mess.Value);
                        using (var scope = provider.CreateScope())
                        {
                            var sProvider = scope.ServiceProvider;
                            var context = sProvider.GetRequiredService<DataContext>();
                            var qcEventService = sProvider.GetRequiredService<QCEventService>();
                            var fileService = sProvider.GetRequiredService<FileService>();
                            var validationResult = qcEventService.ValidateQCMessage(model);
                            if (!validationResult.IsValid)
                            {
                                Console.WriteLine(string.Join('\n',
                                    validationResult.Results.Select(o => o.Message)));
                                return;
                            }
                            var (entity, imagesB64) =
                                ProcessQCMessage(sProvider, model, savePath);
                            var tasks = imagesB64.Select(async (img) =>
                                await fileService.SaveFile(img.Item1, img.Item2));
                            await Task.WhenAll(tasks);
                            entity = qcEventService.CreateQCEvent(entity);
                            context.SaveChanges();
                            Console.WriteLine(entity.Description);
                        }
                    }
                    break;
            }
        }

        protected (QCEvent, List<(byte[], string)>) ProcessQCMessage(IServiceProvider sProvider,
            QCEventMessage model, string savePath)
        {
            var qcEventService = sProvider.GetRequiredService<QCEventService>();
            var qcDeviceService = sProvider.GetRequiredService<QCDeviceService>();
            var fileService = sProvider.GetRequiredService<FileService>();
            var deviceCode = model.Identifier;
            var device = qcDeviceService.QCDevices.Code(deviceCode).Select(o => new QCDevice
            {
                Id = o.Id,
                Code = o.Code,
                ProductionLineId = o.ProductionLineId
            }).First();

            var entity = qcEventService.ConvertToQCEvent(model, device);
            var dateStr = model.CreatedTime.Date.ToString("yyyyMMdd");
            var folderPath = Path.Combine(savePath, dateStr, model.Id);
            var imagesB64 = new List<(byte[], string)>();
            if (model.LeftB64Image != null && model.RightB64Image != null)
            {
                var leftImg = Convert.FromBase64String(model.LeftB64Image);
                var rightImg = Convert.FromBase64String(model.RightB64Image);
                var (leftRel, lFull) = fileService.GetFilePath(folderPath, savePath, ext: ".jpg");
                var (rightRel, rFull) = fileService.GetFilePath(folderPath, savePath, ext: ".jpg");
                entity.LeftImage = leftRel;
                entity.RightImage = rightRel;
                imagesB64.Add((leftImg, lFull));
                imagesB64.Add((rightImg, rFull));
            }
            if (model.SideB64Images != null)
            {
                var sideImages = new List<string>();
                foreach (var b64 in model.SideB64Images)
                {
                    var img = Convert.FromBase64String(b64);
                    var (rel, full) = fileService.GetFilePath(folderPath, savePath, ext: ".jpg");
                    sideImages.Add(rel);
                    imagesB64.Add((img, full));
                }
                entity.SideImages = JsonConvert.SerializeObject(sideImages);
            }
            return (entity, imagesB64);
        }

        public Task StartConsuming(CancellationToken cancellation,
            string savePath,
            int retryAfterSecs = 10, int maxTryCount = 5)
        {
            return Task.Run(async () =>
            {
                var tryCount = 0;
                while (!cancellation.IsCancellationRequested)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    var result = consumer.Consume(cancellation);
                    try
                    {
                        var reqId = Guid.NewGuid().ToString();
                        Console.WriteLine($"{reqId} | Receive message at {DateTime.UtcNow}");
                        await HandleMessage(result, savePath);
                        Console.WriteLine($"{reqId} | Finish handle message");
                        consumer.Commit(result);
                    }
                    catch (Exception e)
                    {
                        if (++tryCount == maxTryCount)
                        {
                            tryCount = 0;
                            Console.WriteLine(e);
                            Console.WriteLine("-------------------------------------");
                            Console.WriteLine($"Fail to handle message");
                            Console.WriteLine("-------------------------------------");
                            consumer.Commit(result);
                        }
                        else
                        {
                            consumer.Unassign();
                            Console.WriteLine(e);
                            Console.WriteLine("-------------------------------------");
                            Console.WriteLine("Waiting for retry");
                            Console.WriteLine("-------------------------------------");
                            Thread.Sleep(retryAfterSecs * 1000);
                        }
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
