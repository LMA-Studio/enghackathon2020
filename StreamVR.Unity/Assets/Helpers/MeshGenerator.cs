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

using System.Linq;

using UnityEngine;

using LMAStudio.StreamVR.Common.Models;
using LMAStudio.StreamVR.Unity.Logic;
using LMAStudio.StreamVR.Unity.Scripts;

namespace LMAStudio.StreamVR.Unity.Helpers
{
    public class MeshGenerator
    {
        public static void ResetFaceMeshes(GeometryElement geo, GameObject parent)
        {
            foreach(UnityEngine.Transform child in parent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            foreach (Face f in geo.Faces)
            {
                GenerateFaceMesh(f, parent);
            }
        }

        public static GameObject GenerateFaceMesh(Face f, GameObject parent)
        {
            Vector3[] vertices = f.Vertices.Select(
                v => new Vector3(
                    (float)v.X * Helpers.Constants.M_PER_FT,
                    (float)v.Z * Helpers.Constants.M_PER_FT,
                    (float)v.Y * Helpers.Constants.M_PER_FT
                )
            ).ToArray();

            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }

            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = f.Indices.ToList().ToArray().Reverse().ToArray();
            msh.uv = uvs;
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            GameObject newFace = new GameObject();
            newFace.transform.position = Vector3.zero;
            newFace.transform.parent = parent.transform;
            newFace.name = $"Mesh Face";
            newFace.gameObject.AddComponent(typeof(MeshRenderer));
            ((MaterialFaceController)newFace.gameObject.AddComponent(typeof(MaterialFaceController))).LoadInstance(f);

            MeshFilter filter = newFace.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = msh;

            newFace.gameObject.AddComponent(typeof(MeshCollider));

            MeshRenderer mr = newFace.GetComponent<MeshRenderer>();
            mr.material = (UnityEngine.Material)UnityEngine.Resources.Load($"Materials/Default/Default");

            if (f.MaterialId != null && f.MaterialId != "-1")
            {
                var mat = MaterialLibrary.LookupMaterial(f.MaterialId);
                if (mat != null)
                {
                    mr.material = mat;
                    newFace.name = $"_Mesh Face";
                }
            }

            if (GeometryLibrary.GetObject(f.ElementId) == null)
            {
                GeometryLibrary.Add(f.ElementId, parent);
            }

            return newFace;
        }
    }
}
