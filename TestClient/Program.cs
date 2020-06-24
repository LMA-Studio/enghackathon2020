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

        /// <summary>
        /// Attempt to connect to NATS server. Repeat on failure
        /// </summary>
        private static async Task TryStartRepeat()
        {
            ICommunicator comms;

            try
            {
                // Swap out for actual NATS endpoint
                comms = new Communicator("192.168.0.119:7002", Console.WriteLine);
            }
            catch
            {
                Console.WriteLine("Waiting for server...");
                Task.Delay(1000).Wait();
                await TryStartRepeat();
                return;
            }

            // Subscribe to incoming async events
            comms.Subscribe(Communicator.FROM_SERVER_CHANNEL, (Message msg) =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(msg));
            });

            // Run test script
            await Test(comms);

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();

            // Stop Revit addin
            comms.Publish(Communicator.TO_SERVER_CHANNEL, new Message { Type = "EXIT" });
        }

        /// <summary>
        /// Run test script
        /// </summary>
        private static async Task Test(ICommunicator comms)
        {
            Console.WriteLine("Getting all");

            // Issue command to get all elements of a given type
            // E.e. <Autodesk.Revit.DB.Material>
            Message response = await comms.Request(Communicator.TO_SERVER_CHANNEL, new Message
            {
                Type = "GET_ALL",
                Data = JObject.FromObject(new
                {
                    Type = "Autodesk.Revit.DB.Material"
                })
            });

            Console.WriteLine(JsonConvert.SerializeObject(response));

            // Parse response to DTO
            List<Material> dataSet = JArray.FromObject(response.Data).
                                            Select(x => (JObject)x).
                                            Select(x => x.ToObject<Material>()).
                                            ToList();

            // Select a specific item
            Material mat = dataSet.FirstOrDefault(
                d => true // e.g. d["Id"].ToString() == "1389804"
            );

            // Update model
            mat.Color = new Color
            {
                Red = 0xff,
                Green = 0x0,
                Blue = 0x0
            };

            Console.WriteLine("Setting");
            Console.WriteLine(JsonConvert.SerializeObject(mat));

            // Issue command to update a given element
            Message response2 = await comms.Request(Communicator.TO_SERVER_CHANNEL, new Message
            {
                Type = "SET",
                Data = JObject.FromObject(mat)
            });
            Console.WriteLine(JsonConvert.SerializeObject(response2));
        }
    }
}