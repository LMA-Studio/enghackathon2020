/*
    This file is part of LMAStudio.StreamVR
    Copyright(C) 2020  Andreas Brake, Lisa-Marie Mueller

    LMAStudio.StreamVR is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMAStudio.StreamVR.Common;
using LMAStudio.StreamVR.Common.Models;

namespace LMAStudio.StreamVR.TestClient
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
                comms.Connect();
            }
            catch
            {
                Console.WriteLine("Waiting for server...");
                Task.Delay(1000).Wait();
                await TryStartRepeat();
                return;
            }

            // Subscribe to incoming async events
            //comms.Subscribe(Communicator.FROM_SERVER_CHANNEL, (Message msg) =>
            //{
            //    Console.WriteLine(JsonConvert.SerializeObject(msg));
            //});

            //// Run test script
            //await Test(comms);

            //Console.WriteLine("Press any key to continue...");
            //Console.ReadLine();

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
                Data = JsonConvert.SerializeObject(new
                {
                    Type = "Autodesk.Revit.DB.Material"
                })
            });

            Console.WriteLine(JsonConvert.SerializeObject(response));

            // Parse response to DTO
            List<Material> dataSet = JArray.Parse(response.Data).
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
                Data = JsonConvert.SerializeObject(mat)
            });
            Console.WriteLine(JsonConvert.SerializeObject(response2));
        }
    }
}