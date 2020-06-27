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
    public class FloorPlacer: MonoBehaviour
    {
        public void Place(List<Floor> floors)
        {
            foreach(var f in floors)
            {
                Vector3 midpoint = new Vector3(0, 0, 0);

                GameObject newWall = new GameObject();
                newWall.transform.position = midpoint;
                newWall.transform.parent = this.transform;
                newWall.name = $"Floor ({f.Id})";

                foreach(var fa in f.Faces)
                {
                    GameObject face = Helpers.MeshGenerator.GenerateFaceMesh(fa, newWall);
                    face.layer = Helpers.Constants.LAYER_FLOOR;
                }
            }
        }
    }
}
