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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LMAStudio.StreamVR.Revit.Conversions;
using System;
using LMAStudio.StreamVR.Common;

namespace LMAStudio.StreamVR.Revit.Commands
{
    public class Set: IBaseCommand
    {
        private readonly Action<string> _log;
        private readonly IGenericConverter _converter;

        public Set(Action<string> log, IGenericConverter converter)
        {
            _log = log;
            _converter = converter;
        }

        public Message Execute(Document doc, Message msg)
        {
            _log("EXECUTE SET");

            JObject dto = JObject.Parse(msg.Data);

            _log("GOT DTO");
            _log(JsonConvert.SerializeObject(dto));

            _log("GETTING ELEMENT");

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Update Element");

                Element dbValue = doc.GetElement(new ElementId(Int32.Parse(dto["Id"].ToString())));

                _log($"GOT MATERIAL {dbValue?.Id.ToString()}");

                _converter.MapFromDTO(dto, dbValue);

                _log($"MAPPED MATERIAL");

                dto = _converter.ConvertToDTO(dbValue);

                tx.Commit();
            }

            _log("NEW VALUE");
            _log(JsonConvert.SerializeObject(dto));

            return new Message
            {
                Type = "CURRENT_VALUE",
                Data = JsonConvert.SerializeObject(dto)
            };
        }
    }
}
