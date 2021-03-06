﻿using Confluent.Kafka;
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
                            var qcEventService = sProvider.GetRequiredService<IQCEventService>();
                            var fileService = sProvider.GetRequiredService<IFileService>();
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
            var qcEventService = sProvider.GetRequiredService<IQCEventService>();
            var qcDeviceService = sProvider.GetRequiredService<IQCDeviceService>();
            var fileService = sProvider.GetRequiredService<IFileService>();
            var deviceCode = model.Identifier;
            var device = qcDeviceService.QCDevices.Code(deviceCode).Select(o => new QCDevice
            {
                Id = o.Id,
                Code = o.Code,
                ProductionLineId = o.ProductionLineId
            }).First();

            var entity = qcEventService.ConvertToQCEvent(model, device);
            var imagesB64 = new List<(byte[], string)>();
            if (model.LeftB64Image != null && model.RightB64Image != null)
            {
                var leftImg = Convert.FromBase64String(model.LeftB64Image);
                var rightImg = Convert.FromBase64String(model.RightB64Image);
                var (leftDir, leftFile) = (Path.GetDirectoryName(model.LeftImage), Path.GetFileName(model.LeftImage));
                var (rightDir, rightFile) = (Path.GetDirectoryName(model.RightImage), Path.GetFileName(model.RightImage));
                var (leftRel, lFull) = fileService.GetFilePath(Path.Combine(savePath, leftDir), savePath, leftFile, ext: ".jpg");
                var (rightRel, rFull) = fileService.GetFilePath(Path.Combine(savePath, rightDir), savePath, rightFile, ext: ".jpg");
                entity.LeftImage = model.LeftImage;
                entity.RightImage = model.RightImage;
                imagesB64.Add((leftImg, lFull));
                imagesB64.Add((rightImg, rFull));
            }
            if (model.SideB64Images != null)
            {
                for (var i = 0; i < model.SideB64Images.Count; i++)
                {
                    var b64 = model.SideB64Images[i];
                    var imgPath = model.SideImages[i];
                    var img = Convert.FromBase64String(b64);
                    var (dir, file) = (Path.GetDirectoryName(imgPath), Path.GetFileName(imgPath));
                    var (rel, full) = fileService.GetFilePath(Path.Combine(savePath, dir), savePath, file, ext: ".jpg");
                    imagesB64.Add((img, full));
                }
                entity.SideImages = JsonConvert.SerializeObject(model.SideImages);
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
