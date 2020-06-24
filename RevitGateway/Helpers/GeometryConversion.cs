using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace RevitGateway.Helpers
{
    public class GeometryConversion
    {
        public static List<Utility.Models.Face> ConvertToDTO(Autodesk.Revit.DB.Solid geometry, out string materialId)
        {
            List<Utility.Models.Face> wallFaces = new List<Utility.Models.Face>();

            IEnumerable<PlanarFace> faces = geometry.Faces.Cast<PlanarFace>();
            PlanarFace topFace = faces.Where(
                (f, i) => i == 1
            ).FirstOrDefault() ?? faces.FirstOrDefault();

            materialId = topFace?.MaterialElementId?.ToString();

            wallFaces = faces.Select(f =>
            {
                Mesh m = f.Triangulate();

                List<int> indices = new List<int>();

                for (int i = 0; i < m.NumTriangles; i++)
                {
                    MeshTriangle mt = m.get_Triangle(i);
                    for (int j = 0; j < 3; j++)
                    {
                        indices.Add((int)mt.get_Index(j));
                    }
                }

                return new Utility.Models.Face
                {
                    MaterialId = f.MaterialElementId?.ToString(),
                    Origin = new Utility.Models.XYZ
                    {
                        X = f.Origin.X,
                        Y = f.Origin.Y,
                        Z = f.Origin.Z,
                    },
                    XVector = new Utility.Models.XYZ
                    {
                        X = f.XVector.X,
                        Y = f.XVector.Y,
                        Z = f.XVector.Z,
                    },
                    YVector = new Utility.Models.XYZ
                    {
                        X = f.YVector.X,
                        Y = f.YVector.Y,
                        Z = f.YVector.Z,
                    },
                    Normal = new Utility.Models.XYZ
                    {
                        X = f.FaceNormal.X,
                        Y = f.FaceNormal.Y,
                        Z = f.FaceNormal.Z,
                    },
                    Indices = indices,
                    Vertices = m.Vertices.Select(v => new Utility.Models.XYZ
                    {
                        X = v.X,
                        Y = v.Y,
                        Z = v.Z,
                    }).ToList()
                };
            }).ToList();

            return wallFaces;
        }
    }
}
