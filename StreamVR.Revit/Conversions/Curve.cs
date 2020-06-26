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
using LMAStudio.StreamVR.Common.Models;

namespace LMAStudio.StreamVR.Revit.Conversions
{
    public class CurveConverter: IConverter<Autodesk.Revit.DB.Curve>
    {
        public JObject ConvertToDTO(Autodesk.Revit.DB.Curve source)
        {
            Autodesk.Revit.DB.XYZ endpoint0 = source.GetEndPoint(0);
            Autodesk.Revit.DB.XYZ endpoint1 = source.GetEndPoint(1);

            LMAStudio.StreamVR.Common.Models.Curve dest = new LMAStudio.StreamVR.Common.Models.Curve
            {
                Endpoint0 = new LMAStudio.StreamVR.Common.Models.XYZ
                {
                    X = endpoint0.X,
                    Y = endpoint0.Y,
                    Z = endpoint0.Z,
                },
                Endpoint1 = new LMAStudio.StreamVR.Common.Models.XYZ
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

        public Autodesk.Revit.DB.Curve CreateFromDTO(Document doc, JObject source)
        {
            throw new NotImplementedException();
        }
    }
}
