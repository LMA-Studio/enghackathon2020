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
    public class CurveConverter: IConverter<Autodesk.Revit.DB.Curve>
    {
        public JObject ConvertToDTO(Autodesk.Revit.DB.Curve source)
        {
            Autodesk.Revit.DB.XYZ endpoint0 = source.GetEndPoint(0);
            Autodesk.Revit.DB.XYZ endpoint1 = source.GetEndPoint(1);

            Utility.Models.Curve dest = new Utility.Models.Curve
            {
                Endpoint0 = new Utility.Models.XYZ
                {
                    X = endpoint0.X,
                    Y = endpoint0.Y,
                    Z = endpoint0.Z,
                },
                Endpoint1 = new Utility.Models.XYZ
                {
                    X = endpoint1.X,
                    Y = endpoint1.Y,
                    Z = endpoint1.Z,
                }
            };

            return JObject.FromObject(dest);
        }

        public void MapFromDTO(JObject sourceJSON, Autodesk.Revit.DB.Curve dest)
        {

        }
    }
}
