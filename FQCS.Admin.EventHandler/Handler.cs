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

        protected async Task HandleMessage(ConsumeResult<Null, string> result)
        {
            var mess = result.Message;
            switch (result.Topic)
            {
                case Kafka.Constants.KafkaTopic.TOPIC_QC_EVENT:
                    {
                        var model = JsonConvert.DeserializeObject<QCEventMessage>(mess.Value);
                        Console.WriteLine(mess.Value);
                        using (var scope = provider.CreateScope())
                        {
                            var context = provider.GetRequiredService<DataContext>();
                            var sProvider = scope.ServiceProvider;
                            var qcEventService = sProvider.GetRequiredService<QCEventService>();
                            var defectTypeService = sProvider.GetRequiredService<DefectTypeService>();
                            var proBatchService = sProvider.GetRequiredService<ProductionBatchService>();
                            var qcDeviceService = sProvider.GetRequiredService<QCDeviceService>();
                            var validationResult = qcEventService.ValidateQCMessage(model);
                            if (!validationResult.IsValid)
                            {
                                Console.WriteLine("Invalid QC message");
                                return;
                            }
                            var deviceId = int.Parse(model.Identifier);
                            var device = qcDeviceService.QCDevices.Id(deviceId).Select(o => new QCDevice
                            {
                                Id = o.Id,
                                Code = o.Code,
                                ProductionLineId = o.ProductionLineId
                            }).First();
                            var defectType = defectTypeService.DefectTypes.QCMappingCode(model.QCDefectCode)
                                .Select(o => new DefectType
                                {
                                    Id = o.Id,
                                    Code = o.Code,
                                    Name = o.Name
                                }).First();
                            var proBatchId = proBatchService.ProductionBatchs.InLine(device.ProductionLineId.Value)
                                .RunningAtTime(model.CreatedTime).Select(o => o.Id).First();
                            var entity = new QCEvent
                            {
                                CreatedTime = model.CreatedTime,
                                QCDeviceId = deviceId,
                                DefectTypeId = defectType.Id,
                                Description = $"Defect type at batch: {proBatchId}-{defectType.Name}-{defectType.Code} at {model.CreatedTime}",
                                ProductionBatchId = proBatchId
                            };
                            entity = qcEventService.CreateQCEvent(entity);
                            context.SaveChanges();
                            Console.WriteLine(entity.Description);
                        }
                    }
                    break;
            }
        }

        public Task StartConsuming(CancellationToken cancellation, int retryAfterSecs = 10, int maxTryCount = 5)
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
                        await HandleMessage(result);
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
