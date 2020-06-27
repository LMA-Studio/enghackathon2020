using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR;

namespace LMAStudio.StreamVR.Unity.Scripts
{
    public class FamilyCreationButtonController : MonoBehaviour
    {
        public GameObject furnatureMenu;
        public string FamilyName;

        public void ButtonClick()
        {
            furnatureMenu.GetComponent<FamilyCreationMenuController>().ButtonClick(this.FamilyName);
        }
    }
}
