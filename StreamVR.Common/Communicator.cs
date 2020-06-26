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

using System;
using System.Threading.Tasks;
using NATS.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LMAStudio.StreamVR.Common
{
    public interface ICommunicator : IDisposable
    {
        void Connect();
        void Publish(string channel, Message msg);
        Task<Message> Request(string channel, Message msg, int timeout = 1000);
        Message RequestSync(string channel, Message msg, int timeout = 1000);
        void Subscribe(string channel, Action<Message> handler);
    }

    public class Message
    {
        public string Type { get; set; }
        public string Reply { get; set; }
        public string Data { get; set; }
    }

    public class Communicator : ICommunicator
    {
        public const string TO_SERVER_CHANNEL = "TO_SERVER";
        public const string FROM_SERVER_CHANNEL = "FROM_SERVER";

        private Options opts;
        private IConnection connection;

        private Action<string> logger;

        public Communicator(string url, Action<string> logger)
        {
            opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = url;
            opts.Timeout = 1000;

            this.logger = logger;
        }

        public void Connect()
        {
            connection = new ConnectionFactory().CreateConnection(opts);
        }

        #region publish/requests

        public void Publish(string channel, Message msg)
        {
            try
            {
                if (msg.Reply == null)
                {
                    connection.Publish(
                        channel,
                        System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg))
                    );
                }
                else
                {
                    connection.Publish(
                        channel,
                        msg.Reply,
                        System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg))
                    );
                }
            }
            catch (Exception e)
            {
                this.logger(e.ToString());
                throw e;
            }
        }

        public async Task<Message> Request(string channel, Message msg, int timeout = 1000)
        {
            try
            {
                Msg response = await connection.RequestAsync(
                    channel,
                    System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg)),
                    timeout
                );

                string argsString = System.Text.Encoding.UTF8.GetString(response.Data);
                JObject argsJSON = JObject.Parse(argsString);

                return argsJSON.ToObject<Message>();
            }
            catch (Exception e)
            {
                this.logger(e.ToString());
                throw e;
            }
        }

        public Message RequestSync(string channel, Message msg, int timeout = 1000)
        {
            try
            {
                Msg response = connection.Request(
                    channel,
                    System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg)),
                    timeout
                );

                string argsString = System.Text.Encoding.UTF8.GetString(response.Data);
                JObject argsJSON = JObject.Parse(argsString);

                return argsJSON.ToObject<Message>();
            }
            catch (Exception e)
            {
                this.logger(e.ToString());
                throw e;
            }
        }

        #endregion

        #region subscriptions

        public void Subscribe(string channel, Action<Message> handler)
        {
            connection.SubscribeAsync(
                channel,
                (object sender, MsgHandlerEventArgs args) =>
                {
                    string argsString = System.Text.Encoding.UTF8.GetString(args.Message.Data);
                    JObject argsJSON = JObject.Parse(argsString);

                    if (args.Message.Reply != null)
                    {
                        argsJSON["Reply"] = args.Message.Reply;
                    }

                    handler(argsJSON.ToObject<Message>());
                }
            );
        }

        #endregion

        public void Dispose()
        {
            connection.Flush();
            connection.Drain();
            connection.Close();
            connection.Dispose();
        }
    }
}
