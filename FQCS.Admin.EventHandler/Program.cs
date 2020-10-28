using Confluent.Kafka;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;
using FQCS.Admin.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using TNT.Core.Helpers.DI;

namespace FQCS.Admin.EventHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = File.ReadAllText("appsettings.json");
            var settings = JsonConvert.DeserializeObject<Settings>(json);
            var services = new ServiceCollection();
            Data.Global.Init(services);
            Business.Global.Init(services);
            services.AddServiceInjection();
            var connStr = settings.ConnStr;
#if TEST
            connStr = connStr.Replace("{envConfig}", ".Test");
#else
            connStr = connStr.Replace("{envConfig}", "");
#endif
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connStr).UseLazyLoadingProxies());
            var provider = services.BuildServiceProvider();
            var cancelSource = new CancellationTokenSource();
            using var handler = new Handler(settings, provider);
            handler.SubscribeTopic(Kafka.Constants.KafkaTopic.TOPIC_QC_EVENT);
            var task = handler.StartConsuming(cancelSource.Token,
                settings.RetryAfterSecs, settings.MaxTryCount);
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
