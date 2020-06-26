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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using LMAStudio.StreamVR.Revit.Helpers;
using LMAStudio.StreamVR.Common.Models;

namespace LMAStudio.StreamVR.Revit.Conversions
{
    public class WallConverter: IConverter<Autodesk.Revit.DB.Wall>
    {
        public JObject ConvertToDTO(Autodesk.Revit.DB.Wall source)
        {
            Autodesk.Revit.DB.Curve curve = (source.Location as LocationCurve).Curve;
            Autodesk.Revit.DB.Line line = curve as Autodesk.Revit.DB.Line;

            if (line == null)
            {
                return null;
            }

            BoundingBoxXYZ bb = source.get_BoundingBox(null);
            Autodesk.Revit.DB.XYZ origin = line.Tessellate().First();
            Autodesk.Revit.DB.XYZ endpoint = line.Tessellate().Last(); /// origin + line.Direction * line.Length;

            List<LMAStudio.StreamVR.Common.Models.Face> wallFaces = new List<LMAStudio.StreamVR.Common.Models.Face>();

            Autodesk.Revit.DB.GeometryElement defaultGeometry = source.get_Geometry(new Options());
            if (defaultGeometry != null)
            {
                Solid solidGeometry = defaultGeometry.FirstOrDefault() as Solid;

                if (solidGeometry != null)
                {
                    wallFaces = GeometryConversion.ConvertToDTO(source, solidGeometry);
                }
            }
            
            LMAStudio.StreamVR.Common.Models.Wall dest = new LMAStudio.StreamVR.Common.Models.Wall
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                Depth = source.Width,
                Orientation = new LMAStudio.StreamVR.Common.Models.XYZ
                {
                    X = source.Orientation.X,
                    Y = source.Orientation.Y,
                    Z = source.Orientation.Z,
                },
                Endpoint0 = new LMAStudio.StreamVR.Common.Models.XYZ
                {
                    X = origin.X,
                    Y = origin.Y,
                    Z = bb.Min.Z,
                },
                Endpoint1 = new LMAStudio.StreamVR.Common.Models.XYZ
                {
                    X = endpoint.X,
                    Y = endpoint.Y,
                    Z = bb.Max.Z,
                },
                Faces = wallFaces
            };

            return JObject.FromObject(dest);
        }

        public void MapFromDTO(JObject sourceJSON, Autodesk.Revit.DB.Wall dest)
        {
            LMAStudio.StreamVR.Common.Models.Wall source = sourceJSON.ToObject<LMAStudio.StreamVR.Common.Models.Wall>();

            LocationCurve curve = (dest.Location) as LocationCurve;
            curve.Curve = Line.CreateBound(
                new Autodesk.Revit.DB.XYZ(
                    source.Endpoint0.X,
                    source.Endpoint0.Y,
                    source.Endpoint0.Z
                ),
                new Autodesk.Revit.DB.XYZ(
                    source.Endpoint1.X,
                    source.Endpoint1.Y,
                    source.Endpoint1.Z
                )
            );
        }

        public Autodesk.Revit.DB.Wall CreateFromDTO(Document doc, JObject source)
        {
            throw new NotImplementedException();
        }
    }
}
