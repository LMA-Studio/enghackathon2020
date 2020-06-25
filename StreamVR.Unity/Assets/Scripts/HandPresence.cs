using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour

{
    public bool showController = false;
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public GameObject handModelPrefab;
    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;
    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
       TryIntialize(); 
    }

    void TryIntialize()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        foreach(var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        if(devices.Count > 0)
        {
            targetDevice = devices[0];

            Debug.Log("TARGET DEVICE: " + targetDevice.name);

            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if(prefab && targetDevice.name.IndexOf("left", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log("FOUND LEFT");
                spawnedController = Instantiate(controllerPrefabs[1], transform);
            }
            else if(prefab && targetDevice.name.IndexOf("right", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log("FOUND RIGHT");
                spawnedController = Instantiate(controllerPrefabs[2], transform);
            }
            else
            {
                Debug.LogError("did not find corresponding controller model");
                spawnedController= Instantiate(controllerPrefabs[0], transform);
            }

            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
        }
    }

    void UpdateHandAnimation()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!targetDevice.isValid)
        {
            TryIntialize();
        }
        
        else
        {
            if(showController)
            {
                spawnedHandModel.SetActive(false);
                spawnedController.SetActive(true);
            }

            else
            {
                spawnedHandModel.SetActive(true);
                spawnedController.SetActive(false);
                UpdateHandAnimation();
            }
        }
    }
}
