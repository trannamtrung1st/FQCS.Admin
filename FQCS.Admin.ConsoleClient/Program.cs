﻿using Confluent.Kafka;
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
        static Settings adminConfig;
        static void Main(string[] args)
        {
            StartAdminConsole();
        }

        static void StartAdminConsole()
        {
            var json = File.ReadAllText("appsettings.json");
            adminConfig = JsonConvert.DeserializeObject<Settings>(json);
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
                            strCmdText = $"/C start {adminConfig.StartZooCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "2":
                        {
                            string strCmdText;
                            strCmdText = $"/C start {adminConfig.StartKafkaCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "3":
                        {
                            string strCmdText;
                            strCmdText = $"/C start {adminConfig.ResetKafkaCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "4":
                        {
                            if (producer != null)
                                producer.Dispose();
                            string strCmdText;
                            strCmdText = $"/C start {adminConfig.StopKafkaCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "5":
                        {
                            string strCmdText;
                            strCmdText = $"/C start {adminConfig.StopZooCmd}";
                            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        }
                        break;
                    case "6":
                        {
                            if (producer == null)
                                producer = KafkaHelper.GetPlainProducer(adminConfig.KafkaServer,
                                    adminConfig.KafkaUsername, adminConfig.KafkaPassword);
                            SendTest();
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        static void SendTest()
        {
            var mess = new Message<Null, string>();
            var testImg = File.ReadAllBytes(adminConfig.TestImage);
            var testImgB64 = Convert.ToBase64String(testImg);
            mess.Value = JsonConvert.SerializeObject(new QCEventMessage
            {
                CreatedTime = DateTime.UtcNow,
                QCDefectCode = "D1u",
                Identifier = "1",
                LeftB64Image = testImgB64,
                RightB64Image = testImgB64,
            });
            producer.Produce(Kafka.Constants.KafkaTopic.TOPIC_QC_EVENT, mess, rep =>
            {
                Console.WriteLine(rep.Status);
            });
        }
    }


}
