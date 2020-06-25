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

using UnityEngine;

using LMAStudio.StreamVR.Common.Models;
using LMAStudio.StreamVR.Unity.Logic;
using LMAStudio.StreamVR.Unity.Extensions;

namespace LMAStudio.StreamVR.Unity.Scripts
{
    public class FamilyController : MonoBehaviour
    {
        private StreamVRController streamAPI;

        private FamilyInstance instanceData = null;
        private Family fam = null;

        public void LoadInstance(FamilyInstance f)
        {
            Matrix4x4 rotM = f.Transform.GetRotation();
            Matrix4x4 rotMI = rotM.inverse;

            Vector3 bbMin = new Vector3((float)f.BoundingBoxMin.X, (float)f.BoundingBoxMin.Y, (float)f.BoundingBoxMin.Z);
            Vector3 bbMax = new Vector3((float)f.BoundingBoxMax.X, (float)f.BoundingBoxMax.Y, (float)f.BoundingBoxMax.Z);

            Vector3 bbMinRot = rotMI.MultiplyPoint(bbMin);
            Vector3 bbMaxRot = rotMI.MultiplyPoint(bbMax);

            //this.transform.localScale = new Vector3(
            //    bbMax.x - bbMin.x,
            //    bbMax.y - bbMin.y,
            //    bbMax.z - bbMin.z
            //);
            this.name = $"Family ({f.Id})";

            this.streamAPI = FindObjectOfType<StreamVRController>().GetComponent<StreamVRController>();
            this.instanceData = f;
            this.fam = FamilylLibrary.GetFamily(f.FamilyId);

            GameObject model = (GameObject)Resources.Load($"Families/{this.fam.Name}/model");
            if (model != null)
            {
                var modelInstance = Instantiate(model);
                var initialRotation = modelInstance.transform.localRotation;

                modelInstance.transform.parent = this.transform;
                modelInstance.transform.localPosition = Vector3.zero;
                modelInstance.transform.localRotation = initialRotation;

                this.name = $"_ Family ({f.Id})";
            }
        }

        private Vector3? currentPostion = null;
        private Quaternion? currentRotation = null;

        private bool hasUpdate = false;
        private DateTime firstUpdate = DateTime.UtcNow;
        private DateTime lastUpdate = DateTime.UtcNow;
        private double DebounceTime = 500;

        void Update()
        {
            if (this.instanceData != null)
            {
                RegisterUpdate();

                if (hasUpdate)
                {
                    if ((lastUpdate - firstUpdate).TotalMilliseconds > DebounceTime)
                    {
                        SaveSelf();
                    }
                }

                if (hasUpdate)
                {
                    if ((DateTime.UtcNow - lastUpdate).TotalMilliseconds > DebounceTime)
                    {
                        SaveSelf();
                    }
                }
            }
        }

        private void RegisterUpdate()
        {
            if (currentPostion != this.transform.position)
            {
                if (currentPostion != null)
                {
                    if (!hasUpdate)
                    {
                        firstUpdate = DateTime.UtcNow;
                    }
                    hasUpdate = true;
                    lastUpdate = DateTime.UtcNow;
                }

                currentPostion = this.transform.position;
                currentRotation = this.transform.rotation;
            }

            if (currentRotation != this.transform.rotation)
            {
                if (currentRotation != null)
                {
                    if (!hasUpdate)
                    {
                        firstUpdate = DateTime.UtcNow;
                    }
                    hasUpdate = true;
                    lastUpdate = DateTime.UtcNow;
                }

                currentPostion = this.transform.position;
                currentRotation = this.transform.rotation;
            }
        }

        private void SaveSelf()
        {
            this.instanceData.Transform.Origin = new XYZ
            {
                X = this.transform.position.x * Helpers.Constants.FT_PER_M,
                Y = this.transform.position.z * Helpers.Constants.FT_PER_M,
                Z = this.transform.position.y * Helpers.Constants.FT_PER_M,
            };

            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(this.transform.rotation);
            this.instanceData.Transform.SetRotation(rotationMatrix, this.instanceData.IsFlipped);

            this.streamAPI.SaveFamilyInstance(this.instanceData);
            hasUpdate = false;
        }
    }
}
