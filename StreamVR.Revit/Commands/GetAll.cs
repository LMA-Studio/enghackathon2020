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

using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using LMAStudio.StreamVR.Revit.Conversions;
using System;
using System.Linq;
using LMAStudio.StreamVR.Common;
using Newtonsoft.Json;

namespace LMAStudio.StreamVR.Revit.Commands
{
    public class GetAll: IBaseCommand
    {
        private readonly Action<string> _log;
        private readonly IGenericConverter _converter;

        public GetAll(Action<string> log, IGenericConverter converter)
        {
            _log = log;
            _converter = converter;
        }

        public Message Execute(Document doc, Message msg)
        {
            JObject msgData = JObject.Parse(msg.Data);
            string dataType = msgData["Type"].ToString();

            _log($"Getting data type {dataType}");

            Type t = doc.GetType().Assembly.GetType(dataType);

            if(t == null)
            {
                return new Message
                {
                    Type = "ERROR",
                    Data = JsonConvert.SerializeObject(new
                    {
                        Msg = "Type does not exist"
                    })
                };
            }

            _log($"Got data type {t?.FullName ?? "NULL"}");

            var materials = new FilteredElementCollector(doc).
                OfClass(t).
                Where(e => e != null).
                Select(e =>
                {
                    _log($"Id {e.Id}");
                    return e;
                }).
                Select(_converter.ConvertToDTO).
                ToList();

            _log($"Found {materials.Count} materials");

            var converted = JArray.FromObject(materials);

            _log($"Converted {converted.Count} materials");

            return new Message
            {
                Type = "CURRENT_STATE",
                Data = JsonConvert.SerializeObject(converted)
            };
        }
    }
}
