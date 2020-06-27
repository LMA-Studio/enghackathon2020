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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR;

namespace LMAStudio.StreamVR.Unity.Scripts
{
    public class FamilyMovementPointerController : MonoBehaviour
    {
        public GameObject physicsPointer = null;

        public XRNode inputSource;
        public float defaultLength = 3.0f;
        public float defaultLengthPlace = 7.0f;
        public float rotationSpeed = 50.0f;

        private LineRenderer lineRenderer = null;

        private GameObject selectedObject = null;

        private Transform previousOwner = null;
        private Vector3 initialPosition = Vector3.zero;
        private Quaternion initialRotation = Quaternion.identity;

        private Quaternion currentRotation = Quaternion.identity;

        private bool clicking = false;
        private bool inHand = false;

        private bool colliderFloorHit = false;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);

            if (selectedObject != null)
            {
                if (inHand)
                {
                    Vector2 stickMove;
                    device.TryGetFeatureValue(CommonUsages.primary2DAxis, out stickMove);

                    currentRotation *= Quaternion.Euler(Vector3.up * stickMove.y * rotationSpeed);

                    Vector3 collisionPoint = CalculatedEndFloor();

                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, collisionPoint);

                    selectedObject.transform.position = collisionPoint;
                    selectedObject.transform.rotation = currentRotation;

                    selectedObject.SetActive(colliderFloorHit);
                }
                else
                {
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, CalculatedEnd());
                }

                bool clicked;
                device.TryGetFeatureValue(CommonUsages.triggerButton, out clicked);

                if (clicked)
                {
                    Debug.Log("CLICKED " + clicking.ToString() + "," + inHand.ToString() + "," + colliderFloorHit.ToString());
                    if (!clicking)
                    {
                        clicking = true;

                        if (inHand && colliderFloorHit)
                        {
                            PutDown();
                        }
                        else if (!inHand)
                        {
                            PickUp();
                        }
                        else
                        {
                            Debug.Log("Clicked into space");
                            Debug.Log(this.transform.position);
                        }
                    }
                }
                else
                {
                    clicking = false;
                }
            }
            else
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, CalculatedEnd());
            }

            bool gripped;
            device.TryGetFeatureValue(CommonUsages.gripButton, out gripped);

            if (gripped)
            {
                Cancel();
            }
        }

        private void PickUp()
        {
            Debug.Log("PICKING UP");
            if (selectedObject != null && selectedObject.transform.parent != null)
            {
                previousOwner = selectedObject.transform.parent.gameObject.transform;
                initialRotation = selectedObject.transform.rotation;
                initialPosition = selectedObject.transform.position;
                currentRotation = initialRotation;
                selectedObject.transform.parent = this.transform;

                inHand = true;
            }
        }
        private void PutDown()
        {
            Debug.Log("PUTTING DOWN");

            if (selectedObject != null)
            {
                selectedObject.transform.parent = previousOwner;

                previousOwner = null;
                initialPosition = Vector3.zero;
                initialRotation = Quaternion.identity;

                FamilyController controller = selectedObject.GetComponent<FamilyController>();
                if (controller != null)
                {
                    controller.UpdatePosition();
                }
            }

            selectedObject = null;
            inHand = false;
        }
        private void Cancel()
        {
            Debug.Log("CANCELLING " + (selectedObject == null).ToString());

            if (inHand)
            {
                selectedObject.transform.position = initialPosition;
                selectedObject.transform.rotation = initialRotation;
                selectedObject.transform.parent = previousOwner;

                previousOwner = null;
                initialPosition = Vector3.zero;
                initialRotation = Quaternion.identity;
            }

            selectedObject = null;
            inHand = false;

            physicsPointer.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private Vector3 CalculatedEnd()
        {
            RaycastHit hit = CreateForwardRaycast();
            Vector3 endPosition = DefaultEnd(defaultLength);

            if (hit.collider)
            {
                Debug.Log("COLLIDING");
                selectedObject = hit.transform.gameObject;
            }
            else if (selectedObject != null)
            {
                selectedObject = null;
            }

            return endPosition;
        }

        private Vector3 CalculatedEndFloor()
        {
            RaycastHit hit = CreateForwardRaycastFloor();

            colliderFloorHit = hit.collider;

            if (hit.collider)
            {
                return hit.point;
            }

            return DefaultEnd(defaultLengthPlace);
        }

        private RaycastHit CreateForwardRaycast()
        {
            RaycastHit hit;

            Ray ray = new Ray(transform.position, transform.forward);

            Physics.Raycast(ray, out hit, defaultLength, 1 << Helpers.Constants.LAYER_FAMILY);

            return hit;
        }

        private RaycastHit CreateForwardRaycastFloor()
        {
            RaycastHit hit;

            Ray ray = new Ray(transform.position, transform.forward);

            Physics.Raycast(ray, out hit, defaultLengthPlace, 1 << 0);

            return hit;
        }


        private Vector3 DefaultEnd(float length)
        {
            return transform.position + (transform.forward * length);
        }
    }
}