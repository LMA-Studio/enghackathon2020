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
        public string CreatedFromFamilyId;

        private StreamVRController streamAPI;

        private FamilyInstance instanceData = null;
        private Family fam = null;

        private void Start()
        {
            if (this.streamAPI == null)
            {
                this.streamAPI = FindObjectOfType<StreamVRController>().GetComponent<StreamVRController>();
            }

            if (this.CreatedFromFamilyId != null)
            {
                PlaceFamily(CreatedFromFamilyId);
            }
        }

        public void PlaceFamily(string familyId)
        {
            if (this.streamAPI == null)
            {
                this.streamAPI = FindObjectOfType<StreamVRController>().GetComponent<StreamVRController>();
            }

            this.transform.parent = this.streamAPI.gameObject.transform;

            if (FamilylLibrary.GetFamily(familyId) != null)
            {
                FamilyInstance newFam = new FamilyInstance
                {
                    FamilyId = familyId,
                    Transform = new Common.Models.Transform
                    {
                        Origin = new XYZ
                        {
                            X = this.transform.position.x * Helpers.Constants.FT_PER_M,
                            Y = this.transform.position.z * Helpers.Constants.FT_PER_M,
                            Z = this.transform.position.y * Helpers.Constants.FT_PER_M
                        }
                    }
                };
                newFam = streamAPI.PlaceFamilyInstance(newFam);
                this.LoadInstance(newFam);
            }
            else
            {
                Debug.LogError($"Can't create family from missing ID {familyId}");
            }
        }

        public void LoadInstance(FamilyInstance f)
        {
            CreatedFromFamilyId = null;

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

            // TODO: Create collider based on BB

            this.name = $"Family ({f.Id})";
            this.instanceData = f;
            this.fam = FamilylLibrary.GetFamily(f.FamilyId);

            GameObject model = (GameObject)Resources.Load($"Families/{this.fam.Name}/model");
            if (model != null)
            {
                Debug.Log("PLACING FAMILY");

                var modelInstance = Instantiate(model);
                var initialRotation = modelInstance.transform.localRotation;

                modelInstance.transform.parent = this.transform;
                modelInstance.transform.localPosition = Vector3.zero;
                modelInstance.transform.localRotation = initialRotation;

                Debug.Log("Parent " + this.gameObject.name);
                Debug.Log("Child " + modelInstance.gameObject.name);

                var childXR = modelInstance.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
                if (childXR != null)
                {
                    Debug.Log("HAS XR");
                    GameObject.Destroy(childXR);
                }


                BoxCollider childBox = modelInstance.GetComponent<BoxCollider>();
                if (childBox != null)
                {
                    Debug.Log("HAS BOX");
                    BoxCollider parentBox = this.gameObject.AddComponent<BoxCollider>();
                    parentBox.size = childBox.size;
                    parentBox.center = childBox.center;

                    modelInstance.layer = 0;
                    this.gameObject.layer = Helpers.Constants.LAYER_FAMILY;

                    GameObject.Destroy(childBox);
                }

                Rigidbody childRB = modelInstance.GetComponent<Rigidbody>();
                if (childRB != null)
                {
                    Debug.Log("HAS RB");
                    Rigidbody parentRB = this.gameObject.AddComponent<Rigidbody>();
                    parentRB.useGravity = false;
                    parentRB.constraints = RigidbodyConstraints.FreezeAll;

                    GameObject.Destroy(childRB);
                }

                int count = 0;
                foreach (UnityEngine.Transform child in transform)
                {
                    count++;
                    if (child.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        Debug.Log("HAS RB " + count);
                    }

                }
                Debug.Log("Children " + count);

                this.name = $"_ Family ({f.Id})";
            }
        }

        private Vector3? currentPostion = null;
        private Quaternion? currentRotation = null;

        //private bool hasUpdate = false;
        //private DateTime firstUpdate = DateTime.UtcNow;
        //private DateTime lastUpdate = DateTime.UtcNow;
        //private double DebounceTime = 500;

        void Update()
        {
            if (currentPostion == null || currentRotation == null)
            {
                currentPostion = this.transform.position;
                currentRotation = this.transform.rotation;
            }
        }

        public void UpdatePosition()
        {
            Debug.Log("Updating position");
            if (this.instanceData != null)
            {
                Debug.Log("Registering Update");

                currentPostion = this.transform.position;
                currentRotation = this.transform.rotation;

                SaveSelf();
                Debug.Log("SAVED");

                //if (hasUpdate)
                //{
                //    if ((lastUpdate - firstUpdate).TotalMilliseconds > DebounceTime)
                //    {
                //        SaveSelf();
                //    }
                //}

                //if (hasUpdate)
                //{
                //    if ((DateTime.UtcNow - lastUpdate).TotalMilliseconds > DebounceTime)
                //    {
                //        SaveSelf();
                //    }
                //}
            }
        }

        //private void RegisterUpdate()
        //{
        //    if (currentPostion != this.transform.position)
        //    {
        //        if (currentPostion != null)
        //        {
        //            if (!hasUpdate)
        //            {
        //                firstUpdate = DateTime.UtcNow;
        //            }
        //            hasUpdate = true;
        //            lastUpdate = DateTime.UtcNow;
        //        }

        //        currentPostion = this.transform.position;
        //        currentRotation = this.transform.rotation;
        //    }

        //    if (currentRotation != this.transform.rotation)
        //    {
        //        if (currentRotation != null)
        //        {
        //            if (!hasUpdate)
        //            {
        //                firstUpdate = DateTime.UtcNow;
        //            }
        //            hasUpdate = true;
        //            lastUpdate = DateTime.UtcNow;
        //        }

        //        currentPostion = this.transform.position;
        //        currentRotation = this.transform.rotation;
        //    }
        //}

        private void SaveSelf()
        {
            if (this.streamAPI == null)
            {
                this.streamAPI = FindObjectOfType<StreamVRController>().GetComponent<StreamVRController>();
            }

            this.instanceData.Transform.Origin = new XYZ
            {
                X = this.transform.position.x * Helpers.Constants.FT_PER_M,
                Y = this.transform.position.z * Helpers.Constants.FT_PER_M,
                Z = this.transform.position.y * Helpers.Constants.FT_PER_M,
            };

            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(this.transform.rotation);
            this.instanceData.Transform.SetRotation(rotationMatrix, this.instanceData.IsFlipped);

            this.streamAPI.SaveFamilyInstance(this.instanceData);
            // hasUpdate = false;
        }
    }
}
