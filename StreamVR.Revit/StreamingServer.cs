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

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LMAStudio.StreamVR.Revit.Commands;
using LMAStudio.StreamVR.Revit.Conversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LMAStudio.StreamVR.Common;

namespace LMAStudio.StreamVR.Revit
{
    [Transaction(TransactionMode.Manual)]

    public class StreamingServer : IExternalCommand
    {
        private IGenericConverter Converter;
        private IBaseCommand Command_GetAll;
        private IBaseCommand Command_Get;
        private IBaseCommand Command_Set;
        private IBaseCommand Command_Paint;
        private IBaseCommand Command_Create;

        private static Queue<Message> msgQueue = new Queue<Message>();
        private Application application;

        private void Debug(string msg)
        {
            this.application.WriteJournalComment(msg, true);
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UIdoc = commandData.Application.ActiveUIDocument;
            Document doc = UIdoc.Document;

            this.application = commandData.Application.Application;

            this.Converter = new GenericConverter(Debug);
            this.Command_GetAll = new GetAll(Debug, this.Converter);
            this.Command_Get = new Get(Debug, this.Converter);
            this.Command_Set = new Set(Debug, this.Converter);
            this.Command_Paint = new Paint(Debug, this.Converter);
            this.Command_Create = new Create(Debug, this.Converter);

            this.ListenForMessages(doc, "192.168.0.119:7002");

            return Result.Succeeded;
        }

        private void ListenForMessages(Document doc, string natsUrl)
        {
            using (var cc = new Communicator(natsUrl, this.Debug))
            {
                cc.Connect();
                cc.Subscribe(Communicator.TO_SERVER_CHANNEL, (Message msg) =>
                {
                    msgQueue.Enqueue(msg);
                });

                bool _shutdown = false;
                while (!_shutdown)
                {
                    if (msgQueue.Count > 0)
                    {
                        Message msg = msgQueue.Dequeue();

                        Debug(JsonConvert.SerializeObject(msg));

                        if (msg.Reply != null)
                        {
                            Message response = HandleClientRequest(doc, msg);
                            response.Reply = msg.Reply;
                            cc.Publish(msg.Reply, response);
                        }
                        else if (msg.Type == "EXIT")
                        {
                            Debug("Exit command received");
                            _shutdown = true;
                        }
                    }
                    Task.Delay(200).Wait();
                }
            }
        }

        private Message HandleClientRequest(Document doc, Message msg)
        {
            try
            {
                switch (msg.Type)
                {
                    case "GET_ALL":
                        return this.Command_GetAll.Execute(doc, msg);
                    case "GET":
                        return this.Command_Get.Execute(doc, msg);
                    case "SET":
                        return this.Command_Set.Execute(doc, msg);
                    case "PAINT":
                        return this.Command_Paint.Execute(doc, msg);
                    case "CREATE":
                        return this.Command_Create.Execute(doc, msg);
                }
            }
            catch(Exception e)
            {
                return new Message
                {
                    Type = "ERROR",
                    Data = JsonConvert.SerializeObject(new
                    {
                        Msg = e.Message,
                        Stack = e.StackTrace
                    })
                };
            }

            return new Message
            {
                Type = "NULL",
                Data = null
            };
        }
    }
}