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

            if (solidGeometry == null)
            {
                return JObject.FromObject(new
                {
                    ERROR = 1,
                    Msg = $"Geometry does not exist for {source.Id}"
                });
            }

            string materialId;
            List<Utility.Models.Face> wallFaces = Helpers.GeometryConversion.ConvertToDTO(solidGeometry, out materialId);

            Utility.Models.Floor dest = new Utility.Models.Floor
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                MaterialId = materialId,
                Faces = wallFaces

            };

            return JObject.FromObject(dest);
        }

        public void MapFromDTO(JObject sourceJSON, Autodesk.Revit.DB.Floor dest)
        {
        }
    }
}
