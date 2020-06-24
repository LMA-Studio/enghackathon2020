using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevitGateway.Commands;
using RevitGateway.Conversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Utility;

namespace RevitGateway
{
    [Transaction(TransactionMode.Manual)]

    public class StreamingServer : IExternalCommand
    {
        private IGenericConverter Converter;
        private IBaseCommand Command_GetAll;
        private IBaseCommand Command_Set;

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
            this.Command_Set = new Set(Debug, this.Converter);

            this.ListenForMessages(doc, "192.168.0.119:7002");

            return Result.Succeeded;
        }

        private void ListenForMessages(Document doc, string natsUrl)
        {
            using (var cc = new Communicator(natsUrl, this.Debug))
            {
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
                    case "SET":
                        return this.Command_Set.Execute(doc, msg);
                }
            }
            catch(Exception e)
            {
                return new Message
                {
                    Type = "ERROR",
                    Data = JObject.FromObject(new
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