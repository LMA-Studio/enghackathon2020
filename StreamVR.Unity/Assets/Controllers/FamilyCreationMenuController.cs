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
