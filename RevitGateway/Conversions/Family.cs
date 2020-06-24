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
    public class FamilyConverter: IConverter<Autodesk.Revit.DB.Family>
    {
        public JObject ConvertToDTO(Autodesk.Revit.DB.Family source)
        {
            Utility.Models.Family dest = new Utility.Models.Family
            {
                Id = source.Id.ToString(),
                Name = source.Name,
            };

            return JObject.FromObject(dest);
        }

        public void MapFromDTO(JObject sourceJSON, Autodesk.Revit.DB.Family dest)
        {
        }
    }
}
