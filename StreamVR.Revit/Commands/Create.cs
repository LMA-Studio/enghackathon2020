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
    public class Create: IBaseCommand
    {
        private readonly Action<string> _log;
        private readonly IGenericConverter _converter;

        public Create(Action<string> log, IGenericConverter converter)
        {
            _log = log;
            _converter = converter;
        }

        public Message Execute(Document doc, Message msg)
        {
            _log("EXECUTE CREATE");

            JObject dto = JObject.Parse(msg.Data);

            _log("GOT DTO");
            _log(JsonConvert.SerializeObject(dto));

            _log("GETTING ELEMENT");

            JObject response;

            Autodesk.Revit.DB.FamilyInstance newFamily;

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create Element");

                response = _converter.CreateFromDTO<Autodesk.Revit.DB.FamilyInstance>(doc, dto, out newFamily);
                _log($"Created element {newFamily?.ToString() ?? "NULL"}");
                _log($" - Id {newFamily.Id?.ToString() ?? "NULL"}");
                _log($" - Fam {newFamily.Symbol?.Family?.Id?.ToString() ?? "NULL"}");
                _log($" - Origin {newFamily.GetTransform()?.Origin?.ToString() ?? "NULL"}");

                tx.Commit();
            }

            if (response["ERROR"] == null && newFamily == null)
            {
                response = new JObject();
                response["ERROR"] = 1;
                response["Msg"] = "New Family Is Null...";
            }
            if (response["ERROR"] == null)
            {
                response = _converter.ConvertToDTO(newFamily);
            }

            _log("NEW VALUE");
            _log(JsonConvert.SerializeObject(response));

            return new Message
            {
                Type = "VALUE",
                Data = JsonConvert.SerializeObject(response)
            };
        }
    }
}
