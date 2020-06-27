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

using LMAStudio.StreamVR.Common.Models;
using LMAStudio.StreamVR.Unity.Logic;
using LMAStudio.StreamVR.Unity.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LMAStudio.StreamVR.Unity.Scripts
{
    public class MaterialFaceController : MonoBehaviour
    {
        private MeshRenderer selfRenderer = null;
        private StreamVRController streamAPI = null;

        private Face instanceData = null;
        private Common.Models.Material currentMaterial = null;

        public void LoadInstance(Face f)
        {
            this.streamAPI = FindObjectOfType<StreamVRController>().GetComponent<StreamVRController>();
            this.selfRenderer = this.GetComponent<MeshRenderer>();
            this.instanceData = f;
            currentMaterial = MaterialLibrary.GetMaterial(f.MaterialId);
        }

        private void Start()
        {

        }

        private void Update()
        {
            UnityEngine.Material attachedMaterial = this.GetComponent<MeshRenderer>().material;
            if (attachedMaterial != null && currentMaterial != null)
            {
                int index = attachedMaterial.name.IndexOf("(") - 1;
                string attachedMaterialName = attachedMaterial.name.Substring(0, index);
                if (attachedMaterialName != currentMaterial.Name)
                {
                    Debug.Log("instance id = " + attachedMaterialName + "," + currentMaterial.Name);
                    currentMaterial = MaterialLibrary.ReverseGetMaterial(attachedMaterialName);
                    instanceData.MaterialId = currentMaterial.Id;
                    streamAPI.PaintFace(instanceData);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "paint")
            {
                Debug.Log(collision.gameObject.name);

                this.GetComponent<MeshRenderer>().material = collision.gameObject.GetComponent<MeshRenderer>().material;
                Destroy(collision.gameObject);
            }
        }
    }
}
