using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FQCS.Admin.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            StartAdminConsole();
        }

        static void StartAdminConsole()
        {
            var json = File.ReadAllText("appsettings.json");
            var adminConfig = JsonConvert.DeserializeObject<Settings>(json);
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1/ Start Zoo");
                Console.WriteLine("2/ Start Kafka");
                Console.WriteLine("3/ Reset Kafka data");
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
                    default:
                        return;
                }
            }
        }
    }


}
