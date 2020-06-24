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
    public class MaterialConverter: IConverter<Autodesk.Revit.DB.Material>
    {
        public JObject ConvertToDTO(Autodesk.Revit.DB.Material source)
        {
            Utility.Models.Material dest = new Utility.Models.Material
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                Color = new Utility.Models.Color
                {
                    Red = source.Color.Red,
                    Blue = source.Color.Blue,
                    Green = source.Color.Green
                },
                Transparency = source.Transparency,
                Shininess = source.Shininess,
                Smoothness = source.Smoothness
            };

            return JObject.FromObject(dest);
        }

        public void MapFromDTO(JObject sourceJSON, Autodesk.Revit.DB.Material dest)
        {
            Utility.Models.Material source = sourceJSON.ToObject<Utility.Models.Material>();

            dest.Color = new Autodesk.Revit.DB.Color(source.Color.Red, source.Color.Green, source.Color.Blue);
            dest.Transparency = source.Transparency;
            dest.Shininess = source.Shininess;
            dest.Smoothness = source.Smoothness;
        }
    }
}
