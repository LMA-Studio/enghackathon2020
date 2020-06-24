using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using Utility.Models;

namespace RevitGateway.Conversions
{
    public class FloorConverter: IConverter<Autodesk.Revit.DB.Floor>
    {
        public JObject ConvertToDTO(Autodesk.Revit.DB.Floor source)
        {
            GeometryElement defaultGeometry = source.get_Geometry(new Options());
            Solid solidGeometry = defaultGeometry.FirstOrDefault() as Solid;

            IEnumerable<PlanarFace> faces = solidGeometry.Faces.Cast<PlanarFace>();
            PlanarFace topFace = faces.FirstOrDefault(
                f => f.FaceNormal.X == 0 && f.FaceNormal.Y == 0 && f.FaceNormal.Z == 1
            );
            IEnumerable<Autodesk.Revit.DB.XYZ> vertexes = topFace.Triangulate().Vertices;

            Utility.Models.Floor dest = new Utility.Models.Floor
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                MaterialId = topFace.MaterialElementId.ToString(),
                FaceLoop = vertexes.Select(v => new Utility.Models.XYZ
                {
                    X = v.X,
                    Y = v.Y,
                    Z = v.Z,
                })
            };

            return JObject.FromObject(dest);
        }

        public void MapFromDTO(JObject sourceJSON, Autodesk.Revit.DB.Floor dest)
        {
        }
    }
}
