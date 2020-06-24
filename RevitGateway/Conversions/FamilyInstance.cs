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
    public class FamilyInstanceConverter: IConverter<Autodesk.Revit.DB.FamilyInstance>
    {
        public JObject ConvertToDTO(Autodesk.Revit.DB.FamilyInstance source)
        {
            Autodesk.Revit.DB.Family fam = source.Symbol.Family;

            BoundingBoxXYZ bb = source.get_BoundingBox(null);
            Autodesk.Revit.DB.Transform trans = source.GetTransform();

            Utility.Models.FamilyInstance dest = new Utility.Models.FamilyInstance
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                FamilyId = fam.Id.ToString(),
                BoundingBoxMin = new Utility.Models.XYZ
                {
                    X = bb.Min.X,
                    Y = bb.Min.Y,
                    Z = bb.Min.Z,
                },
                BoundingBoxMax = new Utility.Models.XYZ
                {
                    X = bb.Max.X,
                    Y = bb.Max.Y,
                    Z = bb.Max.Z,
                },
                IsFlipped = source.FacingFlipped,
                Transform = new Utility.Models.Transform
                {
                    Scale = trans.Scale,
                    Origin = new Utility.Models.XYZ
                    {
                        X = trans.Origin.X,
                        Y = trans.Origin.Y,
                        Z = trans.Origin.Z,
                    },
                    BasisX = new Utility.Models.XYZ
                    {
                        X = trans.BasisX.X,
                        Y = trans.BasisX.Y,
                        Z = trans.BasisX.Z,
                    },
                    BasisY = new Utility.Models.XYZ
                    {
                        X = trans.BasisY.X,
                        Y = trans.BasisY.Y,
                        Z = trans.BasisY.Z,
                    },
                    BasisZ = new Utility.Models.XYZ
                    {
                        X = trans.BasisZ.X,
                        Y = trans.BasisZ.Y,
                        Z = trans.BasisZ.Z,
                    },
                }
            };

            return JObject.FromObject(dest);
        }

        public void MapFromDTO(JObject sourceJSON, Autodesk.Revit.DB.FamilyInstance dest)
        {
            Autodesk.Revit.DB.Transform trans = dest.GetTransform();

            Utility.Models.FamilyInstance source = sourceJSON.ToObject<Utility.Models.FamilyInstance>();

            Autodesk.Revit.DB.XYZ newOrigin = new Autodesk.Revit.DB.XYZ(source.Transform.Origin.X, source.Transform.Origin.Y, source.Transform.Origin.Z);

            double oldAngle = Math.Atan2(trans.BasisX.Y, trans.BasisX.X);
            double newAngle = Math.Atan2(source.Transform.BasisX.Y, source.Transform.BasisX.X);

            Autodesk.Revit.DB.XYZ translation = newOrigin - trans.Origin;
            double rotation = newAngle - oldAngle;

            Autodesk.Revit.DB.XYZ a1 = trans.Origin;
            Autodesk.Revit.DB.XYZ a2 = new Autodesk.Revit.DB.XYZ(a1.X, a1.Y, a1.Z + 10);
            Line axis = Line.CreateBound(a1, a2);

            dest.Location.Rotate(axis, rotation);
            dest.Location.Move(translation);
        }
    }
}
