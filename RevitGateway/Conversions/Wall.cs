using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using RevitGateway.Helpers;
using Utility.Models;

namespace RevitGateway.Conversions
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

            List<Utility.Models.Face> wallFaces = new List<Utility.Models.Face>();

            string materialId = null;
            GeometryElement defaultGeometry = source.get_Geometry(new Options());
            if (defaultGeometry != null)
            {
                Solid solidGeometry = defaultGeometry.FirstOrDefault() as Solid;

                if (solidGeometry != null)
                {
                    wallFaces = GeometryConversion.ConvertToDTO(solidGeometry, out materialId);
                }
            }
            
            Utility.Models.Wall dest = new Utility.Models.Wall
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                MaterialId = materialId,
                Depth = source.Width,
                Orientation = new Utility.Models.XYZ
                {
                    X = source.Orientation.X,
                    Y = source.Orientation.Y,
                    Z = source.Orientation.Z,
                },
                Endpoint0 = new Utility.Models.XYZ
                {
                    X = origin.X,
                    Y = origin.Y,
                    Z = bb.Min.Z,
                },
                Endpoint1 = new Utility.Models.XYZ
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
            Utility.Models.Wall source = sourceJSON.ToObject<Utility.Models.Wall>();

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
    }
}
