using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR;

namespace LMAStudio.StreamVR.Unity.Scripts
{
    public class FamilyCreationMenuController : MonoBehaviour
    {
        public GameObject cataloguePointer;
        public GameObject itemPlacer;
        public GameObject physicsPointer;

        private void Update()
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            bool gripped;
            device.TryGetFeatureValue(CommonUsages.gripButton, out gripped);

            if (gripped)
            {
                cataloguePointer.SetActive(false);
                physicsPointer.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }

        public void ButtonClick(string FamilyName)
        {
            Debug.Log("FAMILY CREATION BUTTON CLICK");

            cataloguePointer.SetActive(false);
            itemPlacer.SetActive(true);
            itemPlacer.GetComponentInChildren<FamilyCreationPointerController>().SpawnFamily(FamilyName);
            this.gameObject.SetActive(false);
        }
    }
}
