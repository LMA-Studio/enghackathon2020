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
using UnityEngine.XR.Interaction.Toolkit;

public class UI_RadialSelection : MonoBehaviour
{
    private static bool triggered = false;

    public GameObject hoverBackground;
    public bool isTrigger = false;
    
    [SerializeField] private UnityEvent OnClick = new UnityEvent();

    private bool isEntered = false;
    public XRNode inputSource;

    // Start is called before the first frame update
    void Start()
    {
        hoverBackground.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEntered)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
            device.TryGetFeatureValue(CommonUsages.triggerButton, out isTrigger);
            if (isTrigger)
            {
                if (!triggered)
                {
                    Debug.Log($"Clicked {this.transform.parent.gameObject.name}");
                    OnClick.Invoke();
                    triggered = true;
                    RaycastExit();
                }
            }
            else if (triggered)
            {
                triggered = false;
            }
        }
    }

    public void RaycastEnter()
    {
        Debug.Log("POINTER ENTER");
        isEntered = true;
        hoverBackground.SetActive(true);
    }
    
    public void RaycastExit()
    {
        Debug.Log("POINTER EXIT");
        isEntered = false;
        hoverBackground.SetActive(false);
    }
}
