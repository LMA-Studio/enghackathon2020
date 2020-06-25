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
