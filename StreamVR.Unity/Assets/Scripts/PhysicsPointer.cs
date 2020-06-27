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