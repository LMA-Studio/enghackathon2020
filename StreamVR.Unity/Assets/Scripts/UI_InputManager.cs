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
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class UI_InputManager : MonoBehaviour
{
    [Header("Actions")]

    public bool? leftGrip = null;
    public bool? pressed = null;

    public Vector2? buttonPosition = null;
    public XRNode inputSource;

    public bool isGripped = false;

    [Header("Scene Objects")]

    public GameObject radialMenu = null;

    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.gripButton, out isGripped);
        radialMenu.SetActive(isGripped);
        // Debug.Log($"Gripping {isGripped}");
    }

    private void OnDestroy()
    {

    }

    private void Position()
    {

    }

    private void Hover()
    {

    }

    private void pressRelease()
    {

    }

}
