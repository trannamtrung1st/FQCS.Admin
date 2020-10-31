using Confluent.Kafka;
using DocumentFormat.OpenXml.Drawing.Charts;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FQCS.Admin.ConsoleClient
{
    class Program
    {
        static IProducer<Null, string> producer;
        static Settings settings;
        static Random random = new Random();
        static void Main(string[] args)
        {
            StartAdminConsole();
        }

        static void StartAdminConsole()
        {
            var json = File.ReadAllText("appsettings.json");
            settings = JsonConvert.DeserializeObject<Settings>(json);
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1/ Start Zoo");
                Console.WriteLine("2/ Start Kafka");
                Console.WriteLine("3/ Reset Kafka data");
                Console.WriteLine("4/ Stop Kafka");
                Console.WriteLine("5/ Stop Zoo");
                Console.WriteLine("6/ Produce test QC message");
                Console.WriteLine("Other/ EXIT");
                Console.Write("Your choice: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        {
                            string strCmdText;
                            strCmdText = $"/C start {settings.StartZooCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "2":
                        {
                            string strCmdText;
                            strCmdText = $"/C start {settings.StartKafkaCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "3":
                        {
                            string strCmdText;
                            strCmdText = $"/C start {settings.ResetKafkaCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "4":
                        {
                            if (producer != null)
                                producer.Dispose();
                            string strCmdText;
                            strCmdText = $"/C start {settings.StopKafkaCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "5":
                        {
                            string strCmdText;
                            strCmdText = $"/C start {settings.StopZooCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "6":
                        {
                            if (producer == null)
                                producer = KafkaHelper.GetPlainProducer(settings.KafkaServer,
                                    settings.KafkaUsername, settings.KafkaPassword);
                            SendTest(settings);
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        static void SendTest(Settings settings)
        {
            var mess = new Message<Null, string>();
            var imgPath = Program.settings.TestImage.Replace("{no}", random.Next(156, 165).ToString());
            var img = File.ReadAllBytes(imgPath);
            var leftImgB64 = Convert.ToBase64String(img);
            imgPath = Program.settings.TestImage.Replace("{no}", random.Next(156, 165).ToString());
            img = File.ReadAllBytes(imgPath);
            var rightImgB64 = Convert.ToBase64String(img);
            mess.Value = JsonConvert.SerializeObject(new QCEventMessage
            {
                CreatedTime = DateTime.UtcNow,
                QCDefectCode = settings.TestDefectCode,
                Identifier = settings.TestDeviceId,
                LeftB64Image = leftImgB64,
                RightB64Image = rightImgB64,
            });
            producer.Produce(Kafka.Constants.KafkaTopic.TOPIC_QC_EVENT, mess, rep =>
            {
                Console.WriteLine(rep.Status);
            });
        }
    }


}
