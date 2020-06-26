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
