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
    public class CeilingConverter: IConverter<Autodesk.Revit.DB.Ceiling>
    {
        public JObject ConvertToDTO(Autodesk.Revit.DB.Ceiling source)
        {
            GeometryElement defaultGeometry = source.get_Geometry(new Options());
            Solid solidGeometry = defaultGeometry.FirstOrDefault() as Solid;

            if (solidGeometry == null)
            {
                return JObject.FromObject(new
                {
                    ERROR = 1,
                    Msg = $"Geometry does not exist for {source.Id}"
                });
            }

            IEnumerable<PlanarFace> faces = solidGeometry.Faces.Cast<PlanarFace>();
            PlanarFace topFace = faces.FirstOrDefault(
                f => Math.Round(f.FaceNormal.X, 2) == 0
                    && Math.Round(f.FaceNormal.Y, 2) == 0
                    && Math.Round(f.FaceNormal.Z, 2) == -1
            );

            if (topFace == null)
            {
                string faceNormals = String.Join(",", faces.Select(f => f.FaceNormal));
                return JObject.FromObject(new
                {
                    ERROR = 1,
                    Msg = $"Could not find face for {source.Id}: {faceNormals}"
                });
            }

            IEnumerable<Autodesk.Revit.DB.XYZ> vertexes = topFace.Triangulate().Vertices;

            if (vertexes == null)
            {
                return JObject.FromObject(new
                {
                    ERROR = 1,
                    Msg = $"Could not find vertexes for {source.Id}"
                });
            }

            Utility.Models.Ceiling dest = new Utility.Models.Ceiling
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                MaterialId = topFace.MaterialElementId?.ToString(),
                FaceLoop = vertexes.Select(v => new Utility.Models.XYZ
                {
                    X = v.X,
                    Y = v.Y,
                    Z = v.Z,
                })
            };

            return JObject.FromObject(dest);
        }

        public void MapFromDTO(JObject sourceJSON, Autodesk.Revit.DB.Ceiling dest)
        {
        }
    }
}
