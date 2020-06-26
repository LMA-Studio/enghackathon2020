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
    public class MaterialConverter: IConverter<Autodesk.Revit.DB.Material>
    {
        public JObject ConvertToDTO(Autodesk.Revit.DB.Material source)
        {
            LMAStudio.StreamVR.Common.Models.Material dest = new LMAStudio.StreamVR.Common.Models.Material
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                Color = new LMAStudio.StreamVR.Common.Models.Color
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
            LMAStudio.StreamVR.Common.Models.Material source = sourceJSON.ToObject<LMAStudio.StreamVR.Common.Models.Material>();

            dest.Color = new Autodesk.Revit.DB.Color(source.Color.Red, source.Color.Green, source.Color.Blue);
            dest.Transparency = source.Transparency;
            dest.Shininess = source.Shininess;
            dest.Smoothness = source.Smoothness;
        }

        public Autodesk.Revit.DB.Material CreateFromDTO(Document doc, JObject source)
        {
            throw new NotImplementedException();
        }
    }
}
