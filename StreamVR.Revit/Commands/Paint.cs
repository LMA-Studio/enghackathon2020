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
using System.Linq;
using System.Collections.Generic;
using LMAStudio.StreamVR.Revit.Helpers;

namespace LMAStudio.StreamVR.Revit.Commands
{
    public class Paint: IBaseCommand
    {
        private readonly Action<string> _log;
        private readonly IGenericConverter _converter;

        public Paint(Action<string> log, IGenericConverter converter)
        {
            _log = log;
            _converter = converter;
        }

        public Message Execute(Document doc, Message msg)
        {
            _log("EXECUTE PAINT");

            JObject dto = JObject.Parse(msg.Data);
            Common.Models.Face face = dto.ToObject<Common.Models.Face>();

            _log("GOT DTO");
            _log(JsonConvert.SerializeObject(face));

            face = UpdateFace(doc, face);

            _log("NEW VALUE");
            _log(JsonConvert.SerializeObject(face));

            return new Message
            {
                Type = "VALUE",
                Data = JsonConvert.SerializeObject(face)
            };

        }

        private Common.Models.Face UpdateFace(Document doc, Common.Models.Face face)
        {
            _log("GETTING ELEMENT");

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Paint Element");

                ElementId materialId = new ElementId(Int32.Parse(face.MaterialId));
                ElementId parentId = new ElementId(Int32.Parse(face.ElementId));
                Element parentElement = doc.GetElement(parentId);

                GeometryElement defaultGeometry = parentElement.get_Geometry(new Options());
                Solid solidGeometry = defaultGeometry.FirstOrDefault() as Solid;
                IEnumerable<PlanarFace> faces = solidGeometry.Faces.Cast<PlanarFace>();

                if (solidGeometry == null)
                {
                    throw new Exception($"Geometry does not exist for {face.ElementId}");
                }

                PlanarFace dbValue = faces.ElementAt(face.FaceIndex);

                if (dbValue == null)
                {
                    throw new Exception($"Could not find face at index {face.FaceIndex}");
                }

                doc.Paint(parentId, dbValue, materialId);

                _log($"MAPPED MATERIAL");

                face = GeometryConversion.ConvertToDTO(parentElement, solidGeometry).ElementAt(face.FaceIndex);

                tx.Commit();
            }

            return face;
        }
    }
}
