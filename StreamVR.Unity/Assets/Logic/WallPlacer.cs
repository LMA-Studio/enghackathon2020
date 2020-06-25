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

using System.Collections.Generic;

using UnityEngine;

using LMAStudio.StreamVR.Common.Models;

namespace LMAStudio.StreamVR.Unity.Logic
{
    public class WallPlacer: MonoBehaviour
    {
        public void Place(List<Wall> walls)
        {
            foreach(var w in walls)
            {
                XYZ startXYZ = w.Endpoint0;
                XYZ endXYZ = w.Endpoint1;

                Vector3 start = new Vector3((float)startXYZ.X * Helpers.Constants.M_PER_FT, (float)startXYZ.Z * Helpers.Constants.M_PER_FT, (float)startXYZ.Y * Helpers.Constants.M_PER_FT);
                Vector3 end = new Vector3((float)endXYZ.X * Helpers.Constants.M_PER_FT, (float)endXYZ.Z * Helpers.Constants.M_PER_FT, (float)endXYZ.Y * Helpers.Constants.M_PER_FT);

                float height = (end.y - start.y);
                Vector3 midpoint = new Vector3(
                    (end.x + start.x) / 2,
                    start.y + (height / 2),
                    (end.z + start.z) / 2
                );

                GameObject newWall = new GameObject();
                newWall.transform.position = midpoint;
                newWall.transform.parent = this.transform;
                newWall.name = $"Wall ({w.Id})";

                foreach(var f in w.Faces)
                {
                    Helpers.MeshGenerator.GenerateFaceMesh(f, newWall);
                }
            }
        }
    }
}
