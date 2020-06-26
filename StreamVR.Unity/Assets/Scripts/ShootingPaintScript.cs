using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ShootingPaintScript : MonoBehaviour
{
    public XRNode inputSource;

    public float speed = 40;

    public GameObject paintBlob;
    public Transform barrel;
    public AudioSource audioSource;
    public AudioClip audioClip;

    private bool clicking = false;

    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);

        bool clicked;
        device.TryGetFeatureValue(CommonUsages.triggerButton, out clicked);

        if (clicked)
        {
            if (!clicking)
            {
                clicking = true;
                Fire();
            }
        }
        else
        {
            clicking = false;
        }

        bool gripped;
        device.TryGetFeatureValue(CommonUsages.gripButton, out gripped);

        if (gripped)
        {
            var pointer = this.transform.parent.gameObject.GetComponentInChildren<PhysicsPointer>(true)?.gameObject;
            if (pointer != null)
            {
                this.gameObject.SetActive(false);
                pointer.SetActive(true);
            }
        }
    }

    public void ChangePaint(Material newPaint)
    {
        paintBlob.GetComponent<MeshRenderer>().material = newPaint;
    }

    public void Fire()
    {
        GameObject spawnedPaint = Instantiate(paintBlob, barrel.position, barrel.rotation);
        spawnedPaint.GetComponent<Rigidbody>().velocity = speed * barrel.up;
        audioSource.PlayOneShot(audioClip);
        Destroy(spawnedPaint, 2);
    }

}
