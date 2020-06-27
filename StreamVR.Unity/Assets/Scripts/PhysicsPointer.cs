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

namespace LMAStudio.StreamVR.Unity.Scripts
{
    public class PhysicsPointer : MonoBehaviour
    {
        public float defaultLength = 3.0f;

        private LineRenderer lineRenderer = null;

        private GameObject colliderHit = null;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            UpdateLength();
        }

        private void UpdateLength()
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, CalculatedEnd());
        }

        private Vector3 CalculatedEnd()
        {
            RaycastHit hit = CreateForwardRaycast();
            Vector3 endPosition = DefaultEnd(defaultLength);

            if (hit.collider)
            {
                endPosition = hit.point;
                Debug.Log($"HIT OBJECT {hit.transform.parent.gameObject.name}");

                if (colliderHit != null)
                {
                    colliderHit.GetComponent<UI_RadialSelection>().RaycastExit();
                }

                colliderHit = hit.transform.gameObject;
                colliderHit.GetComponent<UI_RadialSelection>().RaycastEnter();
            }
            else if (colliderHit != null)
            {
                colliderHit.GetComponent<UI_RadialSelection>().RaycastExit();
                colliderHit = null;
            }

            return endPosition;
        }

        private RaycastHit CreateForwardRaycast()
        {
            RaycastHit hit;

            Ray ray = new Ray(transform.position, transform.forward);

            Physics.Raycast(ray, out hit, defaultLength, 1 << 5);

            return hit;
        }

        private Vector3 DefaultEnd(float length)
        {
            return transform.position + (transform.forward * length);
        }
    }
}