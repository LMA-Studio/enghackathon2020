using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Models;

namespace TestConnector.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TryStartRepeat().Wait();

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        private static async Task TryStartRepeat()
        {
            ICommunicator comms;

            try
            {
                comms = new Communicator("192.168.0.119:7002", Console.WriteLine);
            }
            catch
            {
                Console.WriteLine("Waiting for server...");
                Task.Delay(1000).Wait();
                await TryStartRepeat();
                return;
            }

            comms.Subscribe(Communicator.FROM_SERVER_CHANNEL, (Message msg) =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(msg));
            });


            Console.WriteLine("Getting all");

            Message response = await comms.Request(Communicator.TO_SERVER_CHANNEL, new Message
            {
                Type = "GET_ALL",
                Data = JObject.FromObject(new
                {
                    Type = "Autodesk.Revit.DB.Wall"
                })
            });

            Console.WriteLine(JsonConvert.SerializeObject(response));

            List<JObject> dataSet = JArray.FromObject(response.Data).Select(x => (JObject)x).ToList();

            Material mat = dataSet.FirstOrDefault(d => d["Id"].ToString() == "1389804").ToObject<Material>();
            mat.Color = new Color
            {
                Red = 0xff,
                Green = 0x0,
                Blue = 0x0
            };

            Console.WriteLine("Setting");
            Console.WriteLine(JsonConvert.SerializeObject(mat));

            Message response2 = await comms.Request(Communicator.TO_SERVER_CHANNEL, new Message
            {
                Type = "SET",
                Data = JObject.FromObject(mat)
            });
            Console.WriteLine(JsonConvert.SerializeObject(response2));

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();

            comms.Publish(Communicator.TO_SERVER_CHANNEL, new Message { Type = "EXIT" });
        }
    }
}